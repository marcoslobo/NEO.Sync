using System;

namespace EvoLua.Blockchain.Models
{
    public partial class Transfers
    {
        public long Id { get; set; }
        public Wallet AddressFrom { get; set; }
        public long AddressFromId { get; set; }
        public Wallet AddressTo { get; set; }
        public long AddressToId { get; set; }
        public decimal Amount { get; set; }        
        public int BlockIndex { get; set; }
        public DateTime BlockTime { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Assets Asset { get; set; }
        public long AssetId { get; set; }
    }
}
