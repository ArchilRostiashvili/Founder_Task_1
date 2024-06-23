using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    [System.Serializable]
    public class PromotionPopUpText
    {
        public string languageID;
        public List<TextEntry> textEntries = new List<TextEntry>();

        public bool TryGetTextFromKey(string key, out string text)
        {
            TextEntry entry = textEntries.Find(x => x.textKey == key);
            if (entry != null)
            {
                text = entry.infortext;
                return true;
            }
            text = string.Empty;
            return false;
        }

        public PromotionPopUpText Clone()
        {
            return new PromotionPopUpText()
            {
                languageID = languageID,
                textEntries = CloneTextEntryes()
            };
        }


        private List<TextEntry> CloneTextEntryes()
        {
            List<TextEntry> textEntry = new List<TextEntry>();
            for (int i = 0; i < textEntries.Count; i++)
            {
                textEntry.Add(new TextEntry()
                {
                    textKey = textEntries[i].textKey,
                    infortext = textEntries[i].infortext
                });
            }
            return textEntry;
        }
    }
}

