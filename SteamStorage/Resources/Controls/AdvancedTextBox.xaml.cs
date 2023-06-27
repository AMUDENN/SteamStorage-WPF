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
        public static readonly DependencyProperty AdvancedTextBoxStyleProperty = DependencyProperty.Register(
            name: "AdvancedTextBoxStyle", propertyType: typeof(Style), ownerType: typeof(AdvancedTextBox)
        );
        public static readonly DependencyProperty AdvancedTextProperty = DependencyProperty.Register(
            name: "AdvancedText", propertyType: typeof(string), ownerType: typeof(AdvancedTextBox)
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
        public Style AdvancedTextBoxStyle
        {
            get => (Style)GetValue(dp: AdvancedTextBoxStyleProperty);
            set => SetValue(dp: AdvancedTextBoxStyleProperty, value: value);
        }
        public string AdvancedText
        {
            get => (string)GetValue(dp: AdvancedTextProperty);
            set => SetValue(dp: AdvancedTextProperty, value: value);
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
        private void MainTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (PreviewRegexExpression.Count(text) != text.Length)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        #endregion Methods
    }
}
