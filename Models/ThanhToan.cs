using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ThanhToan
    {
        public ThanhToan()
        {
            this.DonHangs = new HashSet<DonHang>();
        }

        [Key]
        [Display(Name = "Mã Thanh Toán")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kiểu Thanh Toán")]
        public string Name { get; set; }
        public string Allowed { get; set; }


        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
