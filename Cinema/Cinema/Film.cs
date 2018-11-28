using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    [Serializable]
    public class Film
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public short MoveLength { get; set; }

        public byte AgeLimit { get; set; }

        public int BaseTicketPrice { get; set; }

        public DateTime StartInCinema { get; set; }

        public DateTime FinishInCinema { get; set; }

        public Film() { }

        public Film(string name, string description, short moveLength, byte ageLimit, int baseTicketPrice, DateTime startInCinema, DateTime finishInCinema)
        {
            Name = name;
            Description = description;
            MoveLength = moveLength;
            AgeLimit = ageLimit;
            BaseTicketPrice = baseTicketPrice;
            StartInCinema = startInCinema;
            FinishInCinema = finishInCinema;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"{"NAME:",-20}| {Name}");
            Console.WriteLine($"{"SHORT DESCRIPTION:", -20}| {Description}");
            Console.WriteLine($"{"DURATION OF FILM:",-20}| {MoveLength}");
            Console.WriteLine($"{"AGE LIMIT:",-20}| {AgeLimit}");
            Console.WriteLine($"{"START IN CINEMA:",-20}| {StartInCinema}");
            Console.WriteLine($"{"LAST DAY IN CINEMA:",-20}| {FinishInCinema}");
            Console.WriteLine();
        }
    }
}
