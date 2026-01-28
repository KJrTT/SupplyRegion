using System.Text;
using System.Windows;

namespace SupplyRegion
{
    public partial class App : Application
    {
        static App()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            catch
            {
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}
