using CustomLogger;
using NetCoreServer;
using Newtonsoft.Json;
using System;

namespace SSFWServer.Services
{
    public class TradingService
    {
        private string? sessionid;
        private string? env;
        private string? key;

        public TradingService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }


        public class TradeResponse
        {
            public int result = -1;
            public int id = -1;
            public string message = string.Empty;
            
        }

        //<itemGuid, numOfCards>
        public class RootObject
        {
            public List<string>? members { get; set; }
        }

        private static List<TradeTransaction> tradeTransactions = new();
        public class TradeTransaction : TradeResponse
        {
            public string ownerId = string.Empty;
            public string joinerId = string.Empty;
            public int transId = 0;
            public int sequence = 0;

            public int tradeAmount = 0;
            //itemList is a dictionary list of <itemGuid, numOfCards> pairs.
            public Dictionary<string, int> tradeRequesterItemList = new Dictionary<string, int>();
            public Dictionary<string, int> tradePartnerItemList = new Dictionary<string, int>();

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
            TradeTransaction newTradeRequest = new TradeTransaction();
            TradeResponse tradeResponse = new();

            int existingCardTradingTransactionId = 0;
            int sequenceNum = 0;
            var absoPathArray = absolutepath.Split("/");

            string? currentUserId = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            if (string.IsNullOrEmpty(currentUserId))
                return JsonConvert.SerializeObject(tradeResponse);
#if DEBUG
            LoggerAccessor.LogInfo(absoPathArray.Count());
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
                        if (transaction.transId == existingCardTradingTransactionId)
                        {
                            transaction.transId = existingCardTradingTransactionId;
                            transaction.sequence = sequenceNum; //Set to 0 initially till set later

                            try
                            {
                                // Deserialize directly into Dictionary<string, int> using Newtonsoft.Json
                                var reqitemList = JsonConvert.DeserializeObject<Dictionary<string, int>>(req.Body);

                                if (reqitemList == null || reqitemList.Count == 0)
                                {
                                    LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transId} failed to update, request contained no Items to add!");
                                    return JsonConvert.SerializeObject(tradeResponse);
                                }

                                if(transaction.joinerId == currentUserId)
                                {
                                    transaction.tradePartnerItemList = reqitemList;
                                } else //transaction.tradeRequester == curentUserId
                                {
                                    transaction.tradeRequesterItemList = reqitemList;
                                }

                                LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transId} has been updated");
                                tradeTransactions.Add(transaction);

                                tradeResponse.result = 0;
                                return JsonConvert.SerializeObject(tradeResponse);
                            } catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[SSFW] TradingService - Exception caught attempting to remove existing trade transaction id {existingCardTradingTransactionId} with error {ex}");
                                return JsonConvert.SerializeObject(tradeResponse);

                            }


                        }
                    }

                } else // otherwise create new transaction!
                {

                    string CreateTransactionBody = req.Body;
                    RootObject? result = JsonConvert.DeserializeObject<RootObject>(CreateTransactionBody);
                    string memberValue = result.members[0];

                    int index = 1;
                    foreach (var transaction in tradeTransactions)
                    {
                        newTradeRequest.ownerId = currentUserId;
                        newTradeRequest.joinerId = memberValue;

                        //If a existing transaction was created, update it here!
                        if (transaction.transId == existingCardTradingTransactionId)
                        {
                            //index = transaction.transId + 1;

                            //newTradeRequest.transId = index;
                            //tradeTransactions.Add(newTradeRequest);
                        }
                        //Initial first Transaction starts at index 1
                        else if (tradeTransactions.Count() == 0)
                        {
                            newTradeRequest.transId = index;
                            newTradeRequest.status = Status.Active;
                            tradeTransactions.Add(newTradeRequest);

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
                    var existingTrade = tradeTransactions.FirstOrDefault(x => x.transId == existingCardTradingTransactionId);

                    if (existingTrade != null)
                    {
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - Checking current status of transactionId {existingTrade.transId} between Requester {existingTrade.ownerId} & Partner {existingTrade.joinerId}: {existingTrade.status}");

                        //RootObject? result = JsonConvert.DeserializeObject<RootObject>(CreateTransactionBody);

                        newTradeRequest.result = 0;
                        newTradeRequest.message = "Success";
                        newTradeRequest.status = existingTrade.status;
                        newTradeRequest.sequence = existingTrade.sequence;
                        newTradeRequest.joinerId = existingTrade.joinerId;

                        return JsonConvert.SerializeObject(newTradeRequest);

                        /*
                        return $@" 	{{
     	""result"" : 0,
        ""message"" : ""Success"",
        ""sequence"" : {existingTrade.seqNumb},
        ""ownerId"" :""{existingTrade.tradeRequester}"",
        ""joinerId"" :""{existingTrade.tradePartner}""
        ""status"" : {existingTrade.status}""
     }}";*/

                    } else
                    {
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - Checking current status of transactionId {existingTrade.transId} between Requester {existingTrade.ownerId} & Partner {existingTrade.joinerId}: {existingTrade.status}");
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
                            if (transaction.transId == existingCardTradingTransactionId)
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
                if (tradeTransactions.Count() > 0)
                {
                    try
                    {
                        TradeTransaction? tradeRemovalRequest = tradeTransactions.FirstOrDefault(x => x.transId == existingCardTradingTransactionId) ?? null;
                        tradeTransactions.Remove(tradeRemovalRequest);
                        LoggerAccessor.LogError($"[SSFW] TradingService - Successfully cancelled existing trade transaction id {existingCardTradingTransactionId}");

                        tradeResponse.result = 0;
                        return JsonConvert.SerializeObject(tradeResponse);
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