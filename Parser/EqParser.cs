using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressionParser.Parser
{
    public class EqParser
    {

        public static List<Token> ConvertToPostfixToken(string expression)
        {
            int prioriy;
            List<Token> tokens = Token.StringToTokens(expression);
            List<Token> converted = new List<Token>(tokens.Count);

            Stack<Token> stack = new Stack<Token>();

            for (int i = 0; i < tokens.Count; i++)
            {
                Token current = tokens[i];
                if (current.TokenType == Token.Type.OPERATOR &&
                    (current.CValue == '+' ||
                    current.CValue == '-' ||
                    current.CValue == '*' ||
                    current.CValue == '/'))
                {
                    if (stack.Count <= 0)
                        stack.Push(current);
                    else
                    {
                        if (stack.Peek().CValue == '*' ||
                            stack.Peek().CValue == '/')
                            prioriy = 1;
                        else
                            prioriy = 0;
                        if (prioriy == 1)
                        {
                            if (current.CValue == '+' || current.CValue == '-')
                            {
                                converted.Add(stack.Pop());
                                i--;
                            }
                            else
                            {
                                converted.Add(stack.Pop());
                                i--;
                            }
                        }
                        else
                        {
                            if (current.CValue == '+' || current.CValue == '-')
                            {
                                converted.Add(stack.Pop());
                                stack.Push(current);
                            }
                            else
                                stack.Push(current);
                        }
                    }
                }
                else
                {
                    converted.Add(current);
                }
            }
            int len = stack.Count;
            for (int j = 0; j < len; j++)
                converted.Add(stack.Pop());
            return converted;
        }
        public static string ConvertToPostfix(string expression)
        {
            int prioriy;
            string converted = "";
            Stack<char> stack = new Stack<char>();

            for (int i = 0; i < expression.Length; i++)
            {
                char current = expression[i];
                if (current == '+' ||
                    current == '-' ||
                    current == '*' ||
                    current == '/')
                {
                    if (stack.Count <= 0)
                        stack.Push(current);
                    else
                    {
                        if (stack.Peek() == '*' ||
                            stack.Peek() == '/')
                            prioriy = 1;
                        else
                            prioriy = 0;
                        if (prioriy == 1)
                        {
                            if (current == '+' || current == '-')
                            {
                                converted += stack.Pop();
                                i--;
                            }
                            else
                            {
                                converted += stack.Pop();
                                i--;
                            }
                        }
                        else
                        {
                            if (current == '+' || current == '-')
                            {
                                converted += stack.Pop();
                                stack.Push(current);

                            }
                            else
                                stack.Push(current);
                        }
                    }
                }
                else
                {
                    converted += current;
                }
            }
            int len = stack.Count;
            for (int j = 0; j < len; j++)
                converted += stack.Pop();
            return converted;
        }

        public static double EvaluateExpr(string expr)
        {
            string aux = expr + ')';
            char current;
            Stack<double> stack = new Stack<double>();
            double val = 0;
            double x, y;

            for (int i = 0; aux[i] != ')'; i++)
            {
                current = aux[i];
                if (Char.IsDigit(current))
                {
                    stack.Push(current - '0');

                }
                else if (current == '+' ||
                    current == '-' ||
                    current == '*' ||
                    current == '/')
                {
                    x = stack.Pop();
                    y = stack.Pop();

                    switch (current) /* ch is an operator */
                    {
                        case '*':
                            val = y * x;
                            break;

                        case '/':
                            val = y / x;
                            break;

                        case '+':
                            val = y + x;
                            break;

                        case '-':
                            val = y - x;
                            break;
                    }

                    /* push the value obtained above onto the stack */
                    stack.Push(val);
                }

            }
            return val;
        }

        public static double EvaluateExprTokens(List<Token> expr)
        {
            //string aux = expr + ')';
            if (expr.Count == 1 && expr[0].TokenType == Token.Type.NUMBER)
            {
                return expr[0].Value;
            }
            Token current;
            Stack<Token> stack = new Stack<Token>();
            double val = 0;
            Token x, y;

            for (int i = 0; i < expr.Count; i++)
            {
                current = expr[i];
                if (current.TokenType == Token.Type.NUMBER)
                {
                    stack.Push(current);

                }
                else if (current.TokenType == Token.Type.OPERATOR &&
                    (current.CValue == '+' ||
                    current.CValue == '-' ||
                    current.CValue == '*' ||
                    current.CValue == '/'))
                {
                    x = stack.Pop();
                    y = stack.Pop();

                    switch (current.CValue) /* ch is an operator */
                    {
                        case '*':
                            val = y.Value * x.Value;
                            break;

                        case '/':
                            val = y.Value / x.Value;
                            break;

                        case '+':
                            val = y.Value + x.Value;
                            break;

                        case '-':
                            val = y.Value - x.Value;
                            break;
                    }

                    /* push the value obtained above onto the stack */
                    stack.Push(new Token(val));
                }

            }
            return val;
        }


        public static void ReduceX(List<Token> tokens)
        {
            int token1 = tokens.FindIndex(t => t.TokenType == Token.Type.X);
            int token2 = tokens.FindLastIndex(t => t.TokenType == Token.Type.X);
            if (token1 == token2) return;
            double otherTokenValue = tokens[token2 - 1].CValue == '*' ? tokens[token2 - 2].Value : 1;
            if (token1 == 0 || tokens[token1 - 1].CValue != '*')
            {
                tokens.Insert(token1, new Token('*'));
                token2 += 1;
                tokens.Insert(token1, new Token(1 + otherTokenValue));
                token2++;
            }
            else
            {

                tokens[token1 - 2].Value += otherTokenValue;
            }
            tokens.RemoveAt(token2);
            tokens.RemoveAt(token2 - 1);
            tokens.RemoveAt(token2 - 2);
            RemoveUselessOperators(tokens);
        }

        public static void ReduceNumbers(List<Token> tokens, char op1 = '*', char op2 = '/')
        {
            bool stop = false;
            if (op1 == '+' || op2 == '-') stop = true;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == Token.Type.OPERATOR && (tokens[i].CValue == op1 || tokens[i].CValue == op2))
                {
                    if (tokens[i - 1].TokenType == Token.Type.NUMBER && tokens[i + 1].TokenType == Token.Type.NUMBER)
                    {
                        List<Token> expr = new List<Token>(new Token[] { tokens[i - 1], tokens[i + 1], tokens[i] });
                        tokens[i - 1].Value = EvaluateExprTokens(expr);
                        tokens.RemoveAt(i + 1);
                        tokens.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (stop)
            {
                return;
            }
            ReduceNumbers(tokens, '+', '-');
        }

        public static void ReduceXDivision(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == Token.Type.OPERATOR && tokens[i].CValue == '/')
                {
                    if (tokens[i - 1].TokenType == Token.Type.X)
                    {
                        tokens[i - 1].TokenType = Token.Type.NUMBER;
                        tokens[i - 1].Value = 1 / tokens[i + 1].Value;
                        tokens[i + 1].TokenType = Token.Type.X;
                        tokens[i + 1].CValue = 'x';
                        tokens[i + 1].Value = 0;
                        tokens[i].CValue = '*';
                    }
                }
            }

        }

        public static void RemoveUselessOperators(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == Token.Type.OPERATOR)
                {
                    if (i == 0 || i == tokens.Count - 1 || tokens[i - 1].TokenType == Token.Type.OPERATOR || tokens[i - 1].TokenType == Token.Type.EQUAL ||
                     tokens[i + 1].TokenType == Token.Type.OPERATOR || tokens[i + 1].TokenType == Token.Type.EQUAL)
                    {
                        tokens.RemoveAt(i);
                    }
                }
            }
        }

        public static void MinusToPlus(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == Token.Type.OPERATOR && tokens[i].CValue == '-')
                {
                    if (tokens[i + 1].TokenType == Token.Type.NUMBER)
                    {
                        tokens[i].CValue = '+';
                        tokens[i + 1].Value = -tokens[i + 1].Value;
                    }
                }
            }
        }

        public static void PrintTokens(List<Token> tokens)
        {
            string str = "";
            foreach (var token in tokens)
            {
                str += token;
            }
            Console.WriteLine(str);
        }
        public static double EvaluateEquation(string eq)
        {
            List<Token> tokenized = Token.StringToTokens(eq);
            Token equal = tokenized.Find(t => t.TokenType == Token.Type.EQUAL);

            ReduceXDivision(tokenized);


            int eqIndex = tokenized.IndexOf(equal);

            Token[] left = new Token[eqIndex];
            Token[] right = new Token[tokenized.Count - eqIndex - 1];
            Array.Copy(tokenized.ToArray(), left, eqIndex);
            Array.Copy(tokenized.ToArray(), eqIndex + 1, right, 0, tokenized.Count - eqIndex - 1);

            List<Token> leftList = new List<Token>(left);
            List<Token> rightList = new List<Token>(right);

            Console.WriteLine("Left: ");
            PrintTokens(leftList);
            Console.WriteLine("Right: ");
            PrintTokens(rightList);

            while (leftList.FindAll(t => t.TokenType == Token.Type.X).Count > 1 || rightList.FindAll(t => t.TokenType == Token.Type.X).Count > 1)
            {
                ReduceX(leftList);
                ReduceX(rightList);
            }
            MinusToPlus(leftList);
            MinusToPlus(rightList);
            ReduceNumbers(rightList);
            ReduceNumbers(leftList);
            Console.WriteLine("Left: ");
            PrintTokens(leftList);
            Console.WriteLine("Right: ");
            PrintTokens(rightList);

            int indexX = leftList.FindIndex(t => t.TokenType == Token.Type.X);
            double a = indexX > 0 && leftList[indexX - 1].CValue == '*' ? leftList[indexX - 2].Value : indexX < 0 ? 0 : 1;
            indexX = rightList.FindIndex(t => t.TokenType == Token.Type.X);
            double c = indexX > 0 && rightList[indexX - 1].CValue == '*' ? rightList[indexX - 2].Value : indexX < 0 ? 0 : 1;

            indexX = leftList.FindIndex(t => leftList.IndexOf(t) < leftList.Count - 1 && t.TokenType == Token.Type.NUMBER && leftList[leftList.IndexOf(t) + 1].CValue == '+');
            if (indexX < 0)
            {
                if (leftList.Count == 1 && leftList[0].TokenType == Token.Type.NUMBER)
                {
                    indexX = 0;
                }
                else
                {
                    indexX = leftList.FindIndex(t => leftList.IndexOf(t) > 0 && t.TokenType == Token.Type.NUMBER && leftList[leftList.IndexOf(t) - 1].CValue == '+');
                }
            }

            double b = indexX < 0 ? 0 : leftList[indexX].Value;

            indexX = rightList.FindIndex(t => rightList.IndexOf(t) < rightList.Count - 1 && t.TokenType == Token.Type.NUMBER && rightList[rightList.IndexOf(t) + 1].CValue == '+');
            if (indexX < 0)
            {
                if (rightList.Count == 1 && leftList[0].TokenType == Token.Type.NUMBER)
                {
                    indexX = 0;
                }
                else
                {
                    indexX = rightList.FindIndex(t => rightList.IndexOf(t) > 0 && t.TokenType == Token.Type.NUMBER && rightList[rightList.IndexOf(t) - 1].CValue == '+');
                }
            }
            double d = indexX < 0 ? 0 : rightList[indexX].Value;


            Console.WriteLine(a);
            Console.WriteLine(c);
            Console.WriteLine(b);
            Console.WriteLine(d);

            if (a - c == 0) return Double.NaN;
            else if (d - b == 0) return Double.PositiveInfinity;
            else return (d - b) / (a - c);
        }
    }
}
