using Hotel.AppService.UsersService;
using Hotel.Infrastructure;
using Hotel.Infrastructure.MongoLib;
using Hotel.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.AppService.Rooms
{
    public class RoomsAppService : IRoomsAppService
    {
        private readonly IUsersAppService _usersAppService;
        private readonly IMongoBaseRepository _mongoBasdeRepostory;

        public RoomsAppService(IUsersAppService usersAppService, IMongoBaseRepository mongoBaseRepository)
        {
            _mongoBasdeRepostory = mongoBaseRepository;
            _usersAppService = usersAppService;
        }


        public async Task<(bool roomAdded, string message)> AddNewRoom(RoomsModel model)
        {
            var checkForExistingRoom = await _mongoBasdeRepostory.LoadRecord<RoomsModel>(TableNames.Rooms);

            var filter = checkForExistingRoom.Where(x => x.RoomNumber.ToLower().Trim().Contains(model.RoomNumber.ToLower().Trim()));
            
            if (model != null && !filter.Any())
            {
                model.IsBooked = false;
                var insert = await _mongoBasdeRepostory.InsertHotelRecord(TableNames.Rooms, model);
                return insert != null ? (true, "Room successfully Added") : (false, "Could not process request at the db level");
            }
            return (false, "Process failed. Please retry..");
        }

        public async Task<(bool roomAdded, string message)> UpdateRoomAvailability(RoomsModel model)
        {
            var check = await _mongoBasdeRepostory.LoadRecordById<RoomsModel>(TableNames.Rooms, model.Id);
            if (check != null)
            {
                model.DateTimeUpdated = DateTime.Now;
                await _mongoBasdeRepostory.UpdateOrInsert(TableNames.Rooms, model.Id, model);
                return (true, "Room successfully updated");
            }
            return (false, "Process failed. Please retry..");
        }

        public async Task<IEnumerable<RoomsModel>> GetRoomAvailabilityByStatus(bool isBooked)
        {
            var checkForExistingRoom = await _mongoBasdeRepostory.LoadRecord<RoomsModel>(TableNames.Rooms);

            var filter = checkForExistingRoom.Where(x => x.IsBooked == isBooked);
            return filter;
        }

        public async Task<(bool roomAdded, string message)> BookRoom(RoomsOnTransitModel model)
        {
            var checkRoom = await _mongoBasdeRepostory.LoadRecord<RoomsModel>(TableNames.Rooms);
            var checkIfRoomWithRoomNameExist = checkRoom.FirstOrDefault(x => x.RoomNumber == model.RoomId);
            var roomNumber = string.Empty;
            var roomPrice = 0.0m;

            var checkUserAvailability = await _usersAppService.GetAllUsers();
            var checkUser = checkUserAvailability.FirstOrDefault(x => x.Id == model.ClientId || x.EmailAddress == model.EmailAddressOfClient);

            if (checkUser != null)
            {
                if (checkIfRoomWithRoomNameExist != null)
                {
                    TimeSpan noOfDays = model.DateTimeFreed - model.DateTimeBooked;

                    model.TotalCharge = checkIfRoomWithRoomNameExist.Price * noOfDays.Days;

                    model.NoOfDays = noOfDays.Days;
                    model.IsBooked = true;
                }
                if (model != null && checkIfRoomWithRoomNameExist!=null)
                {
                    var insert = await _mongoBasdeRepostory.InsertHotelRecord(TableNames.BookedRooms, model);
                    if (insert != null)
                    {
                        var roomsModel = new RoomsModel
                        {
                            Id = checkIfRoomWithRoomNameExist.Id,
                            RoomNumber = checkIfRoomWithRoomNameExist.RoomNumber,
                            Price = checkIfRoomWithRoomNameExist.Price,
                            DateCreated = checkIfRoomWithRoomNameExist.DateCreated,
                            DateTimeUpdated = DateTime.Now,
                            IsBooked = model.IsBooked
                        };
                        await this.UpdateRoomAvailability(roomsModel);
                        return (true, "Room successfully booked");
                    }
                    else
                    {
                        return (false, "Could not process request at the db level");
                    }
                }
            }
            else
            {
                return (false, "User with the name above does not exist...");
            }
            return (false, "Process failed. Please retry..");
        }

        public async void GetBookedRoomsByRoomId(string id)
        {
            var  getAll = await _mongoBasdeRepostory.LoadRecord<RoomsOnTransitModel>(TableNames.BookedRooms);
            if (getAll.Any())
            {
                var getRoomById = getAll.FirstOrDefault(x => x.Id == id);
                if (getRoomById != null)
                {
                    await this.UpdateBookedRoomAvailability(getRoomById);
                }
            }
        }

        public async Task<(bool roomAdded, string message)> UpdateBookedRoomAvailability(RoomsOnTransitModel model)
        {
            var check = await _mongoBasdeRepostory.LoadRecordById<RoomsOnTransitModel>(TableNames.BookedRooms, model.Id);
            if (check != null)
            {
                model.DateTimeFreed = DateTime.Now;
                await _mongoBasdeRepostory.UpdateOrInsert(TableNames.BookedRooms, model.Id, model);

                var getAllRooms = await _mongoBasdeRepostory.LoadRecord<RoomsModel>(TableNames.Rooms);
                var getByRoomName = getAllRooms.FirstOrDefault(x => x.RoomNumber == model.RoomId);

                var roomsModel = new RoomsModel
                {
                    Id = getByRoomName.Id,
                    Price = getByRoomName.Price,
                    DateCreated = getByRoomName.DateCreated,
                    DateTimeUpdated = DateTime.Now,
                    RoomNumber = getByRoomName.RoomNumber,
                    IsBooked = model.IsBooked
                };

                await this.UpdateRoomAvailability(roomsModel);

                return (true, "Room successfully updated");
            }
            return (false, "Process failed. Please retry..");
        }


        public async Task<DashBoardModel> GetSummaries()
        {
            var getAllForRooms = await _mongoBasdeRepostory.LoadRecord<RoomsModel>(TableNames.Rooms);
            var getAllForUsers = await _mongoBasdeRepostory.LoadRecord<Users>(TableNames.Users);
            var getAllForBookedRooms = await _mongoBasdeRepostory.LoadRecord<RoomsOnTransitModel>(TableNames.BookedRooms);

            var AllBookedRooms = getAllForRooms.Where(x => x.IsBooked == true);
            var AllFreeRooms = getAllForRooms.Where(x => x.IsBooked == false);

            IEnumerable< RoomsOnTransitModel> getAllBookedRoomsForUsers = null;
            foreach (var item in getAllForUsers)
            {
                //Func<RoomsOnTransitModel, bool> func = (x => x.ClientId == item.Id);
                var filter = getAllForBookedRooms.Where(x => x.ClientId == item.FirstName || x.EmailAddressOfClient == item.EmailAddress).GroupBy(x => x.EmailAddressOfClient)
                .Select(group => new RoomsOnTransitModel
                {
                    ClientId = group.Key,
                    EmailAddressOfClient = item.EmailAddress
                });

                if (filter.Any())
                {
                    getAllBookedRoomsForUsers = filter;
                }
                else
                {
                    getAllBookedRoomsForUsers = null;
                }
            }

            var model = new DashBoardModel
            {
                AllRooms = getAllForRooms,
                BookedRooms = AllBookedRooms,
                FreeRooms = AllFreeRooms,
                RoomsBookedByClients = getAllBookedRoomsForUsers,
                UserModels = getAllForUsers
            };

            return model;
        }


        //for background service..
        public async void AutoBookOut()
        {
            var check = await _mongoBasdeRepostory.LoadRecord<RoomsOnTransitModel>(TableNames.BookedRooms);
            if (check.Any())
            {
                var checkBookedRoomsByCheckOutDate = check.Where(x => x.DateTimeBooked >= DateTime.Now);
                if (checkBookedRoomsByCheckOutDate != null || checkBookedRoomsByCheckOutDate.Any())
                {
                    foreach (var item in checkBookedRoomsByCheckOutDate)
                    {
                        var bookedRoomModel = new RoomsOnTransitModel
                        {
                            IsBooked = false
                        };
                        await this.UpdateBookedRoomAvailability(bookedRoomModel);
                    }
                }
            }
        }
    }
}
