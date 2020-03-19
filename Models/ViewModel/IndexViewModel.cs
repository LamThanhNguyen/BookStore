using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        public IEnumerable<TheLoai> TheLoais { get; set; }
    }
}
