using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ==========================================================================================
// this script is just an etension if we ever want to make choices in the dialogue and stuff
// ==========================================================================================
namespace DIALOGUE
{
    public class ConversationQueue
    {
        private Queue<Conversation> conversationQueue = new Queue<Conversation>();

        // this is the very top of the queue or the the very first priority
        // we're just going to see and get the first conversation in the queue
        public Conversation top => conversationQueue.Peek();

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);

        // if we ever integrate a choice system, we need a priority enqueue, to add the conversation at the top of the queue
        public void EnqueuePriority(Conversation conversation)
        {
            // since we cant modify the order of a queue, we just make a new queue and put this queue with the priority at the top of the queue
            Queue<Conversation> queue = new Queue<Conversation>();
            // enqueue the conversation we passed in, so now this one is first in the queue
            queue.Enqueue(conversation);


            while (conversationQueue.Count > 0)
                queue.Enqueue(conversationQueue.Dequeue());

            conversationQueue = queue;
        }

        public void Dequeue()
        {
            if (conversationQueue.Count > 0)
                conversationQueue.Dequeue();
        }

        public bool isEmpty() => conversationQueue.Count == 0;

        public void Clear() => conversationQueue.Clear();

    }

}