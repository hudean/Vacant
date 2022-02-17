using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.Domain.Entites;
using Vacant.Model.ParamModel;

namespace Vacant.WebApi.Controllers
{
    /// <summary>
    /// 文章控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        public readonly IArticleService _articleService;
        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 分页获取文章列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetArticlesByPageAsyncc([FromQuery] ParamArticle param)
        {
            var list = await _articleService.GetPaginatedListAsync(param.KeyWord, param.SortType, param.TopickTypeId, param.ArticleTypeId, param.PageIndex, param.PageIndex);

            return Ok(list);
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<IActionResult> GetArticleDetailAsync(int userId, int id)
        {
            var model = await _articleService.DetailAsync( id);
            return Ok(model);
        }


        /// <summary>
        /// 分页获取医生发表的文章列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("GetPaginatedListByUserIdAsync")]
        public async Task<IActionResult> GetPaginatedListByUserIdAsync([FromQuery] ParamArticleAndCourse param)
        {
            return Ok(await _articleService.GetPaginatedListByUserIdAsync(param.UserId, param.PageIndex, param.PageSize));
        }

        /// <summary>
        /// 我的发布文章列表（医生自己发布的文章列表）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("GetMyPublishPaginatedListByUserIdAsync")]
        public async Task<IActionResult> GetMyPublishPaginatedListByUserIdAsync([FromQuery] ParamArticleAndCourse param)
        {
            return Ok(await _articleService.GetMyPublishPaginatedListByUserIdAsync(param.UserId, param.PageIndex, param.PageSize));
        }

        ///// <summary>
        ///// 添加或修改文章
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("addOrEdit")]
        //public async Task<IActionResult> AddOrEditArticleAsync([FromForm] ParamArticleModel param)
        //{
        //    #region 原先的上传功能微信不支持

        //    //if (param.DoctorId > 0)
        //    //{
        //    //    string uploadFileUrl = "";

        //    //    if (await _articleService.ExistArticleAsync(param.Id,param.Title))
        //    //    {
        //    //        return _articleService.Error((int)HttpStatusCode.InternalServerError, "当前数据已存在，不能重复添加");
        //    //    }

        //    //    if (param.Id > 0)
        //    //    {
        //    //        Article model = await _articleService.GetArticleAsync(param.Id.Value, param.DoctorId);
        //    //        if (model == null && model.Id <= 0)
        //    //        {
        //    //            return _articleService.Error((int)HttpStatusCode.InternalServerError, "该用户发布文章不存在");
        //    //        }
        //    //        model.ArticleTypeId = param.ArticleTypeId;
        //    //        model.Title = param.Title;
        //    //        model.Content = param.Content;
        //    //        model.ArticleCategory = param.ArticleCategory;
        //    //        model.TopicTypeId = param.TopicTypeId;
        //    //        model.UpdatedDate = DateTime.Now;
        //    //        if (param.IsEditImg)
        //    //        {
        //    //            if (param.FormFile != null && param.FormFile.Length > 0)
        //    //            {
        //    //                var file = param.FormFile;
        //    //                try
        //    //                {
        //    //                    string fileName = file.FileName.Trim().Substring(0, file.FileName.Length - Path.GetExtension(file.FileName).Length);
        //    //                    //新的文件名
        //    //                    string newFileName = fileName + DateTime.Now.ToString("_yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + Path.GetExtension(file.FileName);
        //    //                    //文件上传路径
        //    //                    //string filePath = _hostingEnv.WebRootPath + "/images/Article/";
        //    //                    string webRootPath =string.IsNullOrEmpty(_configuration["UploadFilePath"])? _hostingEnv.WebRootPath : _configuration["UploadFilePath"];
        //    //                    string filePath = webRootPath + "/images/Article/";
        //    //                    //图片网路地址
        //    //                    string fileUrl = _configuration["ImgDomain"] + "/Article/";
        //    //                    if (!Directory.Exists(filePath))
        //    //                    {
        //    //                        Directory.CreateDirectory(filePath);
        //    //                    }
        //    //                    string uploadFilePath = filePath + $@"{newFileName}";//指定文件上传路径
        //    //                    uploadFileUrl = fileUrl + $@"{newFileName}";//指定文件上传路径
        //    //                    using (FileStream fs = System.IO.File.Create(uploadFilePath))//创建文件流
        //    //                    {
        //    //                        file.CopyTo(fs);//将上载文件的内容复制到目标流
        //    //                        fs.Flush();//清除此流的缓冲区并导致将任何缓冲数据写入
        //    //                    }
        //    //                    if (!string.IsNullOrEmpty(model.PicUrl))
        //    //                    {
        //    //                        var oldFileName = model.PicUrl.Substring(model.PicUrl.LastIndexOf("/") + 1);
        //    //                        oldFileName = filePath + oldFileName;
        //    //                        //删除原图片
        //    //                        if (System.IO.File.Exists(oldFileName))
        //    //                        {
        //    //                            System.IO.File.Delete(oldFileName);
        //    //                        }
        //    //                    }
        //    //                    model.PicUrl = uploadFileUrl;
        //    //                }
        //    //                catch (Exception ex)
        //    //                {
        //    //                    return _articleService.Error((int)HttpStatusCode.InternalServerError, "修改失败" + ex.Message);
        //    //                }
        //    //            }
        //    //        }

        //    //        if (!await _articleService.EditArticleAsync(model))
        //    //        {
        //    //            return _articleService.Error((int)HttpStatusCode.InternalServerError, "修改失败");
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        if (param.FormFile != null && param.FormFile.Length > 0)
        //    //        {
        //    //            var file = param.FormFile;
        //    //            try
        //    //            {
        //    //                string fileName = file.FileName.Trim().Substring(0, file.FileName.Length - Path.GetExtension(file.FileName).Length);
        //    //                //新的文件名
        //    //                string newFileName = fileName + DateTime.Now.ToString("_yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + Path.GetExtension(file.FileName);
        //    //                //文件上传路径
        //    //                //string filePath = _hostingEnv.WebRootPath + "/images/Article/";
        //    //                string webRootPath = string.IsNullOrEmpty(_configuration["UploadFilePath"]) ? _hostingEnv.WebRootPath : _configuration["UploadFilePath"];
        //    //                string filePath = webRootPath + "/images/Article/";
        //    //                //图片网路地址
        //    //                string fileUrl = _configuration["ImgDomain"] + "/Article/";
        //    //                if (!Directory.Exists(filePath))
        //    //                {
        //    //                    Directory.CreateDirectory(filePath);
        //    //                }
        //    //                filePath = filePath + $@"{newFileName}";//指定文件上传路径
        //    //                uploadFileUrl = fileUrl + $@"{newFileName}";//指定文件上传路径
        //    //                using (FileStream fs = System.IO.File.Create(filePath))//创建文件流
        //    //                {
        //    //                    file.CopyTo(fs);//将上载文件的内容复制到目标流
        //    //                    fs.Flush();//清除此流的缓冲区并导致将任何缓冲数据写入
        //    //                }
        //    //            }
        //    //            catch (Exception ex)
        //    //            {
        //    //                return _articleService.Error((int)HttpStatusCode.InternalServerError, "添加失败" + ex.Message);
        //    //            }
        //    //        }

        //    //        Article article = new Article()
        //    //        {
        //    //            Content = param.Content,
        //    //            CreatedDate = DateTime.Now,
        //    //            DoctorId = param.DoctorId,
        //    //            Title = param.Title,
        //    //            TopicTypeId = param.TopicTypeId,
        //    //            ArticleTypeId = param.ArticleTypeId,
        //    //            ArticleCategory = param.ArticleCategory,
        //    //            PicUrl = uploadFileUrl,
        //    //        };
        //    //        //if (await _articleService.ExistArticleAsync(article))
        //    //        //{
        //    //        //    return _articleService.Error((int)HttpStatusCode.InternalServerError, "当前数据已存在，不能重复添加");
        //    //        //}

        //    //        if (!await _articleService.AddArticleAsync(article))
        //    //        {
        //    //            return _articleService.Error((int)HttpStatusCode.InternalServerError, "添加失败");
        //    //        }
        //    //    }
        //    //}

        //    #endregion

        //    if (param.UserId > 0)
        //    {
        //        if (await _articleService.ExistArticleAsync(param.Id, param.Title))
        //        {
        //            //return _articleService.Error((int)HttpStatusCode.InternalServerError, "当前数据已存在，不能重复添加");
        //        }

        //        if (param.Id > 0)
        //        {
        //            Article model = await _articleService.GetArticleAsync(param.Id.Value, param.DoctorId);
        //            if (model == null && model.Id <= 0)
        //            {
        //                //return _articleService.Error((int)HttpStatusCode.InternalServerError, "该用户发布文章不存在");
        //            }
        //            model.ArticleTypeId = param.ArticleTypeId;
        //            model.Title = param.Title;
        //            model.Content = param.Content;
        //            model.TopicTypeId = param.TopicTypeId;
        //            model.UpdatedDate = DateTime.Now;

        //            try
        //            {
        //                if (model.PicUrl != param.ImgUrl)
        //                {
        //                    //删除旧图片
        //                    if (!string.IsNullOrEmpty(model.PicUrl))
        //                    {
        //                        var fileName = model.PicUrl.Substring(model.PicUrl.LastIndexOf("/") + 1);
        //                        //string filePath = _hostingEnv.WebRootPath + "/images/Article/";
        //                        string webRootPath = string.IsNullOrEmpty(_configuration["UploadFilePath"]) ? _hostingEnv.WebRootPath : _configuration["UploadFilePath"];
        //                        string filePath = webRootPath + "/images/Article/";
        //                        fileName = filePath + fileName;
        //                        //删除原图片
        //                        if (System.IO.File.Exists(fileName))
        //                        {
        //                            System.IO.File.Delete(fileName);
        //                        }
        //                    }
        //                    model.PicUrl = param.ImgUrl;
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                return _articleService.Error((int)HttpStatusCode.InternalServerError, "修改失败" + ex.Message);
        //            }
        //            if (!await _articleService.EditArticleAsync(model))
        //            {
        //                return _articleService.Error((int)HttpStatusCode.InternalServerError, "修改失败");
        //            }
        //        }
        //        else
        //        {
        //            Article article = new Article()
        //            {
        //                Content = param.Content,
        //                CreatedDate = DateTime.Now,
        //                DoctorId = param.DoctorId,
        //                Title = param.Title,
        //                TopicTypeId = param.TopicTypeId,
        //                ArticleTypeId = param.ArticleTypeId,
        //                //ArticleCategory = param.ArticleCategory,
        //                PicUrl = param.ImgUrl,
        //            };

        //            if (!await _articleService.AddArticleAsync(article))
        //            {
        //                return _articleService.Error((int)HttpStatusCode.InternalServerError, "添加失败");
        //            }
        //        }
        //    }

        //    return Ok();
        //}

        ///// <summary>
        ///// 文章删除
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost("delete")]
        //public async Task<IActionResult> DeleteArticleAsync(int id)
        //{
        //    try
        //    {
        //        Article atc = await _articleService.GetArticleByIdAsync(id);
        //        if (await _articleService.DeleteArticleAsync(id))
        //        {
        //            if (!string.IsNullOrEmpty(atc.PicUrl))
        //            {
        //                var fileName = atc.PicUrl.Substring(atc.PicUrl.LastIndexOf("/") + 1);
        //                //string filePath = _hostingEnv.WebRootPath + "/images/Article/";
        //                string webRootPath = string.IsNullOrEmpty(_configuration["UploadFilePath"]) ? _hostingEnv.WebRootPath : _configuration["UploadFilePath"];
        //                string filePath = webRootPath + "/images/Article/";
        //                fileName = filePath + fileName;
        //                //删除原图片
        //                if (System.IO.File.Exists(fileName))
        //                {
        //                    System.IO.File.Delete(fileName);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //return _articleService.Error((int)HttpStatusCode.InternalServerError, "删除失败，" + ex.ToString());
        //    }
        //    return Ok();

        //}


    }           
}
