using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Concrete.Dtos;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;

namespace ProgrammersBlog.Services.Services
{
    public class MailService : IMailService
    {
        private readonly SmtpSettings _smtpSettings;
        public MailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public IResult SendContactEmail(EmailSendDto sendDto)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(_smtpSettings.SenderEmail),
                To = { new MailAddress("oakkus444@gmail.com") },
                Subject = sendDto.Subject,
                IsBodyHtml = true,
                Body = $"Gönderen Kişi:{sendDto.Name},Gönderen E-Posta Adresi: {sendDto.Email}</br>{sendDto.Message}"
            };

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = _smtpSettings.Server,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.Send(mailMessage);

            return new Result(ResultStatusType.Success, $"E-Postanız başarıyla gönderilmiştir.");
        }


        public IResult Send(EmailSendDto sendDto)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(_smtpSettings.SenderEmail),
                To = { new MailAddress(sendDto.Email) },
                Subject = sendDto.Subject,
                IsBodyHtml = true,
                Body = $"Gönderen Kişi:{sendDto.Name},Gönderen E-Posta Adresi: {sendDto.Email}\n{sendDto.Message}"
            };

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = _smtpSettings.Server,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.SenderEmail, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtpClient.Send(mailMessage);
            return new Result(ResultStatusType.Success, $"E-Postanız başarıyla gönderilmiştir.");
        }
    }
}
