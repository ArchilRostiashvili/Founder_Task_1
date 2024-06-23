#import <SafariServices/SafariServices.h>
#import <UIKit/UIKit.h>


extern UIViewController * UnityGetGLViewController();
extern void UnitySendMessage( const char * className, const char * methodName, const char * param );
 
extern "C"
{


  @interface SafariViewController : UIViewController<SFSafariViewControllerDelegate>
 
  @end
 
  @implementation SafariViewController
  - (void)safariViewControllerDidFinish:(SFSafariViewController *)controller {
    NSLog(@"safariViewControllerDidFinish");
    UnitySendMessage("BebiPlugins", "OnBrowserClosed", "");
  }
  - (void)safariViewController:(SFSafariViewController *)controller didCompleteInitialLoad:(BOOL)didLoadSuccessfully{
    NSLog(@"safariViewController didCompleteInitialLoad");
    UnitySendMessage("BebiPlugins", "OnBrowserClosed", "value");
    }
  - (BOOL) shouldAutorotate {
    return YES;
    }

  - (UIInterfaceOrientationMask)supportedInterfaceOrientations {
    return (UIInterfaceOrientationPortrait);
    }
  @end

  SafariViewController * svc;
 
  void launchUrl(const char * url)
  {
    NSLog(@"Launching SFSafariViewController");

    // Get the instance of ViewController that Unity is displaying now
    UIViewController * uvc = UnityGetGLViewController();
 
    // [[UIDevice currentDevice] setValue:@(UIInterfaceOrientationPortrait) forKey:@"orientation"];
    // [UINavigationController attemptRotationToDeviceOrientation];

    // Generate an NSURL object based on the C string passed from C#
    NSURL * URL = [NSURL URLWithString: [[NSString alloc] initWithUTF8String:url]];
 
    // Create an SFSafariViewController object from the generated URL
    SFSafariViewController * sfvc = [[SFSafariViewController alloc] initWithURL:URL];
    
    // Assign a delegate to handle when the user presses the 'Done' button
    svc = [[SafariViewController alloc] init];
    sfvc.delegate = svc;
 
    // Start the generated SFSafariViewController object
    [uvc presentViewController:sfvc animated:YES completion:nil];
    //[uvc.tabBarController.tabBar setHidden:YES];
 
    NSLog(@"Presented SFSafariViewController");
  }
 
  void dismiss()
  {
    UIViewController * uvc = UnityGetGLViewController();
    [uvc dismissViewControllerAnimated:YES completion:nil];
  }
}