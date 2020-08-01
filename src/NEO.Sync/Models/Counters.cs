using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Counters
    {
        public string Name { get; set; }
        public byte[] Ref { get; set; }
        public long? Value { get; set; }
    }
}
