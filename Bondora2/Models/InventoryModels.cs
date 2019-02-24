using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bondora2.Models
{
    public class InventoryModels
    {

        public int Id { get; set; }

        public string EquipmentName { get; set; }

        public EquipmentsTypeModels EquipmentsType { get; set; } = new EquipmentsTypeModels();

    }
}
