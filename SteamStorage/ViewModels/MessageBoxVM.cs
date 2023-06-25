﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Services;

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
        private string text;
        private MessageImages messageImage;
        private MessageButtons messageButton;

        private RelayCommand okCommand;
        private RelayCommand cancelCommand;
        #endregion Fields

        #region Properties
        public string Text
        {
            get => text;
            set => text = value;
        }
        public MessageImages MessageImage
        {
            get => messageImage;
            set => messageImage = value;
        }
        public MessageButtons MessageButton
        {
            get => messageButton;
            set => messageButton = value;
        }
        public bool IsCancelVisible
        {
            get => MessageButton == MessageButtons.OkCancel;
        }
        #endregion Properties

        #region Commands
        public RelayCommand OkCommand
        {
            get
            {
                return okCommand ??= new RelayCommand(DoOkCommand);
            }
        }
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??= new RelayCommand(DoCancelCommand);
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