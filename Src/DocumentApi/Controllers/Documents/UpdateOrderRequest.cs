using System.ComponentModel.DataAnnotations;

namespace DocumentApi.Controllers.Documents
{
    public class UpdateOrderRequest
    {
        [Required]
        public string[] DocumentIds { get; set; }
    }
}
