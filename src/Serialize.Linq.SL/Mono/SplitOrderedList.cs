// SplitOrderedList.cs
//
// Copyright (c) 2010 Jérémie "Garuma" Laval
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//
using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent
{
    internal class SplitOrderedList<TKey, T>
    {
        private class Node
        {
            public bool Marked;

            public ulong Key;

            public TKey SubKey;

            public T Data;

            public Node Next;

            public Node Init(ulong key, TKey subKey, T data)
            {
                this.Key = key;
                this.SubKey = subKey;
                this.Data = data;

                this.Marked = false;
                this.Next = null;

                return this;
            }

            // Used to create dummy node
            public Node Init(ulong key)
            {
                this.Key = key;
                this.Data = default(T);

                this.Next = null;
                this.Marked = false;
                this.SubKey = default(TKey);

                return this;
            }

            // Used to create marked node
            public Node Init(Node wrapped)
            {
                this.Marked = true;
                this.Next = wrapped;

                this.Key = 0;
                this.Data = default(T);
                this.SubKey = default(TKey);

                return this;
            }
        }

        private const int MaxLoad = 5;

        private const uint BucketSize = 512;

        private Node head;

        private Node tail;

        private Node[] buckets = new Node[BucketSize];

        private int count;

        private int size = 2;

        private SimpleRwLock slim = new SimpleRwLock();

        private readonly IEqualityComparer<TKey> comparer;

        public SplitOrderedList(IEqualityComparer<TKey> comparer)
        {
            this.comparer = comparer;
            this.head = new Node().Init(0);
            this.tail = new Node().Init(ulong.MaxValue);
            this.head.Next = this.tail;
            this.SetBucket(0, this.head);
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public T InsertOrUpdate(uint key, TKey subKey, Func<T> addGetter, Func<T, T> updateGetter)
        {
            Node current;
            bool result = this.InsertInternal(key, subKey, default(T), addGetter, out current);

            if (result)
                return current.Data;

            // FIXME: this should have a CAS-like behavior
            return current.Data = updateGetter(current.Data);
        }

        public T InsertOrUpdate(uint key, TKey subKey, T addValue, T updateValue)
        {
            Node current;
            if (this.InsertInternal(key, subKey, addValue, null, out current))
                return current.Data;

            // FIXME: this should have a CAS-like behavior
            return current.Data = updateValue;
        }

        public bool Insert(uint key, TKey subKey, T data)
        {
            Node current;
            return this.InsertInternal(key, subKey, data, null, out current);
        }

        public T InsertOrGet(uint key, TKey subKey, T data, Func<T> dataCreator)
        {
            Node current;
            this.InsertInternal(key, subKey, data, dataCreator, out current);
            return current.Data;
        }

        private bool InsertInternal(uint key, TKey subKey, T data, Func<T> dataCreator, out Node current)
        {
            Node node = new Node().Init(ComputeRegularKey(key), subKey, data);

            uint b = key % (uint)this.size;
            Node bucket;

            if ((bucket = this.GetBucket(b)) == null)
                bucket = this.InitializeBucket(b);

            if (!this.ListInsert(node, bucket, out current, dataCreator))
                return false;

            int csize = this.size;
            if (Interlocked.Increment(ref this.count) / csize > MaxLoad && (csize & 0x40000000) == 0)
                Interlocked.CompareExchange(ref this.size, 2 * csize, csize);

            current = node;

            return true;
        }

        public bool Find(uint key, TKey subKey, out T data)
        {
            Node node;
            uint b = key % (uint)this.size;
            data = default(T);
            Node bucket;

            if ((bucket = this.GetBucket(b)) == null)
                bucket = this.InitializeBucket(b);

            if (!this.ListFind(ComputeRegularKey(key), subKey, bucket, out node))
                return false;

            data = node.Data;

            return !node.Marked;
        }

        public bool CompareExchange(uint key, TKey subKey, T data, Func<T, bool> check)
        {
            Node node;
            uint b = key % (uint)this.size;
            Node bucket;

            if ((bucket = this.GetBucket(b)) == null)
                bucket = this.InitializeBucket(b);

            if (!this.ListFind(ComputeRegularKey(key), subKey, bucket, out node))
                return false;

            if (!check(node.Data))
                return false;

            node.Data = data;

            return true;
        }

        public bool Delete(uint key, TKey subKey, out T data)
        {
            uint b = key % (uint)this.size;
            Node bucket;

            if ((bucket = this.GetBucket(b)) == null)
                bucket = this.InitializeBucket(b);

            if (!this.ListDelete(bucket, ComputeRegularKey(key), subKey, out data))
                return false;

            Interlocked.Decrement(ref this.count);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node node = this.head.Next;

            while (node != this.tail)
            {
                while (node.Marked || (node.Key & 1) == 0)
                {
                    node = node.Next;
                    if (node == this.tail)
                        yield break;
                }
                yield return node.Data;
                node = node.Next;
            }
        }

        private Node InitializeBucket(uint b)
        {
            Node current;
            uint parent = GetParent(b);
            Node bucket;

            if ((bucket = this.GetBucket(parent)) == null)
                bucket = this.InitializeBucket(parent);

            Node dummy = new Node().Init(ComputeDummyKey(b));
            if (!this.ListInsert(dummy, bucket, out current, null))
                return current;

            return this.SetBucket(b, dummy);
        }

        // Turn v's MSB off
        private static uint GetParent(uint v)
        {
            uint t, tt;

            // Find MSB position in v
            var pos = (tt = v >> 16) > 0 ? (t = tt >> 8) > 0 ? 24 + logTable[t] : 16 + logTable[tt] : (t = v >> 8) > 0 ? 8 + logTable[t] : logTable[v];

            return (uint)(v & ~(1 << pos));
        }

        // Reverse integer bits and make sure LSB is set
        private static ulong ComputeRegularKey(uint key)
        {
            return ComputeDummyKey(key) | 1;
        }

        // Reverse integer bits
        private static ulong ComputeDummyKey(uint key)
        {
            return ((ulong)(((uint)reverseTable[key & 0xff] << 24) | ((uint)reverseTable[(key >> 8) & 0xff] << 16) | ((uint)reverseTable[(key >> 16) & 0xff] << 8) | ((uint)reverseTable[(key >> 24) & 0xff]))) << 1;
        }

        // Bucket storage is abstracted in a simple two-layer tree to avoid too much memory resize
        private Node GetBucket(uint index)
        {
            if (index >= this.buckets.Length)
                return null;
            return this.buckets[index];
        }

        private Node SetBucket(uint index, Node node)
        {
            try
            {
                this.slim.EnterReadLock();
                this.CheckSegment(index, true);

                Interlocked.CompareExchange(ref this.buckets[index], node, null);
                return this.buckets[index];
            }
            finally
            {
                this.slim.ExitReadLock();
            }
        }

        // When we run out of space for bucket storage, we use a lock-based array resize
        private void CheckSegment(uint segment, bool readLockTaken)
        {
            if (segment < this.buckets.Length)
                return;

            if (readLockTaken)
                this.slim.ExitReadLock();
            try
            {
                this.slim.EnterWriteLock();
                while (segment >= this.buckets.Length)
                    Array.Resize(ref this.buckets, this.buckets.Length * 2);
            }
            finally
            {
                this.slim.ExitWriteLock();
            }
            if (readLockTaken)
                this.slim.EnterReadLock();
        }

        private Node ListSearch(ulong key, TKey subKey, ref Node left, Node h)
        {
            Node leftNodeNext = null, rightNode = null;

            do
            {
                Node t = h;
                Node tNext = t.Next;
                do
                {
                    if (!tNext.Marked)
                    {
                        left = t;
                        leftNodeNext = tNext;
                    }
                    t = tNext.Marked ? tNext.Next : tNext;
                    if (t == this.tail)
                        break;

                    tNext = t.Next;
                }
                while (tNext.Marked || t.Key < key || (tNext.Key == key && !this.comparer.Equals(subKey, t.SubKey)));

                rightNode = t;

                if (leftNodeNext == rightNode)
                {
                    if (rightNode != this.tail && rightNode.Next.Marked)
                        continue;
                    else
                        return rightNode;
                }

                if (Interlocked.CompareExchange(ref left.Next, rightNode, leftNodeNext) == leftNodeNext)
                {
                    if (rightNode != this.tail && rightNode.Next.Marked)
                        continue;
                    else
                        return rightNode;
                }
            }
            while (true);
        }

        private bool ListDelete(Node startPoint, ulong key, TKey subKey, out T data)
        {
            Node rightNode = null, rightNodeNext = null, leftNode = null;
            data = default(T);
            Node markedNode = null;

            do
            {
                rightNode = this.ListSearch(key, subKey, ref leftNode, startPoint);
                if (rightNode == this.tail || rightNode.Key != key || !this.comparer.Equals(subKey, rightNode.SubKey))
                    return false;

                data = rightNode.Data;
                rightNodeNext = rightNode.Next;

                if (!rightNodeNext.Marked)
                {
                    if (markedNode == null)
                        markedNode = new Node();
                    markedNode.Init(rightNodeNext);

                    if (Interlocked.CompareExchange(ref rightNode.Next, markedNode, rightNodeNext) == rightNodeNext)
                        break;
                }
            }
            while (true);

            if (Interlocked.CompareExchange(ref leftNode.Next, rightNodeNext, rightNode) != rightNode)
                this.ListSearch(rightNode.Key, subKey, ref leftNode, startPoint);

            return true;
        }

        private bool ListInsert(Node newNode, Node startPoint, out Node current, Func<T> dataCreator)
        {
            ulong key = newNode.Key;
            Node rightNode = null, leftNode = null;

            do
            {
                rightNode = current = this.ListSearch(key, newNode.SubKey, ref leftNode, startPoint);
                if (rightNode != this.tail && rightNode.Key == key && this.comparer.Equals(newNode.SubKey, rightNode.SubKey))
                    return false;

                newNode.Next = rightNode;
                if (dataCreator != null)
                    newNode.Data = dataCreator();
                if (Interlocked.CompareExchange(ref leftNode.Next, newNode, rightNode) == rightNode)
                    return true;
            }
            while (true);
        }

        private bool ListFind(ulong key, TKey subKey, Node startPoint, out Node data)
        {
            Node rightNode = null, leftNode = null;
            data = null;

            rightNode = this.ListSearch(key, subKey, ref leftNode, startPoint);
            data = rightNode;

            return rightNode != this.tail && rightNode.Key == key && this.comparer.Equals(subKey, rightNode.SubKey);
        }

        private static readonly byte[] reverseTable = { 0, 128, 64, 192, 32, 160, 96, 224, 16, 144, 80, 208, 48, 176, 112, 240, 8, 136, 72, 200, 40, 168, 104, 232, 24, 152, 88, 216, 56, 184, 120, 248, 4, 132, 68, 196, 36, 164, 100, 228, 20, 148, 84, 212, 52, 180, 116, 244, 12, 140, 76, 204, 44, 172, 108, 236, 28, 156, 92, 220, 60, 188, 124, 252, 2, 130, 66, 194, 34, 162, 98, 226, 18, 146, 82, 210, 50, 178, 114, 242, 10, 138, 74, 202, 42, 170, 106, 234, 26, 154, 90, 218, 58, 186, 122, 250, 6, 134, 70, 198, 38, 166, 102, 230, 22, 150, 86, 214, 54, 182, 118, 246, 14, 142, 78, 206, 46, 174, 110, 238, 30, 158, 94, 222, 62, 190, 126, 254, 1, 129, 65, 193, 33, 161, 97, 225, 17, 145, 81, 209, 49, 177, 113, 241, 9, 137, 73, 201, 41, 169, 105, 233, 25, 153, 89, 217, 57, 185, 121, 249, 5, 133, 69, 197, 37, 165, 101, 229, 21, 149, 85, 213, 53, 181, 117, 245, 13, 141, 77, 205, 45, 173, 109, 237, 29, 157, 93, 221, 61, 189, 125, 253, 3, 131, 67, 195, 35, 163, 99, 227, 19, 147, 83, 211, 51, 179, 115, 243, 11, 139, 75, 203, 43, 171, 107, 235, 27, 155, 91, 219, 59, 187, 123, 251, 7, 135, 71, 199, 39, 167, 103, 231, 23, 151, 87, 215, 55, 183, 119, 247, 15, 143, 79, 207, 47, 175, 111, 239, 31, 159, 95, 223, 63, 191, 127, 255 };

        private static readonly byte[] logTable = { 0xFF, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };

        private struct SimpleRwLock
        {
            private const int RwWait = 1;

            private const int RwWrite = 2;

            private const int RwRead = 4;

            private int rwlock;

            public void EnterReadLock()
            {
                SpinWait sw = new SpinWait();
                do
                {
                    while ((this.rwlock & (RwWrite | RwWait)) > 0)
                        sw.SpinOnce();

                    if ((Interlocked.Add(ref this.rwlock, RwRead) & (RwWait | RwWait)) == 0)
                        return;

                    Interlocked.Add(ref this.rwlock, -RwRead);
                }
                while (true);
            }

            public void ExitReadLock()
            {
                Interlocked.Add(ref this.rwlock, -RwRead);
            }

            public void EnterWriteLock()
            {
                SpinWait sw = new SpinWait();
                do
                {
                    int state = this.rwlock;
                    if (state < RwWrite)
                    {
                        if (Interlocked.CompareExchange(ref this.rwlock, RwWrite, state) == state)
                            return;
                        state = this.rwlock;
                    }
                    // We register our interest in taking the Write lock (if upgradeable it's already done)
                    while ((state & RwWait) == 0 && Interlocked.CompareExchange(ref this.rwlock, state | RwWait, state) != state)
                        state = this.rwlock;
                    // Before falling to sleep
                    while (this.rwlock > RwWait)
                        sw.SpinOnce();
                }
                while (true);
            }

            public void ExitWriteLock()
            {
                Interlocked.Add(ref this.rwlock, -RwWrite);
            }
        }
    }

#if INSIDE_SYSTEM_WEB && !NET_4_0
	internal struct SpinWait
	{
		// The number of step until SpinOnce yield on multicore machine
		const           int  step = 10;
		const           int  maxTime = 200;
		static readonly bool isSingleCpu = (Environment.ProcessorCount == 1);

		int ntime;

		public void SpinOnce ()
		{
			ntime += 1;

			if (isSingleCpu) {
				// On a single-CPU system, spinning does no good
				Thread.Sleep (0);
			} else {
				if (ntime % step == 0)
					Thread.Sleep (0);
				else
					// Multi-CPU system might be hyper-threaded, let other thread run
					Thread.SpinWait (Math.Min (ntime, maxTime) << 1);
			}
		}
	}
#endif
}
