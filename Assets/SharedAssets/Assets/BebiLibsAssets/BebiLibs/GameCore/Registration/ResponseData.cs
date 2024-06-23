using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ServerConfigLoaderSystem.Core
{
    [System.Serializable]
    public class ResponseData
    {
        public readonly string ResponseString;
        public readonly RequestStatus RequestStatus;
        public readonly long ResponseCode;

        public ResponseData(string responseJson, RequestStatus requestStatus, long responseCode)
        {
            ResponseString = responseJson;
            RequestStatus = requestStatus;
            ResponseCode = responseCode;
        }

        public override string ToString()
        {
            return $"Request Status {RequestStatus}, \n Code {ResponseCode}, \n Data {ResponseString} ";
        }
    }
}
