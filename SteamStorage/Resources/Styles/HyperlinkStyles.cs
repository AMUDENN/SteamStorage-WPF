using System.Windows;
using System.Windows.Documents;

namespace SteamStorage.Resources.Styles
{
    public partial class HyperlinkStyles
    {
        private void BrowserHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Hyperlink) return;
            Hyperlink hyperlink = (Hyperlink)sender;
            var sInfo = new System.Diagnostics.ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
