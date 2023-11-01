using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    public interface INewsService
    {
        #region General
        public void SaveChanges();
        public Task SaveChangesAsync();
        public void DoDetached<T>(T entity);
        public string CompareTwoEntity<T>(T entity1, T entity2);
        public void CreateChangeLog(ChangeLog changeLog);
        #endregion General
        #region NewsGroup
        public Task CreateNewsGroupAsync(NewsGroup newsGroup);
        public void CreateNewsGroup(NewsGroup newsGroup);
        public void UpdateNewsGroup(NewsGroup newsGroup);
        public Task<NewsGroup> GetNewsGroupByIdAsync(int id);
        public Task<List<NewsGroup>> GetNewsGroupsAsync();
        public Task RemoveNewsGroup(int id);
        public bool ExistNewsGroup(int id);
        #endregion NewsGroup
        #region Publisher
        public Task<List<Publisher>> GetPublishersAsync();
        public void CreatePublisher(Publisher publisher);
        public void UpdatePublisher(Publisher publisher);
        public void RemovePublisher(int id);
        public bool ExistsPublisher(int id);
        public Task<Publisher> GetPublisherByIdAsync(int id);
        #endregion
        #region News
        public void CreateNews(News news);
        public Task CreageNewsAsync(News news);
        public void UpdateNews(News news);
        public Task<List<News>> GetNewsAsync();
        public Task<List<News>> GetNewsByPagination(int page, int count);
        public Task<News> GetNewsByIdAsync(int id);
        public Task<News> GetNewsByCodeAsync(string code);
        public Task RemoveNews(int id);
        public bool ExistNews(int id);
        public Task<List<News>> GetNewsByGroupIdAsync(int gid);
        public Task<List<News>> GetLastNewsByCountAsync(int count);
        public Task<List<string>> GetMostUsedNewsTags(int? count);

        public Task<List<News>> SearchNews(string serch);
        #endregion News
    }
}
