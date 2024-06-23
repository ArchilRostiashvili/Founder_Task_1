using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem.Core
{

    [System.Serializable]
    public class UserSignInToken
    {
        public bool isValid { get; private set; }

        public string rawToken;

        [JsonProperty("jti")] public string jwtId;
        [JsonProperty("sid")] public string sessionId;
        [JsonProperty("dev")] public string dev;
        [JsonProperty("name")] public string userName;
        [JsonProperty("sub")] public string userID;
        [JsonProperty("email")] public string userEmail;
        [JsonProperty("nbf")] public SDateTime notValidBefore = SDateTime.UtcMin;
        [JsonProperty("exp")] public SDateTime expirationTime = SDateTime.UtcMin;
        [JsonProperty("iat")] public SDateTime issuedAtTime = SDateTime.UtcMin;
        [JsonProperty("iss")] public string issuer;
        [JsonProperty("aud")] public string audience;

        public bool isUserSignedIn
        {
            get
            {
                return DateTime.UtcNow < this.expirationTime.DateTime;
            }
        }

        public bool canRefreshUserToken => this.isUserSignedIn && this.tokenExpiresIn < TimeSpan.FromDays(4);
        public TimeSpan tokenExpiresIn => (DateTime)this.expirationTime - DateTime.UtcNow;

        public UserSignInToken(string rawTokenValue)
        {
            //Debug.LogWarning("GIO_Token " + rawTokenValue);
            int indexOfSharp = rawTokenValue.IndexOf('#');
            this.rawToken = indexOfSharp != -1 ? rawTokenValue.Substring(0, indexOfSharp) : rawTokenValue;
            //Debug.LogWarning("GIO_FIXED_Token " + this.rawToken);
            try
            {
                if (this.TryDecodeSignToken(rawToken, out string decodedToken))
                {
                    JsonConvert.PopulateObject(decodedToken, this);
                    //Debug.LogWarning("User Token Data: " + this);
                    this.isValid = true;
                }
                else
                {
                    Debug.LogError("Unable To Parse User Token: " + this.rawToken);
                    this.isValid = false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error Initializing UserSignInToken: " + e);
                this.isValid = false;
            }
        }

        private bool TryParseJWTTime(string timeString, out SDateTime dateTime)
        {
            if (long.TryParse(timeString, out long expDateLong))
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expDateLong);
                dateTime = dateTimeOffset.LocalDateTime;
                return true;
            }
            dateTime = DateTime.MinValue;
            return false;
        }

        private string DeepParse(string json, params string[] data)
        {
            string retValue = json;
            foreach (var key in data)
            {
                JObject parsedObject = JObject.Parse(retValue);
                if (parsedObject.TryGetValue(key, out JToken jsToken))
                {
                    retValue = jsToken.ToString();
                }
            }
            return retValue;
        }

        private bool TryDecodeSignToken(string token, out string jsonData)
        {
            string[] tokenParts = token.Split('.');
            if (tokenParts.Length == 3)
            {
                string tokenJson = tokenParts[1];
                try
                {
                    byte[] data = ConvertFromBase64String(tokenJson);
                    jsonData = System.Text.Encoding.UTF8.GetString(data);
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error Parsing Sign in token: " + e);
                    jsonData = string.Empty;
                    return false;
                }
            }
            else
            {
                Debug.LogError("Json Data Is Empty");
                jsonData = string.Empty;
                return false;
            }
        }

        private static byte[] ConvertFromBase64String(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return null;
            try
            {
                string working = input.Replace('-', '+').Replace('_', '/'); ;
                while (working.Length % 4 != 0)
                {
                    working += '=';
                }
                //Debug.Log("Fixed JWT: " + working);
                return Convert.FromBase64String(working);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            string report = "";
            report += nameof(this.rawToken) + " " + this.rawToken + "\n";
            report += nameof(this.userName) + " " + this.userName + "\n";
            report += nameof(this.userEmail) + " " + this.userEmail + "\n";
            report += nameof(this.userID) + " " + this.userID + "\n";
            report += nameof(this.notValidBefore) + " " + this.notValidBefore + "\n";
            report += nameof(this.expirationTime) + " " + this.expirationTime + "\n";
            report += nameof(this.issuedAtTime) + " " + this.issuedAtTime + "\n";
            return report;
        }

        public void ResetData()
        {
            this.rawToken = string.Empty;
            this.userName = string.Empty;
            this.userEmail = string.Empty;
            this.userID = string.Empty;
            this.notValidBefore = SDateTime.UtcMin;
            this.expirationTime = SDateTime.UtcMin;
            this.issuedAtTime = SDateTime.UtcMin;
        }
    }
}
