using System.Text.RegularExpressions;
using System.Windows.Input;
using capyBIM.ViewModels;

namespace capyBIM.Views;

public partial class RotateView
{
    public RotateView(RotateViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
    
     private static readonly Regex _regex = new Regex(@"^-?\d*(?:\.\d*)?$");
     private void NumericTextBox(object sender, TextCompositionEventArgs e)
     {
         e.Handled = !_regex.IsMatch(e.Text);
     }
}