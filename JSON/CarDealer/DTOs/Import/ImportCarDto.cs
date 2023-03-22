using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    
    public class ImportCarDto
    {
        [JsonProperty("make")]
        public string Make { get; set; } = null!;
        [JsonProperty("model")]
        public string Model { get; set; } = null!;
        [JsonProperty("traveledDistance")]
        public long TraveledDistance { get; set; }
        [JsonProperty("partsId")]
        public ICollection<int> PartsId { get; set; } = new List<int>(); 
    }
}
