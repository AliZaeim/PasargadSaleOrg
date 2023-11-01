using Core.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class NewsService:INewsService
    {
        private readonly MyContext _MyContext;



        public NewsService(MyContext MyContext)
        {
            _MyContext = MyContext;
        }
        #region General
        public void SaveChanges()
        {
            _MyContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _MyContext.SaveChangesAsync();
        }
        public void DoDetached<T>(T entity)
        {
            _MyContext.Entry(entity).State = EntityState.Detached;
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
            _MyContext.ChangeLogs.Add(changeLog);
        }
        #endregion
        #region NewsGroup
        public void CreateNewsGroup(NewsGroup newsGroup)
        {
            _MyContext.NewsGroups.Add(newsGroup);
        }

        public async Task CreateNewsGroupAsync(NewsGroup newsGroup)
        {
            await _MyContext.NewsGroups.AddAsync(newsGroup);
        }

        public async Task<NewsGroup> GetNewsGroupByIdAsync(int id)
        {
            return await _MyContext.NewsGroups.Include(r => r.News)
                .SingleOrDefaultAsync(s => s.NewsGroup_Id == id);
        }

        public async Task<List<NewsGroup>> GetNewsGroupsAsync()
        {
            return await _MyContext.NewsGroups.Include(r => r.News).ToListAsync();
        }

        public async Task RemoveNewsGroup(int id)
        {
            NewsGroup newsGroup = await _MyContext.NewsGroups.FindAsync(id);
            newsGroup.IsDeleted = true;
            _MyContext.NewsGroups.Update(newsGroup);
        }

        public void UpdateNewsGroup(NewsGroup newsGroup)
        {
            _MyContext.NewsGroups.Update(newsGroup);
        }
        public bool ExistNewsGroup(int id)
        {
            return _MyContext.NewsGroups.Any(a => a.NewsGroup_Id == id);
        }

        #endregion NewsGroup
        #region Publisher
        public async Task<List<Publisher>> GetPublishersAsync()
        {
            return await _MyContext.Publishers.Include(r => r.News).ToListAsync();
        }
        public void CreatePublisher(Publisher publisher)
        {
            _MyContext.Publishers.Add(publisher);
        }

        public void UpdatePublisher(Publisher publisher)
        {
            _MyContext.Publishers.Update(publisher);
        }

        public void RemovePublisher(int id)
        {
            Publisher publisher = _MyContext.Publishers.Find(id);
            _MyContext.Publishers.Remove(publisher);
        }

        public async Task<Publisher> GetPublisherByIdAsync(int id)
        {
            return await _MyContext.Publishers.Include(r => r.News).SingleOrDefaultAsync(s => s.Publisher_Id == id);
        }
        public bool ExistsPublisher(int id)
        {
            return _MyContext.Publishers.Any(a => a.Publisher_Id == id);
        }
        #endregion
        #region News
        public void CreateNews(News news)
        {
            _MyContext.News.Add(news);
        }

        public async Task CreageNewsAsync(News news)
        {
            await _MyContext.News.AddAsync(news);
        }

        public void UpdateNews(News news)
        {
            _MyContext.News.Update(news);
        }

        public async Task<News> GetNewsByIdAsync(int id)
        {
            return await _MyContext.News.Include(r => r.NewsGroup)
                .SingleOrDefaultAsync(s => s.News_Id == id);

        }
        public async Task<List<News>> GetNewsAsync()
        {
            return await _MyContext.News.Include(r => r.NewsGroup).Include(r => r.Publisher).ToListAsync();
        }
        public async Task RemoveNews(int id)
        {
            News news = await _MyContext.News.FindAsync(id);
            news.IsDeleted = true;
            _MyContext.News.Update(news);
        }

        public bool ExistNews(int id)
        {
            return _MyContext.News.Any(a => a.News_Id == id);
        }
        public async Task<News> GetNewsByCodeAsync(string code)
        {
            return await _MyContext.News.Include(r => r.NewsGroup).Include(r => r.Publisher)
                .SingleOrDefaultAsync(s => s.News_Code.Trim() == code);
        }
        public async Task<List<News>> SearchNews(string serch)
        {
            string srch = "%" + serch + "%";
            List<News> news = await _MyContext.News.Include(r => r.NewsGroup).Include(r => r.Publisher).ToListAsync();
            news = news.Where(w => w.News_Text.Contains(serch) || w.NewsGroup.NewsGroup_Title.Contains(serch) || w.News_Title.Contains(serch)
            || w.News_Abstract.Contains(serch) || w.Publisher.Publisher_Title.Contains(serch) || w.NewsGroup.NewsGroup_Title.Contains(serch)
            || w.News_Tags.Contains(serch))
                .ToList();

            return news;
        }

        public async Task<List<News>> GetNewsByGroupIdAsync(int gid)
        {
            return await _MyContext.News.Include(r => r.NewsGroup).Include(r => r.Publisher)
                .Where(w => w.NewsGroup_Id == gid).ToListAsync();
        }

        public async Task<List<News>> GetLastNewsByCountAsync(int count)
        {
            return await _MyContext.News.Include(r => r.NewsGroup).Include(r => r.Publisher)
                .OrderByDescending(r => r.News_Date).Take(count).ToListAsync();
        }

        public async Task<List<string>> GetMostUsedNewsTags(int? count)
        {
            int MCount = count.GetValueOrDefault(5);
            List<News> news = await _MyContext.News.ToListAsync();            
            //make list<string> from List<List<string>> evry news has list<string> of tags 
            List<string> Tags = news.SelectMany(s => s.TagsList).ToList();            
            List<string> Most = Tags.GroupBy(x => x.Trim()).OrderByDescending(x => x.Count()).Select(g => g.Key).Distinct().Take(MCount).ToList();
            
           
            return Most;
        }

        public async Task<List<News>> GetNewsByPagination(int page, int count)
        {
            return await _MyContext.News.Skip((page - 1) * count).Take(count).ToListAsync();
        }
        #endregion News
    }
}
