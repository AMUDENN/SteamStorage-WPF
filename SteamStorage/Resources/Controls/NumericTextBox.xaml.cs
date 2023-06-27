using SteamStorage.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamStorage.Resources.Controls
{
    public partial class NumericTextBox : UserControl
    {
        #region Fields
        public static readonly DependencyProperty InputTypeProperty = DependencyProperty.Register(
            name: "InputType", propertyType: typeof(Type), ownerType: typeof(NumericTextBox)
        );
        public static readonly DependencyProperty NumericTextBoxStyleProperty = DependencyProperty.Register(
            name: "NumericTextBoxStyle", propertyType: typeof(Style), ownerType: typeof(NumericTextBox)
        );
        public static readonly DependencyProperty NumericTextProperty = DependencyProperty.Register(
            name: "NumericText", propertyType: typeof(string), ownerType: typeof(NumericTextBox)
        );
        #endregion Fields

        #region Properties
        public Type InputType
        {
            get => (Type)GetValue(dp: InputTypeProperty);
            set => SetValue(dp: InputTypeProperty, value: value);
        }
        public Style NumericTextBoxStyle
        {
            get => (Style)GetValue(dp: NumericTextBoxStyleProperty);
            set => SetValue(dp: NumericTextBoxStyleProperty, value: value);
        }
        public string NumericText
        {
            get => (string)GetValue(dp: NumericTextProperty);
            set => SetValue(dp: NumericTextProperty, value: value);
        }
        #endregion Properties

        #region Constructor
        public NumericTextBox()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region Methods
        private bool IsTextAllowed(string text)
        {
            try
            {
                TypeConvert.Convert<InputType>(text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = tb.Text;
            if (!IsTextAllowed(text))
            {

            }
        }
        private void MainTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
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
