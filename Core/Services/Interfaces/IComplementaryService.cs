using Core.DTOs.General;
using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    public interface IComplementaryService
    {
        #region General
        public void SaveChanges();
        public Task SaveChangesAsync();
        public void DoDetached<T>(T entity);
        public string CompareTwoEntity<T>(T entity1, T entity2);
        public void CreateChangeLog(ChangeLog changeLog);
       
        #endregion
        #region State
        public Task<List<State>> GetStatesAsync();
        #endregion
        #region County
        public Task<List<County>> GetCountiesAsync();
        public Task<List<County>> GetCountiesofStateAsync(int stateId);
        #endregion
        #region SendMessage
        public bool SendMessage(SendMessageViewModel sendMessageViewModel);
        #endregion
        #region slider
        public void CreateSlider(Slider slider);
        public Task CreateSliderAsync(Slider slider);
        public void UpdateSlider(Slider slider);
        public Task<List<Slider>> GetSlidersAsync();
        public Task<Slider> GetSliderByIdAsync(int id);
        public void RemoveSlider(Slider slider);
        public bool SliderExist(int id);
        #endregion slider
        #region Publisher
        #endregion
        #region UploadInfo
        public void CreateUploadInfo(UploadInfo uploadInfo);
        public void EditUploadInfo(UploadInfo uploadInfo);
        public void RemoveUploadInfo(int id);
        public Task<List<UploadInfo>> GetUploadInfos();
        public Task<UploadInfo> GetUploadInfo(int id);
        public bool ExistUploadInfo(int id);
        #endregion UploadInfo
        #region UserMessage
        public void CreateUserMessage(UserMessage userMessage);
        public void UpdateUserMessage(UserMessage userMessage);
        public Task<bool> RemoveUserMessage(int Id);
        public Task<List<UserMessage>> GetUserMessagesAsync();
        public bool ExistUserMessage(int Id);
        public Task<UserMessage> GetUserMessage(int Id);
        #endregion UserMessage
        #region Branch
        public void CreateBranch(Branch branch);
        public void CreateBranchCollection(List<Branch> branches);
        public void UpdateBranch(Branch branch);
        public Task<Branch> GetBranchByIdAsync(int Id);
        public Task<List<Branch>> GetBranchesAsync();
        public void RemoveBranch(int Id);
        public bool ExistBranch(int Id);
        #endregion Branch
        #region UsersConversation
        public void CreateConversation(Conversation conversation);
        public void EditConversation(Conversation conversation);
        public Task<Conversation> GetConversationByIdAsync(int Id);
        public Task<List<Conversation>> GetConversationsAsync();
        public bool ExistConversation(int Id);
        public void RemoveConversation(int Id);
        public Task<List<Conversation>> GetTodayConversationsAsync(string code);
        public Task<List<Conversation>> GetConversationsByNameAsync(string code);
        public Task<List<Conversation>> GetConversationsByParentIdAsync(int parentId);
        public Task<List<Conversation>> GetUnreadConversationsByNameAsync(string code);
        public Task<Conversation> GetTopParent_ofConversationAsync(int Id);
        public Task<List<Conversation>> GetParents_ofConversationAsync(int id,List<Conversation> parents);
        public Task<List<Conversation>> GetAllChilds_ofConversationAsync(int Id, List<Conversation> childs);
        #endregion UsersConversation

    }
}
