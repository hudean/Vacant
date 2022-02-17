控制器的下的action里的形参
比如下：

        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync(ParamAttention paramAttention)
        { 
        
        }
        这里默认的就是 FromBody形式 

        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync([FromBody]ParamAttention paramAttention)
        { 
        
        }

        FromForm ：表单的形式

        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync([FromForm]ParamAttention paramAttention)
        { 
        
        }

       
        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync([FromQuery]ParamAttention paramAttention)
        { 
        
        }

        FromQuery就和下面的一样
        /// <summary>
        /// 分页获取我的关注列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetMyAttentionByPageAsync(string param1,string param2)
        { 
        
        }
