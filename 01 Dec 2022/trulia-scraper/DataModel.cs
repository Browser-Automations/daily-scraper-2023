using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trulia_scraper
{
    public class DataModel
    {
        public string PropertyUrl { get; set; }=string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Images { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Beds { get; set; } = string.Empty;
        public string Baths { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string ListedBy { get; set; } = string.Empty;
    }
}
