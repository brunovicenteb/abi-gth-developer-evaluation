using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<GetSalesRequest, GetSalesQuery>();
        CreateMap<SaleListItemDto, GetSalesResponse>();
    }
}