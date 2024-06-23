using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    public abstract class BasePurchaseMergeHandler : MonoBehaviour
    {
        [HideInInspector] public bool IsActive = true;
        public abstract void MergePurchases();
    }
}
