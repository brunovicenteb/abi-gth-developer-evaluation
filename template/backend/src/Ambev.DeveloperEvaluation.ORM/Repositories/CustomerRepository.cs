using Ambev.DeveloperEvaluation.Domain.customers.Entities;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CustomerRepository : BaseRepository<DefaultContext, Customer>, ICustomerRepository
{
    public CustomerRepository(DefaultContext context) : base(context)
    {
    }

    protected override DbSet<Customer> Collection => Context.Customers;
}