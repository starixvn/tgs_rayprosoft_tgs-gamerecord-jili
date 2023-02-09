using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.JiliService.Core;
using Tgs.GameRecord.JiliService;

namespace Tgs.GameRecord.JiliService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public object Get()
        {
            var gameRecordInfo = _configuration.GetSection("GameRecordInfo").Get<GameRecordInfoOptions>();
            var mongoSetting = _configuration.GetSection("MongoSetting").Get<MongoSettingOptions>();
            var natsSetting = _configuration.GetSection("NatsSetting").Get<NatsSettingOptions>();

            return new
            {
                gameRecordInfo,
                mongoSetting,
                natsSetting
            };
        }
    }
}
