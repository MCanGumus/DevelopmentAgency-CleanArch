using Azure.Core;
using Newtonsoft.Json;
using DA.Domain.Entities;
using DA.Models;
using System.Net;

namespace DA.Components.System
{
    public static class SessionHelper
    {
        private static readonly string encryptionKey = "1s2d7g8n6m5%2c!f2k0a2w4p4e11b20s"; // 32 karakter uzunluğunda bir anahtar kullanın.
        private static readonly string hmacKey = "9s8d6g8n4m3%1c!d2k0a2w4p4e11b15s";
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            session.SetString(key, json);
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetCookie(this HttpContext context, string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMinutes(60);

            option.HttpOnly = true;
            option.Secure = true;
            option.SameSite = SameSiteMode.Strict;

            context.Response.Cookies.Append(key, value, option);
        }

        public static string GetCookie(this HttpContext context, string key)
        {
            return context.Request.Cookies[key];
        }

        //public static void SetEmployeeLoggingIn(this HttpContext context, LoginSessionModel model)
        //{
        //    var json = JsonConvert.SerializeObject(model);
        //    context.SetCookie("LoginInfos", json, 60); // 60 dakika süreli
        //}

        //public static LoginSessionModel GetEmployeeLoggingIn(this HttpContext context)
        //{
        //    var cookieValue = context.GetCookie("LoginInfos");
        //    return cookieValue == null ? null : JsonConvert.DeserializeObject<LoginSessionModel>(cookieValue);
        //}

        public static void SetEmployeeLoggingIn(this HttpContext context, LoginSessionModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var encryptedValue = EncryptionDecryption.EncryptString(json, encryptionKey); 
            var hmac = EncryptionDecryption.ComputeHMAC(encryptedValue, hmacKey);
            var cookieValue = $"{encryptedValue}:{hmac}";

            context.SetCookie("LoginInfos", cookieValue, 60);
        }

        public static LoginSessionModel GetEmployeeLoggingIn(this HttpContext context)
        {
            var cookieValue = context.GetCookie("LoginInfos");
            if (cookieValue == null)
                return null;

            var parts = cookieValue.Split(':');
            if (parts.Length != 2)
                return null; // Geçersiz çerez formatı

            var encryptedValue = parts[0];
            var hmac = parts[1];

            // HMAC doğrulaması
            if (!EncryptionDecryption.VerifyHMAC(encryptedValue, hmac, hmacKey))
                return null; // Çerez değiştirilmiş, doğrulama başarısız

            var decryptedValue = EncryptionDecryption.DecryptString(encryptedValue, encryptionKey);
            return JsonConvert.DeserializeObject<LoginSessionModel>(decryptedValue);
        }


        public static void SetEmployeeLoggingIn(this ISession session, LoginSessionModel model)
        {
            session.SetObject("LoginInfos", model);
        }

        public static LoginSessionModel GetEmployeeLoggingIn(this ISession session)
        {
            return session.GetObject<LoginSessionModel>("LoginInfos");
        }

        public static string GetIpAddress()
        {
            return Dns.GetHostByName(Dns.GetHostName()).AddressList[1].ToString();
        }
    }
}
