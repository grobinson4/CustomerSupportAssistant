using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace CustomerSupportAssistant.Infrastructure.Services
{
    public class OpenAIService
    {
        private readonly OpenAIClient _client;
        private readonly string _deploymentName;

        public OpenAIService(IConfiguration configuration)
        {
            var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]);
            var key = new AzureKeyCredential(configuration["AzureOpenAI:Key"]);
            _deploymentName = configuration["AzureOpenAI:DeploymentName"];

            _client = new OpenAIClient(endpoint, key);
        }

        public async Task<string> SummarizeProjectAsync(string title, string description)
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are an expert project planner. Given a project title and description, summarize the steps needed to complete it in a professional, concise manner."),
                    new ChatMessage(ChatRole.User, $"Project Title: {title}\nProject Description: {description}")
                }
            };

            var response = await _client.GetChatCompletionsAsync(_deploymentName, chatCompletionsOptions);

            var completions = response.Value.Choices.FirstOrDefault();
            return completions?.Message.Content ?? "No plan generated.";
        }
    }
}
