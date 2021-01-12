using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Model
{
    public class RoomsModel
    {
        public string Id { get; set; }
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateTimeUpdated { get; set; }
        public bool IsBooked { get; set; }
    }
}
