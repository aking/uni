//
//  WebBrowser.m
//  Ava
//
//  Created by Adam King on 2016-12-21.
//  Copyright © 2016 AntiMass. All rights reserved.
//

#import "WebBrowser.h"

#if !TARGET_OS_IOS
@import Foundation;
static NSWindow* _webWindow = nil;
#else
#import <Foundation/Foundation.h>
#endif

@implementation ViewController

- (void)loadView {
  NSLog(@"[VC:loadView] ");
  NSURL* url = [NSURL URLWithString:@"https://google.ca"];
  
#if !TARGET_OS_IOS
  _webView = [[WKWebView alloc] initWithFrame:NSMakeRect(0, 0, 600, 800)];
  _webView.autoresizingMask = NSViewWidthSizable | NSViewHeightSizable;
  _webView.wantsLayer = YES;
  //[_webWindow.contentView addSubview:_webView];
  self.view = _webView;
  
  [_webView loadRequest:[NSURLRequest requestWithURL:url]];
#endif
}

- (void)viewWillAppear {
  NSLog(@"[VC:viewWillAppear] ");
}

@end

int openBrowser() {
  NSLog(@"[WebBrowser:openBrowser] Called5");
  
#if !TARGET_OS_IOS
  if(_webWindow != nil)
  {
    [_webWindow makeKeyAndOrderFront:nil];
    return 11;
  }

  ViewController* vc = [[ViewController alloc] init];
  
  NSRect frame = NSMakeRect(0, 0, 400, 800);
  _webWindow = [[NSWindow alloc] initWithContentRect:frame styleMask:NSWindowStyleMaskResizable|NSWindowStyleMaskTitled backing:NSBackingStoreBuffered defer:NO];
 // _webWindow = [[NSWindow alloc] init];
  _webWindow.showsResizeIndicator = YES;
  _webWindow.contentViewController = vc;
  
  [_webWindow makeKeyAndOrderFront:nil];
#endif
    
  return 47;
  
}

void closeBrowser() {
#if !TARGET_OS_IOS
  [_webWindow orderOut:nil];
#endif
}

