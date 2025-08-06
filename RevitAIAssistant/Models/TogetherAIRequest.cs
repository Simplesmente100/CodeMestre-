using System.Collections.Generic;
using Newtonsoft.Json;

namespace RevitAIAssistant.Models
{
    public class TogetherAIRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; } = "meta-llama/Llama-3.3-70B-Instruct-Turbo";

        [JsonProperty("messages")]
        public List<TogetherAIMessage> Messages { get; set; } = new List<TogetherAIMessage>();

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; } = 2048;

        [JsonProperty("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonProperty("top_p")]
        public double TopP { get; set; } = 0.9;

        [JsonProperty("stream")]
        public bool Stream { get; set; } = false;

        [JsonProperty("stop")]
        public List<string> Stop { get; set; } = new List<string>();
    }

    public class TogetherAIMessage
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        public TogetherAIMessage() { }

        public TogetherAIMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}