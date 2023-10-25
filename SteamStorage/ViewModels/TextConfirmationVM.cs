using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Services.Dialog;

namespace SteamStorage.ViewModels
{
    public class TextConfirmationVM : ObservableObject
    {
        #region Fields
        private string _text;
        private string _confirmationWord;
        private string _inputConfirmationWord;

        private RelayCommand _okCommand;
        private RelayCommand _cancelCommand;
        #endregion Fields

        #region Properties
        public string Text
        {
            get => _text;
            set => _text = value;
        }
        public string ConfirmationWord
        {
            get => _confirmationWord;
            set => _confirmationWord = value;
        }
        public string ConfirmationText
        {
            get => $"Введите слово \"{ConfirmationWord}\" для подтверждения действия";
        }
        public string InputConfirmationWord
        {
            get => _inputConfirmationWord;
            set
            {
                _inputConfirmationWord = value;
                OkCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion Properties

        #region Commands
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand ??= new RelayCommand(DoOkCommand, CanExecuteOkCommand);
            }
        }
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new RelayCommand(DoCancelCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public TextConfirmationVM(string text, string confirmWord)
        {
            Text = text;
            ConfirmationWord = confirmWord;
        }
        #endregion Constructor

        #region Methods
        private void DoOkCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteOkCommand()
        {
            return InputConfirmationWord == ConfirmationWord;
        }
        private void DoCancelCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
