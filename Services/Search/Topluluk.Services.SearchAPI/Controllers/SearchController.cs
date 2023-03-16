using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Shared.BaseModels;
using Topluluk.Shared.Dtos;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Topluluk.Services.SearchAPI.Controllers
{
    
    public class SearchController : BaseController
    {
        [HttpGet]
        public async Task<Response<string>> index() {
            return new();
        }
    }
}

