using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string MaKhachHang { get; set; }

        [NotMapped]
        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; }

        public int BookId { get; set; }

        [NotMapped]
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to {1}")]
        public int Count { get; set; }


    }
}
