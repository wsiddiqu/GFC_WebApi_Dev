using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GFC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GFC.Api
{
    /// <summary>
    /// Test call for configuration setting changes effect on runtime
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationSettingController : Controller
    {
        protected IConfiguration Configuration;
        protected MySettings MySettings { get; set; }

        public ConfigurationSettingController(IOptionsSnapshot<MySettings> settings = null,IConfiguration configuration = null)
        {
            if (settings != null)
            {
                MySettings = settings.Value;
            }
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var m1 = MySettings.Message;
            var m2 = Configuration.GetSection("MySettings")["Message"];
            return Content($"m1:{m1}, m2:{m2}");

        }
    }
}
