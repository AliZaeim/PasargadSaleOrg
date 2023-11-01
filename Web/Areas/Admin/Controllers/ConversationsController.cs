using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Core.DTOs.Admin;
using Core.Services.Interfaces;
using DataLayer.Entities.ComplementaryInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.VisualBasic;
using NPOI.HSSF.Record;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ConversationsController : Controller
    {
        private readonly IComplementaryService _complementaryService;
        private readonly IUserService _userService;
        public ConversationsController(IComplementaryService complementaryService, IUserService userService)
        {
            _complementaryService = complementaryService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {

            var list = await _complementaryService.GetConversationsByNameAsync(User.Identity.Name);
            return View(list);
        }
        //Conversation is Main Conv and parentId = null
        public async Task<IActionResult> ShowConv(int Id)
        {
            Conversation conversation = await _complementaryService.GetConversationByIdAsync(Id);

            if (conversation == null)
            {
                return NotFound();
            }
            List<string> readers = null;
            if(!string.IsNullOrEmpty(conversation.Readers))
            {
                readers = conversation.ReadersList.ToList();
            }
            List<string> receps = null;
            if(!string.IsNullOrEmpty(conversation.RecepiesInfo))
            {
                receps = conversation.RecepiesList.ToList();
            }
            if (receps.Any(a => a.Substring(0, a.IndexOf("-")) == User.Identity.Name))
            {
                if(!string.IsNullOrEmpty(conversation.Readers))
                {
                    if (!readers.Any(a => a.Substring(0, a.IndexOf("-")) == User.Identity.Name))
                    {
                        await AddReaderToConv(Id);
                    }
                }
                else
                {
                    await AddReaderToConv(Id);
                }
                
            }
            
            if (conversation.ParentId != null)
            {
                conversation = await _complementaryService.GetTopParent_ofConversationAsync(Id);
            }
            if(User.Identity.Name !="290070")
            {
                //if (conversation.SenderCode != User.Identity.Name)
                //{
                //    return NotFound("مجاز به مشاهده اطلاعات پیام نمی باشید !");
                //}
                if(!receps.Any(a => a.Substring(0,a.IndexOf("-"))==User.Identity.Name || conversation.SenderCode !=User.Identity.Name))
                {
                    return NotFound("مجاز به مشاهده اطلاعات پیام نمی باشید !");
                }
            }
            
            return View(conversation);

        }
        public async Task<IActionResult> Create(int? pId, int? repId)
        {
            string recinfo = string.Empty;
            if(repId != null)
            {
                Conversation repConversaion = await _complementaryService.GetConversationByIdAsync((int)repId);
                if(repConversaion != null)
                {
                    recinfo = repConversaion.SenderCode + "-" + repConversaion.SenderFullPro;
                }
            }
            ConversationVM conversationVM = new ConversationVM()
            {
                ParentId = pId,


            };
            
            if (pId == null)
            {
                if (string.IsNullOrEmpty(recinfo))
                {
                    if (User.Identity.Name == "290070")
                    {
                        conversationVM.Users = await _userService.GetUsersActive_and_HasActiveRoleAsync();
                    }
                    else
                    {
                        conversationVM.UserInfos.Add("290070-بیمه پاسارگاد");
                    }
                }
                else
                {
                    conversationVM.UserInfos.Add(recinfo);
                }
            }
            else
            {

                Conversation conversation = await _complementaryService.GetConversationByIdAsync((int)pId);
                if (string.IsNullOrEmpty(recinfo))               {                    
                        recinfo = conversation.RecepiesInfo;
                        conversationVM.UserInfos.Add(recinfo);              
                 
                }
                else
                {
                    conversationVM.UserInfos.Add(recinfo);
                }
                
                conversationVM.Subject = conversation.Subject;
                conversationVM.Title = "<h4 class='text-xs-center alert alert-success'>" + "پاسخ به پیام " + "<br />" + conversation.SenderFullPro + "<br />" + "با موضوع " + conversation.Subject + "</h4>";

            }

            if (repId != null)
            {
                Conversation rep = await _complementaryService.GetConversationByIdAsync((int)repId);
                conversationVM.ParentId = repId;
                conversationVM.Title = "<h4 class='text-xs-center alert alert-success'>" + "پاسخ به پیام " + "<br/>" + rep.SenderFullPro + "<br />" + "با موضوع " + rep.Subject + "</h4>";
            }
            return View(conversationVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConversationVM coversationVM)
        {
            if (!ModelState.IsValid)
            {
                if (User.Identity.Name == "290070")
                {
                    coversationVM.Users = await _userService.GetUsersActive_and_HasActiveRoleAsync();
                }
                else
                {
                    coversationVM.Users.Add(await _userService.GetUserByCode("290070"));
                }
                return View(coversationVM);
            }
            List<DataLayer.Entities.User.UserRole> userRoles = await _userService.GetUserRolesByUserCode(User.Identity.Name);
            DataLayer.Entities.User.UserRole ActiveUserRole = userRoles.FirstOrDefault(f => f.IsActive);
            Conversation conversation = new Conversation()
            {
                Subject = coversationVM.Subject,
                Message = coversationVM.Message,
                SenderCode = User.Identity.Name,
                SenderFullPro = ActiveUserRole.FullPro,
                CreateDate = DateTime.Now,
                ParentId = coversationVM.ParentId,
                IsActive = true,

            };
            string RecInfo = string.Empty;
            foreach (var item in coversationVM.UserInfos)
            {
                if (item != coversationVM.UserInfos.LastOrDefault())
                {
                    RecInfo += item + Environment.NewLine;
                }
                else
                {
                    RecInfo += item;
                }
            }
            conversation.RecepiesInfo = RecInfo;
            _complementaryService.CreateConversation(conversation);
            await _complementaryService.SaveChangesAsync();
            if (conversation.ParentId == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("ShowConv", new { Id = (int)conversation.ParentId });
            }

        }
       
        public async Task<IActionResult> Details(int? parentId, int id)
        {
            Conversation conversation = null;
            if (parentId == null)
            {
                conversation = await _complementaryService.GetConversationByIdAsync(id);
            }
            else
            {
                conversation = await _complementaryService.GetConversationByIdAsync((int)parentId);
            }
            if (conversation == null)
            {
                return NotFound("پیامی موجود نمی باشد !");
            }
            bool edit = false;
            List<string> receps = conversation.RecepiesList.ToList();
            receps = receps.Where(w => !string.IsNullOrEmpty(w)).ToList();

            if (conversation.SenderCode != User.Identity.Name)
            {
                if (!string.IsNullOrEmpty(conversation.RecepiesInfo))
                {
                    if (receps.Any(a => a.Substring(0, a.IndexOf("-")) == User.Identity.Name))
                    {
                        if (!string.IsNullOrEmpty(conversation.Readers))
                        {
                            List<string> readers = conversation.ReadersList.ToList();
                            readers = readers.Where(w => !string.IsNullOrEmpty(w)).ToList();

                            if (!readers.Any(a => a.Substring(0, a.IndexOf("-")) == User.Identity.Name))
                            {
                                conversation.Readers += Environment.NewLine + User.Identity.Name + "-" + DateTime.Now;
                                edit = true;
                            }

                        }
                        else
                        {
                            conversation.Readers += User.Identity.Name + "-" + DateTime.Now;
                            edit = true;
                        }
                    }
                }

                if (edit == true)
                {
                    _complementaryService.EditConversation(conversation);
                    await _complementaryService.SaveChangesAsync();
                }
            }
            ViewData["id"] = id;

            return View(conversation);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _complementaryService.GetConversationByIdAsync((int)id);
            if (conversation == null)
            {
                return NotFound();
            }
            if(conversation.SenderCode != User.Identity.Name)
            {
                return NotFound("پیام قابل ویرایش نیست !");
            }
            int? parentId = conversation.ParentId;
            List<string> RecepsCode = conversation.RecepiesList.ToList().Select(x => x.Split("-")[0]).ToList();
            ConversationVM conversationVM = new ConversationVM()
            {
                ParentId = conversation.ParentId,
                SelectedRecepsCode = RecepsCode

            };
            if (conversation.ParentId == null)
            {
                if (User.Identity.Name == "290070")
                {
                    conversationVM.Users = await _userService.GetUsersActive_and_HasActiveRoleAsync();
                }
                else
                {
                    conversationVM.UserInfos.Add("290070-بیمه پاسارگاد");
                }
            }
            else
            {
                Conversation Pconversation = await _complementaryService.GetConversationByIdAsync((int)conversation.ParentId);
                conversationVM.UserInfos.Add(Pconversation.RecepiesInfo);
                
            }
            conversationVM.Subject = conversation.Subject;
            conversationVM.Message = conversation.Message;

            return View(conversationVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConversationVM conversationVM)
        {
            var conversation = await _complementaryService.GetConversationByIdAsync(conversationVM.Id);
            if (!ModelState.IsValid)
            {
                string recinfo = string.Empty;
               
                int? parentId = conversation.ParentId;
                List<string> RecepsCode = conversation.RecepiesList.ToList().Select(x => x.Split("-")[0]).ToList();
                conversationVM.SelectedRecepsCode = RecepsCode;
                if (conversation.ParentId == null)
                {
                    if (User.Identity.Name == "290070")
                    {
                        conversationVM.Users = await _userService.GetUsersActive_and_HasActiveRoleAsync();
                    }
                    else
                    {
                        conversationVM.UserInfos.Add("290070-بیمه پاسارگاد");
                    }
                }
                else
                {

                    conversationVM.UserInfos.Add(conversation.SenderCode + "-" + conversation.SenderFullPro);
                    conversationVM.Subject = conversation.Subject;
                }
                return View(conversationVM);
            }
            Conversation EConversation = await _complementaryService.GetConversationByIdAsync(conversationVM.Id);

            EConversation.Subject = conversationVM.Subject;
            EConversation.Message = conversationVM.Message;

            string RecInfo = string.Empty;
            foreach (var item in conversationVM.UserInfos)
            {
                if (item != conversationVM.UserInfos.LastOrDefault())
                {
                    RecInfo += item + Environment.NewLine;
                }
                else
                {
                    RecInfo += item;
                }
            }
            EConversation.RecepiesInfo = RecInfo;
            _complementaryService.EditConversation(EConversation);
            await _complementaryService.SaveChangesAsync();
            if (conversation.ParentId == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("ShowConv", new { Id = (int)EConversation.ParentId });
            }

        
           
        }
        public async Task<IActionResult> GetConversation(int Id)
        {
            Conversation conversation = await _complementaryService.GetConversationByIdAsync(Id);
            return PartialView(conversation);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _complementaryService.GetConversationByIdAsync((int)id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversation = await _complementaryService.GetConversationByIdAsync(id);

            var Topconversation = await _complementaryService.GetTopParent_ofConversationAsync(id);

            conversation.IsDeleted = true;
            _complementaryService.EditConversation(conversation);
            await _complementaryService.SaveChangesAsync();
            if (conversation.ParentId == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(ShowConv), Topconversation.Id);
        }
        [HttpPost]
        public async Task<bool> RemoveConv(int id)
        {
            var conversation = await _complementaryService.GetConversationByIdAsync(id);
            if (conversation == null)
            {
                return false;
            }
            conversation.IsDeleted = true;
            _complementaryService.EditConversation(conversation);
            await _complementaryService.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddReaderToConv(int id)
        {
            Conversation conversation = await _complementaryService.GetConversationByIdAsync(id);
            if (conversation == null)
            {
                return false;
            }
            bool edit = false;
            if (conversation.SenderCode != User.Identity.Name)
            {
                if (!string.IsNullOrEmpty(conversation.Readers))
                {
                    List<string> readers = conversation.ReadersList.ToList();
                    readers = readers.Where(w => !string.IsNullOrEmpty(w)).ToList();
                    if (!readers.Any(a => a.Substring(0, a.IndexOf("-")) == User.Identity.Name))
                    {
                        conversation.Readers += Environment.NewLine + User.Identity.Name + "-" + DateTime.Now;
                        edit = true;
                    }
                }
                else
                {
                    conversation.Readers += User.Identity.Name + "-" + DateTime.Now;
                    edit = true;
                }
            }
            if (edit == true)
            {
                _complementaryService.EditConversation(conversation);
                await _complementaryService.SaveChangesAsync();
                return true;
            }
            return false;


        }
        public async Task<IActionResult> ShowReaders(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var conversation = await _complementaryService.GetConversationByIdAsync((int)id);
            if (conversation == null)
            {
                return NotFound();
            }
            return View(conversation);
        }
        public async Task<IActionResult> ShowReceps(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var conversation = await _complementaryService.GetConversationByIdAsync((int)id);
            if (conversation == null)
            {
                return NotFound();
            }
            return View(conversation);
        }
        public async Task<int> GetUnreadConvCount()
        {
            List<Conversation> UnreadInnerMessages = await _complementaryService.GetUnreadConversationsByNameAsync(User.Identity.Name);
            if (UnreadInnerMessages == null)
            {
                return 0;
            }
            return UnreadInnerMessages.Count();
        }
    }
}
