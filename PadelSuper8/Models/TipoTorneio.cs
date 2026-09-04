namespace PadelSuper8.Models;

public enum TipoTorneio
{
    RotativoIndividual, // Americano: 8 jogadores trocam de duplas em 7 rodadas
    DuplasFixas         // 8 duplas em 2 grupos (A e B) + Semifinais e Finais
}

public enum StatusPartida
{
    Pendente,
    EmAndamento,
    Finalizada
}

public enum FasePartida
{
    RodadaRegular,
    GrupoA,
    GrupoB,
    Semifinal,
    DisputaTerceiro,
    Final
}
