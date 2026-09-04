using Microsoft.EntityFrameworkCore;
using PadelSuper8.Data;
using PadelSuper8.Models;

namespace PadelSuper8.Services;

public class TorneioRepository
{
    public TorneioRepository()
    {
        try
        {
            // Garante que o banco SQLite e suas tabelas existam
            using var db = new PadelDbContext();
            db.Database.EnsureCreated();

            // Migração defensiva: adiciona colunas se o banco já existia previamente
            try { db.Database.ExecuteSqlRaw("ALTER TABLE Participantes ADD COLUMN SetsVencidos INTEGER NOT NULL DEFAULT 0;"); } catch { }
            try { db.Database.ExecuteSqlRaw("ALTER TABLE Participantes ADD COLUMN SetsPerdidos INTEGER NOT NULL DEFAULT 0;"); } catch { }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao inicializar SQLite: {ex.Message}");
        }
    }

    /// <summary>
    /// Salva ou atualiza um torneio no banco SQLite local de forma segura.
    /// </summary>
    public int SalvarTorneio(int torneioIdAtual, string nome, TipoTorneio tipo, IEnumerable<Jogador> participantes, IEnumerable<Rodada> rodadas)
    {
        try
        {
            using var db = new PadelDbContext();

            TorneioDb? torneio = null;
            if (torneioIdAtual > 0)
            {
                torneio = db.Torneios
                    .Include(t => t.Participantes)
                    .Include(t => t.Partidas)
                    .FirstOrDefault(t => t.Id == torneioIdAtual);
            }

            if (torneio == null)
            {
                torneio = new TorneioDb
                {
                    Nome = nome,
                    Tipo = tipo.ToString(),
                    DataCriacao = DateTime.UtcNow,
                    Status = "Em Andamento"
                };
                db.Torneios.Add(torneio);
                db.SaveChanges(); // Gera o Id
            }
            else
            {
                torneio.Nome = nome;
                torneio.Tipo = tipo.ToString();

                // Limpa dados filhos para atualizar
                db.Participantes.RemoveRange(torneio.Participantes);
                db.Partidas.RemoveRange(torneio.Partidas);
                db.SaveChanges();
            }

        // Salva participantes
        foreach (var p in participantes)
        {
            db.Participantes.Add(new ParticipanteDb
            {
                TorneioDbId = torneio.Id,
                ParticipanteIndex = p.Id,
                Nome = p.Nome,
                Grupo = p.Grupo,
                Jogos = p.Jogos,
                Vitorias = p.Vitorias,
                Derrotas = p.Derrotas,
                SetsVencidos = p.SetsVencidos,
                SetsPerdidos = p.SetsPerdidos,
                GamesPro = p.GamesPro,
                GamesContra = p.GamesContra,
                Pontos = p.Pontos,
                Posicao = p.Posicao
            });
        }

        // Salva partidas
        foreach (var rodada in rodadas)
        {
            foreach (var partida in rodada.Partidas)
            {
                db.Partidas.Add(new PartidaDb
                {
                    TorneioDbId = torneio.Id,
                    NumeroRodada = partida.NumeroRodada,
                    NumeroQuadra = partida.NumeroQuadra,
                    TituloFase = partida.TituloFase,
                    NomeDupla1 = partida.NomeDupla1,
                    NomeDupla2 = partida.NomeDupla2,
                    Placar1 = partida.Placar1,
                    Placar2 = partida.Placar2,
                    Finalizada = partida.EstaFinalizada
                });
            }
        }

            db.SaveChanges();
            return torneio.Id;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Falha ao salvar torneio no SQLite: {ex.Message}");
            return torneioIdAtual;
        }
    }

    /// <summary>
    /// Retorna o histórico de torneios salvos no banco.
    /// </summary>
    public List<TorneioDb> ObterTorneiosSalvos()
    {
        using var db = new PadelDbContext();
        return db.Torneios
            .Include(t => t.Participantes)
            .Include(t => t.Partidas)
            .OrderByDescending(t => t.DataCriacao)
            .Take(15)
            .ToList();
    }

    /// <summary>
    /// Exclui um torneio e seus dados associados.
    /// </summary>
    public void ExcluirTorneio(int id)
    {
        using var db = new PadelDbContext();
        var t = db.Torneios.Find(id);
        if (t != null)
        {
            db.Torneios.Remove(t);
            db.SaveChanges();
        }
    }
}
