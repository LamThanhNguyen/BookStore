using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class TacGia
    {
        public TacGia()
        {
            this.Books = new HashSet<Book>();
        }
        [Key]
        [Display(Name = "Mã Tác Giả")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Tác Giả")]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
