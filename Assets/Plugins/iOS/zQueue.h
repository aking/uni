#include <pthread.h>

#ifndef TRAX_ZQUEUE_H
#define TRAX_ZQUEUE_H

#ifdef __cplusplus
extern "C" {
#endif
    
struct QUEUE_MSG {
  struct QUEUE_MSG* next;
  void* data;
};

typedef struct QUEUE_MSG tQueueMsg;

struct QUEUE {
  pthread_mutex_t mutex;
  tQueueMsg* root;
  size_t msg_count;
  unsigned int total_count;
};

typedef struct QUEUE tQueue;

tQueue* create_queue();

int has_msg(tQueue* _queue);
void post_msg(tQueue* _queue, void* _msg);
void* get_msg(tQueue* _queue);
void* get_msg_newest(tQueue* _queue);

#ifdef __cplusplus
}
#endif

#endif  // TRAX_ZQUEUE_H
