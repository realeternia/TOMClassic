using System.Collections.Generic;

namespace NarlonLib.Math
{
    public class RandomSequence
    {
        private List<int> numberList;
        private int count;
        private int lastNumber;

        public RandomSequence(int count)
        {
            this.count = count;
        }

        public int NextNumber()
        {
            if (numberList == null)
            {
                int[] datas = new int[count];
                for (int i = 0; i < count; i++)
                    datas[i] = i;

                numberList = RandomShuffle.Process(datas);
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
