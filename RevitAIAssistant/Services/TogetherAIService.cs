using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RevitAIAssistant.Models;

namespace RevitAIAssistant.Services
{
    public class TogetherAIService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string API_BASE_URL = "https://api.together.xyz/v1/chat/completions";
        private readonly string _apiKey;

        public TogetherAIService(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            
            if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            }
        }

        public async Task<string> SendMessageAsync(List<ChatMessage> messages, string systemPrompt = null)
        {
            try
            {
                var request = CreateRequest(messages, systemPrompt);
                var jsonContent = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(API_BASE_URL, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API request failed: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<TogetherAIResponse>(responseContent);

                if (apiResponse?.Choices?.Count > 0)
                {
                    return apiResponse.Choices[0].Message.Content;
                }

                throw new InvalidOperationException("No response received from the API");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error communicating with Together.ai API: {ex.Message}", ex);
            }
        }

        private TogetherAIRequest CreateRequest(List<ChatMessage> messages, string systemPrompt)
        {
            var request = new TogetherAIRequest();

            // Add system prompt if provided
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                request.Messages.Add(new TogetherAIMessage(ChatRoles.System, systemPrompt));
            }

            // Add default system prompt for Revit context
            if (string.IsNullOrEmpty(systemPrompt))
            {
                var defaultSystemPrompt = @"You are an AI assistant specialized in helping with Autodesk Revit. 
You can help users with:
- Revit modeling techniques and best practices
- Family creation and parameter management
- Project organization and workflows
- Troubleshooting common issues
- API and plugin development guidance
- BIM standards and coordination

Please provide clear, actionable advice and ask for clarification when needed. 
When discussing technical solutions, consider the user's experience level and provide step-by-step instructions when appropriate.";

                request.Messages.Add(new TogetherAIMessage(ChatRoles.System, defaultSystemPrompt));
            }

            // Convert and add chat messages
            foreach (var message in messages)
            {
                request.Messages.Add(new TogetherAIMessage(message.Role, message.Content));
            }

            return request;
        }

        public async Task<string> SendSingleMessageAsync(string userMessage, string systemPrompt = null)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRoles.User, userMessage)
            };

            return await SendMessageAsync(messages, systemPrompt);
        }

        // Method to validate API key
        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                var testMessage = "Hello, this is a test message.";
                await SendSingleMessageAsync(testMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}