using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GCO.Utility
{
    internal class ConcurrentDictionaryOfQueues<T>
    {
        public ConcurrentDictionaryOfQueues(int maxItemNumberPerQueue, Action<T> onDequeueAction, Func<T, bool> shouldDequeue)
        {
            this.onDequeueAction = onDequeueAction;
            this.shouldDequeue = shouldDequeue;
            this.maxItemNumberPerQueue = maxItemNumberPerQueue;

            currentQueue = new Queue<T>();
            queueDicionary = new ConcurrentDictionary<Guid, Queue<T>>();
        }

        private readonly Action<T> onDequeueAction;
        private readonly Func<T, bool> shouldDequeue;
        private readonly int maxItemNumberPerQueue;

        private readonly Queue<T> currentQueue;
        private readonly ConcurrentDictionary<Guid, Queue<T>> queueDicionary;

        internal void ProcessNext()
        {
            if (!queueDicionary.IsEmpty)
            {
                IterateDictionary();
            }
            else
            {
                IterateQueue(currentQueue);
            }
        }

        internal void AddToNextAvailableQueue(T item)
        {
            if (currentQueue.Count < maxItemNumberPerQueue)
            {
                currentQueue.Enqueue(item);
            }
            else
            {
                var queue = new Queue<T>(currentQueue);
                var notAdded = true;

                while (notAdded)
                {
                    notAdded = !queueDicionary.TryAdd(Guid.NewGuid(), queue);
                }
                currentQueue.Clear();
            }
        }

        private void IterateDictionary()
        {
            if (!queueDicionary.IsEmpty)
            {
                foreach (var item in queueDicionary)
                {
                    var success = queueDicionary.TryGetValue(item.Key, out Queue<T> queue);

                    if (success)
                    {
                        IterateQueue(queue);
                        if (queue.Count < 0)
                        {
                            queueDicionary.TryRemove(item.Key, out _);
                        }
                    }
                }
            }
        }

        private void IterateQueue(Queue<T> queue)
        {
            if (queue.Count > 0)
            {
                if (shouldDequeue(queue.Peek()))
                {
                    var queueItem = queue.Dequeue();
                    onDequeueAction(queueItem);
                }
            }
        }
    }
}
