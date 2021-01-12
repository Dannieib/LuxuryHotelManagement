using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.Model
{
    public class DashBoardModel
    {
        public IEnumerable<RoomsModel> AllRooms  { get; set; }
        public IEnumerable<RoomsModel> FreeRooms { get; set; }
        public IEnumerable<RoomsModel> BookedRooms { get; set; }
        public IEnumerable<RoomsOnTransitModel> RoomsBookedByClients { get; set; }
        public IEnumerable<Users> UserModels { get; set; }
    }
}
