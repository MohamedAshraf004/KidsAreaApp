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
        public IActionResult Index()=>View(new Reservation());
        [HttpPost]
        public async Task<IActionResult> MakeReservation(Reservation reservation)
        {
            //Qr Code writer to write it to Db
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = 
                _qrCode.CreateQrCode(reservation.Receipt.SerialKey.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            var result=BitmapToBytesCode(qrCodeImage);
            reservation.Receipt.BarCode = result;

            #region write  to wwwroot (Server)
            //Qr Code writer to write it to wwwroot (Server)
            //string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{reservation.Receipt.SerialKey}.bmp";
            //var qrcodeWritere = new BarcodeWriter();
            //qrcodeWritere.Format = BarcodeFormat.QR_CODE;
            //qrcodeWritere.Write($"{reservation.Receipt.SerialKey}")
            //              .Save(qrcodePath);
            //For test perposes
            //await _dbContext.Set<Receipt>().AddAsync(reservation.Receipt);
            //await _dbContext.SaveChangesAsync();
            #endregion

            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
            return new ViewAsPdf("PrintReservation",reservation);
        }
     
        [HttpPost]
        public async Task<IActionResult> QrCodeReader(IFormFile qrcodeUploaded)
        {
            #region read from server
            //string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{qrcodeUploaded.FileName}";
            //var qrcodebitmap = (Bitmap)Bitmap.FromFile(qrcodePath);
            //var qrcodeResult = qrcodeReader.Decode(qrcodebitmap);
            //var res =await _dbContext.Reservations
            //    .Include(receipt=>receipt.Receipt)
            //    .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == qrcodeResult.Text);
            //test
            #endregion

            //Read QrCode
            var qrcodeReader = new BarcodeReader();
            using var str = qrcodeUploaded.OpenReadStream();
 
            var resultImg = Image.FromStream(str);
            var result=qrcodeReader.Decode((Bitmap)resultImg);
            var res = await _dbContext.Reservations
                            .Include(receipt => receipt.Receipt)
                            .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == result.Text.ToString());
            res.EndReservationTme = DateTime.UtcNow;
            var timeInArea = res.EndReservationTme.Subtract(res.StartReservationTme);
            var receipt=_dbContext.Set<Receipt>().FirstOrDefault(x => x.SerialKey.ToString() == result.Text.ToString());
            res.Receipt = receipt;
            var ult = BitmapToBytesCode((Bitmap)resultImg);
            res.Receipt.BarCode = ult;
            res.TotatCost=await CalCost(timeInArea);
            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> PrintReceipt(Reservation reservation)
        {
            //to make sure that casher can;t change the total cost
            var reservationFromDb = await _dbContext.Reservations.Include(r => r.Receipt).FirstOrDefaultAsync(reservation => reservation.ReservationId == reservation.ReservationId);
            reservationFromDb.Receipt.BarCode = reservation.Receipt.BarCode;
            reservationFromDb.Discount = reservation.Discount;
            reservationFromDb.CostAfterDiscount = reservationFromDb.TotatCost - reservationFromDb.Discount;
            return new ViewAsPdf("PrintReservation", reservationFromDb);
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
        private async Task<double> CalCost(TimeSpan time,double discount=0)
        {
            var hourCost = (await _dbContext.Hours.FirstOrDefaultAsync()).HourPrice;
            if (time.Hours <1)
            {
                return hourCost-discount;
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
            return (minPrice + hoursprice)-discount;
        }
    }
}
