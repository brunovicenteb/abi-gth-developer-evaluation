using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales
{
    public class GetSalesHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<GetSalesResult> Handle(GetSalesQuery query, CancellationToken cancellationToken)
        {
            var validator = new GetSalesQueryValidator();
            var validationResult = await validator.ValidateAsync(query, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var (items, total) = await _saleRepository.GetPagedAsync(query.Page, query.Size,
                query.OrderBy, query.Filters, cancellationToken);

            var data = _mapper.Map<List<SaleListItemDto>>(items);
            return new GetSalesResult
            {
                Page = query.Page,
                Size = query.Size,
                Total = total,
                Data = data
            };
        }
    }
}