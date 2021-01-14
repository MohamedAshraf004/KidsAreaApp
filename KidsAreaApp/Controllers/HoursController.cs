using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KidsAreaApp.Models;
using Microsoft.AspNetCore.Authorization;
using KidsAreaApp.Utility;

namespace KidsAreaApp.Controllers
{
    [Authorize(Roles = SD.Admin +","+ SD.SupAdmin)]
    public class HoursController : Controller
    {
        private readonly AppDbContext _context;

        public HoursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Hours
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hours.ToListAsync());
        }      

        // GET: Hours/Create
        public IActionResult Create()
        {
            if (_context.Hours.Count()>0)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HourId,HourPrice")] Hour hour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hour);
        }

        // GET: Hours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hour = await _context.Hours.FindAsync(id);
            if (hour == null)
            {
                return NotFound();
            }
            return View(hour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HourId,HourPrice")] Hour hour)
        {
            if (id != hour.HourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HourExists(hour.HourId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hour);
        }

        // GET: Hours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hour = await _context.Hours
                .FirstOrDefaultAsync(m => m.HourId == id);
            if (hour == null)
            {
                return NotFound();
            }

            return View(hour);
        }

        // POST: Hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hour = await _context.Hours.FindAsync(id);
            _context.Hours.Remove(hour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HourExists(int id)
        {
            return _context.Hours.Any(e => e.HourId == id);
        }
    }
}
