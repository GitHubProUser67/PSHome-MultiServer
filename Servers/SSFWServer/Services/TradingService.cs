using CustomLogger;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SSFWServer.Services
{
    public class TradingService
    {
        private readonly string? sessionid;
        private readonly string? env;
        private readonly string? key;

        public TradingService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }

        public class BaseResponse
        {
            [JsonProperty(nameof(result))]
            public int result { get; set; } = -1;
            [JsonProperty(nameof(id))]
            public int id { get; set; } = -1;
            [JsonProperty(nameof(message))]
            public string message { get; set; } = string.Empty;
        }

        public class RootObject
        {
            [JsonProperty(nameof(members))]
            public List<string>? members { get; set; }
        }

        private static List<TradeTransactionResponse> tradeTransactions = new();
        public class TradeTransactionResponse : BaseResponse
        {
            [JsonProperty(nameof(ownerId))]
            public string ownerId { get; set; } = string.Empty;
            [JsonProperty(nameof(joinerId))]
            public string joinerId { get; set; } = string.Empty;
            [JsonProperty(nameof(transactionId))]
            public int transactionId { get; set; } = 0;
            [JsonProperty(nameof(sequence))]
            public long sequence { get; set; } = 0;

            public int tradeAmount { get; set; } = 0;
            //itemList is a dictionary list of <itemGuid, numOfCards> pairs.
            public Dictionary<string, int> tradeRequesterItemList { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> tradePartnerItemList { get; set; } = new Dictionary<string, int>();

            public Status status {  get; set; }
        }

        public enum Status : int
        {
            Active = 0,
            Commited = 1,
            PartiallyCommited = 2,
            Cancelled = 3
        }


        public string HandleTradingService(HttpRequest req, string sessionid, string absolutepath)
        {
            BaseResponse tradeResponse = new();
            TradeTransactionResponse newTradeTransactionResponse = new();

            int existingCardTradingTransactionId = 0;
            int sequenceNum = 0;
            var absoPathArray = absolutepath.Split("/");

            string? currentUserId = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            if (string.IsNullOrEmpty(currentUserId))
                return JsonConvert.SerializeObject(tradeResponse);
#if DEBUG
            LoggerAccessor.LogInfo(absoPathArray.Length);
#endif
            //if this is a existing Trade Transaction, assign!
            if (absoPathArray.Length > 3)
            {
                existingCardTradingTransactionId = Convert.ToInt32(absolutepath.Split("/")[3]);
            }

            //CommitTrade sends SequenceNumber
            if (absoPathArray.Length > 4)
            {
                sequenceNum = Convert.ToInt32(absolutepath.Split("/")[4]);
            }

            if (req.Method == "POST")
            {
                //If we DO have a existing trade transaction in the process, handle it!
                if (existingCardTradingTransactionId > 0) {

                    foreach (var transaction in tradeTransactions)
                    {
                        //ADDTRADEITEMS 
                        //If a existing transaction was created, update it here!
                        if (transaction.transactionId == existingCardTradingTransactionId)
                        {
                            transaction.transactionId = existingCardTradingTransactionId;
                            transaction.sequence = sequenceNum; //Set to 0 initially till set later

                            try
                            {
                                // Deserialize directly into Dictionary<string, int> using Newtonsoft.Json
                                var reqitemList = JsonConvert.DeserializeObject<Dictionary<string, int>>(req.Body);

                                if (reqitemList == null || reqitemList.Count == 0)
                                {
                                    LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transactionId} failed to update, request contained no Items to add!");
                                    return JsonConvert.SerializeObject(tradeResponse);
                                }

                                if(transaction.joinerId == currentUserId)
                                {
                                    transaction.tradePartnerItemList = reqitemList;
                                } else //transaction.tradeRequester == curentUserId
                                {
                                    transaction.tradeRequesterItemList = reqitemList;
                                }

                                LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transactionId} has been updated");
                                tradeTransactions.Add(transaction);

                                tradeResponse.result = 0;
                                return JsonConvert.SerializeObject(tradeResponse);
                            } catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[SSFW] TradingService - Exception caught attempting to remove existing trade transaction id {existingCardTradingTransactionId} with error {ex}");

                                tradeResponse.result = -1;
                                return JsonConvert.SerializeObject(tradeResponse);
                            }

                        }
                    }

                } else // otherwise create new transaction!
                {   
                    RootObject? result = JsonConvert.DeserializeObject<RootObject>(req.Body);
                    string memberValue = string.Empty;
                    if (result != null && result.members != null)
                    {
                        memberValue = result.members[0];
                    } else  {
                        return JsonConvert.SerializeObject(tradeResponse);
                    }

                    int index = 1;
                    foreach (var transaction in tradeTransactions)
                    {
                        newTradeTransactionResponse.ownerId = currentUserId;
                        newTradeTransactionResponse.joinerId = memberValue;

                        //If a existing transaction was created, update it here!
                        if (transaction.transactionId == existingCardTradingTransactionId)
                        {
                            //index = transaction.transId + 1;

                            //newTradeRequest.transId = index;
                            //tradeTransactions.Add(newTradeRequest);
                        }
                        //Initial first Transaction starts at index 1
                        else if (tradeTransactions.Count == 0)
                        {
                            newTradeTransactionResponse.transactionId = index;
                            newTradeTransactionResponse.status = Status.Active;
                            tradeTransactions.Add(newTradeTransactionResponse);

                        }
                    }

                    tradeResponse.result = 0;
                    tradeResponse.id = index;
                    return JsonConvert.SerializeObject(tradeResponse);
                }
            }
            else if (req.Method == "GET")
            {
                #region Request Trade Status - Potentially unused
                //Return current status of transaction
                if (req.Url.Contains("status"))
                {
                    var existingTrade = tradeTransactions.FirstOrDefault(x => x.transactionId == existingCardTradingTransactionId);

                    if (existingTrade != null)
                    {
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - Checking current status of transactionId {existingTrade.transactionId} between Requester {existingTrade.ownerId} & Partner {existingTrade.joinerId}: {existingTrade.status}");

                        //RootObject? result = JsonConvert.DeserializeObject<RootObject>(CreateTransactionBody);

                        JObject jsonResponse = JObject.FromObject(existingTrade);

                        jsonResponse[newTradeTransactionResponse.result] = 0;
                        jsonResponse[newTradeTransactionResponse.message] = "Success";
                        jsonResponse[newTradeTransactionResponse.sequence] = existingTrade.sequence;
                        jsonResponse[newTradeTransactionResponse.ownerId] = existingTrade.ownerId;
                        jsonResponse[newTradeTransactionResponse.joinerId] = existingTrade.joinerId;
                        jsonResponse[newTradeTransactionResponse.status] = Convert.ToInt32(existingTrade.status);

                        return jsonResponse.ToString(Formatting.Indented);

                        /* Original
                        return $@" 	{{
     	""result"" : 0,
        ""message"" : ""Success"",
        ""sequence"" : {existingTrade.sequence},
        ""ownerId"" :""{existingTrade.tradeRequester}"",
        ""joinerId"" :""{existingTrade.joinerId}""
        ""status"" : {existingTrade.status}""
     }}";*/

                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - GET method failed to find existing trade transaction!");
                        return JsonConvert.SerializeObject(tradeResponse);
                    }
                } else
                {

                    if(existingCardTradingTransactionId > 0)
                    {
                        foreach (var transaction in tradeTransactions)
                        {
                            //ADDTRADEITEMS 
                            //If a existing transaction was created, update it here!
                            if (transaction.transactionId == existingCardTradingTransactionId)
                            {
                                return $@"{{
  ""result"": 0,
  ""sequence"": {transaction.sequence},
  ""{transaction.ownerId}"": {{
{string.Join("", transaction.tradeRequesterItemList)}
  }},
  ""{transaction.joinerId}"": {{
{string.Join("", transaction.tradePartnerItemList)}
  }}
}}";
                            }
                        }

                    }

                }
                #endregion
            }
            #region Cancel Trade Transaction
            else if (req.Method == "DELETE")
            {
                if (tradeTransactions.Count > 0)
                {
                    try
                    {
                        TradeTransactionResponse? tradeRemovalResp = tradeTransactions.FirstOrDefault(x => x.transactionId == existingCardTradingTransactionId) ?? null;
                        if (tradeRemovalResp != null)
                        {
                            tradeTransactions.Remove(tradeRemovalResp);
                            tradeResponse.result = 0;

                            LoggerAccessor.LogError($"[SSFW] TradingService - Successfully cancelled existing trade transaction id {existingCardTradingTransactionId}");
                            return JsonConvert.SerializeObject(tradeResponse);
                        }
                        else
                        {
                            LoggerAccessor.LogError($"[SSFW] TradingService - Unable to determine existing trade transaction to delete!");
                        }

                    }
                    catch (Exception e) { 
                        LoggerAccessor.LogError($"[SSFW] TradingService - Exception caught attempting to remove existing trade transaction id {existingCardTradingTransactionId} with error {e}");
                        return JsonConvert.SerializeObject(tradeResponse);
                    }

                }

            }
            #endregion

            tradeResponse.result = 0;
            return JsonConvert.SerializeObject(tradeResponse);
        }
    }
}