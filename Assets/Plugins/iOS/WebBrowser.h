//
//  WebBrowser.h
//  Ava
//
//  Created by Adam King on 2016-12-21.
//  Copyright © 2016 AntiMass. All rights reserved.
//

#pragma once

#import <TargetConditionals.h>

#if !TARGET_OS_IOS
@import Cocoa;
@import WebKit;

@interface ViewController : NSViewController {
#else
#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>
    
@interface ViewController : UIViewController {
#endif
    
  WKWebView* _webView;
};

//@property (strong, nonatomic) NSWindow* window;

@end

