using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.WarehousesSection
{
    public class AddGoodsModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public AddGoodsModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int id)
        {
            ViewData["GoodsId"] = new SelectList(_context.Goods, "GoodsId", "GoodsName");
            if (id == null)
            {
                ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseName");
            }
            else ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "WarehouseId", "WarehouseName", id);

            return Page();
        }

        [BindProperty]
        public WarehouseGoods WarehouseGoods { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var temp = _context.WarehousesGoods.FirstOrDefault(x => x.GoodsId == WarehouseGoods.GoodsId && x.WarehouseId == WarehouseGoods.WarehouseId);

            if (temp != null)
            {
                ModelState.AddModelError("GoodsId", "These goods already exist in this warehouse");
                return OnGet(WarehouseGoods.WarehouseId);
            }

            _context.WarehousesGoods.Add(WarehouseGoods);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Warehouse", new { id = WarehouseGoods.WarehouseId } );
        }
    }
}
