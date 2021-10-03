using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class SortedQueue<TPriority, TElement> : IEnumerable<TElement> {
    SortedDictionary<TPriority, Queue<TElement>> queues = new SortedDictionary<TPriority, Queue<TElement>>();

    public int Count { get; private set; } = 0;

    public void Enqueue(TPriority priority, TElement element) {
        Queue<TElement> queue;
        if (!queues.TryGetValue(priority, out queue)) {
            queue = queues[priority] = new Queue<TElement>();
        }

        Count++;
        queue.Enqueue(element);
    }

    public void Clear() {
        queues.Clear();
    }

    public TElement Dequeue() {
        if (Count == 0) throw new InvalidOperationException();

        Count--;
        var key = queues.Keys.First();
        var queue = queues[key];
        var elem = queue.Dequeue();

        if (queue.Count == 0) {
            queues.Remove(key);
        }

        return elem;
    }

    public TPriority FirstKey => queues.Keys.First();

    public IEnumerator<TElement> GetEnumerator() {
        foreach (var q in queues.Values) {
            foreach (var item in q) {
                yield return item;
            }
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
