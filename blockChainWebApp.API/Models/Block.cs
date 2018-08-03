using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace blockChainWebApp.API.Models
{
    public class Block
    {
        public int Index { get; set; }
        public List<ReceiptPosition> Data { get; set; }
        public string Hash { get; set; }
        public long Timestamp { get; set; }
        public string PreviousHash { get; set; }    
        public string Signature { get; set; }
    }
    public class ReceiptPosition
    {
        public string Product { get; set; }
        public int Price { get; set; }
    }
}