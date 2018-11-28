using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public enum PlaceStatus : byte { Free, Reserved, Sold }

    [Serializable]
    public class Ticket
    {
        public int Row { get; set; }

        public int Place { get; set; }

        public PlaceStatus Status { get; set; }

        public Ticket() { }

        public Ticket(int row, int place)
        {
            Row = row;
            Place = place;
            Status = PlaceStatus.Free;
        }
    }
}
