using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using blockChainWebApp.API.BLL.HelpClasses;

namespace blockChainWebApp.API.Models
{
    public class HashCalculator
    {
        public int Nonce { get; set; }

        public static string CalculateHash(Block block, int nonce)
        {
            var calculatedhash = StringUtil.ApplySha256(
                block.PreviousHash +
                block.Timestamp +
                nonce +
                block.ReceiptPositions
            );
            return calculatedhash;
        }
    }
}