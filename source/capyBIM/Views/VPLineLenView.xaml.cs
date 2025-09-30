using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.UI;
using sDrw = System.Drawing;
using cCon = capyBIM.Utilities.ConvertUtils;
using cVP = capyBIM.Utilities.VPLineUtils;

namespace capyBIM.Views;

public partial class VPLineLenView : Window
{
    private IList<Element> _viewports;
    private Document _doc;
    private string _fontFamily;
    private double _fontSize;
    
    public VPLineLenView(UIApplication uiApp, IList<Element> viewports)
    {
        InitializeComponent();
        PopulateComboBox();
        var uiDoc = uiApp.ActiveUIDocument;
        this._doc = uiDoc.Document;
        
        this._viewports = viewports;

        this.UICorrectionSlider.ValueChanged += this.OnSliderValueChanged;
    }

    private static readonly Regex _regex = new Regex(@"^-?\d*(?:\.\d*)?$");
    
    private void NumericTextBox(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !_regex.IsMatch(e.Text);
    }

    private void FontSizeUp_Click(object sender, RoutedEventArgs e)
    {
        double? value = cCon.StringToDouble(UIFontSize.Text);

        double? newValue = value + 0.5;
        if (newValue > 10)
        {
            newValue = 10;
        }
        
        UIFontSize.Text = newValue.ToString();
    }
    
    private void FontSizeDown_Click(object sender, RoutedEventArgs e)
    {
        double? value = cCon.StringToDouble(UIFontSize.Text);

        double? newValue = value - 0.5;
        if (newValue < 1)
        {
            newValue = 1;
        }
        
        UIFontSize.Text = newValue.ToString();
    }
    
    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        double corFact = this.UICorrectionSlider.Value;
        
        _fontFamily = this.UIFontFamily.Text;
        _fontSize = Convert.ToDouble(this.UIFontSize.Text);
        
        using (TransactionGroup transGroup = new TransactionGroup(_doc, "Resize VP"))
        {
            transGroup.Start();
        
            cVP.ResizeVP(_viewports, _fontFamily, _fontSize, corFact, _doc);
            
            transGroup.Assimilate();
        }
        
    }
    
    private void PreviewBtnClick(object sender, RoutedEventArgs e)
    {
        double corFact = this.UICorrectionSlider.Value;
        _fontFamily = this.UIFontFamily.Text;
        _fontSize = Convert.ToDouble(this.UIFontSize.Text);
        
        using (TransactionGroup transGroup = new TransactionGroup(_doc, "Resize VP"))
        {
            transGroup.Start();
        
            cVP.ResizeVP(_viewports, _fontFamily, _fontSize, corFact, _doc);
            
            transGroup.Assimilate();
        }
    }

    private void SetBtnClick(object sender, RoutedEventArgs e)
    {
        TaskDialog.Show("Sorry", "Feature not implemented yet");
        this.Close();
    }

    private void CloseBtnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void PopulateComboBox()
    {
        UIFontFamily.Items.Clear();

        foreach (var font in sDrw.FontFamily.Families)
        {
            UIFontFamily.Items.Add(font);
            UIFontFamily.DisplayMemberPath = "Name";
        }

        // Select first item
        UIFontFamily.SelectedIndex = 0;
    }

    private void AddShortcutBtnClick(object sender, RoutedEventArgs e)
    {
        TaskDialog.Show("Sorry", "Feature not implemented yet");
        this.Close();
    }
}

