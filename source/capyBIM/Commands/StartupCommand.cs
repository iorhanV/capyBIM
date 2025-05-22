using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using capyBIM.ViewModels;
using capyBIM.Views;

namespace capyBIM.Commands;

/// <summary>
///     External command entry point
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new capyBIMViewModel();
        var view = new capyBIMView(viewModel);
        view.ShowDialog();
    }
}