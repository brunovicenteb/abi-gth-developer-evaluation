namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerResponse
{
    /// <summary>
    /// The unique identifier of the created customer
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The customer's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;
}