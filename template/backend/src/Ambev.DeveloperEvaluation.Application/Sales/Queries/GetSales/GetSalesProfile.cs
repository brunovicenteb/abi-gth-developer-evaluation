using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales
{
    public class GetSalesProfile : Profile
    {
        public GetSalesProfile()
        {
            CreateMap<Sale, SaleListItemDto>();
        }
    }
}