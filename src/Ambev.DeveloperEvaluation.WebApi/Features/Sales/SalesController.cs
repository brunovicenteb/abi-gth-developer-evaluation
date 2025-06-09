using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller responsible for managing operations related to sales,
/// including creation, updates, cancellations, and retrieval.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalesController"/>.
    /// </summary>
    /// <param name="mediator">The MediatR handler for command/query dispatching.</param>
    /// <param name="mapper">The AutoMapper instance for DTO conversion.</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">Request payload containing sale data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Details of the created sale.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateSaleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        var reloadedData = _mapper.Map<CreateSaleResponse>(result);

        return Created(nameof(GetSale), reloadedData);
    }

    /// <summary>
    /// Cancels a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The sale's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Confirmation of cancellation.</returns>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new CancelSaleRequest(id);
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = new CancelSaleCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing sale.
    /// </summary>
    /// <param name="id">The sale's unique identifier.</param>
    /// <param name="request">The updated sale data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Details of the updated sale.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateSaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "ID do parâmetro da rota difere do corpo da requisição"
            });

        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        var data = _mapper.Map<UpdateSaleResponse>(result);

        return Ok(data);
    }

    /// <summary>
    /// Retrieves a specific sale by its unique identifier.
    /// </summary>
    /// <param name="id">The sale's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full sale information.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetSaleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale(Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var query = new GetSaleQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        var reloadedSale = _mapper.Map<GetSaleResponse>(result);

        return Ok(reloadedSale);
    }

    /// <summary>
    /// Cancels a specific item from a sale.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale.</param>
    /// <param name="productId">The unique identifier of the item (product) to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Confirmation of item cancellation.</returns>
    [HttpPut("{saleId}/items/{productId}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSaleItem([FromRoute] Guid saleId, [FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        var request = new CancelSaleItemRequest(saleId, productId);
        var validator = new CancelSaleItemRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = new CancelSaleItemCommand(saleId, productId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Retrieves a paginated list of sales, supporting filtering and ordering through query parameters.
    /// </summary>
    /// <param name="request">
    /// Query parameters:
    /// <list type="bullet">
    /// <item><description><c>field=value</c> - Filters by exact match</description></item>
    /// <item><description><c>field=*value</c> - Applies a "contains" filter</description></item>
    /// <item><description><c>_order=field</c> or <c>_order=-field</c> - Sorts ascending or descending</description></item>
    /// <item><description><c>_page=N</c>, <c>_size=M</c> - Enables pagination</description></item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated sales data.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<GetSalesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSales([FromQuery] GetSalesRequest request, CancellationToken cancellationToken)
    {
        var validator = new GetSalesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        request.PrepareFilters(HttpContext);

        var query = _mapper.Map<GetSalesQuery>(request);
        var result = await _mediator.Send(query, cancellationToken);

        var data = _mapper.Map<List<GetSalesResponse>>(result.Data);
        var response = new PaginatedResponse<GetSalesResponse>()
        {
            CurrentPage = result.Page,
            TotalCount = result.Total,
            TotalPages = (int)Math.Ceiling(result.Total / (double)result.Size),
            Data = data
        };

        return Ok(response);
    }
}