using System.Diagnostics;

namespace TheWorld.Services
{
    class DebugMailService : IMailService
    {
        public void SendMail(string to, string from, string subject, string body)
        {
            Debug.WriteLine($"Sending Mail: To: {to} From: {from} Subject: {subject}");
        }
    }
}