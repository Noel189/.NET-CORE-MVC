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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
    
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        { 

            if(id==null||id==0)
            {
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj =_unitOfWork.Company.Get(u=>u.Id==id);
                return View(companyObj);
            }
            
    }
     
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
               
                
                if(companyObj.Id ==0)
                {
                    _unitOfWork.Company.Add(companyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
               
                return View(companyObj);
            }
            
        }

        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    //Company? companyFromDb1 = _db.Categories.FirstOrDefault(c => c.Id == id); 
        //    //Company? companyFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
        //    if (companyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(companyFromDb);
        //}
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index");

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data=objCompanyList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id == id);
            if (companyToBeDeleted == null)
            {
                return Json(new {succes=false,message="Error while deleting"});
            }

         
            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { success=true, message="Delete Successful" });
        }
        #endregion
    }
}
