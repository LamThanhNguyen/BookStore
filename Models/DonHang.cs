using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class DonHang
    {
        public DonHang()
        {
            this.OrderDetails = new List<OrderDetails>();
        }
        [Key]
        [Display(Name = "Mã Đơn Hàng")]
        public int Id { get; set; }

        [Display(Name = "Total"), Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Total { get; set; }

        [Display(Name = "Order Time"), Required]
        public DateTime OrderTime { get; set; }


        [Display(Name = "Order Date"),]
        [NotMapped]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Status"), Required]
        public string Status { get; set; }

        [Display(Name = "Payment Status"), Required]
        public string PaymentStatus { get; set; }

        [Display(Name = "Pickup Name")]
        public string PickupName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumer { get; set; }

        public string Comments { get; set; }


        [Display(Name = "Mã Vận Chuyển")]
        public int VanChuyenId { get; set; }
        public virtual VanChuyen VanChuyen { get; set; }




        [Display(Name = "Mã Thanh Toán")]
        public int ThanhToanId { get; set; }
        public virtual ThanhToan ThanhToan { get; set; }




        [Display(Name = "Sales Person")]
        public string KhachHangID { get; set; }
        public virtual KhachHang KhachHang { get; set; }

        public virtual IList<OrderDetails> OrderDetails { get; set; }
    }
}
