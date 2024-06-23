using BebiLibs.RegistrationSystem.Remote;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    public static class RegistrationExtensions
    {
        public static AppProductType ToAppProduct(this LocalPurchaseType value)
        {
            switch (value)
            {
                case LocalPurchaseType.Consumable:
                    return AppProductType.Consumable;
                case LocalPurchaseType.NonConsumable:
                    return AppProductType.NonConsumable;
                case LocalPurchaseType.Subscription:
                    return AppProductType.Subscription;
                default:
                    throw new System.Exception("Enum Type is not defined");
            }
        }

        public static LocalPurchaseType ToPurchaseType(this AppProductType value)
        {
            switch (value)
            {
                case AppProductType.Consumable:
                    return LocalPurchaseType.Consumable;
                case AppProductType.NonConsumable:
                    return LocalPurchaseType.NonConsumable;
                case AppProductType.Subscription:
                    return LocalPurchaseType.Subscription;
                default:
                    throw new System.Exception("Enum Type is not defined");
            }
        }
    }
}
