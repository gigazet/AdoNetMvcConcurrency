using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConcurrencyTest.Models {

    /// <summary>
    /// Y entity will match abstract Product with name and price.
    /// Should use pessimistic lock.
    /// </summary>
    public class EntityY {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// User name who modifying this object
        /// </summary>
        [Required]
        [Display(Name = "Locked by")]
        public string LockedBy { get; set; }
        public string LockDate { get; set; }
    }
}