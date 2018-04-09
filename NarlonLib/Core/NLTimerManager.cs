using System;
using System.Collections.Generic;

namespace NarlonLib.Core
{
    public interface INLTimer
    {
        double TimeOffset { get; }
        string Name { get; set; }
        long Id { get; }
    }

    public class NLTimerManager
    {
        /// <summary>
        /// 进入dict的数据结构，hold住了delegate
        /// </summary>
        protected class TimerItem : INLTimer
        {
            public long Id { get; private set; }
            public OnTimer Delegate;
            public object[] UserData;
            public int Loop;

            public double TimeOffset { get; set; }
            public string Name { get; set; }

            public TimerItem(long id)
            {
                Id = id;
            }
        }

        /// <summary>
        /// 进入优先队列排队的数据结构，仅仅包含timer id以及时间，时间是为了优先队列排序
        /// </summary>
        class TimerQueueItem : IComparable<TimerQueueItem>
        {
            public long Id { get; private set; }
            public DateTime Time;

            public TimerQueueItem(long id, DateTime time)
            {
                Id = id;
                Time = time;
            }

            public int CompareTo(TimerQueueItem other)
            {
                return -Time.CompareTo(other.Time);
            }
        }

        public delegate void OnTimer(NLTimerManager mgr, INLTimer timer, object[] userData);
        private NLPriorityQueue<TimerQueueItem> timerQueue = new NLPriorityQueue<TimerQueueItem>();
        private Dictionary<long, TimerItem> timerDict = new Dictionary<long, TimerItem>();

        private static long currentId;

        public void Destroy()
        {
            timerQueue = null;
            timerDict = null;
        }

        private long GenId()
        {
            //多线程需要用 Interlocked.Increment(ref currentId);
            return ++currentId;
        }

        public INLTimer AddTimer(double seconds, OnTimer callback, int loop, params object[] userdata)
        {
            TimerItem timer = new TimerItem(GenId())
            {
                Delegate = callback,
                Loop = loop,
                UserData = userdata,
                TimeOffset = seconds
            };
            timerDict.Add(timer.Id, timer);
            timerQueue.Push(new TimerQueueItem(timer.Id, DateTime.Now.AddSeconds(seconds)));
            return timer;
        }

        public void RemoveTimer(long id)
        {
            timerDict.Remove(id); //todo timerQueue的引用就必须等timer超时，才能释放
        }

        public int DoTimer()
        {
            DateTime timeNow = DateTime.Now;
            int count = 0;
            while (true)
            {
                if (timerQueue == null || timerQueue.Count == 0)
                    return count;

                var queueTimer = timerQueue.Top();
                TimerItem timer;
                if (!timerDict.TryGetValue(queueTimer.Id, out timer))
                {
                    //Console.WriteLine("Remove Timer {0}", timer.Name);
                    timerQueue.Pop();
                    continue;
                }

                if (timeNow >= queueTimer.Time)
                {
                    if (timer.Loop > 0)
                        timer.Loop--;
                    if (timer.Loop == 0 || timer.TimeOffset == 0)
                    {
                        timerQueue.Pop();
                        timerDict.Remove(timer.Id);
                    }
                    else
                    {
                        timerQueue.Pop();
                        queueTimer.Time = queueTimer.Time.AddSeconds(timer.TimeOffset);
                        timerQueue.Push(queueTimer);
                    }
                    count++;

                    TimerOut(timer);
                }
                else
                {
                    return count;
                }
            }
        }

        protected virtual void TimerOut(TimerItem item)
        {
            //try
            {
                item.Delegate(this, item, item.UserData);
            }
            //catch (Exception ex)
            //{
            //    timerList.Pop();
            //    Console.WriteLine("Timer err: {0} {1}", timer.Name, ex.Message);
            //    throw ex;
            //}
        }
    }
}