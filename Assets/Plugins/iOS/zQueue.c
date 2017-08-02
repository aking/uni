#include "zQueue.h"

#include "zLog.h"
#include <stdlib.h>

#define INFO(...) _INFO("zQueue", __VA_ARGS__)

tQueue* create_queue()
{
    tQueue* q = (tQueue*)malloc(sizeof(tQueue));
    
    pthread_mutex_init(&(q->mutex), NULL);
    q->root = NULL;
    q->msg_count = 0;
    q->total_count = 0;
    
    return q;
}

tQueueMsg* create_msg(void* _data)
{
    tQueueMsg* m = (tQueueMsg*)malloc(sizeof(tQueueMsg));
    m->next = NULL;
    m->data = _data;
    
    return m;
}

void post_msg(tQueue* _q, void* _m)
{
    pthread_mutex_lock(&(_q->mutex));
    tQueueMsg* qm = create_msg(_m);
    
    size_t msg_count = 0;
    
    qm->next = NULL;
    if(_q->root == NULL)
    {
        _q->root = qm;
    }
    else
    {
        msg_count++;
        tQueueMsg* child = _q->root;
        while(child->next != NULL)
        {
            msg_count++;
            child = child->next;
        }
        child->next = qm;
    }
    msg_count++;
    _q->msg_count = msg_count;
    
    //INFO("msg count:%zu", msg_count);
    
    _q->total_count++;
    pthread_mutex_unlock(&(_q->mutex));
}

int has_msg(tQueue* _queue) {
    return (_queue->root != NULL);
}

void* get_msg_newest(tQueue* _q)
{
    pthread_mutex_lock(&(_q->mutex));
    
    tQueueMsg* parent = NULL;
    tQueueMsg* msg = _q->root;
    
    if(msg != NULL)
    {
        ASSERT(_q->msg_count > 0, "Invalid msg counts");
        while(msg->next != NULL)
        {
            parent = msg;
            msg = msg->next;
        }
        
        if(parent == NULL)
            _q->root = NULL;
        else
            parent->next = NULL;
    } else {
        ASSERT(_q->msg_count == 0, "We have NO msg, but count says we do:%d", _q->msg_count);
    }
    
    void* data = NULL;
    if(msg)
    {
        data = msg->data;
        _q->msg_count--;
        free(msg);
    }
    
    pthread_mutex_unlock(&(_q->mutex));
    return data;
}

void* get_msg(tQueue* _q)
{
    pthread_mutex_lock(&(_q->mutex));
    
    void* data = NULL;
    tQueueMsg* msg = _q->root;
    if(msg != NULL)
    {
        ASSERT(_q->msg_count > 0, "Invalid msg counts");
        _q->root = msg->next;
        _q->msg_count--;
        data = msg->data;
        free(msg);
    } else {
        ASSERT(_q->msg_count == 0, "We have NO msg, but count says we do:%d", _q->msg_count);
    }
    
    pthread_mutex_unlock(&(_q->mutex));
    return data;
}
