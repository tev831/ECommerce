using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.ProductCatalog
{
    class ServiceFabricProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManager;

        public ServiceFabricProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task AddProduct(Product product)
        {
            // Returns a dictionary. Gets it in the back or creates one if it doesn't exist.
            IReliableDictionary<Guid, Product> products = await _stateManager
                .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            // Everything is done via transactions
            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);

                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            IReliableDictionary<Guid, Product> products = await _stateManager
                .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var results = new List<Product>();

            // Everything is done via transactions
            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                // Get all products from Service Fabric async. (Incurs a call cost)
                // Returns enumerable instead of list because there can be millions of products. 
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts =
                    await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                // Get product enumerator from Service Fabric async. (Incurs a call cost)
                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator =
                    allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        results.Add(current.Value);
                    }
                }
            }

            return results;
        }
    }
}
