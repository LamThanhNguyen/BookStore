using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModel
{
    public class BooksViewModel
    {
        public Book Book { get; set; }

        public IEnumerable<TheLoai> TheLoais { get; set; }

        public IEnumerable<TacGia> TacGias { get; set; }

        public IEnumerable<XuatBan> XuatBans { get; set; }       
    }
}
