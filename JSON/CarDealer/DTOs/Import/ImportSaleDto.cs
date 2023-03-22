using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class ImportSaleDto
    {
        //    "carId": 105,
        //"customerId": 30,
        //"discount": 30
        [JsonProperty("carId")]
        public string CarId { get; set;}
        [JsonProperty("customerId")]
        public string CustomerId { get; set;}
        [JsonProperty("discount")]
        public decimal Discount { get; set; }
    }
}
