using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Models.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly BookStoreContext _db;

        public HomeController(BookStoreContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Books = await _db.Books.Include(m => m.TacGia).Include(m => m.TheLoai).Include(m => m.XuatBan).ToListAsync(),
                TheLoais = await _db.TheLoais.ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCarts.Where(u => u.MaKhachHang == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var bookFromDb = await _db.Books.Include(m => m.TacGia).Include(m => m.TheLoai).Include(m => m.XuatBan).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                Book = bookFromDb,
                BookId = bookFromDb.Id
            };
            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            CartObject.MaKhachHang = claim.Value;

            ShoppingCart cartFromDb = await _db.ShoppingCarts.Where(c => c.MaKhachHang == CartObject.MaKhachHang
                                            && c.BookId == CartObject.BookId).FirstOrDefaultAsync();

            if (cartFromDb == null)
            {
                await _db.ShoppingCarts.AddAsync(CartObject);
            }
            else
            {
                cartFromDb.Count = cartFromDb.Count + CartObject.Count;
            }

            await _db.SaveChangesAsync();

            var count = _db.ShoppingCarts.Where(c => c.MaKhachHang == CartObject.MaKhachHang).ToList().Count();
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
