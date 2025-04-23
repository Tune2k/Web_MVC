using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public class Order
    {
        public string Name { get; internal set; }
        public int UserID { get; internal set; }
        public DateTime CreatedDate { get; internal set; }
        public int Status { get; internal set; }
    }
}