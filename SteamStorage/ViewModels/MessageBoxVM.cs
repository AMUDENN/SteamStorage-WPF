using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Windows;

namespace SteamStorage.ViewModels
{
    public class MessageBoxVM : ObservableObject
    {
        #region Enums
        public enum MessageImages
        {
            Error, Information, Question
        }
        public enum MessageButtons
        {
            Ok, OkCancel
        }
        #endregion Enums

        #region Fields
        private string _text;
        private MessageImages _messageImage;
        private MessageButtons _messageButton;

        private readonly Dictionary<MessageImages, Style> _images = new()
        {
            { MessageImages.Error, Dictionaries.GetStyle("ErrorImage") },
            { MessageImages.Information, Dictionaries.GetStyle("InformationImage") },
            { MessageImages.Question, Dictionaries.GetStyle("QuestionImage") }
        };

        private RelayCommand _okCommand;
        private RelayCommand _cancelCommand;
        #endregion Fields

        #region Properties
        public string Text
        {
            get => _text;
            set => _text = value;
        }
        public MessageImages MessageImage
        {
            get => _messageImage;
            set => _messageImage = value;
        }
        public MessageButtons MessageButton
        {
            get => _messageButton;
            set => _messageButton = value;
        }
        public bool IsCancelVisible
        {
            get => MessageButton == MessageButtons.OkCancel;
        }
        public Style Image
        {
            get => _images[MessageImage];
        }
        #endregion Properties

        #region Commands
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand ??= new RelayCommand(DoOkCommand);
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
        public MessageBoxVM(string text, MessageImages messageImage, MessageButtons messageButton)
        {
            Text = text;
            MessageImage = messageImage;
            MessageButton = messageButton;
        }
        #endregion Constructor

        #region Methods
        private void DoOkCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private void DoCancelCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}