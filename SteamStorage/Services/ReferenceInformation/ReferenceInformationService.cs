using SteamStorage.Utilities;
using System.Diagnostics;

namespace SteamStorage.Services.ReferenceInformation
{
    public class ReferenceInformationService : IReferenceInformationService
    {
        public void OpenReferenceInformation()
        {
            Process.Start(new ProcessStartInfo(ProgramConstants.ReferenceInformationPath) { UseShellExecute = true });
        }
    }
}
