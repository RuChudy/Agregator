namespace Ordering.UnitTests;

using Microsoft.AspNetCore.Http.HttpResults;
using Agregator.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Agregator.API;

[TestClass]
public class OrdersWebApiTest
{
    private readonly ILogger<AgregatorService> _loggerMock;

    public OrdersWebApiTest()
    {
        _loggerMock = Substitute.For<ILogger<AgregatorService>>();
    }

    [TestMethod]
    public async Task Insert_orders_ok_request()
    {
        var data = new Order[]
        {
            new Order("a", 1.0M),
            new Order("b", 2.0M)
        };

        // Act
        var agregatorServices = new AgregatorService(_loggerMock);
        var result = await AgregatorApi.CreateOrderAsync(data.ToList(), agregatorServices);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(Ok));
    }
}