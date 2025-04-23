using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public class OrderDetailViewModel
    {
        public order Order { get; set; }
        public List<OrderDetailInfo> OrderDetails { get; set; }
    }

    

}