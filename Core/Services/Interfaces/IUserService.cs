using Core.DTOs.Admin;
using Core.DTOs.General;
using Core.DTOs.Performance;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.Permissions;
using DataLayer.Entities.User;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Core.Services.Interfaces
{
    public interface IUserService
    {
        #region User
        public Task CreateUserAsync(User user);
        public void CreateUser(User user);
        public void CreateUserRange(List<User> users);
        public void UpdateUser(User user);
        public Task<User> GetUserByUserName(string userName);
        public Task<List<User>> GetUserByPassword(string password);
        public Task<List<User>> GetAllUsers();
        public Task<bool> ExistUserNC(string NC);
        public Task<bool> ExistUserCellphone(string cellphone);
        public Task<bool> ExistUserBankAccount(string bankAccount);
        public Task<bool> ExistUserBankCard(string cardNumber);
        public Task<User> GetUserByNC(string NC);
        public Task<User> GetUserByIdAsync(int id);
        public Task<User> GetUserByCode(string code);
        public Task<User> GetUserByUserName_and_PasswordAsync(string userName, string pass);
        public Task<List<User>> GetUsers_hasNoRoleAsync();
        public Task<User> GetUserByCellphone(string Cellphone);
        public Task<User> GetUserByBankAccountNumber(string bankAccountNumber);
        public Task<User> GetUserByBankCardNumber(string bankCardNumber);
        public Task<List<User>> GetUsersActive_and_HasActiveRoleAsync();
        /// <summary>
        /// دریافت کد کاربری جدید
        /// </summary>
        /// <returns></returns>
        public Task<string> GetNewCode();
        
        public  Task<bool> CreateUserListFromBordro(List<PasargadBordroViewModel> pasargadBordroViewModels);
        public bool SendVerificationCode(string code, string phoneNumber);
        
        #endregion User
        #region Role
        public Task<Role> GetRoleByIdAsync(int id);
        public Task<Role> GetRoleByNameAsync(string roleName);
        public Task<List<Role>> GetAllRolesAsync();
        public Task AddPermissionsToRoleAsync(int roleId, List<int> permission);
        public Role EditRole(Role role);
        public Task DeleteRoleAsync(int roleId);
        public Task<Role> CreateRoleAsync(Role role, List<int> permissions);
        public Task UpdatePermissions_of_RoleAsync(int roleId, List<int> permissionsId);
        public Task<List<User>> GetUsers_of_RoleAsync(int roleId);
        public Task<List<UserRole>> GetUserRoles_of_RoleAsync(int roleId);
        public Task<List<Role>> GetAllRolesofUserWithNCAsync(string NC);
        public Task<List<Role>> GetAllRolesofUserWithNameAsync(string name);
        public void AddRoleToUsers(List<int> usersId, int roleId, string Creator);
        public Task<bool> ChangeRoleofUserAsync(int urId, int NewroleId);
        #endregion Role
        #region UserRole
        public List<ParentWithRate> GetParentsofUserRole(List<ParentWithRate> parentWithRates,int urId,int rate =1 );
       
        public Task<List<UserRole>> GetNonOrgUserRoles();
        public Task<UserRole> GetUserRoleBy_UserName_RoleId(string UserName, int RoleId);
        public Task<List<UserRole>> GetUserRoles();      
        public Task CreateUserRoleAsync(UserRole userRole);
        public void CreateUserRole(UserRole userRole);       
        public Task<List<Role>> Get_User_Roles_ByNC_Async(string NC);
        public Task<List<Role>> Get_User_Roles_ByName_Async(string UserName);
        public Task<UserRole> GetUserRoleByIdAsync(int id);
        public Task<List<UserRole>> GetUsers_hasRoleAsync(string search);
        public Task<List<UserRole>> GetUsers_hasRoleAsyncWithPagination(string search,int? count = null, int? page = null);
        public Task<List<UserRole>> GetUserRolesByUserNC(string NC);
        public Task<List<UserRole>> GetUserRolesByUserCode(string Code);
        public Task<List<UserRole>> GetDirectChildsAsync(int urId);
        
        /// <summary>
        /// لیست تمام کاربران یک ناظر
        /// </summary>
        /// <param name="urId"></param>
        /// <returns></returns>
        public IEnumerable<ChildRate> GetAllChilds(int urId,int Level=0,int Period=0);
       
        public Task<UserRole> GetParentofUserRole(int urId);
        public List<ChildRate> GetParentsSeries(int urId,int rate, List<ChildRate> userRoles);
        /// <summary>
        /// بدست آوردن آمار و پورتفوی جامع کاربر
        /// </summary>
        /// <param name="urId"></param>
        /// <returns></returns>
        public Task<(long SalesSum, int SalesCount)> GetUserRoleActivityStatics(int urId);
        public string GetStringParentSeries(int urId, int rate, List<Seller_Rate_PercentViewModel> seller_Rate_PercentViewModels,string res);
       
       
        public void Recursive_Change_UserRole_Parent(int? urId, int? parent_urId, string OpName);
        public Task<bool> Change_Parent_Of_UserRole(int urId, int? parent_urId, string OpName);
        public Task<string> GetUserOrgLevel(int urId);
        public Task<(int stReq,long portfoReq,string rateTitle)> GetUserHigherRankInfo(int urId);
        #endregion UserRole      
        #region Permission
        Task<List<Permission>> GetAllPermissions();
        Task<List<Permission>> GetPermissions_of_RoleByRoleId(int roleId);
        bool CheckPermissionByRole(int permissionId, string userName, int roleId);
        bool CheckPermissionByNameAsync(int permissionId, string userName);
        #endregion
        #region ChangeLog
        public void CreateChangeLog(ChangeLog changeLog);
        #endregion ChangeLog
        #region RoleComission
        public void CreateRoleCommission(RoleCommission roleCommission);
        public Task CreateRoleCommissionAsync(RoleCommission roleCommission);
        public void UpdateRoleCommission(RoleCommission roleCommission);
        public Task<List<RoleCommission>> GetRoleCommissionsAsync();
        public Task<RoleCommission> GetRoleCommissionByIdAsync(int id);
        public void RemoveRoleCommission(RoleCommission roleCommission);
        public bool RoleCommissionExist(int id);
        #endregion
        #region RoleEqulity
        public void CreateRoleEqulity(RoleEqulity roleEqulity);
        public Task CreateRoleEqulityAsync(RoleEqulity roleEqulity);
        public void UpdateRoleEqulity(RoleEqulity roleEqulity);
        public Task<List<RoleEqulity>> GetRoleEqulitysAsync();
        public Task<RoleEqulity> GetRoleEqulityByIdAsync(int id);
        public void RemoveRoleEqulity(RoleEqulity roleEqulity);
        public bool RoleEqulityExist(int id);
        #endregion RoleEqulity
        #region RolePool
        public void CreateRolePool(RolePool rolePool);
        public Task CreateRolePoolAsync(RolePool rolePool);
        public void UpdateRolePool(RolePool rolePool);
        public Task<List<RolePool>> GetRolePoolsAsync();
        public Task<RolePool> GetRolePoolByIdAsync(int id);
        public void RemoveRolePool(RolePool rolePool);
        public bool RolePoolExist(int id);
        #endregion
        #region Document
        public void CreateDocument(Document document);
        #endregion Document
        #region UserMesaages
        public Task<List<UserMessage>> GetTodayMessages();
        #endregion
        #region Generic
        public bool SendMessage(SendMessageViewModel sendMessageViewModel);
        public string CompareTwoEntity<T>(T entity1, T entity2);
       
        public bool SendUserCode_and_password(string UserCode, string Pass, string phoneNumber);
        public Task<List<ChangeLog>> GetRecordUpdateLogs(int id, string tableName);
        public Task SaveChangesAsync();
        public void SaveChange();
        public void DoAttach<T>(T entity);
        public void DoDetached<T>(T entity);
        public IWorkbook WriteExcelWithNPOI<T>(T Entity, List<T> data,string title, string extension = "xlsx");
        #endregion Generic
    }
}
