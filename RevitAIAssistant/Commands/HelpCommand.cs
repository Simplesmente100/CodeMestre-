using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIAssistant.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class HelpCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var helpText = @"
🤖 AI ASSISTANT FOR REVIT 2026 🤖
Powered by Together.ai Meta Llama 3.3 70B Instruct Turbo

SETUP INSTRUCTIONS:
1. Get a free API key from https://api.together.xyz/
2. Sign up for a free account (no credit card required)
3. Copy your API key
4. Click the AI Assistant button and enter your API key when prompted

FEATURES:
• Chat with AI about Revit modeling techniques
• Get help with family creation and parameters
• Troubleshoot common Revit issues
• Learn about BIM standards and workflows
• Get guidance on Revit API and plugin development
• Context-aware assistance based on your current project

USAGE TIPS:
• The AI can see your current Revit context (document info, selection, etc.)
• Use the 'Include Revit context' checkbox for more relevant answers
• Ask specific questions for better results
• Use Ctrl+Enter to send messages quickly

ABOUT THE MODEL:
This plugin uses Together.ai's free Meta Llama 3.3 70B Instruct Turbo model, 
which provides high-quality responses without any cost.

For more information, visit:
• Together.ai: https://api.together.xyz/
• Plugin support: Contact your system administrator

Version: 1.0.0
";

                var dialog = new TaskDialog("AI Assistant Help");
                dialog.MainInstruction = "AI Assistant for Revit 2026";
                dialog.MainContent = helpText;
                dialog.CommonButtons = TaskDialogCommonButtons.Ok;
                dialog.DefaultButton = TaskDialogResult.Ok;
                dialog.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Error showing help: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}