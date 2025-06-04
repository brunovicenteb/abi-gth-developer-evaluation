using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.customers.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
}