using blockChainWebApp.API.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace blockChainWebApp.API.Controllers
{
    public class BlockController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GenerateKeys()
        {
            try
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(HttpStatusCode.OK.ToString(), Encoding.UTF8, "application/json")
                };

                return response;
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return response;
            }
        }
        [HttpPost]
        [Route("api/block/save")]
        public HttpResponseMessage Save([FromBody] Block block)
        {
            try
            {
                if (block == null)
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not read data from body");

                var hash = block.Hash;
                var signature = block.Signature;
                var previousHash = block.PreviousHash;
                var timestamp = block.Timestamp;
                var blockData = block.Data.ToString();

                var insertNewUser = "INSERT INTO Blocks(Hash,PreviousHash,Signature,BlockData,Timestamp) VALUES (@Hash,@PreviousHash,@Signature,@BlockData,@TimeStamp)";
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AzureSQLConnection"].ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand(insertNewUser, conn);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@Signature", signature);
                    cmd.Parameters.AddWithValue("@PreviousHash", previousHash);
                    cmd.Parameters.AddWithValue("@Timestamp", timestamp);
                    cmd.Parameters.AddWithValue("@BlockData", blockData);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                var response = new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonConvert.SerializeObject(block), Encoding.UTF8, "application/json")
                    };
                return response;
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Content = new StringContent(ex.Message, Encoding.UTF8, "application/json")
                };
                return response;
            }
        }
    }
}