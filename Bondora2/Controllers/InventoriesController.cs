﻿using System;
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
using AutoMapper;
using Bondora2.Infrastructure;
using Bondora2.Models;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace Bondora2.Controllers
{
    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly IInventory _inventory;

        public InventoriesController(IInventory inventory)
        {
            _inventory = inventory;
        }
        // GET: Inventories/Index
        public async Task<ActionResult> Index()
        {
            var allInventories = await _inventory.GetAllInventory();
            var myCarts = await _inventory.GetCustomerRent(User.Identity.GetUserId());

            List<InventoryModels> inventoryList = new List<InventoryModels>();
            List<CustomerCartModels> mycartlist = new List<CustomerCartModels>();

            Mapper.Map(allInventories, inventoryList);
            Mapper.Map(myCarts, mycartlist);

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

            var item = await _inventory.GetInventoryById(id);
            var fees = await _inventory.GetAllFees();

            CustomerCart carts = new CustomerCart();

            carts.InventoryItem = await _inventory.GetInventoryById(id);

            carts.UserId = User.Identity.GetUserId();

            carts.CustomerName = User.Identity.Name;

            carts.StartDate = DateTime.Today;

            carts.EndDate = DateTime.Now.AddDays(sday);

            carts.IsCheckedOut = false;

            carts.RentDays = sday;

            _inventory.SaveCustomerCart(carts);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Remove(int id)
        {
            CustomerCart cartitem = new CustomerCart { Id = id };
            _inventory.DeleteCartItem(cartitem);
            return RedirectToAction("Index");
        }

        private async Task<List<CustomerCartModels>> CalculateUserInvoice(string id)
        {

            var myCarts = await _inventory.CheckoutToGenerateInvoice(User.Identity.GetUserId());

            List<CustomerCartModels> mycartlist = new List<CustomerCartModels>();

            var fees = await _inventory.GetAllFees();

            foreach (var item in myCarts)
            {
                CustomerCartModels model = new CustomerCartModels();
              
                model.InventoryItem.EquipmentName = item.InventoryItem.EquipmentName;
                model.IsCheckedOut = true;
             
                if (item.InventoryItem.EquipmentsType.TypeName == Values.Heavy.ToString())
                {
                    model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;
                    //rental price is one - time rental fee plus premium fee for each day rented.
                    model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() + (fees.Where(x => x.FeeTypeName == "Premium daily").Select(y => y.Fee).FirstOrDefault() * item.RentDays);

                }
                else if (item.InventoryItem.EquipmentsType.TypeName == Values.Regular.ToString())
                {
                    model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;
                    // Regular – rental price is one - time rental fee plus premium fee for the first
                    //2 days plus regular fee for the number of days over 2.
                    if (item.RentDays >= 2)
                    {
                       
                        model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() +
                            (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * 2) +
                             (fees.Where(x => x.FeeTypeName == FeeType.RegularDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (item.RentDays - 2));
                    }
                    else
                    {
                       
                        model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.OneTime.ToString()).Select(y => y.Fee).FirstOrDefault() +
                           (fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault());
                    }
                }
                else if (item.InventoryItem.EquipmentsType.TypeName == Values.Specialized.ToString())
                {
                    model.BonusPoint = item.InventoryItem.EquipmentsType.LoyaltyPoint;
                    if (item.RentDays >= 3)
                    {
                       
                        model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * 3 +
                             (fees.Where(x => x.FeeTypeName == FeeType.RegularDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * (item.RentDays - 3));
                    }
                    else
                    {
                        model.RentalPrice = fees.Where(x => x.FeeTypeName == FeeType.PremiumDaily.ToString()).Select(y => y.Fee).FirstOrDefault() * item.RentDays;
                    }
                }
                mycartlist.Add(model);

               _inventory.UpdateCustomerCart(item.Id);

            }

            return mycartlist;

            
        }

        public async Task<ActionResult> DownloadInvoice(string id)
        {
            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream);
            tw.WriteLine("                    Bondora                        ");
            tw.WriteLine("                  Rent Invoice                     ");
            tw.WriteLine("---------------------------------------------------");
            tw.WriteLine("Equipments                             Rental Price(€)");
            tw.WriteLine("---------------------------------------------------");
            var invoiceList = await CalculateUserInvoice(id);
            foreach (var item in invoiceList)
            {
                string equipment = item.InventoryItem.EquipmentName.ToString();

                if (equipment.Length < 17)
                {
                    tw.WriteLine(equipment.PadRight(equipment.Length + (17 - equipment.Length), ' ') + "                             " + item.RentalPrice.ToString());
                }
                else
                {
                    tw.WriteLine(item.InventoryItem.EquipmentName.ToString() + "                             " + item.RentalPrice.ToString());
                }
            }
            tw.WriteLine("---------------------------------------------------");
            tw.WriteLine("---------------------Summary-----------------------");
            tw.WriteLine("Total Price(€):                             " + invoiceList.Sum(x=>x.RentalPrice).ToString());
            tw.WriteLine("Total Loyalty Point:                             " + invoiceList.Sum(x => x.BonusPoint).ToString());
            tw.WriteLine("---------------------------------------------------");
            tw.WriteLine("---------------------Tere Tulemast------------------");

            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", "RentInvoice.txt");
        }

      

    }
}

