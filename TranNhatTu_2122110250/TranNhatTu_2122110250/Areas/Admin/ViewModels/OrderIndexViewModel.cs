using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Areas.Admin.ViewModels
{
    public class OrderIndexViewModel
    {
        public List<Order> Orders { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
    }

}
