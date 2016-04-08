using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConcurrencyTest.Models {
    /// <summary>
    /// X entity will match "Product".
    /// Should use optimistic lock.
    /// </summary>
    public class EntityX {
       
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
       
        /// <summary>
        /// Latest row version
        /// </summary>
        public Guid RowVersion { get; set; }
    }

    
}