using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Company.Function
{
    public class RecordEntity : TableEntity
    {
        public RecordEntity(string skey, string srow)
        {
            this.PartitionKey = skey;
            this.RowKey = srow;
        }

        public RecordEntity() { }

        public string indoorTemperature { get; set; }
        public string outdoorTemperature { get; set; }
        public string mode { get; set; }
        public string fanRequest { get; set; }
        public string circulationFanRequest { get; set; }

    }
}
