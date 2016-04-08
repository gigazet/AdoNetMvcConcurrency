using System;
using System.Linq;

namespace ConcurrencyTest.Models {

    /// <summary>
    /// Y entity will match "Customer".
    /// Should use pessimistic lock.
    /// </summary>
    public class EntityY {
        public int Id;
        public string FullName;
        public string Email;
        /// <summary>
        /// Last user name who modified this object
        /// </summary>
        public string LockedBy;
    }
}