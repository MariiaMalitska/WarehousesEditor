using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousesEditor.Models
{
    public partial class Warehouse
    {
        public Warehouse()
        {
            WarehousesGoods = new HashSet<WarehouseGoods>();
        }

        public int WarehouseId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string WarehouseName { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "The address must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Goods (In Warehouse)")]
        public virtual ICollection<WarehouseGoods> WarehousesGoods { get; set; }
    }
}
