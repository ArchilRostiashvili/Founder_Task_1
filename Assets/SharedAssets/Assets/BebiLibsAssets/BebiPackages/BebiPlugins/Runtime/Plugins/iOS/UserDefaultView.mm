#import <Foundation/Foundation.h>
#import "UnityAppController.h"
#import <UIKit/UIKit.h>


@interface UserDefaultView : NSObject

@end

@implementation UserDefaultView
    
    NSUserDefaults * sharedDefaults;
        
    +(void)initSharingSystem:(NSString*)groupID{
        NSLog(@"Initialize File Sharing System");
        sharedDefaults = [[NSUserDefaults alloc] initWithSuiteName:groupID];
        [sharedDefaults synchronize];
    }

    +(void)writeSharedString:(NSString*)key addValue:(NSString*) value
    {
        if(sharedDefaults == nil){
            NSLog(@"Unable to write key: %@ ", key);
            return;
        }
        
        //NSString myString = (myString.length > 20) ? [myString substringToIndex:20] : myString;
        //NSLog(@"Setting key: %@ with value %@", key, value);
        [sharedDefaults setObject:value forKey:key];
        [sharedDefaults synchronize];
    }
    
    +(NSString *)readSharedString:(NSString*)key addValue:(NSString*) value
    {
        if(sharedDefaults == nil){
            NSLog(@"Unable to read key: %@", key);
            return @"";
        }
        
        //NSLog(@"Reading value from key: %@", key );
        
        NSString * newValue = [sharedDefaults stringForKey:key];
        if(newValue == nil){
            NSLog(@"Value returned from the shared defauls is nill");
            return @"";
        }
        
        return newValue;
    }

    +(NSString *)readSharedStringFromOtherApp:(NSString*) bundleID keyValue: (NSString*)key addValue:(NSString*) value
    {
        if(sharedDefaults == nil){
            NSLog(@"Unable To Read key: %@", key);
            return @"";
        }
        
        //NSLog(@"Reading value from key: %@", key );
        NSString * newValue = [sharedDefaults stringForKey:key];
        if(newValue == nil){
            NSLog(@"Value returned from the shared defauls is nill");
            return @"";
        }
        
        return newValue;
    }
    
@end


extern "C"
{
    void initSharingSystem(const char *groupID){
        NSString * groupIDString = [NSString stringWithUTF8String:groupID];
        [UserDefaultView initSharingSystem:groupIDString];
    }
    
    void writeSharedString(const char *key, const char *value)
    {
        [UserDefaultView writeSharedString:[NSString stringWithUTF8String:key] addValue:[NSString stringWithUTF8String:value]];
    }

    char* readSharedString(const char *key, const char *defaultValue)
    {
        NSString * keyString = [NSString stringWithUTF8String:key];
        NSString * defaultValueString = [NSString stringWithUTF8String:defaultValue];
        NSString * prefValue = [UserDefaultView readSharedString:keyString addValue:defaultValueString];
        const char * szValue = [prefValue UTF8String];
        return strdup(szValue);
    }

    char* readSharedStringFromOtherApp(const char *bundleID, const char *key, const char *defaultValue)
    {
        NSString * bundleIDString = [NSString stringWithUTF8String:bundleID];
        NSString * keyString = [NSString stringWithUTF8String:key];
        NSString * defaultValueString = [NSString stringWithUTF8String:defaultValue];
        
        
        NSString * prefValue = [UserDefaultView readSharedStringFromOtherApp:bundleIDString keyValue: keyString addValue:defaultValueString];
        const char * szValue = [prefValue UTF8String];
        return strdup(szValue);
    }

}
