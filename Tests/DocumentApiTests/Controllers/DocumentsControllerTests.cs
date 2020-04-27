using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using ByteSizeLib;
using DocumentApi.Controllers.Documents;
using DocumentApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DocumentApi.Tests.Controllers
{
    public class DocumentsControllerTests
    {
        [Theory]
        [AutoMoqData]
        public async Task AddDocument_GivenCorrectInput_AddsDocument(
            [Frozen] Mock<IDocumentService> documentServiceMock,
            [Frozen] Mock<IFormFile> formFileMock,
            MemoryStream fileContentStream,
            Task<string> addDocumentServiceTask,
            AddFileRequest request)
        {
            // Arrange
            formFileMock.Setup(formFile => formFile.OpenReadStream())
                .Returns(fileContentStream);

            documentServiceMock.Setup(service =>
                    service.AddAsync(request.File.Name, request.File.ContentType,
                        ByteSize.FromBytes(request.File.Length), fileContentStream))
                .Returns(addDocumentServiceTask)
                .Verifiable();

            DocumentsController sut = new DocumentsController(documentServiceMock.Object);

            // Act
            Task addDocumentTask = sut.AddDocument(request);
            await addDocumentTask;

            // Verify
            addDocumentTask.Should().BeSameAs(addDocumentServiceTask);

            documentServiceMock.Verify();
        }
    }
}
