using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using O2.Identity.Web.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace O2.Identity.Web.Controllers
{
    [Route("api/sms")]
    public class SmsController: Controller
    {
        private readonly IVerification _verification;

        public SmsController(IVerification verification)
        {
            _verification = verification;
        }
        [HttpGet("send")]
        public async Task<IActionResult> Send()
        {
            // Find your Account Sid and Token at twilio.com/console
          TwilioClient.Init(_verification.Config.AccountSid, _verification.Config.AuthToken);

            var message = MessageResource.Create(
                body: "I love you Veronica!.",
                from: new Twilio.Types.PhoneNumber(_verification.Config.PhoneNumber),
                to: new Twilio.Types.PhoneNumber("+375333762741")
            );

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