using KidsAreaApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public interface IUserService
    {

        Task<IEnumerable<ApplicationUser>> GetUsersExceptCurrentUser(Claim claim);
        Task<IEnumerable<ApplicationUser>> GetUsersForSupAdmin(Claim claim);
        Task<ApplicationUser> GetUserById(string id);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> UserLock(string id);
        Task<bool> UserUnLock(string id);
    }
}