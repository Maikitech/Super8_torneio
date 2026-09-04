using Microsoft.AspNetCore.Mvc;
using MeuProjetoApi.Models;
using MeuProjetoApi.Services;

namespace MeuProjetoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Retorna a lista de todos os produtos cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Produto>>> ObterTodos()
    {
        _logger.LogInformation("Buscando todos os produtos...");
        var produtos = await _produtoService.ObterTodosAsync();
        return Ok(produtos);
    }

    /// <summary>
    /// Retorna os detalhes de um produto específico pelo seu identificador.
    /// </summary>
    /// <param name="id">ID numérico do produto</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Produto>> ObterPorId(int id)
    {
        _logger.LogInformation("Buscando produto com ID {Id}...", id);
        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto == null)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });
        }

        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um novo produto no sistema.
    /// </summary>
    /// <param name="produto">Dados do novo produto</param>
    [HttpPost]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Produto>> Criar([FromBody] Produto produto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Cadastrando novo produto: {Nome}", produto.Nome);
        var novoProduto = await _produtoService.CriarAsync(produto);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoProduto.Id }, novoProduto);
    }

    /// <summary>
    /// Atualiza as informações de um produto existente.
    /// </summary>
    /// <param name="id">ID do produto a ser atualizado</param>
    /// <param name="produto">Novos dados do produto</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Produto produto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Atualizando produto ID {Id}...", id);
        var sucesso = await _produtoService.AtualizarAsync(id, produto);

        if (!sucesso)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado para atualização." });
        }

        return NoContent();
    }

    /// <summary>
    /// Remove um produto pelo seu identificador.
    /// </summary>
    /// <param name="id">ID do produto a ser removido</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        _logger.LogInformation("Removendo produto ID {Id}...", id);
        var removido = await _produtoService.RemoverAsync(id);

        if (!removido)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado para exclusão." });
        }

        return NoContent();
    }
}
