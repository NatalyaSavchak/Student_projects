using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Exchanger
{
    [Serializable]
    public class currency
    {
        [XmlElement("r030", Order = 1)]
        public int Code { get; set; }
        [XmlElement("txt", Order = 2)]
        public string FullName { get; set; }
        [XmlElement("rate", Order = 3)]
        public decimal Rate { get; set; }
        [XmlElement("cc", Order = 4)]
        public string ShortName { get; set; }
        [XmlElement("exchangedate", Order = 5)]
        public string Date { get; set; }

        public override string ToString()
        {
            return $"| {Code,-5}| {FullName,-40}|{Rate,15} | {ShortName,-5}| {Date,-10}|";
        }

        public bool FindByCode(int code)
        {
            if (Code == code)
                return true;
            return false;
        }
    }


}
