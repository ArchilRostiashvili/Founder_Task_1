namespace BebiLibs.GameApplicationConfig
{
    public interface IApplicationConfig
    {
        string GetAppShareURl();
        string GetAssociatedDomain();
        string GetBundleID();
        string GetGroupID();
        string GetInitDeepLinkUrl(string from = null);
        string GetOpenDeepLinkUrl(string from = null);
        string GetPrivacyPolicyUrl();

        string GetStoreSpecificID();
        string GetStoreUrl();
        string GetTermsOfUseUrl();

        NotificationConfig GetNotificationConfig();
        StoreAccessConfig GetStoreAccessData();
    }
}
