using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehousesEditor.Models
{
    public partial class Category
    {
        public Category()
        {
            GoodsCategories = new HashSet<GoodsCategory>();
        }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Name")]
        public string CategoryName { get; set; }

        public virtual ICollection<GoodsCategory> GoodsCategories { get; set; }
    }
}
