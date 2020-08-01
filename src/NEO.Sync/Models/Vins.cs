using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Vins
    {
        public long TransactionId { get; set; }
        public byte[] VoutTransactionHash { get; set; }
        public int VoutN { get; set; }
        public int N { get; set; }
        public int BlockIndex { get; set; }
        public DateTime BlockTime { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
