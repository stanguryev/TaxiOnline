using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.Toolkit.Text
{
    /// <summary>
    /// класс предоставляет допонительные возможности по работе со строками
    /// </summary>
    public static class StringTools
    {
        /// <summary>
        /// удалить вхождения строки в начале исходной строки
        /// </summary>
        /// <param name="sourceString">исходная строка</param>
        /// <param name="stringToTrim">удаляемая строка</param>
        /// <param name="mode"></param>
        /// <returns>строка с отсутствием заданной строки в начале</returns>
        public static string TrimStart(this string sourceString, string stringToTrim, TrimMode mode = TrimMode.None)
        {
            int charactersToTrim = GetCharactersToTrimStart(sourceString, stringToTrim, mode);
            return sourceString.Substring(charactersToTrim);
        }

        /// <summary>
        /// удалить вхождения строки в конце исходной строки
        /// </summary>
        /// <param name="sourceString">исходная строка</param>
        /// <param name="stringToTrim">удаляемая строка</param>
        /// <param name="mode"></param>
        /// <returns>строка с отсутствием заданной строки в конце</returns>
        public static string TrimEnd(this string sourceString, string stringToTrim, TrimMode mode = TrimMode.None)
        {
            int charactersToLeave = GetCharactersToLeaveToTrimEnd(sourceString, stringToTrim, mode);
            return sourceString.Substring(0, charactersToLeave);
        }

        /// <summary>
        /// удалить вхождения строки в начале и в конце исходной строки
        /// </summary>
        /// <param name="sourceString">исходная строка</param>
        /// <param name="stringToTrim">удаляемая строка</param>
        /// <param name="mode"></param>
        /// <returns>строка с отсутствием заданной строки в начале и в конце</returns>
        public static string Trim(this string sourceString, string stringToTrim, TrimMode mode = TrimMode.None)
        {
            int charactersToTrim = GetCharactersToTrimStart(sourceString, stringToTrim, mode);
            int charactersToLeave = GetCharactersToLeaveToTrimEnd(sourceString, stringToTrim, mode);
            charactersToLeave -= charactersToTrim;
            if (charactersToLeave < 0)
                charactersToLeave = 0;
            return sourceString.Substring(charactersToTrim, charactersToLeave);
        }

        private static unsafe int GetCharactersToTrimStart(string sourceString, string stringToTrim, TrimMode mode)
        {
            int stringToTrimLength = stringToTrim.Length;
            int sourceStringLength = sourceString.Length;
            int charToTrimIndex = 0;
            int charactersToTrim = 0;
            fixed (char* charToTrim = stringToTrim)
            fixed (char* sourceChar = sourceString)
                while (charactersToTrim < sourceStringLength - 1)
                {
                    if (sourceChar[charactersToTrim] != charToTrim[charToTrimIndex])
                        break;
                    charactersToTrim++;
                    if ((mode & TrimMode.Once) != TrimMode.None && charactersToTrim == stringToTrimLength)
                        break;
                    charToTrimIndex = (charToTrimIndex + 1) % sourceStringLength;
                }
            if ((mode & TrimMode.WholeEntry) != TrimMode.None)
                charactersToTrim -= charactersToTrim % stringToTrimLength;
            return charactersToTrim;
        }

        private static unsafe int GetCharactersToLeaveToTrimEnd(string sourceString, string stringToTrim, TrimMode mode)
        {
            int stringToTrimLength = stringToTrim.Length;
            int sourceStringLength = sourceString.Length;
            int charactersToLeave = sourceString.Length;
            int charToTrimIndex = stringToTrimLength - 1;
            fixed (char* charToTrim = stringToTrim)
            fixed (char* sourceChar = sourceString)
                while (charactersToLeave > 0)
                {
                    if (sourceChar[charactersToLeave - 1] != charToTrim[charToTrimIndex--])
                        break;
                    charactersToLeave--;
                    if ((mode & TrimMode.Once) != TrimMode.None && sourceStringLength - charactersToLeave == stringToTrimLength)
                        break;
                    if (charToTrimIndex < 0)
                        charToTrimIndex = stringToTrimLength - 1;
                }
            if ((mode & TrimMode.WholeEntry) != TrimMode.None)
                charactersToLeave += (sourceStringLength - charactersToLeave) % stringToTrimLength;
            return charactersToLeave;
        }
    }
}
