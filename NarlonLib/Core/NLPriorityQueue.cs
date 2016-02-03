using System;
using System.Collections;
using System.Collections.Generic;

namespace NarlonLib.Core
{
    class NLUXPriorityQueue<T> : IEnumerable<T>
    {
        IComparer<T> comparer;
        T[] heap;

        private int count;
        public int Count
        {
            get { return count; }            
        }

        public NLUXPriorityQueue() : this(null) { }
        public NLUXPriorityQueue(int capacity) : this(capacity, null) { }
        public NLUXPriorityQueue(IComparer<T> comparer) : this(16, comparer) { }

        public NLUXPriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = (comparer == null) ? Comparer<T>.Default : comparer;
            this.heap = new T[capacity];
        }

        public void Push(T v)
        {
            if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
            heap[Count] = v;
            SiftUp(count++);
        }

        public T Pop()
        {
            T v = Top();
            heap[0] = heap[--count];
            if (Count > 0) SiftDown(0);
            return v;
        }

        public T Top()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }

        void SiftUp(int n)
        {
            T v = heap[n];
            for (int n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) heap[n] = heap[n2];
            heap[n] = v;
        }

        void SiftDown(int n)
        {
            T v = heap[n];
            for (int n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
            {
                if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
                if (comparer.Compare(v, heap[n2]) >= 0) break;
                heap[n] = heap[n2];
            }
            heap[n] = v;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T element in heap)
            {
                if (null == element)
                {
                    continue;
                }
                yield return element;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T element in heap)
            {
                if (null == element)
                {
                    continue;
                }
                yield return element;
            }
        }
    }
}
