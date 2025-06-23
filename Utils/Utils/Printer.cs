using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozurah.Utils
{
    public class Printer
    {
        public static OrderedDictionary<Type, (string startSeq, string endSeq)> Indicator { get; private set; } = new()
        {
            { typeof(string), ("\"", "\"") },
            { typeof(char), ("'", "'") },
            { typeof(Dictionary<,>), ("{", "}") },
            { typeof(List<>), ("[", "]") },
            { typeof(IEnumerable), ("col(", ")") },
        };

        public static void WriteLine(params object[] args)
        {
            Debug.WriteLine(string.Join(", ", args));
        }

        public static void Print(params object[] args)
        {
            WriteLine(PrintStr(args));
        }

        private static void BuildIndicatorFor(object obj)
        {
            if (!Indicator.ContainsKey(obj.GetType()))
            {
                WriteLine(obj);

                foreach (var kv in Indicator)
                {
                    if (kv.Key.IsInstanceOfType(obj))
                    {
                        Indicator.Insert(0, obj.GetType(), kv.Value);
                        break;
                    }
                }
            }
        }

        public static string PrintStr(params object[] args)
        {
            const string SEPARATOR = ", ";
            string text = "";
            bool first = true;
            foreach (var arg in args)
            {
                if (!first)
                    text += SEPARATOR;

                if (arg is null)
                    text += "null";
                else
                {
                    BuildIndicatorFor(arg);

                    // TryGetValue ne supporte pas les Null
                    Indicator.TryGetValue(arg.GetType(), out (string startSeq, string endSeq) indication);

                    text += indication.startSeq;

                    if (arg is IEnumerable enumerable && arg is not string)
                    {
                        //text += "[" + string.Join(", ", enumerable.Select(t => t.ToString())) + "]"; // IEnumerable (et pas IEnumerable<object> par exemple) n'a pas le `.Select`

                        bool firstCol = true;
                        foreach (var item in enumerable)
                        {
                            if (!firstCol) text += SEPARATOR;
                            text += PrintStr(item);
                            firstCol = false;
                        }
                    }
                    else
                    {
                        text += arg.ToString();
                    }
                    text += indication.endSeq;
                }
                first = false;
            }

            return text;
        }
    }
}
