using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Convertors;
using Core.DTOs.Admin;
using Core.DTOs.General;
using Core.Security;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UploadController : Controller
    {
        
        private readonly IBordroService _bordroService;
        
        public UploadController(IBordroService bordroService)
        {            
            _bordroService = bordroService;           
        }
        [PermissionChecker(52)]
        public async Task<IActionResult> Index()
        {
            return View(await _bordroService.GetUploadInfosAsync());
        }
        [PermissionChecker(51)]
        public IActionResult Create()
        {
            UploadViewModel uploadViewModel = new UploadViewModel()
            {
                ValidationStep1 = false
            };
            return PartialView(uploadViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(51)]
        public async Task<IActionResult> Create(UploadViewModel uploadViewModel, IFormFile File)
        {

            if (!ModelState.IsValid)
            {
                uploadViewModel.ValidationStep1 = false;
                return View(uploadViewModel);
            }

            if (uploadViewModel.Type == "bordro")
            {
                if (uploadViewModel.ValidationStep1 == false)
                {
                    if (await _bordroService.ExistBordroABaseByYearMounthAsync(uploadViewModel.Year, uploadViewModel.Mounth))
                    {
                        // ModelState.AddModelError("Mounth", "اطلاعات دوره زمانی " + uploadViewModel.Mounth + " " + uploadViewModel.Year + " موجود است !");
                        uploadViewModel.Message = "اطلاعات دوره زمانی " + uploadViewModel.Mounth + " " + uploadViewModel.Year + " موجود است !";
                        uploadViewModel.ValidationStep1 = true;
                        uploadViewModel.ExistDuration = true;
                        return View(uploadViewModel);
                    }
                    uploadViewModel.ValidationStep1 = true;
                    uploadViewModel.ExistDuration = false;
                    return View(uploadViewModel);
                }
                else
                {
                    if (File != null)
                    {
                        string extension = Path.GetExtension(File.FileName);
                        string[] allowedExtensions = new[] {  ".xlsx" };
                        if (!allowedExtensions.Any(a => a == extension))
                        {
                            ModelState.AddModelError("File", "فایل با این فرمت قابل آپلود نیست !");
                            return View(uploadViewModel);
                        }

                        bool con = MyUtility.ConfirmExcelFile("bordro", File).confirm;
                        string[] ex = MyUtility.ConfirmExcelFile("bordro", File).exp;
                        if (con == false)
                        {
                            uploadViewModel.Message = "فرمت فایل بردرو درست نمی باشد !";
                            return View(uploadViewModel);
                        }

                        string res = MyUtility.ReadUploadedExcel(File);
                        List<PasargadBordroViewModel> bordros = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);

                        string filename = "bordro" + uploadViewModel.Year.ToString() + uploadViewModel.Mounth.ToString() + "_" + DateTime.Now.GetStringShamsiNowDateTime() + extension;
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/bordro", filename);

                        if (uploadViewModel.Action == "add")
                        {

                            CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = await _bordroService.GetDiffrenceNewBordroWithDbAsync(bordros, "add",true,uploadViewModel.Year,uploadViewModel.Mounth);

                            if (compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile != null)
                            {
                                if (compareTwoBaseBordroListsVM.AdditionalDatainUploadedFile.Count() != 0)
                                {
                                    List<PasargadBordroViewModel> NewRecord = await _bordroService.GetFileNewRecords(File);

                                    if (NewRecord != null)
                                    {
                                        if (NewRecord.Count() != 0)
                                        {
                                            
                                            await _bordroService.CreateLifeBordroCollectionAsync(uploadViewModel.Year, uploadViewModel.Mounth, User.Identity.Name, NewRecord);
                                            _bordroService.SaveChange();
                                            if (System.IO.File.Exists(filePath))
                                            {
                                                System.IO.File.Delete(filePath);
                                            }
                                            using var stream = new FileStream(filePath, FileMode.Create);
                                            await File.CopyToAsync(stream);
                                            UploadInfo uploadInfo = new UploadInfo()
                                            {
                                                UpDate = DateTime.Now,
                                                File = filename,
                                                Type="bordro",
                                                Description = "آپلود بردرو" + "|" + DateTime.Now.ToShamsiWithTime(),

                                            };
                                            _bordroService.CreateUploadInfo(uploadInfo);
                                            await _bordroService.SaveChangeAsync();
                                            uploadViewModel.Message = "تعداد" + " " + NewRecord.Count() + " " + "رکورد به پایگاه داده اضافه شد";
                                            return View(uploadViewModel);
                                        }
                                        else
                                        {

                                            uploadViewModel.Message = "تمام داده های فایل " + " " + File.FileName + " قبلا ذخیره شده اند !";
                                            return View(uploadViewModel);
                                        }
                                    }
                                    else
                                    {
                                        uploadViewModel.Message = "تمام داده ها قبلا ثبت شده اند !";
                                        return View(uploadViewModel);
                                    }

                                }
                                else
                                {
                                    uploadViewModel.Message = "تمام داده ها قبلا ثبت شده اند !";
                                    return View(uploadViewModel);
                                }
                            }
                            else
                            {
                                uploadViewModel.Message = "تمام داده ها قبلا ثبت شده اند !";
                                return View(uploadViewModel);
                            }



                        }
                        if (uploadViewModel.Action == "replace")
                        {
                            CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = await _bordroService.GetDiffrenceNewBordroWithDbAsync(bordros, "replace",true, uploadViewModel.Year, uploadViewModel.Mounth);
                            //bool result = _bordroService.UpdateLifeBordroCollectionAsync(compareTwoBaseBordroListsVM.AdditionalDatainDb);
                            bool result = await _bordroService.UpdateLifeBordroCollectionByNewFileAsync(bordros,uploadViewModel.Year,uploadViewModel.Mounth,User.Identity.Name);

                            if (result == true)
                            {
                                await _bordroService.SaveChangeAsync();
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                using var stream = new FileStream(filePath, FileMode.Create);
                                await File.CopyToAsync(stream);
                                UploadInfo uploadInfo = new UploadInfo()
                                {
                                    UpDate = DateTime.Now,
                                    File = filename,
                                    Type = "bordro",
                                    Description = "بازنشانی بردرو" + "|" + DateTime.Now.ToShamsiWithTime(),

                                };
                                _bordroService.CreateUploadInfo(uploadInfo);
                                await _bordroService.SaveChangeAsync();
                                uploadViewModel.Message = "رکوردهای فایل جاری جایگزین اطلاعات موجود در دوره" + " " +uploadViewModel.Mounth + " - " + uploadViewModel.Year + " " + "شدند ";
                                return View(uploadViewModel);
                            }
                            else
                            {
                                uploadViewModel.Message = "خطا رخ داده است !";
                                return View(uploadViewModel);
                            }
                        }
                    }

                }


            }
            if(uploadViewModel.Type == "commission")
            {
                if (uploadViewModel.ValidationStep1 == false)
                {
                    if (await _bordroService.ExistCommissionBaseByYearMounthAsync(uploadViewModel.Year, uploadViewModel.Mounth))
                    {
                        
                        uploadViewModel.Message = "اطلاعات دوره زمانی " + uploadViewModel.Mounth + " " + uploadViewModel.Year + " موجود است !";
                        uploadViewModel.ValidationStep1 = true;
                        uploadViewModel.ExistDuration = true;
                        return View(uploadViewModel);
                    }
                    uploadViewModel.ValidationStep1 = true;
                    uploadViewModel.ExistDuration = false;
                    return View(uploadViewModel);
                }
                else
                {
                    if( File != null)
                    {
                        string extension = Path.GetExtension(File.FileName);
                        string[] allowedExtensions = new[] { ".xlsx" };
                        if (!allowedExtensions.Any(a => a == extension))
                        {
                            ModelState.AddModelError("File", "فقط فایلهای با فرمت xlsx قابل آپلود هستند !");
                            return View(uploadViewModel);
                        }

                        bool con = MyUtility.ConfirmExcelFile("commission", File).confirm;
                        string[] ex = MyUtility.ConfirmExcelFile("commission", File).exp;
                        if (con == false)
                        {
                            uploadViewModel.Message = "فرمت فایل کارمزد درست نمی باشد !";
                            return View(uploadViewModel);
                        }
                        string fileComname = "commission" + uploadViewModel.Year.ToString() + uploadViewModel.Mounth.ToString() + "_" + DateTime.Now.GetStringShamsiNowDateTime() + extension;
                        string fileComPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/commission", fileComname);

                        string res = MyUtility.ReadUploadedExcel(File);
                        List<PasargadCommissionVM> commissions = JsonConvert.DeserializeObject<List<PasargadCommissionVM>>(res);
                        
                        CompareCommissionFileWithDbVM compareCommissionFileWithDbVM = await _bordroService.GetDiffrenceNewCommissonWithDbAsync(commissions);
                        if(compareCommissionFileWithDbVM.ActiveSubmit == true)
                        {
                            if(uploadViewModel.Action =="add")
                            {
                                PasargadCommissionVM pasargadCommissionVMBase = compareCommissionFileWithDbVM.ExludeFileandDb.FirstOrDefault(f => string.IsNullOrEmpty(f.InsNO));
                                bool resp = _bordroService.CreateCommissionCollection(uploadViewModel.Mounth, uploadViewModel.Year, User.Identity.Name, compareCommissionFileWithDbVM.CommonData, pasargadCommissionVMBase);
                                if(resp == true)
                                {
                                    if (System.IO.File.Exists(fileComPath))
                                    {
                                        System.IO.File.Delete(fileComPath);
                                    }
                                    using var stream = new FileStream(fileComPath, FileMode.Create);
                                    await File.CopyToAsync(stream);
                                    UploadInfo uploadInfo = new UploadInfo()
                                    {
                                        UpDate = DateTime.Now,
                                        File = fileComname,
                                        Type = "commission",
                                        Description = "آپلود کارمزد",

                                    };
                                    _bordroService.CreateUploadInfo(uploadInfo);
                                    await _bordroService.SaveChangeAsync();
                                
                                }
                                else
                                {
                                    uploadViewModel.Message = "رکوردهای کارمزد ثبت نشدند !";
                                }
                            }
                            if(uploadViewModel.Action =="replace")
                            {
                                PasargadCommissionVM pasargadCommissionVMBase = compareCommissionFileWithDbVM.ExludeFileandDb.FirstOrDefault(f => string.IsNullOrEmpty(f.InsNO));
                                bool Rem = await _bordroService.RemoveCommissonBaseAsync(uploadViewModel.Year, uploadViewModel.Mounth);
                                await _bordroService.SaveChangeAsync();
                                if(Rem == true)
                                {
                                    bool resp = _bordroService.CreateCommissionCollection(uploadViewModel.Mounth, uploadViewModel.Year, User.Identity.Name, compareCommissionFileWithDbVM.CommonData, pasargadCommissionVMBase);
                                    if (resp == true)
                                    {
                                        if (System.IO.File.Exists(fileComPath))
                                        {
                                            System.IO.File.Delete(fileComPath);
                                        }
                                        using var stream = new FileStream(fileComPath, FileMode.Create);
                                        await File.CopyToAsync(stream);
                                        UploadInfo uploadInfo = new UploadInfo()
                                        {
                                            UpDate = DateTime.Now,
                                            File = fileComname,
                                            Type="commission",
                                            Description = "بازنشانی کارمزد" ,

                                        };
                                        _bordroService.CreateUploadInfo(uploadInfo);
                                        await _bordroService.SaveChangeAsync();
                                        uploadViewModel.Message = "رکوردهای کارمزد با موفقیت ذخیره شدند";
                                    }
                                    else
                                    {
                                        uploadViewModel.Message = "رکوردهای کارمزد ثبت نشدند !";
                                    }
                                }
                                else
                                {
                                    uploadViewModel.Message = "فرآیند بازنشانی کارمزد موفقیت آمیز نبود";
                                }
                                
                            }
                            
                        }
                        else
                        {
                            uploadViewModel.Message = compareCommissionFileWithDbVM.Message;
                            
                        }

                        return View(uploadViewModel);
                    }
                }
            }

            return View(uploadViewModel);




        }

        public IActionResult CreateOther()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(51)]
        public async Task<IActionResult> CreateOther(UploadOtherViewModel uploadOtherViewModel,IFormFile File)
        {
            if (!ModelState.IsValid)
            {
                return View(uploadOtherViewModel);
            }
            if(File == null)
            {
                ModelState.AddModelError("File", "فایل را انتخاب کنید !");
            }
            string extension = Path.GetExtension(File.FileName);
            string[] allowedExtensions = new[] { ".xlsx" };
            if (!allowedExtensions.Any(a => a == extension))
            {
                ModelState.AddModelError("File", "فایل با این فرمت قابل آپلود نیست !");
                return View(uploadOtherViewModel);
            }
            if (uploadOtherViewModel.Type == "addition")
            {
                

                bool con = MyUtility.ConfirmExcelFile("addition", File).confirm;
                string[] ex = MyUtility.ConfirmExcelFile("addition", File).exp;
                if (con == false)
                {
                    uploadOtherViewModel.Message = "فرمت فایل الحاقیه درست نمی باشد !";
                    return View(uploadOtherViewModel);
                }

                string res = MyUtility.ReadUploadedExcel(File);
                List<PasargadBordroViewModel> pasargadAdditions = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);
                AdditionFileToUploadResultModel additionFileToUploadResultModel = _bordroService.GetDiffrenceNewAdditionWithDbAsync(pasargadAdditions, uploadOtherViewModel.Action, true,"upload");
                string fileAdditname = "Addition" + "_" + DateTime.Now.GetStringShamsiNowDateTime() + extension;
                string fileAdditionPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/addition", fileAdditname);
                if (uploadOtherViewModel.Action == "add")
                {
                    bool Deact= await _bordroService.DeactiveBordrosActiveAddition_BaseonAdditionFileInsNO_Async(additionFileToUploadResultModel.QualifiedAdditions.Select(s => s.LifeBordroBase.InsNO).ToList());
                    _bordroService.CreateLifeBordroAdditionCollection(additionFileToUploadResultModel.QualifiedAdditions);
                    await _bordroService.SaveChangeAsync();
                    uploadOtherViewModel.Message = additionFileToUploadResultModel.Message; 
                    
                    if (System.IO.File.Exists(fileAdditionPath))
                    {
                        System.IO.File.Delete(fileAdditname);
                    }
                    


                    using var stream = new FileStream(fileAdditionPath, FileMode.Create);
                    await File.CopyToAsync(stream);
                    UploadInfo uploadInfo = new UploadInfo()
                    {
                        UpDate = DateTime.Now,
                        File = fileAdditname,
                        Type = "addition",
                        Description = "آپلود الحاقیه",

                    };
                    _bordroService.CreateUploadInfo(uploadInfo);
                    await _bordroService.SaveChangeAsync();
                    return View(uploadOtherViewModel);
                }
                if(uploadOtherViewModel.Action == "replace")
                {
                    _bordroService.RemoveLifeBordroAdditionCollection(additionFileToUploadResultModel.AdditionalRecordinDb);
                    _bordroService.CreateLifeBordroAdditionCollection(additionFileToUploadResultModel.QualifiedAdditions);
                    await _bordroService.SaveChangeAsync();
                    uploadOtherViewModel.Message = additionFileToUploadResultModel.Message;
                    
                    if (System.IO.File.Exists(fileAdditionPath))
                    {
                        System.IO.File.Delete(fileAdditname);
                    }
                    using var stream = new FileStream(fileAdditionPath, FileMode.Create);
                    await File.CopyToAsync(stream);
                    UploadInfo uploadInfo = new UploadInfo()
                    {
                        UpDate = DateTime.Now,
                        File = fileAdditname,
                        Type = "addition",
                        Description = "بازنشانی الحاقیه",

                    };
                    _bordroService.CreateUploadInfo(uploadInfo);
                    await _bordroService.SaveChangeAsync();
                    return View(uploadOtherViewModel);
                }

            }
            if(uploadOtherViewModel.Type == "insuredinfo")
            {
                bool con = MyUtility.ConfirmExcelFile("insuredinfo", File).confirm;
                string[] ex = MyUtility.ConfirmExcelFile("insuredinfo", File).exp;
                if (con == false)
                {
                    uploadOtherViewModel.Message = "فرمت فایل اطلاعات بیمه شدگان درست نمی باشد !";
                    return View(uploadOtherViewModel);
                }
                string res = MyUtility.ReadUploadedExcel(File);
                List<PasargadInsuredInfoModel> pasargadInsuredInfoModels = JsonConvert.DeserializeObject<List<PasargadInsuredInfoModel>>(res);
                InsuredFileToUpoadResultModel insuredFileToUpoadResultModel = _bordroService.GetDiffrenceNewInsuredInfoWithDbAsync(pasargadInsuredInfoModels, uploadOtherViewModel.Action, true);


                string fileInsuredname = "insuredInfo" + "_" + DateTime.Now.GetStringShamsiNowDateTime() + extension;
                string fileInsuredPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\upload\insuredInfo", fileInsuredname);
                if (uploadOtherViewModel.Action == "add")
                {
                    _bordroService.CreateInsuredInfoColection(insuredFileToUpoadResultModel.QualifiedRecords);
                    await _bordroService.SaveChangeAsync();
                    uploadOtherViewModel.Message = insuredFileToUpoadResultModel.Message;
                   
                    if (System.IO.File.Exists(fileInsuredPath))
                    {
                        System.IO.File.Delete(fileInsuredname);
                    }
                    using var stream = new FileStream(fileInsuredPath, FileMode.Create);
                    await File.CopyToAsync(stream);
                    UploadInfo uploadInfo = new UploadInfo()
                    {
                        UpDate = DateTime.Now,
                        File = fileInsuredname,
                        Type = "insuredinfo",
                        Description = "آپلود اطلاعات بیمه شدگان",

                    };
                    _bordroService.CreateUploadInfo(uploadInfo);
                    await _bordroService.SaveChangeAsync();
                    return View(uploadOtherViewModel);
                }
                if(uploadOtherViewModel.Action == "replace")
                {
                    _bordroService.RemoveInsuredInfoCollection(insuredFileToUpoadResultModel.AdditionalRecordinDb);
                    _bordroService.CreateInsuredInfoColection(insuredFileToUpoadResultModel.QualifiedRecords);
                    await _bordroService.SaveChangeAsync();
                    uploadOtherViewModel.Message = insuredFileToUpoadResultModel.Message;
                    
                    if (System.IO.File.Exists(fileInsuredPath))
                    {
                        System.IO.File.Delete(fileInsuredname);
                    }
                    using var stream = new FileStream(fileInsuredPath, FileMode.Create);
                    await File.CopyToAsync(stream);
                    UploadInfo uploadInfo = new UploadInfo()
                    {
                        UpDate = DateTime.Now,
                        File = fileInsuredname,
                        Type = "insuredinfo",
                        Description = "بازنشانی اطلاعات بیمه شدگان",

                    };
                    _bordroService.CreateUploadInfo(uploadInfo);
                    await _bordroService.SaveChangeAsync();
                    return View(uploadOtherViewModel);
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult CheckFile(string ftype,string action, IFormFile file)
        {
            if(file == null)
            {
                return null;
            }
            string extension = Path.GetExtension(file.FileName);
            string[] ext = { ".xlsx" };
            FileCheckResultViewModel fileCheckResultViewModel = new FileCheckResultViewModel()
            {
                Conf = false,
                NonExistCol = null
            };
            if (!ext.Any(a => a == extension))
            {
                return Json(fileCheckResultViewModel);
            }
            (bool c, string[] co) = MyUtility.ConfirmExcelFile(ftype, file);
            string FileNonExistColMessage = string.Empty;
            
            

            fileCheckResultViewModel.Conf = c;
            fileCheckResultViewModel.NonExistCol = co;
            string cols = string.Empty;
            
            string res = MyUtility.ReadUploadedExcel(file);

            if(ftype=="bordro")
            {
                if (c == false)
                {
                    if (co != null)
                    {
                        if (co.Length != 0)
                        {
                            FileNonExistColMessage = "<br /><h5 class='alert alert-warning'>" + "ستونهای زیر در فایل بردروی انتخاب شده وجود ندارند ! " + "<span class='tag tag-info'>" +co.Length.ToString()+ "</span>" + "</h5>";
                            string Noncols = string.Empty;
                           
                            foreach (var item in co.ToList())
                            {
                                if (co.ToList().IndexOf(item) == co.ToList().Count - 1)
                                {
                                    Noncols += item ;
                                }
                                else
                                {
                                    Noncols += item + ", ";
                                }
                                                              
                            }
                            FileNonExistColMessage +="<h5 class='text-xs-left alert alert-danger'>"+ Noncols + "</h5>";

                        }
                    }

                }
                List<PasargadBordroViewModel> bordros = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);
                CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = new CompareTwoBaseBordroListsVM();
                compareTwoBaseBordroListsVM = _bordroService.GetDiffrenceNewBordroWithDbAsync(bordros, action,c,0,0).Result;
                fileCheckResultViewModel.Conf = compareTwoBaseBordroListsVM.ActiveSubmit;
                
                fileCheckResultViewModel.Message = compareTwoBaseBordroListsVM.Message;
                if (c == false)
                {
                    fileCheckResultViewModel.Message += FileNonExistColMessage;
                }

            }
            if(ftype == "commission")
            {
                if(c == false)
                {
                    if(co !=null)
                    {
                        if(co.Length !=0)
                        {
                            FileNonExistColMessage = "<br /><h5 class='alert alert-warning'>" + "ستونهای زیر در فایل کارمزد انتخاب شده وجود ندارند ! " + "<span class='tag tag-info'>" + co.Length.ToString() + "</span>" + "</h5>";
                            string Noncols = string.Empty;

                            foreach (var item in co.ToList())
                            {
                                if (co.ToList().IndexOf(item) == co.ToList().Count - 1)
                                {
                                    Noncols += item;
                                }
                                else
                                {
                                    Noncols += item + ", ";
                                }

                            }
                            FileNonExistColMessage += "<h5 class='text-xs-left alert alert-danger'>" + Noncols + "</h5>";
                        }
                    }
                }
                List<PasargadCommissionVM> commissions = JsonConvert.DeserializeObject<List<PasargadCommissionVM>>(res);
                CompareCommissionFileWithDbVM compareCommissionFileWithDbVM = new CompareCommissionFileWithDbVM();
                compareCommissionFileWithDbVM = _bordroService.GetDiffrenceNewCommissonWithDbAsync(commissions).Result;
                fileCheckResultViewModel.Conf = compareCommissionFileWithDbVM.ActiveSubmit;
                fileCheckResultViewModel.Message = compareCommissionFileWithDbVM.Message;
                if(action == "replace")
                {
                    if(compareCommissionFileWithDbVM.CommonData !=null)
                    {
                        if(compareCommissionFileWithDbVM.CommonData.Count()!= 0)
                        {
                            string upMeassage = "<h5 class='alert alert-danger'>" +
                                "رکوردهای این فایل جایگزین رکوردهای سیستم خواهند شد" +
                                "</h5>";
                            fileCheckResultViewModel.Message += upMeassage;
                        }
                    }
                }
                if(c == false)
                {
                    fileCheckResultViewModel.Message += FileNonExistColMessage;
                }
                
            }
            if(ftype== "addition")
            {
                if (c == false)
                {
                    if (co != null)
                    {
                        if (co.Length != 0)
                        {
                            FileNonExistColMessage = "<br /><h5 class='alert alert-warning'>" + "ستونهای زیر در فایل الحاقیه انتخاب شده وجود ندارند ! " + "<span class='tag tag-info'>" + co.Length.ToString() + "</span>" + "</h5>";
                            string Noncols = string.Empty;

                            foreach (var item in co.ToList())
                            {
                                if (co.ToList().IndexOf(item) == co.ToList().Count - 1)
                                {
                                    Noncols += item;
                                }
                                else
                                {
                                    Noncols += item + ", ";
                                }

                            }
                            FileNonExistColMessage += "<h5 class='text-xs-left alert alert-danger'>" + Noncols + "</h5>";

                        }
                    }

                }
                //additions fields are like bordro
                List<PasargadBordroViewModel> additions = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);
                
                AdditionFileToUploadResultModel additionFileToUploadResultModel  = _bordroService.GetDiffrenceNewAdditionWithDbAsync(additions, action, c,"select");
                fileCheckResultViewModel.Conf = additionFileToUploadResultModel.ActiveSubmit;

                fileCheckResultViewModel.Message = additionFileToUploadResultModel.Message;
                if (c == false)
                {
                    fileCheckResultViewModel.Message += FileNonExistColMessage;
                }
            }
            if (ftype == "insuredinfo")
            {
                if (c == false)
                {
                    if (co != null)
                    {
                        if (co.Length != 0)
                        {
                            FileNonExistColMessage = "<br /><h5 class='alert alert-warning'>" + "ستونهای زیر در فایل اطلاعات بیمه شدگان انتخاب شده وجود ندارند ! " + "<span class='tag tag-info'>" + co.Length.ToString() + "</span>" + "</h5>";
                            string Noncols = string.Empty;

                            foreach (var item in co.ToList())
                            {
                                if (co.ToList().IndexOf(item) == co.ToList().Count - 1)
                                {
                                    Noncols += item;
                                }
                                else
                                {
                                    Noncols += item + ", ";
                                }

                            }
                            FileNonExistColMessage += "<h5 class='text-xs-left alert alert-danger'>" + Noncols + "</h5>";

                        }
                    }

                }
                
                List<PasargadInsuredInfoModel> pasargadInsuredInfoModels = JsonConvert.DeserializeObject <List<PasargadInsuredInfoModel>>(res);

                InsuredFileToUpoadResultModel inseuredFileToUpoadResultModel = _bordroService.GetDiffrenceNewInsuredInfoWithDbAsync(pasargadInsuredInfoModels, action, c);
                
                fileCheckResultViewModel.Conf = inseuredFileToUpoadResultModel.ActiveSubmit;

                fileCheckResultViewModel.Message = inseuredFileToUpoadResultModel.Message;
                if (c == false)
                {
                    fileCheckResultViewModel.Message += FileNonExistColMessage;
                }
            }
            return Json(fileCheckResultViewModel);
        }
        [HttpPost]
        public JsonResult CompareBordroWithFileTest(string action, IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            CompareTwoBaseBordroListsVM compareTwoBaseBordroListsVM = new CompareTwoBaseBordroListsVM();
            string extension = Path.GetExtension(file.FileName);
            string[] ext = {  ".xlsx" };
            if (!ext.Any(a => a == extension))
            {
                compareTwoBaseBordroListsVM.ConfAdd = false;
                compareTwoBaseBordroListsVM.ConfUpdate = false;
                compareTwoBaseBordroListsVM.Action = "none";
                compareTwoBaseBordroListsVM.Message = "فایل نامعتبر است !";
                return Json(compareTwoBaseBordroListsVM);
            }
            string mess = string.Empty;
            string res = MyUtility.ReadUploadedExcel(file);
            List<PasargadBordroViewModel> bordros = JsonConvert.DeserializeObject<List<PasargadBordroViewModel>>(res);
            compareTwoBaseBordroListsVM = _bordroService.GetDiffrenceNewBordroWithDbAsync(bordros, action,true,0,0).Result;
            return Json(compareTwoBaseBordroListsVM);
        }
        [HttpPost]
        public  JsonResult CompareComFileWithDb(string ftype,IFormFile file)
        {
            
            CompareCommissionFileWithDbVM compareCommissionFileWithDbVM = new CompareCommissionFileWithDbVM();
            CompareComFileWithResultVM compareComFileWithResultVM = new CompareComFileWithResultVM();
            if (file == null)
            {
                compareComFileWithResultVM.Message = "<p class ='alert alert-danger mt-3'>سرور قادر به دریافت فایل نشده است !</p>";
                return Json(compareComFileWithResultVM);

            }
            string extension = Path.GetExtension(file.FileName);
            string[] ext = {  ".xlsx" };
            if (!ext.Any(a => a == extension))
            {
                //return ("فایل نامعتبر است !");
                compareComFileWithResultVM.Message = "<p>فایل نامعتبر است ! </p>";
                return Json(compareComFileWithResultVM);
            }
            string mess = string.Empty;
            string res = MyUtility.ReadUploadedExcel(file);           
            List<PasargadCommissionVM> commissions = JsonConvert.DeserializeObject<List<PasargadCommissionVM>>(res);
            
            compareCommissionFileWithDbVM = _bordroService.GetDiffrenceNewCommissonWithDbAsync(commissions).Result;
            compareComFileWithResultVM.Message = compareComFileWithResultVM.Message;
            
            return Json(compareComFileWithResultVM);
           
        }
        public string Compare(string action, IFormFile file)
        {
            string fin = file.FileName;
            
            return "success" + fin;
        }
        [HttpPost]
        public IActionResult DownloadFile(string fileName,string type)
        {
            string filePath = string.Empty;
            if(type == "bordro")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/bordro", fileName);
            }
            if (type == "commission")
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/commission", fileName);
            }
            var net = new System.Net.WebClient();
            var data = net.DownloadData(filePath);
            var content = new MemoryStream(data);
            //var contentType = "APPLICATION/octet-stream";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            return File(content, contentType, fileName);
        }



    }
}
