namespace Agregator.API;

public record Order(string productId, decimal quantity);

public static class AgregatorApi
{
    public static RouteGroupBuilder MapAgregatorApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders")/*.HasApiVersion(1.0)*/;

        api.MapGet("/", GetOrdersAsync);
        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Results<Ok<List<Order>>, NotFound>> GetOrdersAsync([FromServices] IAgregatorService services)
    {
        try
        {
            var orders = (await services.GetAllOrdersAsync()).Select(o => new Order(o.ProductId, o.Quantity)).ToList();
            return TypedResults.Ok(orders);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Results<Ok, ProblemHttpResult>> CreateOrderAsync(List<Order> orders, [FromServices] IAgregatorService services)
    {
        try
        {
            var items = orders.Select(o => new OrderItem(o.productId, o.quantity)).ToList();
            await services.AddOrdersAsync(items);
            return TypedResults.Ok();
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
