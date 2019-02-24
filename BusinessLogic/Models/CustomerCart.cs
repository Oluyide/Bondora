using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class CustomerCart
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string CustomerName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int RentDays { get; set; }

        [NotMapped]
        public decimal RentalPrice { get; set; }

        [NotMapped]
        public int BonusPoint { get; set; }

        public bool IsCheckedOut { get; set;}

        public Inventory InventoryItem { get; set; }
    }
}
