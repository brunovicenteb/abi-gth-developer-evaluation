using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Sales.Events;

public record SaleItemCancelledEvent(Sale Sale, SaleItem SaleItem) : INotification;
