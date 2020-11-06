using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousesEditor.Models
{
    public partial class WarehouseGoods
    {
        [Required]
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }

        [Required]
        [Display(Name = "Goods")]
        public int GoodsId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Amount { get; set; }

        public virtual Goods Goods { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
