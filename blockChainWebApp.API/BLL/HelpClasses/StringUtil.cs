using System.Security.Cryptography;
using System.Text;
using blockChainWebApp.API.Models;

namespace blockChainWebApp.API.BLL.HelpClasses
{
    public class StringUtil
    {
        private const int Difficulty = 3;

        public static string ApplySha256(string input)
        {
            var digest = SHA1.Create();
            var outputBytes = digest.ComputeHash(Encoding.ASCII.GetBytes(input));
            var hexString = new StringBuilder();

            foreach (var outputByte in outputBytes)
            {
                var hex = outputByte.ToString("x2");
                if (hex.Length == 1) hexString.Append('0');
                hexString.Append(hex);
            }
            return hexString.ToString();
        }

        //TODO: переместить в BLL
        public static bool IsBlockValid(Block currentBlock)
        {
            //compare registered hash and calculated hash:
            //TODO: nonce надо достать из последного блока из БД
            if (!currentBlock.Hash.Equals(HashCalculator.CalculateHash(currentBlock, 0)))
            {
                return false;
            }

            //check if hash is solved - перевод на русский
            var hashTarget = new string(new char[Difficulty]).Replace('\0', '0');
            return currentBlock.Hash.Substring(0, Difficulty).Equals(hashTarget);
        }
    }
}