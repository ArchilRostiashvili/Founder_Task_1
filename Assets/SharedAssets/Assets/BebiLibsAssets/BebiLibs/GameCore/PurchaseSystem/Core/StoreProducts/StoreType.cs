using System;

[Flags]
public enum StoreType
{
    NotSpecified = 0,
    GooglePlay = 1,
    AmazonAppStore = 2,
    UDP = 3,
    MacAppStore = 4,
    AppleAppStore = 5,
    WinRT = 6,
    fake = 7
}