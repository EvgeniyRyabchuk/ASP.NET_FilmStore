using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using FilmsStorage.Models;
using FilmsStorage.DAL;
using FilmsStorage.Models.Entities;
using FilmsStorage.Mappers;
using FilmsStorage.SL;

namespace FilmsStorage.Controllers
{
    public class AccountController : Controller
    {
        // Это личный профиль пользователя. Разрешение на вход по логину
        [Authorize]
        //Фильтр Autorize требует чтобы пользователь был залогинен
        //TODO: Заменить штатный фильтр на собственный
        public ViewResult Index()
        {
            return View();
        }

        #region Login
        //Окно логина
        public ViewResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                //Проверим есть ли данный пользователь в базе
                User registeredUser = _DAL.Users.ByLogin(loginModel.LoginName);

                //If such user registered
                if (registeredUser != null)
                {
                    //Password check
                    bool isPasswordValid = _SL.Hasher.checkPassword(registeredUser.Password, loginModel.Password);

                    if (isPasswordValid)
                    {
                        _SL.Cookies.SetLoginCookie(registeredUser);

                        return RedirectToAction("Index", "Account");
                        //TODO: create login cookie
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Wrong password";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "No such user";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMsg = "Invalid login form";
                return View();
            }
        }
        #endregion

        public RedirectToRouteResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

        #region Register
        //Окно регистрации
        public ViewResult Register()
        {
            return View(new RegisterModel());
        }

        //Метод-обработчик формы регистрации
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                //Проверим есть ли данный пользователь в базе
                User registerUser = _DAL.Users.ByLogin(registerModel.LoginName);

                //Такого пользователя ещё нет - можно регистрировать
                if (registerUser == null)
                {
                    User userRegisterEntity = UserMapper.FromRegisterModel(registerModel);
                    userRegisterEntity.RegisteredAt = DateTime.Now;

                    User registeredUser = _DAL.Users.Register(userRegisterEntity);

                    //TODO //create login cookie
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ViewBag.ErrorMsg = $"User {registerModel.LoginName} is already registered";

                    return View(registerModel);
                }
            }
            else
            {
                //Ошибки в форме регистрации
                ViewBag.ErrorMsg = "Invalid register form";
                return View(registerModel);
            }
        }
        #endregion
    }
}