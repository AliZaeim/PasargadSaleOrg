
using Core.DTOs.General;
using DataLayer.Entities.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Areas.Admin.ViewComponents
{
    public class Childerens : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync(ShowChilderensVM showChilderensVM)
        {            
            return await Task.FromResult((IViewComponentResult)View("AllChilds", showChilderensVM));
        }
    }
}
