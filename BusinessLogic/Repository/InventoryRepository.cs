using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BusinessLogic.Repository
{
    public class InventoryRepository : IInventory
    {
        BondoraContext context;
        public InventoryRepository(BondoraContext context)
        {
            this.context = context;
        }

        public async Task<Inventory> GetInventoryById(int id)
        {
            var item = await context.Inventory.Include(x => x.EquipmentsType).FirstOrDefaultAsync(x => x.Id == id);
            return item;
        }

        public async Task<List<Inventory>> GetAllInventory()
        {
            var inventorylist = await context.Inventory.Include(x => x.EquipmentsType).ToListAsync();
            return inventorylist;
        }

        public async Task<List<CustomerCart>> GetCustomerRent(string userId)
        {
            var customerRent = await context.CustomerCart.Include(x => x.InventoryItem).Where(x => x.UserId == userId && x.IsCheckedOut == false && x.EndDate > DateTime.Today).ToListAsync();
            return customerRent;
        }

        public async Task<List<CustomerCart>> CheckoutToGenerateInvoice(string userId)
        {
            var customerRent = await context.CustomerCart.Include(x => x.InventoryItem).Include(y => y.InventoryItem.EquipmentsType).Where(x => x.UserId == userId && x.IsCheckedOut == false && x.StartDate == DateTime.Today).ToListAsync();
            return customerRent;
        }

        public async Task<List<FeeSetup>> GetAllFees()
        {
            var fees = await context.FeeSetup.ToListAsync();
            return fees;
        }

        public async Task SaveCustomerCart(CustomerCart cart)
        {
            context.CustomerCart.Add(cart);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCustomerCart(int id)
        {
            CustomerCart cart = await context.CustomerCart.SingleAsync(x => x.Id == id);
            cart.IsCheckedOut = true;
            context.Entry(cart).State = System.Data.Entity.EntityState.Modified;
            await context.SaveChangesAsync();
        }

       public async Task<CustomerCart> CheckItemAlreadyinCart(int id,string userId)
        {
            var IsalreadyinCart = await context.CustomerCart.Include(x => x.InventoryItem).Include(y => y.InventoryItem.EquipmentsType).Where(x =>x.InventoryItem.Id == id && x.UserId == userId && x.IsCheckedOut == false && x.StartDate == DateTime.Today).SingleOrDefaultAsync();
            return IsalreadyinCart;
        }

        public async Task DeleteCartItem(CustomerCart cart)
        {
            context.Entry(cart).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
