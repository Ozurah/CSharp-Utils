using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ozurah.Utils
{
    public class Printer
    {
        /* TODO Problèmes connus / a venir
         * 
            Printer.Print(new string[] { "a", "b", "c" });
            Printer.Print(new string[][] { new string[] { "a", "b" }, new string[] { "c", "d" } });
            Printer.Print(new object[] {new string[] { "a", "b" } });

            -->

            "a", "b", "c"
            a["a", "b"], a["c", "d"]
            a["a", "b"]

            
            ==> On perd l'info du premier niveau du tableau, car il viens se décomposer dans l'args (idem pour tout autre "type référence" !)
         * 
         * 
         * 
            Print des objets
            Print des struct
            Print des namedtuple
         * 
         */

        private const string NULL_REPR = "null";

        private static OrderedDictionary<Type, (string startSeq, string endSeq)> indicatorDefault = new()
        {
            // Les éléments de bases, servant à construire les autres via `BuildIndicatorFor`
            { typeof(string), ("\"", "\"") },
            { typeof(char), ("'", "'") },
            { typeof(Dictionary<,>), ("{", "}") },
            { typeof(List<>), ("l[", "]") },
            { typeof(object[]), ("a[", "]") },
            { typeof(ITuple), ("(", ")") },
            { typeof(IEnumerable), ("col(", ")") },
        };

        public static OrderedDictionary<Type, (string startSeq, string endSeq)> Indicator { get; private set; } = new(indicatorDefault);

        public static void ResetIndicator()
        {
            Indicator = new(indicatorDefault);
        }

        public static void WriteLine(params object?[]? args)
        {
            Debug.WriteLine(
                args is null ?
                NULL_REPR :
                string.Join(", ", args.Select(x => x ?? NULL_REPR))
            );
        }

        public static void Print(params object?[]? args)
        {
            WriteLine(PrintStr(args));
        }

        private static void BuildIndicatorFor(object? obj)
        {
            if (obj is null) return;

            if (!Indicator.ContainsKey(obj.GetType()))
            {
                // Check 1 : Tableau
                if (obj.GetType().IsArray)
                {
                    Indicator.Insert(0, obj.GetType(), Indicator[typeof(object[])]);
                    return;
                }

                // Check 2 : Objet existant vers un générique
                foreach (var kv in Indicator)
                {
                    if (obj.GetType().IsGenericType && kv.Key.IsGenericType && kv.Key.IsAssignableFrom(obj.GetType().GetGenericTypeDefinition()))
                    {
                        Indicator.Insert(0, obj.GetType(), kv.Value);
                        return;
                    }
                }

                // Check 3 : Héritage
                foreach (var kv in Indicator)
                {
                    if (kv.Key.IsInstanceOfType(obj))
                    {
                        Indicator.Insert(0, obj.GetType(), kv.Value);
                        return;
                    }
                }

                // Check 4 : Non trouvé
                Indicator.Insert(0, obj.GetType(), ("", ""));
            }
        }

        public static string PrintStr(params object?[]? args)
        {
            if (args is null)
            {
                return NULL_REPR;
            }
            const string SEPARATOR = ", ";
            string text = "";
            bool first = true;
            foreach (var arg in args)
            {
                if (!first)
                    text += SEPARATOR;

                if (arg is null)
                    text += NULL_REPR;
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

                            // https://stackoverflow.com/questions/2729614/c-sharp-reflection-how-can-i-tell-if-object-o-is-of-type-keyvaluepair-and-then
                            Type? type = item?.GetType();
                            if (type is not null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                            {
                                var key = type.GetProperty("Key")?.GetValue(item, null);
                                var value = type.GetProperty("Value")?.GetValue(item, null);

                                text += PrintStr(key);
                                text += ": ";
                                text += PrintStr(value);
                            }
                            else
                                text += PrintStr(item);
                            firstCol = false;
                        }
                    }
                    else if (typeof(ITuple).IsAssignableFrom(arg.GetType()))
                    {
                        // ToArray pour transformer l'enumerable en params (devient un object[] (=>  args = [1, 2, 3] ))
                        // Remarque :
                        //  Si on avait new List<object> { 1, 2, 3 }.ToArray(), ça marcherait aussi car devient un object[] (=>  args = [1, 2, 3])
                        //          Idem pour des instances d'objets (ex. List<Person>...ToArray() )
                        //            ou string / autres types references
                        //  Par contre, new List<int> { 1, 2, 3 }.ToArray() ne marcherait pas car devient un int[], et donc est transmis comme un int[3] à l'index 0 (=>  args = [[1, 2, 3]] )
                        //          Idem pour les autres types valeurs (bool, float, ... https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types )
                        //  !!!! Les strings ne sont pas des types valeurs, mais des types références !! https://stackoverflow.com/questions/636932/in-c-why-is-string-a-reference-type-that-behaves-like-a-value-type
                        text += PrintStr((arg as ITuple)?.AsEnumerable().ToArray());
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
