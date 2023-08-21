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
        private Regex? previewRegexExpression;
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            name: "MaxLength", propertyType: typeof(int), ownerType: typeof(AdvancedTextBox)
        );
        public static readonly DependencyProperty AllowSpaceProperty = DependencyProperty.Register(
            name: "AllowSpace", propertyType: typeof(bool), ownerType: typeof(AdvancedTextBox)
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
        public bool AllowSpace
        {
            get => Convert.ToBoolean(GetValue(dp: AllowSpaceProperty));
            set => SetValue(dp: AllowSpaceProperty, value: value);
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
        private Regex? PreviewRegexExpression
        {
            get 
            {
                if (PreviewRegex is null) return null;
                return previewRegexExpression ??= new Regex(PreviewRegex);
            }
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
            if (PreviewRegexExpression is not null)
                e.Handled = !PreviewRegexExpression.IsMatch(e.Text);
        }
        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text;
            if (!AllowSpace)
            {
                tb.Text = text.Replace(" ", string.Empty);
            }
            if (text.Length >= MaxLength)
            {
                tb.Text = text[..MaxLength];
                tb.SelectionStart = MaxLength;
            }
        }
        private void MainTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (PreviewRegexExpression is null) return;
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
