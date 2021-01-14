using KidsAreaApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IEnumerable<ApplicationUser>> GetUsersExceptCurrentUser(Claim claim)
        {
            var users = await _dbContext.ApplicationUsers.Where(c => c.Id != claim.Value).ToListAsync();
            return users;
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id);
            
        }
        public async Task<bool> UserLock(string id)
        {
           var r =  (await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id))
                .LockoutEnd= DateTime.Now.AddYears(2000);
           return await _dbContext.SaveChangesAsync() >0 ?true : false ;
            
        }
        public async Task<bool> UserUnLock(string id)
        {
           var r =  (await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id))
                .LockoutEnd= DateTime.Now;
           return await _dbContext.SaveChangesAsync() >0 ?true : false ;
            
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _dbContext.ApplicationUsers.FirstOrDefaultAsync(c => c.Email == email);
            
        }
    }
}
