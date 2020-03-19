using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class XuatBan
    {
        public XuatBan()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        [Display(Name = "Mã Nhà Xuất Bản")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Nhà Xuất Bản")]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
