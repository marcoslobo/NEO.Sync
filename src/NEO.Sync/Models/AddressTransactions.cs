using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class AddressTransactions
    {
        public byte[] AddressHash { get; set; }
        public long TransactionId { get; set; }
        public DateTime BlockTime { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
