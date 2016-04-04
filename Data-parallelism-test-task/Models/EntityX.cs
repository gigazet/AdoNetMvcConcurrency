using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataParallelismTest.Models {
    /// <summary>
    /// X entity will match "Product".
    /// Should use optimistic lock.
    /// </summary>
    public class EntityX {
        [Key]
        public int Id;
        public string Name;
        public decimal Price;
        /// <summary>
        /// Last user name who modified this object
        /// </summary>
        public string LastEditor;
        /// <summary>
        /// Latest row version
        /// </summary>
        public int RowVersion;
    }

    /// <summary>
    /// Y entity will match "Customer".
    /// Should use pessimistic lock.
    /// </summary>
    public class EntityY {
        [Key]
        public int Id;
        public string FullName;
        public string Email;
        /// <summary>
        /// Last user name who modified this object
        /// </summary>
        public string LockedBy;
    }
}