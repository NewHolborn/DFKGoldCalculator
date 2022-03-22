using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WeeklyGain
{
    public class CompletedTransaction
    {
        public CompletedTransaction (string hsh , DateTime dt) { hash = hsh;date = dt; }
        public string hash { get; set; }
        public DateTime date { get; set; }
    }
    public class lsttransactions 
    {
        public List<Transaction> transactions { get; set; }
    }
    public class Transaction
    {
        public string blockHash { get; set; }
        public string blockNumber { get; set; }
        public string ethHash { get; set; }
        public string from { get; set; }
        public string gas { get; set; }
        public string gasPrice { get; set; }
        public string hash { get; set; }
        public string input { get; set; }
        public string nonce { get; set; }
        public string r { get; set; }
        public string s { get; set; }
        public int shardID { get; set; }
        public string timestamp { get; set; }
        public string to { get; set; }
        public int toShardID { get; set; }
        public string transactionIndex { get; set; }
        public string v { get; set; }
        public string value { get; set; }
    }
    public class Log
    {
        public string address { get; set; }
        public string blockHash { get; set; }
        public string blockNumber { get; set; }
        public string data { get; set; }
        public string logIndex { get; set; }
        public bool removed { get; set; }
        public List<string> topics { get; set; }
        public string transactionHash { get; set; }
        public string transactionIndex { get; set; }
    }
    public class Reciept
    {
        public string blockHash { get; set; }
        public int blockNumber { get; set; }
        public string contractAddress { get; set; }
        public int cumulativeGasUsed { get; set; }
        public string from { get; set; }
        public int gasUsed { get; set; }
        public List<Log> logs { get; set; }
        public string logsBloom { get; set; }
        public string root { get; set; }
        public int shardID { get; set; }
        public int status { get; set; }
        public string to { get; set; }
        public string transactionHash { get; set; }
        public int transactionIndex { get; set; }
    }
    public enum DFKItem 
    {
        none = 0,
        Bloaters = 1,
        IronScales =2,
        Lanterney = 3,
        RedGill = 4,
        SailFish = 5,
        Shimerscale = 6,
        Silverfin = 7,
        Tears=100,
        Rune = 101,
        BlueEgg = 102
    }
    public class Account
    {
        #region Constructors
        public Account()
        {
            LoadDict();
        }
        public Account(string wallet)
        {
            mWallet = wallet;
            LoadDict();
        }
        #endregion

        #region variables
        public int Bloaters = 0;
        public int IronScales = 0;
        public int Lanterney = 0;
        public int RedGill = 0;
        public int SailFish = 0;
        public int Shimerscale = 0;
        public int Silverfin = 0;
        public int Tears = 0;
        public int Runes = 0;
        public int BlueEggs = 0;
        public string mWallet = "";
        public List<CompletedTransaction> CompletedHashes = null;
        public List<CompletedTransaction> DatesHashes = null;
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        private Dictionary<string, DFKItem> mDict;
        #endregion

        #region public Methods
        public void reset()
        {
            Bloaters = 0;
            IronScales = 0;
            Lanterney = 0;
            RedGill = 0;
            SailFish = 0;
            Shimerscale = 0;
            Silverfin = 0;
            Tears = 0;
            Runes = 0;
            BlueEggs = 0;
    }
        public double LoadBalance()
        {
            if (mWallet == "") return 0.0;
            BigInteger balance = GetBalance(mWallet);
            return (double)BigInteger.Divide(balance, (BigInteger)1e15) / 1000;
        }
        public void LoadTransactionCount()
        {
            if (mWallet == "") return;
            GetTransactions(mWallet);
        }
        public void LoadTransactionCount(DateTime from, DateTime to)
        {
            DatesHashes = new List<CompletedTransaction>();
            if (CompletedHashes == null || CompletedHashes.Count == 0) return;
            foreach (CompletedTransaction item in CompletedHashes)
            {
                if (item.date >= from.Date && item.date <= to.Date) DatesHashes.Add(item);
            }
        }
        public void LoadTransactionCount(int transactionCount)
        {
            DatesHashes = new List<CompletedTransaction>();
            if (CompletedHashes == null || CompletedHashes.Count == 0) return;
            for (int i=0;i< transactionCount; i++)
            {
                DatesHashes.Add (CompletedHashes[i]);
            }
        }
        public void LoadItemsFromTransactions()
        {
            if (CompletedHashes == null || DatesHashes == null || mWallet == "") return;
            reset();
            GetItemsFromTransactions(DatesHashes);
        }
        #endregion
        #region API methods
        private BigInteger GetBalance(string wallet)
        {
            var t = Task.Run(() => GetBalanceString(mWallet));
            t.Wait();

            if (t != null && mWallet != "")
            {
                dynamic stuff = JObject.Parse((t.Result));
                return stuff.result;
            }
            return -1;
        }

        async Task<string> GetBalanceString(string wallet)
        {
            using (HttpClient client = new HttpClient())
            {
                JArray jarrayObj = new JArray();
                jarrayObj.Add(wallet);

                JObject data = new JObject(
                        new JProperty("id", "1"),
                        new JProperty("jsonrpc", "2.0"),
                        new JProperty("method", "hmyv2_getBalance"),
                        new JProperty("params", jarrayObj));

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = string.Format("https://rpc.s0.t.hmny.io");
                HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
                return null;
            }
        }

        

        async Task<string> GetTransactionsString(string wallet,int pageindex)
        {
            using (HttpClient client = new HttpClient())
            {
                JArray jarrayObj = new JArray();
                jarrayObj.Add(JObject.FromObject(new { address = wallet, pageindex = pageindex, pageSize = 1000 , fullTx = true, txType = "SENT", order = "DESC" }));

                JObject data = new JObject(
                    new JProperty("jsonrpc", "2.0")
                    , new JProperty("method", "hmy_getTransactionsHistory")
                    , new JProperty("params", jarrayObj)
                    , new JProperty("id", "1")
                    );

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = string.Format("https://api.s0.t.hmny.io/");
                HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
                return null;
            }
        }
        
        private void GetTransactions(string wallet)
        {
            if (mWallet == "") return;
            bool exit = false;
            int page = 0;
            lsttransactions transactions;
            CompletedHashes = new List<CompletedTransaction>();
            while (!exit)
            {
                var t = Task.Run(() => GetTransactionsString(wallet, page));
                t.Wait();

                if (t != null)
                {
                    dynamic stuff = JObject.Parse((t.Result));
                    transactions = stuff["result"].ToObject<lsttransactions>();
                    foreach (Transaction trans in transactions.transactions)
                    {
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt32(trans.timestamp, 16)).ToLocalTime();

                        if (trans.input.StartsWith("0x528be0a"))
                        {
                            CompletedHashes.Add(new CompletedTransaction (trans.hash, dateTime.Date));
                        }
                    }
                    if (transactions.transactions!= null && transactions.transactions.Count == 0) exit = true;
                }
                page++;
            }
        }
        async Task<string> GetItemsFromTransactionsString(string transaction)
        {
            using (HttpClient client = new HttpClient())
            {
                JArray jarrayObj = new JArray();
                jarrayObj.Add(transaction);

                JObject data = new JObject(
                    new JProperty("jsonrpc", "2.0")
                    , new JProperty("method", "hmyv2_getTransactionReceipt")
                    , new JProperty("params", jarrayObj)
                    , new JProperty("id", "1")
                    );

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = string.Format("https://api.s0.t.hmny.io/");
                HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
                return null;
            }
        }
        
        private void GetItemsFromTransactions(List<CompletedTransaction> transactions)
        {
            foreach (CompletedTransaction item in transactions)
            {
                var t = Task.Run(() => GetItemsFromTransactionsString(item.hash));
                t.Wait();

                if (t != null)
                {
                    dynamic stuff = JObject.Parse((t.Result));
                    Reciept reciept = stuff["result"].ToObject<Reciept>();
                    if (reciept != null)
                    {
                        DFKItem dfkitem = DFKItem.none;
                        int count = 0;
                        foreach (Log log in reciept.logs)
                        {
                            dfkitem = FindItemInDict(log.address);
                            bool succ = int.TryParse(log.data.Substring (2), out count);
                            if (!succ) continue;
                            switch (dfkitem)
                            {   
                                case DFKItem.none:break;
                                case DFKItem.Bloaters: Bloaters+= count; break;
                                case DFKItem.IronScales: IronScales += count; break;
                                case DFKItem.Lanterney: Lanterney += count; break;
                                case DFKItem.RedGill: RedGill += count; break;
                                case DFKItem.SailFish: SailFish += count; break;
                                case DFKItem.Shimerscale: Shimerscale += count; break;
                                case DFKItem.Silverfin: Silverfin += count; break;
                                case DFKItem.Tears: Tears += count; break;
                                case DFKItem.Rune: Runes += count; break;
                                case DFKItem.BlueEgg: BlueEggs += count; break;
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region Items Dictionary
        private DFKItem FindItemInDict(string ItemAddress)
        {
            DFKItem res;
            bool succ = mDict.TryGetValue(ItemAddress, out res);
            if (!succ) return DFKItem.none;
            else return res;
        }
        private void LoadDict()
        {
            mDict = new Dictionary<string, DFKItem>(comparer);
            mDict.Clear();
            mDict.Add("0x78aED65A2Cc40C7D8B0dF1554Da60b38AD351432", DFKItem.Bloaters);
            mDict.Add("0xe4Cfee5bF05CeF3418DA74CFB89727D8E4fEE9FA", DFKItem.IronScales);
            mDict.Add("0x8Bf4A0888451C6b5412bCaD3D9dA3DCf5c6CA7BE", DFKItem.Lanterney);
            mDict.Add("0xc5891912718ccFFcC9732D1942cCD98d5934C2e1", DFKItem.RedGill);
            mDict.Add("0xb80A07e13240C31ec6dc0B5D72Af79d461dA3A70", DFKItem.SailFish);
            mDict.Add("0x372CaF681353758f985597A35266f7b330a2A44D", DFKItem.Shimerscale);
            mDict.Add("0x2493cfDAcc0f9c07240B5B1C4BE08c62b8eEff69", DFKItem.Silverfin);

            mDict.Add("0x24eA0D436d3c2602fbfEfBe6a16bBc304C963D04", DFKItem.Tears);
            mDict.Add("0x66F5BfD910cd83d3766c4B39d13730C911b2D286", DFKItem.Rune);
            mDict.Add("0x9678518e04Fe02FB30b55e2D0e554E26306d0892", DFKItem.BlueEgg);
        }
        #endregion
    }
}
