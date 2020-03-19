using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using BookStore.Utility;

namespace BookStore.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ThanhToansController : Controller
    {
        private readonly BookStoreContext _db;

        public ThanhToansController(BookStoreContext db)
        {
            _db = db;
        }

        // GET: Admin/ThanhToans
        public async Task<IActionResult> Index()
        {
            return View(await _db.ThanhToans.ToListAsync());
        }

        // GET: Admin/ThanhToans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhToan = await _db.ThanhToans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thanhToan == null)
            {
                return NotFound();
            }

            return View(thanhToan);
        }

        // GET: Admin/ThanhToans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ThanhToans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhToan thanhToan)
        {
            if (ModelState.IsValid)
            {
                _db.Add(thanhToan);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thanhToan);
        }

        // GET: Admin/ThanhToans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhToan = await _db.ThanhToans.FindAsync(id);
            if (thanhToan == null)
            {
                return NotFound();
            }
            return View(thanhToan);
        }

        // POST: Admin/ThanhToans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThanhToan thanhToan)
        {
            if (id != thanhToan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(thanhToan);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThanhToanExists(thanhToan.Id))
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
            return View(thanhToan);
        }

        // GET: Admin/ThanhToans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhToan = await _db.ThanhToans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thanhToan == null)
            {
                return NotFound();
            }

            return View(thanhToan);
        }

        // POST: Admin/ThanhToans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thanhToan = await _db.ThanhToans.FindAsync(id);
            _db.ThanhToans.Remove(thanhToan);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThanhToanExists(int id)
        {
            return _db.ThanhToans.Any(e => e.Id == id);
        }
    }
}
