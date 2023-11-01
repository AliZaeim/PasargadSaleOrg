using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Security;
using Core.Services.Interfaces;
using DataLayer.Context;
using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;

namespace PLWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NewsGroupsController : Controller
    {

        private readonly INewsService _blogService;
        
        public NewsGroupsController(INewsService blogService)
        {
            
            _blogService = blogService;

        }

        // GET: Admin/NewsGroups
        [PermissionChecker(79)]
        public async Task<IActionResult> Index()
        {
            return View(await _blogService.GetNewsGroupsAsync());
        }



        // GET: Admin/NewsGroups/Create
        [PermissionChecker(80)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NewsGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(80)]
        public async Task<IActionResult> Create(NewsGroup newsGroup)
        {
            if (!ModelState.IsValid)
            {
                return View(newsGroup);
            }
            newsGroup.OP_Create = User.Identity.Name;
            newsGroup.CreateDate = DateTime.Now;
            _blogService.CreateNewsGroup(newsGroup);
            await _blogService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/NewsGroups/Edit/5
       [PermissionChecker(81)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsGroup = await _blogService.GetNewsGroupByIdAsync((int)id);
            if (newsGroup == null)
            {
                return NotFound();
            }
            return View(newsGroup);
        }

        // POST: Admin/NewsGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(81)]
        public async Task<IActionResult> Edit(int id, NewsGroup newsGroup)
        {
            if (id != newsGroup.NewsGroup_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    NewsGroup newsGroupOld = await _blogService.GetNewsGroupByIdAsync(id);
                    string res = _blogService.CompareTwoEntity(newsGroup, newsGroupOld);
                    _blogService.UpdateNewsGroup(newsGroup);
                    _blogService.DoDetached(newsGroupOld);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "NewsGroup",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _blogService.CreateChangeLog(changeLog);
                    }
                    await _blogService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsGroupExists(newsGroup.NewsGroup_Id))
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
            return View(newsGroup);
        }

        // GET: Admin/NewsGroups/Delete/5
        [PermissionChecker(82)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsGroup = await _blogService.GetNewsGroupByIdAsync((int)id);
            if (newsGroup == null)
            {
                return NotFound();
            }

            return View(newsGroup);
        }

        // POST: Admin/NewsGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(82)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newsGroup = await _blogService.GetNewsGroupByIdAsync(id);
            newsGroup.IsDeleted = true;
            newsGroup.RemoveDate = DateTime.Now;
            newsGroup.OP_FakeRemove = User.Identity.Name;
             _blogService.UpdateNewsGroup(newsGroup);
            await _blogService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsGroupExists(int id)
        {
            return _blogService.ExistNewsGroup(id);
        }
    }
}
