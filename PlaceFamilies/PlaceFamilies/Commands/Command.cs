#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using PlaceFamilies.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace PlaceFamilies
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            View view = doc.ActiveView;

            if(!(view is ViewPlan))
            {
                TaskDialog.Show("Place Families", "Please make sure that the active view is a plan view.");
                return Result.Cancelled;
            }
            var dlg = new ConfigurationWindow();
            var result = dlg.ShowDialog();
            if (result== false)
            {
                return Result.Cancelled;
            }
            if (uidoc.GetOpenUIViews().Count == 0)
            {
                TaskDialog.Show("View Error", "There is no active view, make sure that the active view is a plan view.");
                return Result.Cancelled;
            }
            UIView uiView = null;
            ElementId vueId = null;
            foreach (var uv in uidoc.GetOpenUIViews())
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiView = uv;
                    vueId = uv.ViewId;
                }
            }
            if (uiView == null)
            {
                TaskDialog.Show("View Error", "There is no active view, make sure that the active view is a plan view.");

                return Result.Cancelled;
            }

            var rect = uiView.GetZoomCorners();
            int l = 0;
            int k = 0;
            int p = 0;
            var allFamilies = FindFamilyTypes(doc);
            if (allFamilies.Count > 0)
            {
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Transaction Name");

                    
                    for (int i = 0; i < allFamilies.Count; i++)
                    {

                        var point = new XYZ(rect[0].X +p, rect[0].Y + l, rect[0].Z);
                        if(PlaceInstance(doc, allFamilies[i], point))
                        {
                            p = p + 7;
                            k = k + 1;
                        }
                       
                        if (k == Properties.Settings.Default.step)
                        {
                            l = l + 7;
                            k = 0;
                            p = 0;
                        }

                    }

                    tx.Commit();
                }

            }

            return Result.Succeeded;
        }
        public static List<FamilySymbol> FindFamilyTypes(Document doc)
        {
            return new FilteredElementCollector(doc)
                            .WherePasses(new ElementClassFilter(typeof(FamilySymbol)))
                            .Cast<FamilySymbol>().ToList();

        }
        public bool PlaceInstance(Document doc, FamilySymbol fs, XYZ location)
        {
            ElementId instanceId = ElementId.InvalidElementId;
            try
            {
                if (fs != null)
                {
                    try
                    {
                        FamilyInstance fi = null;
                        if (!doc.IsFamilyDocument)
                        {
                            fs.Activate();
                            fi = doc.Create.NewFamilyInstance(location, fs, StructuralType.NonStructural);
                        }
                        instanceId = fi.Id;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return (null!=instanceId);
        }
    }
}
