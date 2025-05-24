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
}