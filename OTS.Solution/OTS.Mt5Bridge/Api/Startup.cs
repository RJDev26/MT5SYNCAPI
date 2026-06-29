using System.Web.Http;
using Owin;

namespace OTS.Mt5Bridge.Api
{
    /// <summary>OWIN startup: attribute-routed Web API with JSON output.</summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // JSON only.
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            app.UseWebApi(config);
        }
    }
}
