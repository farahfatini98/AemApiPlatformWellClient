﻿using System;
using System.Collections.Generic;

#nullable disable

namespace PlatformWellApiConsumption.AemModels
{
    public partial class Well
    {
        public int Id { get; set; }
        public int? PlatformId { get; set; }
        public string UniqueName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
