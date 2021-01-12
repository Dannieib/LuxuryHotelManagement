using Hotel.AppService.Rooms;
using Hotel.AppService.UsersService;
using Hotel.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelMgtController : ControllerBase
    {

        private readonly IRoomsAppService _roomsAppService;
        private readonly IUsersAppService _usersAppService;

        public HotelMgtController(IUsersAppService usersAppService, IRoomsAppService roomsAppService)
        {
            _roomsAppService = roomsAppService;
            _usersAppService = usersAppService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> InsertUser(Users model)
        {
            try
            {
                if (model == null) throw new ArgumentNullException("Mode lcannot be validated");
                model.Id = Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("ddmmyyyyhhss");
                var insertUser = await _usersAppService.CreateNewUser(model);
                if (insertUser.isInserted == false)
                {
                    return BadRequest(insertUser.message);
                }
                else
                    return Ok(insertUser.message);
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUser(string emailAddress)
        {
            try
            {
                if (emailAddress == null) throw new ArgumentNullException("user email cannot be validated");

                var insertUser = await _usersAppService.GetUserByEmailAddress(emailAddress);
                if (insertUser == null)
                {
                    return BadRequest("No user found");
                }
                else
                    return Ok(insertUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateNewRoom(RoomsModel model)
        {
            try
            {
                if (model == null) throw new ArgumentNullException("model cannot be validated");
                model.Id= Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("ddmmyyyyhhss");
                var insertUser = await _roomsAppService.AddNewRoom(model);
                if (insertUser.roomAdded == false)
                {
                    return BadRequest(insertUser.message);
                }
                else
                    return Ok(insertUser.message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> BookNewRoom(RoomsOnTransitModel model)
        {
            try
            {
                if (model == null) throw new ArgumentNullException("model cannot be validated");
                model.Id = Guid.NewGuid().ToString() + "_" + DateTime.Now.ToString("ddmmyyyyhhss");
                var insertUser = await _roomsAppService.BookRoom(model);
                if (insertUser.roomAdded == false)
                {
                    return BadRequest(insertUser.message);
                }
                else
                    return Ok(insertUser.message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> BookRoom(string roomId)
        {
            try
            {
                if (roomId == null) throw new ArgumentNullException("roomNumber cannot be empty");
                _roomsAppService.GetBookedRoomsByRoomId(roomId);
                  return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var getSummary = await _roomsAppService.GetSummaries();
                return Ok(getSummary);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
