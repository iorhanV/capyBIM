using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace capyBIM.Views;

public partial class VPLineLen : Window
{
    public VPLineLen()
    {
        InitializeComponent();
    }

    private void FontSizeDown_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

  
    private static readonly Regex _regex = new Regex(@"^-?\d*(?:\.\d*)?$");
    private void NumericTextBox(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !_regex.IsMatch(e.Text);
    }

    private void FontSizeUp_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void PreviewBtnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SetBtnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void CloseBtnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AddShortcutBtnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}