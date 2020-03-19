using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class TheLoai
    {
        public TheLoai()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        [Display(Name = "Mã Thể Loại")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên Thể Loại")]
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
