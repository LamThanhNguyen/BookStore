using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModel;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IEmailSender _emailSender;
        private BookStoreContext _db;
        private int PageSize = 2;

        public OrderController(BookStoreContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                DonHang = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.VanChuyen).Include(o => o.ThanhToan).FirstOrDefaultAsync(o => o.Id == id && o.KhachHangID == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.DonHangId == id).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrderStatus(int? Id)
        {
            return PartialView("_OrderStatus", _db.DonHangs.Where(m => m.Id == Id).FirstOrDefault().Status);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<DonHang> donHangsList = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.ThanhToan).Include(o => o.VanChuyen).Where(u => u.KhachHangID == claim.Value).ToListAsync();

            foreach(DonHang item in donHangsList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    DonHang = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.DonHangId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.DonHang.Id)
                                    .Skip((productPage - 1) * PageSize)
                                    .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.SuperAdminEndUser)]
        public async Task<IActionResult> ManageOrder(int productPage = 1)
        {
            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();

            List<DonHang> donHangsList = await _db.DonHangs.Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess).OrderByDescending(u => u.OrderTime).ToListAsync();

            foreach(DonHang item in donHangsList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    DonHang = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.DonHangId == item.Id).ToListAsync()
                };
                orderDetailsVM.Add(individual);
            }


            return View(orderDetailsVM.OrderBy(o => o.DonHang.OrderTime).ToList());
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                DonHang = await _db.DonHangs.FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.DonHangId == Id).ToListAsync()
            };
            orderDetailsViewModel.DonHang.KhachHang = await _db.KhachHangs.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.DonHang.KhachHangID);
            orderDetailsViewModel.DonHang.ThanhToan = await _db.ThanhToans.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.DonHang.ThanhToanId);
            orderDetailsViewModel.DonHang.VanChuyen = await _db.VanChuyens.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.DonHang.VanChuyenId);

            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }

        [Authorize(Roles = SD.SuperAdminEndUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            DonHang donHang = await _db.DonHangs.FindAsync(OrderId);
            donHang.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.SuperAdminEndUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            DonHang donHang = await _db.DonHangs.FindAsync(OrderId);
            donHang.Status = SD.StatusReady;
            await _db.SaveChangesAsync();

            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == donHang.KhachHangID).FirstOrDefault().Email, "Book - Order Ready for Pickup " + donHang.Id.ToString(), "Order is ready for pickup");

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.SuperAdminEndUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            DonHang donHang = await _db.DonHangs.FindAsync(OrderId);
            donHang.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == donHang.KhachHangID).FirstOrDefault().Email, "Book - Order Cancelled " + donHang.Id.ToString(), "Order has been cancelled successfully.");

            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize]
        public async Task<IActionResult> OrderPickUp(int ProductPage = 1, string searchEmail=null, string searchPhone=null, string searchName=null)
        {
            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if(searchName!=null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail");
            if(searchEmail !=null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if(searchPhone !=null)
            {
                param.Append(searchPhone);
            }

            List<DonHang> donHangsList = new List<DonHang>();
            if(searchName != null || searchEmail != null || searchPhone != null)
            {
                var user = new KhachHang();

                if(searchName != null)
                {
                    donHangsList = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.VanChuyen).Include(o => o.ThanhToan)
                                            .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                                            .OrderByDescending(o => o.OrderDate).ToListAsync();
                }
                else
                {
                    if(searchEmail != null)
                    {
                        user = await _db.KhachHangs.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        donHangsList = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.VanChuyen).Include(o => o.ThanhToan)
                                                    .Where(u => u.KhachHangID == user.Id)
                                                    .OrderByDescending(o => o.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if(searchPhone != null)
                        {
                            donHangsList = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.VanChuyen).Include(o => o.ThanhToan)
                                                        .Where(u => u.PhoneNumer.Contains(searchPhone))
                                                        .OrderByDescending(o => o.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                donHangsList = await _db.DonHangs.Include(o => o.KhachHang).Include(o => o.VanChuyen).Include(o => o.ThanhToan).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }

            foreach(DonHang item in donHangsList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    DonHang = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.DonHangId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.DonHang.Id)
                                    .Skip((ProductPage - 1) * PageSize)
                                    .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = ProductPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.SuperAdminEndUser)]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int orderId)
        {
            DonHang donHang = await _db.DonHangs.FindAsync(orderId);
            donHang.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == donHang.KhachHangID).FirstOrDefault().Email, "Book - Order Completed " + donHang.Id.ToString(), "Order has been completed successfully.");


            return RedirectToAction("OrderPickup", "Order");
        }
    }
}