using System;
using System.Collections.Generic;

namespace NEO.Api.Models
{
    public partial class BlocksMeta
    {
        public int Id { get; set; }
        public int? Index { get; set; }
        public decimal? CumulativeSysFee { get; set; }
    }
}
