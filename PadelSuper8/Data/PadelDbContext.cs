using System.IO;
using Microsoft.EntityFrameworkCore;

namespace PadelSuper8.Data;

public class TorneioDb
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Em Andamento";

    public List<ParticipanteDb> Participantes { get; set; } = new();
    public List<PartidaDb> Partidas { get; set; } = new();
}

public class ParticipanteDb
{
    public int Id { get; set; }
    public int TorneioDbId { get; set; }
    public TorneioDb? Torneio { get; set; }

    public int ParticipanteIndex { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Grupo { get; set; }
    public int Jogos { get; set; }
    public int Vitorias { get; set; }
    public int Derrotas { get; set; }
    public int SetsVencidos { get; set; }
    public int SetsPerdidos { get; set; }
    public int GamesPro { get; set; }
    public int GamesContra { get; set; }
    public int Pontos { get; set; }
    public int Posicao { get; set; }
}

public class PartidaDb
{
    public int Id { get; set; }
    public int TorneioDbId { get; set; }
    public TorneioDb? Torneio { get; set; }

    public int NumeroRodada { get; set; }
    public int NumeroQuadra { get; set; }
    public string TituloFase { get; set; } = string.Empty;
    public string NomeDupla1 { get; set; } = string.Empty;
    public string NomeDupla2 { get; set; } = string.Empty;
    public int Placar1 { get; set; }
    public int Placar2 { get; set; }
    public bool Finalizada { get; set; }
}

public class PadelDbContext : DbContext
{
    public DbSet<TorneioDb> Torneios => Set<TorneioDb>();
    public DbSet<ParticipanteDb> Participantes => Set<ParticipanteDb>();
    public DbSet<PartidaDb> Partidas => Set<PartidaDb>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Salva o arquivo padel.db na pasta de execução do executável/projeto
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "padel.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TorneioDb>()
            .HasMany(t => t.Participantes)
            .WithOne(p => p.Torneio)
            .HasForeignKey(p => p.TorneioDbId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TorneioDb>()
            .HasMany(t => t.Partidas)
            .WithOne(p => p.Torneio)
            .HasForeignKey(p => p.TorneioDbId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
