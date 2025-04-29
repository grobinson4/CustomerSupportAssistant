namespace CustomerSupportAssistant.Domain.Dtos
{
    public class InquiryRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ResourceId { get; set; } // Optional: Azure App Service ResourceId
    }
}
