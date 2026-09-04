using System.IO;
using PadelSuper8.Models;
using PadelSuper8.Services;
using Xunit;

namespace PadelSuper8.Tests;

public class TorneioPadelTests
{
    [Fact]
    public void Super8Rotativo_DeveGerar7RodadasComTodasAs28DuplasUnicas()
    {
        // Arrange
        var gerador = new GeradorTorneioService();
        var jogadores = Enumerable.Range(1, 8)
            .Select(i => new Jogador { Id = i, Nome = $"Jogador {i}" })
            .ToList();

        // Act
        var rodadas = gerador.GerarSuper8Rotativo(jogadores);

        // Assert
        Assert.Equal(7, rodadas.Count);

        var duplasFormadas = new HashSet<string>();

        foreach (var r in rodadas)
        {
            Assert.Equal(2, r.Partidas.Count); // 2 partidas simultâneas por rodada

            // Em cada rodada, todos os 8 jogadores devem jogar exatamente 1 vez
            var jogadoresDaRodada = new List<int>();

            foreach (var partida in r.Partidas)
            {
                Assert.NotNull(partida.Jogador1A);
                Assert.NotNull(partida.Jogador1B);
                Assert.NotNull(partida.Jogador2A);
                Assert.NotNull(partida.Jogador2B);

                jogadoresDaRodada.Add(partida.Jogador1A!.Id);
                jogadoresDaRodada.Add(partida.Jogador1B!.Id);
                jogadoresDaRodada.Add(partida.Jogador2A!.Id);
                jogadoresDaRodada.Add(partida.Jogador2B!.Id);

                // Registra dupla 1
                var d1 = new[] { partida.Jogador1A.Id, partida.Jogador1B.Id }.OrderBy(x => x).ToArray();
                duplasFormadas.Add($"{d1[0]}-{d1[1]}");

                // Registra dupla 2
                var d2 = new[] { partida.Jogador2A.Id, partida.Jogador2B.Id }.OrderBy(x => x).ToArray();
                duplasFormadas.Add($"{d2[0]}-{d2[1]}");
            }

            Assert.Equal(8, jogadoresDaRodada.Distinct().Count());
        }

        // C(8, 2) = 28 duplas possíveis no total
        Assert.Equal(28, duplasFormadas.Count);
    }

    [Fact]
    public void CalculadoraClassificacao_DeveCalcularPontosEVitoriasCorretamente()
    {
        // Arrange
        var calculadora = new CalculadoraClassificacao();
        var j1 = new Jogador { Id = 1, Nome = "J1" };
        var j2 = new Jogador { Id = 2, Nome = "J2" };
        var j3 = new Jogador { Id = 3, Nome = "J3" };
        var j4 = new Jogador { Id = 4, Nome = "J4" };

        var lista = new List<Jogador> { j1, j2, j3, j4 };

        var rodada = new Rodada { Numero = 1, Titulo = "Rodada 1" };
        var partida = new Partida
        {
            Id = 1,
            NumeroRodada = 1,
            NumeroQuadra = 1,
            Jogador1A = j1,
            Jogador1B = j2,
            Jogador2A = j3,
            Jogador2B = j4,
            Placar1 = 6,
            Placar2 = 2,
            Status = StatusPartida.Finalizada
        };
        rodada.Partidas.Add(partida);

        // Act
        calculadora.Recalcular(lista, new[] { rodada });

        // Assert
        Assert.Equal(3, j1.Pontos);
        Assert.Equal(1, j1.Vitorias);
        Assert.Equal(4, j1.SaldoGames); // 6 - 2 = +4
        Assert.Equal(1, j1.Posicao);

        Assert.Equal(3, j2.Pontos);
        Assert.Equal(1, j2.Vitorias);

        Assert.Equal(0, j3.Pontos);
        Assert.Equal(1, j3.Derrotas);
        Assert.Equal(-4, j3.SaldoGames);
    }

    [Fact]
    public void LogService_DeveCriarArquivoERegistrarMensagensComTimestamp()
    {
        // Act
        LogService.RegistrarInicializacao();
        LogService.Info("Teste de mensagem informativa para auditoria", "TesteUnitario");
        LogService.Warn("Teste de aviso para auditoria", null, "TesteUnitario");
        LogService.Error("Teste de erro registrado para auditoria", new InvalidOperationException("Falha simulada"), "TesteUnitario");

        // Assert
        Assert.True(File.Exists(LogService.CaminhoArquivoLogAtual), "O arquivo de log diário deve existir.");
        var conteudo = File.ReadAllText(LogService.CaminhoArquivoLogAtual);
        Assert.Contains("=== INICIALIZANDO PADEL SUPER 8 PRO", conteudo);
        Assert.Contains("[INFO ] [TesteUnitario] Teste de mensagem informativa", conteudo);
        Assert.Contains("[WARN ] [TesteUnitario] Teste de aviso", conteudo);
        Assert.Contains("[ERROR] [TesteUnitario] Teste de erro registrado", conteudo);
        Assert.Contains("Falha simulada", conteudo);
    }
}
