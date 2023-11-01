using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Security;
using Core.Services.Interfaces;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserMessagesController : Controller
    {
        private IComplementaryService _complementaryService;
        public UserMessagesController(IComplementaryService complementaryService)
        {
            _complementaryService = complementaryService;
        }
        [PermissionChecker(97)]
        public async Task<IActionResult> Index()
        {
            return View(await _complementaryService.GetUserMessagesAsync());
        }
        [PermissionChecker(98)]
        public async Task<IActionResult> Edit(int? Id)
        {
            UserMessage userMessage = await _complementaryService.GetUserMessage((int)Id);
            if(userMessage == null)
            {
                return NotFound();
            }
            return View(userMessage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker(98)]
        public async Task<IActionResult> Edit(UserMessage userMessage)
        {
            if(!ModelState.IsValid)
            {
                return View(userMessage);
            }
            _complementaryService.UpdateUserMessage(userMessage);
            await _complementaryService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
