using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class OrderDetails
    {

        [Key]
        public int Id { get; set; }


        public int DonHangId { get; set; }
        public virtual DonHang DonHang { get; set; }



        public int BookId { get; set; }
        public virtual Book Book { get; set; }



        public int Count { get; set; }

        public string Name { get; set; }


        [Display(Name = "Price"), Required]
        public double Price { get; set; }


        [Display(Name = "Discount")]
        public double Discount { get; set; }
    }
}
