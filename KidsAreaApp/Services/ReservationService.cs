using KidsAreaApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace KidsAreaApp.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _dbContex;
        private readonly IWebHostEnvironment hostEnvironment;

        public ReservationService(AppDbContext dbContext, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnvironment)
        {
            _dbContex = dbContext;
            this.hostEnvironment = hostEnvironment;
        }

        public async Task<Reservation> GetReservationAsync(int resservationId)
        {
            var result = await _dbContex.Reservations.FirstOrDefaultAsync(Reservation => Reservation.SerialKey == resservationId);
            return result;
        }
        public async Task<PagingList<Reservation>> ReservationTransactions(DateTime startDate, DateTime endDate, int pageindex)
        {
            PagingList<Reservation> model;
            List<Reservation> query;
            int index = 1;
            if (startDate == new DateTime() && endDate == new DateTime())
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme).ToListAsync();
            }
            else if (startDate != new DateTime() && endDate == new DateTime())
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme)
                    .Where(t => t.StartReservationTme.Day == startDate.Day && t.StartReservationTme.Month == startDate.Month).ToListAsync();
            }
            else
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme)
                   .Where(t => t.EndReservationTme.Day == endDate.Day && t.EndReservationTme.Month == endDate.Month).ToListAsync();
            }
            query.ForEach(r =>
            {
                r.Index = index;
                index++;
            });
            model = PagingList.Create(query, 100, pageindex);
            model.Action = "ReservationTransactions";
            return model;
        }
        public async Task<Reservation> GenerateQRCode(Reservation reservation)
        {
            await _dbContex.Reservations.AddAsync(reservation);
            var result = await _dbContex.SaveChangesAsync();

            #region Qr Code writer to write it to Model
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData =
                _qrCode.CreateQrCode(reservation.SerialKey.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            reservation.BarCode = BitmapToBytesCode(qrCodeImage);
            #endregion

            #region Qr Code writer to write it to wwwroot (Server) for testing
            // Qr Code writer to write it to wwwroot(Server) for testing

            //string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{reservation.SerialKey}.bmp";
            //var qrcodeWritere = new BarcodeWriter();
            //qrcodeWritere.Format = BarcodeFormat.CODABAR;
            //qrcodeWritere.Write($"{reservation.SerialKey}")
            //              .Save(qrcodePath);
            #endregion

            return reservation;
        }

        public async Task<Reservation> GeneratebarCode(Reservation reservation)
        {
            await _dbContex.Reservations.AddAsync(reservation);
            await _dbContex.SaveChangesAsync();
            #region Insert Data For Testing
            //List<Reservation> ls=new List<Reservation>();
            ////test 
            //for (int i = 0; i < 200; i++)
            //{
            //    Reservation reservation1 = new Reservation
            //    {
            //        StartReservationTme = DateTime.Now,
            //        EndReservationTme = DateTime.Now,
            //    };
            //    ls.Add(reservation1);
            //}
            //await _dbContex.Reservations.AddRangeAsync(ls);
            //await _dbContex.SaveChangesAsync();
            #endregion
            #region bar code by zxing
            using (MemoryStream ms = new MemoryStream())
            {
                var qrcodeWritere = new BarcodeWriter();
                qrcodeWritere.Format = BarcodeFormat.CODE_128;
                var res = qrcodeWritere.Write($"{reservation.SerialKey}");
                res.Save(ms, ImageFormat.Png);
                //res.Save(hostEnvironment.WebRootPath + "/Images/QrCode/" + reservation.SerialKey + ".png", ImageFormat.Png);
                //The Image is finally converted to Base64 string.
                reservation.BarCode = ms.ToArray();
            }
            #endregion
            return reservation;
        }

        public async Task<Reservation> GeneratebarCode39(Reservation reservation)
        {

            await _dbContex.Reservations.AddAsync(reservation);
            await _dbContex.SaveChangesAsync();
            #region bar code by zxing
            using (MemoryStream ms = new MemoryStream())
            {
                var qrcodeWritere = new BarcodeWriter();
                qrcodeWritere.Format = BarcodeFormat.CODE_39;
                var res = qrcodeWritere.Write($"{reservation.SerialKey}");
                res.Save(ms, ImageFormat.Png);
                res.Save(hostEnvironment.WebRootPath + "/Images/QrCode/" + reservation.SerialKey + ".png", ImageFormat.Png);
                //The Image is finally converted to Base64 string.
                reservation.BarCode = ms.ToArray();
            }
            #endregion

            return reservation;
        }
        public async Task<Reservation> GeneratebarCodeAllOneD(Reservation reservation)
        {

            await _dbContex.Reservations.AddAsync(reservation);
            await _dbContex.SaveChangesAsync();
            #region bar code by zxing
            using (MemoryStream ms = new MemoryStream())
            {
                var qrcodeWritere = new BarcodeWriter();
                qrcodeWritere.Format = BarcodeFormat.QR_CODE;
                var res = qrcodeWritere.Write($"{reservation.SerialKey.ToString()}");
                res.Save(ms, ImageFormat.Png);
                res.Save(hostEnvironment.WebRootPath + "/Images/QrCode/" + reservation.SerialKey + ".png", ImageFormat.Png);
                //The Image is finally converted to Base64 string.
                reservation.BarCode = ms.ToArray();
            }
            #endregion

            return reservation;
        }

        public async Task<Reservation> GenerateBarCodeByGraphics(Reservation reservation)
        {

            await _dbContex.Reservations.AddAsync(reservation);
            await _dbContex.SaveChangesAsync();

            #region bar code by Graphics
            using (MemoryStream ms = new MemoryStream())
            {
                var key = reservation.SerialKey.ToString();
                //The Image is drawn based on length of Barcode text.
                using (Bitmap bitMap = new Bitmap(key.Length * 40, 80))
                {
                    //The Graphics library object is generated for the Image.
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        //The installed Barcode font.
                        Font oFont = new Font("IDAutomationHC39M Free Version", 16);
                        PointF point = new PointF(2f, 2f);

                        //White Brush is used to fill the Image with white color.
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);

                        //Black Brush is used to draw the Barcode over the Image.
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString(key, oFont, blackBrush, point);
                    }

                    //The Bitmap is saved to Memory Stream.
                    bitMap.Save(ms, ImageFormat.Png);
                    bitMap.Save(hostEnvironment.WebRootPath + "/Images/QrCode/" + reservation.SerialKey + ".png", ImageFormat.Png);

                    //The Image is finally converted to Base64 string.
                    reservation.BarCode = ms.ToArray();
                }
            }
            #endregion


            return reservation;
        }


        public async Task<Reservation> ReadQRCode(string serialKey)
        {
            var reservation = await _dbContex.Reservations
                            .FirstOrDefaultAsync(r => r.SerialKey.ToString() == serialKey);
            if (reservation ==null)
            {
                return null;
            }
            if (reservation.EndReservationTme == new DateTime(2021, 1, 1, 12, 0, 0))
            {
                reservation.EndReservationTme = DateTime.Now;
                //remove it
                //var restest = await _dbContex.SaveChangesAsync();
            }
            var timeInArea = reservation.EndReservationTme.Subtract(reservation.StartReservationTme);
            //If customer come in anthor time to review his receipt
            if (reservation.TotatCost == 0)
            {
                reservation.TotatCost = await CalCost(timeInArea);
                //var reservationUpdated = _dbContex.Reservations.Update(reservation);
                var reservationUpdated = _dbContex.Reservations.Attach(reservation);
                reservationUpdated.State = EntityState.Modified;
            }
            var res = await _dbContex.SaveChangesAsync();

            return reservation;
        }
        public async Task<Reservation> GetReservationForPrintFinalCost(Reservation reservation)
        {
            var reservationd = await _dbContex.Reservations.FindAsync(reservation.SerialKey);
            if (reservationd.TotatCost == 0)
            {
                if (reservationd.EndReservationTme == new DateTime(2021, 1, 1, 12, 0, 0) || reservationd.EndReservationTme == new DateTime())
                {
                    reservationd.EndReservationTme = DateTime.Now;
                }
                reservationd.TotatCost = await CalCost(reservationd.EndReservationTme.Subtract(reservationd.StartReservationTme));
            }
            reservationd.Discount = reservation.Discount;
            reservationd.CostAfterDiscount = reservationd.TotatCost - reservationd.Discount;
            var resupdated = _dbContex.Reservations.Attach(reservationd);
            resupdated.State = EntityState.Modified;
            //_dbContex.Reservations.Update(reservationd);

            var result = await _dbContex.SaveChangesAsync();
            return reservationd;

        }

        #region helper methods
        private async Task<double> CalCost(TimeSpan time, double discount = 0)
        {
            var hour = await _dbContex.Hours.FirstOrDefaultAsync();
            double hourCost;
            if (hour==null)
            {
                hourCost = 0;
            }
            else
            {
                hourCost = hour.HourPrice;
            }
            if (time.Hours < 1)
            {
                return hourCost - discount;
            }
            var hoursprice = time.Hours * hourCost;
            var minPrice = time.Minutes
               switch
            {
                <= 15 => .25 * hourCost,
                > 15 and <= 30 => .50 * hourCost,
                > 30 and <= 45 => .75 * hourCost,
                > 45 => hourCost
            };
            return (minPrice + hoursprice) - discount;
        }
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        #endregion

        #region readerQRCode With Scanner By Image
        //public async Task<Reservation> ReadQRCode(/*IFormFile qrcodeUploaded*/string serialKey)
        //{

        //    //var qrcodeReader = new BarcodeReader();
        //    //using var str = qrcodeUploaded.OpenReadStream();

        //    //var resultImg = Image.FromStream(str);
        //    //var result = qrcodeReader.Decode((Bitmap)resultImg);
        //    //var reservation = await _dbContex.Reservations
        //    //                .Include(receipt => receipt.Receipt)
        //    //                .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == result.Text.ToString());
        //    var reservation = await _dbContex.Reservations
        //                    .Include(receipt => receipt.Receipt)
        //                    .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == serialKey);
        //    if (reservation.EndReservationTme == new DateTime(2021, 1, 1, 12, 0, 0))
        //    {
        //        reservation.EndReservationTme = DateTime.UtcNow;
        //        var restest = await _dbContex.SaveChangesAsync();
        //    }
        //    var timeInArea = reservation.EndReservationTme.Subtract(reservation.StartReservationTme);
        //    //check if receipt is not null
        //    //var receipt = _dbContex.Receipts.FirstOrDefault(x => x.SerialKey.ToString() == result.Text.ToString());
        //    //reservation.Receipt = receipt;

        //    //reservation.Receipt.BarCode = BitmapToBytesCode((Bitmap)resultImg);
        //    //If customer come in anthor time to review his receipt
        //    if (reservation.TotatCost == 0)
        //    {
        //        reservation.TotatCost = await CalCost(timeInArea);
        //        var reservationUpdated = _dbContex.Reservations.Update(reservation);
        //        //var reservationUpdated = _dbContex.Reservations.Attach(reservation);
        //        //reservationUpdated.State = EntityState.Modified;
        //    }
        //    var res = await _dbContex.SaveChangesAsync();

        //    return reservation;

        //}

        #endregion
    }
}
