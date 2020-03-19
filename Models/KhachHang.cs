using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class KhachHang : IdentityUser
    {
        public KhachHang()
        {
            this.DonHangs = new HashSet<DonHang>();
        }


        [Required]
        [Display(Name = "Sales Person")]
        public string Name { get; set; }

        public string Class { get; set; }

        [Required]
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [Required]
        public string ShipAddress { get; set; }




        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
