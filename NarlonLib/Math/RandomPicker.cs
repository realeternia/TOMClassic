using System;
using System.Collections.Generic;
using NarlonLib.Core;

namespace NarlonLib.Math
{
    public class NLRandomPicker<T>
    {
        private readonly List<NLPair<T, float>> datas = new List<NLPair<T, float>>();
        private float totalRate;
        private int seed; //随机数种子
        private Random random;

        public NLRandomPicker()
        {
            seed = DateTime.Now.Millisecond;
            random = new Random(seed);
        }

        internal void AddData(T key, float rate)
        {
            datas.Add(new NLPair<T, float>(key, rate));
            totalRate += rate;
        }

        internal void RemoveData(NLPair<T, float> data)
        {
            if (datas.Contains(data))
            {
                datas.Remove(data);
                totalRate -= data.Value2;
            }
        }

        internal T[] RandomPickN(uint count)
        {
            if (datas.Count < count)
            {
                return new T[0];
            }

            T[] resultArray = new T[count];
            for (int i = 0; i < count; i++)
            {
                resultArray[i] = RandomPick();
            }
            return resultArray;
        }

        internal T RandomPick()
        {
            float roll = (float)(random.NextDouble() * (totalRate));
            float dataAdd = 0;
            foreach (var pair in datas)
            {
                dataAdd += pair.Value2;
                if (roll < dataAdd)
                {
                    RemoveData(pair);
                    return pair.Value1;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 从dataArray数组中按照概率rateArray，随机选出count个物件
        /// </summary>
        /// <param name="dataArray">样本数组</param>
        /// <param name="rateArray">概率数组</param>
        /// <param name="count">选取数</param>
        /// <returns>返回选取的物件</returns>
        public static T[] RandomPickN(T[] dataArray, float[] rateArray, uint count)
        {
            if (dataArray.Length != rateArray.Length || count == 0)
            {
                return new T[0];
            }

            if (dataArray.Length <= count)//全部掉出
            {
                return dataArray;
            }
            //todo 考虑下n选m，时m>n/2，做减法
            NLRandomPicker<T> picker = new NLRandomPicker<T>();
            for (int i = 0; i < dataArray.Length; i++)
            {
                picker.AddData(dataArray[i], rateArray[i]);
            }
            return picker.RandomPickN(count);
        }

        /// <summary>
        /// 从dataArray数组中以等概率随机选出count个物件
        /// </summary>
        /// <param name="dataArray">样本数组</param>        
        /// <param name="count">选取数</param>
        /// <returns>返回选取的物件</returns>
        public static T[] RandomPickN(T[] dataArray, uint count)//等概率
        {
            if (count == 0)
            {
                return new T[0];
            }

            if (dataArray.Length <= count)//全部掉出
            {
                return dataArray;
            }

            NLRandomPicker<T> picker = new NLRandomPicker<T>();
            for (int i = 0; i < dataArray.Length; i++)
            {
                picker.AddData(dataArray[i], 1);
            }
            return picker.RandomPickN(count);
        }

        public static List<T> RandomPickN(List<T> dataArray, uint count)//等概率
        {
            if (count == 0)
            {
                return new List<T>();
            }

            if (dataArray.Count <= count)//全部掉出
            {
                return dataArray;
            }

            NLRandomPicker<T> picker = new NLRandomPicker<T>();
            for (int i = 0; i < dataArray.Count; i++)
            {
                picker.AddData(dataArray[i], 1);
            }
            return new List<T>(picker.RandomPickN(count));
        }

        public static List<T> RandomPickN(List<T> dataArray, uint count, int seed) //等概率
        {
            if (count == 0)
            {
                return new List<T>();
            }

            if (dataArray.Count <= count)//全部掉出
            {
                return dataArray;
            }

            NLRandomPicker<T> picker = new NLRandomPicker<T>();
            picker.seed = seed;
            picker.random = new Random(seed);
            for (int i = 0; i < dataArray.Count; i++)
            {
                picker.AddData(dataArray[i], 1);
            }
            return new List<T>(picker.RandomPickN(count));
        }
    }
}
