using System.Net.Mail;
using System.Net;

namespace DA.Components.System
{
    public class MailSenderService
    {
        public static void SendEmail(string toWho, string subject, string body)
        {
            // SMTP sunucu bilgileri
            string smtpServer = "smtp.gmail.com";
            string senderEmail = Constants.SMTPAddress;
            string senderPassword = Constants.SMTPPassword;
            int smtpPort = 587;

            // MailMessage nesnesi oluşturma
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail, Constants.ProjectShortName + " Portal");
            mail.To.Add(toWho);
            mail.Subject = subject;
            mail.Body = body + "\n\n\nNot: Lütfen bu maili yanıtlamayınız. Bu bilgilendirme maili size " + Constants.ProjectFullName + " Personel Bilgi Sistemi tarafından otomatik olarak üretilerek iletilmiş olup mail içeriği ile ilgili bir problem olduğunu düşünüyorsanız sistem yöneticisi veya birim başkanınız ile görüşmeniz gerekmektedir.";
            mail.Body = mail.Body.Replace("\n", "<br>");
            
            mail.IsBodyHtml = true; // HTML içeriği göndermek istiyorsanız true yapın

            // SmtpClient nesnesi oluşturma ve yapılandırma
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            // E-posta gönderme
            smtpClient.Send(mail);
        }
    }
}
