using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhamTranXuanTan_2122110248.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Admin/Categories
        ECommerceDBEntities1 objECommerceDBEntities1 = new ECommerceDBEntities1();
    
        public ActionResult Index()
        {
            var categories = objECommerceDBEntities1.categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public ActionResult Create()
        {
           
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(category objCategory)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    // Đường dẫn thư mục lưu ảnh
                    string folderPath = Server.MapPath("~/Content/images/categories/");

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
                    if (objCategory.ImageUpload != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);
                        string extension = Path.GetExtension(objCategory.ImageUpload.FileName);
                        filename = filename + extension;

                        // Đường dẫn đầy đủ để lưu file
                        string fullPath = Path.Combine(folderPath, filename);
                        objCategory.image = filename;
                        objCategory.ImageUpload.SaveAs(fullPath);

                        System.Diagnostics.Debug.WriteLine("Ảnh đã được lưu tại: " + fullPath);
                    }

                    // Lưu thông tin sản phẩm
                    objCategory.created_at = DateTime.Now;
                    objECommerceDBEntities1.categories.Add(objCategory);
                    objECommerceDBEntities1.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi: " + ex.Message);
                    return View();
                }
            }
            return View(objCategory);
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            var details = objECommerceDBEntities1.categories.Where(x => x.id == id).FirstOrDefault();
            return View(details);

        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var del = objECommerceDBEntities1.categories.Where(x => x.id == id).FirstOrDefault();
            return View(del);

        }
        [HttpPost]
        public ActionResult Delete(category objCategory)
        {
            try
            {
                var del = objECommerceDBEntities1.categories.Where(x => x.id == objCategory.id).FirstOrDefault();
                if (del != null)
                {
                    // Lấy đường dẫn của file ảnh
                    string folderPath = Server.MapPath("~/Content/images/categories/");
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
                    objECommerceDBEntities1.categories.Remove(del);
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
           
            var edit = objECommerceDBEntities1.categories.Where(x => x.id == id).FirstOrDefault();
            return View(edit);

        }
        [HttpPost]
        public ActionResult Edit(int id, category objCategory)
        {
           
            try
            {
                // Lấy sản phẩm cũ từ database
                var existingProduct = objECommerceDBEntities1.categories.FirstOrDefault(p => p.id == id);
                if (existingProduct == null)
                {
                    return HttpNotFound("Sản phẩm không tồn tại!");
                }

                // Đường dẫn thư mục lưu ảnh
                string folderPath = Server.MapPath("~/Content/images/categories/");

                // Xử lý ảnh mới (nếu có upload ảnh mới)
                if (objCategory.ImageUpload != null)
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
                    string filename = Path.GetFileNameWithoutExtension(objCategory.ImageUpload.FileName);
                    string extension = Path.GetExtension(objCategory.ImageUpload.FileName);
                    filename = filename + extension;

                    string fullPath = Path.Combine(folderPath, filename);
                    objCategory.image = filename; // Gán tên ảnh mới vào product
                    objCategory.ImageUpload.SaveAs(fullPath);

                    System.Diagnostics.Debug.WriteLine("Ảnh mới đã được lưu tại: " + fullPath);
                }
                else
                {
                    // Nếu không upload ảnh mới, giữ nguyên ảnh cũ
                    objCategory.image = existingProduct.image;
                }

                // Cập nhật các thông tin khác của sản phẩm
                objCategory.created_at = DateTime.Now;
                objECommerceDBEntities1.Entry(existingProduct).CurrentValues.SetValues(objCategory);
                objECommerceDBEntities1.SaveChanges();
                // Lưu thông tin sản phẩm

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi chỉnh sửa sản phẩm: " + ex.Message);
                return View(objCategory);
            }
        }
    }
}