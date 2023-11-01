using Core.DTOs.Account;
using Core.DTOs.Admin;
using Core.DTOs.Chart;
using Core.DTOs.General;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    public interface IBordroService
    {
        #region Bordro
        #region General
       
        public void SaveChange();
        public Task SaveChangeAsync();
        public bool CreateTextFile(string Filename, string data);
        public DataTable ConvertListToDataTable<T>(List<T> Data);
        public IWorkbook WriteExcelWithNPOI<T>(T Entity,List<T> data, string title, string extension = "xlsx");
        public (string InsNo, int AddNo) GetInsNo_and_AddNoFromString(string ComplexInsNo, string Seperator="_");

        public string GetBranchfromInsNo(string InsNo);
        public string GetBranchNamefromInsNo(string InsNo);
        public string GetBranchNameByCode(string code);
        //public void DbAttach();
        #endregion
        /// <summary>
        /// خواندن رکوردهای بردرو از یک فایل اکسل
        /// </summary>
        /// <param name="SheetName"></param>
        /// <param name="FileName"></param>
        /// <param name="Root"></param>
        /// <returns></returns>
        public List<LifeBordroAddition> Read_and_PrepareLifeBordroAdditionList_FromExcelFile(string SheetName,string FileName, string Root);

        public int GetBordroExcelFileColumnIndex(string ColTitle);
        public List<Object> ReadExcel(string root);
        public void CreateLifeBordroBase(LifeBordroBase lifeBordroBase);
        public void CreateLifeBordroBaseCollection(List<LifeBordroBase> lifeBordroBases);
        public void CreateLifeBordroAdditionCollection(List<LifeBordroAddition> lifeBordroAdditions);
        public  Task<bool> DeactiveBordrosActiveAddition_BaseonAdditionFileInsNO_Async(List<string> InsNOs);
        public bool RemoveLifeBordroAdditionCollection(List<LifeBordroAddition> lifeBordroAdditions);
        public Task<List<LifeBordroBase>> GetLifeBordroBasesAsync();
        public List<LifeBordroBase> GetLifeBordroBases();
        public Task<CompareTwoBaseBordroListsVM> GetDiffrenceNewBordroWithDbAsync(List<PasargadBordroViewModel> pasargadbordroes,string action,bool FileIsValid,int Year,int Mounth);
        public AdditionFileToUploadResultModel GetDiffrenceNewAdditionWithDbAsync(List<PasargadBordroViewModel> pasargadadditions, string action, bool FileIsValid,string FormActionType);
        public InsuredFileToUpoadResultModel GetDiffrenceNewInsuredInfoWithDbAsync(List<PasargadInsuredInfoModel> pasargadInsuredInfos, string action, bool FileIsValid);
        /// <summary>
        /// ثبت بردروهای جدید بر اساس فایل آپلود شده
        /// </summary>
        /// <param name="pasargadBordroViewModels"></param>
        /// <returns></returns>
        public Task<bool> CreateLifeBordroCollectionAsync(int Year, int Mounth,string LoginName, List<PasargadBordroViewModel> pasargadBordroViewModels);
       
        public bool UpdateLifeBordroCollectionAsync(List<LifeBordroBase> UploadBodro);
        public Task<bool> UpdateLifeBordroCollectionByNewFileAsync(List<PasargadBordroViewModel> bordros,int Year,int Mounth,string LoginName);
        public void CreateCommissionBase(CommissionBase commissionBase);
       
        public Task<bool> RemoveBordroByYearMounthAsync(int year, int mounth);
        public Task<bool> ExistInsNOinBordroAdditionAsync(string InsNO);
        /// <summary>
        /// بررسی وجود بردرو بر اساس سال و ماه
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Mounth"></param>
        /// <returns></returns>
        public Task<bool> ExistBordroABaseByYearMounthAsync(int Year, int Mounth);
        public Task<List<LifeBordroBase>> GetNewLifeBordroBasesFromBFile(List<PasargadBordroViewModel> NewBordro);
       

        public Task<List<PasargadBordroViewModel>> GetFileNewRecords(IFormFile File);

        public string GetStringParentSeries(int urId, int rate, List<Seller_Rate_PercentViewModel> seller_Rate_PercentViewModels, string res);
        
        public Task<(long SalesSum, int SalesCount)> GetUserRoleActivityStatics(int urId);
        public IEnumerable<ChildRate> GetAllChilds(int urId, int Level = 0, int Period = 0);
        

        public Task<List<LifeBordroBase>> GetLifeBordroBasesBySellerNC(string NC,int? RecCount, int? page,bool all,string search, int IsDateRange);
        public Task<LifeBordroBase> GetLifeBordroBaseById(Guid id);
        public Task<List<LifeBordroBase>> GetIndirectBordroBasebyurId(int urId);
        public Task<List<LifeBordroBase>> GetDirectBordroBasebyurIdAsync(int urId);
        public Task<List<LifeBordroBase>> GetDirectBordroBasebyNC(string NC);
        public Task<List<LifeBordroBase>> GetInDirectBordroBasebyNC(string NC);
      
        #endregion
        #region LifeBordroAddition
        public Task<List<NonePaymentBordroesDet>> GetNonPaidBordroesAsync(string Name);
        public Task<LifeBordroAddition> GetLifeBordroAdditionByIdAsync(int id);
        public (DateTime startDate, DateTime endDate) GetStartEnd_12MounthBefore(int Year, int Mounth);
        /// <summary>
        /// بیمه نامه های فروخته شده بر اساس تاریخ صدور در 12 ماه گذشته کاربر
        /// دوازده ماه گذشته : از آغاز ماه بعد ماه داده شده و سال قبل از سال داده شده
        ///  تا پایان ماه داده شده ی سال داده شده 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mounth"></param>
        /// <param name="nc"></param>
        /// <returns></returns>
        public Task<List<LifeBordroAddition>> GetDirectLifeBordroAdditionin12MounthAgoAsync(int year, int mounth, string nc);
        public Task<List<LifeBordroAddition>> GetIndirectLifeBordroAdditionin12MounthAgoAsync(int year, int mounth, string nc);

        public Task<List<ExcelBordroModel>> GetPersonalExcelBordroModelsByNCAsync(string NC);
        public Task<List<ExcelBordroModel>> GetOrgExcelBordroModelsByNCAsync(string NC);
        #endregion LifeBordroAddition
        #region Commission
        public Task<CommissionBase> GetLastCommissionAsync();
        public Task<int> GetNumberofInsCommissionPaid(string InsNo);
        /// <summary>
        ///تعداد اقساط سررسید شده بیمه نامه
        /// </summary>
        /// <param name="InsNo"></param>
        /// <returns></returns>
        public Task<int> GetNumberofIns_OverdueInstallment(string InsNo);

        public Task<bool> ExistCommissionBaseByYearMounthAsync(int Year, int Mounth);
        /// <summary>
        /// بررسی وجود تمام شماره بیمه نامه های کارمزد در بوردرو
        /// </summary>
        /// <param name="commissions"></param>
        /// <returns></returns>
        public Task<bool> ExistAllCommissionInsNO_in_Bordro(List<PasargadCommissionVM> commissions);
        public bool CreateCommissionCollection(int Mounth, int Year, string LoginName, List<PasargadCommissionVM> Pasargadcommissions, PasargadCommissionVM pasargadCommissionBase);
        public Task<CommissionBase> GetCommissionBaseByYearMounthAsync(int Year, int Mounth);
        public Task<bool> RemoveCommissonBaseAsync(int Year, int Mounth);
        public Task<CompareCommissionFileWithDbVM> GetDiffrenceNewCommissonWithDbAsync(List<PasargadCommissionVM> pasargadCommissionVMs);
        public Task<string> GetStandardInsNOfromDb(string Brach_InsNO);
        public Task<List<Commission>> GetCommissionsBySellerNC_Year_Mounth(string SellerNC, int Year, int Mounth);
        public Task<List<Commission>> GetCommissionsByYear_and_Mounth(int Year, int Mounth);
        public Task<Commission> GetCommissionByIdAsync(int id);
        
        public Task<List<OrgUserComVM>> GetOrgCommissionsAsync(List<int> SelectedSubUserUrIds,string LoginIdentityName, int Year = 0, int Mounth = 0);
        /// <summary>
        /// کارمزد سیستم بر اساس کارمزد پاسارگاد برای کاربران سیستم
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mounth"></param>
        /// <returns></returns>
        public Task<List<SystemCommissionVM>> GetSystemCommissonsAsync(List<int> SelectedSubUserUrIds,int year, int mounth);
        public Task<List<OrgCommissionReportModel>> ConvertOrgUserComVM_To_OrgCommissionReportModelAsync(List<OrgUserComVM> orgUserComVMs, int Year, int Mounth);
        public Task<List<PersonalCommissionReportModel>> CovertCommissions_To_PersonalCommissionReportModelAsync(List<Commission> PersoanlCommisssions, int Year, int Mounth);
        public Task<List<SystemCommissionVM>> GetUserSystemCommissionAsync(string Name, int year, int mounth);
        public Task<List<SystemCommissionVM>> ZGetUserSystemCommissionAsync(string Name, int year, int mounth);
        #endregion Commission
        #region SalesPackage

        public Task<List<SalesObject>> GetSalesObjectsofBordroAsync(Guid BordroGId);
        public string GetStringofSalesLevelsAsync(List<SalesObject> salesObjects);
        #endregion
        #region UploadInfo
        public Task<List<UploadInfo>> GetUploadInfosAsync();
        public void CreateUploadInfo(UploadInfo uploadInfo);

        #endregion UploadInfo

        #region RoolPool
        public Task<PoolRewardReportResultVM> GetUserPoolRewardAsync(int? userId,int? year, int? mounth);
        public List<PoolRewardReport_TotalVM> GetTotalPoolRewardReport(PoolRewardReportResultVM poolRewardReportResultVM);
        /// <summary>
        /// پیدا کردن کاربران هر استخر در ماه و سال درخواستی به همراه فروش شخصی و سازمانی
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mounth"></param>
        /// <returns></returns>
        public Task<List<RolePool_Users>> GetRolePoolUsersAsync(int year,int mounth);
        /// <summary>
        /// پیدا کردن اطلاعات هر کاربر در استخرش - درصد سهم و مبلغ سهم
        /// </summary>
        /// <param name="year"></param>
        /// <param name="mounth"></param>
        /// <returns></returns>
        public Task<List<RolePool_Users>> GetRolePool_UsersInfoAsync(int year, int mounth);
        #endregion RoolPool
        #region InsuredInfo
        public void CreateInsuredInfo(InsuredInformation insuredInformation);
        public void CreateInsuredInfoColection(List<InsuredInformation> insuredInformationList);
        public void RemoveInsuredInfoCollection(List<InsuredInformation> insuredInformationList);
        public Task<List<InsuredInfoReportModel>> PrepareInseredInfoReportModelAsync(string NC);
        #endregion InsuredInfo

    }
}
