using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModel;
using BookStore.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly BookStoreContext _db;
        private readonly IEmailSender _emailSender;


        public OrderDetailsCart detailsCart { get; set; }

        public ShoppingCartController(BookStoreContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            detailsCart = new OrderDetailsCart()
            {
                DonHang = new Models.DonHang()
            };

            detailsCart.DonHang.Total = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCarts.Where(c => c.MaKhachHang == claim.Value);
            if(cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }

            foreach(var list in detailsCart.listCart)
            {
                list.Book = await _db.Books.FirstOrDefaultAsync(m => m.Id == list.BookId);
                detailsCart.DonHang.Total = detailsCart.DonHang.Total + ((list.Book.Price - (list.Book.Price * list.Book.Discount)) * list.Count);
            }

            return View(detailsCart);
        }

        public async Task<IActionResult> Summary()
        {
            detailsCart = new OrderDetailsCart()
            {
                DonHang = new Models.DonHang()
            };

            detailsCart.DonHang.Total = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            KhachHang khachHang = await _db.KhachHangs.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCarts.Where(c => c.MaKhachHang == claim.Value);
            if(cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }

            foreach (var list in detailsCart.listCart)
            {
                list.Book = await _db.Books.FirstOrDefaultAsync(m => m.Id == list.BookId);
                detailsCart.DonHang.Total = detailsCart.DonHang.Total + ((list.Book.Price - (list.Book.Price * list.Book.Discount)) * list.Count);
            }

            detailsCart.DonHang.PickupName = khachHang.Name;
            detailsCart.DonHang.PhoneNumer = khachHang.PhoneNumber;
            detailsCart.DonHang.OrderTime = DateTime.Now;

            return View(detailsCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCart detailsCart = new OrderDetailsCart()
            {
                listCart = new List<ShoppingCart>(),
                DonHang = new Models.DonHang()
            };

            detailsCart.listCart = await _db.ShoppingCarts.Where(c => c.MaKhachHang == claim.Value).ToListAsync();


            detailsCart.DonHang.PaymentStatus = SD.PaymentStatusPending;
            detailsCart.DonHang.OrderDate = DateTime.Now;
            detailsCart.DonHang.KhachHangID = claim.Value;
            detailsCart.DonHang.Status = SD.PaymentStatusPending;
            detailsCart.DonHang.OrderTime = Convert.ToDateTime(detailsCart.DonHang.OrderDate.ToShortDateString() + " " + detailsCart.DonHang.OrderTime.ToShortTimeString());
            detailsCart.DonHang.ThanhToanId = 1;
            detailsCart.DonHang.VanChuyenId = 1;

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.DonHangs.Add(detailsCart.DonHang);
            await _db.SaveChangesAsync();

            detailsCart.DonHang.Total = 0;

            
            foreach(var item in detailsCart.listCart)
            {
                item.Book = await _db.Books.FirstOrDefaultAsync(m => m.Id == item.BookId);

                OrderDetails orderDetails = new OrderDetails();
                orderDetails.BookId = item.BookId;
                orderDetails.DonHangId = detailsCart.DonHang.Id;
                orderDetails.Name = item.Book.Name;
                orderDetails.Price = item.Book.Price;
                orderDetails.Discount = item.Book.Discount;
                orderDetails.Count = item.Count;

                detailsCart.DonHang.Total += (orderDetails.Price - (orderDetails.Price * orderDetails.Discount)) * orderDetails.Count;
                _db.OrderDetails.Add(orderDetails);
            }

            _db.ShoppingCarts.RemoveRange(detailsCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();            

            detailsCart.DonHang.PaymentStatus = SD.PaymentStatusApproved;
            detailsCart.DonHang.Status = SD.StatusSubmitted;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            if(cart.Count==1)
            {
                _db.ShoppingCarts.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCarts.Where(u => u.MaKhachHang == cart.MaKhachHang).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.ShoppingCarts.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCarts.Where(u => u.MaKhachHang == cart.MaKhachHang).ToList().Count();
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);

            return RedirectToAction(nameof(Index));
        }
    }
}