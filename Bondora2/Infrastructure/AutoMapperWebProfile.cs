using Bondora2.Models;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bondora2.Infrastructure
{
    public class AutoMapperWebProfile: AutoMapper.Profile
    {
        public AutoMapperWebProfile()
        {
            CreateMap<InventoryModels, Inventory>();
            CreateMap<Inventory, InventoryModels>();

            CreateMap<CustomerCartModels, CustomerCart>();
            CreateMap<CustomerCart, CustomerCartModels>();

            CreateMap<EquipmentsTypeModels, EquipmentsType>();
            CreateMap<EquipmentsType, EquipmentsTypeModels>();

            CreateMap<PaymentReadingTypeModels, PaymentReadingType>();
            CreateMap<PaymentReadingType, PaymentReadingTypeModels>();

        }
        public static void Run()
        {
            AutoMapper.Mapper.Initialize(a =>
            {
                a.AddProfile<AutoMapperWebProfile>();
            });
        }
        

    }
}