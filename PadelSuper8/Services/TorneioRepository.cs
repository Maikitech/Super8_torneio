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
            LogService.Info("Banco de dados SQLite inicializado e verificado com sucesso.", "BancoDados");

            // Migração defensiva: adiciona colunas se o banco já existia previamente
            try { db.Database.ExecuteSqlRaw("ALTER TABLE Participantes ADD COLUMN SetsVencidos INTEGER NOT NULL DEFAULT 0;"); } catch { }
            try { db.Database.ExecuteSqlRaw("ALTER TABLE Participantes ADD COLUMN SetsPerdidos INTEGER NOT NULL DEFAULT 0;"); } catch { }
        }
        catch (Exception ex)
        {
            LogService.Error($"Erro crítico ao inicializar o banco de dados SQLite: {ex.Message}", ex, "BancoDados");
        }
    }

    /// <summary>
    /// Salva ou atualiza um torneio no banco SQLite local de forma segura.
    /// </summary>
    public int SalvarTorneio(int torneioIdAtual, string nome, TipoTorneio tipo, IEnumerable<Jogador> participantes, IEnumerable<Rodada> rodadas)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            LogService.Info($"Iniciando salvamento do torneio '{nome}' (Tipo: {tipo}, IdAtual: {torneioIdAtual})...", "BancoDados");

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
                LogService.Info($"Novo registro de torneio criado no banco com Id #{torneio.Id}.", "BancoDados");
            }
            else
            {
                torneio.Nome = nome;
                torneio.Tipo = tipo.ToString();

                // Limpa dados filhos para atualizar
                db.Participantes.RemoveRange(torneio.Participantes);
                db.Partidas.RemoveRange(torneio.Partidas);
                db.SaveChanges();
                LogService.Info($"Atualizando torneio existente Id #{torneio.Id}.", "BancoDados");
            }

            int countPart = 0;
            // Salva participantes
            foreach (var p in participantes)
            {
                countPart++;
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

            int countPartidas = 0;
            // Salva partidas
            foreach (var rodada in rodadas)
            {
                foreach (var partida in rodada.Partidas)
                {
                    countPartidas++;
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
            stopwatch.Stop();

            LogService.Info($"Torneio #{torneio.Id} salvo com êxito em {stopwatch.ElapsedMilliseconds}ms ({countPart} participantes, {countPartidas} partidas).", "BancoDados");
            return torneio.Id;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogService.Error($"Falha ao salvar torneio '{nome}' no SQLite: {ex.Message}", ex, "BancoDados");
            return torneioIdAtual;
        }
    }

    /// <summary>
    /// Retorna o histórico de torneios salvos no banco.
    /// </summary>
    public List<TorneioDb> ObterTorneiosSalvos()
    {
        try
        {
            using var db = new PadelDbContext();
            var lista = db.Torneios
                .Include(t => t.Participantes)
                .Include(t => t.Partidas)
                .OrderByDescending(t => t.DataCriacao)
                .Take(15)
                .ToList();

            LogService.Info($"Histórico de torneios consultado: {lista.Count} registro(s) encontrado(s).", "BancoDados");
            return lista;
        }
        catch (Exception ex)
        {
            LogService.Error($"Erro ao consultar histórico de torneios salvos: {ex.Message}", ex, "BancoDados");
            return new List<TorneioDb>();
        }
    }

    /// <summary>
    /// Exclui um torneio e seus dados associados.
    /// </summary>
    public void ExcluirTorneio(int id)
    {
        try
        {
            using var db = new PadelDbContext();
            var t = db.Torneios.Find(id);
            if (t != null)
            {
                db.Torneios.Remove(t);
                db.SaveChanges();
                LogService.Info($"Torneio Id #{id} ('{t.Nome}') foi excluído com sucesso do banco de dados.", "BancoDados");
            }
            else
            {
                LogService.Warn($"Tentativa de exclusão do torneio Id #{id}, mas o registro não foi localizado.", null, "BancoDados");
            }
        }
        catch (Exception ex)
        {
            LogService.Error($"Erro ao tentar excluir torneio Id #{id}: {ex.Message}", ex, "BancoDados");
        }
    }
}
