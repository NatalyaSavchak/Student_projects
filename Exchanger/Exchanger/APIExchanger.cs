using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchanger
{
    class APIExchanger
    {
        private Exchanger exchanger = new Exchanger();
        public APIExchanger()
        {
            exchanger.Load();
            exchanger.nbuRatesList.Load();
        }
        public void StartWork()
        {
            User us = User.customer;
            while (us != User.finishWork)
            {
                Console.WriteLine();
                Console.WriteLine("ADMIN - enter \"1\"");
                Console.WriteLine("CUSTOMER - enter \"2\"");
                Console.WriteLine("To finish work of terminal - enter \"0\"");
                Console.WriteLine();
                Console.Write("Choose action...");
                try
                {
                    us = (User)Enum.Parse(typeof(User), Console.ReadLine());
                    Console.Clear();
                    switch (us)
                    {
                        case User.admin: AdminOptions(); break;
                        case User.customer: CustomerOptions(); break;
                        case User.finishWork: exchanger.Export(); break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }
        private void AdminOptions()
        {
            int action = -1;
            while (action != 0)
            {
                Console.WriteLine();
                Console.WriteLine("To print currency ballance list - enter \"1\"");
                Console.WriteLine("To print nbu currency rate list - enter \"2\"");
                Console.WriteLine("To add currency to exchanger - enter \"3\"");
                Console.WriteLine("To remove currency from exchanger - enter \"4\"");
                Console.WriteLine("To come back to main menu - enter \"0\"");
                Console.WriteLine();
                Console.Write("Choose action...");
                action = Int32.Parse(Console.ReadLine());
                Console.Clear();
                switch (action)
                {
                    case 1: exchanger.PrintReport(User.admin); break;
                    case 2: exchanger.nbuRatesList.PrintInfo(); break;
                    case 3:
                        {
                            Console.WriteLine("Enter the code of currency you want to add...");
                            int currencyCode = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the sum of currency...");
                            decimal sum = Decimal.Parse(Console.ReadLine());
                            if (exchanger.AddNewCurrency(currencyCode, sum))
                            {
                                Console.WriteLine();
                                Console.WriteLine("Transaction was successful!");
                            }
                        }
                        break;
                    case 4:
                        {
                            Console.WriteLine("Enter the code of currency you want to remove...");
                            int currencyCode = Int32.Parse(Console.ReadLine());
                            if (exchanger.RemoveCurrency(currencyCode))
                            {
                                Console.WriteLine();
                                Console.WriteLine("Transaction was successful!");
                            }
                        }
                        break;
                    case 0: break;
                    default: Console.WriteLine("You enter uncorrect number!"); break;
                }
                if (action != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                }
                Console.Clear();
            }
        }
        private void CustomerOptions()
        {
            int action = -1;
            while (action != 0)
            {
                Console.WriteLine();
                Console.WriteLine("To see currency list to buy - enter \"1\"");
                Console.WriteLine("To see currency list to sell - enter \"2\"");
                Console.WriteLine("To exchange currency - enter \"3\"");
                Console.WriteLine("To come back to main menu - enter \"0\"");
                Console.WriteLine();
                Console.Write("Choose action...");
                action = Int32.Parse(Console.ReadLine());
                Console.Clear();
                switch (action)
                {
                    case 1: exchanger.PrintReport(User.customer); break;
                    case 2: exchanger.nbuRatesList.PrintInfo(); break;
                    case 3:
                        {
                            Console.Write("Enter the code of currency you want to sell...");
                            int currencyCodeSell = Int32.Parse(Console.ReadLine());
                            Console.Write("Enter the sum of currency you want to sell...");
                            decimal sumCurrencySell = Decimal.Parse(Console.ReadLine());
                            Console.Write("Enter the code of currency you want to buy...");
                            int currencyCodeBuy = Int32.Parse(Console.ReadLine());
                            try
                            {
                                if (exchanger.ToExchange(currencyCodeSell, sumCurrencySell, currencyCodeBuy))
                                    Console.WriteLine("Transaction was successful!");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        break;
                    case 0: break;
                    default: Console.WriteLine("You enter uncorrect number!"); break;
                }
                if (action != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                }
                Console.Clear();
            }
        }
    }
}
