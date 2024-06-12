using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public class EmailSender : IEmailSender
    {
  
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, email);
        }

        public Task Execute(string subject, string mensaje, string email)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(email);
            mm.Subject = subject;
            mm.Body = mensaje;
            mm.From = new MailAddress("support@baezstone.com");
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("apikey", "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");

            return smtp.SendMailAsync(mm);
        }
    }
}
