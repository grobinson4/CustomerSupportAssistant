using Azure.AI.OpenAI;
using CustomerSupportAssistant.Domain.Dtos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomerSupportAssistant.Business.Services
{
    public class InquiryProcessorService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<InquiryProcessorService> _logger;

        public InquiryProcessorService(OpenAIClient openAIClient, ILogger<InquiryProcessorService> logger)
        {
            _openAIClient = openAIClient;
            _logger = logger;
        }

        public async Task<InquiryAnalysisResult> AnalyzeInquiryAsync(InquiryRequestDto inquiry)
        {
            string systemPrompt = "You are an expert Azure App Service Support Assistant. Only respond with JSON.";

            string userPrompt = $@"
Customer Inquiry:
Title: {inquiry.Title}
Description: {inquiry.Description}

Output ONLY valid JSON matching this structure. Do NOT include any explanation or markdown.

JSON format:
{{
  ""rootCauses"": [
    {{
      ""hypothesis"": ""string"",
      ""confidence"": number,
      ""actions"": [""string""],
      ""docs"": [{{ ""title"": ""string"", ""link"": ""string"" }}]
    }}
  ],
  ""azureCommands"": [""string""],
  ""eli5Explanation"": ""string""
}}
";

            try
            {
                var chatResponse = await _openAIClient.GetChatCompletionsAsync(
                    deploymentOrModelName: "gpt-4o",
                    new ChatCompletionsOptions
                    {
                        Messages =
                        {
                            new ChatMessage(ChatRole.System, systemPrompt),
                            new ChatMessage(ChatRole.User, userPrompt)
                        },
                        Temperature = 0.2f,
                        MaxTokens = 1000
                    });

                var content = chatResponse.Value.Choices[0].Message.Content?.Trim();

                _logger.LogInformation("Raw OpenAI Response: {content}", content);

                var result = JsonSerializer.Deserialize<InquiryAnalysisResult>(content!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || result.RootCauses == null || result.RootCauses.Count == 0)
                {
                    _logger.LogWarning("AI response parsed, but no valid root causes found.");
                    return new InquiryAnalysisResult
                    {
                        Eli5Explanation = "The AI responded but did not return meaningful suggestions."
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze inquiry using OpenAI.");
                return new InquiryAnalysisResult
                {
                    Eli5Explanation = "The AI response could not be processed. Please try again later."
                };
            }
        }
    }
}