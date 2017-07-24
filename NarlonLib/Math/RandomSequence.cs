using System;
using System.Collections.Generic;
using NarlonLib.Tools;

namespace NarlonLib.Math
{
    public class RandomSequence
    {
        private List<int> numberList;
        private int count;
        private int lastNumber;
        private Random rd;

        public RandomSequence(int count)
        {
            this.count = count;
        }
        public RandomSequence(int count, Random r)
        {
            this.count = count;
            rd = r;
        }

        public int NextNumber()
        {
            if (numberList == null)
            {
                int[] datas = new int[count];
                for (int i = 0; i < count; i++)
                    datas[i] = i;

                numberList= new List<int>(datas);
                if (rd == null)
                {
                    ArraysUtils.RandomShuffle(numberList);
                }
                else
                {
                    ArraysUtils.RandomShuffle(numberList, rd);
                }
                
                lastNumber = 0;
            }
            else
            {
                lastNumber++;
            }

            return numberList[lastNumber];
        }
    }
}
