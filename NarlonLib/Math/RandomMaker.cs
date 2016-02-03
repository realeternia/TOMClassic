using System.Collections.Generic;

namespace NarlonLib.Math
{
    public class RandomMaker
    {
        private List<int> datas = new List<int>();

        public void Add(int data, int rate)
        {
            for (int i = 0; i < datas.Count; i+=2)
            {
                if (datas[i] == data)
                {
                    datas[i + 1] = System.Math.Max(rate + datas[i + 1], 0);
                    return;
                }
            }
            datas.Add(data);
            datas.Add(rate);
        }

        public int[] Process(int rcount)
        {
            int[] results = new int[rcount];
            for (int i = 0; i < rcount; i++)
            {
                results[i] = 0;
            }

            if (datas.Count/2 <= rcount) //如果数据过少
            {
                for (int i = 0; i < datas.Count/2; i++)
                {
                    results[i] = datas[i*2];
                }
            }
            else
            {
                int total = 0;
                for (int i = 1; i < datas.Count; i += 2)
                {
                    total += datas[i];
                }

                for (int i = 0; i < rcount; ++i)
                {
                    while (true)
                    {
                        int var = MathTool.GetRandom(total);
                        int tempsum = 0;
                        for (int j = 0; j < datas.Count; j += 2)
                        {
                            tempsum += datas[j + 1];
                            if (var < tempsum)
                            {
                                var = datas[j];
                                break;
                            }
                        }

                        bool flag = false;
                        for (int j = 0; j < i; j++)
                        {
                            if (var == results[j])
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            results[i] = var;
                            break;
                        }
                    }
                }
            }
            return results;
        }
    }
}
