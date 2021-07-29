#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

#endregion

namespace PlaceFamilies
{
    class Main : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            var tabName = "Bimrise";
            try
            {
                RevitUi.AddRibbonTab(a, tabName);

            }
            catch (Exception)
            {

            }     
            var panel = RevitUi.AddRibbonPanel(a, tabName, "Place Families");
            var btn = RevitUi.AddPushButton(panel, "Place", typeof(Command), Properties.Resources.Place_32, Properties.Resources.Place_32, typeof(AvailableIfOpenDoc));
            btn.ToolTip = "Click to place loaded the families on the active view plan.";
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.ChmFile, "mailto:ridaabderrahmane@gmail.com");
            btn.SetContextualHelp(contextHelp);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        private class AvailableIfOpenDoc : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
            {
                if (applicationData.ActiveUIDocument != null && applicationData.ActiveUIDocument.Document != null)
                    return true;
                return false;
            }
        }
    }
}
