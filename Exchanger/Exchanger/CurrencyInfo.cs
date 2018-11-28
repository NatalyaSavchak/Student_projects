using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Serialization;
using System.IO;

namespace Exchanger
{
    public class CurrencyInfo
    {
        public List<currency> currencyList = new List<currency>();
        public void Load()
        {
            WebClient nbuAPI = new WebClient();
            XmlSerializer serializer = new XmlSerializer(typeof(List<currency>), new XmlRootAttribute("exchange"));
            using (StreamReader reader = new StreamReader(nbuAPI.OpenRead("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange")))
            {
                currencyList = serializer.Deserialize(reader) as List<currency>;
            }
        }
        public void PrintInfo()
        {
            Console.WriteLine("NBU CURRENCY RATE");
            Console.WriteLine($"|{"Code",-5}|{"Currency full name",-34}|{"Short name",-11}|{"Rate",-9}|{"Date",-10}|");
            foreach (var item in currencyList)
            {
                Console.WriteLine($"|{item.Code, 5}|{item.FullName,-34}|{item.ShortName,-11}|{Math.Round(item.Rate, 2, MidpointRounding.AwayFromZero), 9}|{item.Date,-10}|");
            }
        }
    }
}
