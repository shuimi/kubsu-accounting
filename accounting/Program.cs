using accounting.Services;

namespace accounting
{

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new EnterpriseAccountingMainForm());
        }
    }
}