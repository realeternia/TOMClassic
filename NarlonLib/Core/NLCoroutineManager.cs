using System;
using System.Collections;
using System.Collections.Generic;

namespace NarlonLib.Core
{
    public class NLCoroutineManager : IDisposable
    {
        public delegate void Coroutine();
        private readonly Dictionary<long, NLTimeCoroutine> coroutineDict = new Dictionary<long, NLTimeCoroutine>();
        private bool disposed;
        public NLTimerManager Timer { get; private set; }

        public NLCoroutineManager(NLTimerManager timerManager)
        {
            Timer = timerManager;
        }

        public NLTimeCoroutine StartCoroutine(IEnumerator routine)
        {
            if (disposed)
            {
                return null;
            }

            NLTimeCoroutine coroutine = new NLTimeCoroutine(this, routine);
            if (coroutine.Start() && coroutine.Id > 0)
            {
                coroutineDict.Add(coroutine.Id, coroutine);
            }
            return coroutine;
        }

        public void Dispose()
        {
            foreach (var coroutine in coroutineDict.Values)
            {
                coroutine.Stop(false);
            }
            coroutineDict.Clear();
            disposed = true;
        }

        public void RemoveCoroutine(NLTimeCoroutine coroutine)
        {
            if (coroutine.Id > 0)
            {
                coroutineDict.Remove(coroutine.Id);
            }
        }

    }
}