using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vacant.WebApi.Controllers
{
    /// <summary>
    /// 一级评论控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        public CommentController()
        { 
        }
    }
}
