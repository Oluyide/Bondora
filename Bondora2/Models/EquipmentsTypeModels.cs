using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bondora2.Models
{
    public class EquipmentsTypeModels
    {
        public int Id { get; set; }

        public string TypeName{ get; set; }

        public string RentTerms { get; set; }

        public int LoyaltyPoint { get; set; }


        public ICollection<InventoryModels> Inventory { get; set; }
    }
}
