namespace capyBIM.Extensions;

public static class ElementExt
{
    public static Parameter TypeParameter(this Element element, BuiltInParameter bip)
    {
        return element.Document.GetElement(element.GetTypeId()).get_Parameter(bip);
    }
    
    public static Parameter TypeParameter(this Element element, string paramName)
    {
        return element.Document.GetElement(element.GetTypeId()).LookupParameter(paramName);
    }
}