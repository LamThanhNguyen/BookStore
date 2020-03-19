using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> listCart { get; set; }

        public DonHang DonHang { get; set; }
    }
}
