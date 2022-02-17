using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Model.ParamModel;

namespace Vacant.WebApi.Controllers
{
    /// <summary>
    /// 收藏控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {

        private readonly ICollectionService _collectionService;
        public CollectionController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }



        /// <summary>
        /// 分页获取我的收藏列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyCollectionByPageAsync([FromQuery] ParamLike param)
        {
            var list = await _collectionService.GetPaginatedListAsync(param.UserId, param.PageIndex, param.PageIndex);

            return Ok(list);
        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <returns></returns>
        //[HttpDelete("delete")]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelCollectionAsync(int id)
        {
            if (!await _collectionService.DeleteAsync(id))
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddCollectionAsync(ParamLikeAdd param)
        {

            if (!await _collectionService.InsertAsync(param.UserId, param.RelationId, param.RelationType))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
