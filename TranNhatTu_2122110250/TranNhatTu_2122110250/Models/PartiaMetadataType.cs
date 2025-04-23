using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PhamTranXuanTan_2122110248.Context;
using System.Linq;
using System.Web;
using PhamTranXuanTan_2122110248.Models;
namespace PhamTranXuanTan_2122110248.Context
{
    [MetadataType(typeof (UserMasterData))]
    public partial class user
    {
       
    }
    [MetadataType(typeof(ProductMasterData))]
    public partial class product
    {
        [NotMapped]
        public System.Web.HttpPostedFileBase ImageUpload { get; set; }
    }
    [MetadataType(typeof(CategoriesMasterData))]
    public partial class category
    {
        [NotMapped]
        public System.Web.HttpPostedFileBase ImageUpload { get; set; }
    }
    [MetadataType(typeof(BrandsMasterData))]
    public partial class brand
    {
        [NotMapped]
        public System.Web.HttpPostedFileBase ImageUpload { get; set; }
    }

}