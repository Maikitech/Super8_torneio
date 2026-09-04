using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using PadelSuper8.Services;

namespace PadelSuper8.Models;

public class CronometroQuadra : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _tempoAtual;
    private readonly TimeSpan _tempoPadrao;
    private bool _isRodando;
    private bool _tempoEsgotado;
    private readonly int _quadraNumero;

    public CronometroQuadra(int quadraNumero, int minutosPadrao = 20)
    {
        _quadraNumero = quadraNumero;
        _tempoPadrao = TimeSpan.FromMinutes(minutosPadrao);
        _tempoAtual = _tempoPadrao;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
    }

    public int QuadraNumero => _quadraNumero;

    public string TempoFormatado => _tempoAtual.ToString(@"mm\:ss");

    public bool IsRodando
    {
        get => _isRodando;
        set => SetField(ref _isRodando, value);
    }

    public bool TempoEsgotado
    {
        get => _tempoEsgotado;
        set => SetField(ref _tempoEsgotado, value);
    }

    public void IniciarOuPausar()
    {
        if (IsRodando)
        {
            _timer.Stop();
            IsRodando = false;
            LogService.Info($"Cronômetro da Quadra {_quadraNumero} pausado em {TempoFormatado}.", "Cronômetro");
        }
        else
        {
            if (_tempoAtual <= TimeSpan.Zero)
            {
                _tempoAtual = _tempoPadrao;
                TempoEsgotado = false;
            }
            _timer.Start();
            IsRodando = true;
            LogService.Info($"Cronômetro da Quadra {_quadraNumero} iniciado/retomado. Tempo atual: {TempoFormatado}.", "Cronômetro");
        }
    }

    public void Resetar(int minutos = 20)
    {
        _timer.Stop();
        IsRodando = false;
        _tempoAtual = TimeSpan.FromMinutes(minutos);
        TempoEsgotado = false;
        OnPropertyChanged(nameof(TempoFormatado));
        LogService.Info($"Cronômetro da Quadra {_quadraNumero} resetado para {minutos} minutos ({TempoFormatado}).", "Cronômetro");
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_tempoAtual > TimeSpan.Zero)
        {
            _tempoAtual = _tempoAtual.Subtract(TimeSpan.FromSeconds(1));
            OnPropertyChanged(nameof(TempoFormatado));

            if (_tempoAtual <= TimeSpan.Zero)
            {
                _timer.Stop();
                IsRodando = false;
                TempoEsgotado = true;
                LogService.Warn($"TEMPO ESGOTADO! O cronômetro da Quadra {_quadraNumero} chegou a 00:00.", null, "Cronômetro");
            }
        }
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
