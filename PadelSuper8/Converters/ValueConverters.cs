using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PadelSuper8.Models;

namespace PadelSuper8.Converters;

public class PosicaoToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Ouro = new(Color.FromRgb(251, 191, 36));     // #FBBF24
    private static readonly SolidColorBrush Prata = new(Color.FromRgb(209, 213, 219));   // #D1D5DB
    private static readonly SolidColorBrush Bronze = new(Color.FromRgb(249, 115, 22));   // #F97316
    private static readonly SolidColorBrush Quarto = new(Color.FromRgb(52, 211, 153));   // #34D399
    private static readonly SolidColorBrush Padrao = new(Color.FromRgb(156, 163, 175));  // #9CA3AF

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int pos)
        {
            return pos switch
            {
                1 => Ouro,
                2 => Prata,
                3 => Bronze,
                4 => Quarto,
                _ => Padrao
            };
        }
        return Padrao;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class PosicaoToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int pos)
        {
            return pos switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                4 => "⭐",
                _ => $"{pos}º"
            };
        }
        return "-";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class StatusToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Pendente = new(Color.FromRgb(107, 114, 128));   // Cinza #6B7280
    private static readonly SolidColorBrush Andamento = new(Color.FromRgb(14, 165, 233));   // Azul #0EA5E9
    private static readonly SolidColorBrush Finalizada = new(Color.FromRgb(16, 185, 129));  // Verde #10B981

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StatusPartida status)
        {
            return status switch
            {
                StatusPartida.Finalizada => Finalizada,
                StatusPartida.EmAndamento => Andamento,
                _ => Pendente
            };
        }
        return Pendente;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class SaldoFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int saldo)
        {
            return saldo > 0 ? $"+{saldo}" : saldo.ToString();
        }
        return "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            bool invert = parameter?.ToString()?.Equals("invert", StringComparison.OrdinalIgnoreCase) == true;
            return (b ^ invert) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class FinalizadaToTextoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool finalizada && finalizada)
        {
            return "✔ Resultado Computado (Clique para Reabrir)";
        }
        return "✅ LANÇAR RESULTADO NA TABELA";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class FinalizadaToButtonBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush BotaoNaoFinalizado = new(Color.FromRgb(16, 185, 129)); // Verde Padel
    private static readonly SolidColorBrush BotaoFinalizado = new(Color.FromRgb(51, 65, 85));       // Slate escuro

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool finalizada && finalizada)
        {
            return BotaoFinalizado;
        }
        return BotaoNaoFinalizado;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

