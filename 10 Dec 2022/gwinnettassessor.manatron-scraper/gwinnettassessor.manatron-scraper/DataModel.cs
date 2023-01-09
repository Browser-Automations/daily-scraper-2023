using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gwinnettassessor.manatron_scraper
{
    public class DataModel
    {
        public string ParcelId { get; set; }
        public string OwnerName { get; set; }
        public string PropertyStreet { get; set; }
        public string PropertyCity { get; set; }
        public string PropertyState { get; set; }
        public string PropertyZip { get; set; }
        public string MailingAddress { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingZip { get; set; }
        public string YearBuilt { get; set; }
        public string PropertyClass { get; set; }
    }
}
