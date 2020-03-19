using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SuperAdminEndUser)]
    public class VanChuyensController : Controller
    {
        private readonly BookStoreContext _db;

        public VanChuyensController(BookStoreContext db)
        {
            _db = db;
        }

        // GET: Admin/VanChuyens
        public IActionResult Index()
        {
            return View( _db.VanChuyens.ToList());
        }

        // GET: Admin/VanChuyens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vanChuyen = await _db.VanChuyens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vanChuyen == null)
            {
                return NotFound();
            }

            return View(vanChuyen);
        }

        // GET: Admin/VanChuyens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/VanChuyens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VanChuyen vanChuyen)
        {
            if (ModelState.IsValid)
            {
                _db.Add(vanChuyen);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vanChuyen);
        }

        // GET: Admin/VanChuyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vanChuyen = await _db.VanChuyens.FindAsync(id);
            if (vanChuyen == null)
            {
                return NotFound();
            }
            return View(vanChuyen);
        }

        // POST: Admin/VanChuyens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VanChuyen vanChuyen)
        {
            if (id != vanChuyen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(vanChuyen);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VanChuyenExists(vanChuyen.Id))
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
            return View(vanChuyen);
        }

        // GET: Admin/VanChuyens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vanChuyen = await _db.VanChuyens
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vanChuyen == null)
            {
                return NotFound();
            }

            return View(vanChuyen);
        }

        // POST: Admin/VanChuyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vanChuyen = await _db.VanChuyens.FindAsync(id);
            _db.VanChuyens.Remove(vanChuyen);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VanChuyenExists(int id)
        {
            return _db.VanChuyens.Any(e => e.Id == id);
        }
    }
}
