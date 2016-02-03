using System;

namespace NarlonLib.Core
{
    public interface INLTimer
    {
        void Destroy();
        TimeSpan TimeOffset { get; }
    }

    public class NLTimerManager
    {
        class TimerItem : INLTimer, IComparable<TimerItem>
        {            
            public DateTime Time;
            public OnTimer Delegate;
            public object UserData;
            public int Loop;
            private TimeSpan timeOffset;
            public TimeSpan TimeOffset
            {
                get { return timeOffset; }
                set { timeOffset = value; }
            }
            public string Name;
            public bool NeedRemove;

            public TimerItem()
            {                
                NeedRemove = false;
            }

            public void Destroy()
            {
                NeedRemove = true;
            }

            public int CompareTo(TimerItem other)
            {
                return -Time.CompareTo(other.Time);
            }
        }

        public delegate void OnTimer(INLTimer timer, object userData);
        private NLUXPriorityQueue<TimerItem> m_timerList = new NLUXPriorityQueue<TimerItem>();

        public void Destroy()
        {
            m_timerList = null;
        }

        public INLTimer AddTimer(string name, DateTime time, OnTimer callback, object userdata)
        {
            TimerItem timer = new TimerItem();
            timer.Name = name;
            timer.Time = time;
            timer.Delegate = callback;
            timer.Loop = 1;
            timer.UserData = userdata;
            timer.TimeOffset = new TimeSpan(0);
            m_timerList.Push(timer);
            return timer;
        }

        public INLTimer AddTimer(string name, TimeSpan timeOffset, OnTimer callback, int loop, object userdata)
        {
            TimerItem timer = new TimerItem();
            timer.Name = name;
            timer.Time = DateTime.Now.Add(timeOffset);
            timer.Delegate = callback;
            timer.Loop = loop;
            timer.UserData = userdata;
            timer.TimeOffset = timeOffset;
            m_timerList.Push(timer);
            return timer;
        }

        public void RemoveTimer(string name)
        {
            foreach (TimerItem timer in m_timerList)
            {
                if (timer.Name == name)
                {
                    timer.NeedRemove = true;
                }
            }
        }

        public int DoTimer()
        {
            DateTime timeNow = DateTime.Now;
            int count = 0;
            while (true)
            {
                if (m_timerList == null || m_timerList.Count == 0)
                    return count;

                TimerItem timer = m_timerList.Top();
                if (timer.NeedRemove)
                {
                    m_timerList.Pop();
                    continue;
                }

                if (timeNow >= timer.Time)
                {
                    if (timer.Loop > 0)
                        timer.Loop--;
                    if (timer.Loop == 0 || timer.TimeOffset.Ticks == 0)
                    {
                        m_timerList.Pop();
                    }
                    else
                    {
                        m_timerList.Pop();
                        timer.Time = timer.Time.Add(timer.TimeOffset);
                        m_timerList.Push(timer);
                    }
                    count++;

                    timer.Delegate(timer, timer.UserData);
                }
                else
                {
                    return count;
                }
            }
        }
    }
}
