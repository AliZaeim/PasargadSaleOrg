using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Convertors;
using Core.DTOs.Admin;
using Core.DTOs.General;
using Core.Security;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminUsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IComplementaryService _complementaryService;


        public AdminUsersController(IUserService userService, IComplementaryService complementaryService)
        {
            _userService = userService;
            _complementaryService = complementaryService;
        }
        [PermissionChecker(53)]
        public async Task<IActionResult> UsersWithRole(int? RecCount, int? page)
        {
            List<UserRole> AlluserRoles = null;
            User Loginuser = await _userService.GetUserByCode(User.Identity.Name);
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);
            if (User.Identity.Name == "290070")
            {
                AlluserRoles = await _userService.GetUsers_hasRoleAsync(string.Empty);
            }
            else
            {
                if (Active_userRole != null)
                {
                    AlluserRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).ToList();
                }

            }
            AlluserRoles = AlluserRoles.OrderByDescending(r => int.Parse(r.User.Code)).ToList();
            int zpage = page.GetValueOrDefault(1);
            int zReccount = RecCount.GetValueOrDefault(30);
            ReportUserRolesViewModel reportUserRolesViewModel = new ReportUserRolesViewModel()
            {
                AllUserRoles = AlluserRoles,
                CurPage = page.GetValueOrDefault(1),
                RecCount = RecCount.GetValueOrDefault(30),
                TotalRecCount = AlluserRoles.Count()

            };
            if (AlluserRoles.Count() % RecCount.GetValueOrDefault(30) == 0)
            {
                reportUserRolesViewModel.TotalPages = AlluserRoles.Count() / RecCount.GetValueOrDefault(30);

            }
            else
            {
                reportUserRolesViewModel.TotalPages = (AlluserRoles.Count() / RecCount.GetValueOrDefault(30)) + 1;
            }
            reportUserRolesViewModel.PageUserRoles = AlluserRoles.Skip((zpage - 1) * zReccount).Take(zpage * zReccount).ToList();
            return View(reportUserRolesViewModel);
        }
        [HttpPost]
        [PermissionChecker(53)]
        public async Task<IActionResult> UsersWithRole(int? RecCount, int? page, string search, string SearchField)
        {
            List<UserRole> AlluserRoles = null;
            User Loginuser = await _userService.GetUserByCode(User.Identity.Name);
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);
            if (User.Identity.Name == "290070")
            {
                AlluserRoles = await _userService.GetUsers_hasRoleAsync(string.Empty);
            }
            else
            {
                if (Active_userRole != null)
                {
                    AlluserRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).ToList();
                }

            }
            string sfname = string.Empty;
            if (!string.IsNullOrEmpty(search))
            {
                if (SearchField == "all")
                {
                    sfname = "تمام ستونها";
                    AlluserRoles = AlluserRoles.ToList().Where(w => w.FullPro.Contains(search) ||
                        w.User.Code == search ||
                        (w.UserRoleParent != null && w.UserRoleParent.FullPro.Contains(search)) ||
                        w.User.County.State.StateName == search ||
                        w.User.County.CountyName == search ||
                        w.User.RegDate.ToShamsi().Contains(search)
                        ).ToList();
                }
                if(SearchField == "fullname")
                {
                    sfname = "نام کامل";
                    AlluserRoles = AlluserRoles.Where(w => w.FullPro.Contains(search)).ToList();
                }
                if (SearchField == "role")
                {
                    sfname = "نقش";
                    AlluserRoles = AlluserRoles.Where(w => w.Role.RoleTitle.Contains(search)).ToList();
                }
                if (SearchField == "code")
                {
                    sfname = "کد کاربری";
                    AlluserRoles = AlluserRoles.Where(w => w.User.Code == search).ToList();
                }
                if (SearchField == "state")
                {
                    sfname = "استان";
                    AlluserRoles = AlluserRoles.Where(w => w.User.County.State.StateName.Contains(search)).ToList();
                }
                if (SearchField == "county")
                {
                    sfname = "شهرستان";
                    AlluserRoles = AlluserRoles.Where(w => w.User.County.CountyName.Contains(search)).ToList();
                }
                if (SearchField == "parent")
                {
                    sfname = "ناظر";
                    AlluserRoles = AlluserRoles.Where(w => w.UserRoleParent !=null && w.UserRoleParent.FullPro.Contains(search)).ToList();
                }
                if (SearchField == "rdate")
                {
                    sfname = "تاریخ ثبت";
                    if (search.Count(a => a =='-') == 1)
                    {
                        string sdate = search.Split("-")[0].ToString();
                        string edate = search.Split("-")[1].ToString();
                        if(!string.IsNullOrEmpty(sdate) && !string.IsNullOrEmpty(edate))
                        {
                            DateTime gsdate = sdate.ChangeToMiladiWithoutTime();
                            DateTime gedate = edate.ChangeToMiladiWithoutTime();
                            AlluserRoles = AlluserRoles.Where(w => w.RegisterDate >= gsdate && w.RegisterDate <= gedate).ToList();
                        }
                        else
                        {
                            sfname = "محدوده تاریخ اشتباه";
                        }
                        
                    }
                    else
                    {
                        AlluserRoles = AlluserRoles.Where(w => w.User.RegDate.ToShamsi().Contains(search)).ToList();
                    }
                    
                    
                }
            }
            int zpage = page.GetValueOrDefault(1);
            int zReccount = RecCount.GetValueOrDefault(30);
            ReportUserRolesViewModel reportUserRolesViewModel = new ReportUserRolesViewModel()
            {
                AllUserRoles = AlluserRoles,
                SearchField = sfname,
                CurPage = page.GetValueOrDefault(1),
                RecCount = RecCount.GetValueOrDefault(30),
                TotalRecCount = AlluserRoles.Count(),
                SearchText = search
            };

            if (AlluserRoles.Count() % RecCount.GetValueOrDefault(30) == 0)
            {
                reportUserRolesViewModel.TotalPages = AlluserRoles.Count() / RecCount.GetValueOrDefault(30);
            }
            else
            {
                reportUserRolesViewModel.TotalPages = (AlluserRoles.Count() / RecCount.GetValueOrDefault(30)) + 1;
            }
            reportUserRolesViewModel.PageUserRoles = AlluserRoles.Skip((zpage - 1) * zReccount).Take(zpage * zReccount).ToList();
            return View(reportUserRolesViewModel);
        }
        [PermissionChecker(53)]
        public async Task<IActionResult> UsersWithoutRole()
        {
            return View(await _userService.GetUsers_hasNoRoleAsync());
        }
        [PermissionChecker(53)]
        public async Task<IActionResult> AddRoleToUser(int userId)
        {
            User user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {

                AddRoleToUserViewModel addRoleToUserViewModel = new AddRoleToUserViewModel()
                {
                    User = user,
                    UserId = userId,
                    Roles = await _userService.GetAllRolesAsync(),
                    Parents = await _userService.GetUserRolesByUserNC(user.SponserCode),
                    IsSuccess = false
                };
                return PartialView(addRoleToUserViewModel);
            }
            return PartialView();

        }
        
        [PermissionChecker(56)]
        public async Task<IActionResult> ChangeParent(int urId)
        {
            ChangeParentViewModel changeParentViewModel = new ChangeParentViewModel()
            {
                User_URId = urId

            };
            changeParentViewModel.userRole = await _userService.GetUserRoleByIdAsync(urId);
            changeParentViewModel.CuParent = changeParentViewModel.userRole.UserRoleParent;
            List<UserRole> userRoles = await _userService.GetUserRoles();
            userRoles = userRoles.Where(w => w.URId != urId && w.IsActive == true && w.User.IsActive == true).ToList();
            changeParentViewModel.Parents = userRoles;
            if (changeParentViewModel.CuParent != null)
            {
                userRoles = userRoles.Where(w => w.URId != changeParentViewModel.CuParent.URId).ToList();
                changeParentViewModel.Parents = userRoles;
            }

            return PartialView(changeParentViewModel);
        }
        [PermissionChecker(56)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeParent(ChangeParentViewModel changeParentViewModel)
        {
            if (!ModelState.IsValid)
            {
                changeParentViewModel.userRole = await _userService.GetUserRoleByIdAsync(changeParentViewModel.User_URId);
                changeParentViewModel.CuParent = changeParentViewModel.userRole.UserRoleParent;
                List<UserRole> userRoles = await _userService.GetUserRoles();
                userRoles = userRoles.Where(w => w.URId != changeParentViewModel.User_URId).ToList();
                changeParentViewModel.Parents = userRoles;
                if (changeParentViewModel.CuParent != null)
                {
                    userRoles = userRoles.Where(w => w.URId != changeParentViewModel.CuParent.URId && w.IsActive == true && w.User.IsActive == true).ToList();
                    changeParentViewModel.Parents = userRoles;
                }
                return PartialView(changeParentViewModel);
            }
            //await _userService.Change_UserRole_Parent(changeParentViewModel.User_URId, changeParentViewModel.User_NewParent_URId, User.Identity.Name);
            await _userService.Change_Parent_Of_UserRole(changeParentViewModel.User_URId, changeParentViewModel.User_NewParent_URId, User.Identity.Name);
            //_userService.Recursive_Change_UserRole_Parent(changeParentViewModel.User_URId, changeParentViewModel.User_NewParent_URId, User.Identity.Name);
            _userService.SaveChange();
            return RedirectToAction(nameof(UsersWithRole));
        }
        [PermissionChecker(53)]
        public async Task<IActionResult> CreateUser()
        {
            RegisterUserViewModel registerUserViewModel = new RegisterUserViewModel()
            {
                Roles = await _userService.GetAllRolesAsync()
            };
            return View(registerUserViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(53)]
        public async Task<IActionResult> CreateUser(RegisterUserViewModel registerUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                registerUserViewModel.Roles = await _userService.GetAllRolesAsync();
                return View(registerUserViewModel);
            }
            if (Core.Utility.MyUtility.IsValidNC(registerUserViewModel.UserNC) == false)
            {
                ModelState.AddModelError("UserNC", "کد ملی نامعتبر است !");
                return View(registerUserViewModel);
            }
            if (await _userService.ExistUserNC(registerUserViewModel.UserNC))
            {
                ModelState.AddModelError("UserNC", "این کد ملی قبلا ثبت شده است !");
                return View(registerUserViewModel);
            }
            if (await _userService.ExistUserCellphone(registerUserViewModel.UserCellphone))
            {
                ModelState.AddModelError("UserCellphone", "این تلفن همراه قبلا ثبت شده است !");
                return View(registerUserViewModel);
            }
            User Myuser = new User()
            {
                FName = registerUserViewModel.UserFirstName,
                LName = registerUserViewModel.UserFamiy,
                NC = registerUserViewModel.UserNC,
                Cellphone = registerUserViewModel.UserCellphone,
                CountyId = registerUserViewModel.CountyId,
                Code = registerUserViewModel.UserCellphone,
                Password = Core.Security.PasswordHelper.EncodePasswordMd5(registerUserViewModel.UserNC),
                BirthDate = Core.Convertors.DateConvertor.ChangeToMiladiWithoutTime(registerUserViewModel.BDateYear + "/" + registerUserViewModel.BDateMounth + "/" + registerUserViewModel.BDateDay),
                RegDate = DateTime.Now

            };
            await _userService.CreateUserAsync(Myuser);
            UserRole userRole = new UserRole()
            {
                RegisterDate = DateTime.Now,
                RoleId = (int)registerUserViewModel.RoleId,
                User_ID = Myuser.Id,
                IsActive = true

            };
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(UsersWithRole));
        }
        [HttpPost]
        public async Task<bool> ChangeUserState(string userCode, bool state)
        {
            User user = await _userService.GetUserByCode(userCode);
            if (user == null)
            {
                return false;
            }
            user.IsActive = state;
            _userService.UpdateUser(user);
            await _userService.SaveChangesAsync();
            return true;
        }
        [HttpPost]
        public async Task<IActionResult> EditInitialStatic(int userId, int InitialStatic, long InitialPortfo)
        {
            if (userId != 0)
            {
                User user = await _userService.GetUserByIdAsync(userId);
                user.InitialStatic = InitialStatic;
                user.InitialPortfo = InitialPortfo;
                _userService.UpdateUser(user);
                await _userService.SaveChangesAsync();
                return RedirectToAction(nameof(UsersWithoutRole));
            }

            return View();
        }
        [PermissionChecker(55)]
        public async Task<IActionResult> UserRolesDetails(int Id)
        {
            return View(await _userService.GetUserRoleByIdAsync(Id));
        }
        public IActionResult OrgUsers()
        {
            return View();
        }
        [Route("Admin/ShowChilderens/{code?}")]
        public async Task<IActionResult> ShowChilderens(int? code)
        {
            ShowChilderensVM showChilderensVM = new ShowChilderensVM();
            List<UserRole> Login_userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_Login_userRole = Login_userRoles.FirstOrDefault(f => f.IsActive == true);
            List<UserRole> LoginUser_Childs = _userService.GetAllChilds(Active_Login_userRole.URId, 0, 0).Select(s => s.UserRole).ToList();

            showChilderensVM.ActiveLoginUserRole = Active_Login_userRole;
            showChilderensVM.LoginUserChilderens = LoginUser_Childs;
            if (code == null)
            {


                List<UserRole> SelectedUser_UserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
                showChilderensVM.ActiveSelectedUser_UserRole = Active_Login_userRole;
                showChilderensVM.SelectedUserRoleChilderens = LoginUser_Childs;
                return View(showChilderensVM);
            }
            else
            {
                if (LoginUser_Childs.Any(a => a.User.Code == code.ToString()) || Login_userRoles.Any(a => a.User.Code == code.ToString()))
                {
                    List<UserRole> SelectedUser_UserRoles = await _userService.GetUserRolesByUserCode(code.ToString());
                    showChilderensVM.ActiveSelectedUser_UserRole = SelectedUser_UserRoles.FirstOrDefault(f => f.IsActive == true);
                    showChilderensVM.LoginUserChilderens = LoginUser_Childs;
                    showChilderensVM.SelectedUserRoleChilderens = _userService.GetAllChilds(SelectedUser_UserRoles.FirstOrDefault(f => f.IsActive == true).URId, 0, 0).Select(s => s.UserRole).ToList();
                    return View(showChilderensVM);
                }
                else
                {

                    string res = "<div  style='display: flex;justify-content: center;margin-top: 10mm;width : 100%;color:brown;'><h2 style='width:50%;padding:5mm;margin:auto 0 ;text-align:center;height:30mm;padding-top:15mm;border:1px solid grey;background-color:#e7b7b7'>...مجوز مشاهده اطلاعات این کد را ندارید</h3><br/><a style='float:left' href='/Admin/ShowChilderens'>بازگشت</a></div>";
                    return Content(res, "text/html", Encoding.UTF8);
                }


            }

        }
        /// <summary>
        /// تغییر نقش کاربر
        /// </summary>
        [PermissionChecker(57)]
        public async Task<IActionResult> ChangeRoleofUser(int urId)
        {
            UserRole userRole = await _userService.GetUserRoleByIdAsync(urId);
            if (userRole == null)
            {
                return NotFound();
            }
            ChangeRoleofUserVM changeRoleofUserVM = new ChangeRoleofUserVM()
            {
                User = userRole.User,
                Roles = await _userService.GetAllRolesAsync()
            };
            return PartialView(changeRoleofUserVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(57)]
        public async Task<IActionResult> ChangeRoleofUser(ChangeRoleofUserVM changeRoleofUserVM)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(changeRoleofUserVM);
            }
            bool Res = await _userService.ChangeRoleofUserAsync(changeRoleofUserVM.UrId, changeRoleofUserVM.NewRoleId);
            _userService.SaveChange();
            changeRoleofUserVM.IsSuccess = true;
            return RedirectToAction(nameof(UsersWithRole));
        }
        public async Task<IActionResult> ListCountiesofState(int stateId)
        {
            List<County> counties = await _complementaryService.GetCountiesofStateAsync(stateId);
            return PartialView(counties);
        }
        [PermissionChecker(54)]
        public async Task<IActionResult> EditUser(int? Id)
        {
            if (Id == null)
            {
                return NotFound("کاربر درست انتخاب نشده است !");
            }
            User user = await _userService.GetUserByIdAsync((int)Id);
            UserUpdateVM userUpdateVM = new UserUpdateVM
            {
                States = await _complementaryService.GetStatesAsync(),
                Counties = await _complementaryService.GetCountiesofStateAsync(user.County.StateId),
                FName = user.FName,
                LName = user.LName,
                NC = user.NC,
                FatherName = user.FatherName,
                Education = user.Education,
                StateId = user.County.StateId,
                CountyId = user.CountyId,
                HomeAddress = user.HomeAddress,
                WorkAddress = user.WorkAddress,
                PasargadOrgCode = user.PasargadOrgCode,
                HomePostalCode = user.HomePostalCode,
                WorkPostalCode = user.WorkPostalCode,
                InitialPortfo = user.InitialPortfo,
                InitialStatic = user.InitialStatic,
                Cellphone = user.Cellphone,
                Phone = user.Phone,
                Email = user.Email,
                SponserBranch = user.SponserBranch,
                SponserCode = user.SponserCode,
                BankAccountNumber = user.BankAccountNumber,
                BankCardNumber = user.BankCardNumber?.GetCardNumberWithSeprator()

            };
            //if(!string.IsNullOrEmpty(user.SponserCode))
            //{
            //    userUpdateVM.SponserCode = user.SponserCode;
            //}
            return View(userUpdateVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(54)]
        public async Task<IActionResult> EditUser(UserUpdateVM userUpdateVM)
        {
            if (!ModelState.IsValid)
            {

                userUpdateVM.States = await _complementaryService.GetStatesAsync();
                userUpdateVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateVM.StateId);
                return View(userUpdateVM);
            }
            User userNc = await _userService.GetUserByNC(userUpdateVM.NC.Trim());
            User userCellphone = await _userService.GetUserByCellphone(userUpdateVM.Cellphone.Trim());
            if (userCellphone != null)
            {
                if (userCellphone.IsActive == true)
                {
                    if (userNc != userCellphone)
                    {
                        userUpdateVM.States = await _complementaryService.GetStatesAsync();
                        userUpdateVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)userUpdateVM.StateId);
                        ModelState.AddModelError("Cellphone", "شماره وارد شده در سیستم فعال است !");
                        return View(userUpdateVM);
                    }

                }
            }
            User user = await _userService.GetUserByIdAsync(userUpdateVM.Id);
            user.FName = userUpdateVM.FName;
            user.LName = userUpdateVM.LName;
            user.FatherName = userUpdateVM.FatherName;
            user.NC = userUpdateVM.NC;
            user.Education = userUpdateVM.Education;
            user.Email = user.Email;
            user.HomeAddress = userUpdateVM.HomeAddress;
            user.HomePostalCode = userUpdateVM.HomePostalCode;
            user.WorkAddress = userUpdateVM.WorkAddress;
            user.WorkPostalCode = userUpdateVM.WorkPostalCode;
            user.Phone = userUpdateVM.Phone;
            user.CountyId = userUpdateVM.CountyId;
            user.InitialPortfo = userUpdateVM.InitialPortfo;
            user.InitialStatic = userUpdateVM.InitialStatic;
            user.Cellphone = userUpdateVM.Cellphone;
            user.PasargadOrgCode = userUpdateVM.PasargadOrgCode;
            user.SponserBranch = userUpdateVM.SponserBranch;
            user.SponserCode = userUpdateVM.SponserCode;
            user.BankAccountNumber = userUpdateVM.BankAccountNumber;
            user.BankCardNumber = userUpdateVM.BankCardNumber;
            _userService.UpdateUser(user);
            await _userService.SaveChangesAsync();
            return RedirectToAction(nameof(UsersWithRole));
        }
        /// <summary>
        /// کاربران به جز سازمان فروش
        /// </summary>
        /// <returns></returns>
        [PermissionChecker(62)]
        public async Task<IActionResult> NonOrgUsersIndex()
        {
            return View(await _userService.GetNonOrgUserRoles());
        }
        /// <summary>
        /// ثبت نام اپراتور و کاربر ویژه
        /// </summary>
        /// <returns></returns>
        [PermissionChecker(63)]
        public async Task<IActionResult> CreateNonOrgUser()
        {
            CreateAdminUserVM createAdminUserVM = new CreateAdminUserVM()
            {
                States = await _complementaryService.GetStatesAsync()
            };
            return View(createAdminUserVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(63)]
        public async Task<IActionResult> CreateNonOrgUser(CreateAdminUserVM createAdminUserVM)
        {
            if (!ModelState.IsValid)
            {
                createAdminUserVM.States = await _complementaryService.GetStatesAsync();
                createAdminUserVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)createAdminUserVM.StateId);
                return View(createAdminUserVM);
            }
            if (!createAdminUserVM.UserNC.IsValidNC())
            {
                ModelState.AddModelError("UserNC", "کد ملی نامعبر است !");
            }
            User user = await _userService.GetUserByNC(createAdminUserVM.UserNC);

            if (user != null)
            {
                List<UserRole> userRoles = await _userService.GetUserRolesByUserNC(createAdminUserVM.UserNC);
                if (userRoles.Any(a => a.RoleId == createAdminUserVM.RoleId))
                {
                    ModelState.AddModelError("RoleId", createAdminUserVM.UserFirstName + " " + createAdminUserVM.UserFamily + " " + "بااین نقش قبلا ثبت شده است");
                    createAdminUserVM.States = await _complementaryService.GetStatesAsync();
                    createAdminUserVM.Counties = await _complementaryService.GetCountiesofStateAsync((int)createAdminUserVM.StateId);
                    return View(createAdminUserVM);

                }
                UserRole userRole = new UserRole()
                {
                    User_ID = user.Id,
                    RoleId = (int)createAdminUserVM.RoleId,
                    RegisterDate = DateTime.Now,
                    IsActive = true,
                    OP_Create = User.Identity.Name,
                };
                _userService.CreateUserRole(userRole);
                await _userService.SaveChangesAsync();
            }
            else
            {
                User Reguser = new User()
                {
                    FName = createAdminUserVM.UserFirstName,
                    LName = createAdminUserVM.UserFamily,
                    NC = createAdminUserVM.UserNC,
                    Cellphone = createAdminUserVM.UserCellphone,
                    CellphoneIsConfirmed = true,
                    IsActive = true,
                    RegDate = DateTime.Now,
                    Code = createAdminUserVM.UserCellphone,
                    Password = createAdminUserVM.UserNC,
                    BirthDate = createAdminUserVM.UserBirthDate.ChangeToMiladi("00:00"),
                    CountyId = createAdminUserVM.CountyId,
                    OP_Create = User.Identity.Name
                };
                _userService.CreateUser(user);
                UserRole userRole = new UserRole()
                {
                    User_ID = Reguser.Id,
                    RoleId = (int)createAdminUserVM.RoleId,
                    IsActive = true,
                    RegisterDate = DateTime.Now,
                    OP_Create = User.Identity.Name,

                };
                _userService.CreateUserRole(userRole);
                await _userService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(NonOrgUsersIndex));
        }
        [HttpPost]
        public JsonResult SearchUserWithNC(string NC)
        {
            User user = _userService.GetUserByNC(NC).Result;
            if (user != null)
            {
                CreateAdminUserVM createAdminUserVM = new CreateAdminUserVM()
                {
                    UserFirstName = user.FName,
                    UserFamily = user.LName,
                    UserBirthDate = user.BirthDate.ToShamsi(),
                    UserCellphone = user.Cellphone,
                    StateId = user.County.StateId,
                    CountyId = user.CountyId

                };
                return Json(createAdminUserVM);
            }
            return null;
        }
        [HttpPost]
        public async Task<IActionResult> WriteUsersExcelFile()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<UserRole> userRoles = null;
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);
            string tit = string.Empty;
            if (User.Identity.Name == "290070")
            {
                userRoles = await _userService.GetUsers_hasRoleAsync(string.Empty);
                 tit = "لیست کامل کاربران دارای نقش" + " | " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            }
            else
            {
                if (Active_userRole != null)
                {
                    userRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).ToList();
                }
                tit = "لیست کامل همکاران" + " | " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();

            }
            userRoles = userRoles.Where(w => w.UserRoleParent != null && w.IsActive).ToList();
            List<UsersInfoModelForExcel> usersInfoModelForExcels = userRoles.ToList().Select(x => new UsersInfoModelForExcel
            {
                Name = x.User.FName,
                Family = x.User.LName,
                Code = x.User.Code,
                ActiveRole = x.Role.RoleTitle,
                BirthDate = x.User.BirthDate.ToShamsi(),
                FatherName = x.User.FatherName,
                Cellphone = x.User.Cellphone,
                Phone = x.User.Phone,
                Ejucation = x.User.Education,
                RegisterDate = x.User.RegDate.ToShamsi(),
                BankCardNumber = x.User.BankCardNumber,
                BonkAccountNumber = x.User.BankAccountNumber,
                Email = x.User.Email,
                HomeAddress = x.User.County.State.StateName + " - " + x.User.County.CountyName + " - " + x.User.HomeAddress,
                InitialPortfo = x.User.InitialPortfo.ToString(),
                InitialStatic = x.User.InitialStatic.ToString(),
                Supervisor = x.UserRoleParent.User.FullName,

            }).ToList();
           
            IWorkbook workbook = _userService.WriteExcelWithNPOI(new UsersInfoModelForExcel(), usersInfoModelForExcels,tit);
            string contentType; // Scope

            MemoryStream tempStream = null;
            MemoryStream stream = null;
            try
            {
                // 1. Write the workbook to a temporary stream
                tempStream = new MemoryStream();
                workbook.Write(tempStream);
                // 2. Convert the tempStream to byteArray and copy to another stream
                var byteArray = tempStream.ToArray();
                stream = new MemoryStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Seek(0, SeekOrigin.Begin);
                // 3. Set file content type
                contentType = workbook.GetType() == typeof(XSSFWorkbook) ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/vnd.ms-excel";
                // 4. Return file
                return File(
                    fileContents: stream.ToArray(),
                    contentType: contentType,
                    fileDownloadName: "UsersInfo " + user.Code + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }

        public async Task<IActionResult> CoworkersInfo()
        {
            List<UserRole> AlluserRoles = null;
            User user = await _userService.GetUserByCode(User.Identity.Name);
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);

            if (Active_userRole != null)
            {
                AlluserRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).Where(w => w.IsActive == true).ToList();
            }
            AlluserRoles = AlluserRoles.OrderByDescending(r => int.Parse(r.User.Code)).ToList();
            int zpage = 1;
            int zReccount = 30;
            ReportUserRolesViewModel reportUserRolesViewModel = new ReportUserRolesViewModel()
            {
                AllUserRoles = AlluserRoles,
                CurPage = 1,
                RecCount = 30,
                TotalRecCount = AlluserRoles.Count()

            };
            if (AlluserRoles.Count() % zReccount == 0)
            {
                reportUserRolesViewModel.TotalPages = AlluserRoles.Count() / zReccount;

            }
            else
            {
                reportUserRolesViewModel.TotalPages = (AlluserRoles.Count() / zReccount) + 1;
            }
            reportUserRolesViewModel.PageUserRoles = AlluserRoles.Skip((zpage - 1) * zReccount).Take(zpage * zReccount).ToList();
            return View(reportUserRolesViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CoworkersInfo(int? RecCount, int? page, string search)
        {
            List<UserRole> AlluserRoles = null;
            User Loginuser = await _userService.GetUserByCode(User.Identity.Name);
            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);

            if (Active_userRole != null)
            {
                AlluserRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).Where(w => w.IsActive).ToList();
                AlluserRoles = AlluserRoles.OrderByDescending(r => int.Parse(r.User.Code)).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                AlluserRoles = AlluserRoles.Where(w => w.FullPro.Contains(search) ||
                w.User.FullName.Contains(search) ||
                w.Role.RoleTitle.Contains(search) ||
                w.User.FName.Contains(search) ||
                w.User.LName.Contains(search) ||
                w.User.NC.Contains(search) ||
                w.User.BirthDate.ToShamsi().Contains(search) ||
                w.User.Cellphone.Contains(search) ||
                w.User.Code.Contains(search) ||
                w.User.County.CountyName.Contains(search) ||
                w.User.County.State.StateName.Contains(search) ||
                w.User.Education.Contains(search)

            ).ToList();
            }


            int zpage = page.GetValueOrDefault(1);
            int zReccount = RecCount.GetValueOrDefault(30);
            ReportUserRolesViewModel reportUserRolesViewModel = new ReportUserRolesViewModel()
            {
                AllUserRoles = AlluserRoles,

                CurPage = page.GetValueOrDefault(1),
                RecCount = RecCount.GetValueOrDefault(30),
                TotalRecCount = AlluserRoles.Count(),
                SearchText = search
            };

            if (AlluserRoles.Count() % RecCount.GetValueOrDefault(30) == 0)
            {
                reportUserRolesViewModel.TotalPages = AlluserRoles.Count() / RecCount.GetValueOrDefault(30);
            }
            else
            {
                reportUserRolesViewModel.TotalPages = (AlluserRoles.Count() / RecCount.GetValueOrDefault(30)) + 1;
            }
            reportUserRolesViewModel.PageUserRoles = AlluserRoles.Skip((zpage - 1) * zReccount).Take(zReccount).ToList();
            return View(reportUserRolesViewModel);
        }
        public async Task<IActionResult> CoworkersDetails(int urid)
        {
            List<UserRole> AlluserRoles = null;

            List<UserRole> LoginUserRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole Active_userRole = LoginUserRoles.FirstOrDefault(f => f.IsActive);

            if (Active_userRole != null)
            {
                AlluserRoles = _userService.GetAllChilds(Active_userRole.URId).Select(x => x.UserRole).Where(w => w.IsActive).ToList();
            }
            if (!AlluserRoles.Any(a => a.URId == urid))
            {
                return NotFound("مجوز مشاهده اطلاعات کاربر را ندارید !");
            }
            return View(await _userService.GetUserRoleByIdAsync(urid));
        }




    }
}