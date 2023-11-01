using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Convertors;
using Core.DTOs.Admin;
using Core.DTOs.General;
using Core.Security;
using Core.Services.Interfaces;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Stimulsoft.Base;


namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
   
    public class BordroReportsController : Controller
    {
        private readonly IBordroService _bordroService;
        private readonly IUserService _userService;

        public BordroReportsController(IBordroService bordroService, IUserService userService)
        {
            _bordroService = bordroService;
            _userService = userService;

            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");
        }
        [PermissionChecker(22)]
        public async Task<IActionResult> LifeAdditions()
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> IndbordroBases = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                IndbordroBases = IndbordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                List<LifeBordroBase> DirbordroBase = await _bordroService.GetDirectBordroBasebyNC(item.User.NC);
                DirbordroBase = DirbordroBase.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                if (DirbordroBase != null)
                {
                    lifeBordroBases.AddRange(DirbordroBase);
                }
                if (IndbordroBases != null)
                {
                    if (IndbordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(IndbordroBases);
                    }

                }
                lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).Distinct().ToList();
                
                
            }
            int recCount = 30;
            
            ReportLifeInsurancesVM reportLifeInsurancesVM = new ReportLifeInsurancesVM()
            {
                AllBordroes = lifeBordroBases,
                TotalRecCount = lifeBordroBases.Count(),
                CurPage = 1,
                RecCount = recCount,
                PageBordroes = lifeBordroBases.Take(recCount).ToList()
            };
            


            if (lifeBordroBases.Count() % recCount == 0)
            {
                reportLifeInsurancesVM.TotalPages = lifeBordroBases.Count() / recCount;
            }
            else
            {
                reportLifeInsurancesVM.TotalPages = (lifeBordroBases.Count() / recCount) + 1;
            }

            return View(reportLifeInsurancesVM);
        }
        [HttpPost]
        public async Task<IActionResult> LifeAdditions(int? RecCount, int? page, string search, string searchField)
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            string sfName = string.Empty;
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> IndbordroBases = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                IndbordroBases = IndbordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                List<LifeBordroBase> DirbordroBase = await _bordroService.GetDirectBordroBasebyNC(item.User.NC);
                DirbordroBase = DirbordroBase.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                if (DirbordroBase != null)
                {
                    lifeBordroBases.AddRange(DirbordroBase);
                }
                if (IndbordroBases != null)
                {
                    if (IndbordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(IndbordroBases);
                    }

                }
                
                lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).Distinct().ToList();
                //lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.InsurerFullName.Contains(search))).ToList();
                if(!string.IsNullOrEmpty(searchField))
                {
                    if(!string.IsNullOrEmpty(search))
                    {
                        if(searchField == "all")
                        {
                            sfName = "تمام ستونها";
                            lifeBordroBases = lifeBordroBases.Where(w =>
                                w.IssueDate.ToShamsi().Contains(search) || 
                                w.InsNO.Contains(search) || 
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.InsurerFullNameList.Any(n => n == search)) ||
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.InsuredFullNameList.Any(n => n == search)) ||
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.SellerFullNameList.Any(n => n == search)) ||
                                
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.PaymentMethod==search.Trim()) ||
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.InsuredNC == search) ||
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.Type.Contains(search)) ||                                 
                                w.LifeBordroAdditions.Any(a => a.IsActive && a.Status.Contains(search))

                            ).ToList();
                        }
                       
                        
                        if(searchField == "insno")
                        {
                            sfName = "شماره بیمه نامه";
                            lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search)).ToList();
                        }
                        if (searchField == "issuedate")
                        {
                            sfName = "تاریخ صدور";
                            lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate.ToShamsi().Contains(search)).ToList();
                        }
                        if (searchField == "insurerfullname")
                        {
                            sfName = "بیمه گذار";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.InsurerFullNameList.Any(n => n == search))).ToList();
                        }
                        if (searchField == "insuredfullname")
                        {
                            sfName = "بیمه شده";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.InsuredFullNameList.Any(n => n == search))).ToList();
                        }
                        if (searchField == "paymethod")
                        {
                            sfName = "روش پرداخت";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.PaymentMethod == search.Trim())).ToList();
                        }
                        if (searchField == "agent")
                        {
                            sfName = "نماینده";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.SellerFullNameList.Any(n => n == search.Trim()))).ToList();
                        }
                        if (searchField == "state")
                        {
                            sfName = "وضعیت";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.Status.Contains(search.Trim()))).ToList();
                        }
                        if (searchField == "type")
                        {
                            sfName = "نوع";
                            lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.IsActive && a.Type.Contains(search.Trim()))).ToList();
                        }

                    }
                }



            }
            int zcount = RecCount.GetValueOrDefault(30);
            int zpage = page.GetValueOrDefault(1);
            ReportLifeInsurancesVM reportLifeInsurancesVM = new ReportLifeInsurancesVM()
            {
                AllBordroes = lifeBordroBases,
                TotalRecCount = lifeBordroBases.Count(),
                CurPage = 1,
                RecCount = zcount,
                SearchField = searchField,
                SearchText = search,
                SearchFieldName = sfName,
                PageBordroes = lifeBordroBases.Skip((zpage-1)*zcount).Take(zpage*zcount).ToList()
            };
            if (lifeBordroBases.Count() % zcount == 0)
            {
                reportLifeInsurancesVM.TotalPages = lifeBordroBases.Count() / zcount;
            }
            else
            {
                reportLifeInsurancesVM.TotalPages = (lifeBordroBases.Count() / zcount) + 1;
            }
            return View(reportLifeInsurancesVM);
        }
        [PermissionChecker(24)]
        public async Task<IActionResult> NonPaymentBordroes()
        {   
            int recCount = 30;
            List<NonePaymentBordroesDet> nonePaymentBordroesDets = await _bordroService.GetNonPaidBordroesAsync(User.Identity.Name);
            NonPaymentBordroesModel nonPaymentBordroesModel = new NonPaymentBordroesModel()
            {
                NonePaymentBordroesDets =nonePaymentBordroesDets.Take(recCount).ToList(),
                TotalRecCount = nonePaymentBordroesDets.Count(),
                CurPage = 1,
                RecCount = recCount
            };
            if (nonePaymentBordroesDets.Count() % recCount == 0)
            {
                nonPaymentBordroesModel.TotalPages = nonePaymentBordroesDets.Count() / recCount;
            }
            else
            {
                nonPaymentBordroesModel.TotalPages = (nonePaymentBordroesDets.Count() / recCount) + 1;
            }
            return View(nonPaymentBordroesModel);
        }
        [PermissionChecker(24)]
        [HttpPost]
        public async Task<IActionResult> NonPaymentBordroes(int? RecCount, int? page, string search, string searchField)
        {
            
            int zpage = page.GetValueOrDefault(1);
            int zcount = RecCount.GetValueOrDefault(30);
            string srchName = string.Empty;
            List<NonePaymentBordroesDet> nonePaymentBordroesDets = await _bordroService.GetNonPaidBordroesAsync(User.Identity.Name);
            if(!string.IsNullOrEmpty(search))
            {
                if(searchField =="all")
                {
                    srchName = "تمام ستونها";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.InsNO.Contains(search) ||
                    w.Insured.Contains(search) || 
                    w.Insurer.Contains(search) ||
                    w.InsuredPhone.Contains(search) ||
                    w.IssueDate.ToShamsi().Contains(search) || 
                    w.LastReceiveDate.ToShamsi().Contains(search) ||
                    w.Seller.Contains(search) ||
                    w.PaymentMethod.Contains(search) ||
                    w.PaymentMethodValue.ToString() == search ||
                    w.Status.Contains(search) ||
                    w.Type.Contains(search) || 
                    w.TotalPremiumReceived.ToString() == search ||
                    w.NonReceivedCount.ToString() == search
                    ).ToList();
                }
                if(searchField =="insno")
                {
                    srchName = "شماره بیمه نامه";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.InsNO.Contains(search)).ToList();
                }
                if (searchField == "issuedate")
                {
                    srchName = "تاریخ صدور";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.IssueDate.ToShamsi().Contains(search)).ToList();
                }
                if (searchField == "insurerfullname")
                {
                    srchName = "بیمه گذار";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.Insurer.Contains(search)).ToList();
                }
                if (searchField == "insuredfullname")
                {
                    srchName = "بیمه شده";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.Insured.Contains(search)).ToList();
                }
                if (searchField == "phone")
                {
                    srchName = "تلفن بیمه شده";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.InsuredPhone.Contains(search)).ToList();
                }
                if (searchField == "agent")
                {
                    srchName = "عامل فروش";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.Seller.Contains(search)).ToList();
                }
                if (searchField == "paymethod")
                {
                    srchName = "روش پرداخت";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.PaymentMethod.Contains(search)).ToList();
                }
                if (searchField == "paymethodvalue")
                {
                    srchName = "حق بیمه";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.PaymentMethodValue.ToString().Contains(search)).ToList();
                }
                if (searchField == "state")
                {
                    srchName = "وضعیت";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.Status.Contains(search)).ToList();
                }
               
                if (searchField == "type")
                {
                    srchName = "نوع";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.Type.Contains(search)).ToList();
                }
                if (searchField == "lastrDate")
                {
                    srchName = "آخرین وصول";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.LastReceiveDate.ToShamsi().Contains(search)).ToList();
                }
                if (searchField == "noncount")
                {
                    srchName = "تعداد معوق";
                    nonePaymentBordroesDets = nonePaymentBordroesDets.Where(w => w.NonReceivedCount.ToString() == search).ToList();
                }
            }
            NonPaymentBordroesModel nonPaymentBordroesModel = new NonPaymentBordroesModel()
            {
                NonePaymentBordroesDets = nonePaymentBordroesDets.Skip((zpage-1)*zcount).Take( zcount).ToList(),
                TotalRecCount = nonePaymentBordroesDets.Count(),
                CurPage = zpage,
                RecCount = zcount,
                SearchFieldName = srchName,
                SearchText = search
            };
            if (nonePaymentBordroesDets.Count() % zcount == 0)
            {
                nonPaymentBordroesModel.TotalPages = nonePaymentBordroesDets.Count() / zcount;
            }
            else
            {
                nonPaymentBordroesModel.TotalPages = (nonePaymentBordroesDets.Count() / zcount) + 1;
            }
            return View(nonPaymentBordroesModel);
            
        }
        [PermissionChecker(19)]
        public async Task<IActionResult> LifeInsurances()
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);

            if (ActiveUserRole == null)
            {
                return View(null);
            }
            int recCount = 30;
            List<LifeBordroBase> All_lifeBordroBases = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, recCount, 1, true, string.Empty, 0);
            ReportLifeInsurancesVM reportLifeInsurancesVM = new ReportLifeInsurancesVM()
            {
                AllBordroes = All_lifeBordroBases,
                TotalRecCount = All_lifeBordroBases.Count(),
                CurPage = 1,
                RecCount = recCount,
                PageBordroes = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, recCount, 1, false, string.Empty, 0)
            };
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();


            if (All_lifeBordroBases.Count() % recCount == 0)
            {
                reportLifeInsurancesVM.TotalPages = All_lifeBordroBases.Count() / recCount;
            }
            else
            {
                reportLifeInsurancesVM.TotalPages = (All_lifeBordroBases.Count() / recCount) + 1;
            }

            return View(reportLifeInsurancesVM);
        }
        [HttpPost]
        [PermissionChecker(19)]
        public async Task<IActionResult> LifeInsurances(int? RecCount, int? page, string search, int IsDateRange)
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);

            if (ActiveUserRole == null)
            {
                return View(null);
            }
            ReportLifeInsurancesVM reportLifeInsurancesVM = new ReportLifeInsurancesVM();
            List<LifeBordroBase> All_lifeBordroBases = null;
            if (!string.IsNullOrEmpty(search))
            {
                int countL = search.Count(f => f == '-');
                int countSlash = search.Count(f => f == '/');
                if (IsDateRange == 1)
                {

                    if (countL > 1 || (countSlash != 4 && countSlash != 2))
                    {
                        reportLifeInsurancesVM.CurPage = page.GetValueOrDefault(1);
                        reportLifeInsurancesVM.RecCount = RecCount.GetValueOrDefault(30);
                        reportLifeInsurancesVM.SearchText = search;
                        ModelState.AddModelError("SearchText", "امکان جستجوی این محدوده تاریخ وجود ندارد !");
                        return View(reportLifeInsurancesVM);
                    }
                    All_lifeBordroBases = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, RecCount, page, true, search, 1);

                }
                else
                {
                    All_lifeBordroBases = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, RecCount, page, true, search, 0);
                }
            }
            else
            {
                All_lifeBordroBases = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, RecCount, page, true, search, 0);
            }




            All_lifeBordroBases = All_lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
            if (All_lifeBordroBases != null)
            {
                
                reportLifeInsurancesVM.AllBordroes = All_lifeBordroBases;
                reportLifeInsurancesVM.PageBordroes = All_lifeBordroBases.Skip((page.GetValueOrDefault(1) - 1) * RecCount.GetValueOrDefault(30)).Take(RecCount.GetValueOrDefault(30)).ToList();
                reportLifeInsurancesVM.TotalRecCount = All_lifeBordroBases.Count();
            }

            reportLifeInsurancesVM.CurPage = page.GetValueOrDefault(1);
            reportLifeInsurancesVM.RecCount = RecCount.GetValueOrDefault(30);
            //PageBordroes = await _bordroService.GetLifeBordroBasesBySellerNC(ActiveUserRole.User.NC, RecCount, page, false, search),

            reportLifeInsurancesVM.SearchText = search;
            if (All_lifeBordroBases != null)
            {
                if (All_lifeBordroBases.Count() % RecCount.GetValueOrDefault(30) == 0)
                {
                    reportLifeInsurancesVM.TotalPages = All_lifeBordroBases.Count() / RecCount.GetValueOrDefault(30);
                }
                else
                {
                    reportLifeInsurancesVM.TotalPages = (All_lifeBordroBases.Count() / RecCount.GetValueOrDefault(30)) + 1;
                }
            }

            return View(reportLifeInsurancesVM);
        }
        [PermissionChecker(19)]
        public async Task<IActionResult> LifeInsuranceDetails(Guid id, string retUrl)
        {
            LifeBordroBase lifeBordroBase = await _bordroService.GetLifeBordroBaseById(id);
            if (lifeBordroBase != null)
            {
                ViewData["retUrl"] = retUrl;
                return View(lifeBordroBase);

            }
            else
            {
                return NotFound();
            }

        }
        [PermissionChecker(20)]
        public async Task<IActionResult> GetOrgLifeInsurances()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 & w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            List<UserRole> AllChilds = new List<UserRole>();
            foreach (var item in userRoles)
            {
                List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                AllChilds.AddRange(Childs);
            }
            OrgLifeInsuranceModel orgLifeInsuranceModel = new OrgLifeInsuranceModel()
            {
                Organizations = AllChilds
            };
            return View(orgLifeInsuranceModel);
        }
        [PermissionChecker(20)]
        public async Task<IActionResult> OrgLifeInsurances()
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> bordroBases = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                if (bordroBases != null)
                {
                    if (bordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(bordroBases);
                    }

                }
            }
            lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
            ReportOrgLifeInsurancesVM reportOrgLifeInsurancesVM = new ReportOrgLifeInsurancesVM()
            {
                AllBordroes = lifeBordroBases,
                PageBordroes = lifeBordroBases.Take(30).ToList(),
                CurPage = 1,
                TotalRecCount = lifeBordroBases.Count(),
                RecCount = 30,

            };
            if (lifeBordroBases.Count() % 30 == 0)
            {
                reportOrgLifeInsurancesVM.TotalPages = lifeBordroBases.Count / 30;
            }
            else
            {
                reportOrgLifeInsurancesVM.TotalPages = (lifeBordroBases.Count / 30) + 1;
            }
            return View(reportOrgLifeInsurancesVM);

        }
        [HttpPost]
        [PermissionChecker(20)]
        public async Task<IActionResult> OrgLifeInsurances(int? RecCount, int? page, string search, int IsDateRange)
        {
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            User user = await _userService.GetUserByCode(User.Identity.Name);
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> bordroBases = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                if (bordroBases != null)
                {
                    if (bordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(bordroBases);
                    }
                }
            }
            lifeBordroBases = lifeBordroBases.OrderByDescending(r => r.IssueDate).ThenByDescending(r => r.InsNO).ToList();
            ReportOrgLifeInsurancesVM reportOrgLifeInsurancesVM = new ReportOrgLifeInsurancesVM()
            {
                AllBordroes = lifeBordroBases,
                CurPage = page.GetValueOrDefault(1),

                RecCount = RecCount.GetValueOrDefault(30),
                SearchText = search
            };
            if (!string.IsNullOrEmpty(search))
            {

                if (IsDateRange == 0)
                {

                    lifeBordroBases = lifeBordroBases.Where(w => w.InsNO.Contains(search) ||
                            w.IssueDate.ToShamsi().Contains(search) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsurerFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.InsuredFullName.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.PaymentMethod.Contains(search)) ||
                            w.LifeBordroAdditions.Any(a => a.IsActive == true && a.Seller.Contains(search))

                    ).ToList();


                }
                else
                {
                    if (IsDateRange == 1)
                    {
                        if (search.Count(f => f == '-') == 1)
                        {
                            string[] srch = search.Split("-");

                            string sdate = srch[0].Trim();
                            string edate = srch[1].Trim();
                            string[] svdate = sdate.Split("/");
                            string[] evdate = edate.Split("/");
                            if ((svdate.Length > 1 && svdate.Length <= 3) && (evdate.Length > 1 && evdate.Length <= 3))
                            {
                                DateTime MSdate = sdate.GetMiladiDateWithoutTime(false);

                                DateTime MEDate = edate.GetMiladiDateWithoutTime(true);
                                lifeBordroBases = lifeBordroBases.Where(w => w.IssueDate >= MSdate && w.IssueDate <= MEDate).ToList();
                            }
                            else
                            {
                                ModelState.AddModelError("SearchText", "امکان جستجوی این محدوده تاریخ وجود ندارد !");
                                return View(reportOrgLifeInsurancesVM);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("SearchText", "امکان جستجوی این محدوده تاریخ وجود ندارد !");
                            return View(reportOrgLifeInsurancesVM);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SearchText", "امکان جستجوی این محدوده تاریخ وجود ندارد !");
                        return View(reportOrgLifeInsurancesVM);
                    }





                }

            }
            reportOrgLifeInsurancesVM.TotalRecCount = lifeBordroBases.Count();
            reportOrgLifeInsurancesVM.PageBordroes = lifeBordroBases.Skip((page.GetValueOrDefault(1) - 1) * RecCount.GetValueOrDefault(30)).Take(RecCount.GetValueOrDefault(30)).ToList();
            reportOrgLifeInsurancesVM.AllBordroes = lifeBordroBases;
            if (lifeBordroBases.Count() % RecCount.GetValueOrDefault(30) == 0)
            {
                reportOrgLifeInsurancesVM.TotalPages = lifeBordroBases.Count / RecCount.GetValueOrDefault(30);
            }
            else
            {
                reportOrgLifeInsurancesVM.TotalPages = (lifeBordroBases.Count / RecCount.GetValueOrDefault(30)) + 1;
            }
            return View(reportOrgLifeInsurancesVM);
        }
        /// <summary>
        /// صفحه گزارش کارمزد های شخصی
        /// </summary>
        /// <returns></returns>
        [PermissionChecker(25)]
        public async Task<IActionResult> PCommissionsPage()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return NotFound("کاربری موجود نمی باشد !");
            }
            PersianCalendar PC = new PersianCalendar();
            int sY = PC.GetYear(user.RegDate);
            PCommissionsReport pCommissionsReport = new PCommissionsReport()
            {
                FullName = user.FullName,
                StartYear = sY,
                CurrentYear = PC.GetYear(DateTime.Now)

            };
            return View(pCommissionsReport);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(25)]
        public async Task<IActionResult> PCommissionsPage(PCommissionsReport pCommissionsReport)
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);

            if (!ModelState.IsValid)
            {

                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                pCommissionsReport.FullName = user.FullName;
                pCommissionsReport.StartYear = sY;
                pCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                return View(pCommissionsReport);
            }

            pCommissionsReport.CommissionBase = await _bordroService.GetCommissionBaseByYearMounthAsync((int)pCommissionsReport.Year, (int)pCommissionsReport.Mounth);
            pCommissionsReport.Commissions = await _bordroService.GetCommissionsBySellerNC_Year_Mounth(user.NC, (int)pCommissionsReport.Year, (int)pCommissionsReport.Mounth);
            double value = 0;
            if (pCommissionsReport.Commissions != null)
            {
                foreach (var item in pCommissionsReport.Commissions)
                {
                    List<SalesObject> salesObjects = await _bordroService.GetSalesObjectsofBordroAsync(item.LifeBordroBase.Id);
                    float per = salesObjects.FirstOrDefault(w => w.UserRole.User == user).SPercent;

                    float NewPer = 0;
                    NewPer = (per / item.Percent) * (item.Percent / 30);
                    value += Math.Round(NewPer * (item.LifeCommission + item.SupCommission), 0);
                }
            }

            pCommissionsReport.CommissionSum = value;
            if (pCommissionsReport.CommissionBase == null)
            {
                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                pCommissionsReport.FullName = user.FullName;
                pCommissionsReport.StartYear = sY;
                pCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                pCommissionsReport.Message = "گزارش کارمزدی برای ماه " + " " + pCommissionsReport.Mounth + " از سال " + pCommissionsReport.Year + "وجود ندارد !";
                return View(pCommissionsReport);
            }
            if (pCommissionsReport.Commissions == null)
            {
                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                pCommissionsReport.FullName = user.FullName;
                pCommissionsReport.StartYear = sY;
                pCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                pCommissionsReport.Message = "گزارش کارمزدی برای ماه " + " " + pCommissionsReport.Mounth + " از سال " + pCommissionsReport.Year + "وجود ندارد !";
                return View(pCommissionsReport);
            }
            else
            {
                if (pCommissionsReport.Commissions.Count() == 0)
                {
                    PersianCalendar PC = new PersianCalendar();
                    int sY = PC.GetYear(user.RegDate);
                    pCommissionsReport.FullName = user.FullName;
                    pCommissionsReport.StartYear = sY;
                    pCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                    pCommissionsReport.Message = "گزارش کارمزدی برای ماه " + " " + pCommissionsReport.Mounth + " از سال " + pCommissionsReport.Year + "وجود ندارد !";
                    return View(pCommissionsReport);
                }
            }
            return View(pCommissionsReport);
        }
        [PermissionChecker(25)]
        public async Task<IActionResult> PCommissionDetails(int cId)
        {
            Commission commission = await _bordroService.GetCommissionByIdAsync(cId);
            return View(commission);
        }
        [PermissionChecker(26)]
        public async Task<IActionResult> SOrgCommissionsPage()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 & w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            if (user == null)
            {
                return NotFound("کاربری موجود نمی باشد !");
            }
            PersianCalendar PC = new PersianCalendar();
            int sY = PC.GetYear(user.RegDate);
            SOrgCommissionsReport sOrgCommissionsReport = new SOrgCommissionsReport()
            {
                FullName = user.FullName,
                StartYear = sY,
                CurrentYear = PC.GetYear(DateTime.Now)
            };
            List<UserRole> AllChilds = new List<UserRole>();
            foreach (var item in userRoles)
            {
                AllChilds.Add(item);
            }
            foreach (var item in userRoles)
            {
                List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                AllChilds.AddRange(Childs);
            }

            if (AllChilds.Count() != 0)
            {
                sOrgCommissionsReport.UserRoles = AllChilds;
            }

            return View(sOrgCommissionsReport);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(26)]
        public async Task<IActionResult> SOrgCommissionsPage(SOrgCommissionsReport sOrgCommissionsReport)
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            if (!ModelState.IsValid)
            {
                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                sOrgCommissionsReport.FullName = user.FullName;
                sOrgCommissionsReport.StartYear = sY;
                sOrgCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                List<UserRole> AllChilds = new List<UserRole>();
                foreach (var item in userRoles)
                {
                    List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                    AllChilds.AddRange(Childs);
                }
                if (AllChilds.Count() != 0)
                {
                    sOrgCommissionsReport.UserRoles = AllChilds;
                }

                return View(sOrgCommissionsReport);
            }
            CommissionBase commissionb = await _bordroService.GetCommissionBaseByYearMounthAsync((int)sOrgCommissionsReport.Year, (int)sOrgCommissionsReport.Mounth);
            if (commissionb == null)
            {
                sOrgCommissionsReport.Message = "گزارش کارمزدی برای ماه " + " " + sOrgCommissionsReport.Mounth + " از سال " + sOrgCommissionsReport.Year + "وجود ندارد !";
                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                sOrgCommissionsReport.FullName = user.FullName;
                sOrgCommissionsReport.StartYear = sY;
                sOrgCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                List<UserRole> AllChilds = new List<UserRole>();
                foreach (var item in userRoles)
                {
                    List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                    AllChilds.AddRange(Childs);
                }
                if (AllChilds.Count() != 0)
                {
                    sOrgCommissionsReport.UserRoles = AllChilds;
                }

                return View(sOrgCommissionsReport);
            }
            sOrgCommissionsReport.OrgUserComVMs = await _bordroService.GetOrgCommissionsAsync(sOrgCommissionsReport.OrgUrIds, User.Identity.Name, (int)sOrgCommissionsReport.Year, (int)sOrgCommissionsReport.Mounth);

            return View(sOrgCommissionsReport);
        }
        [PermissionChecker(23)]
        public async Task<IActionResult> Insureds()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<InsuredInfoReportModel> insuredInfoReportModels = await _bordroService.PrepareInseredInfoReportModelAsync(user.NC);
           
            if (user == null)
            {
                return NotFound();
            }
            InsurancePoliciesVM insurancePoliciesVM = new InsurancePoliciesVM()
            {
                CurPage = 1,
                RecCount = 30,              

            };
            insurancePoliciesVM.InsuredInfoReportModels = insuredInfoReportModels;            
            insurancePoliciesVM.TotalRecCount = insuredInfoReportModels.Count();
            insurancePoliciesVM.PageinsuredInfoReportModels = insuredInfoReportModels.Take(30).ToList();


            if (insuredInfoReportModels.Count() % 30 == 0)
            {
                insurancePoliciesVM.TotalPages = insuredInfoReportModels.Count() / 30;
            }
            else
            {
                insurancePoliciesVM.TotalPages = (insuredInfoReportModels.Count() / 30) + 1;
            }
            //insurancePoliciesVM.AllBordroBases.AddRange(All.ToList());

            insurancePoliciesVM.User = user;
            return View(insurancePoliciesVM);
        }
        [HttpPost]
        [PermissionChecker(23)]
        public async Task<IActionResult> Insureds(int? RecCount, int? page, string search, string searchField)
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<InsuredInfoReportModel> insuredInfoReportModels = await _bordroService.PrepareInseredInfoReportModelAsync(user.NC);
            insuredInfoReportModels = insuredInfoReportModels.ToList();
            if (user == null)
            {
                return NotFound();
            }
            page = page.GetValueOrDefault(1);
            InsurancePoliciesVM insurancePoliciesVM = new InsurancePoliciesVM()
            {
                CurPage = page,
                RecCount = RecCount.GetValueOrDefault(30),
                SearchText = search 
            };
           
            
            if (!string.IsNullOrEmpty(search))
            {
                if(string.IsNullOrEmpty(searchField))
                {
                    insuredInfoReportModels = insuredInfoReportModels.Where(w => w.InsNO.Contains(search) || w.IssueDate.Contains(search) ||
                            EF.Functions.Like(w.InsuredNC, "%" + search + "%") ||
                            EF.Functions.Like(w.InsuredFullName, search) ||
                            EF.Functions.Like(w.InsurerFullName, "%" + search + "%") ||
                            w.InsuredBirthDate.Contains(search) ||
                            w.PaymentMethod.Contains(search) ||
                            w.PaymentMethod.Contains(search) ||
                            w.InsNO.Contains(search) ||
                            w.Duration == search || 
                            w.IssueDate.Contains(search) || 
                            w.State.Contains(search) ||
                            w.City.Contains(search)
                            
                            ).ToList();
                }
                else
                {
                    if(searchField == "all")
                    {
                        insurancePoliciesVM.SearchField = "تمام ستونها";
                        insuredInfoReportModels = insuredInfoReportModels.ToList().Where(w => w.InsNO.Contains(search) || w.IssueDate.Contains(search) ||
                           w.InsuredNC.Contains(search) ||
                           w.InsuredFullNameList.Any(a => a == search) ||
                           w.InsurerFullNameList.Any(a => a == search) ||                            
                           w.InsuredBirthDate.Contains(search) ||
                           w.PaymentMethod.Contains(search) ||                           
                           w.InsNO.Contains(search) ||
                           w.Duration == search ||
                           w.IssueDate.Contains(search) ||
                           w.State.Contains(search) ||
                           w.City.Contains(search) || 
                           w.SellerFullNameList.Any(a => a == search) ||
                           w.Address.Contains(search)
                           ).ToList();
                    }
                    if(searchField=="insuredfullname")
                    {
                        insurancePoliciesVM.SearchField = "نام کامل بیمه شده";
                        insuredInfoReportModels = insuredInfoReportModels.ToList().Where(w => w.InsuredFullNameList.Any(a => a == search)).ToList();
                    }
                    if (searchField == "insurednc")
                    {
                        insurancePoliciesVM.SearchField = "کد ملی بیمه شده";
                        insuredInfoReportModels = insuredInfoReportModels.ToList().Where(w => w.InsuredNC.Contains(search)).ToList();
                    }
                    if (searchField == "insuredbirthdate")
                    {
                        insurancePoliciesVM.SearchField = "تاریخ تولد بیمه شده";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.InsuredBirthDate.Contains(search)).ToList();
                    }
                    if (searchField == "insno")
                    {
                        insurancePoliciesVM.SearchField = "شماره بیمه نامه";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.InsNO.Contains(search)).ToList();
                    }
                    if (searchField == "insurerfullname")
                    {
                        insurancePoliciesVM.SearchField = "نام کامل بیمه گذار";
                        insuredInfoReportModels = insuredInfoReportModels.ToList().Where(w => w.InsurerFullNameList.Any(a => a == search)).ToList();
                    }
                    if (searchField == "issuedate")
                    {
                        insurancePoliciesVM.SearchField = "تاریخ صدور";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.IssueDate.Contains(search)).ToList();
                    }
                    if (searchField == "paymethod")
                    {
                        insurancePoliciesVM.SearchField = "روش پرداخت";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.PaymentMethod.Contains(search)).ToList();
                    }
                    if (searchField == "state")
                    {
                        insurancePoliciesVM.SearchField = "استان";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.State.Contains(search)).ToList();
                    }
                    if (searchField == "city")
                    {
                        insurancePoliciesVM.SearchField = "شهر";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.City.Contains(search)).ToList();
                    }
                    if(searchField == "agent")
                    {
                        insurancePoliciesVM.SearchField = "نماینده";
                        insuredInfoReportModels = insuredInfoReportModels.Where(w => w.SellerFullNameList.Any(a => a == search)).ToList();
                    }
                    
                }
                
            }

            insurancePoliciesVM.TotalRecCount = insuredInfoReportModels.Count();
            insurancePoliciesVM.PageinsuredInfoReportModels = insuredInfoReportModels.Skip((int)((page-1)*RecCount)).Take(RecCount.GetValueOrDefault(30)).ToList();


            if (insuredInfoReportModels.Count() % RecCount.GetValueOrDefault(30) == 0)
            {
                insurancePoliciesVM.TotalPages = insuredInfoReportModels.Count() / RecCount.GetValueOrDefault(30);
            }
            else
            {
                insurancePoliciesVM.TotalPages = (insuredInfoReportModels.Count() / RecCount.GetValueOrDefault(30)) + 1;
            }
            

            insurancePoliciesVM.User = user;
            return View(insurancePoliciesVM);
        }
        [PermissionChecker(23)]
        public async Task<IActionResult> InsuredInfoDetails(string InsNo)
        {
            string htmlMess = string.Empty;
            (string, InsuredInfoReportModel) model = (string.Empty,null);
            
            if (string.IsNullOrEmpty(InsNo))
            {
                return NotFound("بیمه شده ای یافت نشد !");
            }
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<InsuredInfoReportModel> insuredInfoReportModels = await _bordroService.PrepareInseredInfoReportModelAsync(user.NC);
            insuredInfoReportModels = insuredInfoReportModels.ToList();
            if(!insuredInfoReportModels.Any(a => a.InsNO == InsNo))
            {
               
                model.Item1 = "مجاز به مشاهده اطلاعات این بیمه شده نمی باشید !";
                model.Item2 = null;
                return View(model);
            }
             model.Item2 =  insuredInfoReportModels.FirstOrDefault(f => f.InsNO == InsNo);
            return View(model);
        }
        [PermissionChecker(29)]
        public async Task<IActionResult> PoolRewardReport()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 & w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            if (user == null)
            {
                return NotFound("کاربری موجود نمی باشد !");
            }
            PersianCalendar PC = new PersianCalendar();
            int sY = PC.GetYear(user.RegDate);
            PoolRewardVM poolRewardVM = new PoolRewardVM()
            {
                FullName = user.FullName,
                StartYear = sY,
                CurrentYear = PC.GetYear(DateTime.Now)
            };
            return View(poolRewardVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(29)]
        public async Task<IActionResult> PoolRewardReport(PoolRewardVM poolRewardVM)
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (!ModelState.IsValid)
            {

                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                poolRewardVM.FullName = user.FullName;
                poolRewardVM.StartYear = sY;
                poolRewardVM.CurrentYear = PC.GetYear(DateTime.Now);

                return View(poolRewardVM);
            }
            CommissionBase commissionBase = await _bordroService.GetCommissionBaseByYearMounthAsync((int)poolRewardVM.Year, (int)poolRewardVM.Mounth);
            if (commissionBase == null)
            {
                PersianCalendar PC = new PersianCalendar();
                int sY = PC.GetYear(user.RegDate);
                poolRewardVM.FullName = user.FullName;
                poolRewardVM.StartYear = sY;
                poolRewardVM.CurrentYear = PC.GetYear(DateTime.Now);
                poolRewardVM.Message = "پاداش بهره وری برای ماه " + poolRewardVM.Mounth + " از سال " + poolRewardVM.Year + " قابل محاسبه نیست !";
                return View(poolRewardVM);
            }
            // Get Pools users Info
            PoolRewardReportResultVM poolRewardReportResultVM = await _bordroService.GetUserPoolRewardAsync(null, poolRewardVM.Year, poolRewardVM.Mounth);
            poolRewardReportResultVM.CommissionBase = commissionBase;
            poolRewardVM.CommissionBase = commissionBase;

            poolRewardVM.IsShow = true;
            poolRewardVM.rolePools = await _userService.GetRolePoolsAsync();

            if (user.Code == "290070")
            {
                poolRewardVM.PoolRewardReportResultVM = await _bordroService.GetUserPoolRewardAsync(null, poolRewardVM.Year, poolRewardVM.Mounth);
            }
            else
            {
                poolRewardVM.PoolRewardReportResultVM = await _bordroService.GetUserPoolRewardAsync(user.Id, poolRewardVM.Year, poolRewardVM.Mounth);
            }
            poolRewardVM.poolRewardReport_TotalVMs = _bordroService.GetTotalPoolRewardReport(poolRewardReportResultVM);

            return View(poolRewardVM);
        }
        [PermissionChecker(28)]
        public async Task<IActionResult> CommissionsList()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 & w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            if (user == null)
            {
                return NotFound("کاربری موجود نمی باشد !");
            }
            PersianCalendar PC = new PersianCalendar();
            int sY = PC.GetYear(user.RegDate);
            SOrgCommissionsReport sOrgCommissionsReport = new SOrgCommissionsReport()
            {
                FullName = user.FullName,
                StartYear = sY,
                CurrentYear = PC.GetYear(DateTime.Now)
            };
            List<UserRole> AllChilds = new List<UserRole>();
            foreach (var item in userRoles)
            {
                AllChilds.Add(item);
            }
            foreach (var item in userRoles)
            {
                List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                AllChilds.AddRange(Childs);
            }

            if (AllChilds.Count() != 0)
            {
                sOrgCommissionsReport.UserRoles = AllChilds;
            }

            return View(sOrgCommissionsReport);
        }
        [HttpPost]
        [PermissionChecker(28)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CommissionsList(SOrgCommissionsReport sOrgCommissionsReport)
        {
           
            List<CommissionListModel> commissionListModels = null;
            if (string.IsNullOrEmpty(sOrgCommissionsReport.ComListJsonString))
            {
                User user = await _userService.GetUserByUserName(User.Identity.Name);
                List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
                userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
                UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
                if (!ModelState.IsValid)
                {
                    PersianCalendar PC = new PersianCalendar();
                    int sY = PC.GetYear(user.RegDate);
                    sOrgCommissionsReport.FullName = user.FullName;
                    sOrgCommissionsReport.StartYear = sY;
                    sOrgCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                    List<UserRole> AllChilds = new List<UserRole>();
                    foreach (var item in userRoles)
                    {
                        List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                        AllChilds.AddRange(Childs);
                    }
                    if (AllChilds.Count() != 0)
                    {
                        sOrgCommissionsReport.UserRoles = AllChilds;
                    }

                    return View(sOrgCommissionsReport);
                }
                CommissionBase commissionb = await _bordroService.GetCommissionBaseByYearMounthAsync((int)sOrgCommissionsReport.Year, (int)sOrgCommissionsReport.Mounth);
                if (commissionb == null)
                {
                    sOrgCommissionsReport.Message = "گزارش کارمزدی برای ماه " + " " + sOrgCommissionsReport.Mounth + " از سال " + sOrgCommissionsReport.Year + "وجود ندارد !";
                    PersianCalendar PC = new PersianCalendar();
                    int sY = PC.GetYear(user.RegDate);
                    sOrgCommissionsReport.FullName = user.FullName;
                    sOrgCommissionsReport.StartYear = sY;
                    sOrgCommissionsReport.CurrentYear = PC.GetYear(DateTime.Now);
                    List<UserRole> AllChilds = new List<UserRole>();
                    foreach (var item in userRoles)
                    {
                        List<UserRole> Childs = _userService.GetAllChilds(item.URId).Select(s => s.UserRole).ToList();
                        AllChilds.AddRange(Childs);
                    }
                    if (AllChilds.Count() != 0)
                    {
                        sOrgCommissionsReport.UserRoles = AllChilds;
                    }

                    return View(sOrgCommissionsReport);
                }

                sOrgCommissionsReport.SystemCommissionVMs = await _bordroService.GetSystemCommissonsAsync(sOrgCommissionsReport.OrgUrIds, (int)sOrgCommissionsReport.Year, (int)sOrgCommissionsReport.Mounth);
                //PoolRewardReportResultVM poolRewardReportResultVM = await _bordroService.GetUserPoolRewardAsync(null, (int)sOrgCommissionsReport.Year, (int)sOrgCommissionsReport.Mounth);
                //poolRewardReportResultVM.CommissionBase = commissionb;

                //sOrgCommissionsReport.PoolRewardReport_TotalVMs = _bordroService.GetTotalPoolRewardReport(poolRewardReportResultVM);
                if (string.IsNullOrEmpty(sOrgCommissionsReport.ComListJsonString))
                {
                     commissionListModels = sOrgCommissionsReport.SystemCommissionVMs.OrderByDescending(r => int.Parse(r.User.Code)).GroupBy(g => g.User)
                        .Select(cl => new CommissionListModel
                        {
                            FullName = cl.Key.FullName,
                            BankAccount = cl.Key.BankAccountNumber,
                            Title = cl.FirstOrDefault().Title,
                            Code = cl.Key.Code,
                            PersonalCommAll = cl.Sum(s => s.PersonalCommissionsTotal),
                            OrgCommAll = cl.Sum(s => s.OrgCommissionsTotal),
                            EqRewAll = cl.Sum(s => s.EqulityRewardTotal),
                            PoolRewAll = cl.Sum(s => s.PoolRewardTotal),
                            RowCommSum = cl.Sum(s => s.PersonalCommissionsTotal + s.OrgCommissionsTotal + s.EqulityRewardTotal + s.PoolRewardTotal),

                        }
                        ).ToList();
                    sOrgCommissionsReport.ComListJsonString = JsonConvert.SerializeObject(commissionListModels);
                    sOrgCommissionsReport.OrgUserRolesJsonString = JsonConvert.SerializeObject(sOrgCommissionsReport.UserRoles);

                }
                //return View(sOrgCommissionsReport);
            }
            else
            {
                commissionListModels = JsonConvert.DeserializeObject<List<CommissionListModel>>(sOrgCommissionsReport.ComListJsonString);
                sOrgCommissionsReport.UserRoles = JsonConvert.DeserializeObject<List<UserRole>>(sOrgCommissionsReport.OrgUserRolesJsonString);
            }
            if(!string.IsNullOrEmpty(sOrgCommissionsReport.search))
            {
                commissionListModels = commissionListModels.Where(w => w.FullName.Contains(sOrgCommissionsReport.search) ||
                w.Title.Contains(sOrgCommissionsReport.search) ||
                w.Code.Equals(sOrgCommissionsReport.search.Trim())
                ).ToList();
            }
            sOrgCommissionsReport.TotalRec = commissionListModels.Count();
            int page = sOrgCommissionsReport.CurPage.GetValueOrDefault(1);
            int count = sOrgCommissionsReport.RecCount.GetValueOrDefault(100);
            sOrgCommissionsReport.CurPage = page;
            sOrgCommissionsReport.RecCount = count;
            sOrgCommissionsReport.AllTotalPersonalCom = commissionListModels.Sum(x => x.PersonalCommAll);
            sOrgCommissionsReport.AllTotalOrgCom = commissionListModels.Sum(x => x.OrgCommAll);
            sOrgCommissionsReport.AllTotalEqRew = commissionListModels.Sum(x => x.EqRewAll);
            sOrgCommissionsReport.AllTotalPoolRew = commissionListModels.Sum(x => x.PoolRewAll);
            string data = string.Empty;
            foreach (var item in commissionListModels.Where(w => !string.IsNullOrEmpty(w.BankAccount) && w.RowCommSum != 0))
            {
                if (string.IsNullOrEmpty(data))
                {
                    data = item.BankAccount + "," + item.RowCommSum + "," + "خیابان فاطمی - کارمزد ماه" + sOrgCommissionsReport.Mounth + " سال " + sOrgCommissionsReport.Year + " " + item.FullName;
                }
                else
                {
                    data += Environment.NewLine + item.BankAccount + "," + item.RowCommSum + "," + "خیابان فاطمی - " + " کارمزد ماه " + sOrgCommissionsReport.Mounth + " سال " + sOrgCommissionsReport.Year + " " + item.FullName;
                }
            }
            sOrgCommissionsReport.TextFileString = data;
            //_bordroService.CreateTextFile("Com" + DateTime.Now.ToShamsi(),  data);
            sOrgCommissionsReport.CommissionListModels = commissionListModels.Skip((page - 1) * count).Take(count).ToList();
            return View(sOrgCommissionsReport);
        }
        [HttpPost]
        public string Createtxt(string StrData)
        {
            PersianCalendar pc = new PersianCalendar();
            string FName = "ComList-" + "y" + pc.GetYear(DateTime.Now).ToString() + "m" + pc.GetMonth(DateTime.Now).ToString() + "d" + pc.GetDayOfMonth(DateTime.Now).ToString() + "h" + pc.GetHour(DateTime.Now).ToString() + "m" + pc.GetMinute(DateTime.Now).ToString() + "s" + pc.GetSecond(DateTime.Now).ToString();
            bool res = _bordroService.CreateTextFile(FName, StrData);
            if (res == true)
            {
                return "فایل با نام " + FName + " ثبت شد ";
            }
            else
            {
                return "امکان ثبت فایل وجود ندارد !";
            }
        }
        [HttpPost]
        public async Task<IActionResult> WritePersonalBordroExcelFile()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return null;
            }
            List<ExcelBordroModel> ExcelBordroModels = await _bordroService.GetPersonalExcelBordroModelsByNCAsync(user.NC);
            string tit = string.Empty;
            if(User.Identity.Name !="290070")
            {
                tit = "بردروی شخصی " + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            }
            else
            {
                 tit = "بردروهای " + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            }
           
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new ExcelBordroModel(), ExcelBordroModels,tit);
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
                    fileDownloadName: "PersonalBordro " + user.Code + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WriteOrgBordroExcelFile()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<ExcelBordroModel> ExcelBordroModels = await _bordroService.GetOrgExcelBordroModelsByNCAsync(user.NC);
            string tit = "بردرو سازمانی " + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new ExcelBordroModel(), ExcelBordroModels,tit);
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
                    fileDownloadName: "OrgBordro " + user.Code + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public IActionResult WriteComListExcelFile(string ComListjsonString)
        {
            
            List<CommissionListModel> commissionLists = JsonConvert.DeserializeObject<List<CommissionListModel>>(ComListjsonString);
            string tit = "لیست کارمزد" + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new CommissionListModel(), commissionLists,tit);
            string contentType = ""; // Scope

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
                    fileDownloadName: "CommissionList " + User.Identity.Name + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WritePersonalCommissionsExcelFile(int Year, int Mounth)
        {
            User Login = await _userService.GetUserByUserName(User.Identity.Name);
            List<Commission> personalCommissions = await _bordroService.GetCommissionsBySellerNC_Year_Mounth(Login.NC, Year, Mounth);
            List<PersonalCommissionReportModel> personalCommissionReportModels = await _bordroService.CovertCommissions_To_PersonalCommissionReportModelAsync(personalCommissions, Year, Mounth);
            string tit = "کارمزد فروش شخصی" + " " + Login.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new PersonalCommissionReportModel(), personalCommissionReportModels.ToList(),tit);
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
                    fileDownloadName: "PersonalCommissions " + User.Identity.Name + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WriteSOrgCommissionsExcelFile(List<int> OrgUrIds, int Year, int Mounth)
        {
            User user = await _userService.GetUserByCode(User.Identity.Name);
            List<OrgUserComVM> orgUserComVMs = await _bordroService.GetOrgCommissionsAsync(OrgUrIds, User.Identity.Name, Year, Mounth);
            List<OrgCommissionReportModel> orgCommissionReportModels = await _bordroService.ConvertOrgUserComVM_To_OrgCommissionReportModelAsync(orgUserComVMs, Year, Mounth);
            string tit = "کارمزد فروش سازمانی" + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new OrgCommissionReportModel(), orgCommissionReportModels,tit);
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
                    fileDownloadName: "OrgCommissions " + User.Identity.Name + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WriteInsuredInfoExcelFile()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<InsuredInfoReportModel> insuredInfoReportModels = await _bordroService.PrepareInseredInfoReportModelAsync(user.NC);
            string tit = "اطلاعات بیمه شدگان" + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new InsuredInfoReportModel(), insuredInfoReportModels,tit);
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
                    fileDownloadName: "InsuredInfo " + user.Code + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WriteLifeAdditionsExcelFile()
        {
            User user = await _userService.GetUserByUserName(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            List<UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            userRoles = userRoles.Where(w => w.RoleId != 2 && w.RoleId != 3).ToList();
            UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive == true);
            List<LifeBordroBase> lifeBordroBases = new List<LifeBordroBase>();
            string sfName = string.Empty;
            foreach (var item in userRoles)
            {
                List<LifeBordroBase> IndbordroBases = await _bordroService.GetIndirectBordroBasebyurId(item.URId);
                IndbordroBases = IndbordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                List<LifeBordroBase> DirbordroBase = await _bordroService.GetDirectBordroBasebyNC(item.User.NC);
                DirbordroBase = DirbordroBase.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).ToList();
                if (DirbordroBase != null)
                {
                    lifeBordroBases.AddRange(DirbordroBase);
                }
                if (IndbordroBases != null)
                {
                    if (IndbordroBases.Count() != 0)
                    {
                        lifeBordroBases.AddRange(IndbordroBases);
                    }

                }

                lifeBordroBases = lifeBordroBases.Where(w => w.LifeBordroAdditions.Any(a => a.Number > 0)).Distinct().ToList();              
               
            }
            List<AdditionsReportModel> additionsReportModels = lifeBordroBases.ToList().Select(x => new AdditionsReportModel
            {
                InsNO = x.InsNO,
                Number = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Number.ToString(),
                IssueDate = x.IssueDate.ToShamsi(),
                InsuredFullName = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsuredFullName,
                InsuredNC = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsuredNC,
                InsurerFullName = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsurerFullName,
                InsurerNC = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InsurerNC,
                InitialStartDate = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).InitialStartDate.ToShamsi(),
                StartDate = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).StartDate.ToShamsiN(),
                Seller = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Seller,
                Status = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Status,
                AdditionType = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Type,
                PaymentMethod = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PaymentMethod,
                PaymentMethodValue = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).PremiumbyPaymentMethod,
                Duration = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Duration.ToString(),
                Deposit = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).Deposit.ToString(),
                LifeInsCapital = x.LifeBordroAdditions.FirstOrDefault(f => f.IsActive).CapitalDied.ToString()

            }).ToList();
            string tit = "الحاقیه های " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new AdditionsReportModel(), additionsReportModels,tit);
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
                    fileDownloadName: "LifeAdditions " + user.Code + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        public async Task<IActionResult> WriteNonePayBordroesExcelFile()
        {
            User user =await _userService.GetUserByCode(User.Identity.Name);
            List<NonePaymentBordroesDet> nonePaymentBordroesDets = await _bordroService.GetNonPaidBordroesAsync(User.Identity.Name);
            List<NonPaymentBordroesExcelModel> nonPaymentBordroesExcelModels = nonePaymentBordroesDets.Select(x => new NonPaymentBordroesExcelModel
            {
                InsNO = x.InsNO,
                Insurer = x.Insurer,
                Insured=x.Insured,
                InsuredPhone = x.InsuredPhone,
                PayMethod = x.PaymentMethod,
                PayMethodValue = x.PaymentMethodValue,
                Deposite = x.Deposit,
                IssueDate = x.IssueDate.ToShamsi(),
                Seller = x.Seller,
                Status = x.Status,
                Type = x.Type,
                LastReceiveDate = x.LastReceiveDate.ToShamsi(),
                TotalPremiumReceived = x.TotalPremiumReceived,
                NonReceivedCount=x.NonReceivedCount

            }).ToList();
            string tit = "گزارش عدم وصول" + " " + user.FullName + " | " + DateTime.Now.ToShamsiWithTime();
            IWorkbook workbook = _bordroService.WriteExcelWithNPOI(new NonPaymentBordroesExcelModel(), nonPaymentBordroesExcelModels,tit);
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
                    fileDownloadName: "NonePayBordroes " + User.Identity.Name + "-" + DateTime.Now.ToShamsi() + ((workbook.GetType() == typeof(XSSFWorkbook)) ? ".xlsx" : "xls"));
            }
            finally
            {
                if (tempStream != null) tempStream.Dispose();
                if (stream != null) stream.Dispose();
            }
        }
        [PermissionChecker(30)]
        public IActionResult TxtFilesList()
        {
            //string[] files = Directory.GetFiles("wwwroot/txtFiles");

            DirectoryInfo d = new DirectoryInfo(@"wwwroot/txtFiles");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            return View(Files);
        }


        public IActionResult DownloadFile(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/txtFiles", fileName);
            var net = new System.Net.WebClient();
            var data = net.DownloadData(filePath);
            var content = new MemoryStream(data);
            //var contentType = "APPLICATION/octet-stream";
            var contentType = "text/plain";


            return File(content, contentType, fileName);
        }




    }
}
