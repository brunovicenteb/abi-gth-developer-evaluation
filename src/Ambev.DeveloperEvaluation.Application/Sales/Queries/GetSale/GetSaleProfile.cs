using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;

public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();
    }
}