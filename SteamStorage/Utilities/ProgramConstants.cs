using System;

namespace SteamStorage.Utilities
{
    public static class ProgramConstants
    {
        public const string Version = "v 0.0.1";
        public const string DateTimeFormat = "yyyy.MM.dd HH:mm:ss";
        public const string DateFormat = "dd.MM.yyyy";
        public const string DateTimeFormatForExport = "dd.MM.yyyy HH_mm_ss";
        public const string DBPath = @"../../../DataBase/SteamStorageDB.db";
        public const string LogPath = @"../../../Logs/logs.txt";
        public const string ExportPath = @"../../../Export/";
        public static string ReferenceInformationPath => $"{Environment.CurrentDirectory}/../../../ReferenceInformation/index.html";
    }
}
