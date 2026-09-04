using System.Windows;

namespace PadelSuper8;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Previne fechamento inesperado do aplicativo em caso de erros não tratados
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show(
                $"Ocorreu um imprevisto na operação:\n\n{args.Exception.Message}\n\nDetalhes técnicos:\n{args.Exception.InnerException?.Message ?? args.Exception.StackTrace}",
                "Aviso do Sistema - Padel Super 8",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            args.Handled = true; // Impede que o aplicativo feche sozinho!
        };
    }
}
