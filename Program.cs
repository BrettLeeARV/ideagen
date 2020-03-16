using System;

namespace ideagen
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] exps = {
                                "1 + 1",
                                "2 * 2",
                                "1 + 2 + 3",
                                "6 / 2",
                                "11 + 23",
                                "11.1 + 23",
                                "1 + 1 * 3",
                                "( 11.5 + 15.4 ) + 10.1",
                                "23 - ( 29.3 - 12.5 )",
                                "10 - ( 2 + 3 * ( 7 - 5 ) )"
                            };

            foreach (string exp in exps)
            {
                Console.WriteLine($"Calculate('{exp}') = " + Calculate(exp));                
            }
        }

        /// <summary>
        /// Computes a math expression
        /// </summary>
        /// <param name="sum">Math expression (must be seperated by spaces)</param>
        /// <returns>Computed result. -1 if error occured</returns>
        public static double Calculate(string sum)
        {
            char separator = ' ';
            string[] exp = sum.Trim().Split(separator);
            double operand = 0;
            char? op = null;
            double accumulator = -1;
            string nested = string.Empty;
            int precedenceOperatorIdx = -1;
            int multiplyOpIdx = -1;
            int divOpIdx = -1;

            try
            {
                //Handle nested expressions first
                while (sum.Contains('(') || sum.Contains(')'))
                {
                    nested = sum.Substring(sum.LastIndexOf('('), sum.IndexOf(')') - sum.LastIndexOf('(') + 1);
                    accumulator = Calculate(nested.Replace("( ", "").Replace(" )", ""));

                    sum = sum.Replace(nested, accumulator.ToString());
                    exp = sum.Trim().Split(' ');
                }

                //Handle operator precedence for multiply (*) and division (/)
                while (exp.Length > 3 && (Array.Exists(exp, e => e == "*") || Array.Exists(exp, e => e == "/")))
                {
                    multiplyOpIdx = Array.IndexOf(exp, "*");
                    divOpIdx = Array.IndexOf(exp, "/");

                    precedenceOperatorIdx = (multiplyOpIdx < divOpIdx && multiplyOpIdx > -1 || divOpIdx == -1) ?
                                            multiplyOpIdx :
                                            divOpIdx;

                    nested = exp[precedenceOperatorIdx - 1] + " " + exp[precedenceOperatorIdx] + " " + exp[precedenceOperatorIdx + 1];
                    accumulator = Calculate(nested);

                    sum = sum.Replace(nested, accumulator.ToString());
                    exp = sum.Trim().Split(' ');
                }

                //Eexpression main handler
                foreach (string element in exp)
                {
                    //Extract operands and operators
                    if (Double.TryParse(element, out operand))
                    {
                        //Extract Operand
                        //If there's an operator, compute operand with accumulator
                        if (op.HasValue)
                        {
                            switch (op.Value)
                            {
                                case '+': accumulator += operand; break;
                                case '-': accumulator -= operand; break;
                                case '*': accumulator *= operand; break;
                                case '/': accumulator /= operand; break;
                            }
                        }
                        else
                        {
                            //First run, set accumulator as operand
                            accumulator = operand;
                        }
                    }
                    else
                    {
                        //Extract operator
                        op = Convert.ToChar(element);
                    }
                }
            }
            catch (Exception ex)
            {
                //Error occured, return -1
                accumulator = -1;
            }

            return accumulator;
        }
    }
}