using System;
using System.Collections;

namespace NarlonLib.Core
{
    public class NLWaitForSeconds
    {
        public float Seconds;

        public NLWaitForSeconds(float seconds)
        {
            Seconds = seconds;
        }
    }
    /// <summary>
    /// 一个脱离mono的Coroutine简化类
    /// </summary>
    public class NLTimeCoroutine : IDisposable
    {
        public long Id { get; private set; }//是第一次的timerid
        public long TimerId { get; private set; }
        private bool disposed;
        private IEnumerator enumerator;

        private NLCoroutineManager _dgCoroutineManager;

        public NLTimeCoroutine(NLCoroutineManager coroutineManager, IEnumerator e)
        {
            enumerator = e;
            _dgCoroutineManager = coroutineManager;
        }

        public bool Start()
        {
            if (MoveNext())
            {
                DispatchCoroutine();
                Id = TimerId;
                return true;
            }
            return false;//如果只有一步，就不用加入coroutineManager中了
        }

        private bool MoveNext()
        {
            bool r = false;
            try
            {
                r = enumerator.MoveNext();
                if (disposed)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Stop();
                r = false;
                throw e;
            }
            return r;
        }

        private void DispatchCoroutine()
        {
            object obj = enumerator.Current;
            if (obj == null)
            {
                _dgCoroutineManager.RemoveCoroutine(this);
                return;
            }

            if (obj is NLWaitForSeconds)
            {
                INLTimer time = _dgCoroutineManager.Timer.AddTimer((obj as NLWaitForSeconds).Seconds, WaitForSeconds, 1, this);
                TimerId = time.Id;
            }
            else
            {
                throw new NotSupportedException("this yield return type is not supported.");
            }
        }

        private static void WaitForSeconds(NLTimerManager manager, INLTimer t, object[] userData)
        {
            (userData[0] as NLTimeCoroutine).NextStep();
        }

        public void Stop(bool forceDelCo = true)
        {
            if (disposed)
            {
                return;
            }

            if (forceDelCo)
                _dgCoroutineManager.RemoveCoroutine(this);

            Dispose();
        }

        public bool NextStep()
        {
            if (disposed)
            {
                return false;
            }
            if (MoveNext())
            {
                DispatchCoroutine();
                return true;
            }
            else
            {
                if (_dgCoroutineManager != null)
                {
                    _dgCoroutineManager.RemoveCoroutine(this);
                }
                Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            if (TimerId > 0)
                _dgCoroutineManager.Timer.RemoveTimer(TimerId);
            _dgCoroutineManager = null;
            enumerator = null;
        }
    }
}
