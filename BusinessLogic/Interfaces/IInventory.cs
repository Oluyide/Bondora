using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IInventory : IDisposable
    {
        Task<Inventory> GetInventoryById(int id);

        Task<List<Inventory>> GetAllInventory();

        Task<List<CustomerCart>> GetCustomerRent(string userId);

        Task<List<CustomerCart>> CheckoutToGenerateInvoice(string userId);

        Task<List<FeeSetup>> GetAllFees();

        void SaveCustomerCart(CustomerCart cart);

        void DeleteCartItem(CustomerCart cart);

        void UpdateCustomerCart(int id);
    }



       
}
