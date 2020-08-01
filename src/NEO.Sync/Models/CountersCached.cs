using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class CountersCached
    {
        public string Name { get; set; }
        public byte[] Ref { get; set; }
        public int Value { get; set; }
    }
}
