using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class VanChuyen
    {
        public VanChuyen()
        {
            this.DonHangs = new HashSet<DonHang>();
        }

        [Key]
        [Display(Name = "Mã Vận Chuyển")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Công Ty")]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }


        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
