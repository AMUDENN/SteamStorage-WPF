﻿using System;
using System.IO;
using System.Text;

namespace SteamStorage.Services.Logger
{
    public class LoggerService : ILoggerService
    {
        #region Fields
        private readonly StreamWriter _innerWriter;
        #endregion Fields

        #region Constructor
        public LoggerService(string logPath)
        {
            if (!File.Exists(logPath))
            {
                Directory.CreateDirectory(logPath.Remove(logPath.LastIndexOf('/')));
                File.Create(logPath).Close();
            }
            _innerWriter = new StreamWriter(logPath, true, Encoding.UTF8, 8192);
        }
        #endregion Constructor

        #region Dispose
        public void Dispose()
        {
            _innerWriter.Flush();
            _innerWriter.Dispose();
        }
        #endregion Dispose

        #region Methods
        public void WriteMessage(string message)
        {
            _innerWriter.WriteLine($"[{DateTime.Now}]: {message}");
            _innerWriter.Flush();
        }
        public void WriteMessage(string message, Type sender)
        {
            _innerWriter.WriteLine($"[{DateTime.Now}]: {message}" +
                                   $"\n\tSender: {sender.Name}");
            _innerWriter.Flush();
        }
        public void WriteMessage(Exception exception)
        {
            _innerWriter.WriteLine($"[{DateTime.Now}]: " +
                                   $"\n\tMessage: {exception.Message} " +
                                   $"\n\tSource: {exception.Source}" +
                                   $"\n\tStackTrace: {exception.StackTrace}" +
                                   $"\n\tTargetSite: {exception.TargetSite}");
            _innerWriter.Flush();
        }
        public void WriteMessage(Exception exception, string message)
        {
            _innerWriter.WriteLine($"[{DateTime.Now}]: {message}" +
                                   $"\n\tMessage: {exception.Message} " +
                                   $"\n\tSource: {exception.Source}" +
                                   $"\n\tStackTrace: {exception.StackTrace}" +
                                   $"\n\tTargetSite: {exception.TargetSite}");
            _innerWriter.Flush();
        }
        #endregion Methods
    }
}
