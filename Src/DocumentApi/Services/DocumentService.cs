using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using DocumentApi.Dal;

namespace DocumentApi.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentListRepository listRepo;
        private readonly IDocumentRepository documentRepo;

        public DocumentService(IDocumentListRepository listRepo, IDocumentRepository documentRepo)
        {
            this.listRepo = listRepo;
            this.documentRepo = documentRepo;
        }

        public async Task<string> AddAsync(string userFileName, string mimeType, ByteSize size, Stream content)
        {
            string id = await this.documentRepo.AddAsync(mimeType, userFileName, content);

            await AppendToListAsync(id, userFileName, size);

            return id;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            IList<Dal.Document> dalDocuments = await this.listRepo.GetAsync();

            List<Document> allDocuments = dalDocuments
                .Select(dalDocument => new Document(dalDocument)
                    {
                        DownloadUri = this.documentRepo.GetDirectDownloadUri(dalDocument.Id)
                    })
                .ToList();

            return allDocuments;
        }

        public async Task UpdateDocumentOrderAsync(IEnumerable<string> documentIdSetInNewOrder)
        {
            HashSet<string> newIds = new HashSet<string>(documentIdSetInNewOrder);

            if (newIds.Count != documentIdSetInNewOrder.Count())
            {
                throw new ArgumentException("Document id list contains duplicates.");
            }

            IList<Dal.Document> allDocuments = await this.listRepo.GetAsync();

            IEnumerable<string> existingIds = allDocuments.Select(document => document.Id);
            bool noChanges = newIds.SetEquals(existingIds);

            if (!noChanges)
            {
                throw new ArgumentException("Document id ordered set does not correspond to current document list.");
            }

            allDocuments = (
                from id in documentIdSetInNewOrder
                join document in allDocuments on id equals document.Id
                select document
               ).ToList();

            await this.listRepo.UpsertAsync(allDocuments);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            Task<bool>[] removeTasks =
                {
                    this.RemoveFromListAsync(id),
                    this.documentRepo.RemoveAsync(id)
                };

            await Task.WhenAll(removeTasks);

            bool anythingDeleted = removeTasks
                .Select(task => task.Result)
                .Any(isDeleted => isDeleted);

            return anythingDeleted;
        }

        private async Task<bool> RemoveFromListAsync(string id)
        {
            IList<Dal.Document> allDocuments = await this.listRepo.GetAsync();

            Dal.Document document = allDocuments.FirstOrDefault(document => document.Id == id);
            if (document == null)
            {
                return false;
            }

            allDocuments.Remove(document);

            await this.listRepo.UpsertAsync(allDocuments);

            return true;
        }

        private async Task AppendToListAsync(string id, string userFileName, ByteSize size)
        {
            IList<Dal.Document> allDocuments = await this.listRepo.GetAsync();

            allDocuments.Add(new Dal.Document
                {
                    Id = id,
                    Name = userFileName,
                    SizeInBytes = size.Bytes
                });

            await this.listRepo.UpsertAsync(allDocuments);
        }
    }
}
