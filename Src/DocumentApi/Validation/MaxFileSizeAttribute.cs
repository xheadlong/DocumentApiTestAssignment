using System;
using System.ComponentModel.DataAnnotations;
using ByteSizeLib;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentApi.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly string configPath = null;

        public MaxFileSizeAttribute(string configPath) 
        {
            if (string.IsNullOrWhiteSpace(configPath)) 
            {
                throw new ArgumentException("Configuration path is not initialized.", "configPath");
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

            IConfiguration configuration = validationContext.GetRequiredService<IConfiguration>();

            string rawMaxSize = configuration.GetValue<string>(this.configPath);

            if (!ByteSize.TryParse(rawMaxSize, out ByteSize maxSize))
            {
                return new ValidationResult("Max size value is invalid.");
            }
                
            ByteSize actualSize = ByteSize.FromBytes(fileInfo.Length);

            if (actualSize > maxSize)
            {
                return  new ValidationResult($"File size exceeds allowed {maxSize.ToString()}.");
            }

            return ValidationResult.Success;
        }
    }
}
