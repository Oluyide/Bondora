using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bondora2.Models
{
    public class CustomerCartModels
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string CustomerName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int RentDays { get; set; }

        public bool IsCheckedOut { get; set; }

        public decimal RentalPrice { get; set; }

        public int BonusPoint { get; set; }

        public InventoryModels InventoryItem { get; set; } = new InventoryModels();
    }
}
