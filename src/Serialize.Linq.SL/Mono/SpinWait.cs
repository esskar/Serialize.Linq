// SpinWait.cs
//
// Copyright (c) 2008 Jérémie "Garuma" Laval
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
using System.Threading;

namespace System.Collections.Concurrent
{
    internal struct SpinWait
    {
        // The number of step until SpinOnce yield on multicore machine
        const int step = 5;
        const int maxTime = 50;
        static readonly bool isSingleCpu = (Environment.ProcessorCount == 1);

        int ntime;

        public void SpinOnce()
        {
            if (isSingleCpu)
            {
                // On a single-CPU system, spinning does no good
                Thread.Sleep(1);
            }
            else
            {
                if ((this.ntime = this.ntime == maxTime ? maxTime : this.ntime + 1) % step == 0)
                    Thread.Sleep(1);
                else
                    // Multi-CPU system might be hyper-threaded, let other thread run
                    Thread.SpinWait(this.ntime << 1);
            }
        }

        public static void SpinUntil(Func<bool> predicate)
        {
            SpinWait sw = new SpinWait();
            while (!predicate())
                sw.SpinOnce();
        }

        public void Reset()
        {
            this.ntime = 0;
        }

        public bool NextSpinWillYield
        {
            get
            {
                return isSingleCpu ? true : this.ntime % step == 0;
            }
        }

        public int Count
        {
            get
            {
                return this.ntime;
            }
        }
    }
}
