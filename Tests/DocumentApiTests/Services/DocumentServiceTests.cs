using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using ByteSizeLib;
using DocumentApi.Dal;
using DocumentApi.Services;
using FluentAssertions;
using Moq;
using Xunit;

using DalDocument = DocumentApi.Dal.Document;

namespace DocumentApi.Tests.Services
{
    public class DocumentServiceTests
    {
        [Theory]
        [AutoMoqData]
        public async Task AddAsync_GivenValidInput_CreatesDocument(
            [Frozen] Mock<IDocumentRepository> documentRepoMock,
            DocumentService sut,
            string userFileName,
            string mimeType,
            ByteSize size,
            string newDocumentId,
            MemoryStream content) 
        {
            using (content)
            {
                // Arrange
                documentRepoMock.Setup(repo => repo.AddAsync(mimeType, userFileName, content))
                    .ReturnsAsync(newDocumentId)
                    .Verifiable();

                // Act
                string documentId = await sut.AddAsync(userFileName, mimeType, size, content);

                // Verify
                documentRepoMock.Verify();
                documentId.Should().Be(documentId);
            }
        }

        [Theory]
        [AutoMoqData]
        public async Task AddAsync_GivenValidInput_AppendsDocumentToList(
            [Frozen] Mock<IDocumentRepository> documentRepoMock,
            [Frozen] Mock<IDocumentListRepository> documentListRepoMock,
            DocumentService sut,
            string userFileName,
            string mimeType,
            ByteSize size,
            string newDocumentId, 
            [Frozen]Mock<IList<DalDocument>> existingDocuments,
            MemoryStream content)
        {
            using (content)
            {
                // Arrange
                documentRepoMock.Setup(repo => 
                        repo.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                    .ReturnsAsync(newDocumentId);

                documentListRepoMock.Setup(repo => repo.GetAsync())
                    .ReturnsAsync(existingDocuments.Object);

                // Act
                await sut.AddAsync(userFileName, mimeType, size, content);

                // Verify
                DalDocument expectedDocument = new DalDocument
                    { 
                        Id = newDocumentId,
                        Name = userFileName,
                        SizeInBytes = size.Bytes
                    };
                existingDocuments.Verify(list => list.Add(It.Is<DalDocument>(actualDocument => Equals(actualDocument, expectedDocument))));
            }
        }

        private static bool Equals(DalDocument doc1, DalDocument doc2) 
        {
            bool equals = 
                (   (doc1.Id == doc2.Id) 
                 && (doc1.Name == doc2.Name) 
                 && (doc1.SizeInBytes == doc2.SizeInBytes)
                );
            return equals;
        }
    }
}
