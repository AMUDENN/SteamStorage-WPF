using System.Windows;
using System.Windows.Controls;

namespace SteamStorage.Services.ToolTip
{
    public static class ToolTipServiceHelper
    {
        public static void SetToolTipInitialShowDelay(int delay)
        {
            ToolTipService.InitialShowDelayProperty
                .OverrideMetadata(typeof(FrameworkElement),
                                  new FrameworkPropertyMetadata(delay));
        }
    }
}
