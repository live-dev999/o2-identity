namespace O2.Identity.Web.Configuration
{
    public class Twilio
    {
        public string AccountSid { get; set; } = "ACa9a20dd4f6f83827a70577125a028d54";
        public string AuthToken { get; set; } = "325d415c458d3613ef56908ba88c6f24";
        public string VerificationSid { get; set; } = "VAf8474fe46d2bb889435cb8584ceb80cd";
        public bool NotificationSms { get; set; } 
        public string PhoneNumber { get; set; } = "+12057297918";
    }
}