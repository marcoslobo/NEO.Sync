using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class Transactions
    {
        public long Id { get; set; }        
        public string Hash { get; set; }        
        public Blocks Block { get; set; }
        public long BlockId { get; set; }        
        public decimal NetFee { get; set; }
        public decimal SysFee { get; set; }
        public long? Nonce { get; set; }        
        public int Size { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public int N { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        internal void BindBlock(Blocks block)
        {
            if (block != null)
            {
                Block = block;
                BlockId = block.Index;
            }
        }
    }
}
