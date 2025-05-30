#import <SafariServices/SafariServices.h>

static SFSafariViewController *safariVC = nil;

extern "C" void OpenInSafariView(const char* urlString) {
    NSString *urlStr = [NSString stringWithUTF8String:urlString];
    UIViewController *rootVC = [UIApplication sharedApplication].keyWindow.rootViewController;
    safariVC = [[SFSafariViewController alloc] initWithURL:[NSURL URLWithString:urlStr]];
    [rootVC presentViewController:safariVC animated:YES completion:nil];
}

extern "C" void CloseSafariView() {
    if (safariVC != nil) {
        [safariVC dismissViewControllerAnimated:YES completion:^{
            safariVC = nil;
        }];
    }
}

