using Microsoft.AspNet.SignalR;  
using Microsoft.Owin;  
using Owin;  
  
[assembly: OwinStartup(typeof(TiktokTools.Web.Startup))]  
namespace TiktokTools.Web
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ////使用自己的idprovider，文章后面有介绍，如果不需要可以删除这两行  
            //var idProvider = new CustomUserIdProvider();
            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);

            // Any connection or hub wire up and configuration should go here  
            app.MapSignalR();
        }
    }
}