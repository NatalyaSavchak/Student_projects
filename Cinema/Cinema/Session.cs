using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    [Serializable]
    public class Session
    {
        public string FilmName { get; set; }

        public CinemaHallScheme HallScheme { get; set; }

        public List<Ticket> tickets = new List<Ticket>();

        public string Finish { get; set; }

        public Session() { }

        public Session(string filmName, CinemaHallScheme hall, string finish)
        {
            FilmName = filmName;
            HallScheme = hall;
            Finish = finish;
            for (int i = 0; i < HallScheme.CountOfRows; ++i)
            {
                for (int j = 0; j < HallScheme.CountOfPlacesInRow; ++j)
                {
                    tickets.Add(new Ticket(i, j));
                }
            }
        }

        public void PrintInfoAboutTickets()
        {
            Console.WriteLine($"Hall {HallScheme.Name}");
            Console.WriteLine("Free places:");
            int counter = 0;
            for (int i = 0; i < HallScheme.CountOfRows; ++i)
            {
                Console.Write($"Row {i + 1,3}:  |");
                for (int j = 0; j < HallScheme.CountOfPlacesInRow; ++j)
                {
                    if (tickets[counter].Status == PlaceStatus.Free)
                        Console.Write($"{j + 1,3};");
                    else
                        Console.Write($"{"__",3};");
                    ++counter;
                }
                Console.WriteLine();
            }
        }
    }
}
