using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository db)
        {
           categoryRepository = db;
        }
        public IActionResult Index()
        {
            List<Category>  objCategoryList = categoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name==obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                categoryRepository.Add(obj);
               categoryRepository.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
            
        }

        public IActionResult Edit(int? id)
        {
            if(id == null||id==0)
            {
                return NotFound();
            }
            Category? categroyFromDb = categoryRepository.Get(u=>u.Id==id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(c => c.Id == id); 
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categroyFromDb==null)
            {
                return NotFound(); 
            }
            return View(categroyFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {    
            if (ModelState.IsValid)
            {
                categoryRepository.Update(obj);
                categoryRepository.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categroyFromDb = categoryRepository.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(c => c.Id == id); 
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categroyFromDb == null)
            {
                return NotFound();
            }
            return View(categroyFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = categoryRepository.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            categoryRepository.Remove(obj);
            categoryRepository.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
