using System;

namespace RevitAIAssistant.Models
{
    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage()
        {
            Timestamp = DateTime.Now;
        }

        public ChatMessage(string role, string content) : this()
        {
            Role = role;
            Content = content;
        }
    }

    public static class ChatRoles
    {
        public const string System = "system";
        public const string User = "user";
        public const string Assistant = "assistant";
    }
}