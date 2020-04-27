using System.ComponentModel.DataAnnotations;
using Azure.Storage.Blobs.Models;

namespace DocumentApi.Configuration
{
    public class BlobStorageConfiguration
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string ContainerName { get; set; }

        public PublicAccessType PublicAccessType { get; set; }
    }
}
