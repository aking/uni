//
//  WebSocketImpl
//  uni
//
//  Created by Adam King on 2017-07-29.
//  Copyright © 2017 AntiMass. All rights reserved.
//

#import "WebSocketImpl.h"

#if !TARGET_OS_IOS
  @import Foundation;
#else
  #import <Foundation/Foundation.h>
#endif
#import "zLog.h"

#define INFO(...) _INFO("WebSocketImpl", __VA_ARGS__)
#define WARN(...) _WARN("WebSocketImpl", __VA_ARGS__)

static struct lws_context* s_lwsContext = NULL;
struct per_session_data__dumb_increment {
  int number;
};

static int
callback_http(struct lws* _wsi, enum lws_callback_reasons _reasons, void* _user, void* _in, size_t _len) {
  INFO("HELLO:%d", _reasons);
  //NSLog(@"[WebSocketImpl:callback_http] CALLED: %d : %zu", _reasons, _len);
  switch (_reasons) {
    case LWS_CALLBACK_GET_THREAD_ID:
      break;
      
    default:
      INFO("IGNORED: %d : %zu", _reasons, _len);
      break;
  }
  return 0;
}

static int
callback_ws(struct lws* _wsi, enum lws_callback_reasons _reasons, void* _user, void* _in, size_t _len) {
  INFO("CALLED: %d : %zu", _reasons, _len);
  return 0;
}

static struct lws_protocols s_protocols[] = {
  // first protocol must always be HTTP handler
  {
    "http-only",
    callback_http,
    0,
    0,
    0, NULL
  },
  {
    "trax",
    callback_ws,
    sizeof(struct per_session_data__dumb_increment),
    0x10,
    0, NULL
  },
  {
    NULL, NULL, 0, 0, 0, NULL   // end of list
  }
};

void _init() {
  INFO("HELLO WORLD!");

  if(s_lwsContext == NULL) {
    struct lws_context_creation_info info;
    memset(&info, 0, sizeof(info));
    info.port = 9044;
    info.iface = NULL;
    info.protocols = s_protocols;
    info.gid = -1;
    info.uid = -1;
    info.extensions = NULL;   // DEPRECATED: lws_get_internal_extensions();
    s_lwsContext = lws_create_context(&info);
    if(s_lwsContext == NULL) {
      WARN("FAILED to create lwsContext2");
    } else {
      INFO("SUCCESS to create lwsContext");
    }
  } else {
    INFO("WebSocket already created!");
  }
}

void _tick() {
  if(s_lwsContext != NULL) {
    lws_service(s_lwsContext, 0);
  }
}

