using Ozurah.Utils.Tuples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
            Print des objets -> S'ils implémente le ToString, affichera le ToString / sinon, affichera la classe
                                Pour afficher la classe au lieu du ToString, il suffit de `Print(obj.GetType()())`
            Print des struct -> A l'instar des objets
            Print des namedtuple -> Comme un simple tuple, on affiche pas le noms "items"

            Print des types anonymes : non traité, Par exemple `new { a = 10, b = "str" }` s'affichera comme suit : `{ a = 10, b = str }`
                                        (fonctionnement par défaut) ==> il y aura les {}, et pas de formatage pour le contenu
                                        Exemple de détection
                                            https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
                                            https://stackoverflow.com/a/79255648
         * 
         */

        public enum WriteLineMode
        {
            Console,
            Debug,
            Silent,
            UnityLog,
        }

        public static WriteLineMode UsedWriteLineMode { get; set; } = Printer.WriteLineMode.Debug;

        public static Func<string>? WriteLinePrefix { get; set; } = null;
        public static Func<string>? WriteLineSuffix { get; set; } = null;

        private const string NULL_REPR = "null";

        private static Dictionary<Type, (string startSeq, string endSeq)> indicatorDefault = new()
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

        public static Dictionary<Type, (string startSeq, string endSeq)> Indicator { get; private set; } = new(indicatorDefault);

        public static void ResetIndicator()
        {
            Indicator = new(indicatorDefault);
        }

        public static void WriteLine(params object?[]? args)
        {
            string txt = args is null ?
                NULL_REPR :
                string.Join(", ", args.Select(x => x ?? NULL_REPR));

            string prefix = WriteLinePrefix is null ? "" : WriteLinePrefix() + " ";
            string suffix = WriteLineSuffix is null ? "" : " " + WriteLineSuffix();

            txt = prefix + txt + suffix;

            switch (UsedWriteLineMode)
            {
                case WriteLineMode.Console:
                    Console.WriteLine(txt);
                    break;
                case WriteLineMode.Debug:
                    Debug.WriteLine(txt);
                    break;
                case WriteLineMode.Silent:
                    break; // Pas de sortie
                case WriteLineMode.UnityLog:
                    UnityEngine.Debug.Log(txt);
                    break;
                default:
                    Debug.WriteLine("!! Mode non géré, affichage dans la sortie de debug !!as");
                    Debug.WriteLine(txt);
                    break;
            }
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
                    Indicator.Add(obj.GetType(), Indicator[typeof(object[])]);
                    return;
                }

                // Check 2 : Objet existant vers un générique
                foreach (var kv in Indicator)
                {
                    if (obj.GetType().IsGenericType && kv.Key.IsGenericType && kv.Key.IsAssignableFrom(obj.GetType().GetGenericTypeDefinition()))
                    {
                        Indicator.Add(obj.GetType(), kv.Value);
                        return;
                    }
                }

                // Check 3 : Héritage
                foreach (var kv in Indicator)
                {
                    if (kv.Key.IsInstanceOfType(obj))
                    {
                        Indicator.Add(obj.GetType(), kv.Value);
                        return;
                    }
                }

                // Check 4 : Non trouvé
                Indicator.Add(obj.GetType(), ("", ""));
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
                        text += arg; // .ToString non nécessaire, il est traité par défaut dans ce cas :)
                    }
                    text += indication.endSeq;
                }
                first = false;
            }

            return text;
        }

        public static void PrintObject(object obj, bool usePrintStr = false)
        {
            // Permet d'afficher les champs/propriété publics d'un objet (ou struct)

            Type type = obj.GetType();

            WriteLine($"Struct Type = {type}");

            // Champs (public uniquement), possibilité d'ajouter d'autres flags pour les privé par exemple
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = field.GetValue(obj);
                if (usePrintStr)
                    value = PrintStr(value);
                Debug.WriteLine($"(Field) {field.Name} = {value}");
            }

            // Propriétés (si le struct en a)
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead)
                    continue;

                var value = prop.GetValue(obj);
                if (usePrintStr)
                    value = PrintStr(value);
                Debug.WriteLine($"(Property) {prop.Name} = {value}");
            }
        }
    }
}
