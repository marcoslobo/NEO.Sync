using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class AddressTransactionBalancesQueue
    {
        public byte[] AddressHash { get; set; }
        public long TransactionId { get; set; }
        public byte[] AssetHash { get; set; }
        public decimal Value { get; set; }
        public DateTime BlockTime { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
