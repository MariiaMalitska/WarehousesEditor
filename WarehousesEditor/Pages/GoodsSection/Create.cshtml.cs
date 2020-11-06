using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using WarehousesEditor.Helpers;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.GoodsSection
{
    public class CreateModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;
        private readonly BarcodeGenerator _barcodeGenerator;

        public CreateModel(WarehousesEditor.Models.WarehouseDbContext context, BarcodeGenerator barcodeGenerator)
        {
            _context = context;
            _barcodeGenerator = barcodeGenerator;
        }

        public IActionResult OnGet()
        {
            var categories = _context.Categories.Select(c =>
                                  new {
                                      c.CategoryId,
                                      c.CategoryName
                                  }).ToList();

            ViewData["Categories"] = new MultiSelectList(categories, "CategoryId", "CategoryName");
            ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "Code");

            return Page();
        }

        [BindProperty]
        public Goods Goods { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var barcodes = _context.Goods.Select(x=>x.BarcodeNumber).ToList();

            // generating random unique barcode number
            string barcode = _barcodeGenerator.GenerateBarcode();
            while (barcodes.Any( x => x == barcode )) 
            {
                barcode = _barcodeGenerator.GenerateBarcode();
            }

            Goods.BarcodeNumber = barcode;

            _context.Goods.Add(Goods);
            await _context.SaveChangesAsync();

            int newGoodsId = Goods.GoodsId;

            // adding categories
            var categories = Request.Form["Categories"];
            foreach (var categoryId in categories)
            {
                _context.GoodsCategories.Add(new GoodsCategory() { CategoryId = int.Parse(categoryId), GoodsId = newGoodsId });
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
