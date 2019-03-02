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

        Task<List<CustomerCart>> ClearRentNotCheckedOut(string userId);

        Task<List<CustomerCart>> CheckoutToGenerateInvoice(string userId);

        Task<List<FeeSetup>> GetAllFees();

        Task SaveCustomerCart(CustomerCart cart);

        Task DeleteCartItem(CustomerCart cart);

        Task UpdateCustomerCart(int id);

        Task<CustomerCart> CheckItemAlreadyinCart(int id, string userId);
    }



       
}
