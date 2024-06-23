using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [CreateAssetMenu(fileName = "FormatSystem", menuName = "BebiLibs/FormatSystem/LinkFormatData", order = 0)]
    public class LinkFormatData : StringFormatBase
    {
        [SerializeField] private string _key;
        [SerializeField] private bool _isLinkEnd;
        [HideField("_isLinkEnd", false)]
        [SerializeField] private string _linkID;

        public override string key => _key;
        public override string value => _isLinkEnd ? TMPFormatterUtility.GetLinkEndMarkup() : TMPFormatterUtility.GetLinkMarkup(_linkID);
    }
}
