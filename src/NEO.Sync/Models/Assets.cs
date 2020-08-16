using System;

namespace NEO.Api.Models
{
    public partial class Assets
    {

        public long Id { get; set; }
        public long? TransactionId { get; set; }
        public string Hash { get; set; }
        public Wallet AdminWallet { get; set; }
        public long AdminWalletId { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public Wallet OwnerWallet { get; set; }
        public long OwnerWalletId { get; set; }
        public int Precision { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public decimal? Issued { get; set; }        
        public DateTime InsertedAt { get; set; }        

    }
}
