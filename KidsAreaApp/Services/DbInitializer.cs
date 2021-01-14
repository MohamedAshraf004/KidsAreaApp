using KidsAreaApp.Models;
using KidsAreaApp.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> logger;

        public DbInitializer(AppDbContext dbContext, UserManager<ApplicationUser> userManager
                            , RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this.logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (_dbContext.Roles.Any(r => r.Name == SD.Admin) == true) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.SupAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Receptionist)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name="Mohamed",
                EmailConfirmed = true,
                PhoneNumber = "1201339358"
            }, "Admin@123").GetAwaiter().GetResult();


            ApplicationUser user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");

            (_userManager.AddToRoleAsync(user, SD.Admin)).GetAwaiter().GetResult();
            _dbContext.SaveChangesAsync().GetAwaiter().GetResult();

        }
    }
}
