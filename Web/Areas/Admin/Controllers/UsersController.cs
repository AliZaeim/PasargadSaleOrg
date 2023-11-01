using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs.Account;
using Core.DTOs.General;
using Core.Generators;
using Core.Services.Interfaces;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IComplementaryService _complementaryService;
        public UsersController(IUserService userService,IComplementaryService complementaryService)
        {
            _userService = userService;
            _complementaryService = complementaryService;
        }
        //public IActionResult UploadNC()
        //{
        //    return PartialView();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UploadNC(UploadDocumentViewModel uploadDocumentViewModel, FormFile FileName)
        //{
        //    if(!ModelState.IsValid)
        //    {
        //        return PartialView(uploadDocumentViewModel);
        //    }
        //    return PartialView();
        //}
       
        public async Task<IActionResult> ShowProfile()
        {

            User user = await _userService.GetUserByUserName(User.Identity.Name).ConfigureAwait(false);
            return View(user);
        }
        public async Task<IActionResult> UpdateProfile()
        {
            User current = await _userService.GetUserByCode(User.Identity.Name);
            if (current == null)
            {
                return NotFound("مجاز به ویرایش اطلاعات کاربر خود نیستید !");
            }

            UserUpdateProFileVM userUpdateProFileVM = new UserUpdateProFileVM()
            {
                Id = current.Id,
                BankAccountNumber = current.BankAccountNumber,
                BankCardNumber = current.BankCardNumber,
                StateId = current.County.StateId,
                CountyId = current.CountyId,
                HomeAddress = current.HomeAddress,
                Education = current.Education,
                Email = current.Email,
                Avatar = current.Avatar,
                States = await _complementaryService.GetStatesAsync(),
                Counties = await _complementaryService.GetCountiesofStateAsync(current.County.StateId)
            };
            return View(userUpdateProFileVM);

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserUpdateProFileVM userUpdateProFileVM, IFormFile Avatar)
        {
            if(!ModelState.IsValid)
            {
                userUpdateProFileVM.States = await _complementaryService.GetStatesAsync();
                userUpdateProFileVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateProFileVM.StateId);
            }
            User user = await _userService.GetUserByIdAsync(userUpdateProFileVM.Id);
            if (await _userService.ExistUserBankAccount(userUpdateProFileVM.BankAccountNumber))
            {
                User BAN_User = await _userService.GetUserByBankAccountNumber(userUpdateProFileVM.BankAccountNumber);
                if(BAN_User != user)
                {
                    ModelState.AddModelError("BankAccountNumber", "این شماره حساب بانکی قبلا در سیستم ثبت شده است !");
                    userUpdateProFileVM.States = await _complementaryService.GetStatesAsync();
                    userUpdateProFileVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateProFileVM.StateId);
                    return View(userUpdateProFileVM);
                }
               
            }
            if (await _userService.ExistUserBankCard(userUpdateProFileVM.BankCardNumber))
            {
                User BCN_User = await _userService.GetUserByBankCardNumber(userUpdateProFileVM.BankCardNumber);
                if(BCN_User != user)
                {
                    ModelState.AddModelError("BankCardNumber", "این شماره کارت بانکی قبلا در سیستم ثبت شده است !");
                    userUpdateProFileVM.States = await _complementaryService.GetStatesAsync();
                    userUpdateProFileVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateProFileVM.StateId);
                    return View(userUpdateProFileVM);
                }
                
            }
            if(!userUpdateProFileVM.BankAccountNumber.Contains("."))
            {
                ModelState.AddModelError("BankAccountNumber", "از جدا کننده . در شماره حساب استفاده نشده است !");
                userUpdateProFileVM.States = await _complementaryService.GetStatesAsync();
                userUpdateProFileVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateProFileVM.StateId);
                return View(userUpdateProFileVM);
            }
            
            if (Avatar != null)
            {
                if(Avatar.Length > .5*1024*1024)
                {
                    ModelState.AddModelError("Avatar", "حجم عکس از 500 کیلو بایت بیشتر است !");
                    userUpdateProFileVM.States = await _complementaryService.GetStatesAsync();
                    userUpdateProFileVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateProFileVM.StateId);
                    return View(userUpdateProFileVM);
                }
                
                string avfileName = GeneratorClass.GenerateUniqueCode() + Path.GetExtension(Avatar.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/usersavatar", avfileName);

               
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Avatar.CopyToAsync(stream);
                }
                user.Avatar = avfileName;
            }
            
            user.BankAccountNumber = userUpdateProFileVM.BankAccountNumber;
            user.BankCardNumber = userUpdateProFileVM.BankCardNumber;
            user.CountyId = userUpdateProFileVM.CountyId;
            user.HomeAddress = userUpdateProFileVM.HomeAddress;
            user.Education = userUpdateProFileVM.Education;
            user.Email = userUpdateProFileVM.Email;
            _userService.UpdateUser(user);
            await _userService.SaveChangesAsync();
            userUpdateProFileVM.Message = "ویرایش پروفایل با موفقیت انجام شد";


            return RedirectToAction("ShowProfile");
        }


        [Route("ChangePassword")]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel()
            {
                IsSuccess = false
            };
            return View(changePasswordViewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }

            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user.Password !=changePasswordViewModel.Password)
            {
                ModelState.AddModelError("Password", "رمز عبور فعلی نادرست است !");
            }
            user.Password = changePasswordViewModel.NewPassowrd;
            
            user.OP_Update += "-" + User.Identity.Name + "|" + DateTime.Now;
            _userService.UpdateUser(user);
            await _userService.SaveChangesAsync();
            changePasswordViewModel.IsSuccess = true;
            return View(changePasswordViewModel);
        }
    }
}