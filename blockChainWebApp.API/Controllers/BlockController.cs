using blockChainWebApp.API.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using blockChainWebApp.API.BLL;
using blockChainWebApp.API.DAL;
using blockChainWebApp.API.BLL.HelpClasses;

namespace blockChainWebApp.API.Controllers
{
    public class BlockController : ApiController
    {
        private readonly BlockRepository _blockRepository = new BlockRepository();

        [HttpPost]
        [Route("api/block/save")]
        public HttpResponseMessage Save([FromBody] Block block)
        {
            try
            {
                var resultCode = HttpStatusCode.Forbidden;

                if (block == null)
                    Request.CreateErrorResponse(resultCode, "Could not read data from body");

                var isValidTransaction = block != null && KeyServices.VerifySignature(block.ReceiptPositions, block.PublicKey, block.Signature);
                if (isValidTransaction)
                {
                    //TODO: перенести логику проверки хеша в BLL
                    if (StringUtil.IsBlockValid(block))
                    {
                        _blockRepository.AddBlock(block);
                        resultCode = HttpStatusCode.OK;
                    }
                }

                var response = new HttpResponseMessage
                {
                    StatusCode = resultCode,
                    Content = new StringContent(JsonConvert.SerializeObject(block), Encoding.UTF8,
                        "application/json")
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

        [HttpGet]
        [Route("api/block/getAll")]
        public HttpResponseMessage GetBlocks()
        {
            try
            {
                var resultCode = HttpStatusCode.Forbidden;

                var result = _blockRepository.GetAllBlocks();
                resultCode = HttpStatusCode.OK;

                var response = new HttpResponseMessage  
                {
                    StatusCode = resultCode,
                    Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                        "application/json")
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