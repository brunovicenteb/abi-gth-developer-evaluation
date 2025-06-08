using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Middleware;

public class ValidationExceptionMiddlewareTests
{
    private readonly Faker _faker = new("pt_BR");

    private static DefaultHttpContext CreateContext(out MemoryStream bodyStream)
    {
        var context = new DefaultHttpContext();
        bodyStream = new MemoryStream();
        context.Response.Body = bodyStream;
        return context;
    }

    [Fact(DisplayName = "Should handle ValidationException and return 400")]
    public async Task Given_ValidationException_When_Invoked_Then_ReturnsBadRequest()
    {
        // Arrange
        var context = CreateContext(out var stream);
        var logger = Substitute.For<ILogger<ValidationExceptionMiddleware>>();

        var failures = new List<ValidationFailure>
        {
            new("Nome", "Nome requerido."),
            new("Email", "Email mal formatado.")
        };

        var exception = new FluentValidation.ValidationException(failures);

        RequestDelegate next = _ => throw exception;
        var middleware = new ValidationExceptionMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Contains("ValidationError", body);
        Assert.Contains("Nome requerido", body);
        Assert.Contains("Email mal formatado", body);
    }

    [Fact(DisplayName = "Should handle KeyNotFoundException and return 404")]
    public async Task Given_KeyNotFoundException_When_Invoked_Then_ReturnsNotFound()
    {
        // Arrange
        var context = CreateContext(out var stream);
        var logger = Substitute.For<ILogger<ValidationExceptionMiddleware>>();
        var message = _faker.Lorem.Sentence();

        RequestDelegate next = _ => throw new KeyNotFoundException(message);
        var middleware = new ValidationExceptionMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream).ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Contains("NotFound", body);
        Assert.Contains(message, body);
    }

    [Fact(DisplayName = "Should handle UnauthorizedAccessException and return 401")]
    public async Task Given_UnauthorizedAccessException_When_Invoked_Then_ReturnsUnauthorized()
    {
        // Arrange
        var context = CreateContext(out var stream);
        var logger = Substitute.For<ILogger<ValidationExceptionMiddleware>>();
        var message = _faker.Lorem.Sentence();

        RequestDelegate next = _ => throw new UnauthorizedAccessException(message);
        var middleware = new ValidationExceptionMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream).ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        Assert.Contains("Unauthorized", body);
        Assert.Contains(message, body);
    }

    [Fact(DisplayName = "Should handle generic exception and return 500")]
    public async Task Given_GenericException_When_Invoked_Then_ReturnsServerError()
    {
        // Arrange
        var context = CreateContext(out var stream);
        var logger = Substitute.For<ILogger<ValidationExceptionMiddleware>>();

        RequestDelegate next = _ => throw new Exception("Erro interno");
        var middleware = new ValidationExceptionMiddleware(next, logger);

        // Act
        await middleware.Invoke(context);

        // Assert
        stream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(stream).ReadToEndAsync();

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Contains("ServerError", body);
        Assert.Contains("Ocorreu um erro inesperado", body);
    }
}