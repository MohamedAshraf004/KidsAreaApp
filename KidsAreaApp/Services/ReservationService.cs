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
            var result = await _dbContex.Reservations.Include(r => r.Receipt).FirstOrDefaultAsync(Reservation => Reservation.ReservationId == resservationId);
            return result;
        }
        public async Task<PagingList<Reservation>> ReservationTransactions(DateTime startDate, DateTime endDate, int pageindex)
        {
            PagingList<Reservation> model;
            List<Reservation> query;
            if (startDate == new DateTime() && endDate == new DateTime())
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme).ToListAsync();
                model = PagingList.Create(query, 10, pageindex);
                model.Action = "ReservationTransactions";
                return model;
                //return query;
            }
            if (startDate != new DateTime() && endDate == new DateTime())
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme)
                    .Where(t => t.StartReservationTme.Day == startDate.Day && t.StartReservationTme.Month == startDate.Month).ToListAsync();
                model = PagingList.Create(query, 10, pageindex);
                model.Action = "ReservationTransactions";
                return model;
                //return query;

            }
            if (startDate == new DateTime() && endDate != new DateTime())
            {
                query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme)
                   .Where(t => t.EndReservationTme.Day == endDate.Day && t.EndReservationTme.Month == endDate.Month).ToListAsync();

                model = PagingList.Create(query, 10, pageindex);
                model.Action = "ReservationTransactions";
                return model;
                //return query;
            }

            query = await _dbContex.Reservations.AsNoTracking().OrderByDescending(x => x.StartReservationTme)
               .Where(t => t.StartReservationTme.Day >= startDate.Day && t.EndReservationTme.Day <= endDate.Day
               && t.EndReservationTme.Month == endDate.Month && t.StartReservationTme.Month == startDate.Month).ToListAsync();
            model = PagingList.Create(query, 10, pageindex);
            model.Action = "ReservationTransactions";
            return model;
            //return query;
        }
        public async Task<Reservation> GenerateQRCode(Reservation reservation)
        {
            //Qr Code writer to write it to Model
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData =
                _qrCode.CreateQrCode(reservation.Receipt.SerialKey.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            reservation.Receipt.BarCode = BitmapToBytesCode(qrCodeImage);

            await _dbContex.Reservations.AddAsync(reservation);
            var result = await _dbContex.SaveChangesAsync();
            #region Qr Code writer to write it to wwwroot (Server) for testing
            //Qr Code writer to write it to wwwroot (Server) for testing
            //string qrcodePath = hostEnvironment.WebRootPath + $"/Images/QrCode/{reservation.Receipt.SerialKey}.bmp";
            //var qrcodeWritere = new BarcodeWriter();
            //qrcodeWritere.Format = BarcodeFormat.QR_CODE;
            //qrcodeWritere.Write($"{reservation.Receipt.SerialKey}")
            //              .Save(qrcodePath);
            #endregion

            return reservation;
        }
        public async Task<Reservation> ReadQRCode(string serialKey)
        {
            var reservation = await _dbContex.Reservations
                            .Include(receipt => receipt.Receipt)
                            .FirstOrDefaultAsync(r => r.Receipt.SerialKey.ToString() == serialKey);
            if (reservation.EndReservationTme == new DateTime(2021, 1, 1, 12, 0, 0))
            {
                reservation.EndReservationTme = DateTime.UtcNow;
                var restest = await _dbContex.SaveChangesAsync();
            }
            var timeInArea = reservation.EndReservationTme.Subtract(reservation.StartReservationTme);
            //If customer come in anthor time to review his receipt
            if (reservation.TotatCost == 0)
            {
                reservation.TotatCost = await CalCost(timeInArea);
                var reservationUpdated = _dbContex.Reservations.Update(reservation);
                //var reservationUpdated = _dbContex.Reservations.Attach(reservation);
                //reservationUpdated.State = EntityState.Modified;
            }
            var res = await _dbContex.SaveChangesAsync();

            return reservation;
        }
        public async Task<Reservation> GetReservationForPrintFinalCost(Reservation reservation)
        {
            var reservationd = await _dbContex.Reservations.FindAsync(reservation.ReservationId);
            //var reservationFromDb = await _dbContex.Reservations.Include(r => r.Receipt).FirstOrDefaultAsync(reservation => reservation.Receipt.SerialKey == reservation.Receipt.SerialKey);

            if (reservationd.TotatCost == 0)
            {
                if (reservationd.EndReservationTme == new DateTime(2021, 1, 1, 12, 0, 0) || reservationd.EndReservationTme == new DateTime())
                {
                    reservationd.EndReservationTme = DateTime.UtcNow;
                }
                reservationd.TotatCost = await CalCost(reservationd.EndReservationTme.Subtract(reservationd.StartReservationTme));
            }
            //reservationd.Receipt.BarCode = reservation.Receipt.BarCode;
            reservationd.Discount = reservation.Discount;
            reservationd.CostAfterDiscount = reservationd.TotatCost - reservationd.Discount;
            _dbContex.Reservations.Update(reservationd);

            //var resupdated = _dbContex.Reservations.Attach(reservationFromDb);
            //resupdated.State = EntityState.Modified;
            var result = await _dbContex.SaveChangesAsync();
            return reservationd;

        }

        #region helper methods
        private async Task<double> CalCost(TimeSpan time, double discount = 0)
        {
            var hourCost = (await _dbContex.Hours.FirstOrDefaultAsync()).HourPrice;
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
