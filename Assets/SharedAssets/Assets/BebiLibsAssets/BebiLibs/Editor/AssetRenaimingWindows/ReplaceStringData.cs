namespace BebiLibs.EditorExtensions.AssetRenaming
{
    [System.Serializable]
    public class ReplaceStringData
    {
        public string Replace;
        public string ReplaceWith;
        public bool UseSmartReplace;

        public ReplaceStringData(string stringToReplace, string stringToReplaceWith, bool useSmartReplace = false)
        {
            Replace = stringToReplace;
            ReplaceWith = stringToReplaceWith;
            UseSmartReplace = useSmartReplace;
        }
    }
}
