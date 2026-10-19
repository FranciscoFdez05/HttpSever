using HttpServer.UI;

namespace HttpServer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => MostrarErrorInesperado(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => MostrarErrorInesperado(e.ExceptionObject as Exception);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        private static void MostrarErrorInesperado(Exception? ex)
        {
            MessageBox.Show(
                $"Se ha producido un error inesperado:{Environment.NewLine}{Environment.NewLine}{ex?.Message}",
                "HttpServer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
