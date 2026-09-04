using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PadelSuper8.Data;
using PadelSuper8.Models;
using PadelSuper8.Services;

namespace PadelSuper8.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly GeradorTorneioService _geradorService = new();
    private readonly CalculadoraClassificacao _calculadora = new();
    private readonly TorneioRepository _repositorio = new();

    private int _torneioIdAtual = 0;
    private string _nomeTorneio = "Torneio Super 8 Padel Arena";
    private TipoTorneio _tipoSelecionado = TipoTorneio.RotativoIndividual;
    private int _telaAtual = 0; // 0: Cadastro, 1: Torneio
    private bool _modoPodio = false;
    private bool _modoHistorico = false;
    private Rodada? _rodadaSelecionada;
    private string _mensagemAlerta = string.Empty;
    private bool _temAlerta;

    public CronometroQuadra CronometroQuadra1 { get; } = new(1, 20);
    public CronometroQuadra CronometroQuadra2 { get; } = new(2, 20);

    public ObservableCollection<TorneioDb> ListaTorneiosSalvos { get; set; } = new();

    public MainViewModel()
    {
        // 8 slots iniciais de nomes para o cadastro
        for (int i = 1; i <= 8; i++)
        {
            SlotsCadastro.Add(new SlotCadastro { Numero = i, Nome = string.Empty });
        }

        // Inicializa com dados de exemplo recomendados
        CarregarExemploPadrao();

        // Comandos
        PreencherExemploCommand = new RelayCommand(CarregarExemploPadrao);
        IniciarTorneioCommand = new RelayCommand(IniciarTorneio);
        VoltarCadastroCommand = new RelayCommand(() => TelaAtual = 0);
        SelecionarRodadaCommand = new RelayCommand(param =>
        {
            if (param is Rodada r) RodadaSelecionada = r;
        });

        IncrementarP1Command = new RelayCommand(param => ModificarPlacar(param, 1, 1));
        DecrementarP1Command = new RelayCommand(param => ModificarPlacar(param, 1, -1));
        IncrementarP2Command = new RelayCommand(param => ModificarPlacar(param, 2, 1));
        DecrementarP2Command = new RelayCommand(param => ModificarPlacar(param, 2, -1));
        AlternarFinalizadaCommand = new RelayCommand(AlternarPartidaFinalizada);

        GerarMataMataCommand = new RelayCommand(GerarProximaFaseMataMata);
        GerarFinalRotativoCommand = new RelayCommand(GerarFinalRotativo);
        CopiarWhatsAppCommand = new RelayCommand(CopiarParaWhatsApp);
        SalvarBancoCommand = new RelayCommand(SalvarNoBanco);

        AbrirPodioCommand = new RelayCommand(() => ModoPodio = true);
        FecharPodioCommand = new RelayCommand(() => ModoPodio = false);

        AbrirHistoricoCommand = new RelayCommand(CarregarHistoricoTorneios);
        FecharHistoricoCommand = new RelayCommand(() => ModoHistorico = false);
        ExcluirTorneioCommand = new RelayCommand(ExcluirTorneio);

        GerarSumulaCommand = new RelayCommand(GerarSumulaImpressao);
    }

    public string NomeTorneio
    {
        get => _nomeTorneio;
        set => SetField(ref _nomeTorneio, value);
    }

    public TipoTorneio TipoSelecionado
    {
        get => _tipoSelecionado;
        set
        {
            if (SetField(ref _tipoSelecionado, value))
            {
                OnPropertyChanged(nameof(IsRotativo));
                OnPropertyChanged(nameof(IsDuplasFixas));
                OnPropertyChanged(nameof(DescricaoModo));
                CarregarExemploPadrao();
            }
        }
    }

    public bool IsRotativo
    {
        get => TipoSelecionado == TipoTorneio.RotativoIndividual;
        set { if (value) TipoSelecionado = TipoTorneio.RotativoIndividual; }
    }

    public bool IsDuplasFixas
    {
        get => TipoSelecionado == TipoTorneio.DuplasFixas;
        set { if (value) TipoSelecionado = TipoTorneio.DuplasFixas; }
    }

    public string DescricaoModo => IsRotativo
        ? "8 Atletas individuais com rodízio perfeito de parceiros em 7 rodadas (Americano)."
        : "8 Duplas fixas divididas em Grupo A e Grupo B + Semifinais e Finais eliminatórias.";

    public int TelaAtual
    {
        get => _telaAtual;
        set
        {
            if (SetField(ref _telaAtual, value))
            {
                OnPropertyChanged(nameof(IsTelaCadastro));
                OnPropertyChanged(nameof(IsTelaTorneio));
            }
        }
    }

    public bool IsTelaCadastro => TelaAtual == 0;
    public bool IsTelaTorneio => TelaAtual == 1;

    public ObservableCollection<SlotCadastro> SlotsCadastro { get; set; } = new();
    public ObservableCollection<Jogador> Participantes { get; set; } = new();
    public ObservableCollection<Rodada> Rodadas { get; set; } = new();

    public Rodada? RodadaSelecionada
    {
        get => _rodadaSelecionada;
        set => SetField(ref _rodadaSelecionada, value);
    }

    public ObservableCollection<Jogador> ClassificacaoGeral { get; set; } = new();
    public ObservableCollection<Jogador> ClassificacaoGrupoA { get; set; } = new();
    public ObservableCollection<Jogador> ClassificacaoGrupoB { get; set; } = new();

    public bool PodeGerarSemifinais => IsDuplasFixas && Rodadas.Count == 3 && Rodadas.All(r => r.TodasFinalizadas);
    public bool PodeGerarFinais => IsDuplasFixas && Rodadas.Count == 4 && Rodadas[3].TodasFinalizadas;
    public bool PodeGerarFinalRotativo => IsRotativo && Rodadas.Count == 7 && Rodadas.All(r => r.TodasFinalizadas);

    public bool ModoPodio
    {
        get => _modoPodio;
        set => SetField(ref _modoPodio, value);
    }

    public bool ModoHistorico
    {
        get => _modoHistorico;
        set => SetField(ref _modoHistorico, value);
    }

    public Jogador? Campeao1 => ClassificacaoGeral.FirstOrDefault(p => p.Posicao == 1);
    public Jogador? Campeao2 => ClassificacaoGeral.FirstOrDefault(p => p.Posicao == 2);
    public Jogador? Campeao3 => ClassificacaoGeral.FirstOrDefault(p => p.Posicao == 3);

    public int TotalJogos => Rodadas.SelectMany(r => r.Partidas).Count();
    public int TotalJogosFinalizados => Rodadas.SelectMany(r => r.Partidas).Count(p => p.EstaFinalizada);

    public string MensagemAlerta
    {
        get => _mensagemAlerta;
        set => SetField(ref _mensagemAlerta, value);
    }

    public bool TemAlerta
    {
        get => _temAlerta;
        set => SetField(ref _temAlerta, value);
    }

    // Comandos
    public ICommand PreencherExemploCommand { get; }
    public ICommand IniciarTorneioCommand { get; }
    public ICommand VoltarCadastroCommand { get; }
    public ICommand SelecionarRodadaCommand { get; }
    public ICommand IncrementarP1Command { get; }
    public ICommand DecrementarP1Command { get; }
    public ICommand IncrementarP2Command { get; }
    public ICommand DecrementarP2Command { get; }
    public ICommand AlternarFinalizadaCommand { get; }
    public ICommand GerarMataMataCommand { get; }
    public ICommand GerarFinalRotativoCommand { get; }
    public ICommand CopiarWhatsAppCommand { get; }
    public ICommand SalvarBancoCommand { get; }
    public ICommand AbrirPodioCommand { get; }
    public ICommand FecharPodioCommand { get; }
    public ICommand AbrirHistoricoCommand { get; }
    public ICommand FecharHistoricoCommand { get; }
    public ICommand ExcluirTorneioCommand { get; }
    public ICommand GerarSumulaCommand { get; }

    public void CarregarExemploPadrao()
    {
        if (IsRotativo)
        {
            string[] atletas = {
                "Ale Galán", "Arturo Coello", "Agustín Tapia", "Fede Chingotto",
                "Paquito Navarro", "Juan Lebrón", "Franco Stupaczuk", "Martin Di Nenno"
            };
            for (int i = 0; i < 8; i++)
            {
                SlotsCadastro[i].Nome = atletas[i];
            }
        }
        else
        {
            string[] duplas = {
                "Coello & Tapia", "Galán & Chingotto", "Lebrón & Paquito", "Stupa & Di Nenno",
                "Yanguas & Garrido", "Sanz & Nieto", "Belasteguín & Tello", "Bergamini & Ruiz"
            };
            for (int i = 0; i < 8; i++)
            {
                SlotsCadastro[i].Nome = duplas[i];
            }
        }
    }

    private void IniciarTorneio()
    {
        try
        {
            // Valida se os 8 nomes foram preenchidos
            for (int i = 0; i < 8; i++)
            {
                if (string.IsNullOrWhiteSpace(SlotsCadastro[i].Nome))
                {
                    MostrarAlerta($"Por favor, preencha o nome do participante #{i + 1}.");
                    return;
                }
            }

            Participantes.Clear();
            for (int i = 0; i < 8; i++)
            {
                Participantes.Add(new Jogador
                {
                    Id = i + 1,
                    Nome = SlotsCadastro[i].Nome.Trim()
                });
            }

            Rodadas.Clear();
            if (IsRotativo)
            {
                var rodadasGeradas = _geradorService.GerarSuper8Rotativo(Participantes.ToList());
                foreach (var r in rodadasGeradas) Rodadas.Add(r);
            }
            else
            {
                var rodadasGeradas = _geradorService.GerarSuper8DuplasFixas(Participantes.ToList());
                foreach (var r in rodadasGeradas) Rodadas.Add(r);
            }

            RodadaSelecionada = Rodadas.FirstOrDefault();
            AtualizarClassificacao();

            // Salva estado inicial no SQLite de forma protegida
            try
            {
                _torneioIdAtual = _repositorio.SalvarTorneio(0, NomeTorneio, TipoSelecionado, Participantes, Rodadas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gravar no SQLite: {ex.Message}");
            }

            TelaAtual = 1; // Vai para tela de jogos
            MostrarAlerta("Torneio iniciado! Lance os placares e clique em 'Lançar Resultado'.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível iniciar o torneio:\n\n{ex.Message}", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ModificarPlacar(object? param, int time, int delta)
    {
        if (param is Partida partida)
        {
            if (time == 1)
            {
                if (delta > 0) partida.IncrementarPlacar1();
                else partida.DecrementarPlacar1();
            }
            else
            {
                if (delta > 0) partida.IncrementarPlacar2();
                else partida.DecrementarPlacar2();
            }

            AtualizarClassificacao();
        }
    }

    private void AlternarPartidaFinalizada(object? param)
    {
        if (param is Partida partida)
        {
            partida.AlternarFinalizada();
            AtualizarClassificacao();
            
            // Persiste no SQLite
            SalvarNoBanco();

            if (partida.EstaFinalizada)
            {
                MostrarAlerta($"✅ Resultado Lançado! [{partida.NomeQuadra}] {partida.NomeDupla1} {partida.Placar1} x {partida.Placar2} {partida.NomeDupla2}. Pontos somados na classificação e gravados no SQLite!");
            }
            else
            {
                MostrarAlerta($"Partida da [{partida.NomeQuadra}] reaberta para edição. Os pontos foram recalculados temporariamente.");
            }

            VerificarFasesMataMata();
        }
    }

    public void SalvarNoBanco()
    {
        try
        {
            _torneioIdAtual = _repositorio.SalvarTorneio(_torneioIdAtual, NomeTorneio, TipoSelecionado, Participantes, Rodadas);
        }
        catch (Exception ex)
        {
            MostrarAlerta($"Erro ao gravar no banco SQLite: {ex.Message}");
        }
    }

    private void AtualizarClassificacao()
    {
        _calculadora.Recalcular(Participantes, Rodadas);

        // Atualiza coleções observáveis de classificação
        ClassificacaoGeral.Clear();
        foreach (var p in Participantes.OrderBy(x => x.Posicao))
        {
            ClassificacaoGeral.Add(p);
        }

        if (IsDuplasFixas)
        {
            ClassificacaoGrupoA.Clear();
            int posA = 1;
            foreach (var p in Participantes.Where(x => x.Grupo == "A").OrderByDescending(x => x.Pontos).ThenByDescending(x => x.SaldoGames))
            {
                p.Posicao = posA++;
                ClassificacaoGrupoA.Add(p);
            }

            ClassificacaoGrupoB.Clear();
            int posB = 1;
            foreach (var p in Participantes.Where(x => x.Grupo == "B").OrderByDescending(x => x.Pontos).ThenByDescending(x => x.SaldoGames))
            {
                p.Posicao = posB++;
                ClassificacaoGrupoB.Add(p);
            }
        }

        OnPropertyChanged(nameof(PodeGerarSemifinais));
        OnPropertyChanged(nameof(PodeGerarFinais));
        OnPropertyChanged(nameof(PodeGerarFinalRotativo));
        OnPropertyChanged(nameof(Campeao1));
        OnPropertyChanged(nameof(Campeao2));
        OnPropertyChanged(nameof(Campeao3));
        OnPropertyChanged(nameof(TotalJogos));
        OnPropertyChanged(nameof(TotalJogosFinalizados));
    }

    private void VerificarFasesMataMata()
    {
        if (PodeGerarSemifinais)
        {
            MostrarAlerta("Fase de grupos concluída! Clique no botão 'Gerar Semifinais' para continuar.");
        }
        else if (PodeGerarFinais)
        {
            MostrarAlerta("Semifinais concluídas! Clique no botão 'Gerar Grande Final'!");
        }
        else if (PodeGerarFinalRotativo)
        {
            MostrarAlerta("Todas as 7 rodadas concluídas! Você pode gerar a Grande Final com o Top 4.");
        }
    }

    private void GerarProximaFaseMataMata()
    {
        if (PodeGerarSemifinais)
        {
            var p1A = ClassificacaoGrupoA[0];
            var p2A = ClassificacaoGrupoA[1];
            var p1B = ClassificacaoGrupoB[0];
            var p2B = ClassificacaoGrupoB[1];

            int proximoId = Rodadas.SelectMany(r => r.Partidas).Max(p => p.Id) + 1;
            var semi = _geradorService.GerarSemifinais(p1A, p2A, p1B, p2B, proximoId);
            Rodadas.Add(semi);
            RodadaSelecionada = semi;
            MostrarAlerta("Semifinais geradas com sucesso!");
        }
        else if (PodeGerarFinais)
        {
            var semiRodada = Rodadas[3];
            var jogo1 = semiRodada.Partidas[0];
            var jogo2 = semiRodada.Partidas[1];

            var vencedor1 = jogo1.Placar1 > jogo1.Placar2 ? jogo1.Dupla1 : jogo1.Dupla2;
            var perdedor1 = jogo1.Placar1 > jogo1.Placar2 ? jogo1.Dupla2 : jogo1.Dupla1;

            var vencedor2 = jogo2.Placar1 > jogo2.Placar2 ? jogo2.Dupla1 : jogo2.Dupla2;
            var perdedor2 = jogo2.Placar1 > jogo2.Placar2 ? jogo2.Dupla2 : jogo2.Dupla1;

            if (vencedor1 != null && vencedor2 != null && perdedor1 != null && perdedor2 != null)
            {
                int proximoId = Rodadas.SelectMany(r => r.Partidas).Max(p => p.Id) + 1;
                var finais = _geradorService.GerarFinais(vencedor1, vencedor2, perdedor1, perdedor2, proximoId);
                Rodadas.Add(finais);
                RodadaSelecionada = finais;
                MostrarAlerta("Grande Final e Disputa de 3º Lugar geradas com sucesso!");
            }
        }
    }

    private void GerarFinalRotativo()
    {
        if (ClassificacaoGeral.Count >= 4)
        {
            var top4 = ClassificacaoGeral.Take(4).ToList();
            int proximoId = Rodadas.SelectMany(r => r.Partidas).Max(p => p.Id) + 1;
            var finalRot = _geradorService.GerarFinalRotativo(top4[0], top4[1], top4[2], top4[3], proximoId);
            Rodadas.Add(finalRot);
            RodadaSelecionada = finalRot;
            MostrarAlerta("Grande Final dos Top 4 gerada!");
        }
    }

    private void CopiarParaWhatsApp()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"🎾 *{NomeTorneio.ToUpper()}*");
        sb.AppendLine($"📌 *Formato:* {(IsRotativo ? "Super 8 Rotativo (Americano)" : "Super 8 Duplas Fixas")}");
        sb.AppendLine();
        sb.AppendLine("📊 *CLASSIFICAÇÃO ATUAL:*");

        IEnumerable<Jogador> lista = IsRotativo 
            ? ClassificacaoGeral 
            : Participantes.OrderByDescending(p => p.Pontos);
        foreach (var p in lista)
        {
            string medalha = p.Posicao switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => $"{p.Posicao}º"
            };
            sb.AppendLine($"{medalha} {p.Nome} - {p.Pontos} pts | {p.Vitorias}V-{p.Derrotas}D | Sets: {p.SetsVencidos}V-{p.SetsPerdidos}D | Games: {p.GamesPro}GP-{p.GamesContra}GC (SG: {(p.SaldoGames > 0 ? "+" : "")}{p.SaldoGames})");
        }

        if (RodadaSelecionada != null)
        {
            sb.AppendLine();
            sb.AppendLine($"📍 *{RodadaSelecionada.Titulo}:*");
            foreach (var part in RodadaSelecionada.Partidas)
            {
                string status = part.EstaFinalizada ? "✅ Finalizado" : (part.Status == StatusPartida.EmAndamento ? "⏳ Em jogo" : "🕒 A iniciar");
                sb.AppendLine($"• [{part.NomeQuadra}] {part.NomeDupla1} {part.Placar1} x {part.Placar2} {part.NomeDupla2} ({status})");
            }
        }

        sb.AppendLine();
        sb.AppendLine("⚡ _Gerado pelo Padel Super 8 Manager_");

        try
        {
            Clipboard.SetText(sb.ToString());
            MostrarAlerta("📋 Resumo copiado para a Área de Transferência! Pronto para colar no WhatsApp.");
        }
        catch
        {
            MostrarAlerta("Não foi possível acessar a área de transferência diretamente.");
        }
    }

    private void CarregarHistoricoTorneios()
    {
        ListaTorneiosSalvos.Clear();
        var lista = _repositorio.ObterTorneiosSalvos();
        foreach (var t in lista) ListaTorneiosSalvos.Add(t);
        ModoHistorico = true;
    }

    private void ExcluirTorneio(object? param)
    {
        if (param is TorneioDb t)
        {
            _repositorio.ExcluirTorneio(t.Id);
            ListaTorneiosSalvos.Remove(t);
            MostrarAlerta($"Torneio '{t.Nome}' removido do banco.");
        }
    }

    private void GerarSumulaImpressao()
    {
        try
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
            html.AppendLine("<title>Súmula Oficial - " + NomeTorneio + "</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: 'Segoe UI', Arial, sans-serif; margin: 30px; background: #fff; color: #1e293b; }");
            html.AppendLine("h1 { color: #0284c7; border-bottom: 2px solid #0284c7; padding-bottom: 8px; margin-bottom: 4px; }");
            html.AppendLine(".sub { color: #64748b; font-size: 14px; margin-bottom: 24px; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-bottom: 28px; }");
            html.AppendLine("th, td { border: 1px solid #cbd5e1; padding: 8px 12px; text-align: left; font-size: 13px; }");
            html.AppendLine("th { background-color: #f1f5f9; font-weight: bold; }");
            html.AppendLine(".badge { background: #e0f2fe; color: #0369a1; padding: 2px 6px; border-radius: 4px; font-weight: bold; }");
            html.AppendLine(".gold { background: #fef3c7; color: #b45309; }");
            html.AppendLine(".silver { background: #f1f5f9; color: #475569; }");
            html.AppendLine(".bronze { background: #ffedd5; color: #c2410c; }");
            html.AppendLine("@media print { .btn-print { display: none; } }");
            html.AppendLine("</style></head><body>");

            html.AppendLine($"<button class='btn-print' onclick='window.print()' style='float:right; padding:10px 18px; background:#0284c7; color:#fff; border:none; border-radius:6px; cursor:pointer; font-weight:bold;'>🖨️ Imprimir / Salvar PDF</button>");
            html.AppendLine($"<h1>🎾 {NomeTorneio.ToUpper()}</h1>");
            html.AppendLine($"<div class='sub'>Formato: {(IsRotativo ? "Super 8 Rotativo (Americano)" : "Super 8 Duplas Fixas")} | Data: {DateTime.Now:dd/MM/yyyy HH:mm} | Gerado por Padel Super 8 Pro</div>");

            html.AppendLine("<h2>🏆 Classificação Oficial</h2>");
            html.AppendLine("<table><tr><th>Pos</th><th>Participante</th><th>Pts</th><th>J</th><th>V</th><th>D</th><th>Sets (V/P)</th><th>Games Pró (GP)</th><th>Games Contra (GC)</th><th>Saldo Games (SG)</th><th>Aprov</th></tr>");

            IEnumerable<Jogador> ranking = IsRotativo 
                ? ClassificacaoGeral 
                : Participantes.OrderByDescending(p => p.Pontos);
            foreach (var p in ranking)
            {
                string cor = p.Posicao switch { 1 => "gold", 2 => "silver", 3 => "bronze", _ => "" };
                html.AppendLine($"<tr class='{cor}'><td><b>{p.Posicao}º</b></td><td><b>{p.Nome}</b></td><td><b>{p.Pontos} pts</b></td><td>{p.Jogos}</td><td>{p.Vitorias}</td><td>{p.Derrotas}</td><td>{p.SetsVencidos} / {p.SetsPerdidos}</td><td>{p.GamesPro}</td><td>{p.GamesContra}</td><td>{(p.SaldoGames > 0 ? "+" : "")}{p.SaldoGames}</td><td>{p.Aproveitamento}</td></tr>");
            }
            html.AppendLine("</table>");

            html.AppendLine("<h2>📋 Resultados das Partidas</h2>");
            foreach (var r in Rodadas)
            {
                html.AppendLine($"<h3>{r.Titulo}</h3>");
                html.AppendLine("<table><tr><th>Quadra</th><th>Fase</th><th>Dupla 1</th><th>Placar</th><th>Dupla 2</th><th>Status</th></tr>");
                foreach (var part in r.Partidas)
                {
                    string status = part.EstaFinalizada ? "Finalizada" : "Pendente";
                    html.AppendLine($"<tr><td>{part.NomeQuadra}</td><td>{part.TituloFase}</td><td><b>{part.NomeDupla1}</b></td><td style='text-align:center;'><b>{part.Placar1} x {part.Placar2}</b></td><td><b>{part.NomeDupla2}</b></td><td>{status}</td></tr>");
                }
                html.AppendLine("</table>");
            }

            html.AppendLine("</body></html>");

            var tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sumula_Torneio_Padel.html");
            File.WriteAllText(tempFile, html.ToString(), Encoding.UTF8);

            Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            MostrarAlerta("📄 Súmula gerada com sucesso e aberta no navegador!");
        }
        catch (Exception ex)
        {
            MostrarAlerta($"Erro ao gerar súmula: {ex.Message}");
        }
    }

    private void MostrarAlerta(string mensagem)
    {
        MensagemAlerta = mensagem;
        TemAlerta = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class SlotCadastro : INotifyPropertyChanged
{
    private string _nome = string.Empty;

    public int Numero { get; set; }

    public string Nome
    {
        get => _nome;
        set
        {
            if (_nome != value)
            {
                _nome = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nome)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
