using System.Drawing;

namespace capyBIM.Utilities;

public abstract class ToolsUtils
{
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

    public static void ResizeVP(ICollection<Element> viewports, string fontFam, double fontSize, double correctionFactor, Document doc)
    {
        ElementId? textNoteType = CreateTextNoteType(fontFam, fontSize, doc);

        foreach (var element in viewports)
        {
            var viewport = (Viewport)element;
            var vpTitle = viewport.get_Parameter(BuiltInParameter.VIEWPORT_VIEW_NAME).AsString();
            var sizeTitle = GetStringLength(vpTitle, textNoteType, doc);
            double titleSize = sizeTitle + correctionFactor;
            viewport.LabelLineLength = titleSize;
        }
        
        doc.Delete(textNoteType);
        
    }

    private static double GetStringLength(string text, ElementId? textNoteType, Document doc)
    {
        // Create variables
        XYZ xyz = new XYZ();
        
       // Create text note at origin
        TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, xyz, text, textNoteType);
        
        
        //TODO Text Wrapping not working, need to find a workaround. Maybe get bounding box or something.
        // textNote.IsTextWrappingActive 

        // textNote.get = true;
        var stringLength = textNote.Width;
        
        doc.Delete(textNote.Id);

        return stringLength;
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