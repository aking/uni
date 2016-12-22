//
//  WebBrowser.m
//  Ava
//
//  Created by Adam King on 2016-12-21.
//  Copyright © 2016 AntiMass. All rights reserved.
//

#import <TargetConditionals.h>
#import "WebBrowser.h"

@import Foundation;

static NSWindow* _webWindow = nil;

@implementation ViewController

- (void)loadView {
  NSLog(@"[VC:loadView] ");
  NSURL* url = [NSURL URLWithString:@"https://google.ca"];
  
  _webView = [[WKWebView alloc] initWithFrame:NSMakeRect(0, 0, 600, 800)];
  _webView.autoresizingMask = NSViewWidthSizable | NSViewHeightSizable;
  _webView.wantsLayer = YES;
  //[_webWindow.contentView addSubview:_webView];
  self.view = _webView;
  
  [_webView loadRequest:[NSURLRequest requestWithURL:url]];
}

- (void)viewWillAppear {
  NSLog(@"[VC:viewWillAppear] ");
}

@end

int openBrowser() {
  NSLog(@"[WebBrowser:openBrowser] Called5");
  
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
  return 46;
  
}

void closeBrowser() {
  [_webWindow orderOut:nil];
}

