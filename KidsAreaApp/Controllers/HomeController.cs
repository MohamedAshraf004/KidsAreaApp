using KidsAreaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using ZXing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using KidsAreaApp.Services;
using KidsAreaApp.Utility;
using ReflectionIT.Mvc.Paging;
using System.Collections.Generic;

namespace KidsAreaApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {      
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IReservationService _reservationService;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext, IReservationService reservationService
            , IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _dbContext = dbContext;
            _reservationService = reservationService;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index() => View(new Reservation());

        //[Authorize(SD.Admin+","+SD.SupAdmin)]
        public async Task<IActionResult> ReservationTransactions(DateTime endDate = new DateTime(), DateTime startDate = new DateTime(), int pageindex = 1)
        {
            var result = await _reservationService.ReservationTransactions(startDate, endDate, pageindex);

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> MakeReservation(Reservation reservation)
        {
            //Call method generate qrcode for all of these
            reservation = await _reservationService.GenerateQRCode(reservation);
            return new ViewAsPdf("PrintReservation", reservation);
        }

        [HttpPost]
        public async Task<IActionResult> QrCodeReader(/*IFormFile qrcodeUploaded*/string serialKey)
        {
            var result = await _reservationService.ReadQRCode(serialKey);
            //var finalres=await _reservationService.GetReservationAsync(result.ReservationId);
            //return View(finalres);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> PrintReceipt(Reservation reservation)
        {
            var result = await _reservationService.GetReservationForPrintFinalCost(reservation);
            return new ViewAsPdf("PrintReservation", result);
        }
    }
}
