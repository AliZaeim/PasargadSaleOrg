using Core.DTOs.General;
using Core.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.EntityFrameworkCore;
using SmsIrRestfulNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Convertors;
using DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore.Internal;
using System.Reflection.Metadata.Ecma335;

namespace Core.Services
{
    public class ComplementaryService : IComplementaryService
    {
        private readonly MyContext _Context;
        public ComplementaryService(MyContext Context)
        {
            _Context = Context;
        }
        #region General
        public void SaveChanges()
        {
            _Context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _Context.SaveChangesAsync();
        }
        public void DoDetached<T>(T entity)
        {
            _Context.Entry(entity).State = EntityState.Detached;
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
        public void CreateChangeLog(ChangeLog changeLog)
        {
            _Context.ChangeLogs.Add(changeLog);
        }


        #endregion
        #region State
        public async Task<List<State>> GetStatesAsync()
        {
            return await _Context.States.ToListAsync();
        }
        #endregion
        #region County
        public async Task<List<County>> GetCountiesAsync()
        {
            return await _Context.Counties.ToListAsync();
        }

        public async Task<List<County>> GetCountiesofStateAsync(int stateId)
        {
            State state = await _Context.States.Include(r => r.Counties).SingleOrDefaultAsync(s => s.StateId == stateId);
            return state.Counties.ToList();
        }


        #endregion
        #region SendMessage
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
        #endregion
        #region Slider
        public void CreateSlider(Slider slider)
        {
            _Context.Sliders.Add(slider);
        }

        public async Task CreateSliderAsync(Slider slider)
        {
            await _Context.Sliders.AddAsync(slider);
        }

        public void UpdateSlider(Slider slider)
        {
            _Context.Sliders.Update(slider);
        }

        public async Task<List<Slider>> GetSlidersAsync()
        {
            return await _Context.Sliders.ToListAsync();
        }

        public async Task<Slider> GetSliderByIdAsync(int id)
        {
            return await _Context.Sliders.FindAsync(id);
        }

        public void RemoveSlider(Slider slider)
        {
            _Context.Sliders.Remove(slider);
        }

        public bool SliderExist(int id)
        {
            return _Context.Sliders.Any(a => a.Id == id);
        }


        #endregion Slider
        #region UploadInfo
        public void CreateUploadInfo(UploadInfo uploadInfo)
        {
            _Context.UploadInfos.Add(uploadInfo);
        }

        public void EditUploadInfo(UploadInfo uploadInfo)
        {
            _Context.UploadInfos.Update(uploadInfo);
        }

        public void RemoveUploadInfo(int id)
        {
            UploadInfo uploadInfo = _Context.UploadInfos.Find(id);
            _Context.UploadInfos.Remove(uploadInfo);
        }

        public async Task<List<UploadInfo>> GetUploadInfos()
        {
            return await _Context.UploadInfos.ToListAsync();
        }

        public async Task<UploadInfo> GetUploadInfo(int id)
        {
            return await _Context.UploadInfos.FindAsync(id);
        }

        public bool ExistUploadInfo(int id)
        {
            return _Context.UploadInfos.Any(a => a.Id == id);
        }


        #endregion UploadInfo
        #region UserMessage
        public void CreateUserMessage(UserMessage userMessage)
        {
            _Context.UserMessages.Add(userMessage);
        }

        public void UpdateUserMessage(UserMessage userMessage)
        {
            _Context.UserMessages.Update(userMessage);
        }

        public async Task<bool> RemoveUserMessage(int Id)
        {
            UserMessage userMessage = await _Context.UserMessages.FindAsync(Id);
            if (userMessage == null)
            {
                return false;
            }
            _Context.UserMessages.Remove(userMessage);
            return true;
        }

        public async Task<List<UserMessage>> GetUserMessagesAsync()
        {
            return await _Context.UserMessages.ToListAsync();
        }

        public bool ExistUserMessage(int Id)
        {
            return _Context.UserMessages.Any(a => a.Id == Id);
        }

        public async Task<UserMessage> GetUserMessage(int Id)
        {
            return await _Context.UserMessages.FindAsync(Id);
        }



        #endregion UserMessage
        #region Branch
        public void CreateBranch(Branch branch)
        {
            _Context.Branches.Add(branch);
        }

        public void UpdateBranch(Branch branch)
        {
            _Context.Branches.Update(branch);
        }

        public async Task<Branch> GetBranchByIdAsync(int Id)
        {
            return await _Context.Branches.FindAsync(Id);
        }

        public async Task<List<Branch>> GetBranchesAsync()
        {
            return await _Context.Branches.ToListAsync();
        }

        public void RemoveBranch(int Id)
        {
            Branch branch = _Context.Branches.Find(Id);
            if (branch != null)
            {
                _Context.Branches.Remove(branch);
            }
        }

        public bool ExistBranch(int Id)
        {
            return _Context.Branches.Any(a => a.Id == Id);
        }

        public void CreateBranchCollection(List<Branch> branches)
        {
            _Context.AddRange(branches);
        }

        #endregion Branch
        #region UserConversations
        public void CreateConversation(Conversation conversation)
        {
            _Context.Conversations.Add(conversation);
        }

        public void EditConversation(Conversation conversation)
        {
            _Context.Conversations.Update(conversation);
        }

        public async Task<Conversation> GetConversationByIdAsync(int Id)
        {
            Conversation Conversation = await _Context.Conversations.Include(r => r.Parent)
                .FirstOrDefaultAsync(f => f.Id == Id);
            return Conversation;
        }

        public async Task<List<Conversation>> GetConversationsAsync()
        {
            return await _Context.Conversations.Include(r => r.Parent).ToListAsync();
        }

        public void RemoveConversation(int Id)
        {
            Conversation Conversation = _Context.Conversations.Find(Id);
            if (Conversation != null)
            {
                _Context.Conversations.Remove(Conversation);
            }
        }

        public async Task<List<Conversation>> GetTodayConversationsAsync(string code)
        {

            if (code == "290070")
            {
                return await _Context.Conversations
                .Where(w => w.CreateDate.Date == DateTime.Now.Date).ToListAsync();
            }
            else
            {

                List<Conversation> TodayusersConversations = await _Context.Conversations.Include(r => r.Parent)
                .Where(w => w.CreateDate.Date == DateTime.Now.Date).ToListAsync();
                TodayusersConversations = TodayusersConversations.Where(w => w.SenderCode == code || w.Parent.SenderCode == code).ToList();
                return TodayusersConversations;
            }

        }

        public async Task<List<Conversation>> GetConversationsByNameAsync(string code)
        {
            User user = await _Context.Users.FirstOrDefaultAsync(f => f.Code == code.Trim());
            if (user == null)
            {
                return null;
            }
            if (code != "290070")
            {
                List<Conversation> conversations = await _Context.Conversations.Include(r => r.Parent).ToListAsync();
                return conversations.Where(w => w.SenderCode == code || w.RecepiesList.Any(a => a.Substring(0, a.IndexOf("-")) == code)).ToList();
            }
            else
            {
                List<Conversation> conversations = await _Context.Conversations.Include(r => r.Parent).ToListAsync();
                return conversations;
            }

        }

        public async Task<List<Conversation>> GetConversationsByParentIdAsync(int parentId)
        {
            return await _Context.Conversations.Include(r => r.Parent)
                .Where(w => w.ParentId == parentId).ToListAsync();
        }

        public bool ExistConversation(int Id)
        {
            return _Context.Conversations.Any(a => a.Id == Id);
        }

        public async Task<List<Conversation>> GetUnreadConversationsByNameAsync(string code)
        {
            List<Conversation> conversations = await _Context.Conversations.Where(w => !string.IsNullOrEmpty(w.RecepiesInfo)).ToListAsync();
            List<Conversation> LoginConversations = conversations.Where(w => w.RecepiesList.ToList().Any(a => a.Substring(0, a.IndexOf("-")) == code)).ToList();
            List<Conversation> LoginUnreadConversations = LoginConversations.Where(w => string.IsNullOrEmpty(w.Readers) ||
                                   (!string.IsNullOrEmpty(w.Readers) && !w.ReadersList.ToList().Any(a => a.Substring(0, a.IndexOf("-")) == code))).ToList();

            return LoginUnreadConversations;
        }
        public async Task<Conversation> GetTopParent_ofConversationAsync(int Id)
        {
            Conversation conversation = await _Context.Conversations.Include(x => x.Parent).SingleOrDefaultAsync(r => r.Id == Id);
            while(conversation.ParentId != null)
            {
                conversation = _Context.Conversations.Include(x => x.Parent).SingleOrDefault(r => r.Id ==(int) conversation.ParentId);
            }
            if(conversation.Id == Id)
            {
                return null;
            }
           
            return conversation;
        }
        public async Task<List<Conversation>> GetParents_ofConversationAsync(int id, List<Conversation> parents)
        {
            Conversation current =await _Context.Conversations.Include(r => r.Parent).FirstOrDefaultAsync(x => x.Id == id);
            if(current.Parent != null)
            {
                parents.Add(current.Parent);
                await GetParents_ofConversationAsync((int)current.ParentId, parents);
            }
            return parents;
        }
        public async Task<List<Conversation>> GetAllChilds_ofConversationAsync(int Id, List<Conversation> childs)
        {
            List<Conversation> children = await _Context.Conversations.Include(r => r.Parent).Where(w => w.ParentId == Id).ToListAsync();
            while(children != null)
            {
                childs.AddRange(children);
                foreach (var item in children)
                {
                    await GetAllChilds_ofConversationAsync(item.Id, childs);
                }

            }
            return childs;
        }

       
        #endregion UserConversations

    }
}
