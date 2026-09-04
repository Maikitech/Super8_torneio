using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PadelSuper8.Models;

/// <summary>
/// Representa um Atleta (no modo individual) ou uma Dupla (no modo duplas fixas).
/// </summary>
public class Jogador : INotifyPropertyChanged
{
    private int _posicao;
    private string _nome = string.Empty;
    private string? _grupo;
    private int _jogos;
    private int _vitorias;
    private int _derrotas;
    private int _gamesPro;
    private int _gamesContra;
    private int _pontos;

    public int Id { get; set; }

    public int Posicao
    {
        get => _posicao;
        set => SetField(ref _posicao, value);
    }

    public string Nome
    {
        get => _nome;
        set => SetField(ref _nome, value);
    }

    public string? Grupo
    {
        get => _grupo;
        set => SetField(ref _grupo, value);
    }

    public int Jogos
    {
        get => _jogos;
        set => SetField(ref _jogos, value);
    }

    public int Vitorias
    {
        get => _vitorias;
        set
        {
            if (SetField(ref _vitorias, value))
            {
                OnPropertyChanged(nameof(Aproveitamento));
            }
        }
    }

    public int Derrotas
    {
        get => _derrotas;
        set => SetField(ref _derrotas, value);
    }

    private int _setsVencidos;
    private int _setsPerdidos;

    public int SetsVencidos
    {
        get => _setsVencidos;
        set
        {
            if (SetField(ref _setsVencidos, value))
            {
                OnPropertyChanged(nameof(SaldoSets));
            }
        }
    }

    public int SetsPerdidos
    {
        get => _setsPerdidos;
        set
        {
            if (SetField(ref _setsPerdidos, value))
            {
                OnPropertyChanged(nameof(SaldoSets));
            }
        }
    }

    public int SaldoSets => SetsVencidos - SetsPerdidos;

    public int GamesPro
    {
        get => _gamesPro;
        set
        {
            if (SetField(ref _gamesPro, value))
            {
                OnPropertyChanged(nameof(SaldoGames));
            }
        }
    }

    public int GamesContra
    {
        get => _gamesContra;
        set
        {
            if (SetField(ref _gamesContra, value))
            {
                OnPropertyChanged(nameof(SaldoGames));
            }
        }
    }

    public int SaldoGames => GamesPro - GamesContra;

    public int Pontos
    {
        get => _pontos;
        set => SetField(ref _pontos, value);
    }

    public string Aproveitamento
    {
        get
        {
            if (Jogos == 0) return "0%";
            var pct = (double)Vitorias / Jogos * 100.0;
            return $"{pct:F0}%";
        }
    }

    public void ZerarEstatisticas()
    {
        Jogos = 0;
        Vitorias = 0;
        Derrotas = 0;
        SetsVencidos = 0;
        SetsPerdidos = 0;
        GamesPro = 0;
        GamesContra = 0;
        Pontos = 0;
        Posicao = 0;
        OnPropertyChanged(nameof(SaldoSets));
        OnPropertyChanged(nameof(SaldoGames));
        OnPropertyChanged(nameof(Aproveitamento));
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
