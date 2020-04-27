using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentApi.Dal
{
    public interface IDocumentListRepository
    {
        Task UpsertAsync(IList<Document> allDocuments);

        Task<bool> ExistsAsync();

        Task<IList<Document>> GetAsync();       
    }
}
