using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using capyBIM.ViewModels;
using capyBIM.Views;
using capyBIM.Views.Utils;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using cVP = capyBIM.Utilities.VPLineUtils;
using cScr = capyBIM.Utilities.ScriptUtils;

namespace capyBIM.CmdsTools;

#region Rotate
[Transaction(TransactionMode.Manual)]
public class CmdRotate : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // Collect the document
        var uiApp = commandData.Application;
        var uiDoc = uiApp.ActiveUIDocument;
        var doc = uiDoc.Document;
        Double angle = 45;
        ICollection<Element> selElement = new List<Element>();
        
        try
        {
            var options = SetupOptionsBar();
            selElement = PickToElements(uiDoc, doc);  
            angle = options.Angle;
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        finally
        {
            using var transaction = new Transaction(doc);
            transaction.Start("Rotate Elements");
            foreach (Element elem in selElement)
            {
                RotateElements(elem, doc, angle);
            }
            transaction.Commit();
            RibbonController.HideOptionsBar();
        }

        RibbonController.HideOptionsBar();
        
        return Result.Succeeded;
    }

    private RotateViewModel SetupOptionsBar()
    {
        var options = new RotateViewModel
        {
            Angle = 0.0
        };
        
        var view = new RotateView(options);
        
        RibbonController.ShowOptionsBar(view);
        
        return options;
    }
    
    private static ICollection<Element> PickToElements(UIDocument uiDoc, Document doc)
    {
        ICollection<Element> selectedElements = new List<Element>();
        
        IList<Reference> selRef = uiDoc.Selection.PickObjects(ObjectType.Element, "Pick elements to rotate");
        if (selRef != null)
        {
            selectedElements = selRef.Select(doc.GetElement).ToList();
        }
        return selectedElements;
    }
    
    public static void RotateElements(Element elem, Document doc,  double angle)
    {
        BoundingBoxXYZ bBox = elem.get_BoundingBox(doc.ActiveView);
        XYZ pointBox = (bBox.Min + bBox.Max) / 2;
        
        // Create Vertical Axis Line
        Line axisLine = Line.CreateBound(pointBox, pointBox + XYZ.BasisZ);
        
        // Convert to radians
        double radians = (Math.PI / 180) * angle;
        
        ElementTransformUtils.RotateElement(doc, elem.Id, axisLine, radians);
    }
}
#endregion

#region VPLineLen
[Transaction(TransactionMode.Manual)]
public class CmdVPLineLen : IExternalCommand
{
    private double _minVersion = 2022;
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiApp = commandData.Application;
        var uiDoc = uiApp.ActiveUIDocument;
        var doc = uiDoc.Document;
        var activeView = doc.ActiveView;

        if (!(activeView is ViewSheet))
        {
            TaskDialog.Show("Error", "Please open a sheet view to run this command.");
            return Result.Cancelled;
        }
        
        // Check for alt fire
        var altFire = cScr.KeyHeldShift();
        
        var collector = new FilteredElementCollector(doc, activeView.Id);
        var viewports = collector.OfClass(typeof(Viewport)).WhereElementIsNotElementType().ToElements();

        if (!viewports.Any())
        {
            TaskDialog.Show("Error", "No viewports in active sheet.");
            return Result.Cancelled;}
        
        if (altFire)
        {
            using (TransactionGroup transGroup = new TransactionGroup(doc, "Configuring VPLineLen"))
            {
                transGroup.Start();
                var form = new VPLineLenView(uiApp, viewports);
                var result = form.ShowDialog();
                transGroup.Assimilate();
            }
            return Result.Succeeded;
        }
        
        // FontFamily fontFamily = new FontFamily("Century Gothic");
        string fontFamily = "Century Gothic";
        double size = 5;
        double corFact = 0.0023;
        
        using (TransactionGroup transGroup = new TransactionGroup(doc, "Resize VP"))
        {
            transGroup.Start();
        
            cVP.ResizeVP(viewports, fontFamily, size, corFact, doc);
            
            transGroup.Assimilate();
        }
        return Result.Succeeded;
    }
}
#endregion

#region VPLineAll
[Transaction(TransactionMode.Manual)]
public class CmdVPLineLenAll : IExternalCommand
{
    public static readonly string CmdName = "Resize All VP Line";
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiApp = commandData.Application;
        var uiDoc = uiApp.ActiveUIDocument;
        var doc = uiDoc.Document;
        var activeView = doc.ActiveView;

        if (!(activeView is ViewSheet))
        {
            TaskDialog.Show("Error", "Please open a sheet view to run this command.");
            return Result.Cancelled;
        }
        
        // Check for alt fire
        var altFire = cScr.KeyHeldShift();
        
        // FontFamily fontFamily = new FontFamily("Century Gothic");
        string fontFamily = "Century Gothic";
        double size = 5;
        double corFact = 0.0023;
        
        var collector = new FilteredElementCollector(doc, activeView.Id);
        var viewports = collector.OfClass(typeof(Viewport)).WhereElementIsNotElementType().ToElements();
        
        using (TransactionGroup transGroup = new TransactionGroup(doc, CmdName))
        {
            transGroup.Start();
            var form = new VPLineLenView(uiApp, viewports);
            var result = form.ShowDialog();
            transGroup.Assimilate();
        }
        
        // var taskDia = new TaskDialog(CmdName);
        // taskDia.MainInstruction = ("This will affect all viewports project");
        // taskDia.MainContent = "Would you like to proceed?";
        // taskDia.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
        // var tastResult = taskDia.Show();
        
        return Result.Succeeded;
    }
}

#endregion