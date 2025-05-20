using YueJia.Ebk.Application.Contracts.SysApp;

namespace YueJia.Ebk.Web.Controllers
{
    /// <summary>
    ///公共接口
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CommonController : AbpController
    {

        private ISysEnumApp SysEnumApp => LazyServiceProvider.LazyGetRequiredService<ISysEnumApp>();
        /// <summary>
        /// 按枚举名称获取枚举集合
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("{enumName}/enumAll")]
        //[HttpGet, Route("[controller]/{enumName}/create")]
        public IResult FindEnumByEnumName([FromRoute] string enumName) => ApiResult.HandleResult(SysEnumApp.GetEnumDataList(enumName));



    }

}
