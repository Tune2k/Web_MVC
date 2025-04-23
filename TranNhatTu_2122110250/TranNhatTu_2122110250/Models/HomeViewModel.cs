using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public class HomeViewModel
    {
        public List<Context.category> Categories 
        { 
            get; set; 
        }
        public List<Context.brand> Brands
        {
            get; set;
        }
        public List<Context.product> Product
        {
            get; set;
        }
        public List<Context.product> ProductRecommended
        {
            get; set;
        }

    }
}