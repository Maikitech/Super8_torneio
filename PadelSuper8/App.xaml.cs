using System.Windows;
using PadelSuper8.Services;

namespace PadelSuper8;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Inicializa o sistema de auditoria e grava o cabeçalho técnico
        LogService.RegistrarInicializacao();
        LogService.Info($"Aplicação iniciada com sucesso. Argumentos de linha de comando: {string.Join(" ", e.Args)}", "CicloDeVida");

        // Captura e registra exceções da thread de UI (WPF Dispatcher)
        DispatcherUnhandledException += (s, args) =>
        {
            LogService.Error($"Exceção não tratada na interface (Dispatcher): {args.Exception.Message}", args.Exception, "DispatcherUI");

            MessageBox.Show(
                $"Ocorreu um imprevisto na operação:\n\n{args.Exception.Message}\n\nDetalhes foram registrados no log em:\n{LogService.CaminhoArquivoLogAtual}",
                "Aviso do Sistema - Padel Super 8",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            args.Handled = true; // Impede que o aplicativo feche inesperadamente
        };

        // Captura exceções em segundo plano / threads não-UI
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                LogService.Error($"Exceção crítica de domínio não tratada (IsTerminating={args.IsTerminating}): {ex.Message}", ex, "AppDomain");
            }
            else
            {
                LogService.Error($"Exceção crítica não-gerenciada: {args.ExceptionObject}", null, "AppDomain");
            }
        };

        // Captura exceções assíncronas em Tasks não observadas
        TaskScheduler.UnobservedTaskException += (s, args) =>
        {
            LogService.Error($"Exceção assíncrona não observada em Task: {args.Exception.Message}", args.Exception, "TaskScheduler");
            args.SetObserved();
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        LogService.Info($"Aplicação finalizando com código de saída {e.ApplicationExitCode}.", "CicloDeVida");
        LogService.RegistrarEncerramento();
        base.OnExit(e);
    }
}
