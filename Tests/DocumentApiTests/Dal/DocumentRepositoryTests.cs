using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Azure.Storage.Blobs.Models;
using DocumentApi.Dal;
using DocumentApi.Infrastructure;
using Moq;
using Xunit;

namespace DocumentApi.Tests.Dal
{
    public class DocumentRepositoryTests
    {
        [Theory]
        [AutoMoqData]
        public async Task AddAsync_GivenVaidInput_CorectlyUpdatesBlob(
            [Frozen] Mock<IBlobStore> blobStoreMock,
            DocumentRepository sut,
            string mimeType, 
            string userFileName,
            MemoryStream content) 
        {
            using (content) 
            {
                // Arrange // Act 
                string id = await sut.AddAsync(mimeType, userFileName, content);

                // Verify
                BlobHttpHeaders expectedHeaders = new BlobHttpHeaders()
                    {
                        ContentType = mimeType,
                        ContentDisposition = $"attachment; filename = \"{userFileName}\""
                    };

                blobStoreMock.Verify(store => 
                store.UpsertAsync(id, content, It.Is<BlobHttpHeaders>(actualHeaders => Equals(actualHeaders, expectedHeaders)), null),
                    Times.Once);
            }
        }

        private static bool Equals(BlobHttpHeaders headers1, BlobHttpHeaders headers2)
        {
            bool equals =
                (   (headers1.CacheControl == headers2.CacheControl)
                 && (headers1.ContentDisposition == headers2.ContentDisposition)
                 && (headers1.ContentEncoding == headers2.ContentEncoding)
                 && (headers1.ContentHash == headers2.ContentHash)
                 && (headers1.ContentLanguage == headers2.ContentLanguage)
                 && (headers1.ContentType == headers2.ContentType)             
                );
            return equals;
        }
    }
}
