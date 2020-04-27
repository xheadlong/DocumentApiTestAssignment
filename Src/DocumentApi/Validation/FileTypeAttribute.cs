using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentApi.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FileTypeAttribute : ValidationAttribute
    {
        private readonly string configPath = null;

        public FileTypeAttribute(string configPath)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new ArgumentException("Configuration path is not initialized", "configPath");
            }

            this.configPath = configPath;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile fileInfo = value as IFormFile;

            if (fileInfo == null) 
            {
                if (value == null)
                {
                    return ValidationResult.Success;
                }
                else 
                {
                    return new ValidationResult("Value is not of IFormFile type.");
                }
            }

            if (string.IsNullOrWhiteSpace(fileInfo.ContentType))
            {
                return new ValidationResult("File does not have a mime type specified.");
            }

            IConfiguration configuration = validationContext.GetRequiredService<IConfiguration>();

            Dictionary<string, string[]> allowedFileTypes = new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);
            configuration.GetSection(this.configPath).Bind(allowedFileTypes);

            if (!allowedFileTypes.TryGetValue(fileInfo.ContentType, out string[] fileExtensions))
            {
                return new ValidationResult("File is not of expected mime type.");
            }

            if (fileExtensions == null)
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrWhiteSpace(fileInfo.FileName))
            {
                return new ValidationResult($"File does not have a file name specified.");
            }

            string fileExtension = Path.GetExtension(fileInfo.FileName);

            if (fileExtension == null)
            {
                return new ValidationResult($"File does not have extension.");
            }

            bool validFileExtension = fileExtensions.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase);
            if (!validFileExtension)
            {
                return new ValidationResult($"File extension does not correspond to the indicated mime type.");
            }

            return ValidationResult.Success;
        }
    }
}
