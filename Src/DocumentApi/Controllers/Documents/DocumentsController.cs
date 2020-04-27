using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using DocumentApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentApi.Controllers.Documents
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService documentService;

        public DocumentsController(IDocumentService documentService) 
        {
            this.documentService = documentService;
        }

        [HttpGet]
        public async Task<IEnumerable<DocumentView>> GetAllDocuments()
        {
            IEnumerable<Document> documents = await this.documentService.GetAllAsync();
            return documents.Select(document => new DocumentView(document));
        }

        [HttpPost]
        public Task AddDocument([FromForm] AddFileRequest request)
        {
            return this.documentService.AddAsync(request.File.FileName, request.File.ContentType, ByteSize.FromBytes(request.File.Length), request.File.OpenReadStream());
        }

        [HttpPut("Order")]
        public async Task<IActionResult> UpdateDocumentOrder([FromBody]UpdateOrderRequest request)
        {
            try
            {
                await this.documentService.UpdateDocumentOrderAsync(request.DocumentIds);
            }
            catch (ArgumentException ex) 
            {
                return this.BadRequest(ex.Message);
            }

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            bool foundAndDeleted = await this.documentService.RemoveAsync(id);

            IActionResult result = foundAndDeleted 
                ? this.Ok() 
                : (IActionResult)this.NotFound();

            return result;
        }
    }
}
