using System;
using System.Threading.Tasks;
using RestSharp;

namespace Insane_Mechanical.Services
{
    public class WhatsAppServices
    {

        public async void SendVerificationCode(string toWhatsAppNumber, string verificationCode)
        {
            var url = "https://api.ultramsg.com/instance88186/messages/chat";
            var client = new RestClient(url);

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "rm37yibboqrgj0iz");
            request.AddParameter("to", $"{toWhatsAppNumber}");
            request.AddParameter("body", $"Su codigo de verificacion es {verificationCode}");


            RestResponse response = await client.ExecuteAsync(request);
        }
    }
}
