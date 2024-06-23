using BebiLibs.Analytics;
using BebiLibs.PurchaseSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using BebiLibs.RegistrationSystem.Remote;
using BebiLibs.ServerConfigLoaderSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    public class SubscriptionMergeHandler : BasePurchaseMergeHandler
    {
        [Header("Game Shared Data")]
        [SerializeField] private PurchaseHistoryData _purchaseData;
        [SerializeField] private GameUserDataSO _userData;
        [SerializeField] private PurchaseManagerBase _purchaseManager;

        [Header("Registration Specific Data")]
        [SerializeField] private ServerInitData _initData;
        [SerializeField] private RemotePurchaseDataSO _remotePurchases;
        [SerializeField] private SubscriptionPostDataSO _subPostDataSO;

        public override void MergePurchases()
        {
            //Debug.Log($"Start Merging {_remotePurchases.isLoaded} {_userData.isUserSignedIn} {IsActive}");
            if (_remotePurchases.isLoaded && _userData.isUserSignedIn && IsActive)
            {
                if (!_purchaseData.HasAnyActiveSubscription)
                {
                    CheckRemoteLockState();
                }
                else if (_purchaseData.HasAnyNonExpiredSubscription)
                {
                    CheckBindingButtonState();
                }
                else
                {
                    _userData.isBindButtonEnabled = false;
                }
            }
            else
            {
                Debug.LogWarning("Subscription Merge Failed " + _remotePurchases.isLoaded + " " + _userData.isUserSignedIn + " " + !_purchaseData.HasAnyActiveSubscription);
            }
        }

        private void CheckRemoteLockState()
        {
            List<RemotePurchase> sortedPurchases = _remotePurchases.PurchasesList.OrderByDescending(x => x.expiryTime.DateTime).ToList();
            RemotePurchase activePurchase = sortedPurchases.FirstOrDefault();

            if (activePurchase != null && activePurchase.IsSubscribed)
            {
                AnalyticsManager.LogEvent("unlock_from_registration");
                _purchaseManager.ForceChangeLockState("registration_merge", true);
            }
        }


        private void CheckBindingButtonState()
        {
            bool enableBindButton = true;
            foreach (PurchaseData localPurchase in _purchaseData.PurchasesDataList)
            {
                if (localPurchase.HasReceipt && localPurchase.IsSubscribed)
                {
                    foreach (var remotePurchase in _remotePurchases.PurchasesList)
                    {
                        if (remotePurchase.partnerId == localPurchase.TransactionID || remotePurchase.partnerId.Contains(localPurchase.TransactionID))
                        {
                            enableBindButton = false;
                        }
                    }
                }
            }

            // Debug.LogWarning("Finished Checking Binding Button Statues: " + enableBindButton);
            _userData.isBindButtonEnabled = enableBindButton;
        }

    }
}
