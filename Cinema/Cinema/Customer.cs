using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    [Serializable]
    public class Customer
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime Birth { get; set; }

        public string Password { get; set; }

        public int CountWatchedFilms { get; set; }

        public Customer() { }

        public Customer(string name, string surname, DateTime birth, string password)
        {
            Name = name;
            Surname = surname;
            Birth = birth;
            Password = password;
            CountWatchedFilms = 0;
        }

        public void ByTicket()
        {
            CountWatchedFilms++;
        }

        public bool Autorize(string password)
        {
            if (Password == password)
                return true;
            else
                return false;
        }

        public void PrintInfo()
        {
            Console.WriteLine("{0,15} {1,15} | {2,5}", Name, Surname, CountWatchedFilms);
        }

        public override bool Equals(object obj)
        {
            var customer = obj as Customer;
            return customer != null &&
                   CountWatchedFilms == customer.CountWatchedFilms;
        }

        public override int GetHashCode()
        {
            return 1587141753 + CountWatchedFilms.GetHashCode();
        }

        public static bool operator < (Customer c1, Customer c2)
        {
            if (c1.CountWatchedFilms < c2.CountWatchedFilms)
                return true;
            else
                return false;
        }

        public static bool operator > (Customer c1, Customer c2)
        {
            if (c1.CountWatchedFilms < c2.CountWatchedFilms)
                return true;
            else
                return false;
        }

        public static bool operator == (Customer c1, Customer c2)
        {
            if (c1.CountWatchedFilms == c2.CountWatchedFilms)
                return true;
            else
                return false;
        }

        public static bool operator !=(Customer c1, Customer c2)
        {
            if (c1.CountWatchedFilms != c2.CountWatchedFilms)
                return true;
            else
                return false;
        }
    }
}
