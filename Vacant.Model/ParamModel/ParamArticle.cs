using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Model.ParamModel
{
    public class ParamArticle : ParamPage
    {
        public string KeyWord { get; set; }
        public string SortType { get; set; }
        public int TopickTypeId { get; set; }
        public int ArticleTypeId { get; set; }
    }

    public class ParamArticleAndCourse : ParamPage
    { 
        public int UserId { get; set; }
    }


    public class ParamArticleModel
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }


        public int UserId { get; set; }


        /// <summary>
        /// 话题类型Id
        /// </summary>
        public int? TopicTypeId { get; set; }

        /// <summary>
        /// 文章类型Id
        /// </summary>
        public int ArticleTypeId { get; set; }


        #region 原先的上传功能

        ///// <summary>
        ///// 是否修改图片 ，编辑修改图片，传一个true
        ///// </summary>
        //public bool IsEditImg { get; set; }

        //public IFormFile FormFile { get; set; }

        #endregion

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
    }

}
