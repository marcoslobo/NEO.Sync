using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Vouts
    {
        public long TransactionId { get; set; }
        public byte[] TransactionHash { get; set; }
        public int N { get; set; }
        public byte[] AddressHash { get; set; }
        public byte[] AssetHash { get; set; }
        public decimal Value { get; set; }
        public DateTime BlockTime { get; set; }
        public bool Claimed { get; set; }
        public bool Spent { get; set; }
        public int StartBlockIndex { get; set; }
        public int? EndBlockIndex { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
