#import <Foundation/Foundation.h>    

extern "C"
{ 
    @interface SampleClass:NSObject
      +(BOOL) isAppInstalled:(NSURL *)schema;
    @end

  @implementation SampleClass
      + (BOOL)isAppInstalled:(NSURL * )schema {
          BOOL value = [[UIApplication sharedApplication] canOpenURL:schema];
            NSLog(@"Is %@ -> Installed: %s", schema, value ? "True" : "False");
            return value;
      }
  @end


    bool isAppInstalled(const char * url)
    {
        NSString * urlData = [[NSString alloc] initWithUTF8String:url];
        //NSString * finalURL = [urlData stringByAppendingFormat:@"%@", @"://"];
        NSURL * URL = [NSURL URLWithString: urlData];
        return [SampleClass isAppInstalled:URL];
    }
}