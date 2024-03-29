﻿
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShoppingListCore.Repository;
using ShoppingListCoreProject.Models;
using ShoppingListProject.Models;
using ShoppingListProject.Validators;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ShoppingListCore.Areas.Member.Controllers
{
    [Authorize(Roles = "Member")]
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class ListController : Controller 
    {
     
        //List, Listdetail,Category ve product tabloları için genericrepository oluşturduk
        GenericRepository<List> listRepository = new GenericRepository<List>();
        GenericRepository<ListDetail> listDetailRepository = new GenericRepository<ListDetail>();
        GenericRepository<Product> productRepository = new GenericRepository<Product>();
        GenericRepository<Category> categoryRepository = new GenericRepository<Category>();
       static int Userid;


    

        public IActionResult Index()
        {
            //Oturumu açık olan kullanıcının idsi ile listelerini getirdik
            Userid = Convert.ToInt32(HttpContext.User.Identity.Name);
            var lists = listRepository.GetListByFilter(x => x.UserId == Userid);
            return View(lists);
        }
        public IActionResult DeleteList(int id)
        {
            //Listdetail idsine göre liste siler

            listRepository.Delete(listRepository.GetByFilter(x=>x.UserId==Userid&x.ListId==id));
            return RedirectToAction("Index", "List");

        }
        public IActionResult DeleteProductonList(int id,int ListId)
        {
            //product id ye göre listdetail içerisindeki seçilen ürünü siler
            var identitycontrol = listRepository.GetByFilter(x => x.UserId == Userid & x.ListId == ListId);
            if (identitycontrol != null)
                listDetailRepository.Delete(listDetailRepository.GetByID(id));
            return RedirectToAction("ListDetail", new { id = ListId, isActive = true });
        }
        public IActionResult DeleteProductonShop(int id, int ListId)
        {
            //product id ye göre listdetail içerisindeki seçilen ürünü siler
            var identitycontrol = listRepository.GetByFilter(x => x.UserId == Userid & x.ListId == ListId);
            if (identitycontrol != null)
                listDetailRepository.Delete(listDetailRepository.GetByID(id));
            return RedirectToAction("ListDetail", new { id = ListId, isActive = false });
        }
        [HttpPost]
        public IActionResult AddList(List list)
        {
          //kullanıcıya liste ekler
            ListValidator validator = new ListValidator();
            ValidationResult result = validator.Validate(list);
            if (result.IsValid == false)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();

            }
            list.UserId = Userid;
            list.isActive= true;
            listRepository.Insert(list);
            var lists = listRepository.GetListByFilter(x => x.UserId == Userid);
            return View("Index",lists);







        }

        [HttpGet]
        public IActionResult AddList()
        {

            return View();


        }

        [HttpGet]
        public IActionResult ListDetail(int id)
        {
          

            //kullanıcının listdetail tablosundaki verilerini product ve category bilgileriyle birleştirerek getirir
            
            Expression<Func<ListDetail, object>>[] expressionList = { x => x.Product, x => x.Product.Category };
            var detail = listDetailRepository.GetAll(x => x.ListId == id&x.List.UserId== Userid, expressionList);
            ViewBag.Listid=id;
            ViewBag.isActive = listRepository.GetByID(id).isActive;
            GetCategories();
            return View(detail);
    
        }

        public IActionResult FilterProductbyCategory(int id,int Listid)
        {

            Expression<Func<ListDetail, object>>[] expressionList = { x => x.Product, x => x.Product.Category };
            var productlist = listDetailRepository.GetAll(x=>x.ListId==Listid&x.List.UserId==Userid, expressionList);
                      
            if ( id != 0)
            {
                productlist = productlist.Where(c => c.Product.CategoryId == id).ToList();
            }

            GetCategories(id);

            ViewBag.isActive = listRepository.GetByID(Listid).isActive;
            ViewBag.Listid = Listid;
            return PartialView("ListDetail", productlist);
        }
        [HttpPost]
        public IActionResult FilterProductbyName(string name, int Listid)
        {
            Expression<Func<ListDetail, object>>[] expressionList = { x => x.Product, x => x.Product.Category };
            if (name == null)
                name = "";

            var productlist = listDetailRepository.GetAll(x => x.ListId == Listid & x.List.UserId == Userid& x.Product.ProductName.Contains(name), expressionList);
            
            GetCategories();

            ViewBag.isActive = listRepository.GetByID(Listid).isActive;
            ViewBag.Listid = Listid;
            if(productlist.Count!=0)
            return PartialView("ListDetail", productlist);
            else {
                var notfound  = listDetailRepository.GetAll(x => x.ListId == Listid & x.List.UserId == Userid , expressionList);
                ViewBag.Notfound = "Ürün Bulunamadı";
                return PartialView("ListDetail",notfound); 
                    
                    }




        }
        private void GetCategories(int id = 0)
        {
            //kategori listesini getirir
            List<Category> categories = new List<Category>();
         
            var categories1 = categoryRepository.GetList();
            if (id != 0)
            {
                //ürün sayfasına gönderirken dropdownda ilk sırada filtrelenen ürünün kategorisi olsun
                var ct = categoryRepository.GetByID(id);
                categories.Add(ct);
                categories.Add(new Category { CategoryId = 0, CategoryName = "Tümü" });
                foreach (var item in categories1)
                {
                    if (item.CategoryId != ct.CategoryId)
                    {
                        categories.Add(item);
                    }

                }



            }
            else
            {
                categories.Add(new Category { CategoryId = 0, CategoryName = "Tümü" });
                foreach (var item in categories1)
                {
                    
                        categories.Add(item);
                    

                }
            }
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }

        public IActionResult ProductDetail(int id,bool isActive)
        {
            //seçilen ürünün detaylarını getirir
            ListDetail listproduct = new ListDetail();
            ViewBag.isActive = isActive;
            var product = listDetailRepository.GetAll(x=>x.Id==id&x.List.UserId==Userid,y=> y.Product);
            foreach (var item in product)
            {
                listproduct = item;

            }
            return View(listproduct);
        }

        [HttpPost]
        public IActionResult UpdateProduct(ListDetail l)
        {
            //ürünü günceller/ kullanıcı sadece girdiği detayı güncelleyebilir
            var identitycontrol = listRepository.GetByFilter(x => x.UserId == Userid & x.ListId == l.ListId);
            if (identitycontrol != null) { 
                var listdetail=listDetailRepository.GetByID(l.Id);
            listdetail.Detail=l.Detail;
            listDetailRepository.Update(listdetail);
}
            return RedirectToAction("ListDetail", new{id=l.ListId});

        }
        public IActionResult ChangeActiveStatus(int id) 
        {
            //alışverişe çık ve alışverişi bitir butonları tıklandığında listenin aktiflik durumunu değiştirir
            var a=listRepository.GetByID(id);
            if (a.isActive)
            { a.isActive = false; }
            else
            { a.isActive = true; }
            listRepository.Update(a);
            
            return RedirectToAction("ListDetail", new { id = id, isActive = a.isActive });
        
        }

        [HttpGet]
        public IActionResult AddProduct(int listid)
        {
            //ürünlerin listeli olduğu sayfaya yönlendirirken zaten kullanıcının listesinde bulunan ürünleri filtreler
            GetCategories();
            var product = productRepository.GetAll(null, y => y.Category);
            var listproducts=listDetailRepository.GetListByFilter(x=>x.ListId==listid&x.List.UserId==Userid);
            for (int i = 0; i < product.Count; i++)
            {
                for (int j = 0; j < listproducts.Count; j++)
                {

                    if (listproducts[j].ProductId == product[i].ProductId)
                    {
                        product.RemoveAt(i);
                      i--; break;
                    }

                }

            }
           

            ViewBag.listid = listid;
            return View(product);
        }
        public IActionResult FilterAddProductList(int id, int Listid)
        {
            //Kategori filtreleme

            var product = productRepository.GetAll(null, y => y.Category);
            var listproducts = listDetailRepository.GetListByFilter(x => x.ListId == Listid );
            for (int i = 0; i < product.Count; i++)
            {
                for (int j = 0; j < listproducts.Count; j++)
                {

                    if (listproducts[j].ProductId == product[i].ProductId)
                    {
                        product.RemoveAt(i);
                        i--; break;
                    }

                }

            }


            if (id != 0)
            {
                product = product.Where(c => c.CategoryId == id).ToList();
            }

            GetCategories(id);

         
            ViewBag.Listid = Listid;
            return View("AddProduct", product);
        }

        [HttpPost]
        public IActionResult FilterAddProductListbyName(string name, int Listid)
        {
            //ürün adı filtreleme
            if (name == null)
                name = "";
            var product = productRepository.GetAll(x=>x.ProductName.Contains(name), y => y.Category);
            var notfound = product;
            var listproducts = listDetailRepository.GetListByFilter(x => x.ListId == Listid);
            for (int i = 0; i < product.Count; i++)
            {
                for (int j = 0; j < listproducts.Count; j++)
                {

                    if (listproducts[j].ProductId == product[i].ProductId)
                    {
                        product.RemoveAt(i);
                        notfound.RemoveAt(i);
                        i--; break;
                    }

                }

            }
            GetCategories(); ;
            ViewBag.Listid = Listid;
            if (product.Count != 0)
            {
                return View("AddProduct", product);
            }
            else
            {

               
                ViewBag.Notfound = "Ürün Bulunamadı"; 
                return View("AddProduct", notfound);
            }
          

 
          





        }
        [HttpGet]
        public IActionResult AddProducttoList(int id,int listid)
        {
            //seçilen ürünü ilgili listeye ekler

            ListDetail listDetail = new ListDetail();
            listDetail.ListId= listid;
            listDetail.ProductId = id;
            var identitycontrol = listRepository.GetByFilter(x => x.UserId == Userid&x.ListId==listid);
            if(identitycontrol != null) 
          listDetailRepository.Insert(listDetail);
            return RedirectToAction("AddProduct", new { listid=listid });
        }

       




    }
}
