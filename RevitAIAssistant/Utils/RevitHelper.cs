using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAIAssistant.Utils
{
    public static class RevitHelper
    {
        public static string GetDocumentInfo(Document doc)
        {
            if (doc == null) return "No document is currently open.";

            var info = new StringBuilder();
            info.AppendLine($"Document: {doc.Title}");
            info.AppendLine($"Application: {doc.Application.VersionName}");
            info.AppendLine($"Is Family Document: {doc.IsFamilyDocument}");
            info.AppendLine($"Is Workshared: {doc.IsWorkshared}");
            info.AppendLine($"Path: {doc.PathName ?? "Not saved"}");

            return info.ToString();
        }

        public static string GetSelectionInfo(UIDocument uidoc)
        {
            if (uidoc?.Selection == null) return "No selection available.";

            var selection = uidoc.Selection;
            var selectedIds = selection.GetElementIds();

            if (selectedIds.Count == 0)
                return "No elements are currently selected.";

            var info = new StringBuilder();
            info.AppendLine($"Selected Elements: {selectedIds.Count}");

            var doc = uidoc.Document;
            var elementsByCategory = new Dictionary<string, int>();

            foreach (var id in selectedIds.Take(20)) // Limit to first 20 for performance
            {
                var element = doc.GetElement(id);
                if (element != null)
                {
                    var categoryName = element.Category?.Name ?? "Unknown";
                    if (elementsByCategory.ContainsKey(categoryName))
                        elementsByCategory[categoryName]++;
                    else
                        elementsByCategory[categoryName] = 1;
                }
            }

            info.AppendLine("Categories:");
            foreach (var kvp in elementsByCategory)
            {
                info.AppendLine($"  - {kvp.Key}: {kvp.Value}");
            }

            if (selectedIds.Count > 20)
            {
                info.AppendLine($"... and {selectedIds.Count - 20} more elements");
            }

            return info.ToString();
        }

        public static string GetProjectPhases(Document doc)
        {
            if (doc == null) return "No document available.";

            try
            {
                var phases = new FilteredElementCollector(doc)
                    .OfClass(typeof(Phase))
                    .Cast<Phase>()
                    .OrderBy(p => p.get_Parameter(BuiltInParameter.PHASE_SEQUENCE_NUMBER)?.AsInteger() ?? 0)
                    .ToList();

                if (phases.Count == 0)
                    return "No phases found in the project.";

                var info = new StringBuilder();
                info.AppendLine("Project Phases:");
                foreach (var phase in phases)
                {
                    info.AppendLine($"  - {phase.Name}");
                }

                return info.ToString();
            }
            catch (Exception ex)
            {
                return $"Error retrieving phases: {ex.Message}";
            }
        }

        public static string GetProjectParameters(Document doc)
        {
            if (doc == null) return "No document available.";

            try
            {
                var projectParams = new StringBuilder();
                projectParams.AppendLine("Project Information:");

                // Get project information parameters
                var projectInfo = doc.ProjectInformation;
                if (projectInfo != null)
                {
                    foreach (Parameter param in projectInfo.Parameters)
                    {
                        if (param.HasValue && !string.IsNullOrEmpty(param.AsString()))
                        {
                            projectParams.AppendLine($"  - {param.Definition.Name}: {param.AsValueString()}");
                        }
                    }
                }

                return projectParams.ToString();
            }
            catch (Exception ex)
            {
                return $"Error retrieving project parameters: {ex.Message}";
            }
        }

        public static string GetCurrentView(UIDocument uidoc)
        {
            if (uidoc?.ActiveView == null) return "No active view.";

            var view = uidoc.ActiveView;
            var info = new StringBuilder();
            info.AppendLine($"Active View: {view.Name}");
            info.AppendLine($"View Type: {view.ViewType}");
            info.AppendLine($"Scale: {view.Scale}");

            if (view is ViewPlan viewPlan)
            {
                info.AppendLine($"Associated Level: {viewPlan.GenLevel?.Name ?? "None"}");
            }

            return info.ToString();
        }

        public static string GetRevitContext(UIDocument uidoc)
        {
            if (uidoc == null) return "No Revit context available.";

            var context = new StringBuilder();
            context.AppendLine("=== REVIT CONTEXT ===");
            context.AppendLine(GetDocumentInfo(uidoc.Document));
            context.AppendLine();
            context.AppendLine(GetCurrentView(uidoc));
            context.AppendLine();
            context.AppendLine(GetSelectionInfo(uidoc));
            context.AppendLine();
            context.AppendLine(GetProjectPhases(uidoc.Document));
            context.AppendLine();
            context.AppendLine(GetProjectParameters(uidoc.Document));
            context.AppendLine("====================");

            return context.ToString();
        }

        public static void ShowMessage(string title, string message, bool isError = false)
        {
            if (isError)
            {
                TaskDialog.Show(title, message, TaskDialogCommonButtons.Ok, TaskDialogResult.Ok);
            }
            else
            {
                TaskDialog.Show(title, message);
            }
        }
    }
}