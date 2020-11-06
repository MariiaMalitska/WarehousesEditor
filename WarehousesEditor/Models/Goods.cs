using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousesEditor.Models
{
    public partial class Goods
    {
        public Goods()
        {
            GoodsCategories = new HashSet<GoodsCategory>();
            WarehousesGoods = new HashSet<WarehouseGoods>();
        }

        public int GoodsId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string GoodsName { get; set; }

        [Required]
        [Display(Name = "Price (Base Currency)")]
        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal BaseCurrencyPrice { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.000}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }

        [Display(Name = "Barcode Number")]
        public string BarcodeNumber { get; set; }

        [Display(Name = "Currency")]
        public virtual Currency Currency { get; set; }

        [Display(Name = "Categories")]
        public virtual ICollection<GoodsCategory> GoodsCategories { get; set; }

        [Display(Name = "Warehouses")]
        public virtual ICollection<WarehouseGoods> WarehousesGoods { get; set; }
    }
}
