using KidsAreaApp.Models;
using KidsAreaApp.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserService(AppDbContext dbContext,UserManager<ApplicationUser> userManager
                ,SignInManager<ApplicationUser> signInManager)
        {
            this._dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersExceptCurrentUser(Claim claim)
        {
            var users = await _dbContext.Users.Where(c => c.Id != claim.Value).ToListAsync();
            //var users = await userManager.GetUsersInRoleAsync(SD.Receptionist);
            return users;
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == id);
            
        }
        public async Task<bool> UserLock(string id)
        {
           var r =  (await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == id))
                .LockoutEnd= DateTime.Now.AddYears(2000);
           return await _dbContext.SaveChangesAsync() >0 ?true : false ;
            
        }
        public async Task<bool> UserUnLock(string id)
        {
           var r =  (await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == id))
                .LockoutEnd= DateTime.Now;
           return await _dbContext.SaveChangesAsync() >0 ?true : false ;
            
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Email == email);            
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersForSupAdmin(Claim claim)
        {
            List<ApplicationUser> Users = new List<ApplicationUser>();

            var recUsers = await userManager.GetUsersInRoleAsync(SD.Receptionist);
            var supadminUsers = (await userManager.GetUsersInRoleAsync(SD.SupAdmin)).Where(c => c.Id != claim.Value);
            Users.AddRange(recUsers);
            Users.AddRange(supadminUsers);
            return Users;

        }
    }
}
