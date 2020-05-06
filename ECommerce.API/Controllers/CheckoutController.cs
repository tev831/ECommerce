using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.API.Model;
using ECommerce.CheckoutService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        Random rnd = new Random();

        [HttpGet("{userId}")]
        public async Task<ApiCheckoutSummary> CheckoutAsync (string userId)
        {
            CheckoutSummary summary = await GetCheckoutService().CheckoutAsync(userId);
            return ToApiCheckoutSummary(summary);
        }

        [HttpGet("history/{userId}")]

        public async Task<IEnumerable<ApiCheckoutSummary>> GetHistoryAsync(string userId)
        {
            IEnumerable<CheckoutSummary> history =
                await GetCheckoutService().GetOrderHistoryAsync(userId);

            return history.Select(ToApiCheckoutSummary);
        }

        private ApiCheckoutSummary ToApiCheckoutSummary(CheckoutSummary summary)
        {
            return new ApiCheckoutSummary()
            {
                Product = summary.Products.Select(p => new ApiCheckoutProduct
                {
                    ProductId = p.Product.Id,
                    ProductName = p.Product.Name,
                    Price = p.Price,
                    Quantity = p.Quantity
                }).ToList(),

                Date = summary.Date,

                TotalPrice = summary.TotalPrice
            };
        }

        private ICheckoutService GetCheckoutService()
        {
            var proxyFactory = new ServiceProxyFactory(
                c => new FabricTransportServiceRemotingClientFactory());

            return proxyFactory.CreateServiceProxy<ICheckoutService>(
                new Uri("fabric:/ECommerce/ECommerce.CheckoutService"),
                new ServicePartitionKey(LongRandom()));
        }

        private long LongRandom()
        {
            byte[] buf = new byte[8];

            rnd.NextBytes(buf);

            return BitConverter.ToInt64(buf, 0);
        }
    }
}