using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.DTOs.Account;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;


namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IComplementaryService _complementaryService;
        private readonly IBordroService _bordroService;
        public AccountController(IUserService userService, IComplementaryService complementaryService,IBordroService bordroService)
        {
            _userService = userService;
            _complementaryService = complementaryService;
            _bordroService = bordroService;
        }
        public IActionResult GetCode()
        {
            string cd = _userService.GetNewCode().Result;
            return Content(cd);
        }
        [Route("Login")]
        public IActionResult Login(string retUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            LoginViewModel loginViewModel = new LoginViewModel()
            {
                ReturnUrl = retUrl,
            };
            return View(loginViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            User user = await _userService.GetUserByUserName_and_PasswordAsync(loginViewModel.UserName, loginViewModel.UserPassword);
            if (user != null)
            {
                if (user.CellphoneIsConfirmed == false)
                {
                    ModelState.AddModelError("UserName", "تلفن همراه شما هنوز اعتبارسنجی نشده است !");
                    return View(loginViewModel);
                }
                if (user.IsActive == false)
                {
                    ModelState.AddModelError("UserName", "کاربری شما فعال نشده است !");
                    return View(loginViewModel);
                }
                var claims = new List<Claim>(){
                                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                                        new Claim(ClaimTypes.Name,user.Code),
                                        new Claim("Mobile",user.Cellphone),
                                        new Claim("FullName",user.FullName),
                                        new Claim("NC",user.NC.ToString()),

                                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.Remember
                };
                await HttpContext.SignInAsync(principal, properties);
                return this.RedirectToAction("Index", "Home", new { area = "Admin" });



            }
            else
            {
                ModelState.AddModelError("UserName", "کد کاربری یا رمز عبور اشتباه است !");
                return View(loginViewModel);
            }

        }
        [Route("Register")]
        public async Task<IActionResult> Register()
        {

            RegisterViewModel registerViewModel = new RegisterViewModel()
            {

                States = await _complementaryService.GetStatesAsync()
            };
            return View(registerViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            registerViewModel.States = await _complementaryService.GetStatesAsync();

            if (!ModelState.IsValid)
            {
                registerViewModel.Counties = await _complementaryService.GetCountiesofStateAsync((int)registerViewModel.StateId);
                return View(registerViewModel);
            }
            registerViewModel.Counties = await _complementaryService.GetCountiesofStateAsync((int)registerViewModel.StateId);
            if (Core.Utility.MyUtility.IsValidNC(registerViewModel.UserNC) == false)
            {
                ModelState.AddModelError("UserNC", "کد ملی نامعتبر است !");
                return View(registerViewModel);
            }
            if (await _userService.ExistUserNC(registerViewModel.UserNC))
            {
                ModelState.AddModelError("UserNC", "این کد ملی قبلا ثبت شده است !");
                return View(registerViewModel);
            }
            if (await _userService.ExistUserCellphone(registerViewModel.UserCellphone))
            {
                ModelState.AddModelError("UserCellphone", "این تلفن همراه قبلا ثبت شده است !");
                return View(registerViewModel);
            }
            if (await _userService.ExistUserBankAccount(registerViewModel.BankAccountNumber))
            {
                ModelState.AddModelError("BankAccountNumber", "این شماره حساب قبلا ثبت شده است !");
                return View(registerViewModel);
            }
            if (registerViewModel.BankAccountNumber.Contains(".") == false)
            {
                ModelState.AddModelError("BankAccountNumber", "برای جدا کننده از کاراکتر . استفاده کنید !");
                return View(registerViewModel);
            }

            if (await _userService.ExistUserBankCard(registerViewModel.BankCardNumber.Trim().ToString()))
            {
                ModelState.AddModelError("BankCardNumber", "این شماره کارت قبلا ثبت شده است !");
                return View(registerViewModel);
            }
            User userSP = await _userService.GetUserByCode(registerViewModel.SponserCode);
            if (userSP == null)
            {
                ModelState.AddModelError("SponserCode", "ناظری با این کد سازمانی موجود نیست !");
                return View(registerViewModel);
            }
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(registerViewModel.SponserCode);
            UserRole ParentActiveUserRole = null;
            if (userRoles == null)
            {
                ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                return View(registerViewModel);
            }
            if (userRoles != null)
            {
                ParentActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
                if (ParentActiveUserRole == null)
                {
                    ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                    return View(registerViewModel);
                }
                if (ParentActiveUserRole.RoleId == 10)
                {
                    ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                    return View(registerViewModel);
                }
                int ParentSaleStatics = ParentActiveUserRole.User.InitialStatic;
                double ParentSalePortfo = ParentActiveUserRole.User.InitialPortfo;
                List<LifeBordroBase> lifeBordroBases = await _bordroService.GetDirectBordroBasebyNC(ParentActiveUserRole.User.NC);
                if(ParentActiveUserRole.RoleId == 13)
                {
                    if (lifeBordroBases != null)
                    {
                        if (lifeBordroBases.Count() != 0)
                        {
                            ParentSaleStatics += lifeBordroBases.Count();
                            long DepositSum = lifeBordroBases.SelectMany(s => s.LifeBordroAdditions).Where(w =>w.IsActive).Sum(x => x.Deposit);
                            double PercentOfDeposit = Math.Round(DepositSum * .1);
                            ParentSalePortfo += lifeBordroBases.SelectMany(s => s.LifeBordroAdditions).Where(w => w.IsActive).Sum(x => x.PremiumbyPaymentMethod) + PercentOfDeposit;
                            if (ParentSaleStatics < 5 || ParentSalePortfo < 50000000)
                            {
                                ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                                return View(registerViewModel);
                            }
                        }
                        else
                        {
                            if (ParentSaleStatics < 5 || ParentSalePortfo < 50000000)
                            {
                                ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                                return View(registerViewModel);
                            }
                        }
                    }
                    else
                    {
                        if (ParentSaleStatics < 5 || ParentSalePortfo < 50000000)
                        {
                            ModelState.AddModelError("SponserCode", "این کد مجوز نظارت ندارد !");
                            return View(registerViewModel);
                        }
                    }

                }
                
               
            }
            string pass = Core.Generators.GeneratorClass.GeneratePassword(8, "digit");
           if(!string.IsNullOrEmpty(registerViewModel.BankCardNumber))
            {
                int c = registerViewModel.BankCardNumber.Count(c => c == '-');
                if(c !=3)
                {

                }
            }
            User Myuser = new User()
            {
                FName = registerViewModel.UserFirstName.Replace("ي","ی"),
                LName = registerViewModel.UserFamily.Replace("ي", "ی"),
                NC = registerViewModel.UserNC,
                Cellphone = registerViewModel.UserCellphone,
                BankAccountNumber = registerViewModel.BankAccountNumber.Trim(),
                BankCardNumber = registerViewModel.BankCardNumber.GetCardNumberWithSeprator(),
                CountyId = registerViewModel.CountyId,
                Code = await _userService.GetNewCode(),
                Password = pass,
                BirthDate = Core.Convertors.DateConvertor.ChangeToMiladiWithoutTime(registerViewModel.BDateYear + "/" + registerViewModel.BDateMounth + "/" + registerViewModel.BDateDay),
                RegDate = DateTime.Now,
                SponserCode = registerViewModel.SponserCode,
                Education = registerViewModel.Education,
                Sex = registerViewModel.Sex,
                IsActive = true
            };

            _userService.CreateUser(Myuser);

            await _userService.SaveChangesAsync();

            return RedirectToAction(nameof(ConfirmCellphone), new { Myuser.NC });
        }
        public async Task<IActionResult> ConfirmCellphone(string NC)
        {
            User userNC = await _userService.GetUserByNC(NC);
            string code = Core.Generators.GeneratorClass.GeneratePassword(4, "digit");
            userNC.CellphoneConfirmCode = code;
            _userService.UpdateUser(userNC);
            await _userService.SaveChangesAsync();
            ConfirmCellphoneViewModel confirmCellphoneViewModel = new ConfirmCellphoneViewModel()
            {
                Name = userNC.FName,
                Family = userNC.LName,
                NC = userNC.NC,
                Cellphone = userNC.Cellphone

            };

            _userService.SendVerificationCode(code, userNC.Cellphone);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCellphone(ConfirmCellphoneViewModel confirmCellphoneViewModel)
        {
            User user = await _userService.GetUserByNC(confirmCellphoneViewModel.NC);
            if (confirmCellphoneViewModel.ConfirmCode != user.CellphoneConfirmCode)
            {
                ModelState.AddModelError("ConfirmCode", "کد وارد شده نامعتبر است !");
                return View(confirmCellphoneViewModel);
            }
            
            List<UserRole> userRoles = await _userService.GetUserRolesByUserNC(confirmCellphoneViewModel.NC);
            
            List<UserRole> ParentUserRoles = await _userService.GetUserRolesByUserCode(user.SponserCode);
            UserRole Parent = null;
            if (ParentUserRoles != null)
            {
                if (ParentUserRoles.Count() != 0)
                {
                    Parent = ParentUserRoles.FirstOrDefault(f => f.IsActive == true);
                }
            }
            if(Parent == null)
            {
                ModelState.AddModelError("ConfirmCode", "ناظر شما نامعتبر است !");
                return View(confirmCellphoneViewModel);

            }
            if(user.CellphoneIsConfirmed)
            {
                ModelState.AddModelError("ConfirmCode", "کاربری شما فعال است !");
                return View(confirmCellphoneViewModel);
            }
            //if(userRoles.Any(a => a.User_ID == user.Id && a.RoleId == 13 && a.UserRoleParentId == Parent.URId))
            //{
            //    ModelState.AddModelError("ConfirmCode", "کاربری شما فعال است !");
            //    return View(confirmCellphoneViewModel);
            //}
            //if (userRoles.Any(a => a.IsActive == true))
            //{
            //    ModelState.AddModelError("ConfirmCode", "کاربری شما فعال است !");
            //    return View(confirmCellphoneViewModel);
            //}
            user.IsActive = true;
            user.CellphoneIsConfirmed = true;
            _userService.UpdateUser(user);
            
           
            if (Parent != null)
            {
                UserRole userRole = new UserRole()
                {
                    User_ID = user.Id,
                    RoleId = 13,
                    IsActive = true,
                    UserRoleParentId = Parent.URId,
                    RegisterDate = DateTime.Now,
                    UserRoleParentReceiveDate = DateTime.Now
                };
                _userService.CreateUserRole(userRole);
            }
            await _userService.SaveChangesAsync();

            TempData["message"] = "تلفن همراه شما تایید و ثبت نام نهایی شد" + Environment.NewLine + "کد کاربری و رمز عبور به تلفن همراه شما ارسال گردید";
            _userService.SendUserCode_and_password(user.Code, user.Password, user.Cellphone);

            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> ListCountiesofState(int stateId)
        {
            List<County> counties = await _complementaryService.GetCountiesofStateAsync(stateId);
            return PartialView(counties);
        }
        public IActionResult ConfirmCellphoneSingle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCellphoneSingle(ConfirmCellphoneSingle confirmCellphoneSingle)
        {
            if (!ModelState.IsValid)
            {
                return View(confirmCellphoneSingle);
            }
            User user = await _userService.GetUserByCellphone(confirmCellphoneSingle.UserCellphone);
            
            if (user != null)
            {
                if (user.CellphoneIsConfirmed == true)
                {
                    ModelState.AddModelError("UserCellphone", "تلفن همراه این کاربری قبلا تایید شده است !");
                    return View(confirmCellphoneSingle);
                }
                if(user.NC != confirmCellphoneSingle.UserNC)
                {
                    ModelState.AddModelError("UserCellphone", "تلفن همراه یا کد ملی نامعتبر است !");
                    return View(confirmCellphoneSingle);
                }
                return RedirectToAction(nameof(ConfirmCellphone), new { user.NC });
            }
            else
            {
                ModelState.AddModelError("UserCellphone", "تلفن همراه وارد شده ثبت نشده است !");
                return View(confirmCellphoneSingle);
            }

        }
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("ForgotPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordViewModel);
            }
            User user = await _userService.GetUserByCellphone(forgotPasswordViewModel.UserCellphone);
            if (user == null)
            {
                ModelState.AddModelError("UserCellphone", "تلفن همراه یا کد ملی نامعتبر است !");
                return View(forgotPasswordViewModel);
            }
            
            if (user.NC != forgotPasswordViewModel.UserNC.Trim())
            {
                ModelState.AddModelError("UserCellphone", "تلفن همراه یا کد ملی نامعتبر است !");
                return View(forgotPasswordViewModel);
            }
            if (user.CellphoneIsConfirmed == false)
            {
                ModelState.AddModelError("UserName", "تلفن همراه شما هنوز اعتبارسنجی نشده است !");
                return View(forgotPasswordViewModel);
            }
            if (user.IsActive == false)
            {
                ModelState.AddModelError("UserName", "کاربری شما فعال نشده است !");
                return View(forgotPasswordViewModel);
            }
            user.Password = Core.Generators.GeneratorClass.GeneratePassword(8, "digit");
            _userService.UpdateUser(user);
            await _userService.SaveChangesAsync();
            _userService.SendUserCode_and_password(user.Code, user.Password, user.Cellphone);
            TempData["message"] = "رمز عبور شما تغییر داده شد" + Environment.NewLine + "کد کاربری و رمز عبور به تلفن همراه شما ارسال گردید"; ;
            return RedirectToAction(nameof(Login));
        }
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
        public async Task<CheckCodeViewModel> CheckCode(string Code)
        {
            CheckCodeViewModel checkCodeViewModel = new CheckCodeViewModel();
            if (!string.IsNullOrEmpty(Code))
            {


                User userNc = _userService.GetUserByUserName(Code).Result;
                if (userNc == null)
                {
                    checkCodeViewModel.Validate = false;
                    checkCodeViewModel.Message = "ناظری با این کد سازمانی در سیستم وجود ندارد !";
                    return checkCodeViewModel;
                }
                List<UserRole> userRoles =  _userService.GetUserRolesByUserCode(Code).Result;
                if (userRoles == null)
                {
                    checkCodeViewModel.Validate = false;
                    checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                    return checkCodeViewModel;
                }
                UserRole ParentActiveUserRole = null;
                if (userRoles != null)
                {
                    ParentActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
                    if (ParentActiveUserRole == null)
                    {
                        checkCodeViewModel.Validate = false;
                        checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                        return checkCodeViewModel;
                    }
                    if (ParentActiveUserRole.RoleId == 10)
                    {
                        checkCodeViewModel.Validate = false;
                        checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                        return checkCodeViewModel;
                    }
                    if(ParentActiveUserRole.RoleId == 13)
                    {
                        int ParentSaleStatics = ParentActiveUserRole.User.InitialStatic;
                        double ParentSalePortfo = ParentActiveUserRole.User.InitialPortfo;
                        List<LifeBordroBase> lifeBordroBases = await _bordroService.GetDirectBordroBasebyNC(userNc.NC);
                        if (lifeBordroBases != null)
                        {
                            if (lifeBordroBases.Count() != 0)
                            {
                                ParentSaleStatics += lifeBordroBases.Count();
                                long DepositSum = lifeBordroBases.SelectMany(s => s.LifeBordroAdditions).Where(w => w.IsActive).Sum(x => x.Deposit);
                                double PercentOfDeposit = Math.Round(DepositSum * .1);
                                ParentSalePortfo += lifeBordroBases.SelectMany(s => s.LifeBordroAdditions).Where(w => w.IsActive).Sum(x => x.PremiumbyPaymentMethod) + PercentOfDeposit;
                                if (ParentSaleStatics < 5 || ParentSalePortfo < 50000000)
                                {
                                    checkCodeViewModel.Validate = false;
                                    checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                                    return checkCodeViewModel;
                                }
                            }
                            else
                            {
                                if (ParentSaleStatics < 5 || ParentSalePortfo < 50000000)
                                {
                                    checkCodeViewModel.Validate = false;
                                    checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                                    return checkCodeViewModel;
                                }
                            }
                        }
                        else
                        {
                            if (ParentSaleStatics <= 5 || ParentSalePortfo <= 50000000)
                            {
                                checkCodeViewModel.Validate = false;
                                checkCodeViewModel.Message = "این کد مجوز نظارت ندارد ! !";
                                return checkCodeViewModel;
                            }
                        }
                    }
                    

                }
                
                
                checkCodeViewModel.Validate = true;
                checkCodeViewModel.Message = userNc.FullName;

            }
            return checkCodeViewModel;
        }

    }
}