//
//  zLog.h
//  amino
//
//  Created by Adam King on 2016-01-18.
//  Copyright © 2016 Adam King. All rights reserved.
//

#pragma once

#include <assert.h>
#include <stdarg.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <libgen.h>

#define ENABLE_INFO 1
#define ENABLE_INFO_IF 1
#define ENABLE_WARN 1
#define ENABLE_ASSERT 1

#define s_logSize 2048

#ifdef __cplusplus
//extern "C" {
#endif
    void zPlatform_debugPrint(const char* _text);
#ifdef __cplusplus
//}
#endif

static const size_t s_maxLogBuffers = 4;
static char* s_logBuffers[s_maxLogBuffers][s_logSize];
static size_t s_currentLogBuffer = 0;

static char* logOutput(const char* _msg, ... )
{
    char* buffer = (char*)s_logBuffers[s_currentLogBuffer];
    va_list arg;
    va_start(arg, _msg);
    vsnprintf(buffer, s_logSize, _msg, arg);
    va_end(arg);
    
    // Verify the buffer is valid...
    if(buffer[0] == 0)
    {
        printf("[zLog:logOutput] Warning: buffer is 0 length!");
    }
    s_currentLogBuffer++;
    if(s_currentLogBuffer>=s_maxLogBuffers)
        s_currentLogBuffer=0;
    return buffer;
}
#define __FILENAME__ (strrchr(__FILE__, '/') ? strrchr(__FILE__, '/') + 1 : __FILE__)
//#define PRINTFILE() { char buf[] = __FILE__; printf("Filename:  %s\n", basename(buf)); }

#if ENABLE_INFO
#define _INFO(module, ...)                 \
{                             \
char* INFO_msg = logOutput(__VA_ARGS__);  \
char buffer__[s_logSize+1024];   \
snprintf(buffer__, sizeof(buffer__), "INFO[%s:%s:%d] %s\n",     \
__FILENAME__, __FUNCTION__,                   \
__LINE__, INFO_msg );       \
zPlatform_debugPrint(buffer__);  \
}
#else
#define _INFO(...) {}
#endif

#if ENABLE_INFO_IF
#define _INFO_IF(cond, module, ...)       \
if(cond) {                              \
char* INFO_msg = logOutput(__VA_ARGS__); \
char buffer[s_logSize+1024];   \
snprintf(buffer, sizeof(buffer), "INFO[%s:%s:%d] %s\n",       \
module, __FUNCTION__,             \
__LINE__, INFO_msg );             \
zPlatform_debugPrint(buffer);         \
}
#else
#define _INFO_IF(...) {}
#endif

#if ENABLE_WARN
#define _WARN(module, ...)                    \
{                             \
char* WARN_msg = logOutput(__VA_ARGS__);     \
char buffer[s_logSize+1024];   \
snprintf(buffer, sizeof(buffer), "WARN[%s:%s:%d] %s\n",     \
module, __FUNCTION__,           \
__LINE__, WARN_msg);       \
zPlatform_debugPrint(buffer);         \
}
#else
#define _WARN(...) {}
#endif

#if ENABLE_ASSERT

#define ASSERT(atest, ...)                                        \
{                                                                 \
if(!(atest))                                                    \
{                                                               \
char* ASSERT_msg = logOutput(__VA_ARGS__);                         \
char buffer[s_logSize+1024];   \
snprintf(buffer, sizeof(buffer), "\n%s\nASSERT[%s] %s \n%s\n[%s:%s:%d]\n%s\n\n",      \
"=====================================================",\
__FUNCTION__, ASSERT_msg,                                      \
"-----------------------------------------------------",\
__FILE__,                                               \
__PRETTY_FUNCTION__, __LINE__,                          \
"=====================================================" \
);                                                      \
zPlatform_debugPrint(buffer);                               \
assert(0);                                                  \
}                                                               \
}

#else
#define ASSERT(...) {}
#endif
