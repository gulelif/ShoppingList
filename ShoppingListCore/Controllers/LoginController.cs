using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

using ShoppingListCore.Models;
using ShoppingListCore.Repository;
using ShoppingListCoreProject.Models;
using ShoppingListProject.Models;
using ShoppingListProject.Validators;
using System.Security.Claims;
using System.Security.Principal;

namespace ShoppingListCoreProject.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        //User tablosu için repository oluşturuyoruz
        GenericRepository<User> userRepository = new GenericRepository<User>();


        [HttpGet]
        public IActionResult SignUp()
        {
            HttpContext.SignOutAsync();                   
            return View();
        }
        [HttpPost]
        public  IActionResult SignUp(UserRegisterViewModel u) 
        {
            //UserRegisterViewModel içerisinde User bilgileri ve confirmpassword var 
            //bilgilerin uservalidatordaki şartları sağlayıp sağlamadığına bakıyoruz
            UserValidator validator = new UserValidator();
            ValidationResult result = validator.Validate(u);
            if (result.IsValid == false)
            {
                foreach (var error in result.Errors)
                {
                    //şart sağlanmamışsa ekrana hata mesajını gönderiyoruz
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();

            }
          
            User user = new User()
            {
                UserPassword = u.UserPassword,
                UserSurname = u.UserSurname,
                UserMail = u.UserMail,
                UserName = u.UserName
            };

           
           
                //veritabanında 
                var mail = userRepository.GetByFilter(x => x.UserMail == user.UserMail);
            if (mail == null)//veritabanında böyle bir mail yoksa kaydet
            {
                userRepository.Insert(user);
                return RedirectToAction("SignIn");
            }

           
            else
            {
                ModelState.AddModelError("emailused", "Mail adresi daha önce alınmış");
                return View();
            }


        }

        [HttpGet]
        public  IActionResult SignIn()
        {
            //Daha önce giriş yapılmışsa SignIn sayfasından rolüne göre yönlendiriyoruz.
         if( User.IsInRole("Administrator"))
            { return RedirectToAction("Index", "Category", new { area = "Admin" });}  

         if(User.IsInRole("Member"))
            {  return RedirectToAction("Index", "List", new { area = "Member" }); }
            //Giriş yapılmamışsa SignIn sayfasına yönlendiriyoruz
            var Userid = String.IsNullOrEmpty(HttpContext.User.Identity.Name);

            if (Userid)
                ViewBag.isauthenticated = false;
            else
                ViewBag.isauthenticated = true;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInViewModel p)
        {
            //Sayfadan gelen kullanıcı adı ve parolanın boş olup olmadığını kontrol ediyoruz
            LoginValidator validator = new LoginValidator();
            ValidationResult result = validator.Validate(p);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();

            }
            //boş değilse..
            if (result.IsValid)
            {
                string role = "";
                User user=ControlLogin(p);

             
                if(user!=null)
                {
                    //Kullanıcı rolünü belirledik
                    role=user.UserAdminStatus ?  "Administrator" : "Member";
                 
                    //Authentication işlemlerini cookie ile yapıyoruz
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, role),
                };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();
                   
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);//önceden açık oturum varsa kapat
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity),
                    authProperties);//oturumu aç

                    //kullanıcı rolüne göre yönlendirme işlemi
                    return user.UserAdminStatus 
                        ? RedirectToAction("Index", "Category", new { area = "Admin" }) 
                        : RedirectToAction("Index", "List", new { area = "Member" });
                  

                }

                else
            {
                ViewBag.message = "Hatalı Kullanıcı adı veya Parola";
            }
                }
          

            return View();
           
        }
        private User ControlLogin(UserSignInViewModel p)
        {
           //mail adresi ve parolanın veritabanından kontrolü
            var user = userRepository.GetByFilter(x => x.UserMail == p.email && x.UserPassword == p.password);
            return user;

        }

        public async Task<IActionResult> LogOut()
        {
            //oturumu kapat
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
