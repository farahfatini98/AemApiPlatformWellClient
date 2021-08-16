using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformWellApiConsumption
{
    public class Well
    {
        public int id { get; set; }
        public int platformId { get; set; }
        public string uniqueName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }



    }
}
