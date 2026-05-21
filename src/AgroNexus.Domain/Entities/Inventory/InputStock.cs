using AgroNexus.Domain.Exceptions;

namespace AgroNexus.Domain.Entities.Inventory;

/// <summary>
/// Representa o estoque atual de um insumo em uma fazenda.
/// </summary>
public sealed class InputStock : BaseEntity
{
    /// <summary>
    /// FK para a fazenda.
    /// </summary>
    public Guid FarmId { get; private set; }

    /// <summary>
    /// FK para o insumo.
    /// </summary>
    public Guid InputId { get; private set; }

    /// <summary>
    /// Quantidade atual em estoque.
    /// </summary>
    public decimal Quantidade { get; private set; }

    /// <summary>
    /// Data de validade do insumo (se aplicável).
    /// </summary>
    public DateTime? Validade { get; private set; }

    private InputStock() { }

    public static InputStock Create(Guid farmId, Guid inputId, decimal quantidade, DateTime? validade = null)
    {
        if (farmId == Guid.Empty)
            throw new DomainException("Fazenda é obrigatória.", "INPUT_STOCK_FARM_REQUIRED");

        if (inputId == Guid.Empty)
            throw new DomainException("Insumo é obrigatório.", "INPUT_STOCK_INPUT_REQUIRED");

        if (quantidade < 0)
            throw new DomainException("Quantidade não pode ser negativa.", "INPUT_STOCK_QTY_INVALID");

        var stock = new InputStock
        {
            FarmId = farmId,
            InputId = inputId,
            Quantidade = quantidade,
            Validade = validade
        };

        return stock;
    }

    /// <summary>
    /// Adiciona quantidade ao estoque (entrada).
    /// </summary>
    public void AddStock(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade a adicionar deve ser maior que zero.", "INPUT_STOCK_ADD_INVALID");

        Quantidade += quantidade;
        MarkAsUpdated();
    }

    /// <summary>
    /// Remove quantidade do estoque (saída/uso).
    /// </summary>
    public void RemoveStock(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("Quantidade a remover deve ser maior que zero.", "INPUT_STOCK_REMOVE_INVALID");

        if (quantidade > Quantidade)
            throw new DomainException(
                $"Estoque insuficiente. Disponível: {Quantidade}, solicitado: {quantidade}.",
                "INPUT_STOCK_INSUFFICIENT");

        Quantidade -= quantidade;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se o insumo está vencido.
    /// </summary>
    public bool IsVencido()
    {
        return Validade.HasValue && Validade.Value < DateTime.UtcNow;
    }
}