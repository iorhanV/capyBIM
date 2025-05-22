using capyBIM.ViewModels;

namespace capyBIM.Views;

public sealed partial class capyBIMView
{
    public capyBIMView(capyBIMViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}