using System.Collections.ObjectModel;
using PadelSuper8.Models;

namespace PadelSuper8.Services;

public class GeradorTorneioService
{
    /// <summary>
    /// Gera o torneio Super 8 Rotativo (Americano): 8 jogadores, 7 rodadas e 2 quadras.
    /// Algoritmo cíclico de parceria única (cada jogador joga com todos os outros como parceiro exatamente 1 vez).
    /// </summary>
    public ObservableCollection<Rodada> GerarSuper8Rotativo(List<Jogador> jogadores)
    {
        if (jogadores.Count != 8)
            throw new ArgumentException("O Super 8 Rotativo requer exatamente 8 jogadores.");

        var rodadas = new ObservableCollection<Rodada>();
        int partidaId = 1;

        for (int r = 0; r < 7; r++)
        {
            var rodada = new Rodada
            {
                Numero = r + 1,
                Titulo = $"Rodada {r + 1} de 7"
            };

            // Duplas da rodada utilizando fatoração cíclica modulo 7 (jogador 7 é o pivô fixo):
            var p1A = jogadores[7];
            var p1B = jogadores[r];

            var p2A = jogadores[(r + 1) % 7];
            var p2B = jogadores[(r + 6) % 7];

            var p3A = jogadores[(r + 2) % 7];
            var p3B = jogadores[(r + 5) % 7];

            var p4A = jogadores[(r + 3) % 7];
            var p4B = jogadores[(r + 4) % 7];

            // Partida Quadra 1: Dupla 1 vs Dupla 2
            rodada.Partidas.Add(new Partida
            {
                Id = partidaId++,
                NumeroRodada = r + 1,
                NumeroQuadra = 1,
                Fase = FasePartida.RodadaRegular,
                TituloFase = $"Rodada {r + 1} - Quadra 1",
                Jogador1A = p1A,
                Jogador1B = p1B,
                Jogador2A = p2A,
                Jogador2B = p2B
            });

            // Partida Quadra 2: Dupla 3 vs Dupla 4
            rodada.Partidas.Add(new Partida
            {
                Id = partidaId++,
                NumeroRodada = r + 1,
                NumeroQuadra = 2,
                Fase = FasePartida.RodadaRegular,
                TituloFase = $"Rodada {r + 1} - Quadra 2",
                Jogador1A = p3A,
                Jogador1B = p3B,
                Jogador2A = p4A,
                Jogador2B = p4B
            });

            rodadas.Add(rodada);
        }

        return rodadas;
    }

    /// <summary>
    /// Gera a fase de grupos do Super 8 de Duplas Fixas (Grupo A e Grupo B, 4 duplas cada).
    /// </summary>
    public ObservableCollection<Rodada> GerarSuper8DuplasFixas(List<Jogador> duplas)
    {
        if (duplas.Count != 8)
            throw new ArgumentException("O Super 8 de Duplas Fixas requer exatamente 8 duplas.");

        // Atribui grupos
        for (int i = 0; i < 4; i++) duplas[i].Grupo = "A";
        for (int i = 4; i < 8; i++) duplas[i].Grupo = "B";

        var gA = duplas.Take(4).ToList();
        var gB = duplas.Skip(4).Take(4).ToList();

        var rodadas = new ObservableCollection<Rodada>();
        int partidaId = 1;

        // Rodada 1
        var r1 = new Rodada { Numero = 1, Titulo = "Fase de Grupos - Rodada 1" };
        r1.Partidas.Add(CriarPartidaDuplas(partidaId++, 1, 1, FasePartida.GrupoA, "Grupo A", gA[0], gA[1]));
        r1.Partidas.Add(CriarPartidaDuplas(partidaId++, 1, 2, FasePartida.GrupoB, "Grupo B", gB[0], gB[1]));
        r1.Partidas.Add(CriarPartidaDuplas(partidaId++, 1, 1, FasePartida.GrupoA, "Grupo A", gA[2], gA[3]));
        r1.Partidas.Add(CriarPartidaDuplas(partidaId++, 1, 2, FasePartida.GrupoB, "Grupo B", gB[2], gB[3]));
        rodadas.Add(r1);

        // Rodada 2
        var r2 = new Rodada { Numero = 2, Titulo = "Fase de Grupos - Rodada 2" };
        r2.Partidas.Add(CriarPartidaDuplas(partidaId++, 2, 1, FasePartida.GrupoA, "Grupo A", gA[0], gA[2]));
        r2.Partidas.Add(CriarPartidaDuplas(partidaId++, 2, 2, FasePartida.GrupoB, "Grupo B", gB[0], gB[2]));
        r2.Partidas.Add(CriarPartidaDuplas(partidaId++, 2, 1, FasePartida.GrupoA, "Grupo A", gA[1], gA[3]));
        r2.Partidas.Add(CriarPartidaDuplas(partidaId++, 2, 2, FasePartida.GrupoB, "Grupo B", gB[1], gB[3]));
        rodadas.Add(r2);

        // Rodada 3
        var r3 = new Rodada { Numero = 3, Titulo = "Fase de Grupos - Rodada 3" };
        r3.Partidas.Add(CriarPartidaDuplas(partidaId++, 3, 1, FasePartida.GrupoA, "Grupo A", gA[0], gA[3]));
        r3.Partidas.Add(CriarPartidaDuplas(partidaId++, 3, 2, FasePartida.GrupoB, "Grupo B", gB[0], gB[3]));
        r3.Partidas.Add(CriarPartidaDuplas(partidaId++, 3, 1, FasePartida.GrupoA, "Grupo A", gA[1], gA[2]));
        r3.Partidas.Add(CriarPartidaDuplas(partidaId++, 3, 2, FasePartida.GrupoB, "Grupo B", gB[1], gB[2]));
        rodadas.Add(r3);

        return rodadas;
    }

    private Partida CriarPartidaDuplas(int id, int rodada, int quadra, FasePartida fase, string nomeFase, Jogador d1, Jogador d2)
    {
        return new Partida
        {
            Id = id,
            NumeroRodada = rodada,
            NumeroQuadra = quadra,
            Fase = fase,
            TituloFase = $"{nomeFase} - Quadra {quadra}",
            Dupla1 = d1,
            Dupla2 = d2
        };
    }

    /// <summary>
    /// Gera a rodada de semifinais cruzando os classificados dos Grupos A e B.
    /// </summary>
    public Rodada GerarSemifinais(Jogador primeiroA, Jogador segundoA, Jogador primeiroB, Jogador segundoB, int proximoId)
    {
        var rodada = new Rodada
        {
            Numero = 4,
            Titulo = "Fase Final - Semifinais"
        };

        rodada.Partidas.Add(new Partida
        {
            Id = proximoId++,
            NumeroRodada = 4,
            NumeroQuadra = 1,
            Fase = FasePartida.Semifinal,
            TituloFase = "Semifinal 1 (1ºA vs 2ºB)",
            Dupla1 = primeiroA,
            Dupla2 = segundoB
        });

        rodada.Partidas.Add(new Partida
        {
            Id = proximoId++,
            NumeroRodada = 4,
            NumeroQuadra = 2,
            Fase = FasePartida.Semifinal,
            TituloFase = "Semifinal 2 (1ºB vs 2ºA)",
            Dupla1 = primeiroB,
            Dupla2 = segundoA
        });

        return rodada;
    }

    /// <summary>
    /// Gera as finais: Disputa de 3º Lugar e Grande Final.
    /// </summary>
    public Rodada GerarFinais(Jogador finalista1, Jogador finalista2, Jogador terceiro1, Jogador terceiro2, int proximoId)
    {
        var rodada = new Rodada
        {
            Numero = 5,
            Titulo = "Fase Final - Grande Decisão"
        };

        rodada.Partidas.Add(new Partida
        {
            Id = proximoId++,
            NumeroRodada = 5,
            NumeroQuadra = 2,
            Fase = FasePartida.DisputaTerceiro,
            TituloFase = "Disputa do 3º Lugar",
            Dupla1 = terceiro1,
            Dupla2 = terceiro2
        });

        rodada.Partidas.Add(new Partida
        {
            Id = proximoId++,
            NumeroRodada = 5,
            NumeroQuadra = 1,
            Fase = FasePartida.Final,
            TituloFase = "🏆 GRANDE FINAL",
            Dupla1 = finalista1,
            Dupla2 = finalista2
        });

        return rodada;
    }

    /// <summary>
    /// Gera a Grande Final do Super 8 Rotativo com os 4 melhores do ranking geral (1º & 4º vs 2º & 3º).
    /// </summary>
    public Rodada GerarFinalRotativo(Jogador p1, Jogador p2, Jogador p3, Jogador p4, int proximoId)
    {
        var rodada = new Rodada
        {
            Numero = 8,
            Titulo = "🏆 Grande Final dos Top 4"
        };

        rodada.Partidas.Add(new Partida
        {
            Id = proximoId,
            NumeroRodada = 8,
            NumeroQuadra = 1,
            Fase = FasePartida.Final,
            TituloFase = "Final de Campeões (1º & 4º vs 2º & 3º)",
            Jogador1A = p1,
            Jogador1B = p4,
            Jogador2A = p2,
            Jogador2B = p3
        });

        return rodada;
    }
}
