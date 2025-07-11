﻿namespace Ozurah.Utils.Enums
{
    // Note : Nammed "Enum utils" (instead of simply "Enums") to avoid issue if call "Enums.XXX" inside our "Enum data classes" (their namespace override this one)
    // Note : Having generics is more "confortable" instead typing everytime "typeof(XXX)"

    /// <summary>
    /// Utility class to get some informations about an enum, like if the name is defined or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks> This class provide aliases or additionals methods for <see cref="Enum"/></remarks>
    public static class EnumUtils<T> where T : struct, Enum
    {
        // IsDefined source : https://stackoverflow.com/questions/13248869/c-sharp-enum-contains-value
        /// <summary>
        /// Alias for <see cref="Enum.IsDefined(Type, object)"/> <br/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Is the name / value exists in the enum ; <see langword="null"/> -> <see langword="false"/></returns>
        public static bool IsDefined(object? value)
        {
            if (value is null) return false;
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// If <paramref name="value"/> is not defined for the enum, throw an <see cref="ArgumentException"/> with <paramref name="message"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowIfUnkownValue(object? value, string? message = null)
        {
            message ??= $"Value '{value}' is not defined for enum '{typeof(T).Name}'";

            if (value is null) throw new ArgumentNullException(nameof(value), message);

            if (!IsDefined(value))
                throw new ArgumentException(message);
        }

        /// <summary>
        /// Alias for <see cref="Enum.GetValues"/>
        /// </summary>
        /// <returns></returns>
        public static T[] ToArray()
        {
            return Enum.GetValues<T>();
        }

        /// <summary>
        /// Alias for <see cref="Enum.Parse"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T FromString(string name, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            return Enum.Parse<T>(name, ignoreCase);
        }
    }
}
