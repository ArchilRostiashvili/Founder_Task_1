#nullable enable
using Newtonsoft.Json;
using System;
using System.Net.Mail;

namespace BebiLibs.RegistrationSystem
{
    [System.Serializable]
    public class UserCredential
    {
        [JsonProperty("username")]
        public string Email;

        [JsonProperty("password")]
        public string Password;

        public bool HasPassword;

        public UserCredential(string email, string password, bool hasPassword)
        {
            Email = email;
            HasPassword = hasPassword;
            Password = password;
        }

        public bool IsValid()
        {
            return IsEmailValid() && (!HasPassword || Password.Length > 0);
        }

        public bool IsEmailValid()
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return false;
                }

                MailAddress emailAddress = new MailAddress(Email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPasswordValid()
        {
            return Password.Length > 0 && HasPassword;
        }

        public UserCredential Clone()
        {
            return new UserCredential(Email, Password, HasPassword);
        }

        public void SetEmailAndPassword(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public override string ToString()
        {
            return $"Email: {Email}, Password: {Password}, HasPassword: {HasPassword}";
        }

        internal bool TryGetJsonData(out string json)
        {
            return JsonHandler.TrySerializeObjectToJson(this, out json);
        }
    }
}
