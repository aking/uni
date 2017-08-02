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

#import "libwebsockets.h"
#import "zLog.h"
#import "zQueue.h"

#define INFO(...) _INFO("WebSocketImpl", __VA_ARGS__)
#define WARN(...) _WARN("WebSocketImpl", __VA_ARGS__)

static const char* s_webSocketImplVersion = "0.2.0";
static const char* s_buildDate = __DATE__;

static struct lws_context* s_lwsContext = NULL;
static struct lws* s_webSocket = NULL;

static tQueue* s_receiveQueue = NULL;
static tQueue* s_writeQueue = NULL;

struct QueueData {
  size_t bufferLen;
  size_t writeIndex;    // the number of bytes already written
  unsigned char* buffer;
};
typedef struct QueueData tQueueData;

//-----------------------------------------------------------------------------
struct ws_session_data {
  size_t packetLength;
  size_t idx;
  char* buffer;
};

//-----------------------------------------------------------------------------
static int
callback_http(struct lws* _wsi, enum lws_callback_reasons _reasons, void* _user,
              void* _in, size_t _len)
{
  //INFO("CALLED: %d : %zu", _reasons, _len);
  switch (_reasons) {
    case LWS_CALLBACK_PROTOCOL_INIT:
      INFO("Initializing HTTP Protocol");
      break;
      
    case LWS_CALLBACK_GET_THREAD_ID:
    case LWS_CALLBACK_ADD_POLL_FD:
    case LWS_CALLBACK_LOCK_POLL:
    case LWS_CALLBACK_UNLOCK_POLL:
      break;
      
    default:
      INFO("IGNORED: %d : %zu", _reasons, _len);
      break;
  }
  return 0;
}

//-----------------------------------------------------------------------------
static int
callback_ws(struct lws* _wsi, enum lws_callback_reasons _reasons, void* _user,
            void* _in, size_t _len)
{
  switch (_reasons) {
    case LWS_CALLBACK_PROTOCOL_INIT:
      INFO("Initializing WebSocket Protocol");
      break;
      
    case LWS_CALLBACK_ESTABLISHED:
      INFO("Callback establised");
      s_webSocket = _wsi;
      break;
      
    case LWS_CALLBACK_CLIENT_WRITEABLE:
      INFO("Callback Client Writeable");
      break;
      
    case LWS_CALLBACK_SERVER_WRITEABLE: {
      INFO("Server Writable");
      tQueueData* data = (tQueueData*)get_msg(s_writeQueue);
      if(data != NULL) {
        int bytesWritten = lws_write(_wsi, &(data->buffer[LWS_PRE]),
                                     data->bufferLen, LWS_WRITE_TEXT);
        
        INFO("Server Writable: bytes to write:%zu  bytes written:%d",
             data->bufferLen, bytesWritten);
        
        free(data->buffer);
        free(data);
      }
      break;
    }
      
    case LWS_CALLBACK_RECEIVE: {
      if(_len == 0) {
        INFO("No data received");
        return -1;
      }
      size_t remainingBytes = lws_remaining_packet_payload(_wsi);
      INFO("REMAINING: %zu", remainingBytes);
      
      struct ws_session_data* pktData = (struct ws_session_data*)_user;
      if(pktData->packetLength == 0) {
        // new packet
        if(remainingBytes == 0) {
          INFO("Received (SOLO): %s", (const char*)_in);
          char* inBuf = (char*)malloc(_len+1);
          memcpy(inBuf, _in, _len);
          inBuf[_len] = 0;
          post_msg(s_receiveQueue, inBuf);
        } else {
          INFO("Received (START): %s", (const char*)_in);
          pktData->packetLength = remainingBytes + _len;
          pktData->idx = _len;
          pktData->buffer = (char*)malloc(pktData->packetLength + 1);
          memset(pktData->buffer, 0, pktData->packetLength);
          memcpy(pktData->buffer, _in, _len);
        }
      } else {
        // packet done?
        if(remainingBytes == 0) {
          memcpy(pktData->buffer + pktData->idx, _in, _len);
          pktData->buffer[pktData->packetLength] = 0;
          pktData->packetLength = 0;
          pktData->idx = 0;
          INFO("Received (FULL_COMPLETE: %s", pktData->buffer);
          post_msg(s_receiveQueue, pktData->buffer);
          pktData->buffer = NULL;
        } else {
          // not done
          INFO("Received (INPROGRESS): %s", (const char*)_in);
          memcpy(pktData->buffer + pktData->idx, _in, _len);
          pktData->idx += _len;
        }
      }
      ASSERT(pktData->packetLength >= pktData->idx, "Packet OVERRUN");
      break;
    }
      
    default:
      INFO("Unhandled: %d : %zu", _reasons, _len);
      break;
  }
  
  return 0;
}

//-----------------------------------------------------------------------------
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
    sizeof(struct ws_session_data),
    0x10,
    0, NULL
  },
  {
    NULL, NULL, 0, 0, 0, NULL   // end of list
  }
};

//-----------------------------------------------------------------------------
void _init(int _port) {
  INFO("WebSocket Initialization (%s:[%s])", s_webSocketImplVersion, s_buildDate);
  
  if(s_lwsContext == NULL) {
    s_receiveQueue = create_queue();
    s_writeQueue = create_queue();
    
    struct lws_context_creation_info info;
    memset(&info, 0, sizeof(info));
    info.port = _port;
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

//-----------------------------------------------------------------------------
void _tick() {
  if(s_lwsContext != NULL) {
    lws_service(s_lwsContext, 0);
  }
}

//-----------------------------------------------------------------------------
const char* _getData() {
  if(has_msg(s_receiveQueue) > 0) {
    return get_msg(s_receiveQueue);
  }
  
  return NULL;
}

//-----------------------------------------------------------------------------
void _dispatch(const char* _msg) {
  INFO("Dispatch: %s", _msg);
  
  if(s_webSocket) {
    size_t msgLen = strlen(_msg);
    size_t bufLen = LWS_PRE + msgLen;
    unsigned char* buf = (unsigned char*)malloc(bufLen);
    memset(buf, 0, bufLen);
    memcpy(&buf[LWS_PRE], _msg, msgLen);
    
    tQueueData* dataInfo = (tQueueData*)malloc(sizeof(tQueueData));
    dataInfo->writeIndex = 0;
    dataInfo->bufferLen = msgLen;
    dataInfo->buffer = buf;
    
    post_msg(s_writeQueue, dataInfo);
    lws_callback_on_writable(s_webSocket);
  } else {
    WARN("No WebSocket to send too");
  }
}


