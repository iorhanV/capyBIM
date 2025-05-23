using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace capyBIM.CmdsResources;

[Transaction(TransactionMode.Manual)]
public class CmdGitHub : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        string url = "https://github.com/iorhanV/capyBIM";
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        return Result.Succeeded;
    }
}

[Transaction(TransactionMode.Manual)]
public class CmdIn : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        string url = "https://www.linkedin.com/in/iorhanv/";
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        return Result.Succeeded;
    }
}    

[Transaction(TransactionMode.Manual)]
public class CmdRVTDocs : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        string url = "https://rvtdocs.com/";
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        return Result.Succeeded;
    }
}