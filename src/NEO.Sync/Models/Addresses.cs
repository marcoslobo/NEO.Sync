using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Addresses
    {
        public byte[] Hash { get; set; }
        public DateTime FirstTransactionTime { get; set; }
        public DateTime LastTransactionTime { get; set; }
        public int TxCount { get; set; }
        public int AtbCount { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
