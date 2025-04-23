using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhamTranXuanTan_2122110248.Context;
namespace PhamTranXuanTan_2122110248.Models
{
    public class CartModel
    {
        public product product { get; set; }
        public int Quantity { get; set; }
    }
}