using YueJia.Ebk.Domain.Company;
using YueJia.Ebk.Domain.SysUser;
using YueJia.Ebk.Web.ViewModels;

namespace YueJia.Ebk.Web
{
    public class MenuManage
    {

        public List<MenuModel> UserMenuList(AccountTypeEnum userAccountType)
        {
            return new List<MenuModel>()
                {
                      new MenuModel(){
                         pageIndex ="page1",
                         title = "首页",
                         url ="/Home/Index",
                         isShow = new List<AccountTypeEnum>(){  AccountTypeEnum.SuperAdmin, AccountTypeEnum.SysAdmin,AccountTypeEnum.NormalUser }.Contains(userAccountType)
                      },
                      new MenuModel(){
                         pageIndex ="page3",
                         title = "酒店管理",
                         url = "/Hotel/UserHotelMgr" ,
                         isShow = new List<AccountTypeEnum>(){  AccountTypeEnum.SysAdmin,AccountTypeEnum.NormalUser }.Contains(userAccountType),
                      },
                      new MenuModel(){
                         pageIndex ="page2",
                         title = "系统管理",
                         isShow = new List<AccountTypeEnum>(){  AccountTypeEnum.SuperAdmin, AccountTypeEnum.SysAdmin}.Contains(userAccountType),
                         children = new List<MenuModel>(){
                             new MenuModel(){
                                 pageIndex = "page2-1" ,
                                 title ="公司管理",
                                 url = "/Company/Index" ,
                                 isShow = new List<AccountTypeEnum>(){  AccountTypeEnum.SuperAdmin }.Contains(userAccountType)
                             },
                             new MenuModel(){
                                 pageIndex = "page2-2" ,
                                 title ="部门管理",
                                 url = "/Dept/Index" ,
                                 isShow = new List<AccountTypeEnum>(){ AccountTypeEnum.SysAdmin }.Contains(userAccountType)
                             },
                             new MenuModel(){
                                 pageIndex = "page2-3" ,
                                 title ="用户管理",
                                 url = "/SysUser/Index" ,
                                 isShow = new List<AccountTypeEnum>(){ AccountTypeEnum.SysAdmin }.Contains(userAccountType)
                             },
                         }
                      },
                };
        }
    }
}

               