//
//  WebSocketImpl
//  uni
//
//  Created by Adam King on 2017-07-29.
//  Copyright © 2017 AntiMass. All rights reserved.
//

#pragma once

#import <TargetConditionals.h>
#import "libwebsockets.h"

#if !TARGET_OS_IOS
@import Cocoa;

#else
#import <UIKit/UIKit.h>

#endif

void _init(void);
void _tick(void);
