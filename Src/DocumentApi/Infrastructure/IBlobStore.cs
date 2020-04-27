using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace DocumentApi.Infrastructure
{
    public interface IBlobStore
    {
        Task InitAsync();

        Task UpsertAsync(string name, Stream content, BlobHttpHeaders headers = null, Dictionary<string, string> metaData = null);

        Task<bool> DeleteAsync(string name);

        Task<bool> ExistsAsync(string name);

        Task GetContentAsync(string name, Stream destinationStream);

        Uri GetDirectDownloadUri(string name);
    }
}
