using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Admin.ViewComponents
{
    public class ChildUsers:ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(int urId)
        {
            return await Task.FromResult((IViewComponentResult)View("Childs",urId));
        }
        

    }
}
