using KidsAreaApp.Models;
using Microsoft.AspNetCore.Http;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public interface IReservationService
    {
        Task<PagingList<Reservation>> ReservationTransactions(DateTime startDate, DateTime endDate,int pageindex);
        //Task<IEnumerable<Reservation>> ReservationTransactions(DateTime startDate, DateTime endDate,int pageindex);
        Task<Reservation> GenerateQRCode(Reservation reservation);
        Task<Reservation> GetReservationAsync(int resservationId);
        Task<Reservation> GetReservationForPrintFinalCost(Reservation reservation);
        Task<Reservation> ReadQRCode(string serialKey);
    }
}