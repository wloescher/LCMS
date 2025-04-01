using LCMS.Utilities.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace LCMS.Utilities
{
    public class EmailUtility : IEmailUtility
    {
        public string SmtpServer { get; set; } = string.Empty;
        public string SmtpServerEmailAddress { get; set; } = string.Empty;
        public string SmtpServerRealName { get; set; } = string.Empty;
        public string SmtpServerUserName { get; set; } = string.Empty;
        public string SmtpServerPassword { get; set; } = string.Empty;
        public int SmtpServerPort { get; set; }
        public bool SmtpServerEnableSsl { get; set; }

        public EmailUtility(IConfiguration config)
        {
            var configSection = config.GetSection("LCMS:Email");
            if (configSection != null)
            {
                SmtpServer = configSection["SmtpServer"] ?? string.Empty;
                SmtpServerEmailAddress = configSection["SmtpServerEmailAddress"] ?? string.Empty;
                SmtpServerRealName = configSection["SmtpServerRealName"] ?? string.Empty;
                SmtpServerUserName = configSection["SmtpServerUserName"] ?? string.Empty;
                SmtpServerPassword = configSection["SmtpServerPassword"] ?? string.Empty;
                SmtpServerPort = Convert.ToInt32(configSection["SmtpServerPort"]);
                SmtpServerEnableSsl = Convert.ToBoolean(configSection["SmtpServerEnableSsl"]);
            }
        }

        #region Public Methods

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <returns></returns>
        public bool SendMail(string to, string subject, string body, out string returnMessage, bool isHtml = false)
        {
            return SendMail("noreply@lcms.com", to, string.Empty, string.Empty, subject, body, out returnMessage, isHtml);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <returns></returns>
        public bool SendMail(string from, string to, string subject, string body, out string returnMessage, bool isHtml = false)
        {
            return SendMail(from, to, string.Empty, string.Empty, subject, body, out returnMessage, isHtml);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="cc">The email address of anybody that is to be copied on the email</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <returns></returns>
        public bool SendMail(string from, string to, string cc, string subject, string body, out string returnMessage, bool isHtml, string replyTo = "")
        {
            return SendMail(from, to, cc, string.Empty, subject, body, out returnMessage, isHtml, replyTo);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="cc">The email address of anybody that is to be copied on the email</param>
        /// <param name="bcc">The email address of anybody that is to be blind copied on the email</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="returnMessage"></param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <param name="attachments"></param>
        /// <param name="deleteAttachments"></param>
        /// <returns>Message Success or Error message</returns>
        public bool SendMail(string from, string to, string cc, string bcc, string subject, string body, out string returnMessage, bool isHtml = false, string replyTo = "", string[]? attachments = null, bool deleteAttachments = true)
        {
            var result = false;
            returnMessage = string.Empty;

            using (var msg = GetMailMessage(from, to, cc, bcc, subject, body, isHtml, replyTo))
            {
                // Check for attachments
                if (attachments != null && attachments.Length > 0)
                {
                    // Add attachments
                    foreach (var attachment in attachments)
                    {
                        try
                        {
                            if (File.Exists(attachment))
                            {
                                msg.Attachments.Add(new Attachment(attachment));
                            }
                            else
                            {
                                returnMessage += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: The file {0} does not exist! ", attachment);
                            }
                        }
                        catch (FileNotFoundException e)
                        {
                            returnMessage += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: {0} ", e.Message);
                        }
                    }

                    // If there is an error in attaching, let the recipient know also.
                    msg.Body = returnMessage + body;
                }

                var sendMailError = string.Empty;
                result = SendMail(msg, out sendMailError);

                returnMessage += sendMailError;
            }

            // Delete attachments
            if (attachments != null && attachments.Length > 0 && deleteAttachments)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        if (File.Exists(attachment))
                        {
                            File.Delete(attachment);
                            returnMessage += " Attachments Deleted ";
                        }
                        else
                        {
                            returnMessage += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: The file {0} does not exist, so it cannot be deleted. ", attachments);
                        }
                    }
                    catch (FileNotFoundException e)
                    {
                        returnMessage += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: {0}", e.Message);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="cc">The email address of anybody that is to be copied on the email</param>
        /// <param name="bcc">The email address of anybody that is to be blind copied on the email</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <returns>Message Success or Error message</returns>
        public static MailMessage GetMailMessage(string from, string to, string cc, string bcc, string subject, string body, bool isHtml, string replyTo)
        {
            var message = new MailMessage
            {
                From = new MailAddress(from),
                IsBodyHtml = isHtml,
                Subject = subject,
                Body = body,
                Priority = !string.IsNullOrEmpty(subject) && subject.Contains("RUSH - ") ? MailPriority.High : MailPriority.Normal,
            };

            // Check for recipient(s)
            if (!string.IsNullOrEmpty(to))
            {
                foreach (var address in to.Split(','))
                {
                    if (!string.IsNullOrEmpty(address))
                    {
                        message.To.Add(new MailAddress(address));
                    }
                }
            }

            // Check for CC
            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var address in cc.Split(','))
                {
                    if (!string.IsNullOrEmpty(address))
                    {
                        message.CC.Add(new MailAddress(address));
                    }
                }
            }

            // Check for BCC
            if (!string.IsNullOrEmpty(bcc))
            {
                foreach (var address in bcc.Split(','))
                {
                    if (!string.IsNullOrEmpty(address))
                    {
                        message.Bcc.Add(new MailAddress(address));
                    }
                }
            }

            // Check for "reply to" address
            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo));
            }

            return message;
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <returns></returns>
        public async Task<string> SendMailAsync(string to, string subject, string body, bool isHtml = false)
        {
            return await SendMailAsync("noreply@lcms.com", to, string.Empty, string.Empty, subject, body, isHtml);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <returns></returns>
        public async Task<string> SendMailAsync(string from, string to, string subject, string body, bool isHtml = false)
        {
            return await SendMailAsync(from, to, string.Empty, string.Empty, subject, body, isHtml);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="cc">The email address of anybody that is to be copied on the email</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <returns></returns>
        public async Task<string> SendMailAsync(string from, string to, string cc, string subject, string body, bool isHtml, string replyTo = "")
        {
            return await SendMailAsync(from, to, cc, string.Empty, subject, body, isHtml, replyTo);
        }

        /// <summary>
        /// Sends an email with the provided inputs
        /// </summary>
        /// <param name="from">The email address of the sender</param>
        /// <param name="to">The email address of the recipient</param>
        /// <param name="cc">The email address of anybody that is to be copied on the email</param>
        /// <param name="bcc">The email address of anybody that is to be blind copied on the email</param>
        /// <param name="subject">The subject for the email to be sent</param>
        /// <param name="body">The body of the email to be sent</param>
        /// <param name="isHtml">True/False whether the email body is HTML</param>
        /// <param name="attachments"></param>
        /// <param name="deleteAttachments"></param>
        /// <returns>Message Success or Error message</returns>
        public async Task<string> SendMailAsync(string from, string to, string cc, string bcc, string subject, string body, bool isHtml = false, string replyTo = "", string[]? attachments = null, bool deleteAttachments = true)
        {
            var result = string.Empty;

            using (var msg = GetMailMessage(from, to, cc, bcc, subject, body, isHtml, replyTo))
            {
                // Check for attachments
                if (attachments != null && attachments.Length > 0)
                {
                    // Add attachments
                    foreach (var attachment in attachments)
                    {
                        try
                        {
                            if (File.Exists(attachment))
                            {
                                msg.Attachments.Add(new Attachment(attachment));
                            }
                            else
                            {
                                result += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: The file {0} does not exist! ", attachment);
                            }
                        }
                        catch (FileNotFoundException e)
                        {
                            result += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: {0} ", e.Message);
                        }
                    }

                    // If there is an error in attaching, let the recipient know also.
                    msg.Body = result + body;
                }

                var sendMailError = await SendMailAsync(msg);
                result += sendMailError;
            }

            // Delete attachments
            if (attachments != null && attachments.Length > 0 && deleteAttachments)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        if (File.Exists(attachment))
                        {
                            File.Delete(attachment);
                            result += " Attachments Deleted ";
                        }
                        else
                        {
                            result += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: The file {0} does not exist, so it cannot be deleted. ", attachments);
                        }
                    }
                    catch (FileNotFoundException e)
                    {
                        result += string.Format(System.Globalization.CultureInfo.InvariantCulture, "ERROR: {0}", e.Message);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private Methods        

        private bool SendMail(MailMessage msg, out string sendMailError)
        {
            // Initialize output parameter
            sendMailError = string.Empty;

            if (msg == null) return false;

            // Create client
            using (var client = new SmtpClient(SmtpServer, SmtpServerPort)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = SmtpServerEnableSsl,
                Credentials = new NetworkCredential(SmtpServerUserName, SmtpServerPassword),
            })
            {
                // From address must match email and "real name" of account for Office365
                msg.From = new MailAddress(SmtpServerEmailAddress, SmtpServerRealName);

                // Check security protocol (0 = SystemDefault in .NET 4.7+)
                var securityProtocol = (int)ServicePointManager.SecurityProtocol;
                if (securityProtocol != 0)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }

                // Send mail message
                try
                {
#if !DEBUG
                    client.Send(msg);
#endif
                }
                catch (SmtpException ex)
                {
                    // TODO: Make this actually do something meaningful on failure?
                    sendMailError = ex.Message;
                    if (ex.InnerException != null)
                    {
                        sendMailError += " " + ex.InnerException.Message;
                    }
                    return false;
                }
            }

            return true;
        }

        private async Task<string> SendMailAsync(MailMessage msg)
        {
            // Create client
            using (var client = new SmtpClient(SmtpServer, SmtpServerPort)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = SmtpServerEnableSsl,
                Credentials = new System.Net.NetworkCredential(SmtpServerUserName, SmtpServerPassword),
            })
            {
                // From address must match email and "real name" of account for Office365
                msg.From = new MailAddress(SmtpServerEmailAddress, SmtpServerRealName);

                // Send mail message
                try
                {
                    await client.SendMailAsync(msg);
                }
                catch (SmtpException ex)
                {
                    // TODO: Log exception?
                    return ex.Message;
                }
            }

            return string.Empty;
        }

        #endregion
    }
}
