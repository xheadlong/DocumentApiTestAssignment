using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DocumentApi.Services;

namespace DocumentApi.Controllers.Documents
{
    public class DocumentView
    {
        public DocumentView() 
        { 
        }

        public DocumentView(Document document) 
        {
            this.Id = document.Id;
            this.Name = document.Name;
            this.FileSize = document.Size;
            this.Location = document.DownloadUri;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("file-size")]
        public double FileSize { get; set; }

        public Uri Location { get; set; }
    }
}
