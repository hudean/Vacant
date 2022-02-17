using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Model.ParamModel;

namespace Vacant.WebApi.Controllers
{
    /// <summary>
    /// 点赞控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;
        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }


        /// <summary>
        /// 分页获取我的点赞列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyLikeByPageAsync([FromQuery] ParamLike param)
        {
            var list = await _likeService.GetPaginatedListAsync(param.UserId, param.PageIndex, param.PageIndex);

            return Ok(list);
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <returns></returns>
        //[HttpDelete("delete")]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelLikeAsync(int id)
        {
            if (!await _likeService.DeleteAsync(id))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// 添加点赞
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddLikeAsync(ParamLikeAdd param)
        {

            if (!await _likeService.InsertAsync(param.UserId, param.RelationId, param.RelationType))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
