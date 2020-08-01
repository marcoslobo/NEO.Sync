using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class VoutsQueue
    {
        public long? VinTransactionId { get; set; }
        public byte[] TransactionHash { get; set; }
        public int N { get; set; }
        public bool Claimed { get; set; }
        public bool Spent { get; set; }
        public int? EndBlockIndex { get; set; }
        public DateTime BlockTime { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
