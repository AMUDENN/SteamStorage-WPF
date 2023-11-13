using System;

namespace SteamStorage.Services.Logger
{
    public interface ILoggerService : IDisposable
    {
        public void WriteMessage(string message);
        public void WriteMessage(string message, Type sender);
        public void WriteMessage(Exception exception);
        public void WriteMessage(Exception exception, string message);
    }
}
