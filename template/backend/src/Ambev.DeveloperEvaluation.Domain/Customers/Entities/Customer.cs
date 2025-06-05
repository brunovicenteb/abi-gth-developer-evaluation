using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.customers.Entities;

public class Customer : BaseEntity
{
    public const string INVALID_NAME = "O nome do cliente é obrigatório.";
    public const string INVALID_DOCUMENT = "O documento do cliente é obrigatório.";
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
}