using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bondora2.Models
{
     public class PaymentReadingTypeModels
     {

        public int Id { get; set; }

        public string TypeofRent { get; set; }

        public ICollection<FeeSetupModels> Inventory { get; set; }
    }
}
