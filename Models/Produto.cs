using System.ComponentModel.DataAnnotations;

namespace MeuProjetoApi.Models;

/// <summary>
/// Representa um produto no sistema.
/// </summary>
public class Produto
{
    /// <summary>
    /// Identificador único do produto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do produto.
    /// </summary>
    [Required(ErrorMessage = "O nome do produto é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do produto.
    /// </summary>
    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Preço unitário do produto.
    /// </summary>
    [Range(0.01, 100000.00, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    /// <summary>
    /// Data de cadastro do produto.
    /// </summary>
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
