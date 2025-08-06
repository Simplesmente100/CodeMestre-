using System;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAIAssistant.UI;
using RevitAIAssistant.Utils;

namespace RevitAIAssistant.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AIAssistantCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Get the current Revit application and document
                var uiApp = commandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;

                if (uiDoc == null)
                {
                    RevitHelper.ShowMessage("AI Assistant", "Please open a Revit document first.", true);
                    return Result.Failed;
                }

                // Check if AI Assistant window is already open
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is AIAssistantWindow)
                    {
                        window.Activate();
                        window.WindowState = WindowState.Normal;
                        return Result.Succeeded;
                    }
                }

                // Create and show the AI Assistant window
                var assistantWindow = new AIAssistantWindow(uiDoc);
                assistantWindow.Show();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Error launching AI Assistant: {ex.Message}";
                RevitHelper.ShowMessage("AI Assistant Error", message, true);
                return Result.Failed;
            }
        }
    }
}