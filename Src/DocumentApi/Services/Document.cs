using System;

namespace DocumentApi.Services
{
    public class Document
    {
        public Document(Dal.Document document) 
        {
            this.Id = document.Id;
            this.Name = document.Name;
            this.Size = document.SizeInBytes;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public Uri DownloadUri { get; set; }
    }
}
