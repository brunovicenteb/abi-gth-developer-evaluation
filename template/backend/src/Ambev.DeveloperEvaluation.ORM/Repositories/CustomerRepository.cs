using Ambev.DeveloperEvaluation.Domain.customers.Entities;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CustomerRepository : BaseRepository<DefaultContext, Customer>, ICustomerRepository
{
    private const string DEFAULT_ORDER_BY = "Name";

    public CustomerRepository(DefaultContext context) : base(context)
    {
    }

    protected override DbSet<Customer> Collection => Context.Customers;

    protected override string GetDefaultOrderBySearch() => DEFAULT_ORDER_BY;
}