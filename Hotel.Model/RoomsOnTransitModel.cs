using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Model
{
    public class RoomsOnTransitModel
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public DateTime DateTimeBooked { get; set; }
        public DateTime DateTimeFreed { get; set; }
        public int NoOfDays { get; set; }
        public string EmailAddressOfClient { get; set; }
        public string ClientId { get; set; }
        public decimal TotalCharge { get; set; }
        public bool IsBooked { get; set; }
    }

    public enum BookingStatus:int
    {
        IsBooked=1,
        IsFree =2
    }
}
