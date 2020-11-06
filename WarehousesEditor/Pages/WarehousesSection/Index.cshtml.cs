using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WarehousesEditor.Models;

namespace WarehousesEditor.Pages.WarehousesSection
{
    public class IndexModel : PageModel
    {
        private readonly WarehousesEditor.Models.WarehouseDbContext _context;

        public IndexModel(WarehousesEditor.Models.WarehouseDbContext context)
        {
            _context = context;
        }

        public IList<Warehouse> Warehouse { get;set; }

        public async Task OnGetAsync()
        {
            Warehouse = await _context.Warehouses.ToListAsync();
        }
    }
}
