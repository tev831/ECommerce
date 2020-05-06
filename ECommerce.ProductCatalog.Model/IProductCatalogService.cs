using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.ProductCatalog.Model
{
    public interface IProductCatalogService : IService
    {
        Task<Product> GetProduct(Guid productId);

        Task<Product[]> GetAllProductsAssync();

        Task AddProductAsync(Product product);
    }
}
