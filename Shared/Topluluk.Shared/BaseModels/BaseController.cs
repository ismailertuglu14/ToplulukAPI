using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Shared.Helper;

namespace Topluluk.Shared.BaseModels
{
    [EnableCors("MyPolicy")]
	public class BaseController : ControllerBase
	{
		public string UserName { get { return GetUserName(); } }
		public string Token { get { return GetRequestToken(); } }
        public string UserId { get { return GetUserId(); } }
        
        [NonAction]
        private string GetUserName()
        {
            return TokenHelper.GetUserNameByToken(Request);
        }

        [NonAction]
        private string GetUserId()
        {
            return TokenHelper.GetUserIdByToken(Request);
        }
        [NonAction]
        private string GetRequestToken()
        {
            return TokenHelper.GetToken(Request);
        }

        public BaseController()
        {
        }
    }
}

