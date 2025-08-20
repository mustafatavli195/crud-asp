using asp_core_crud.Data;
using asp_core_crud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace asp_core_crud.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Get products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Get: /Product/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        //Post: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int CategoryId, int[] SelectedTags, string Description, string Manufacturer)
        {
            if (ModelState.IsValid)
            {
                product.CategoryId = CategoryId;

                if (SelectedTags != null && SelectedTags.Length > 0)
                {
                    product.Tags = new List<Tag>();
                    foreach (var tagId in SelectedTags)
                    {
                        var tag = await _context.Tags.FindAsync(tagId);
                        if (tag != null)
                            product.Tags.Add(tag);
                    }
                }

                product.ProductDetail = new ProductDetail
                {
                    Description = Description,
                    Manufacturer = Manufacturer
                };

                _context.ProductDetails.Add(product.ProductDetail); // EKLENDİ 🔥

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Tags = _context.Tags.ToList();
            return View(product);
        }

        //Get: /Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        //Get: /Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        //Post: /Produt/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //Get: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Tags)
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Tags = _context.Tags.ToList();

            return View(product);
        }

        //Post: /Product/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CategoryId")] Product product, int[] SelectedTags, string Description, string Manufacturer)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products
                        .Include(p => p.Tags)
                        .Include(p => p.ProductDetail)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null) return NotFound();

                    // Temel alanlar
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryId = product.CategoryId;

                    // Etiketleri güncelle (önce temizle)
                    existingProduct.Tags.Clear();
                    if (SelectedTags != null && SelectedTags.Length > 0)
                    {
                        foreach (var tagId in SelectedTags)
                        {
                            var tag = await _context.Tags.FindAsync(tagId);
                            if (tag != null)
                                existingProduct.Tags.Add(tag);
                        }
                    }

                    // ProductDetail güncelle veya oluştur
                    if (existingProduct.ProductDetail == null)
                        existingProduct.ProductDetail = new ProductDetail();

                    existingProduct.ProductDetail.Description = Description;
                    existingProduct.ProductDetail.Manufacturer = Manufacturer;

                    // Save Changes
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Kaydetme sırasında hata oluştu: " + ex.InnerException?.Message);
                }
            }

            // Hata durumunda ViewBag doldur
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Tags = _context.Tags.ToList();

            // Modeli tam olarak geri ver (ProductDetail ve Tags dahil)
            var productForView = await _context.Products
                .Include(p => p.Tags)
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == id);

            return View(productForView);
        }


    }
}
