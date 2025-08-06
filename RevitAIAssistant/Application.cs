using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace RevitAIAssistant
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Create a ribbon tab for the AI Assistant
                string tabName = "AI Assistant";
                application.CreateRibbonTab(tabName);

                // Create a ribbon panel
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "AI Tools");

                // Get the assembly path for button images and commands
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                // Create the main AI Assistant button
                PushButtonData buttonData = new PushButtonData(
                    "AIAssistantButton",
                    "AI Assistant",
                    assemblyPath,
                    "RevitAIAssistant.Commands.AIAssistantCommand"
                );

                buttonData.ToolTip = "Launch AI Assistant powered by Together.ai Meta Llama 3.3 70B";
                buttonData.LongDescription = "Open the AI Assistant window to get help with Revit modeling, " +
                                           "troubleshooting, API questions, and more. Powered by Together.ai's " +
                                           "free Meta Llama 3.3 70B Instruct Turbo model.";

                // Try to set an icon (will work if icon file exists)
                try
                {
                    Uri iconUri = new Uri("pack://application:,,,/RevitAIAssistant;component/Resources/ai_icon_32.png");
                    buttonData.LargeImage = new BitmapImage(iconUri);
                }
                catch
                {
                    // If icon doesn't exist, that's fine - button will use default appearance
                }

                // Add the button to the panel
                PushButton button = panel.AddItem(buttonData) as PushButton;

                // Add a separator and help button
                panel.AddSeparator();

                PushButtonData helpButtonData = new PushButtonData(
                    "AIAssistantHelp",
                    "Help",
                    assemblyPath,
                    "RevitAIAssistant.Commands.HelpCommand"
                );

                helpButtonData.ToolTip = "Get help and setup instructions for AI Assistant";
                helpButtonData.LongDescription = "Open help documentation and setup instructions for the AI Assistant plugin.";

                PushButton helpButton = panel.AddItem(helpButtonData) as PushButton;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("AI Assistant Startup Error", 
                               $"Failed to initialize AI Assistant plugin:\n{ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // Cleanup code if needed
            return Result.Succeeded;
        }
    }
}