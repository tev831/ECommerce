using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Model
{
    public class ApiCheckoutSummary
    {
        [JsonProperty("productId")]
        public List<ApiCheckoutProduct> Product { get; set; }

        [JsonProperty("productId")]
        public double TotalPrice { get; set; }

        [JsonProperty("productId")]
        public DateTime Date { get; set; }
    }
}
