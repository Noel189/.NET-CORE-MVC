using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
using Bulky.DataAccess.Repository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll(includeProperties:"Category").ToList();

            return View(objCategoryList);
        }

        public IActionResult Upsert(int? id)
        { 

            CategoryVM CategoryVm = new ()
            { 
                  CategoryList =  _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Category = new Category()
             };
            if(id==null||id==0)
            {
                return View(CategoryVm);
            }
            else
            {
                //update
                CategoryVm.Category=_unitOfWork.Category.Get(u=>u.Id==id);
                return View(CategoryVm);
            }
            
    }
     
        [HttpPost]
        public IActionResult Upsert(CategoryVM CategoryVM,IFormFile?file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string CategoryPath = Path.Combine(wwwRootPath, @"images\Category");


                    if(!string.IsNullOrEmpty(CategoryVM.Category.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath=Path.Combine(wwwRootPath,CategoryVM.Category.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(CategoryPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    CategoryVM.Category.ImageUrl =@"\images\Category\"+fileName;
                }

                if(CategoryVM.Category.Id ==0)
                {
                    _unitOfWork.Category.Add(CategoryVM.Category);
                }
                else
                {
                    _unitOfWork.Category.Update(CategoryVM.Category);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                CategoryVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(CategoryVM);
            }
            
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Category? CategoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
        //    //Category? CategoryFromDb1 = _db.Categories.FirstOrDefault(c => c.Id == id); 
        //    //Category? CategoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
        //    if (CategoryFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(CategoryFromDb);
        //}
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll(includeProperties: "Category").ToList();
            return Json(new {data=objCategoryList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CategoryToBeDeleted = _unitOfWork.Category.Get(u=>u.Id == id);
            if (CategoryToBeDeleted == null)
            {
                return Json(new {succes=false,message="Error while deleting"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, 
                CategoryToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Category.Remove(CategoryToBeDeleted);
            _unitOfWork.Save();
            List<Category> objCategoryList = _unitOfWork.Category.GetAll(includeProperties: "Category").ToList();
            return Json(new { success=true, message="Delete Successful" });
        }
        #endregion
    }
}
