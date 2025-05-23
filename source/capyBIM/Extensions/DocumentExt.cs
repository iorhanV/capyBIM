using Autodesk.Revit.UI;

namespace capyBIM.Extensions;
public static class DocumentExt
{
    public static string GetDocPath(this Document doc)
    {
        if (doc.IsFamilyDocument)
        {
            TaskDialog.Show("Family Document","Family file are not supported.");
            return null;
        }
        else if (doc.IsWorkshared)
        {
            ModelPath centralPath = doc.GetWorksharingCentralModelPath();
            return ModelPathUtils.ConvertModelPathToUserVisiblePath(centralPath);
        }
        else
        {
            return doc.PathName;
        }
    }
}