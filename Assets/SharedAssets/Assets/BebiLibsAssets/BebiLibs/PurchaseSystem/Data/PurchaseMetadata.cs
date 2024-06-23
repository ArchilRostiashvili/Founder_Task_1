using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [System.Serializable]
    public class PurchaseMetadata
    {
        public string LocalizedPriceString;
        public string LocalizedTitle;
        public string LocalizedDescription;
        public string IsoCurrencyCode;
        public decimal LocalizedPrice;

        public string localizedPriceWithCurrency => LocalizedPrice.ToString("F", new System.Globalization.CultureInfo("en-US")) + " " + IsoCurrencyCode;
        public string localizedPriceString => LocalizedPrice.ToString("F", new System.Globalization.CultureInfo("en-US"));
        public string isoCurrency => IsoCurrencyCode;
        public decimal localizedPrice => LocalizedPrice;

        public static PurchaseMetadata Empty
        {
            get
            {
                PurchaseMetadata purchaseMetadata = new PurchaseMetadata();
                purchaseMetadata.ResetData();
                return purchaseMetadata;
            }
        }


        public void SetEmptyData()
        {
            LocalizedPriceString = "□□□□";
            LocalizedTitle = "Load Error";
            LocalizedDescription = "Load Error";
            IsoCurrencyCode = "□□□";
            LocalizedPrice = 0;
        }

        public void ResetData()
        {
            LocalizedPriceString = string.Empty;
            LocalizedTitle = string.Empty;
            LocalizedDescription = string.Empty;
            IsoCurrencyCode = string.Empty;
            LocalizedPrice = 0;
        }

        public PurchaseMetadata Copy()
        {
            return new PurchaseMetadata()
            {
                LocalizedPriceString = LocalizedPriceString,
                LocalizedTitle = LocalizedTitle,
                LocalizedDescription = LocalizedDescription,
                IsoCurrencyCode = IsoCurrencyCode,
                LocalizedPrice = LocalizedPrice
            };
        }

        public string ToString(string indent)
        {
            string compText = "";
            compText += indent + nameof(LocalizedPriceString) + ": " + LocalizedPriceString.ToString() + "\n";
            compText += indent + nameof(LocalizedTitle) + ": " + LocalizedTitle.ToString() + "\n";
            compText += indent + nameof(LocalizedDescription) + ": " + LocalizedDescription.ToString() + "\n";
            compText += indent + nameof(IsoCurrencyCode) + ": " + IsoCurrencyCode.ToString() + "\n";
            compText += indent + nameof(LocalizedPrice) + ": " + LocalizedPrice.ToString() + "\n";
            return compText;
        }

        public override string ToString()
        {
            return ToString("");
        }

        public bool Equals(PurchaseMetadata other)
        {
            if (other is null) return false;
            if (System.Object.ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            bool lps = LocalizedPriceString != other.LocalizedPriceString;
            bool lt = LocalizedTitle != other.LocalizedTitle;
            bool ld = LocalizedDescription != other.LocalizedDescription;
            bool lcc = IsoCurrencyCode != other.IsoCurrencyCode;
            bool lp = LocalizedPrice != other.LocalizedPrice;
            return !(lps || lt || ld || lcc || lp);
        }

        public static bool operator ==(PurchaseMetadata lhs, PurchaseMetadata rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public override bool Equals(object obj) => Equals(obj as PurchaseMetadata);
        public override int GetHashCode() => (LocalizedDescription, LocalizedPrice, LocalizedPriceString, LocalizedTitle, IsoCurrencyCode).GetHashCode();
        public static bool operator !=(PurchaseMetadata lhs, PurchaseMetadata rhs) => !(lhs == rhs);
    }
}
