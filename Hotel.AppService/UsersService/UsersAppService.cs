using Hotel.Infrastructure;
using Hotel.Infrastructure.MongoLib;
using Hotel.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.AppService.UsersService
{
    public class UsersAppService : IUsersAppService
    {
        private readonly IMongoBaseRepository _mongoBasdeRepostory;

        public UsersAppService(IMongoBaseRepository mongoBaseRepository)
        {
            _mongoBasdeRepostory = mongoBaseRepository;
        }

        public async Task<(bool isInserted, string message)> CreateNewUser(Users model)
        {
            var getUserByEmail = await this.GetUserByEmailAddress(model.EmailAddress);
            if (model != null && getUserByEmail == null)
            {
                var insert = await _mongoBasdeRepostory.InsertHotelRecord<Users>(TableNames.Users, model);
                return insert != null ? (true, "User successfully inserted") : (false, "Could not process user at the db level");
            }
            else if (getUserByEmail != null)
            {
                return (false, "User with this email address already exist..");
            }
            return (false, "Process failed. Please retry..");
        }

        public async Task<Users> GetUserByEmailAddress(string emailAddress)
        {
            var getUser = await _mongoBasdeRepostory.LoadRecordByEmailAddress<Users>(TableNames.Users, emailAddress);
            return getUser != null ? getUser : null;
        }

        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            var getUser = await _mongoBasdeRepostory.LoadRecord<Users>(TableNames.Users);
            return getUser != null ? getUser : null;
        }
    }
}
