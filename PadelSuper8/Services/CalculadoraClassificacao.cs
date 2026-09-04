using PadelSuper8.Models;

namespace PadelSuper8.Services;

public class CalculadoraClassificacao
{
    /// <summary>
    /// Recalcula as estatísticas de todos os participantes com base nas partidas finalizadas.
    /// </summary>
    public void Recalcular(IEnumerable<Jogador> participantes, IEnumerable<Rodada> rodadas)
    {
        var listaParticipantes = participantes.ToList();

        // 1. Zera estatísticas
        foreach (var p in listaParticipantes)
        {
            p.ZerarEstatisticas();
        }

        var dic = listaParticipantes.ToDictionary(p => p.Id);

        // 2. Processa todas as partidas concluídas
        foreach (var rodada in rodadas)
        {
            foreach (var partida in rodada.Partidas)
            {
                if (partida.Status != StatusPartida.Finalizada)
                    continue;

                ProcessarPartida(partida, dic);
            }
        }

        // 3. Ordena e atribui posições com base nos critérios oficiais de desempate:
        //    Pontos > Vitórias > Saldo de Sets > Sets Pró > Saldo de Games > Games Pró > Menos Games Contra
        var ordenados = listaParticipantes
            .OrderByDescending(p => p.Pontos)
            .ThenByDescending(p => p.Vitorias)
            .ThenByDescending(p => p.SaldoSets)
            .ThenByDescending(p => p.SetsVencidos)
            .ThenByDescending(p => p.SaldoGames)
            .ThenByDescending(p => p.GamesPro)
            .ThenBy(p => p.GamesContra)
            .ThenBy(p => p.Nome)
            .ToList();

        for (int i = 0; i < ordenados.Count; i++)
        {
            ordenados[i].Posicao = i + 1;
        }
    }

    private void ProcessarPartida(Partida partida, Dictionary<int, Jogador> dic)
    {
        var time1 = ObterParticipantesTime1(partida, dic);
        var time2 = ObterParticipantesTime2(partida, dic);

        int p1 = partida.Placar1;
        int p2 = partida.Placar2;

        bool time1Venceu = p1 > p2;
        bool time2Venceu = p2 > p1;
        bool empate = p1 == p2;

        // Atualiza Time 1
        foreach (var j in time1)
        {
            j.Jogos++;
            j.GamesPro += p1;
            j.GamesContra += p2;

            if (time1Venceu)
            {
                j.Vitorias++;
                j.SetsVencidos++;
                j.Pontos += 3; // 3 pontos por vitória
            }
            else if (empate)
            {
                j.Pontos += 1; // 1 ponto por empate
            }
            else
            {
                j.Derrotas++;
                j.SetsPerdidos++;
            }
        }

        // Atualiza Time 2
        foreach (var j in time2)
        {
            j.Jogos++;
            j.GamesPro += p2;
            j.GamesContra += p1;

            if (time2Venceu)
            {
                j.Vitorias++;
                j.SetsVencidos++;
                j.Pontos += 3;
            }
            else if (empate)
            {
                j.Pontos += 1;
            }
            else
            {
                j.Derrotas++;
                j.SetsPerdidos++;
            }
        }
    }

    private List<Jogador> ObterParticipantesTime1(Partida partida, Dictionary<int, Jogador> dic)
    {
        var lista = new List<Jogador>();
        if (partida.Dupla1 != null && dic.TryGetValue(partida.Dupla1.Id, out var d1))
        {
            lista.Add(d1);
        }
        else
        {
            if (partida.Jogador1A != null && dic.TryGetValue(partida.Jogador1A.Id, out var j1a)) lista.Add(j1a);
            if (partida.Jogador1B != null && dic.TryGetValue(partida.Jogador1B.Id, out var j1b)) lista.Add(j1b);
        }
        return lista;
    }

    private List<Jogador> ObterParticipantesTime2(Partida partida, Dictionary<int, Jogador> dic)
    {
        var lista = new List<Jogador>();
        if (partida.Dupla2 != null && dic.TryGetValue(partida.Dupla2.Id, out var d2))
        {
            lista.Add(d2);
        }
        else
        {
            if (partida.Jogador2A != null && dic.TryGetValue(partida.Jogador2A.Id, out var j2a)) lista.Add(j2a);
            if (partida.Jogador2B != null && dic.TryGetValue(partida.Jogador2B.Id, out var j2b)) lista.Add(j2b);
        }
        return lista;
    }
}
