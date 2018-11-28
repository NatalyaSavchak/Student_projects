using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchanger
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            APIExchanger apiExchanger = new APIExchanger();
            apiExchanger.StartWork();
            return 1;
        }
    }
}
