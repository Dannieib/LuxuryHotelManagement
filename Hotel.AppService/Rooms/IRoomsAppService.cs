using Hotel.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.AppService.Rooms
{
    public interface IRoomsAppService
    {
        Task<(bool roomAdded, string message)> AddNewRoom(RoomsModel model);
        void AutoBookOut();
        Task<(bool roomAdded, string message)> BookRoom(RoomsOnTransitModel model);
        Task<IEnumerable<RoomsModel>> GetRoomAvailabilityByStatus(bool isBooked);
        Task<(bool roomAdded, string message)> UpdateBookedRoomAvailability(RoomsOnTransitModel model);
        Task<(bool roomAdded, string message)> UpdateRoomAvailability(RoomsModel model);
        void GetBookedRoomsByRoomId(string id);

        Task<DashBoardModel> GetSummaries();
    }
}