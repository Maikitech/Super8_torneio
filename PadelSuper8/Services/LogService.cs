using System.IO;
using System.Text;

namespace PadelSuper8.Services;

/// <summary>
/// Serviço centralizado de auditoria e logging da aplicação Padel Super 8.
/// Grava todos os eventos, ações do usuário, alterações de placar e erros em arquivos locais diários.
/// </summary>
public static class LogService
{
    private static readonly object _lock = new();
    private static readonly string _pastaLogs;

    static LogService()
    {
        try
        {
            _pastaLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(_pastaLogs))
            {
                Directory.CreateDirectory(_pastaLogs);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Falha ao criar pasta de logs: {ex.Message}");
            _pastaLogs = AppDomain.CurrentDomain.BaseDirectory;
        }
    }

    /// <summary>
    /// Retorna o caminho do arquivo de log da data atual.
    /// </summary>
    public static string CaminhoArquivoLogAtual
    {
        get
        {
            var nomeArquivo = $"padel_super8_{DateTime.Now:yyyy-MM-dd}.log";
            return Path.Combine(_pastaLogs, nomeArquivo);
        }
    }

    /// <summary>
    /// Retorna o diretório onde os logs estão armazenados.
    /// </summary>
    public static string PastaLogs => _pastaLogs;

    public static void Info(string mensagem, string categoria = "Geral")
    {
        Gravar("INFO", categoria, mensagem);
    }

    public static void Warn(string mensagem, Exception? ex = null, string categoria = "Geral")
    {
        var msgCompleta = ex != null 
            ? $"{mensagem} | Detalhes: {ex.Message}" 
            : mensagem;
        Gravar("WARN", categoria, msgCompleta);
    }

    public static void Error(string mensagem, Exception? ex = null, string categoria = "Geral")
    {
        var sb = new StringBuilder();
        sb.Append(mensagem);
        if (ex != null)
        {
            sb.AppendLine();
            sb.AppendLine($"[Exceção] {ex.GetType().FullName}: {ex.Message}");
            if (ex.InnerException != null)
            {
                sb.AppendLine($"[Exceção Interna] {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}");
            }
            sb.AppendLine($"[StackTrace] {ex.StackTrace}");
        }
        Gravar("ERROR", categoria, sb.ToString());
    }

    public static void Debug(string mensagem, string categoria = "Geral")
    {
#if DEBUG
        Gravar("DEBUG", categoria, mensagem);
#endif
    }

    /// <summary>
    /// Registra um cabeçalho completo de inicialização do sistema com dados de hardware, SO e ambiente.
    /// </summary>
    public static void RegistrarInicializacao()
    {
        var separador = new string('=', 80);
        var sb = new StringBuilder();
        sb.AppendLine(separador);
        sb.AppendLine($"=== INICIALIZANDO PADEL SUPER 8 PRO - {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} ===");
        sb.AppendLine(separador);
        sb.AppendLine($"Sistema Operacional : {Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})");
        sb.AppendLine($".NET Runtime        : {Environment.Version}");
        sb.AppendLine($"Diretório Base      : {AppDomain.CurrentDomain.BaseDirectory}");
        sb.AppendLine($"Pasta de Logs       : {_pastaLogs}");
        sb.AppendLine($"Banco de Dados      : {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "padel.db")}");
        sb.AppendLine($"Nome da Máquina     : {Environment.MachineName}");
        sb.AppendLine($"Usuário do Windows  : {Environment.UserName}");
        sb.AppendLine(separador);

        EscreverDireto(sb.ToString());
    }

    /// <summary>
    /// Registra o encerramento da aplicação.
    /// </summary>
    public static void RegistrarEncerramento()
    {
        var separador = new string('-', 80);
        var sb = new StringBuilder();
        sb.AppendLine(separador);
        sb.AppendLine($"=== ENCERRAMENTO DA SESSÃO PADEL SUPER 8 - {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff} ===");
        sb.AppendLine(separador);
        EscreverDireto(sb.ToString());
    }

    private static void Gravar(string nivel, string categoria, string mensagem)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var linha = $"[{timestamp}] [{nivel,-5}] [{categoria}] {mensagem}";

        // Envia também para o Console / Output do Visual Studio
        System.Diagnostics.Trace.WriteLine(linha);

        EscreverDireto(linha + Environment.NewLine);
    }

    private static void EscreverDireto(string texto)
    {
        lock (_lock)
        {
            try
            {
                File.AppendAllText(CaminhoArquivoLogAtual, texto, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Fallback defensivo para nunca quebrar a aplicação caso o arquivo esteja temporariamente bloqueado
                System.Diagnostics.Trace.WriteLine($"[FALHA_LOG] Não foi possível escrever no arquivo de log: {ex.Message}");
            }
        }
    }
}
