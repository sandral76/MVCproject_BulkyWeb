using Microsoft.AspNetCore.Identity.UI.Services;


namespace Bulky.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //nije implemetirana logika za slanje imejla, samo ya potree testiranja je dodat ovaj return, ako bude potrebeno ovde implementirati logiku
            return Task.CompletedTask;
        }
    }
}
