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
        private readonly IReservationService _reservationService;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeController(ILogger<HomeController> logger,IReservationService reservationService
            , IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _reservationService = reservationService;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index() => View(new Reservation());

        public async Task<IActionResult> ReservationTransactions(DateTime endDate = new DateTime(), DateTime startDate = new DateTime(), int pageindex = 1)
        {
            var result = await _reservationService.ReservationTransactions(startDate, endDate, pageindex);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> MakeReservation(Reservation reservation)
        {
            //Call method generate qrcode for all of these
            reservation = await _reservationService.GeneratebarCode(reservation);
            var re = new ViewAsPdf("PrintReservation")
            {
                PageMargins= { Left = 3, Bottom = 3, Right = 3, Top = 3 },
                PageWidth =55,
                PageHeight=50,
                Model=reservation
            };
            return re;
        }
        #region test reservation
        //[HttpPost]
        //public async Task<IActionResult> MakeReservation1(Reservation reservation)
        //{
        //    //Call method generate qrcode for all of these
        //    //reservation = await _reservationService.GenerateQRCode(reservation);
        //    reservation = await _reservationService.GenerateBarCodeByGraphics(reservation);
        //    return View("PrintReservation1", reservation);
        //}

        //[HttpPost]
        //public async Task<IActionResult> MakeReservation2(Reservation reservation)
        //{
        //    //Call method generate qrcode for all of these
        //    //reservation = await _reservationService.GenerateQRCode(reservation);
        //    reservation = await _reservationService.GeneratebarCode39(reservation);
        //    return View("PrintReservation2", reservation);
        //    //return new ViewAsPdf("PrintReservation", reservation);
        //}

        //[HttpPost]
        //public async Task<IActionResult> MakeReservation3(Reservation reservation)
        //{
        //    //Call method generate qrcode for all of these
        //    //reservation = await _reservationService.GenerateQRCode(reservation);
        //    reservation = await _reservationService.GeneratebarCodeAllOneD(reservation);
        //    return View("PrintReservation1", reservation);
        //    //return new ViewAsPdf("PrintReservation", reservation);
        //}
        #endregion

        [HttpPost]
        public async Task<IActionResult> QrCodeReader(string serialKey)
        {
            var result = await _reservationService.ReadQRCode(serialKey);
            if (result ==null)
            {
                return View("NotFound");
            }
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> PrintReceipt(Reservation reservation)
        {
            var result = await _reservationService.GetReservationForPrintFinalCost(reservation);
            var ret= new ViewAsPdf("PrintReservation")
            {
                PageMargins = { Left = 3, Bottom = 3, Right = 3, Top = 3 },
                PageWidth = 55,
                PageHeight = 62,
                Model = result
            };
            return ret;
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
