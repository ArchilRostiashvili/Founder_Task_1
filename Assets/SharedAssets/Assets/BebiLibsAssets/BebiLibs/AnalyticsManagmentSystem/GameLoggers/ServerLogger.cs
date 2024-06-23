using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Collections;
using BebiLibs.Analytics.GameEventLogger;
//using UnityEngine.Networking;
using System.Net;
using System;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Networking;
using System.Linq;

namespace BebiLibs.Analytics
{
    public class ServerLogger : LoggerBase
    {
        public Queue<IGameEvent> gameEvents = new Queue<IGameEvent>();
        public bool enableLogging = false;
        private StringBuilder _parameterBuilder = new StringBuilder();

        public List<string> urls = new List<string>();

        private void Start()
        {
            if (Debug.isDebugBuild)
            {
                enableLogging = true;
            }

            if (enableLogging)
            {
                InvokeRepeating("InvokeLog", 0.5f, 0.5f);
            }
        }

        private void InvokeLog()
        {
            if (enableLogging && gameEvents.Count > 0)
            {
                IGameEvent gameEvent = gameEvents.Dequeue();
                gameEvent.InvokeLog(this);
            }
        }

        public void Ping_all()
        {
            if (!enableLogging) return;
            string gate_ip = NetworkGateway();
            string[] array = gate_ip.Split('.');
            for (int i = 0; i <= 255; i++)
            {
                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;
                PingPort(ping_var);
            }
        }

        public void PingPort(string host)
        {
            System.UriBuilder uriBuilder = new UriBuilder(host + ":" + 5544);
            StartCoroutine(PingRequest(uriBuilder.Uri));
            IEnumerator PingRequest(Uri uri)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        urls.Add(webRequest.url);
                    }
                    else
                    {
                        webRequest.Dispose();
                    }
                }
            }
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                Debug.Log($"{ip}");
            }
        }

        static string NetworkGateway()
        {
            string ip = null;

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        ip = d.Address.ToString();
                    }
                }
            }
            return ip;
        }

        public void MakeRequest(string logEvent)
        {
            for (int i = 0; i < urls.Count; i++)
            {
                CreateRequest(urls[i], logEvent);
            }
        }

        private async void CreateRequest(string url, string gameEvent)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                var content = new StringContent(gameEvent, Encoding.UTF8, "application/xml");
                try
                {
                    var response = await client.PostAsync(url, content);
                    var respString = await response.Content.ReadAsStringAsync();
                }
                catch
                {

                }
            }
        }

        public override void RecordEvent(IGameEvent gameEvent)
        {
            if (enableLogging)
            {
                gameEvents.Enqueue(gameEvent);
            }
        }

        public override void LogEvent(SimpleEvent simpleEvent)
        {
            MakeRequest($"Log Name: {simpleEvent.EventName}");
        }

        public override void LogEvent(StringEvent stringEvent)
        {
            MakeRequest($"Log Name: {stringEvent.EventName}\nParameter Name: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(LongEvent stringEvent)
        {
            MakeRequest($"Log Name: {stringEvent.EventName}\nParameter Name: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(DoubleEvent stringEvent)
        {
            MakeRequest($"Log Name: {stringEvent.EventName}\nParameter Name: {stringEvent.ParameterName}, Value: {stringEvent.Value}");
        }

        public override void LogEvent(MultiParameterEvent multiParameterEvent)
        {
            _parameterBuilder.Clear();
            List<IGameParameter> parameters = multiParameterEvent.ParametersList.GroupBy(x => x.GetParameterName()).Select(x => x.Last()).ToList();
            for (int i = 0; i < parameters.Count; i++)
            {
                parameters[i].UserParameter(this);
            }

            MakeRequest($"Log Name: {multiParameterEvent.EventName}\n{_parameterBuilder}");
        }

        public override void SetProperty(GameProperty property)
        {
            MakeRequest($"User Property: {property.PropertyName}, value {property.Value}");
        }

        public override void HandleParameter(StringParameter parameter)
        {
            _parameterBuilder.AppendLine($"Parameter Name: {parameter.ParameterName}, Value: {parameter.Value}");
        }

        public override void HandleParameter(LongParameter parameter)
        {
            _parameterBuilder.AppendLine($"Parameter Name: {parameter.ParameterName}, Value: {parameter.Value}");
        }

        public override void HandleParameter(DoubleParameter parameter)
        {
            _parameterBuilder.AppendLine($"Parameter Name: {parameter.ParameterName}, Value: {parameter.Value}");
        }


    }
}
