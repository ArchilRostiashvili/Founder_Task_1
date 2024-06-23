//extern UIViewController* UnityGetGLViewController();
#import <StoreKit/StoreKit.h>

extern "C" int _IOSCheckIfThereIsAppWithScheme(const char* scheme)
{
   NSString *combined = [NSString stringWithFormat:@"%@%@", [NSString stringWithUTF8String:scheme], @"://"];
   //NSLog(@"== %@", combined);
    
   BOOL bl = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString: combined]];
   return bl?1:0;
}

extern "C" void _IOSOpenReview()
{
    //UnitySendMessage("ManagerNative", "CallBackNotification", "levan");
    [SKStoreReviewController requestReview];
}

