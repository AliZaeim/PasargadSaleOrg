using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Core.Services.Interfaces;
using DataLayer.Entities.Blogs;
using DataLayer.Entities.ComplementaryInfo;
using Core.Security;

namespace PLWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PublishersController : Controller
    {
        
        private readonly INewsService _blogService;
       
        public PublishersController(INewsService blogService)
        {
           
            _blogService = blogService;
            
        }

        // GET: Admin/Publishers
        [PermissionChecker(83)]
        public async Task<IActionResult> Index()
        {
            return View(await _blogService.GetPublishersAsync());
        }

        

        // GET: Admin/Publishers/Create
        [PermissionChecker(84)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Publishers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(84)]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                publisher.CreateDate = DateTime.Now;
                publisher.OP_Create = User.Identity.Name;
                _blogService.CreatePublisher(publisher);
                await _blogService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        // GET: Admin/Publishers/Edit/5
        [PermissionChecker(85)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _blogService.GetPublisherByIdAsync((int)id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Admin/Publishers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(85)]
        public async Task<IActionResult> Edit(int id, Publisher publisher)
        {
            if (id != publisher.Publisher_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Publisher publisherOld = await _blogService.GetPublisherByIdAsync(id);
                    string res = _blogService.CompareTwoEntity(publisher, publisherOld);
                    _blogService.UpdatePublisher(publisher);
                    _blogService.DoDetached(publisherOld);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "Publisher",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _blogService.CreateChangeLog(changeLog);
                    }
                    await _blogService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.Publisher_Id))
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
            return View(publisher);
        }

        // GET: Admin/Publishers/Delete/5
        [PermissionChecker(86)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _blogService.GetPublisherByIdAsync((int)id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Admin/Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(86)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisher = await _blogService.GetPublisherByIdAsync(id);
            publisher.IsDeleted = true;
            publisher.RemoveDate = DateTime.Now;
            publisher.OP_FakeRemove = User.Identity.Name;
            _blogService.UpdatePublisher(publisher);
            await _blogService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublisherExists(int id)
        {
            return _blogService.ExistsPublisher(id);
        }
    }
}
