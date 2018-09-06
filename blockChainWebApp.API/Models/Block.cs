namespace blockChainWebApp.API.Models
{
    public class Block
    {
        public int Index { get; set; }
        public string ReceiptPositions { get; set; }
        public string Hash { get; set; }
        public long Timestamp { get; set; }
        public string PreviousHash { get; set; }    
        public string Signature { get; set; }
        public string PublicKey { get; set; }
    }
}