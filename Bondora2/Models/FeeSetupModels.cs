using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bondora2.Models
{
    public class FeeSetupModels
    {
       
        public int Id { get; set; }

        public string  FeeTypeName  { get; set; }

        public decimal Fee { get; set; }
        
        public PaymentReadingTypeModels TypeofRent {get; set;}

    }
}
