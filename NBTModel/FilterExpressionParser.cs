using System;
using System.Collections.Generic;

namespace NBTExplorer.Model
{
    internal class FilterExpressionParser
    {
        private Stack<string> argStack = new Stack<string>();

        /*public bool Parse (DataNode targetNode, List<string> tokens)
        {
            Queue<string> tokenQueue = new Queue<string>(FilterExpressionConverter.Convert(tokens));

            while (tokenQueue.Count > 0) {
                string token = tokenQueue.Dequeue();

                switch (token) {
                    case "equal":
                }
            }
        }*/
    }

    internal static class FilterExpressionConverter
    {
        private static readonly List<List<string>> OperatorGroups = new List<List<string>>
        {
            new List<string> {"equal", "greater", "less", "contains", "begins", "ends"},
            new List<string> {"not"},
            new List<string> {"and", "or"}
        };

        public static List<string> Convert(List<string> tokens)
        {
            var tokenQueue = new Queue<string>(tokens);
            var output = new List<string>();
            var opStack = new Stack<string>();

            while (tokenQueue.Count > 0)
            {
                var token = tokenQueue.Dequeue();

                if (IsGroupStart(token))
                {
                    opStack.Push(token);
                }
                else if (IsGroupEnd(token))
                {
                    while (opStack.Count > 0 && !IsGroupStart(opStack.Peek()))
                        output.Add(opStack.Pop());
                    if (opStack.Count == 0)
                        throw new Exception("Mismatched grouping");
                    opStack.Pop();
                }
                else if (IsOperator(token))
                {
                    while (opStack.Count > 0 && IsOperator(opStack.Peek()))
                        if (Precedence(token) > Precedence(opStack.Peek()))
                            output.Add(opStack.Pop());
                    opStack.Push(token);
                }
                else
                {
                    output.Add(token);
                }
            }

            while (opStack.Count > 0)
            {
                if (IsGroupStart(opStack.Peek()))
                    throw new Exception("Mismatched grouping");
                output.Add(opStack.Pop());
            }

            return output;
        }

        private static bool IsGroupStart(string token)
        {
            return token == "(";
        }

        private static bool IsGroupEnd(string token)
        {
            return token == ")";
        }

        private static bool IsOperator(string token)
        {
            foreach (var group in OperatorGroups)
                if (group.Contains(token))
                    return true;
            return false;
        }

        private static int Precedence(string op)
        {
            for (var i = 0; i < OperatorGroups.Count; i++)
                if (OperatorGroups[i].Contains(op))
                    return i;
            return int.MaxValue;
        }
    }
}