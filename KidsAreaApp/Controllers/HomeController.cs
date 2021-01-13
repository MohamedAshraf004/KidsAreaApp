using KidsAreaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using ZXing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KidsAreaApp.Controllers
{
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult MakeReservation()
        {
            return View();
        }
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

            string qrcodePath = hostEnvironment.WebRootPath + $"/Images/{reservation.Receipt.SerialKey}.bmp";
            var qrcodeWritere = new BarcodeWriter();
            qrcodeWritere.Format = BarcodeFormat.QR_CODE;
            qrcodeWritere.Write($"{reservation.Receipt.SerialKey}")
                          .Save(qrcodePath);

            var qrcodebitmap = (Bitmap)Bitmap.FromFile(qrcodePath);
            var qrcodeReader = new BarcodeReader();
            var qrcodeResult = qrcodeReader.Decode(qrcodebitmap);
            Debug.WriteLine($"Decode barcode text : {qrcodeResult.Text}");
            Debug.WriteLine($"barcode format: {qrcodeResult.BarcodeFormat}");
            Debug.WriteLine("Qr code read successfully");
            reservation.Receipt.QrCodePath = $"{qrcodePath}";
            await _dbContext.Set<Receipt>().AddAsync(reservation.Receipt);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Reservations.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();
           


            return View(reservation);
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

        /*QRCode Writer
            var qrcodeWritere=    new BarCodeWritert();
            qrcodeWritere.Format=BarCodeFormat.QR_Code;
            qrcodeWritere.Write("Content")
                          .save("Paht.bmp);

        QrCode Reader

            var qrcodebitmap = (Bitmap)Bitmap.FromFile(@"F:\qrcode\demo.bmp");
            var qrcodeReader = new BarcodeReader();
            var qrcodeResult = qrcodeReader.Decode(qrcodebitmap);

            Console.WriteLine($"Decode barcode text : {qrcodeResult.Text}");
            Console.WriteLine($"barcode format: {qrcodeResult.BarcodeFormat}");
            Console.WriteLine("Qr code read successfully");
         */


        public async Task<IActionResult> PrintReceipt(Reservation reservation)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> QrCodeReader(IFormFile qrcodeUploaded)
        {
          //  qrcodeUploaded=HttpContext.Request.Form.Files.FirstOrDefault();
            //Read QrCode
            string qrcodePath = hostEnvironment.WebRootPath + $"/Images/{qrcodeUploaded.FileName}";

            var qrcodebitmap = (Bitmap)Bitmap.FromFile(qrcodePath);
            var qrcodeReader = new BarcodeReader();
            var qrcodeResult = qrcodeReader.Decode(qrcodebitmap);
            Debug.WriteLine($"Decode barcode text : {qrcodeResult.Text}");
            Debug.WriteLine($"barcode format: {qrcodeResult.BarcodeFormat}");
            Debug.WriteLine("Qr code read successfully");

            var res =await _dbContext.Reservations
                .Include(receipt=>receipt.Receipt)
                .Include(receipt=>receipt.Customer)
                .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == qrcodeResult.Text);
            res.EndReservationTme = DateTime.UtcNow;
            var timeInArea = res.EndReservationTme.Subtract(res.StartReservationTme);
            var receipt=_dbContext.Set<Receipt>().FirstOrDefault(x => x.SerialKey.ToString() == qrcodeResult.Text);
            res.Receipt = receipt;
            res.Cost=CalCost(timeInArea);

            return View(res);
        }
        [NonAction]
        private static double CalCost(TimeSpan time)
        {
            var hoursprice=time.Hours * 10;
            var minPrice = time.Minutes 
               switch
            {
                <= 15 => .25 *10, 
                > 15 and <= 30 => .50 *10, 
                > 30 and <= 45 => .75 *10, 
                > 45 => 10
            };
            return minPrice + hoursprice;
        }
    }
}
