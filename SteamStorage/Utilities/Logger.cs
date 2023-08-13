using System;
using System.IO;
using System.Text;

namespace SteamStorage.Utilities
{
    public class Logger : IDisposable
    {
        #region Fields
        private StreamWriter _InnerWriter;
        #endregion Fields

        #region Constructor
        public Logger(string logPath)
        {
            _InnerWriter = new StreamWriter(logPath, true, Encoding.UTF8, 8192);
        }
        #endregion Constructor

        #region Dispose
        public void Dispose()
        {
            _InnerWriter.Flush();
            _InnerWriter.Dispose();
        }
        #endregion Dispose

        #region Methods
        public void WriteMessage(string message)
        {
            _InnerWriter.WriteLine($"[{DateTime.Now}]: {message}");
            _InnerWriter.Flush();
        }
        public void WriteMessage(string message, Type sender)
        {
            _InnerWriter.WriteLine($"[{DateTime.Now}]: ({sender.Name}) {message}");
            _InnerWriter.Flush();
        }
        #endregion Methods
    }
}
