using System;
using System.Collections.Generic;
using System.Collections;

namespace NarlonLib.Core
{
    public class NLYieldInstruction
    {
    }

    public class NLWaitForSeconds : NLYieldInstruction
    {
        internal float Seconds;

        public NLWaitForSeconds(float seconds)
        {
            Seconds = seconds;
        }
    }

    public class NLCoroutine : NLYieldInstruction
    {
        private static NLTimerManager timer = new NLTimerManager();
        private static LinkedList<NLCoroutine>  coroutines = new LinkedList<NLCoroutine>();

        protected IEnumerator m_enumerator;

        public static void StartCoroutine(IEnumerator routine)
        {
            NLCoroutine c = new NLCoroutine(routine);
            coroutines.AddLast(c);            
        }

        public static void DoTimer()
        {
            timer.DoTimer();
        }

        public NLCoroutine(IEnumerator e)
        {
            e.MoveNext();
            m_enumerator = e;
            DispatchCoroutine();
        }

        private void DispatchCoroutine()
        {
            object obj = m_enumerator.Current;
            if (obj is NLWaitForSeconds)
            {
                timer.AddTimer("Coroutine WaitForSeconds", TimeSpan.FromSeconds((obj as NLWaitForSeconds).Seconds), WaitForSeconds, 1, this);
            }
            else
            {
                throw new NotSupportedException("this yeild return type is not supported.");
            }
        }

        private static void WaitForSeconds(INLTimer t, object userData)
        {
            (userData as NLCoroutine).NextStep();
        }

        public virtual bool NextStep()
        {
            if (m_enumerator.MoveNext())
            {
                DispatchCoroutine();
                return true;
            }
            return false;
        }
    }
}
