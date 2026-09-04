using MeuProjetoApi.Models;

namespace MeuProjetoApi.Services;

/// <summary>
/// Contrato para operações de negócio relacionadas a produtos.
/// </summary>
public interface IProdutoService
{
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<Produto?> ObterPorIdAsync(int id);
    Task<Produto> CriarAsync(Produto produto);
    Task<bool> AtualizarAsync(int id, Produto produto);
    Task<bool> RemoverAsync(int id);
}
