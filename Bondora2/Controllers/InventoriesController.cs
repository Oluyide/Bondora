using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using Bondora2.Infrastructure;
using Bondora2.Models;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NLog;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace Bondora2.Controllers
{
  
    public class InventoriesController : Controller
    {
        private readonly IInventory _inventory;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public InventoriesController(IInventory inventory)
        {
            _inventory = inventory;
        }
        // GET: Inventories/Index
        [Authorize]
       
        public async Task<ActionResult> Index()
        {
            List<InventoryModels> inventoryList = new List<InventoryModels>();
            List<CustomerCartModels> mycartlist = new List<CustomerCartModels>();

            try
            {
                var allInventories = await _inventory.GetAllInventory();
                var myCarts = await _inventory.GetCustomerRent(User.Identity.GetUserId());

                Mapper.Map(allInventories, inventoryList);
                Mapper.Map(myCarts, mycartlist);
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }

            ViewData["Inventories"] = inventoryList;
            ViewData["MyCarts"] = mycartlist;
            
            return View();

        }

       
        public async Task<ActionResult> RentItem(int id)
        {
                var item = await _inventory.GetInventoryById(id);
                var equipmentdetails = JsonConvert.SerializeObject(item, Formatting.None,
                           new JsonSerializerSettings()
                           {
                               ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                           });
            return Json(new { data = equipmentdetails }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> AddtoCart(FormCollection fc)
        {
            
            var id = int.Parse(fc[0].ToString());
            var sday = int.Parse(fc[1].ToString());

            try
            {
                var checkingifExit = await _inventory.CheckItemAlreadyinCart(id, User.Identity.GetUserId());
                if (checkingifExit == null)
                {
                    var item = await _inventory.GetInventoryById(id);
                    CustomerCart carts = new CustomerCart()
                    {
                        InventoryItem = await _inventory.GetInventoryById(id),
                        UserId = User.Identity.GetUserId(),
                        CustomerName = User.Identity.Name,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Now.AddDays(sday),
                        IsCheckedOut = false,
                        RentDays = sday
                    };

                    await _inventory.SaveCustomerCart(carts);
                }
                else
                {
                    TempData["AlreadyAdded"] = string.Format("{0} is already added to your cart", checkingifExit.InventoryItem.EquipmentName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Remove(int id)
        {
            try
            {
                CustomerCart cartitem = new CustomerCart { Id = id };

                await _inventory.DeleteCartItem(cartitem);
            }
            catch(Exception ex) {

                logger.Error(ex.Message);
            }
        
            return RedirectToAction("Index");
        }

        private async Task<List<CustomerCartModels>> CalculateUserInvoice(string id)
        {

            List<CustomerCartModels> mycartlist = new List<CustomerCartModels>();
            try
            {

                var myCarts = await _inventory.CheckoutToGenerateInvoice(User.Identity.GetUserId());

                //I assumed all fees will be input by a Application Admin which will be persisted to the database therefore
                //I have the Fees table which store all the fees available
                var fees = await _inventory.GetAllFees();

                foreach (var item in myCarts)
                {
                    CustomerCartModels model = new CustomerCartModels();

                    model.InventoryItem.EquipmentName = item.InventoryItem.EquipmentName;
                    model.IsCheckedOut = true;

                    if (item.InventoryItem.EquipmentsType.TypeName == EquipType.Heavy.ToString())
                    {
                        model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;

                        //rental price is one - time rental fee plus premium fee for each day rented.
                        model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() +
                                            (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * item.RentDays);
                        //What am doing here is to compare,fecth price from fee table and calculate
                    }
                    else if (item.InventoryItem.EquipmentsType.TypeName == EquipType.Regular.ToString())
                    {
                        model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;
                        // Regular – rental price is one - time rental fee plus premium fee for the first
                        //2 days plus regular fee for the number of days over 2.
                        if (item.RentDays >= (int)DaysEval.TwoDays)
                        {
                            model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() +
                                (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (int)DaysEval.TwoDays) +
                                 (fees.Where(x => x.FeeTypeName == FeeType.RegularDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (item.RentDays - (int)DaysEval.TwoDays));
                        }
                        else
                        {

                            model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() +
                               (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault());
                        }
                    }
                    else if (item.InventoryItem.EquipmentsType.TypeName == EquipType.Specialized.ToString())
                    {
                        model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;
                        //Specialized – rental price is premium fee for the first 3 days plus regular fee times the number 
                        //of days over 3.

                        if (item.RentDays >= (int)DaysEval.ThreeDays)
                        {

                            model.RentalPrice = (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (int)DaysEval.ThreeDays) +
                                 (fees.Where(x => x.FeeTypeName == FeeType.RegularDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (item.RentDays - (int)DaysEval.ThreeDays));
                        }
                        else
                        {
                            model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * item.RentDays;
                        }
                    }
                    mycartlist.Add(model);

                    await _inventory.UpdateCustomerCart(item.Id);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return mycartlist;
        }

       

        public async Task<ActionResult> DownloadInvoice(string id)
        {
            
                MemoryStream memoryStream = new MemoryStream();
                TextWriter tw = new StreamWriter(memoryStream);
            try
            {
                tw.WriteLine("                    Bondora                        ");
                tw.WriteLine("                  Rent Invoice                     ");
                tw.WriteLine("---------------------------------------------------");
                tw.WriteLine("Equipments                             Rental Price(€)");
                tw.WriteLine("---------------------------------------------------");
                var invoiceList = await CalculateUserInvoice(id);
                foreach (var item in invoiceList)
                {
                    string equipment = item.InventoryItem.EquipmentName.ToString();

                    if (equipment.Length < 21)
                    {
                        tw.WriteLine(equipment.PadRight(equipment.Length + (42 - equipment.Length), ' ') +item.RentalPrice.ToString());
                    }
                    else
                    {
                        tw.WriteLine(equipment.PadRight(42,' ') + item.RentalPrice.ToString());
                    }
                }
                tw.WriteLine("---------------------------------------------------");
                tw.WriteLine("---------------------Summary-----------------------");
                tw.WriteLine("Total Price(€):" + invoiceList.Sum(x => x.RentalPrice).ToString().PadLeft(33, ' '));
                tw.WriteLine("Loyalty Point:"  + invoiceList.Sum(x => x.BonusPoint).ToString().PadLeft(29, ' '));
                tw.WriteLine("---------------------------------------------------");
                tw.WriteLine("---------------------Tere Tulemast------------------");

                tw.Flush();
                tw.Close();

            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
            }

            return File(memoryStream.GetBuffer(), "text/plain", "RentInvoice.txt");
        }

    }
}


