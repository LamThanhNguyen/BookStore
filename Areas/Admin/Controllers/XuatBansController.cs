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
    public class XuatBansController : Controller
    {
        private readonly BookStoreContext _db;

        public XuatBansController(BookStoreContext db)
        {
            _db = db;
        }

        // GET: Admin/XuatBans
        public IActionResult Index()
        {
            return View(_db.XuatBans.ToList());
        }

        // GET: Admin/XuatBans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xuatBan = await _db.XuatBans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xuatBan == null)
            {
                return NotFound();
            }

            return View(xuatBan);
        }

        // GET: Admin/XuatBans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/XuatBans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(XuatBan xuatBan)
        {
            if (ModelState.IsValid)
            {
                _db.Add(xuatBan);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xuatBan);
        }

        // GET: Admin/XuatBans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xuatBan = await _db.XuatBans.FindAsync(id);
            if (xuatBan == null)
            {
                return NotFound();
            }
            return View(xuatBan);
        }

        // POST: Admin/XuatBans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, XuatBan xuatBan)
        {
            if (id != xuatBan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(xuatBan);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XuatBanExists(xuatBan.Id))
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
            return View(xuatBan);
        }

        // GET: Admin/XuatBans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xuatBan = await _db.XuatBans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xuatBan == null)
            {
                return NotFound();
            }

            return View(xuatBan);
        }

        // POST: Admin/XuatBans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var xuatBan = await _db.XuatBans.FindAsync(id);
            _db.XuatBans.Remove(xuatBan);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XuatBanExists(int id)
        {
            return _db.XuatBans.Any(e => e.Id == id);
        }
    }
}
