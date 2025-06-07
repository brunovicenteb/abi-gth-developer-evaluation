using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Sales.Events;

public record SaleCancelledEvent(Sale Sale) : INotification;
