using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModel
{
    public class OrderDetailsViewModel
    {
        public DonHang DonHang { get; set; }

        public IEnumerable<VanChuyen> VanChuyens { get; set; }

        public IEnumerable<ThanhToan> ThanhToans { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }
}
