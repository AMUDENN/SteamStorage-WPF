using System;
using System.IO;
using System.Text;

namespace SteamStorage.Utilities
{
    public class Logger : IDisposable
    {
        #region Fields
        private StreamWriter _innerWriter;
        #endregion Fields

        #region Constructor
        public Logger(string logPath)
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
            _innerWriter.WriteLine($"[{DateTime.Now}]: ({sender.Name}) {message}");
            _innerWriter.Flush();
        }
        #endregion Methods
    }
}
