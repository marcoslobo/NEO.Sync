using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class TransactionAssets
    {
        public long TransactionId { get; set; }
        public byte[] AssetHash { get; set; }
    }
}
