using System;
using System.Collections.Generic;

namespace WarehousesEditor.Models
{
    public partial class GoodsCategory
    {
        public int GoodsId { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Goods Goods { get; set; }
    }
}
