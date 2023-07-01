using System;
using System.IO;
using System.Text;

namespace SteamStorage.Utilities
{
    public class Logger : IDisposable
    {
        #region Fields
        private StreamWriter InnerWriter;
        #endregion Fields

        #region Constructor
        public Logger()
        {
            InnerWriter = new StreamWriter(Constants.Logpath, true, Encoding.UTF8, 8192);
        }
        #endregion Constructor

        #region Dispose
        public void Dispose()
        {
            InnerWriter.Flush();
            InnerWriter.Dispose();
        }
        #endregion Dispose

        #region Methods
        public void WriteMessage(string message)
        {
            InnerWriter.WriteLine($"[{DateTime.Now}]: {message}");
            InnerWriter.Flush();
        }
        public void WriteMessage(string message, Type sender)
        {
            InnerWriter.WriteLine($"[{DateTime.Now}]: ({sender.Name}) {message}");
            InnerWriter.Flush();
        }
        #endregion Methods
    }
}
