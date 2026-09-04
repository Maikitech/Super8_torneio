using System.Collections.Concurrent;
using MeuProjetoApi.Models;

namespace MeuProjetoApi.Services;

/// <summary>
/// Implementação em memória do serviço de produtos (thread-safe).
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly ConcurrentDictionary<int, Produto> _produtos = new();
    private int _proximoId = 1;

    public ProdutoService()
    {
        // Dados de exemplo iniciais
        AdicionarExemplo("Notebook Gamer", "Notebook com 16GB RAM, SSD 512GB e placa dedicada", 4599.90m);
        AdicionarExemplo("Mouse Sem Fio", "Mouse ergonômico óptico com conexão 2.4GHz", 129.90m);
        AdicionarExemplo("Teclado Mecânico", "Teclado mecânico RGB com switches azuis", 349.00m);
    }

    private void AdicionarExemplo(string nome, string descricao, decimal preco)
    {
        var id = _proximoId++;
        _produtos[id] = new Produto
        {
            Id = id,
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            DataCriacao = DateTime.UtcNow
        };
    }

    public Task<IEnumerable<Produto>> ObterTodosAsync()
    {
        return Task.FromResult<IEnumerable<Produto>>(_produtos.Values.OrderBy(p => p.Id));
    }

    public Task<Produto?> ObterPorIdAsync(int id)
    {
        _produtos.TryGetValue(id, out var produto);
        return Task.FromResult(produto);
    }

    public Task<Produto> CriarAsync(Produto produto)
    {
        var novoId = Interlocked.Increment(ref _proximoId);
        produto.Id = novoId;
        produto.DataCriacao = DateTime.UtcNow;
        _produtos[novoId] = produto;

        return Task.FromResult(produto);
    }

    public Task<bool> AtualizarAsync(int id, Produto produtoAtualizado)
    {
        if (!_produtos.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        produtoAtualizado.Id = id;
        _produtos[id] = produtoAtualizado;
        return Task.FromResult(true);
    }

    public Task<bool> RemoverAsync(int id)
    {
        return Task.FromResult(_produtos.TryRemove(id, out _));
    }
}
