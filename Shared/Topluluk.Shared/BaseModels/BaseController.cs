using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Topluluk.Shared.Helper;

namespace Topluluk.Shared.BaseModels
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("MyPolicy")]
	public class BaseController : ControllerBase
	{
		public string UserName { get { return GetUserName(); } }
		public string Token { get { return GetRequestToken(); } }
        public string UserId { get { return GetUserId(); } }

        public List<string> Roles
        {
            get { return GetRoles(); }
        }

        [NonAction]
        private List<string> GetRoles()
        {
            return TokenHelper.GetUserRolesByToken(Request);
        }
        
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

