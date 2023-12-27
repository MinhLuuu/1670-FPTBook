using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using minh.Data;
using minh.Models;

namespace minh.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        // GET: Books
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index(string searchString, string BookGenre)
        {
            //var applicationDbContext = _context.Book.Include(b => b.Genre);
            //return View(await applicationDbContext.ToListAsync());
            if (_context.Book == null) { }
            IQueryable<string> genreQuery = from m in _context.Book
                                            orderby m.Genre.Name
                                            select m.Genre.Name;
            var books = from m in _context.Book.Include(b => b.Genre)
                        select m;
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Name!.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(BookGenre))
            {
                books = books.Where(s => s.Genre.Name == BookGenre);
            }
            var BookGenreView = new BookGenreView
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Books = await books.ToListAsync(),
                SearchString= searchString,
                BookGenre= BookGenre
            };
            return View(BookGenreView);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", book.GenreId);
            return View(book);
        }

        // GET: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book model)
        {
            if (model.ProfileImage != null)
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = UploadFile(model);
                    Book emp = new Book
                    {
                        Name = model.Name,
                        GenreId = model.GenreId,
                        Price = model.Price,
                        ProfilePicture = uniqueFileName
                    };
                    _context.Add(emp);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", model.GenreId);
            return View();
        }
        private string UploadFile(Book model)
        {
            string uniqueFileName = null;
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filepath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", book.GenreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,GenreId,ProfilePicture,ProfileImage")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (book.ProfileImage != null)
                    {
                        // New image is provided, handle the upload
                        string uniqueFileName = UploadFile(book);
                        book.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        // No new image provided, retain existing image data
                        Book existingBook = _context.Book.AsNoTracking().FirstOrDefault(b => b.Id == id);
                        book.ProfilePicture = existingBook.ProfilePicture;
                        //ModelState.Remove("ProfileImage");
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", book.GenreId);
            return View(book);
        }
        
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
