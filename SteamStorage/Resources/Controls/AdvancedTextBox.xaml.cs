using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamStorage.Resources.Controls
{
    public partial class AdvancedTextBox : UserControl
    {
        #region Fields
        private Regex previewRegexExpression;
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            name: "MaxLength", propertyType: typeof(int), ownerType: typeof(AdvancedTextBox)
        );
        public static readonly DependencyProperty PreviewRegexProperty = DependencyProperty.Register(
            name: "PreviewRegex", propertyType: typeof(string), ownerType: typeof(AdvancedTextBox)
        );
        public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register(
            name: "TextBoxStyle", propertyType: typeof(Style), ownerType: typeof(AdvancedTextBox)
        );
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            name: "Text", propertyType: typeof(string), ownerType: typeof(AdvancedTextBox)
        );
        #endregion Fields

        #region Properties
        public int MaxLength
        {
            get => Convert.ToInt32(GetValue(dp: MaxLengthProperty));
            set => SetValue(dp: MaxLengthProperty, value: value);
        }
        public string PreviewRegex
        {
            get => (string)GetValue(dp: PreviewRegexProperty);
            set => SetValue(dp: PreviewRegexProperty, value: value);
        }
        public Style TextBoxStyle
        {
            get => (Style)GetValue(dp: TextBoxStyleProperty);
            set => SetValue(dp: TextBoxStyleProperty, value: value);
        }
        public string Text
        {
            get => (string)GetValue(dp: TextProperty);
            set => SetValue(dp: TextProperty, value: value);
        }
        private Regex PreviewRegexExpression
        {
            get => previewRegexExpression ??= new Regex(PreviewRegex);
            set => previewRegexExpression = value;
        }
        #endregion Properties

        #region Constructor
        public AdvancedTextBox()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region Methods
        private void MainTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !PreviewRegexExpression.IsMatch(e.Text);
        }
        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text;
            if (text.Length >= MaxLength)
            {
                tb.Text = text[..MaxLength];
                tb.SelectionStart = MaxLength;
            }
        }
        #endregion Methods
    }
}
