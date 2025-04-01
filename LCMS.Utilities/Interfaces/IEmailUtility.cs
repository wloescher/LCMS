namespace LCMS.Utilities.Interfaces
{
    public interface IEmailUtility
    {
        string SmtpServer { get; set; }
        string SmtpServerEmailAddress { get; set; }
        bool SmtpServerEnableSsl { get; set; }
        string SmtpServerPassword { get; set; }
        int SmtpServerPort { get; set; }
        string SmtpServerRealName { get; set; }
        string SmtpServerUserName { get; set; }

        bool SendMail(string to, string subject, string body, out string returnMessage, bool isHtml = false);
        bool SendMail(string from, string to, string subject, string body, out string returnMessage, bool isHtml = false);
        bool SendMail(string from, string to, string cc, string subject, string body, out string returnMessage, bool isHtml, string replyTo = "");
        bool SendMail(string from, string to, string cc, string bcc, string subject, string body, out string returnMessage, bool isHtml = false, string replyTo = "", string[]? attachments = null, bool deleteAttachments = true);
        Task<string> SendMailAsync(string to, string subject, string body, bool isHtml = false);
        Task<string> SendMailAsync(string from, string to, string subject, string body, bool isHtml = false);
        Task<string> SendMailAsync(string from, string to, string cc, string subject, string body, bool isHtml, string replyTo = "");
        Task<string> SendMailAsync(string from, string to, string cc, string bcc, string subject, string body, bool isHtml = false, string replyTo = "", string[]? attachments = null, bool deleteAttachments = true);
    }
}
