using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    class Cinema
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            APICinema cinema = new APICinema();
            User us = User.Customer;
            while (us != User.EndOfWork)
            {
                Console.WriteLine($"{"ADMIN - ",-30} {"enter \"1\"", -10}");
                Console.WriteLine($"{"CUSTOMER - ",-30} {"enter \"2\"", -10}");
                Console.WriteLine($"{"To finish work of terminal - ",-30} {"enter \"0\"",-10}");
                Console.WriteLine();
                Console.Write("Choose action...");
                try
                {
                    us = (User)Enum.Parse(typeof(User), Console.ReadLine());
                    Console.Clear();
                    switch (us)
                    {
                        case User.Admin:
                            {
                                Console.WriteLine("Enter login...");
                                string login = Console.ReadLine();
                                Console.WriteLine("Enter password...");
                                string password = Console.ReadLine();
                                Console.Clear();
                                if (!cinema.AdminAuthentication(login, password))
                                    Console.WriteLine("You entered uncorect login or password!");
                                Console.Clear();
                            }
                            break;
                        case User.Customer: cinema.CustomerOptions(); break;
                        case User.EndOfWork: break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("You enter uncorrect number!");
                    Console.WriteLine("To come back to menu press \"Enter\"");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            cinema.Export();
            return 1;
        }
    }
}
