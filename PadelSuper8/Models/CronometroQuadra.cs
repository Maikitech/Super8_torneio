using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

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
        }
    }

    public void Resetar(int minutos = 20)
    {
        _timer.Stop();
        IsRodando = false;
        _tempoAtual = TimeSpan.FromMinutes(minutos);
        TempoEsgotado = false;
        OnPropertyChanged(nameof(TempoFormatado));
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
