using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem;
using System.Text;
using UnityEngine;

namespace BebiLibs
{
    public class MailReportSystem : GenericSingletonClass<MailReportSystem>
    {
        [SerializeField] private PurchaseManagerBase _purchaseManagerBase;
        private StringBuilder _stringBuilder = new StringBuilder(100);

        public string ConstructReport()
        {
            _stringBuilder.Clear();

            _stringBuilder.AppendLine($"Product ID: {Application.identifier}");
            _stringBuilder.AppendLine($"Device ID: {SystemInfo.deviceUniqueIdentifier}");
            _stringBuilder.AppendLine($"App version: {Application.version}");
            _stringBuilder.AppendLine($"Operation system: {SystemInfo.operatingSystem}");
            _stringBuilder.AppendLine($"Device Model: {SystemInfo.deviceModel}");

            _stringBuilder.AppendLine("Describe your problem below this line: ");
            _stringBuilder.AppendLine("——————————————————————————————————————");

            _stringBuilder.AppendLine();
            return _stringBuilder.ToString();
        }

        public static void SendReport()
        {
            Instance.SendEmail();
        }

        public void SendEmail()
        {
            string email = "info@bebi.family";
            string subject = EncodeUrl("I am having an issue with the app");
            string body = EncodeUrl(ConstructReport());
            string mailToString = $"mailto:{email}?subject={subject}&body={body}";
            Application.OpenURL(mailToString);
        }

        public string EncodeUrl(string url)
        {
            return System.Net.WebUtility.UrlEncode(url).Replace("+", "%20"); ;
        }
    }
}
