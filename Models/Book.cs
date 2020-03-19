using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        public Book()
        {
            this.OrderDetails = new List<OrderDetails>();
        }

        [Key]
        [Display(Name = "Mã Sách")]
        public int Id { get; set; }

        [Display(Name = "Tên Sách"), Required]
        public string Name { get; set; }

        [Display(Name = "Số Trang"), Required]
        public int SoTrang { get; set; }

        [Display(Name = "Image"), Required]
        public string Image { get; set; }

        [Display(Name = "Price"), Required]
        public double Price { get; set; }

        [Display(Name = "Discount")]
        public double Discount { get; set; }




        [Display(Name = "Mã Thể Loại")]
        public int TheLoaiId { get; set; }
        public virtual TheLoai TheLoai { get; set; }



        [Display(Name = "Mã Nhà Xuất Bản")]
        public int XuatBanId { get; set; }
        public virtual XuatBan XuatBan { get; set; }



        [Display(Name = "Mã Tác Giả")]
        public int TacGiaId { get; set; }
        public virtual TacGia TacGia { get; set; }


        public virtual IList<OrderDetails> OrderDetails { get; set; }

    }
}
