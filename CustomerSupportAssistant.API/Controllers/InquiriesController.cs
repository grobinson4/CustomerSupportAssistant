using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerSupportAssistant.Domain.Dtos;
using CustomerSupportAssistant.Business.Services;

namespace CustomerSupportAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InquiriesController : ControllerBase
    {
        private readonly InquiryProcessorService _inquiryProcessorService;

        public InquiriesController(InquiryProcessorService inquiryProcessorService)
        {
            _inquiryProcessorService = inquiryProcessorService;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<InquiryAnalysisResult>> AnalyzeInquiry([FromBody] InquiryRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
                return BadRequest("Title and Description are required.");

            var result = await _inquiryProcessorService.AnalyzeInquiryAsync(request);

            return Ok(result);
        }
    }
}
