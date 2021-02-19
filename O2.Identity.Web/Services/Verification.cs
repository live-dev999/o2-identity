using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;


namespace O2.Identity.Web.Services
{
    public interface IVerification
    {
        Configuration.Twilio Config { get; set; }
        Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel);

        Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code);
    }

    public class Verification : IVerification
    {

        
        private Configuration.Twilio _config;

        public Verification(Configuration.Twilio configuration)
        {
            _config = configuration;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }

        public Configuration.Twilio Config
        {
            get => _config;
            set => _config = value;
        }

        public async Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel)
        {
            try
            {
                var verificationResource = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: channel,
                    pathServiceSid: _config.VerificationSid
                );
                return new VerificationResult(verificationResource.Sid);
            }
            catch (TwilioException e)
            {
                return new VerificationResult(new List<string>{e.Message});
            }
        }

        public async Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code)
        {
            try
            {
                var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: _config.VerificationSid
                );
                return verificationCheckResource.Status.Equals("approved") ?
                    new VerificationResult(verificationCheckResource.Sid) :
                    new VerificationResult(new List<string>{"Wrong code. Try again."});
            }
            catch (TwilioException e)
            {
                return new VerificationResult(new List<string>{e.Message});
            }
        }
    }
}