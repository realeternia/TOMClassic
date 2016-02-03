using System;
using System.Collections.Generic;
using System.Collections;

namespace NarlonLib.Math
{
    public class Calculator
    {
        private string _mathExpression;
        public Calculator()
        {

        }
        public Calculator(string MathExpression)
        {
            _mathExpression = MathExpression;
        }
        public string MathExpression
        {
            set
            {
                _mathExpression = value;
            }
            get
            {
                return _mathExpression;
            }
        }
        public Double MathExpressionValue()
        {
            List<string> ListExp = Parse(_mathExpression);
            return Calculate(ListExp);
        }
        public Double MathExpressionValue(string MathExpress)
        {
            List<string> ListExp = Parse(MathExpress);
            return Calculate(ListExp);
        }
        /// <summary>  
        /// Get Suffix expressions . eg:1+2*3 --> 123*+  
        /// </summary>  
        /// <param name="MathExpressions"></param>  
        /// <returns></returns>  
        public List<string> Parse(string MathExpressions)
        {
            int i, j = 0;
            string temp, tempSec;
            Stack myStack = new System.Collections.Stack();
            //return value  
            List<string> listResult = new List<string>();

            List<string> strListA = StringSpit(MathExpressions);
            //temp string  
            string[] strB = new string[strListA.Count];

            foreach (string strch in strListA)
            {

                temp = strch;
                //if is digit input it to strB[]  
                if (IsOperand(temp))
                {
                    strB[j++] = temp;
                }
                else
                {
                    if (temp == "(")
                        myStack.Push(temp);
                    else if (temp == ")")
                    {
                        while (!IsEmpty(myStack)) //Stack pop until ')'  
                        {
                            temp = (string)myStack.Pop();
                            if (temp == "(")
                                break;
                            else
                                strB[j++] = temp;
                        }
                    }
                    else
                    {
                        if (!IsEmpty(myStack))
                        {
                            do
                            {
                                tempSec = (string)myStack.Pop();
                                if (Priority(temp) > Priority(tempSec))
                                {
                                    myStack.Push(tempSec);
                                    myStack.Push(temp);
                                    break;
                                }
                                else
                                {
                                    strB[j++] = tempSec;
                                    if (IsEmpty(myStack))
                                    {
                                        myStack.Push(temp);
                                        break;
                                    }
                                }
                            } while (!IsEmpty(myStack));
                        }
                        else
                        {
                            myStack.Push(temp);
                        }
                    }
                }
            }
            while (!IsEmpty(myStack))
                strB[j++] = (string)myStack.Pop();
            for (i = 0; i < strB.Length; i++)
            {
                if (!string.IsNullOrEmpty(strB[i]))
                {
                    listResult.Add(strB[i]);
                }
            }
            return listResult;
        }


        /// <summary>  
        /// Check if is is a digit  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns>if str is digit, return true else return false</returns>  
        private bool IsOperand(string str)
        {
            string[] operators = { "+", "-", "*", "/", "(", ")" };
            for (int i = 0; i < operators.Length; i++)
                if (str == operators[i])
                    return false;
            return true;
        }
        /// <summary>  
        /// Check Stack empty  
        /// </summary>  
        /// <param name="st"></param>  
        /// <returns></returns>  
        private bool IsEmpty(Stack st)
        {
            return st.Count == 0 ? true : false;
        }
        /// <summary>  
        /// Spit string with + - * / ( )  
        /// </summary>  
        /// <param name="StrSource"></param>  
        /// <returns></returns>  
        public List<string> StringSpit(string StrSource)
        {
            char[] strarray = StrSource.ToCharArray();
            List<string> ListStr = new List<string>();
            int start = 0;
            int LastOpIndex = 0;
            string temp;
            for (int i = 0; i < strarray.Length; i++)
            {
                switch (strarray[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '(':
                    case ')':
                        temp = StrSource.Substring(start, i - start);
                        if (!string.IsNullOrEmpty(temp))
                        {
                            ListStr.Add(temp);
                        }
                        temp = StrSource.Substring(i, 1);
                        if (!string.IsNullOrEmpty(temp))
                        {
                            ListStr.Add(temp);
                        }
                        start = i + 1;
                        LastOpIndex = i;
                        break;
                }
                if (i == strarray.Length - 1)
                {
                    ListStr.Add(StrSource.Substring(LastOpIndex + 1, strarray.Length - LastOpIndex - 1));
                }

            }
            return ListStr;
        }
        /// <summary>  
        ///  Get operator priority  
        /// </summary>  
        /// <param name="Opr"></param>  
        /// <returns></returns>  
        private int Priority(string Opr)
        {
            int priority;
            switch (Opr)
            {
                case "+":
                    priority = 1;
                    break;
                case "-":
                    priority = 1;
                    break;
                case "*":
                    priority = 2;
                    break;
                case "/":
                    priority = 2;
                    break;
                default:
                    priority = 0;
                    break;
            }
            return priority;
        }

        /// <summary>  
        /// Calculate suffix express value eg: 123*+ ---> 7  
        /// </summary>  
        /// <param name="ListstrA"></param>  
        /// <returns></returns>  
        public double Calculate(List<string> ListstrA)
        {
            double numFir, numSec, ret;
            string temp;

            Stack<double> myStack = new Stack<double>();
            foreach (string str in ListstrA)
            {
                temp = str;
                if (IsOperand(temp))//If data, push to stack  
                {
                    myStack.Push(double.Parse(temp));
                }
                else //if operate , caculate   
                {
                    numFir = myStack.Pop();
                    numSec = myStack.Pop();
                    ret = GetValue(temp, numFir, numSec);
                    myStack.Push(ret);
                }
            }
            return myStack.Pop();
        }
        /// <summary>  
        /// Get Operate value  
        /// </summary>  
        /// <param name="op"></param>  
        /// <param name="numFir"></param>  
        /// <param name="numSec"></param>  
        /// <returns></returns>  
        private double GetValue(string op, double numFir, double numSec)
        {
            switch (op)
            {
                case "+":
                    return numSec + numFir;
                case "-":
                    return numSec - numFir;
                case "*":
                    return numSec * numFir;
                case "/":
                    return numSec / numFir;
                default:
                    return 0;
            }
        }

    }
}