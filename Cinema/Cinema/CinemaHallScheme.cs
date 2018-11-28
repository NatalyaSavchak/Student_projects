using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    [Serializable]
    public class CinemaHallScheme
    {
        public string Name { get; set; }

        public int CountOfRows { get; set; }

        public int CountOfPlacesInRow { get; set; }

        public CinemaHallScheme() { }

        public CinemaHallScheme(string name, int countOfRows, int countOfPlacesInRow)
        {
            Name = name;
            if (countOfRows > 0)
                CountOfRows = countOfRows;
            else
                throw new ArgumentOutOfRangeException("Count of rows can be only positive number!");
            if (countOfPlacesInRow > 0)
                CountOfPlacesInRow = countOfPlacesInRow;
            else
                throw new ArgumentOutOfRangeException("Count of places in a row can be only positive number!");
        }
    }
}
