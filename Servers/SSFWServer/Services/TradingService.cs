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

        private static List<TradeTransaction> tradeTransactions = new();

        //<itemGuid, numOfCards>
        public class RootObject
        {
            public List<string>? members { get; set; }
        }

        public class TradeTransaction
        {
            public string tradeRequester = string.Empty;
            public string tradePartner = string.Empty;
            public int transId = 0;
            public int seqNumb = 0;

            public int tradeAmount = 0;
            //itemList is a dictionary list of <itemGuid, numOfCards> pairs.
            public Dictionary<string, int> tradeRequesterItemList = new Dictionary<string, int>();
            public Dictionary<string, int> tradePartnerItemList = new Dictionary<string, int>();

            public Status status {  get; set; }
            //cards to trade etc
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

            int existingCardTradingTransactionId = 0;
            int sequenceNum = 0;
            var absoPathArray = absolutepath.Split("/");

            string? currentUserId = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            if (string.IsNullOrEmpty(currentUserId))
                return $" {{ \"result\": -1, \"id\": -1 }} ";

            LoggerAccessor.LogInfo(absoPathArray.Count());

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
                            transaction.seqNumb = sequenceNum; //Set to 0 initially till set later

                            try
                            {
                                // Deserialize directly into Dictionary<string, int> using Newtonsoft.Json
                                var reqitemList = JsonConvert.DeserializeObject<Dictionary<string, int>>(req.Body);

                                if (reqitemList == null || reqitemList.Count == 0)
                                {
                                    LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transId} failed to update, request contained no Items to add!");
                                    return $" {{ \"result\": -1, \"id\": -1 }} ";
                                }

                                if(transaction.tradePartner == currentUserId)
                                {
                                    transaction.tradePartnerItemList = reqitemList;
                                } else //transaction.tradeRequester == curentUserId
                                {
                                    transaction.tradeRequesterItemList = reqitemList;
                                }

                                LoggerAccessor.LogInfo($"[SSFW] TradingService - Existing transaction {transaction.transId} has been updated");
                                tradeTransactions.Add(transaction);
                                return $" {{ \"result\": 0, \"id\": -1 }} ";
                            } catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[SSFW] TradingService - Exception caught attempting to remove existing trade transaction id {existingCardTradingTransactionId} with error {ex}");

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
                        newTradeRequest.tradeRequester = currentUserId;
                        newTradeRequest.tradePartner = memberValue;

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
                    return $"{{ \"result\": 0, \"id\": {index} }}";

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
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - Checking current status of transactionId {existingTrade.transId} between Requester {existingTrade.tradeRequester} & Partner {existingTrade.tradePartner}: {existingTrade.status}");

                        return $@" 	{{
     	""result"" : 0,
        ""message"" : ""Success"",
        ""sequence"" : sequenceNumber,
        ""ownerId"" :""{existingTrade.tradeRequester}"",
        ""joinerId"" :""{existingTrade.tradePartner}""""
        ""status"" : {existingTrade.status}""
     }}";
                    } else
                    {
                        LoggerAccessor.LogInfo($"[SSFW] TradingService - Checking current status of transactionId {existingTrade.transId} between Requester {existingTrade.tradeRequester} & Partner {existingTrade.tradePartner}: {existingTrade.status}");
                        return $@" 	{{
     	""result"" : 0,
        ""message"" : ""Success"",
        ""sequence"" : sequenceNumber,
        ""ownerId"" :""{existingTrade.tradeRequester}"",
        ""joinerId"" :""{existingTrade.tradePartner}""""
        ""status"" : {existingTrade.status}""
     }}";
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
  ""sequence"": {transaction.seqNumb},
  ""{transaction.tradeRequester}"": {{
{string.Join("", transaction.tradeRequesterItemList)}
  }},
  ""{transaction.tradePartner}"": {{
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
                    }
                    catch (Exception e) { 
                        LoggerAccessor.LogError($"[SSFW] TradingService - Exception caught attempting to remove existing trade transaction id {existingCardTradingTransactionId} with error {e}");
                        return $" {{ \"result\": 0, \"id\": -1 }} ";
                    }

                }

            }
            #endregion
            return $" {{ \"result\": 0, \"id\": -1 }} ";

        }
    }
}