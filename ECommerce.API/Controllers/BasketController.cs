using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("{userId}")] 
        public async Task<ApiBasket> GetAsync(string userId)
        {
            IUserActor actor = GetActor(userId);

            // We should not use any complicated types over the wire (interfaces, generics, etc)
            //Dictionary<Guid, int> products = await actor.GetBasket();
            BasketItem[] products = await actor.GetBasket();

            return new ApiBasket() 
            {
                UserId = userId,
                Items = products.Select( // Convert from Product to ApiBasketItem
                    p => new ApiBasketItem
                    {
                        ProductId = p.ProductId.ToString(),//Key.ToString(),
                        Quantity = p.Quantity//Value
                    }).ToArray()
            };
        }

        [HttpPost("{userId}")]
        public async Task AddAsync(string userId, [FromBody] ApiBasketAddRequest request)
        {
            IUserActor actor = GetActor(userId);

            await actor.AddToBasket(request.ProductId, request.Quantity);
        }

        [HttpDelete("{userId}")]
        public async Task DeleteAsync(string userId)
        {
            IUserActor actor = GetActor(userId);

            await actor.ClearBasket();
        }

        private IUserActor GetActor(string userId)
        {
            // Actor Proxy is a helper function.
            return ActorProxy.Create<IUserActor>(
                new ActorId(userId),
                new Uri("fabric:/ECommerce/UserActorService"));
        }
    }
}