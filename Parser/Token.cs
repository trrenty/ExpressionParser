using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressionParser.Parser
{
    public class Token
    {
        public Type TokenType { get; set; }
        public char CValue { get; set; }
        public double Value { get; set; }

        public Token(double x)
        {
            TokenType = Type.NUMBER;
            Value = x;
            CValue = '\0';

        }

        public Token(char x)
        {
            if ("+-/*".Contains(x))
            {
                TokenType = Type.OPERATOR;

            }
            else if ('x' == x)
            {
                TokenType = Type.X;
            }
            else if ('=' == x)
            {
                TokenType = Type.EQUAL;
            }
            else throw new ArgumentException();

            CValue = x;
            Value = 0;

        }

        public enum Type
        {
            NUMBER, OPERATOR, X, EQUAL
        }

        public override string ToString()
        {
            if (Value != 0)
            {
                return "(" + Value + ")";
            }
            else return "(" + CValue + ")";
        }

        public static List<Token> StringToTokens(string str)
        {
            // -10x+3=-11x-4

            List<Token> finalTokens = new List<Token>();


            int lastPos = 0;
            string[] sides = str.Split("=");
            bool negative = false;

            foreach (string side in sides)
            {
                string[] tokens;
                if (side[0] == '-')
                {
                    tokens = side.Substring(1).Split(new char[] { '+', '-', '/', '*' });
                    negative = true;
                    lastPos++;
                }
                else tokens = side.Split(new char[] { '+', '-', '/', '*' });

                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i].Contains("x") && tokens[i].Length == 1)
                    {
                        finalTokens.Add(new Token('x'));
                        lastPos++;
                        finalTokens.Add(new Token(str[lastPos]));
                        lastPos++;
                    }
                    else if (tokens[i].Contains("x") && tokens[i].Length > 1)
                    {
                        double nr = Convert.ToDouble(tokens[i].Remove(tokens[i].Length - 1));
                        if (negative && i == 0)
                        {
                            finalTokens.Add(new Token(-nr));
                            negative = false;
                        }
                        else
                            finalTokens.Add(new Token(nr));
                        finalTokens.Add(new Token('*'));
                        finalTokens.Add(new Token('x'));
                        lastPos += tokens[i].Length;
                        if (lastPos < str.Length && "+-/*".Contains(str[lastPos]))
                        {
                            finalTokens.Add(new Token(str[lastPos]));
                            lastPos++;
                        }
                    }
                    else
                    {
                        double nr = Convert.ToDouble(tokens[i]);
                        if (negative && i == 0)
                        {
                            finalTokens.Add(new Token(-nr));
                            negative = false;
                        }
                        else
                            finalTokens.Add(new Token(nr));
                        lastPos += tokens[i].Length;
                        if (lastPos < str.Length && "+-/*".Contains(str[lastPos]))
                        {
                            finalTokens.Add(new Token(str[lastPos]));
                            lastPos++;
                        }

                    }
                }
                negative = false;
                if (lastPos <= str.IndexOf('='))
                {
                    finalTokens.Add(new Token('='));
                    lastPos++;
                }
            }
            return finalTokens;
        }
    }
}
