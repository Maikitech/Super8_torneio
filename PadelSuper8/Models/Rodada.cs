using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PadelSuper8.Models;

public class Rodada : INotifyPropertyChanged
{
    private int _numero;
    private string _titulo = string.Empty;

    public int Numero
    {
        get => _numero;
        set => SetField(ref _numero, value);
    }

    public string Titulo
    {
        get => _titulo;
        set => SetField(ref _titulo, value);
    }

    public ObservableCollection<Partida> Partidas { get; set; } = new();

    public bool TodasFinalizadas => Partidas.Count > 0 && Partidas.All(p => p.Status == StatusPartida.Finalizada);

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
