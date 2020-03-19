using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using System.IO;
using BookStore.Utility;
using Microsoft.AspNetCore.Hosting.Internal;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly HostingEnvironment _hostingEnvironment;


        public BooksController(BookStoreContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

        }

        // GET: Admin/Books
        public async Task<IActionResult> Index()
        {
            var bookStoreContext = _context.Books.Include(b => b.TacGia).Include(b => b.TheLoai).Include(b => b.XuatBan);
            return View(await bookStoreContext.ToListAsync());
        }

        // GET: Admin/Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.TacGia)
                .Include(b => b.TheLoai)
                .Include(b => b.XuatBan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Admin/Books/Create
        public IActionResult Create()
        {
            ViewData["TacGiaId"] = new SelectList(_context.TacGias, "Id", "Name");
            ViewData["TheLoaiId"] = new SelectList(_context.TheLoais, "Id", "Name");
            ViewData["XuatBanId"] = new SelectList(_context.XuatBans, "Id", "Name");
            return View();
        }

        // POST: Admin/Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SoTrang,Image,Price,Discount,TheLoaiId,XuatBanId,TacGiaId")] Book book)
        {
            _context.Add(book);
            //await _context.SaveChangesAsync();
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, book.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                book.Image = @"\images\" + book.Id + extension;
            }
            else
            {
                //no file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultBookImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + book.Id + ".png");
                book.Image = @"\images\" + book.Id + ".png";
            }
            //_context.Add(book);
            //await _context.SaveChangesAsync();
            //string webRootPath = _hostingEnvironment.WebRootPath;
            //var files = HttpContext.Request.Form.Files;

            //var productsFromDb = _context.Books.Find(book.Id);

            //if (files.Count != 0)
            //{
            //    //Image has been uploaded
            //    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
            //    var extension = Path.GetExtension(files[0].FileName);

            //    using (var filestream = new FileStream(Path.Combine(uploads, book.Id + extension), FileMode.Create))
            //    {
            //        files[0].CopyTo(filestream);
            //    }
            //    productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + book.Id + extension;
            //}
            //else
            //{
            //    //when user does not upload image
            //    var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultBookImage);
            //    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + book.Id + ".png");
            //    productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + book.Id + ".png";
            //}
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            //ViewData["TacGiaId"] = new SelectList(_context.TacGias, "Id", "Name", book.TacGiaId);
            //ViewData["TheLoaiId"] = new SelectList(_context.TheLoais, "Id", "Name", book.TheLoaiId);
            //ViewData["XuatBanId"] = new SelectList(_context.XuatBans, "Id", "Name", book.XuatBanId);
            //return View(book);
        }

        // GET: Admin/Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["TacGiaId"] = new SelectList(_context.TacGias, "Id", "Name", book.TacGiaId);
            ViewData["TheLoaiId"] = new SelectList(_context.TheLoais, "Id", "Name", book.TheLoaiId);
            ViewData["XuatBanId"] = new SelectList(_context.XuatBans, "Id", "Name", book.XuatBanId);
            return View(book);
        }

        // POST: Admin/Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SoTrang,Image,Price,Discount,TheLoaiId,XuatBanId,TacGiaId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            _context.Update(book);

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var bookfromDB = await _context.Books.FindAsync(book.Id);



            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                //var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                //if (System.IO.File.Exists(imagePath))
                //{
                //    System.IO.File.Delete(imagePath);
                //}

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, book.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                bookfromDB.Image = @"\images\" + book.Id + extension_new;
            }
            else
            {
                book.Image = @"\images\" + book.Id + ".png";
            }
            await _context.SaveChangesAsync();

            if (!BookExists(book.Id))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));

            //ViewData["TacGiaId"] = new SelectList(_context.TacGias, "Id", "Name", book.TacGiaId);
            //ViewData["TheLoaiId"] = new SelectList(_context.TheLoais, "Id", "Name", book.TheLoaiId);
            //ViewData["XuatBanId"] = new SelectList(_context.XuatBans, "Id", "Name", book.XuatBanId);
            //return View(book);
        }

        // GET: Admin/Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.TacGia)
                .Include(b => b.TheLoai)
                .Include(b => b.XuatBan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
