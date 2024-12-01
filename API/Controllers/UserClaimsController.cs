using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClaimsController : ControllerBase
    {
        public UserClaimsController() {  }

        //public async Task<IActionResult> GetsClaim(string key, int offset, int limit = 20, string sortField = "", string sortOrder = "asc")
        //{

        //    return 1;
        //}
    }
}
