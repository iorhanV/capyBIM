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
        
        selElement = uiDoc.Selection.PickObjects(ObjectType.Element
                , "Pick elements to rotate")
            .Select(x => doc.GetElement(x)).ToList();
        
        TaskDialog.Show("Rotate Elements", "Rotate Elements");

        // var ui = new RotateElemView();
        // RibbonController.ShowOptionsBar(ui);
        // // ICollection<Element> selElement = PickToElements(uiDoc, doc);            
        // angle = ui.Angle;
        
        // try
        // {
        //     var options = SetupOptionsBar();
        //     selElement = PickToElements(uiDoc, doc);  
        //     angle = options.Angle;
        // }
        // catch (OperationCanceledException)
        // {
        //     // ignored
        // }
        // finally
        // {
        //     using var transaction = new Transaction(doc);
        //     transaction.Start("Rotate Elements");
        //     foreach (Element elem in selElement)
        //     {
        //         zTools.RotateElements(elem, doc, angle);
        //     }
        //     transaction.Commit();
        //     RibbonController.HideOptionsBar();
        // }
        

        // RibbonController.HideOptionsBar();
        
        return Result.Succeeded;
    }

    // private RotateViewModel SetupOptionsBar()
    // {
    //     var options = new RotateViewModel
    //     {
    //         Angle = 90
    //     };
    //     
    //     var view = new RotateView(options);
    //     
    //     RibbonController.ShowOptionsBar(view);
    //     
    //     return options;
    // }
    
    private static ICollection<Element> PickToElements(UIDocument uiDoc, Document doc)
    {
        // Get the element selection of current document.
        ICollection<Reference> selectedReferences = uiDoc.Selection.GetReferences();

        if (0 == selectedReferences.Count)
        {
            // If no elements selected.
            selectedReferences =
                uiDoc.Selection.PickObjects(ObjectType.Element, "Pick elements to rotate");
        }

        ICollection<Element> selectedElements = new List<Element>();

        foreach (Reference reference in selectedReferences)
        {
            selectedElements.Add(doc.GetElement(reference));
        }
        return selectedElements;
    }
}
