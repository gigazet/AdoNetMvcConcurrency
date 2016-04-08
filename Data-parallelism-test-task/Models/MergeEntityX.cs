using System;
using System.Linq;

namespace ConcurrencyTest.Models {

    public class MergeEntityX {
        public EntityX NewItem { get; set; }
        public EntityX SavedItem { get; set; }

        public bool UpdateName { get; set; } = true;
        public bool UpdatePrice { get; set; } = true;
    }
}