using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLayer.Context;
using DataLayer.Entities.ComplementaryInfo;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Core.Security;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SlidersController : Controller
    {
        
        private readonly IComplementaryService _complementaryService;
        public SlidersController(IComplementaryService complementaryService)
        {
            _complementaryService = complementaryService;
            
        }

        // GET: Admin/Sliders
        [PermissionChecker(92)]
        public async Task<IActionResult> Index()
        {
            return View(await _complementaryService.GetSlidersAsync());
        }

        // GET: Admin/Sliders/Details/5
        [PermissionChecker(95)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _complementaryService.GetSliderByIdAsync((int)id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Create
        [PermissionChecker(93)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(93)]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (ModelState.IsValid)
            {
                slider.CreateDate = DateTime.Now;
                slider.OP_Create = User.Identity.Name;
                _complementaryService.CreateSlider(slider);
                await _complementaryService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Edit/5
        [PermissionChecker(94)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _complementaryService.GetSliderByIdAsync((int)id);
            if (slider == null)
            {
                return NotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(94)]
        public async Task<IActionResult> Edit(int id,  Slider slider)
        {
            if (id != slider.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sliderOld = await _complementaryService.GetSliderByIdAsync(id);
                    string res = _complementaryService.CompareTwoEntity(slider, sliderOld);
                    _complementaryService.DoDetached(sliderOld);
                    _complementaryService.UpdateSlider(slider);
                    if (!string.IsNullOrEmpty(res))
                    {
                        ChangeLog changeLog = new ChangeLog()
                        {
                            ChangedBy = User.Identity.Name,
                            DateChanged = DateTime.Now,
                            EntityName = "Slider",
                            PrimaryKeyValue = id.ToString(),
                            Description = res
                        };
                        _complementaryService.CreateChangeLog(changeLog);
                    }
                    await _complementaryService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SliderExists(slider.Id))
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
            return View(slider);
        }

        // GET: Admin/Sliders/Delete/5
        [PermissionChecker(96)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slider = await _complementaryService.GetSliderByIdAsync((int)id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [PermissionChecker(96)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slider = await _complementaryService.GetSliderByIdAsync(id);
            slider.RemoveDate = DateTime.Now;
            slider.IsDeleted = true;
            slider.OP_FakeRemove = User.Identity.Name;
            _complementaryService.UpdateSlider(slider);
            await _complementaryService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return _complementaryService.SliderExist(id);
        }
    }
}
