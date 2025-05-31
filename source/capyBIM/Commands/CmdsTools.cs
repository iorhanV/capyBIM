using System.Drawing;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using zTools = capyBIM.Utilities.ToolsUtils;
using capyBIM.ViewModels;
using capyBIM.Views;
using capyBIM.Views.Utils;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace capyBIM.CmdsTools;

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
                zTools.RotateElements(elem, doc, angle);
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
}

[Transaction(TransactionMode.Manual)]
public class CmdVPLineLen : IExternalCommand
{
    private double _minVersion = 2022;
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiApp = commandData.Application;
        var uiDoc = uiApp.ActiveUIDocument;
        var doc = uiDoc.Document;
        
        // FontFamily fontFamily = new FontFamily("Century Gothic");
        string fontFamily = "Century Gothic";
        double size = 3;
        double corFact = 0;


        var collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
        var viewports = collector.OfClass(typeof(Viewport)).WhereElementIsNotElementType().ToElements();
        
        using var transaction = new Transaction(doc);
        transaction.Start("Rotate Elements");
        
        zTools.ResizeVP(viewports, fontFamily, size, corFact, doc);
        
        transaction.Commit();
        
        return Result.Succeeded;
    }
}
