using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Core.Generators;
using Core.Security;
using Core.Services.Interfaces;

using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;

namespace PLWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NewsController : Controller
    {

        private readonly INewsService _blogService;
        public NewsController(INewsService blogService)
        {
            _blogService = blogService;

        }

        // GET: Admin/News
        [PermissionChecker(87)]
        public async Task<IActionResult> Index()
        {

            return View(await _blogService.GetNewsAsync());
        }

        // GET: Admin/News/Details/5
        [PermissionChecker(90)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _blogService.GetNewsByIdAsync((int)id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/News/Create
        [PermissionChecker(88)]
        public async Task<IActionResult> Create()
        {
            ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title");
            ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title");
            List<string> codes = _blogService.GetNewsAsync().Result.Select(s => s.News_Code).ToList();
            News news = new News()
            {
                News_Date = DateTime.Now,
                News_Code = GeneratorClass.GenerateKey(codes, 4)

            };
            return View(news);
        }

        // POST: Admin/News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(88)]
        public async Task<IActionResult> Create(News news, IFormFile News_Image)
        {
            if (!ModelState.IsValid)
            {
                ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title", news.NewsGroup_Id);
                ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title", news.Publisher_Id);
                return View(news);
            }
            string NFileName = string.Empty;
            if (News_Image != null)
            {
                if (News_Image.Length > .05 * 1024 * 1024)
                {
                    ModelState.AddModelError("News_Image", "حجم تصویر بیشتر از 50 کیلو بایت است !");
                    ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title", news.NewsGroup_Id);
                    ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title", news.Publisher_Id);
                    return View(news);
                }
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", News_Image.FileName);
                string newsfileName = GeneratorClass.GenerateUniqueCode() + Path.GetExtension(News_Image.FileName);
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", newsfileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await News_Image.CopyToAsync(stream);
                }
                NFileName = newsfileName;
            }
            
            news.News_Image = NFileName;
            news.OP_Create = User.Identity.Name;
            news.CreateDate = DateTime.Now;
            _blogService.CreateNews(news);
            await _blogService.SaveChangesAsync();
            

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/News/Edit/5
        [PermissionChecker(89)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _blogService.GetNewsByIdAsync((int)id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title", news.NewsGroup_Id);
            ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title", news.Publisher_Id);
            return View(news);
        }

        // POST: Admin/News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(89)]
        public async Task<IActionResult> Edit(int id, News news, IFormFile News_Image)
        {
            if (id != news.News_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (News_Image != null)
                    {
                        if (News_Image.Length > .05 * 1024 * 1024)
                        {
                            ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title", news.NewsGroup_Id);
                            ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title", news.Publisher_Id);
                            ModelState.AddModelError("News_Image", "حجم تصویر بیشتر از 50 کیلو بایت است !");
                            return View(news);
                        }

                    }
                    if (News_Image != null)
                    {
                        string curimgFile = news.News_Image;
                        if (!string.IsNullOrEmpty(curimgFile))
                        {
                            string currentimgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", curimgFile);
                            if (System.IO.File.Exists(currentimgPath))
                            {
                                System.IO.File.Delete(currentimgPath);
                            }
                        }

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", News_Image.FileName);
                        string newsfileName = GeneratorClass.GenerateUniqueCode() + Path.GetExtension(News_Image.FileName);
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/blogs", newsfileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await News_Image.CopyToAsync(stream);
                        }
                        news.News_Image = newsfileName;
                    }
                    News newsOld = await _blogService.GetNewsByIdAsync(id);
                    string res = _blogService.CompareTwoEntity(news, newsOld);
                    _blogService.DoDetached(newsOld);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "News",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _blogService.CreateChangeLog(changeLog);
                    }
                    _blogService.UpdateNews(news);
                    await _blogService.SaveChangesAsync();
                    

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.News_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NewsGroup_Id"] = new SelectList(await _blogService.GetNewsGroupsAsync(), "NewsGroup_Id", "NewsGroup_Title", news.NewsGroup_Id);
            ViewData["Publisher_Id"] = new SelectList(await _blogService.GetPublishersAsync(), "Publisher_Id", "Publisher_Title", news.Publisher_Id);
            return View(news);
        }

        // GET: Admin/News/Delete/5
        [PermissionChecker(91)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _blogService.GetNewsByIdAsync((int)id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(91)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            News news = await _blogService.GetNewsByIdAsync(id);
            news.IsDeleted = true;
            news.OP_Remove = User.Identity.Name;
            _blogService.UpdateNews(news);
            await _blogService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        
        private bool NewsExists(int id)
        {
            return _blogService.ExistNews(id);
        }
    }
}
