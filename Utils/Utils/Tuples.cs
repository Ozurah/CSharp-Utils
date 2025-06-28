using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ozurah.Utils.Tuples
{
    public static class Tuples
    {
        public static IEnumerable<object> AsEnumerable(this ITuple tuple)
        {
            // https://stackoverflow.com/questions/39335805/how-to-turn-a-tuple-into-an-array-in-c
            for (var index = 0; index < tuple.Length; index++)
                yield return tuple[index];
        }
    }
}