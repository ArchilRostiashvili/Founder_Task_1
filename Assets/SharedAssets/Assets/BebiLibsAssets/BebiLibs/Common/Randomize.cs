using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace BebiLibs
{
    public static class Randomize
    {

#if UNITY_EDITOR
        private static readonly Dictionary<string, int> _randomData = new Dictionary<string, int>();
#endif

        /// <summary>
        /// Return a random integer number between min [inclusive] and max [exclusive]
        /// In Unity Editor every call returns min value incremented by one
        /// </summary>
        /// <returns>Random Integer</returns>    
        public static int Range(int min, int max)
        {
            int value = Random.Range(min, max);
#if UNITY_EDITOR
            StackTrace stackTrace = new StackTrace(true);
            StackFrame stackFrame = stackTrace.GetFrame(1);
            string ID = stackFrame.GetMethod().Name + stackFrame.GetFileLineNumber() + stackFrame.GetFileName();
            if (_randomData.ContainsKey(ID))
            {
                if (_randomData[ID] + 1 >= max)
                {
                    _randomData[ID] = min;
                }
                else
                {
                    _randomData[ID] += 1;
                }
            }
            else
            {
                _randomData.Add(ID, min);
            }
            value = _randomData[ID];
#endif
            return value;
        }
    }
}