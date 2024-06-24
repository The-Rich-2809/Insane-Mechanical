using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using RestSharp;

namespace Insane_Mechanical.Services
{
    public class WhatsAppServices
    {

        public async void SendVerificationCode(string toWhatsAppNumber, string verificationCode)
        {
            var url = "https://api.ultramsg.com/instance88621/messages/chat";
            var client = new RestClient(url);

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "md30yevtkjutr9cc");
            request.AddParameter("to", $"{toWhatsAppNumber}");
            request.AddParameter("body", $"Su codigo de verificacion es {verificationCode}");


            RestResponse response = await client.ExecuteAsync(request);
        }

        public async void SendAdminReplyMessage(string toWhatsAppNumber, string nameArticule)
        {
            var url = "https://api.ultramsg.com/instance88621/messages/chat";
            var client = new RestClient(url);

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "md30yevtkjutr9cc");
            request.AddParameter("to", $"{toWhatsAppNumber}");
            request.AddParameter("body", $"Su comentario a sido respondido por uno de nuestros administradores, revisa su respuesta. En el siguiente articulo {nameArticule}");


            RestResponse response = await client.ExecuteAsync(request);
        }
    }
}
