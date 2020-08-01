using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Blocks
    {
        public Blocks()
        {
            Transactions = new List<Transactions>();
        }

        public string Hash { get; set; }
        public long Index { get; set; }
        public string MerkleRoot { get; set; }
        public string NextConsensus { get; set; }
        public string Nonce { get; set; }
        public string Script { get; set; }
        public int Size { get; set; }
        public DateTime Time { get; set; }
        public int Version { get; set; }
        public int TxCount { get; set; }
        public decimal TotalSysFee { get; set; }
        public decimal TotalNetFee { get; set; }
        public decimal? CumulativeSysFee { get; set; }
        public decimal GasGenerated { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Transactions> Transactions { get; set; }

    }
}
