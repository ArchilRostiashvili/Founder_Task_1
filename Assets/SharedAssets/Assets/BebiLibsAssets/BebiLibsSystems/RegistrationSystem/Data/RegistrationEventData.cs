using BebiLibs;
using BebiLibs.RegistrationSystem.Core;

namespace BebiLibs.RegistrationSystem
{
    [System.Serializable]
    public class RegistrationEventData
    {
        public string Sender;
        public Provider Provider;
        public string SignPrefix;

        public RegistrationEventData()
        {

        }

        public RegistrationEventData(string sender, Provider provider = Provider.Unknown, string signPrefix = null)
        {
            Sender = sender;
            Provider = provider;
            SignPrefix = signPrefix;
        }

    }

}
