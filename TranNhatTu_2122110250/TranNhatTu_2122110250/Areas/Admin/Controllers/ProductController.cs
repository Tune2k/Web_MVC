using PagedList;
using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using static PhamTranXuanTan_2122110248.Common;

namespace PhamTranXuanTan_2122110248.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities1 = new ECommerceDBEntities1();
        // GET: Admin/Product
        public ActionResult Index(string currentFilter, string SearchString, int? page) 
        { 
            var lstProduct = new List<product>();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                    SearchString = currentFilter;
            }
        
            if (!string.IsNullOrEmpty(SearchString))
                {
                    lstProduct = objECommerceDBEntities1.products.Where(n => n.name.Contains(SearchString)).ToList();
                }
                //lây ds sản phẩm theo từ khóa tìm kiếm

                else
                {
                    lstProduct = objECommerceDBEntities1.products.ToList();
                }
                //lay all sản phẩm trong bảng product
                ViewBag.CurrentFilter = SearchString;
            //số lượng item của 1 trang = 4
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            // sắp xếp theo id sản phẩm, sp mới đưa lên đầu
            lstProduct=lstProduct.OrderByDescending(n => n.id).ToList();
            return View(lstProduct.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Create()
        {
            this.loadData();
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(product objProduct)
        {
            this.loadData();
            if (ModelState.IsValid)
            {
                try
                {
                    // Đường dẫn thư mục lưu ảnh
                    string folderPath = Server.MapPath("~/Content/images/products/");

                    // Log danh sách các file trong thư mục trước khi lưu
                    if (Directory.Exists(folderPath))
                    {
                        string[] files = Directory.GetFiles(folderPath);
                        System.Diagnostics.Debug.WriteLine("Các file hiện có trong thư mục:");
                        foreach (var file in files)
                        {
                            System.Diagnostics.Debug.WriteLine(file);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Thư mục không tồn tại: " + folderPath);
                    }

                    // Lưu ảnh nếu có
                    if (objProduct.ImageUpload != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);
                        string extension = Path.GetExtension(objProduct.ImageUpload.FileName);
                        filename = filename + extension;

                        // Đường dẫn đầy đủ để lưu file
                        string fullPath = Path.Combine(folderPath, filename);
                        objProduct.image = filename;
                        objProduct.ImageUpload.SaveAs(fullPath);

                        System.Diagnostics.Debug.WriteLine("Ảnh đã được lưu tại: " + fullPath);
                    }

                    // Lưu thông tin sản phẩm
                    objProduct.created_at = DateTime.Now;
                    objECommerceDBEntities1.products.Add(objProduct);
                    objECommerceDBEntities1.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi: " + ex.Message);
                    return View();
                }
            }
            return View(objProduct);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var details = objECommerceDBEntities1.products.Where(x => x.id == id).FirstOrDefault();
            return View(details);

        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var del = objECommerceDBEntities1.products.Where(x => x.id == id).FirstOrDefault();
            return View(del);

        }
        [HttpPost]
        public ActionResult Delete(product objPro)
        {
            try
            {
                var del = objECommerceDBEntities1.products.Where(x => x.id == objPro.id).FirstOrDefault();
                if (del != null)
                {
                    // Lấy đường dẫn của file ảnh
                    string folderPath = Server.MapPath("~/Content/images/products/");
                    string filePath = Path.Combine(folderPath, del.image);

                    // Kiểm tra và xóa file ảnh nếu tồn tại
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        TempData["LogMessage"] = "File ảnh đã được xóa: " + filePath;
                    }
                    else
                    {
                        TempData["LogMessage"] = "File ảnh không tồn tại: " + filePath;
                    }

                    // Xóa sản phẩm khỏi database
                    objECommerceDBEntities1.products.Remove(del);
                    objECommerceDBEntities1.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["LogMessage"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            this.loadData();
            var edit = objECommerceDBEntities1.products.Where(x => x.id == id).FirstOrDefault();
            return View(edit);

        }
        [HttpPost]
        public ActionResult Edit(int id, product objProduct)
        {
            this.loadData();
            try
            {
                // Lấy sản phẩm cũ từ database
                var existingProduct = objECommerceDBEntities1.products.FirstOrDefault(p => p.id == id);
                if (existingProduct == null)
                {
                    return HttpNotFound("Sản phẩm không tồn tại!");
                }

                // Đường dẫn thư mục lưu ảnh
                string folderPath = Server.MapPath("~/Content/images/products/");

                // Xử lý ảnh mới (nếu có upload ảnh mới)
                if (objProduct.ImageUpload != null)
                {
                    // Xóa ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(existingProduct.image))
                    {
                        string oldImagePath = Path.Combine(folderPath, existingProduct.image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                            System.Diagnostics.Debug.WriteLine("Đã xóa ảnh cũ: " + oldImagePath);
                        }
                    }

                    // Lưu ảnh mới
                    string filename = Path.GetFileNameWithoutExtension(objProduct.ImageUpload.FileName);
                    string extension = Path.GetExtension(objProduct.ImageUpload.FileName);
                    filename = filename + extension;

                    string fullPath = Path.Combine(folderPath, filename);
                    objProduct.image = filename; // Gán tên ảnh mới vào product
                    objProduct.ImageUpload.SaveAs(fullPath);

                    System.Diagnostics.Debug.WriteLine("Ảnh mới đã được lưu tại: " + fullPath);
                }
                else
                {
                    // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                    objProduct.image = existingProduct.image;
                }

                // Cập nhật các thông tin khác của sản phẩm
                objProduct.created_at = DateTime.Now;
                objECommerceDBEntities1.Entry(existingProduct).CurrentValues.SetValues(objProduct);
                objECommerceDBEntities1.SaveChanges();
                // Lưu thông tin sản phẩm

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi chỉnh sửa sản phẩm: " + ex.Message);
                return View(objProduct);
            }
        }

        void loadData()
        {
            Common common = new Common();
            //lấy dữ liệu danh mục dưới DB
            var lstCat = objECommerceDBEntities1.categories.ToList();
            //Convert sang select list dạng value, text
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            DataTable dtCategory = converter.ToDataTable(lstCat);
            ViewBag.ListCategory = common.ToSelectList(dtCategory, "Id", "Name");
            // lấy dữ liệu thương hiệu dưới DB
            var lstBrand = objECommerceDBEntities1.brands.ToList();
            DataTable dtBrand = converter.ToDataTable(lstBrand);
            //Convert sang select list dạng value, text
            ViewBag.ListBrand = common.ToSelectList(dtBrand, "Id", "Name");

        }
        
    }
}