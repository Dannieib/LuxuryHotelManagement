using Hotel.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel.AppService.UsersService
{
    public interface IUsersAppService
    {
        Task<(bool isInserted, string message)> CreateNewUser(Users model);
        Task<Users> GetUserByEmailAddress(string emailAddress);
        Task<IEnumerable<Users>> GetAllUsers();
    }
}