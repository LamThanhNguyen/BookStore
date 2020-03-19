using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SuperAdminEndUser)]
    public class AdminUsersController : Controller
    {
        private readonly BookStoreContext _db;

        public AdminUsersController(BookStoreContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.KhachHangs.Where(u=>u.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var adminUser = await _db.KhachHangs.FirstOrDefaultAsync(m => m.Id == id);

            if(adminUser == null)
            {
                return NotFound();
            }

            adminUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var adminUser = await _db.KhachHangs.FirstOrDefaultAsync(m => m.Id == id);

            if(adminUser == null)
            {
                return NotFound();
            }

            adminUser.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET Edit
        public async Task<IActionResult> Edit(string id)
        {
            if(id==null || id.Trim().Length==0)
            {
                return NotFound();
            }

            var adminUser = await _db.KhachHangs.FindAsync(id);
            if(adminUser==null)
            {
                return NotFound();
            }

            return View(adminUser);
        }


        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(string id, KhachHang khachHang)
        {
            if(id!=khachHang.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                KhachHang adminUser = _db.KhachHangs.Where(u => u.Id == id).FirstOrDefault();
                adminUser.Name = khachHang.Name;
                adminUser.PhoneNumber = khachHang.PhoneNumber;
                adminUser.Class = khachHang.Class;
                adminUser.Address = khachHang.Address;
                adminUser.City = khachHang.City;
                adminUser.Country = khachHang.Country;
                adminUser.ShipAddress = khachHang.ShipAddress;

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(khachHang);
        }
    }
}