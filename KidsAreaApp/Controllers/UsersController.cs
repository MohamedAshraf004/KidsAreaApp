using System.Security.Claims;
using System.Threading.Tasks;
using KidsAreaApp.Services;
using KidsAreaApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KidsAreaApp.Controllers
{
    [Authorize(Roles = SD.Admin+","+SD.SupAdmin)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = ((ClaimsIdentity)this.User.Identity);
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _userService.GetUsersExceptCurrentUser(claim));
        }
        public async Task<IActionResult> Lock(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (await _userService.UserLock(id))
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
        public async Task<IActionResult> UnLock(string id)
        {
            if (id==null)
            {
                return NotFound();
            }
            if (await _userService.UserUnLock(id))
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

    }
}
