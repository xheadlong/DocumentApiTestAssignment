using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DocumentApi.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentApi.Controllers.Documents
{
    public class AddFileRequest
    {
        [Required(ErrorMessage = "The uploaded form does not contain a file under 'file' key.")]
        [FileType("AllowedFileTypes")]
        [MaxFileSize("MaxFileSize")]
        public IFormFile File { get; set; }
    }
}
