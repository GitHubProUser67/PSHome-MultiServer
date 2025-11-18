using S22.Imap;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MultiServerLibrary.ManagedMail
{
    public abstract class EmailProcessor
    {
        public int MailRetrieverDelay { get; set; } = 6000;

        public string smtpAddress;

        public ushort smtpSenderPort;
        public ushort smtpReceiverPort;

        public bool secure;

        private volatile bool listenThreadActive;

        private ImapClient imapClient = null;

        public EmailProcessor(string smtpAddress = "smtp.gmail.com", ushort smtpSenderPort = 587, ushort smtpReceiverPort = 993, bool secure = true)
        {
            this.smtpAddress = smtpAddress;
            this.smtpSenderPort = smtpSenderPort;
            this.smtpReceiverPort = smtpReceiverPort;
            this.secure = secure;
        }

        protected abstract void OnNewEmail(MailMessage message);

        public void Stop()
        {
            // stop listener
            listenThreadActive = false;
            imapClient?.Dispose();
        }

        public async Task ListenAsync(string fromEmail, string passwordFromEmail, bool unseenOnly)
        {
            imapClient = new ImapClient(smtpAddress, smtpReceiverPort, fromEmail, passwordFromEmail, AuthMethod.Login, secure);

            if (!imapClient.Supports("IDLE"))
            {
                listenThreadActive = false;
                CustomLogger.LoggerAccessor.LogError($"[EmailProcessor] - Targetted IMAP host:{smtpAddress} doesn't support Idle, impossible to listen on it!");
                return;
            }

            imapClient.IdleError += (sender, ex) =>
            {
                CustomLogger.LoggerAccessor.LogError($"[EmailProcessor] - Idle error in host:{smtpAddress}. (Exception:{ex})");
            };

            listenThreadActive = true;

            if (unseenOnly)
            {
                while (listenThreadActive)
                {
                    try
                    {
                        foreach (uint oid in imapClient.Search(SearchCondition.Unseen()))
                        {
                            try
                            {
                                OnNewEmail(imapClient.GetMessage(oid));
                            }
                            catch (Exception ex)
                            {
                                CustomLogger.LoggerAccessor.LogError($"[Emailprocessor] - Error while reading unseen mail with oid:{oid}. (Exception:{ex})");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LoggerAccessor.LogError($"[Emailprocessor] - Error while searching unseen mails. (Exception:{ex})");
                    }

                    await Task.Delay(MailRetrieverDelay).ConfigureAwait(false);
                }
            }
            else
            {
                while (listenThreadActive)
                {
                    try
                    {
                        foreach (uint oid in imapClient.Search(SearchCondition.All()))
                        {
                            try
                            {
                                OnNewEmail(imapClient.GetMessage(oid));
                            }
                            catch (Exception ex)
                            {
                                CustomLogger.LoggerAccessor.LogError($"[Emailprocessor] - Error while reading mail with oid:{oid}. (Exception:{ex})");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LoggerAccessor.LogError($"[Emailprocessor] - Error while searching mails. (Exception:{ex})");
                    }

                    await Task.Delay(MailRetrieverDelay).ConfigureAwait(false);
                }
            }
        }

        public void SendEmail(string fromEmail, string passwordFromEmail, string toEmail, string subject, string body)
        {
            using (SmtpClient client = new SmtpClient(smtpAddress, smtpSenderPort))
            {
                client.EnableSsl = secure;
                client.Credentials = new NetworkCredential(fromEmail, passwordFromEmail);

                client.Send(new MailMessage(fromEmail, toEmail, subject, body));
            }
        }
    }
}
