using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class AddressBalancesQueue
    {
        public byte[] AddressHash { get; set; }
        public byte[] AssetHash { get; set; }
        public decimal Value { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
