using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
        
        using (TransactionGroup transGroup = new TransactionGroup(doc, "Resize VP"))
        {
            transGroup.Start();
        
            ResizeVP(viewports, fontFamily, size, corFact, doc);
            
            transGroup.Assimilate();
        }
        return Result.Succeeded;
    }
    
        public static void ResizeVP(ICollection<Element> viewports, string fontFam, double fontSize, double correctionFactor, Document doc)
    {
        ElementId? textNoteType;
        ICollection<ElementId?> toDelete = new List<ElementId?>();

        using (Transaction t1 = new Transaction(doc, "Create text note type"))
        {
            t1.Start();
            textNoteType = CreateTextNoteType(fontFam, fontSize, doc);
            toDelete.Add(textNoteType);
            t1.Commit();
        }

        foreach (var element in viewports)
        {
            var viewport = (Viewport)element;
            var vpTitle = viewport.get_Parameter(BuiltInParameter.VIEWPORT_VIEW_NAME).AsString();
            var textNote = CreateTextNote(vpTitle, textNoteType, doc);
            toDelete.Add(textNote.Id);
            var sizeTitle = textNote.Width;
            double titleSize = sizeTitle + correctionFactor;
            
            using (Transaction t3 = new Transaction(doc, "Set Line"))
            {
                t3.Start();
                viewport.LabelLineLength = titleSize;
                t3.Commit();
            }
        }
        
        using (Transaction t4 = new Transaction(doc, "Delete Elements"))
        {
            t4.Start();
            doc.Delete(toDelete);
            t4.Commit();
        }

        
    }

    private static TextNote CreateTextNote(string text, ElementId? textNoteType, Document doc)
    {
        // Create variables
        XYZ xyz = new XYZ();
        View activeView = doc.ActiveView;
        TextNote textNote;

        // First transaction
        using (Transaction t2 = new Transaction(doc, "Create text"))
        {
            t2.Start();
            // Create text note at origin
            textNote = TextNote.Create(doc, activeView.Id, xyz, text, textNoteType);
            t2.Commit();
        }
        return textNote;
    }
    
    private static ElementId? CreateTextNoteType(string fontFam, double fontSize, Document doc)
    {
        // Create variables
        string font = fontFam;
        double size = UnitUtils.ConvertToInternalUnits(fontSize, UnitTypeId.Meters)/1000;
        
        TextNoteType? defaultTextType = new FilteredElementCollector(doc)
            .OfClass(typeof(TextNoteType))
            .Cast<TextNoteType>()
            .FirstOrDefault();

        if (defaultTextType != null)
        {
            var textNoteType = defaultTextType.Name;
            var newType = defaultTextType.Duplicate("tempTEXT14");
 
            // Modify parameters (font, size)
            newType.get_Parameter(BuiltInParameter.TEXT_FONT).Set(font);
            newType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(size); // feet
        
            // Fixed parameters
            newType.get_Parameter(BuiltInParameter.TEXT_STYLE_BOLD).Set(0); // 1 = true 0 = false
            newType.get_Parameter(BuiltInParameter.TEXT_STYLE_ITALIC).Set(0);
            newType.get_Parameter(BuiltInParameter.TEXT_STYLE_UNDERLINE).Set(0);
            newType.get_Parameter(BuiltInParameter.TEXT_WIDTH_SCALE).Set(1);
            // newType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(0);
            // newType.get_Parameter(BuiltInParameter.TEXT_TAB_SIZE).Set(1);
            // newType.get_Parameter(BuiltInParameter.TEXT_BOX_VISIBILITY).Set(0);
            // newType.get_Parameter(BuiltInParameter.LEADER_OFFSET_SHEET).Set(0);
            // newType.get_Parameter(BuiltInParameter.LINE_PEN).Set(1);

            return newType.Id;
        }
        return null;
    }
    
}
