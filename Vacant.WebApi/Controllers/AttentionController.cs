using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Model.ParamModel;

namespace Vacant.WebApi.Controllers
{
    /// <summary>
    /// 关注控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AttentionController : ControllerBase
    {
        private readonly IAttentionService _attentionService;
        public AttentionController(IAttentionService attentionService)
        {
            _attentionService = attentionService;
        }

        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync([FromQuery] ParamAttention param)
        {
            var list = await _attentionService.GetPaginatedListAsync(param.UserId, param.PageIndex, param.PageIndex);

            return Ok(list);
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <returns></returns>
        //[HttpDelete("delete")]
        [HttpPost("delete")]
        public async Task<IActionResult> CancelAttentionAsync(int id)
        {
            if (!await _attentionService.DeleteAsync(id))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// 新增关注
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddAttentionAsync(ParamAttentionAdd param)
        {

            if (!await _attentionService.InsertAsync(param.UserId, param.FollowedUserId))
            {
                return BadRequest();
            }

            return Ok();
        }

    }
}
