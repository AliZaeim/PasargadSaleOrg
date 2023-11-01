using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.DTOs.General;
using Core.Services.Interfaces;
using Core.Utility;
using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;
using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBordroService _bordroService;
        private readonly IUserService _userService;
        private readonly INewsService _newsService;
        private readonly IComplementaryService _complementaryService;
        public HomeController(IBordroService bordroService, IUserService userService, INewsService newsService,IComplementaryService complementaryService)
        {
            _bordroService = bordroService;
            _userService = userService;
            _newsService = newsService;
            _complementaryService = complementaryService;
        }

      public IActionResult Test()
        {
            return View();
            
        }
        public void zTest()
        {
            Conversation conversation = _complementaryService.GetConversationByIdAsync(41).Result;
            var TopP = _complementaryService.GetTopParent_ofConversationAsync(41);
        }
        
        public IActionResult Index()
        {            
            return View();
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }
        [Route("News")]
        [Route("News/{gid?}")]
        public async Task<IActionResult> News(int? page,int? gid)
        {
            NewsVM newsVM = new NewsVM();
            page = page.GetValueOrDefault(1);
            int count = 15;
            if(gid == null)
            {
                newsVM.AllNews = await _newsService.GetNewsAsync();
            }
            else
            {
                newsVM.AllNews = await _newsService.GetNewsByGroupIdAsync((int)gid);
                newsVM.NewsGroup = await _newsService.GetNewsGroupByIdAsync((int)gid);
                newsVM.GId = gid;
            }
            newsVM.NewsGroups = await _newsService.GetNewsGroupsAsync();
            newsVM.PageNews = newsVM.AllNews.Skip(((int)page - 1) * count).Take(count).ToList();
            newsVM.TotalNewsCount = newsVM.AllNews.Count();
            if(newsVM.AllNews.Count % count == 0)
            {
                newsVM.TotalPage = newsVM.AllNews.Count / count;
            }
            else
            {
                newsVM.TotalPage = (newsVM.AllNews.Count / count) + 1;
            }
            newsVM.LastNews = await _newsService.GetLastNewsByCountAsync(5);
            newsVM.CurrentPage =(int) page;
            newsVM.NewsPerPage = 15;
            
            newsVM.Tags = await _newsService.GetMostUsedNewsTags(7);
            
            return View(newsVM);
        }
        [Route("News/d/{code}")]
        public async Task<IActionResult> NewsDetails(string code)
        {
            if(string.IsNullOrEmpty(code))
            {
                return NotFound("کد خبر یافت نشد !");
            }
            News news = await _newsService.GetNewsByCodeAsync(code);
            return View(news);
        }
        [Route("Contact")]
        public IActionResult Contact()
        {
            
            return View(new ContactVM {IsSaved=false });
        }
        [Route("Contact")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactVM contactVM)
        {
            if(!ModelState.IsValid)
            {
                contactVM.IsSaved = false;
                return View(contactVM);
            }
            UserMessage userMessage = new UserMessage()
            {
                CreateDate = DateTime.Now,
                FullName = contactVM.FullName,
                Email = contactVM.Email,
                Subject = contactVM.Subject,
                Message = contactVM.Message

            };
            _complementaryService.CreateUserMessage(userMessage);
            await _complementaryService.SaveChangesAsync();

            
            return View(new ContactVM {IsSaved = true });
        }
        

    }
}