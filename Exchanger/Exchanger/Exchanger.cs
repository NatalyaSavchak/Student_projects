using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Exchanger
{
    enum User { admin = 1, customer = 2, finishWork = 0}

    [Serializable]
    public class DictionaryObject
    {
        public int Key { get; set; }
        public decimal Value { get; set; }
        public DictionaryObject() { }
        public DictionaryObject(int key, decimal value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class ToExchangeMethodException : ApplicationException
    {
        public ToExchangeMethodException() { }
        public ToExchangeMethodException(string message) : base(message) { }
        public ToExchangeMethodException(string message, Exception inner) : base(message, inner) { }
        protected ToExchangeMethodException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    class Exchanger
    {
        public CurrencyInfo nbuRatesList = new CurrencyInfo();
        public Dictionary<int, decimal> realCurrencyList = new Dictionary<int, decimal>();
        public void Load()
        {
            List<DictionaryObject> transfer = new List<DictionaryObject>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<DictionaryObject>));
            using (Stream stream = new FileStream("Balance.xml", FileMode.Open))
            {
                transfer = serializer.Deserialize(stream) as List<DictionaryObject>;
            }
            foreach (var item in transfer)
            {
                realCurrencyList.Add(item.Key, item.Value);
            }
        }
        public void Export()
        {
            List<DictionaryObject> transfer = new List<DictionaryObject>();
            foreach (var item in realCurrencyList)
            {
                transfer.Add(new DictionaryObject(item.Key, item.Value));
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<DictionaryObject>));
            using (Stream stream = new FileStream("Balance.xml", FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, transfer);
            }
        }
        public bool AddNewCurrency (int currencyCode, decimal sum)
        {
            if (nbuRatesList.currencyList.Find(i => (i.Code == currencyCode)) == null)
                throw new ToExchangeMethodException("Error! You enter not existing currency number!");
            if (sum <= 0)
                throw new ArgumentOutOfRangeException("sum", "Error! The sum can't be less or equal zero!");
            realCurrencyList.Add(currencyCode, sum);
            return true;
        }
        public bool RemoveCurrency (int currencyCode)
        {
            if (!realCurrencyList.ContainsKey(currencyCode))
                throw new ToExchangeMethodException("Sorry. the exchanger has no currency specified.");
            realCurrencyList.Remove(currencyCode);
            return true;
        }
        public bool ToExchange(int currencyCodeSell, decimal currencySumSell, int currencyCodeBuy)
        {
            if(nbuRatesList.currencyList.Find(i => (i.Code == currencyCodeSell)) == null)
                throw new ToExchangeMethodException("Error! You enter not existing currency number!");
            if (currencySumSell <= 0)
                throw new ArgumentOutOfRangeException("currencySumSell", "Error! The sum to sell can't be less or equal zero!");
            if (!realCurrencyList.ContainsKey(currencyCodeBuy))
                throw new ToExchangeMethodException("Sorry. the exchanger has no currency specified to buy.");
            decimal currencyRateSell =(nbuRatesList.currencyList.Find(i => (i.Code == currencyCodeSell))).Rate;
            decimal currencyRateBuy = (nbuRatesList.currencyList.Find(i => (i.Code == currencyCodeBuy))).Rate;
            decimal currencySumBuy = currencyRateSell * currencySumSell / currencyRateBuy;
            if (realCurrencyList[currencyCodeBuy] <= currencySumBuy)
                throw new ToExchangeMethodException("Sorry, the exchanger does not have enough currency specified to buy.");
            if (!realCurrencyList.ContainsKey(currencyCodeSell))
                AddNewCurrency(currencyCodeSell, currencySumSell);
            else
                realCurrencyList[currencyCodeSell] += currencySumSell;
            realCurrencyList[currencyCodeBuy] -= currencySumBuy;
            if (realCurrencyList[currencyCodeBuy] == 0)
                RemoveCurrency(currencyCodeBuy);
            return true;
        }
        public void PrintReport(User us)
        {
            currency currentItem = new currency();
            switch(us)
            {
                case User.admin:
                    {
                        Console.WriteLine("LIST OF CURRENCY BALANCE");
                        Console.WriteLine($"| { "Code", 5}| {"Currency",-40}| {"Rate", 9}| {"Balance",-10}|");
                        foreach (var item in realCurrencyList)
                        {
                            currentItem = nbuRatesList.currencyList.Find(i => (i.Code == item.Key));
                            Console.WriteLine($"| {item.Key, 5}| {currentItem.FullName,-40}| {Math.Round(currentItem.Rate, 2, MidpointRounding.AwayFromZero), 9}| {Math.Round(item.Value, 2, MidpointRounding.AwayFromZero),-10}|");
                        }
                    } break;
                case User.customer:
                    {
                        Console.WriteLine("LIST OF CURRENCY TO BUY");
                        Console.WriteLine($"| {"Code", 5}| {"Currency",-40}| {"Rate", 9}|");
                        foreach (var item in realCurrencyList)
                        {
                            currentItem = nbuRatesList.currencyList.Find(i => (i.Code == item.Key));
                            Console.WriteLine($"| {item.Key, 5}| {currentItem.FullName,-40}| {Math.Round(currentItem.Rate, 2, MidpointRounding.AwayFromZero), 9}|");
                        }
                    }
                    break;
            }
            
        }
    }
}
