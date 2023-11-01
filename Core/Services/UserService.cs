using Core.DTOs.General;
using Core.Security;
using Core.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.Permissions;
using DataLayer.Entities.User;
using Core.Convertors;
using Microsoft.EntityFrameworkCore;
using SmsIrRestfulNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.DTOs.Admin;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using Core.Utility;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.ComponentModel;

using Core.DTOs.Performance;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly MyContext _Context;
        public UserService(MyContext Context)
        {
            _Context = Context;
        }
        #region User
        public async Task<List<UserRole>> GetNonOrgUserRoles()
        {
            return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Where(w => w.RoleId == 2 || w.RoleId == 3).ToListAsync();
        }
        public async Task CreateUserAsync(User user)
        {
            await _Context.Users.AddAsync(user);
        }
        public void CreateUser(User user)
        {
            _Context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _Context.Users.Update(user);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _Context.Users.ToListAsync();
        }

        public async Task<List<User>> GetUserByPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                string pass = PasswordHelper.EncodePasswordMd5(password.Trim());
                return await _Context.Users.Where(w => w.Password == pass).ToListAsync();
            }
            return null;

        }
        public async Task<bool> ExistUserBankAccount(string bankAccount)
        {
            return await _Context.Users.AnyAsync(f => f.BankAccountNumber.Trim() == bankAccount.Trim());
        }

        public async Task<bool> ExistUserBankCard(string cardNumber)
        {

            return await _Context.Users.AnyAsync(f => f.BankCardNumber.Trim() == cardNumber.Trim());

        }
        public async Task<User> GetUserByUserName(string userName)
        {
            return await _Context.Users.Include(r => r.UserRoles).Include(r => r.Documents)
                .Include(r => r.County).Include(r => r.County.State)
                .SingleOrDefaultAsync(s => s.Code == userName.Trim());
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _Context.Users.Include(r => r.UserRoles).Include(r => r.Documents)
                .Include(r => r.County).Include(r => r.County.State)
                .SingleOrDefaultAsync(s => s.Id == id);
        }
        public async Task<bool> ExistUserNC(string NC)
        {
            if (Core.Utility.MyUtility.IsValidNC(NC) == false)
            {
                return false;
            }
            else
            {
                User userNC = await _Context.Users.FirstOrDefaultAsync(f => f.NC == NC);
                if (userNC == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<bool> ExistUserCellphone(string cellphone)
        {
            User UserCell = await _Context.Users.FirstOrDefaultAsync(f => f.Cellphone == cellphone);
            if (UserCell == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<User> GetUserByNC(string NC)
        {
            return await _Context.Users.Include(r => r.County).Include(r => r.County.State)
                .SingleOrDefaultAsync(s => s.NC == NC);
        }
        public async Task<User> GetUserByUserName_and_PasswordAsync(string userName, string pass)
        {

            User userWithCodePassword = await _Context.Users.FirstOrDefaultAsync(w => w.Code == userName.Trim() && w.Password.Equals(pass));
            return userWithCodePassword;

        }

        public async Task<List<User>> GetUsers_hasNoRoleAsync()
        {
            return await _Context.Users.Include(r => r.UserRoles).Include(r => r.Documents)
                .Where(w => w.UserRoles.Count() == 0).ToListAsync();
        }

        public async Task<bool> CreateUserListFromBordro(List<PasargadBordroViewModel> pasargadBordroViewModels)
        {
            List<User> users = new List<User>();
            List<UserRole> userRoles = new List<UserRole>();

            foreach (var item in pasargadBordroViewModels)
            {
                string cell = Core.Generators.GeneratorClass.GeneratePassword(7, "digit");
                var output = Regex.Replace(item.AgentName, @"[\d-]", string.Empty).Trim();
                string[] fullName = output.Split(" ");
                List<int> countyIds = await _Context.Counties.Select(s => s.CountyId).ToListAsync();
                var random = new Random();
                int Countyindex = random.Next(1, 1333);
                while (!countyIds.Any(a => a == Countyindex))
                {
                    Countyindex = random.Next(1, 1333);
                }

                User buser = new User()
                {
                    FName = fullName.First(),
                    LName = fullName.Last(),
                    NC = item.AgentNC,
                    RegDate = item.IssueDate.ChangeToMiladiWithoutTime(),
                    Cellphone = "0912" + cell,
                    Password = item.AgentNC,
                    Code = item.AgentNC,
                    BirthDate = item.StartDate.ChangeToMiladiWithoutTime(),
                    CountyId = Countyindex,
                    CellphoneIsConfirmed = true,
                    IsActive = true
                };


                if (!users.Any(a => a.NC == buser.NC))
                {
                    users.Add(buser);
                }



            }
            if (users.Count() != 0)
            {
                foreach (var usr in users)
                {
                    int roleid = new Random().Next(5, 11);
                    UserRole userRole = new UserRole()
                    {
                        RoleId = roleid,
                        User = usr,
                        RegisterDate = DateTime.Now,
                        OP_Create = "system",
                        IsActive = true
                    };

                }
                _Context.Users.AddRange(users);
            }
            if (userRoles.Count() != 0)
            {
                //userRoles = userRoles.Distinct().ToList();
                _Context.UserRoles.AddRange(userRoles);
            }

            if (users.Count() != 0 || userRoles.Count() != 0)
            {
                await _Context.SaveChangesAsync();
            }

            return true;

        }
        public void CreateUserRange(List<User> users)
        {
            _Context.Users.AddRange(users);
        }
        public async Task<User> GetUserByCode(string code)
        {
            return await _Context.Users.Include(r => r.UserRoles).Include(r => r.Documents).Include(r => r.County).Include(r => r.County.State)
                .SingleOrDefaultAsync(s => s.Code == code);
        }
        public async Task<string> GetNewCode()
        {
            string code = "907";
            int c = int.Parse(code);
            List<User> users = await _Context.Users.Where(w => w.Code.StartsWith("907")).ToListAsync();
            User user = users.OrderByDescending(r => int.Parse(r.Code)).FirstOrDefault();
            if (user != null)
            {
                if (user.Code.StartsWith("907"))
                {
                    if (user.Code == "907")
                    {
                        c = 907 * 10;
                        c++;
                        return c.ToString();
                    }
                    else
                    {
                        code = user.Code.Remove(0, 3);
                        c = int.Parse(code) + 1;
                        string cd = "907" + c.ToString();
                        return "907" + c.ToString();
                    }

                }
            }
            return code;

        }

        public bool SendVerificationCode(string code, string phoneNumber)
        {
            var token = new Token().GetToken("2027ea4381a2e4def2bf654", "@#rth@123456#");

            var restVerificationCode = new RestVerificationCode()
            {
                Code = code,
                MobileNumber = phoneNumber
            };

            var restVerificationCodeRespone = new VerificationCode().Send(token, restVerificationCode);
            if (restVerificationCode != null)
            {
                if (restVerificationCodeRespone.IsSuccessful)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<User> GetUserByCellphone(string Cellphone)
        {
            return await _Context.Users.Include(r => r.UserRoles).SingleOrDefaultAsync(s => s.Cellphone == Cellphone);
        }

        public async Task<User> GetUserByBankAccountNumber(string bankAccountNumber)
        {
            if (!string.IsNullOrEmpty(bankAccountNumber))
            {
                return await _Context.Users.SingleOrDefaultAsync(s => s.BankAccountNumber == bankAccountNumber);
            }
            else
            {
                return null;
            }
        }

        public async Task<User> GetUserByBankCardNumber(string bankCardNumber)
        {
            if (!string.IsNullOrEmpty(bankCardNumber))
            {
                return await _Context.Users.SingleOrDefaultAsync(s => s.BankCardNumber == bankCardNumber);
            }
            else
            {
                return null;
            }
        }

        public async Task<List<User>> GetUsersActive_and_HasActiveRoleAsync()
        {
            List<User> users = await _Context.UserRoles.Include(r => r.User).Where(w => w.IsActive && w.User.IsActive).Select(x => x.User).Distinct().ToListAsync();
            return users;
        }
        #endregion User
        #region Role
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _Context.Roles.FindAsync(id);
        }
        public async Task AddPermissionsToRoleAsync(int roleId, List<int> permission)
        {
            foreach (var p in permission)
            {
                await _Context.RolePermissions.AddAsync(new RolePermission()
                {
                    PermissionId = p,
                    RoleId = roleId
                });
            }
        }
        public async Task<Role> CreateRoleAsync(Role role, List<int> permissions)
        {
            await _Context.AddAsync(role);
            await AddPermissionsToRoleAsync(role.RoleId, permissions);
            return role;
        }
        public Role EditRole(Role role)
        {
            _Context.Roles.Update(role);
            return role;
        }
        public async Task UpdatePermissions_of_RoleAsync(int roleId, List<int> permissionsId)
        {
            //_Context.RolePermissions.Where(p => p.RoleId == roleId).ToList().ForEach(p => _Context.RolePermissions.Remove(p));
            List<int> CurPer = await _Context.RolePermissions.Where(w => w.RoleId == roleId).Select(s => s.Permission.PermissionId).ToListAsync();
            List<int> RemovePer = CurPer.Except(permissionsId).ToList();
            List<int> AddPer = permissionsId.Except(CurPer).ToList();
            if (RemovePer != null && RemovePer.Count() != 0)
            {
                foreach (var p in RemovePer)
                {
                    RolePermission rolePermission = await _Context.RolePermissions.SingleOrDefaultAsync(s => s.RoleId == roleId && s.PermissionId == p);
                    _Context.RolePermissions.Remove(rolePermission);

                }
            }
            if (AddPer != null && AddPer.Count() != 0)
            {
                await AddPermissionsToRoleAsync(roleId, AddPer);
            }

        }
        public async Task<List<User>> GetUsers_of_RoleAsync(int roleId)
        {
            List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)

                .Where(w => w.RoleId == roleId).ToListAsync();
            return userRoles.Select(s => s.User).ToList();
        }
        public async Task DeleteRoleAsync(int roleId)
        {
            Role role = await _Context.Roles.SingleOrDefaultAsync(r => r.RoleId == roleId);
            _Context.Roles.Remove(role);
        }

        public Task<List<Role>> GetAllRolesofUserWithNCAsync(string NC)
        {
            if (!string.IsNullOrEmpty(NC))
            {
                return _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Where(w => w.User.NC == NC).Select(s => s.Role).ToListAsync();
            }
            return null;
        }
        public async Task<List<UserRole>> GetUserRoles_of_RoleAsync(int roleId)
        {
            return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                .Where(w => w.RoleId == roleId).ToListAsync();
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _Context.Roles.FirstOrDefaultAsync(f => f.RoleName == roleName.Trim());
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _Context.Roles.Include(r => r.UserRoles).ToListAsync();
        }
        public async Task<bool> ChangeRoleofUserAsync(int urId, int NewroleId)
        {
            UserRole currnet = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.Childeren)
                               .SingleOrDefaultAsync(s => s.URId == urId);
            if (currnet == null)
            {
                return false;
            }
            List<UserRole> childs = currnet.Childeren.ToList();
            currnet.Childeren = null;
            currnet.IsActive = false;
            _Context.UserRoles.Update(currnet);
            UserRole NewuserRole = new UserRole()
            {
                User_ID = currnet.User_ID,
                RoleId = NewroleId,
                IsActive = true,
                Childeren = childs,
                RegisterDate = DateTime.Now,
                UserRoleParentReceiveDate = DateTime.Now,
                UserRoleParentId = currnet.UserRoleParentId
            };
            await _Context.UserRoles.AddAsync(NewuserRole);

            return true;
        }

        #endregion Role
        #region UserRole
        public List<ParentWithRate> GetParentsofUserRole(List<ParentWithRate> parentWithRates, int urId, int rate = 1)
        {
            UserRole current = _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.Childeren).Include(r => r.UserRoleParent)
                    .FirstOrDefault(f => f.URId == urId);

            if (current == null)
            {
                return parentWithRates;
            }
            if (current.UserRoleParent != null)
            {
                parentWithRates.Add(new ParentWithRate { Rate = rate,UserRole = current.UserRoleParent});
                GetParentsofUserRole(parentWithRates, current.UserRoleParent.URId, rate+1);
            }

            return parentWithRates;


        }

        public async Task<List<Role>> Get_User_Roles_ByNC_Async(string NC)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.NC == NC);
            if (user == null)
            {
                return null;
            }
            else
            {
                List<UserRole> userRoles = await _Context.UserRoles.Where(w => w.User_ID == user.Id).Include(r => r.User).Include(r => r.Role).ToListAsync();
                if (userRoles != null)
                {
                    List<Role> roles = userRoles.Select(s => s.Role).ToList();
                    return roles;
                }
                return null;

            }
        }
        public async Task<List<Role>> Get_User_Roles_ByName_Async(string Name)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.Code.Trim() == Name);
            if (user == null)
            {
                return null;
            }
            else
            {
                List<UserRole> userRoles = await _Context.UserRoles.Where(w => w.User_ID == user.Id).Include(r => r.User).Include(r => r.Role).ToListAsync();
                if (userRoles != null)
                {
                    List<Role> roles = userRoles.Select(s => s.Role).ToList();
                    return roles;
                }
                return null;

            }
        }
        public async Task<UserRole> GetUserRoleBy_UserName_RoleId(string UserName, int RoleId)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.Code == UserName);
            if (user == null)
            {
                return null;
            }
            Role role = await _Context.Roles.SingleOrDefaultAsync(s => s.RoleId == RoleId);
            if (role == null)
            {
                return null;
            }
            UserRole userRole = await _Context.UserRoles.FirstOrDefaultAsync(f => f.RoleId == RoleId && f.User_ID == user.Id);
            if (userRole == null)
            {
                return null;
            }
            return userRole;
        }

        public async Task<List<UserRole>> GetUserRoles()
        {
            return await _Context.UserRoles
                .Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                .OrderBy(r => r.URId)
                .ToListAsync();
        }


        public void CreateUserRole(UserRole userRole)
        {
            _Context.UserRoles.Add(userRole);
        }

        public async Task CreateUserRoleAsync(UserRole userRole)
        {
            await _Context.UserRoles.AddAsync(userRole);
        }

        public async Task<UserRole> GetUserRoleByIdAsync(int id)
        {
            return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                .Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                .Include(r => r.Childeren).Include(r => r.User.County).Include(r => r.User.County.State)
                .SingleOrDefaultAsync(s => s.URId == id);
        }
        public async Task<List<UserRole>> GetUsers_hasRoleAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                List<UserRole> userRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                    .Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                    .Include(r => r.User.Documents).Include(r => r.User.County).Include(r => r.User.County.State).ToListAsync();
                userRoles = userRoles.Where(w => w.FullPro.Contains(search.Trim()) || w.User.FullName.Contains(search.Trim()) || w.Role.RoleTitle.Contains(search.Trim()) || w.User.Code == search.Trim())
                .ToList();
                return userRoles;
            }
            else
            {
                return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                    .Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                    .Include(r => r.User.Documents).Include(r => r.User.County).Include(r => r.User.County.State).ToListAsync();
            }

        }
        public async Task<List<UserRole>> GetUsers_hasRoleAsyncWithPagination(string search, int? count = null, int? page = null)
        {
            int acCount = count.GetValueOrDefault(30);
            int curPage = page.GetValueOrDefault(1);

            List<UserRole> AllUserRoles = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent).Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                List<UserRole> userRoles = AllUserRoles.OrderByDescending(r => int.Parse(r.User.Code)).ToList();
                userRoles = userRoles.Where(w => w.FullPro.Contains(search.Trim()) || w.User.FullName.Contains(search.Trim()) || w.Role.RoleTitle.Contains(search.Trim()) || w.User.Code == search.Trim()).ToList();
                userRoles = userRoles.Skip(acCount * (curPage - 1)).Take(acCount).ToList();

                return userRoles;
            }
            else
            {
                List<UserRole> userRoles = AllUserRoles.OrderByDescending(r => int.Parse(r.User.Code)).ToList();
                userRoles = userRoles.Skip(acCount * (curPage - 1)).Take(acCount).ToList();
                return userRoles;
            }

        }
        public void AddRoleToUsers(List<int> usersId, int roleId, string Creator)
        {
            foreach (var item in usersId)
            {
                _Context.UserRoles.Add(new UserRole()
                {
                    User_ID = item,
                    RoleId = roleId,
                    RegisterDate = DateTime.Now,
                    OP_Create = Creator,

                });
            }
        }
        public async Task<List<UserRole>> GetUserRolesByUserNC(string NC)
        {
            return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role)
                .Where(w => w.User.NC.Trim() == NC.Trim()).ToListAsync();
        }
        public async Task<UserRole> GetUser_CurrentParent(int urId)
        {
            UserRole userRole = await _Context.UserRoles.Include(r => r.UserRoleParent).SingleOrDefaultAsync(s => s.URId == urId);
            if (userRole == null) return null;
            int? parentId = userRole.UserRoleParentId;
            if (parentId == null) return null;
            return await _Context.UserRoles.Include(r => r.UserRoleParent).Include(r => r.User).Include(r => r.Role)
                .SingleOrDefaultAsync(s => s.URId == parentId);
        }

        public async Task<List<UserRole>> GetDirectChildsAsync(int urId)
        {
            return await _Context.UserRoles.Include(r => r.UserRoleParent).Include(r => r.User).Include(r => r.Role)
                .Where(w => w.UserRoleParentId == urId).ToListAsync();
        }
        public IEnumerable<ChildRate> GetAllChilds(int urId, int Level = 0, int Period = 0)
        {
            UserRole current = _Context.UserRoles.Include(p => p.User).Include(p => p.Role).Include(r => r.Childeren)
               .Include(p => p.UserRoleParent).Include(r => r.User.County).Include(r => r.User.County.State)
               .SingleOrDefault(f => f.URId == urId);

            Period += 1;
            List<UserRole> userRoles = current.Childeren.ToList();

            if (userRoles.Any())
            {

                Level = 1;
                foreach (var item in userRoles)
                {

                    yield return new ChildRate { UserRole = item, Rate = Level };

                    if (GetAllChilds(item.URId) != null)
                    {

                        Level += 1;
                        foreach (var subcategory in GetAllChilds(item.URId))
                        {

                            yield return new ChildRate { UserRole = subcategory.UserRole, Rate = Level };
                        }
                    }
                    else
                    {
                        Level = 1;
                    }
                }
            }
            else
            {
                Level = 1;
            }
        }


        public async Task<bool> Change_Parent_Of_UserRole(int urId, int? parent_urId, string OpName)
        {
            UserRole current = await _Context.UserRoles.Include(r => r.UserRoleParent).Include(r => r.Childeren)
                                .SingleOrDefaultAsync(s => s.URId == urId);

            UserRole newParent = await _Context.UserRoles.Include(r => r.UserRoleParent)
            .SingleOrDefaultAsync(r => r.URId == parent_urId);

            if (current == null)
            {
                return false;
            }
            if (newParent == null)
            {
                return false;
            }
            //foreach (var item in _Context.UserRoles.Where(w => w.User_ID == current.User_ID && w.RoleId == current.RoleId))
            //{
            //    item.IsActive = false;
            //    _Context.UserRoles.Update(item);

            //}
            List<UserRole> childs = current.Childeren.ToList();
            //foreach (var item in current.Childeren)
            //{
            //    item.IsActive = false;
            //}
            current.IsActive = false;
            _Context.UserRoles.Update(current);

            UserRole newUserRole = new UserRole()
            {
                User_ID = current.User_ID,
                RoleId = current.RoleId,
                UserRoleParentId = parent_urId,
                UserRoleParentReceiveDate = DateTime.Now,
                IsActive = true,
                Childeren = childs,
                RegisterDate = DateTime.Now,
                OP_Create = OpName
            };
            _Context.UserRoles.Add(newUserRole);

            return true;
        }
        public void Recursive_Change_UserRole_Parent(int? urId, int? parent_urId, string OpName)
        {


            if (urId != null && parent_urId != null)
            {
                UserRole current = _Context.UserRoles.Include(r => r.UserRoleParent).Include(r => r.Childeren)
                               .SingleOrDefault(s => s.URId == urId);
                current.IsActive = false;
                _Context.UserRoles.Update(current);

                UserRole newParent = _Context.UserRoles.Include(r => r.UserRoleParent).Include(r => r.User).Include(r => r.Role)
                                    .SingleOrDefault(r => r.URId == parent_urId);

                UserRole newUserRole = new UserRole()
                {
                    User_ID = current.User_ID,
                    RoleId = current.RoleId,
                    UserRoleParentId = parent_urId,
                    UserRoleParentReceiveDate = DateTime.Now,
                    IsActive = true,
                    RegisterDate = DateTime.Now,
                    OP_Create = OpName
                };
                _Context.UserRoles.Add(newUserRole);

                foreach (var item in current.Childeren)
                {
                    item.IsActive = false;

                }

                _Context.UserRoles.Update(current);

                _Context.SaveChanges();
                if (current.Childeren != null)
                {
                    if (current.Childeren.Count() != 0)
                    {
                        foreach (var item in current.Childeren)
                        {
                            Recursive_Change_UserRole_Parent(item.URId, newUserRole.URId, OpName);
                        }
                    }
                    else
                    {
                        Recursive_Change_UserRole_Parent(null, null, OpName);
                    }
                }
                else
                {
                    Recursive_Change_UserRole_Parent(null, null, OpName);
                }
            }

        }
        public async Task<List<UserRole>> GetUserRolesByUserCode(string Code)
        {
            return await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                .Include(r => r.Childeren)
                .Include(r => r.UserRoleParent.User).Include(r => r.UserRoleParent.Role)
                .Where(w => w.User.Code == Code.Trim()).ToListAsync();
        }
        public async Task<UserRole> GetParentofUserRole(int urId)
        {
            UserRole userRole = await _Context.UserRoles
                .Include(r => r.UserRoleParent).Include(r => r.User).Include(r => r.Role)
                .Include(r => r.UserRoleParent.User).Include(r => r.Role)
                .SingleOrDefaultAsync(s => s.URId == urId);
            if (userRole == null)
            {
                return null;
            }
            return userRole.UserRoleParent;

        }
        public List<ChildRate> GetParentsSeries(int urId, int rate, List<ChildRate> userRoles)
        {
            UserRole current = _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                .SingleOrDefault(s => s.URId == urId && s.IsActive == true);
            ChildRate childRate = new ChildRate { UserRole = current, Rate = rate };
            userRoles.Add(childRate);
            if (current.UserRoleParent != null)
            {
                GetParentsSeries((int)current.UserRoleParentId, rate + 1, userRoles);
            }
            return userRoles;
        }

        public string GetStringParentSeries(int urId, int rate, List<Seller_Rate_PercentViewModel> seller_Rate_PercentViewModels, string res)
        {

            UserRole current = _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Include(r => r.UserRoleParent)
                .SingleOrDefault(s => s.URId == urId && s.IsActive == true);
            float per = 0;
            long SumPremium = GetUserRoleActivityStatics(urId).Result.SalesSum;
            int SumStatic = GetUserRoleActivityStatics(urId).Result.SalesCount;
            RoleCommission roleCommission = _Context.RoleCommissions.OrderByDescending(r => r.MinSaleValue).OrderByDescending(r => r.MinSaleCount).FirstOrDefault(w => w.RoleId == current.RoleId && w.MinSaleCount <= SumStatic && w.MinSaleValue <= SumPremium);
            if (roleCommission != null)
            {
                if (rate == 1)
                {
                    per = roleCommission.PersonalSalesPercent;
                }
                else
                {
                    per = roleCommission.OrganizationSalesPercent;
                }

            }
            Seller_Rate_PercentViewModel seller_Rate_PercentViewModel = new Seller_Rate_PercentViewModel { Seller = current, Rate = rate, Percent = per, UrId = urId };

            seller_Rate_PercentViewModels.Add(seller_Rate_PercentViewModel);
            if (current.UserRoleParent != null)
            {
                return GetStringParentSeries((int)current.UserRoleParentId, rate + 1, seller_Rate_PercentViewModels, res);
            }
            else
            {
                foreach (var item in seller_Rate_PercentViewModels.OrderByDescending(r => r.Rate))
                {
                    res += item.UrId + "-" + item.Rate + "-" + item.Percent + Environment.NewLine;
                }
                return res;
            }


        }
        public async Task<(long SalesSum, int SalesCount)> GetUserRoleActivityStatics(int urId)
        {
            UserRole current = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).FirstOrDefaultAsync(f => f.URId == urId);
            List<ChildRate> childRates = GetAllChilds(urId).ToList();
            long SumSales = _Context.LifeBordroAdditions.Where(w => w.SellerNC == current.User.NC).Sum(s => s.PremiumbyPaymentMethod) + current.User.InitialPortfo;
            int CountSales = _Context.LifeBordroAdditions.Where(w => w.SellerNC == current.User.NC).Count() + current.User.InitialStatic; ;
            foreach (var item in childRates.Select(s => s.UserRole).Where(w => w.IsActive == true))
            {
                SumSales += _Context.LifeBordroAdditions.Where(w => w.SellerNC == item.User.NC).Sum(s => s.PremiumbyPaymentMethod);
                CountSales += _Context.LifeBordroAdditions.Where(w => w.SellerNC == item.User.NC).Count();
            }

            return (SumSales, CountSales);

        }
        public async Task<string> GetUserOrgLevel(int urId)
        {
            UserRole userRole = await _Context.UserRoles.Include(r => r.User).Include(r => r.Role).SingleOrDefaultAsync(s => s.URId == urId);
            if (userRole == null)
            {
                return "-";
            }
            long SumPremium = GetUserRoleActivityStatics(urId).Result.SalesSum;
            int SumStatic = GetUserRoleActivityStatics(urId).Result.SalesCount;
            RoleCommission roleCommission = await _Context.RoleCommissions.OrderByDescending(r => r.MinSaleValue).ThenByDescending(r => r.MinSaleCount).FirstOrDefaultAsync(w => w.RoleId == userRole.RoleId && w.MinSaleCount <= SumStatic && w.MinSaleValue <= SumPremium);
            if (roleCommission == null)
            {
                return "-";
            }
            return roleCommission.Rate;
        }
        public async Task<(int stReq, long portfoReq,string rateTitle)> GetUserHigherRankInfo(int urId)
        {
           
            UserRole userRole = await _Context.UserRoles.AsNoTracking().Include(r => r.User).Include(r => r.Role).SingleOrDefaultAsync(s => s.URId == urId);
           
            if (userRole == null)
            {
                return (-1, -100,string.Empty);
            }
            
            long SumPremium = GetUserRoleActivityStatics(urId).Result.SalesSum;
            int SumStatic = GetUserRoleActivityStatics(urId).Result.SalesCount;

            RoleCommission CurroleCommission = await _Context.RoleCommissions.OrderByDescending(r => r.MinSaleValue).ThenByDescending(r => r.MinSaleCount).FirstOrDefaultAsync(w => w.RoleId == userRole.RoleId && w.MinSaleCount <= SumStatic && w.MinSaleValue <= SumPremium);
            //List<RoleCommission> roleCommissions = await _Context.RoleCommissions.Include(r => r.Role).OrderBy(r => r.MinSaleValue).ThenBy(r => r.MinSaleCount).ToListAsync();
            if(CurroleCommission == null)
            {
                return (-100, -10000, "<span class ='text-danger'>-</span>");
            }
            RoleCommission NextroleCommission = await _Context.RoleCommissions.Include(r => r.Role).OrderBy(r => r.MinSaleValue).ThenBy(r => r.MinSaleCount).FirstOrDefaultAsync(w => w.MinSaleCount > CurroleCommission.MinSaleCount && w.MinSaleValue > CurroleCommission.MinSaleValue);

            if (NextroleCommission == null)
            {
                return (-10, -1000, "<span class ='text-danger'>بالاتر</span>");
            }
            string rate ="<span class='bg-success pr-1 pl-1 rounded white'>" + NextroleCommission.Role.RoleTitle + " | " + NextroleCommission.Rate + "</span>";
            int DifSaleCount = 0; long DifSaleValue=0;
            if((int)NextroleCommission.MinSaleCount > SumStatic)
            {
                DifSaleCount = (int)NextroleCommission.MinSaleCount - SumStatic;
            }
            if(NextroleCommission.MinSaleValue > SumPremium)
            {
                DifSaleValue = NextroleCommission.MinSaleValue - SumPremium;
            }
            return (DifSaleCount, DifSaleValue,rate);
        }
        public async Task<List<Role>> GetAllRolesofUserWithNameAsync(string name)
        {
            User user = await _Context.Users.SingleOrDefaultAsync(s => s.Code == name);
            if (user == null) return null;
            return _Context.UserRoles.Include(r => r.User).Include(r => r.Role).Where(w => w.User_ID == user.Id).Select(s => s.Role).ToList();
        }




        #endregion UserRole
        #region Permissions
        public async Task<List<Permission>> GetAllPermissions()
        {
            return await _Context.Permissions.Include(r => r.Permissions).ToListAsync();
        }
        public async Task<List<Permission>> GetPermissions_of_RoleByRoleId(int roleId)
        {
            return await _Context.RolePermissions
               .Where(r => r.RoleId == roleId)
               .Select(s => s.Permission).ToListAsync();
        }
        public bool CheckPermissionByRole(int permissionId, string userName, int roleId)
        {
            int userId = _Context.Users.FirstOrDefault(u => u.Code == userName).Id;
            Role role = _Context.Roles.FirstOrDefault(s => s.RoleId == roleId);
            RolePermission rolePermission = _Context.RolePermissions
                .FirstOrDefault(p => p.PermissionId == permissionId && p.RoleId == roleId);
            bool result = _Context.RolePermissions
                .Any(p => p.PermissionId == permissionId && p.RoleId == roleId);

            return result;
        }
        public bool CheckPermissionByNameAsync(int permissionId, string userName)
        {
            int userId = _Context.Users.Single(u => u.Code == userName).Id;

            List<int> UserRoles = _Context.UserRoles
                .Where(r => r.User_ID == userId && r.IsActive == true).Select(r => r.RoleId).ToList();

            if (!UserRoles.Any())
                return false;

            List<int> RolesPermission = _Context.RolePermissions
                .Where(p => p.PermissionId == permissionId)
                .Select(p => p.RoleId).ToList();

            return RolesPermission.Any(p => UserRoles.Contains(p));
        }



        #endregion Permissions
        #region ChangeLog
        public void CreateChangeLog(ChangeLog changeLog)
        {
            _Context.ChangeLogs.Add(changeLog);
        }
        #endregion
        #region General
        private DataTable ConvertListToDataTable<T>(List<T> Data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in Data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
        public IWorkbook WriteExcelWithNPOI<T>(T Entity, List<T> data, string title, string extension = "xlsx")
        {
            // Get DataTable
            DataTable dt = ConvertListToDataTable(data);
            // Instantiate Wokrbook
            IWorkbook workbook;
            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new Exception("The format '" + extension + "' is not supported.");
            }
            //make top row
            ISheet sheet1 = workbook.CreateSheet("Sheet 1");
            sheet1.IsRightToLeft = true;
            IFont TopRowFont = workbook.CreateFont();
            TopRowFont.FontName = "topFont";
            TopRowFont.IsBold = true;
            TopRowFont.FontHeight = 350;

            IRow topRow = sheet1.CreateRow(0);
            var CellStyleTop = workbook.CreateCellStyle();
            CellStyleTop.Alignment = HorizontalAlignment.Center;
            CellStyleTop.VerticalAlignment = VerticalAlignment.Center;
            CellStyleTop.SetFont(TopRowFont);
            ICell cellTop = topRow.CreateCell(0);
            cellTop.CellStyle = CellStyleTop;
            cellTop.SetCellValue(title);

            var cra = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dt.Columns.Count - 1);
            sheet1.AddMergedRegion(cra);

            //make a header row
            IFont font1 = workbook.CreateFont();
            font1.FontName = "Font1";
            font1.IsBold = true;
            font1.Color = IndexedColors.Black.Index;



            IRow row1 = sheet1.CreateRow(1);
            var CellStyleHeader = workbook.CreateCellStyle();
            CellStyleHeader.Alignment = HorizontalAlignment.Center;
            CellStyleHeader.VerticalAlignment = VerticalAlignment.Center;

            // center-align currency values
            CellStyleHeader.Alignment = HorizontalAlignment.Center;
            CellStyleHeader.VerticalAlignment = VerticalAlignment.Center;
            CellStyleHeader.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            CellStyleHeader.FillPattern = FillPattern.SolidForeground;
            CellStyleHeader.SetFont(font1);



            var CellStyleBody = workbook.CreateCellStyle();
            // center-align currency values
            CellStyleBody.Alignment = HorizontalAlignment.Center;
            CellStyleBody.VerticalAlignment = VerticalAlignment.Center;




            PropertyInfo[] props = Entity.GetType().GetProperties();
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                string Title = MyUtility.GetDisplayName(props[j]);
                if (!string.IsNullOrEmpty(Title))
                {
                    cell.SetCellValue(Title);
                    cell.CellStyle = CellStyleHeader;
                }


            }

            //loops through data
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    string columnName = dt.Columns[j].ToString();
                    string columnValue = dt.Rows[i][columnName].ToString();
                    string Title = MyUtility.GetDisplayName(props[j]);
                    if (columnName == "SalesOrg")
                    {
                        string[] cellval = columnValue.Split("|");
                        string nstr = string.Empty;
                        int loop = 1;
                        foreach (var item in cellval)
                        {
                            if (item != cellval.LastOrDefault())
                            {
                                nstr += $"{item}\n";
                            }
                            else
                            {
                                nstr += item;
                            }
                            loop++;
                        }
                        cell.SetCellValue(nstr);

                        ICellStyle cs = workbook.CreateCellStyle();
                        cs.Alignment = HorizontalAlignment.Center;
                        cs.VerticalAlignment = VerticalAlignment.Center;
                        cs.WrapText = true;
                        cs.ShrinkToFit = true;
                        cell.CellStyle = cs;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Title))
                        {
                            cell.CellStyle.WrapText = true;
                            cell.CellStyle = CellStyleBody;
                            if (dt.Columns[j].DataType.IsNumeric())
                            {
                                cell.SetCellValue(double.Parse(columnValue));
                            }
                            else
                            {
                                cell.SetCellValue(columnValue);
                            }


                        }

                    }

                }
            }
            // Auto size columns
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < row1.LastCellNum; j++)
                {
                    sheet1.AutoSizeColumn(j);
                }
            }
            return workbook;
        }
        public async Task SaveChangesAsync()
        {
            await _Context.SaveChangesAsync();
        }

        public void SaveChange()
        {
            _Context.SaveChanges();
        }
        public void DoAttach<T>(T entity)
        {
            _Context.Attach(entity);
        }
        public void DoDetached<T>(T entity)
        {
            _Context.Entry(entity).State = EntityState.Detached;
        }



        public bool SendMessage(SendMessageViewModel sendMessageViewModel)
        {
            if (string.IsNullOrEmpty(sendMessageViewModel.Key) && string.IsNullOrEmpty(sendMessageViewModel.SecurityCode) && string.IsNullOrEmpty(sendMessageViewModel.SMSirLineNumber))
            {
                sendMessageViewModel.Key = "ff58ee01b10397c8756ee01";
                sendMessageViewModel.SecurityCode = "@543!%1358&";
                sendMessageViewModel.SMSirLineNumber = "30004554552992";
            }
            var messageSendObject = new MessageSendObject()
            {

                Messages = sendMessageViewModel.Messages.ToArray(),
                MobileNumbers = sendMessageViewModel.MobileNumbers.ToArray(),
                LineNumber = sendMessageViewModel.SMSirLineNumber,
                SendDateTime = DateTime.Now,
                CanContinueInCaseOfError = false
            };
            SmsIrRestfulNetCore.Token token = new Token();
            string result = token.GetToken(sendMessageViewModel.Key, sendMessageViewModel.SecurityCode);
            if (!string.IsNullOrEmpty(result))
            {
                SmsIrRestfulNetCore.MessageSendResponseObject MessageSendResponseObject = new MessageSend().Send(result, messageSendObject);
                return MessageSendResponseObject.IsSuccessful;
            }
            else
            {
                return false;
            }

        }
        public bool SendUserCode_and_password(string UserCode, string Pass, string phoneNumber)
        {
            var token = new Token().GetToken("2027ea4381a2e4def2bf654", "@#rth@123456#");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = long.Parse(phoneNumber),
                TemplateId = 30030,
                ParameterArray = new List<UltraFastParameters>()
            {
        new UltraFastParameters()
        {
            Parameter = "username" , ParameterValue = UserCode,

        },
        new UltraFastParameters()
        {
            Parameter ="password",ParameterValue = Pass
        }

    }.ToArray()

            };

            UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public T CheckUpdateObject<T>(T originalObj, T updateObj)
        {

            foreach (var property in updateObj.GetType().GetProperties())
            {
                if (property.GetValue(updateObj, null) == null)
                {
                    property.SetValue(updateObj, originalObj.GetType().GetProperty(property.Name)
                    .GetValue(originalObj, null));
                }
            }
            return updateObj;
        }



        public string CompareTwoEntity<T>(T entity1, T entity2)
        {
            //Type t = entity1.GetType();

            PropertyInfo[] props = entity1.GetType().GetProperties();
            string result = string.Empty;


            foreach (var item in props)
            {
                var v1 = item.GetValue(entity1);
                var v2 = item.GetValue(entity2);//is old
                var pn = Utility.MyUtility.GetDisplayName(item);
                if (v1 == null)
                {
                    v1 = string.Empty;
                }
                if (v2 == null)
                {
                    v2 = string.Empty;
                }

                if (v1.ToString().Trim() != v2.ToString().Trim())
                {
                    if (pn != null)
                    {
                        result += "تغییر در " + " " + pn + " " + " از مقدار " + v2 + " به مقدار " + v1 + Environment.NewLine;
                    }
                    else
                    {
                        result += "تغییر در " + " " + item.Name + " " + " از مقدار " + v2 + " به مقدار " + v1 + Environment.NewLine;
                    }

                }

            }
            return result;
        }
        public async Task<List<ChangeLog>> GetRecordUpdateLogs(int id, string tableName)
        {
            return await _Context.ChangeLogs.Where(w => w.PrimaryKeyValue == id.ToString() && w.EntityName.Trim() == tableName.Trim()).ToListAsync();

        }

        #endregion General
        #region RoleCommission
        public void CreateRoleCommission(RoleCommission roleCommission)
        {
            _Context.RoleCommissions.Add(roleCommission);
        }

        public async Task CreateRoleCommissionAsync(RoleCommission roleCommission)
        {
            await _Context.RoleCommissions.AddAsync(roleCommission);
        }

        public void UpdateRoleCommission(RoleCommission roleCommission)
        {
            _Context.RoleCommissions.Update(roleCommission);
        }

        public async Task<List<RoleCommission>> GetRoleCommissionsAsync()
        {
            return await _Context.RoleCommissions.Include(r => r.Role).ToListAsync();
        }

        public async Task<RoleCommission> GetRoleCommissionByIdAsync(int id)
        {
            return await _Context.RoleCommissions.Include(r => r.Role).SingleOrDefaultAsync(s => s.Id == id);
        }

        public void RemoveRoleCommission(RoleCommission roleCommission)
        {
            _Context.RoleCommissions.Remove(roleCommission);
        }
        public bool RoleCommissionExist(int id)
        {
            return _Context.RoleCommissions.Any(a => a.Id == id);
        }
        #endregion RoleCommission
        #region RoleEqulity
        public void CreateRoleEqulity(RoleEqulity roleEqulity)
        {
            _Context.RoleEqulities.Add(roleEqulity);
        }

        public async Task CreateRoleEqulityAsync(RoleEqulity roleEqulity)
        {
            await _Context.RoleEqulities.AddAsync(roleEqulity);
        }

        public void UpdateRoleEqulity(RoleEqulity roleEqulity)
        {
            _Context.RoleEqulities.Update(roleEqulity);
        }

        public async Task<List<RoleEqulity>> GetRoleEqulitysAsync()
        {
            return await _Context.RoleEqulities.Include(r => r.Role).ToListAsync();
        }

        public async Task<RoleEqulity> GetRoleEqulityByIdAsync(int id)
        {
            return await _Context.RoleEqulities.Include(r => r.Role).SingleOrDefaultAsync(s => s.Id == id);
        }

        public void RemoveRoleEqulity(RoleEqulity roleEqulity)
        {
            _Context.RoleEqulities.Remove(roleEqulity);
        }

        public bool RoleEqulityExist(int id)
        {
            return _Context.RoleEqulities.Any(a => a.Id == id);
        }


        #endregion RoleEqulity
        #region RolePool
        public void CreateRolePool(RolePool rolePool)
        {
            _Context.RolePools.Add(rolePool);
        }

        public async Task CreateRolePoolAsync(RolePool rolePool)
        {
            await _Context.RolePools.AddAsync(rolePool);
        }

        public void UpdateRolePool(RolePool rolePool)
        {
            _Context.RolePools.Update(rolePool);
        }

        public async Task<List<RolePool>> GetRolePoolsAsync()
        {
            return await _Context.RolePools.Include(r => r.Role).ToListAsync();
        }

        public async Task<RolePool> GetRolePoolByIdAsync(int id)
        {
            return await _Context.RolePools.Include(r => r.Role).SingleOrDefaultAsync(s => s.Id == id);
        }

        public void RemoveRolePool(RolePool rolePool)
        {
            _Context.RolePools.Remove(rolePool);
        }

        public bool RolePoolExist(int id)
        {
            return _Context.RolePools.Any(a => a.Id == id);
        }




        #endregion RolePool
        #region Document
        public void CreateDocument(DataLayer.Entities.User.Document document)
        {
            _Context.Documents.Add(document);
        }
        #endregion Document
        #region UserMessages
        public async Task<List<UserMessage>> GetTodayMessages()
        {
            return await _Context.UserMessages.Where(w => w.CreateDate.Value.Date == DateTime.Now.Date).ToListAsync();
        }

        



        #endregion
    }
}
