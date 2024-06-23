using BebiLibs.PurchaseSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SubscriptionPanelData", menuName = "BebiToddlers/SubscriptionPanelData", order = 0)]
public class SubscriptionPanelData : ScriptableObject
{
    [SerializeField] private ProductIdentifier _baseMonthlySubscription;
    [SerializeField] private ProductIdentifier _monthlySubscription;
    [SerializeField] private ProductIdentifier _yearlySubscription;


    public ProductIdentifier MonthlySubscription => _monthlySubscription;
    public ProductIdentifier YearlySubscription => _yearlySubscription;
    public ProductIdentifier BaseMonthlySubscription => _baseMonthlySubscription;

    public List<ProductIdentifier> GetProductIdentifiers()
    {
        return new List<ProductIdentifier>() { _monthlySubscription, _yearlySubscription, _baseMonthlySubscription };
    }

    public void SetPanelData(ProductIdentifier monthlySubscription, ProductIdentifier yearlySubscription, ProductIdentifier baseMonthlySubscription)
    {
        _monthlySubscription = monthlySubscription;
        _yearlySubscription = yearlySubscription;
        _baseMonthlySubscription = baseMonthlySubscription;
    }


    public void SetPanelData(SubscriptionPanelData data)
    {
        _monthlySubscription = data.MonthlySubscription;
        _yearlySubscription = data.YearlySubscription;
        _baseMonthlySubscription = data.BaseMonthlySubscription;
    }
}