//
//  WebBrowser.h
//  Ava
//
//  Created by Adam King on 2016-12-21.
//  Copyright © 2016 AntiMass. All rights reserved.
//

#pragma once

@import Cocoa;
@import WebKit;

@interface ViewController : NSViewController {
  WKWebView* _webView;
};

@property (strong, nonatomic) NSWindow* window;

@end

