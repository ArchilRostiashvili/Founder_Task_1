using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [System.Serializable]
    public class PersistentVariable
    {
        protected readonly string _parameterID;
        protected readonly string _parameterLockID;
        protected bool _isValueUpdated = false;
        public bool IsLocked => PlayerPrefs.GetInt(_parameterLockID, 1) == 0;

        public bool isInitialized => PlayerPrefs.HasKey(_parameterID);

        public PersistentVariable(string parameterID)
        {
            _parameterID = parameterID;
            _parameterLockID = parameterID + "_Lock";
        }

        public void EraseKey()
        {
            PlayerPrefs.DeleteKey(_parameterID);
            PlayerPrefs.DeleteKey(_parameterLockID);
        }

        protected int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        public void Lock()
        {
            PlayerPrefs.SetInt(_parameterLockID, 0);
        }

        public void Unlock()
        {
            PlayerPrefs.SetInt(_parameterLockID, 1);
        }
    }
}
