using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.GoodsSection
{
    public class IndexModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public IndexModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        public IList<Goods> Goods { get;set; }

        public IList<GoodsCategory> GoodsCategories { get; set; }

        public async Task OnGetAsync()
        {
            Goods = await _context.Goods
                .Include(g => g.Currency).Include(g=>g.GoodsCategories).ToListAsync();

            GoodsCategories = await _context.GoodsCategories
                .Include(g => g.Goods).Include(g => g.Category).ToListAsync();
        }

        public List<GoodsCategory> GetCategories(int goodsId)
        {
            return GoodsCategories.Where(x => x.GoodsId == goodsId).ToList();
        }
    }
}
