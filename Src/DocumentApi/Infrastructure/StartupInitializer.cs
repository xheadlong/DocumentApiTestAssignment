using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentApi.Dal;

namespace DocumentApi.Infrastructure
{
    public class StartupInitializer : IStartupInitializer
    {
        private readonly IDictionary<string, IBlobStore> blobStoreRegistry;
        private readonly IDocumentListRepository listRepo;

        public StartupInitializer(IDictionary<string, IBlobStore> blobStoreRegistry, IDocumentListRepository listRepo)
        {
            this.blobStoreRegistry = blobStoreRegistry;
            this.listRepo = listRepo;
        }

        public async Task InitAsync()
        {
            try
            {   
                await Task.WhenAll(blobStoreRegistry.Values
                        .Select(blobStore => blobStore.InitAsync()));

                bool listExists = await this.listRepo.ExistsAsync();

                if (!listExists)
                {
                    await this.listRepo.UpsertAsync(new List<Dal.Document>(0));
                }
            }
            catch (Exception ex) 
            {
                throw new Exception("Failed to initialize storage.", ex);
            }
        }
    }
}
