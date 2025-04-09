#import <SafariServices/SafariServices.h>

extern "C" void OpenInSafariView(const char* urlString) {
    NSString *urlStr = [NSString stringWithUTF8String:urlString];
    UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    SFSafariViewController *safariVC = [[SFSafariViewController alloc] initWithURL:[NSURL URLWithString:urlStr]];
    [rootVC presentViewController:safariVC animated:YES completion:nil];
}
