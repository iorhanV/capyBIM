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

    public static void ResizeVP(Viewport viewport, FontFamily fontFam, double fontSize, double correctionFactor)
    {
        var vpTitle = viewport.get_Parameter(BuiltInParameter.VIEWPORT_VIEW_NAME).AsString();
        
        var sizeTitle = GetStringLength(vpTitle, fontFam, fontSize);
        
        // Convert from points to feet
        
        
        double titleSize = sizeTitle + 10 + correctionFactor;
        
        viewport.LabelLineLength = titleSize;
    }

    private static double GetStringLength(string text, FontFamily fontFam, double fontSize, Document doc)
    {
        XYZ xyz = new XYZ();
        ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
        // ElementId newTypeId = defaultTextType.Duplicate();
        
        // TODO duplicate textnotetype
        TextNoteType newType = null;
        
        // Modify parameters (e.g., font, size, bold)
        newType.get_Parameter(BuiltInParameter.TEXT_FONT).Set("Arial");
        newType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(0.2); // feet
        newType.get_Parameter(BuiltInParameter.TEXT_STYLE_BOLD).Set(0); // 1 = true 0 = false
        newType.get_Parameter(BuiltInParameter.TEXT_STYLE_ITALIC).Set(0);
        newType.get_Parameter(BuiltInParameter.TEXT_STYLE_UNDERLINE).Set(0);
        newType.get_Parameter(BuiltInParameter.TEXT_WIDTH_SCALE).Set(1);
        newType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(0);
        newType.get_Parameter(BuiltInParameter.TEXT_TAB_SIZE).Set(0);
        newType.get_Parameter(BuiltInParameter.TEXT_BOX_VISIBILITY).Set(0);
        newType.get_Parameter(BuiltInParameter.LEADER_OFFSET_SHEET).Set(0);
        newType.get_Parameter(BuiltInParameter.LINE_PEN).Set(1);
        
        
        ElementId typeId = newType.Id;  
        
        TextNote textNote = TextNote.Create(doc, doc.ActiveView, xyz, text, typeId);
        
    }
}