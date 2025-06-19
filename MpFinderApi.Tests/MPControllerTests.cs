using Moq;
using Xunit;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using MpFinderApi.Controllers;
using Moq.Protected;
using System.Threading;
using System.Threading.Tasks;

public class MPControllerTests
{
    [Fact]
    public async Task GetMPByPostcode_ReturnsMPInfo()
    {
        // Mock HTTP handler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"mp\": \"Test MP\", \"postcode\": \"N1 1AA\"}")
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Mock config
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TheyWorkForYou:ApiKey", "dummy-key" }
            })
            .Build();

        // Act
        var controller = new MPController(factoryMock.Object, config);
        var result = await controller.GetMPByPostcode("N1 1AA");

        Assert.NotNull(result);
    }
}