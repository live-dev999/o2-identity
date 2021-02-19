using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using O2.Identity.Web.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace O2.Identity.Web.Controllers
{
    [Route("api/sms")]
    public class SmsController: Controller
    {
        private readonly IVerification _verification;
        private readonly ILogger<SmsController> _logger;

        public SmsController(IVerification verification, ILogger<SmsController> logger)
        {
            _verification = verification;
            _logger = logger;
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> Send(string username, string password,string phoneNumber)
        {
            // Find your Account Sid and Token at twilio.com/console
            //if (_verification.Config.NotificationSms=="true")
            // {
                // Find your Account Sid and Token at twilio.com/console
                TwilioClient.Init(_verification.Config.AccountSid, _verification.Config.AuthToken);

                var message = MessageResource.Create(
                    body:
                    $"\"#PF_R COMMUNITY\". https://pfr-centr.com. Username: {username}, Password: {password}.",
                    from: new Twilio.Types.PhoneNumber(_verification.Config.PhoneNumber),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );


                _logger.LogInformation(
                    $"Send sms to account PhoneNumber = {message.To} ,SID SMS= {message.Sid} ");
             //}

            // Console.WriteLine(message.Sid);
            // TwilioClient.Init(_verification.Config.AccountSid, _verification.Config.AuthToken);
            //
            // var message = await MessageResource.CreateAsync(
            //     body: "O2 Platfrom",
            //     @from: new Twilio.Types.PhoneNumber(_verification.Config.PhoneNumber),
            //     to: new Twilio.Types.PhoneNumber("+375447987208")
            // );
             return Ok(message.Sid);
        }
    }
}