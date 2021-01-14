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

namespace KidsAreaApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeController(ILogger<HomeController> logger,AppDbContext dbContext
            ,IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            this._dbContext = dbContext;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()=>View();
        [HttpGet]
        public IActionResult MakeReservation()=>View();
        [HttpPost]
        public async Task<IActionResult> MakeReservation(Reservation reservation)
        {
            if (!ModelState.IsValid)
                return View(reservation);
            //Qr Code writer to write it to Db
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = 
                _qrCode.CreateQrCode(reservation.Receipt.SerialKey.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            var result=BitmapToBytesCode(qrCodeImage);
            reservation.Receipt.BarCode = result;
            //Qr Code writer to write it to wwwroot (Server)
            string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{reservation.Receipt.SerialKey}.bmp";
            var qrcodeWritere = new BarcodeWriter();
            qrcodeWritere.Format = BarcodeFormat.QR_CODE;
            qrcodeWritere.Write($"{reservation.Receipt.SerialKey}")
                          .Save(qrcodePath);
            //For test perposes
            //await _dbContext.Set<Receipt>().AddAsync(reservation.Receipt);
            //await _dbContext.SaveChangesAsync();
            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
            return new ViewAsPdf("PrintReservation",reservation);
        }
        [HttpPost]
        public async Task<IActionResult> QrCodeReader(IFormFile qrcodeUploaded)
        {
            //Read QrCode
            string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{qrcodeUploaded.FileName}";

            var qrcodebitmap = (Bitmap)Bitmap.FromFile(qrcodePath);
            var qrcodeReader = new BarcodeReader();
            var qrcodeResult = qrcodeReader.Decode(qrcodebitmap);
            var res =await _dbContext.Reservations
                .Include(receipt=>receipt.Receipt)
                .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == qrcodeResult.Text);
            res.EndReservationTme = DateTime.UtcNow;
            var timeInArea = res.EndReservationTme.Subtract(res.StartReservationTme);
            var receipt=_dbContext.Set<Receipt>().FirstOrDefault(x => x.SerialKey.ToString() == qrcodeResult.Text);
            res.Receipt = receipt;
            res.Cost=await CalCost(timeInArea);
            return new ViewAsPdf("PrintReservation", res);
        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        [NonAction]
        private async Task<double> CalCost(TimeSpan time)
        {
            var hourCost = (await _dbContext.Hours.FirstOrDefaultAsync()).HourPrice;
            if (time.Hours <1)
            {
                return hourCost;
            }
            var hoursprice=time.Hours * hourCost;
            var minPrice = time.Minutes 
               switch
            {
                <= 15 => .25 * hourCost, 
                > 15 and <= 30 => .50 * hourCost, 
                > 30 and <= 45 => .75 * hourCost, 
                > 45 => hourCost
            };
            return minPrice + hoursprice;
        }
    }
}
