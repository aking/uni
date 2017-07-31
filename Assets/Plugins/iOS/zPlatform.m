//
//  zPlatform.m
//  amino
//
//  Created by Adam King on 2016-01-18.
//  Copyright © 2016 Adam King. All rights reserved.
//

#import "zPlatform.h"
#import <Foundation/Foundation.h>

//=============================================================================
//             _
//    _____  _| |_ ___ _ __ _ __
//   / _ \ \/ / __/ _ \ '__| '_ \
//  |  __/>  <| ||  __/ |  | | | |
//   \___/_/\_\\__\___|_|  |_| |_|
//
//=============================================================================

//-----------------------------------------------------------------------------
void zPlatform_debugPrint(const char* _text) {
    NSLog(@"%s", _text);
    //fprintf(stderr, "%s", _text);
}

int get_library_dir(char* _buffer, size_t _len) {
    snprintf(_buffer, _len, "lib");
    return 0;
}
