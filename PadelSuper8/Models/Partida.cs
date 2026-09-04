using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PadelSuper8.Models;

public class Partida : INotifyPropertyChanged
{
    private int _placar1;
    private int _placar2;
    private StatusPartida _status = StatusPartida.Pendente;
    private string _observacoes = string.Empty;

    public int Id { get; set; }
    public int NumeroRodada { get; set; }
    public int NumeroQuadra { get; set; }
    public string NomeQuadra => $"Quadra {NumeroQuadra}";
    public FasePartida Fase { get; set; } = FasePartida.RodadaRegular;
    public string TituloFase { get; set; } = string.Empty;

    // Participantes no modo Rotativo Individual
    public Jogador? Jogador1A { get; set; }
    public Jogador? Jogador1B { get; set; }
    public Jogador? Jogador2A { get; set; }
    public Jogador? Jogador2B { get; set; }

    // Participantes no modo Duplas Fixas
    public Jogador? Dupla1 { get; set; }
    public Jogador? Dupla2 { get; set; }

    public string NomeDupla1
    {
        get
        {
            if (Dupla1 != null) return Dupla1.Nome;
            if (Jogador1A != null && Jogador1B != null) return $"{Jogador1A.Nome} & {Jogador1B.Nome}";
            return "Dupla 1";
        }
    }

    public string NomeDupla2
    {
        get
        {
            if (Dupla2 != null) return Dupla2.Nome;
            if (Jogador2A != null && Jogador2B != null) return $"{Jogador2A.Nome} & {Jogador2B.Nome}";
            return "Dupla 2";
        }
    }

    public int Placar1
    {
        get => _placar1;
        set
        {
            if (value < 0) value = 0;
            if (SetField(ref _placar1, value))
            {
                AtualizarStatusAutomatico();
                OnPropertyChanged(nameof(PlacarFormatado));
            }
        }
    }

    public int Placar2
    {
        get => _placar2;
        set
        {
            if (value < 0) value = 0;
            if (SetField(ref _placar2, value))
            {
                AtualizarStatusAutomatico();
                OnPropertyChanged(nameof(PlacarFormatado));
            }
        }
    }

    public StatusPartida Status
    {
        get => _status;
        set
        {
            if (SetField(ref _status, value))
            {
                OnPropertyChanged(nameof(EstaFinalizada));
                OnPropertyChanged(nameof(StatusDescricao));
            }
        }
    }

    public bool EstaFinalizada => Status == StatusPartida.Finalizada;

    public string StatusDescricao => Status switch
    {
        StatusPartida.Finalizada => "Finalizada",
        StatusPartida.EmAndamento => "Em Andamento",
        _ => "A Iniciar"
    };

    public string PlacarFormatado => $"{Placar1} x {Placar2}";

    public string Observacoes
    {
        get => _observacoes;
        set => SetField(ref _observacoes, value);
    }

    private void AtualizarStatusAutomatico()
    {
        if (Status != StatusPartida.Finalizada)
        {
            if (Placar1 > 0 || Placar2 > 0)
                Status = StatusPartida.EmAndamento;
            else
                Status = StatusPartida.Pendente;
        }
    }

    public void IncrementarPlacar1() => Placar1++;
    public void DecrementarPlacar1() => Placar1--;
    public void IncrementarPlacar2() => Placar2++;
    public void DecrementarPlacar2() => Placar2--;

    public void AlternarFinalizada()
    {
        Status = Status == StatusPartida.Finalizada 
            ? ((Placar1 > 0 || Placar2 > 0) ? StatusPartida.EmAndamento : StatusPartida.Pendente)
            : StatusPartida.Finalizada;
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
