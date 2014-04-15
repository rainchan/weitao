using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WT.Components.Common.Utility
{
    public static class StringUtil
    {
        #region string convert
        public static string GetStringValue(object obj)
        {
            if (obj == null) return string.Empty;

            return obj.ToString();
        }

        public static bool GetBoolValue(object obj)
        {
            if (obj == null) return false;

            return GetBoolValue(obj.ToString());
        }
        public static bool GetBoolValue(string str)
        {
            if (IsBool(str))
            {
                return bool.Parse(str);
            }
            else
            {
                return false;
            }
        }
        public static int GetIntValue(object obj)
        {
            if (obj == null) return 0;

            return GetIntValue(obj.ToString());
        }
        public static int GetIntValue(string str)
        {
            if (IsInt(str))
            {
                return int.Parse(str);
            }
            else
            {
                return 0;
            }
        }
        public static long GetLongValue(object obj)
        {
            if (obj == null) return 0;

            return GetLongValue(obj.ToString());
        }
        public static long GetLongValue(string str)
        {
            if (IsLong(str))
            {
                return long.Parse(str);
            }
            else
            {
                return 0;
            }
        }
        public static decimal GetDecimalValue(object obj)
        {
            if (obj == null) return 0;

            return GetDecimalValue(obj.ToString());
        }
        public static decimal GetDecimalValue(string str)
        {
            if (IsDecimal(str))
            {
                return decimal.Parse(str);
            }
            else
            {
                return 0;
            }
        }
        public static DateTime GetDateTimeValue(object obj)
        {
            if (obj == null) return DateTime.MinValue;

            return GetDateTimeValue(obj.ToString());
        }
        public static DateTime GetDateTimeValue(string str)
        {
            if (IsDateTime(str))
            {
                return DateTime.Parse(str);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        #region private

        private static bool IsBool(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            bool tempInt;
            return bool.TryParse(beCheckedStr, out tempInt);
        }
        private static bool IsInt(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            int tempInt;
            return int.TryParse(beCheckedStr, out tempInt);
        }
        private static bool IsLong(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            long tempInt;
            return long.TryParse(beCheckedStr, out tempInt);
        }
        private static bool IsDecimal(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            decimal tempDecimal;
            return decimal.TryParse(beCheckedStr, out tempDecimal);
        }
        private static bool IsDouble(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            double tempDouble;
            return double.TryParse(beCheckedStr, out tempDouble);
        }
        private static bool IsDateTime(string beCheckedStr)
        {
            if (string.IsNullOrEmpty(beCheckedStr)) return false;

            DateTime tempDateTime;
            return DateTime.TryParse(beCheckedStr, out tempDateTime);
        }
        #endregion
        #endregion

        #region word filter

        /// <summary>
        /// 过滤 Sql 语句字符串中的注入脚本
        /// </summary>
        /// <param name="source">传入的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlFilter(string source)
        {
            source = source.Replace("\"", "");
            source = source.Replace("&", "&amp");
            source = source.Replace("<", "&lt");
            source = source.Replace(">", "&gt");
            source = source.Replace("delete", "");
            source = source.Replace("update", "");
            source = source.Replace("insert", "");
            source = source.Replace("'", "''");
            source = source.Replace(";", "；");
            source = source.Replace("(", "（");
            source = source.Replace(")", "）");
            source = source.Replace("Exec", "");
            source = source.Replace("Execute", "");
            source = source.Replace("xp_", "x p_");
            source = source.Replace("sp_", "s p_");
            source = source.Replace("0x", "0 x");
            return source;
        }
        /// <summary>
        /// Removes all the words passed in the filter words 
        /// parameters. The replace is NOT case
        /// sensitive.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="filterWords">The words to 
        /// repace in the input string.</param>
        /// <returns>A string.</returns>
        public static string FilterWords(string input,
           params string[] filterWords)
        {
            return StringUtil.FilterWords(input,
               char.MinValue, filterWords);
        }
        /// <summary>
        /// Removes all the words passed in the filter words 
        /// parameters. The replace is NOT case
        /// sensitive.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="mask">A character that is inserted for each 
        /// letter of the replaced word.</param>
        /// <param name="filterWords">The words to 
        // repace in the input string.</param>
        /// <returns>A string.</returns>
        public static string FilterWords(string input, char mask,
           params string[] filterWords)
        {
            string stringMask = mask == char.MinValue ?
               string.Empty : mask.ToString();
            string totalMask = stringMask;

            foreach (string s in filterWords)
            {
                Regex regEx = new Regex(s,
                   RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < s.Length; i++)
                        totalMask += stringMask;
                }

                input = regEx.Replace(input, totalMask);

                totalMask = stringMask;
            }

            return input;
        }

        /// <summary>
        /// Checks the passed string to see if has any of the 
        /// passed words. Not case-sensitive.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="hasWords">The words to check for.</param>
        /// <returns>A collection of the matched words.</returns>
        public static MatchCollection HasWords(string input,
           params string[] hasWords)
        {
            StringBuilder sb = new StringBuilder(hasWords.Length + 50);
            //sb.Append("[");

            foreach (string s in hasWords)
            {
                sb.AppendFormat("({0})|",
                   StringUtil.HtmlSpecialEntitiesEncode(s.Trim()));
            }

            string pattern = sb.ToString();
            pattern = pattern.TrimEnd('|'); // +"]";

            Regex regEx = new Regex(pattern,
               RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Matches(input);
        }

        #endregion

        #region encode and decode
        /// <summary>
        /// Base64 encodes a string.
        /// </summary>
        /// <param name="input">A string</param>
        /// <returns>A base64 encoded string</returns>
        public static string Base64StringEncode(string input)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64 decodes a string.
        /// </summary>
        /// <param name="input">A base64 encoded string</param>
        /// <returns>A decoded string</returns>
        public static string Base64StringDecode(string input)
        {
            byte[] decbuff = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="input">The string to be encoded</param>
        /// <returns>An encoded string</returns>
        public static string HtmlSpecialEntitiesEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// A wrapper around HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="input">The string to be decoded</param>
        /// <returns>The decode string</returns>
        public static string HtmlSpecialEntitiesDecode(string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        static readonly String baseDigits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        static int baseNum = 62;
        public static string Base62ToString(long fromValue)
        {

            string toValue = fromValue == 0 ? "0" : "";

            int mod = 0;

            while (fromValue != 0)
            {
                mod = (int)(fromValue % baseNum); //should be safe

                toValue = baseDigits.Substring(mod, 1) + toValue;

                fromValue = fromValue / baseNum;

            }
            return toValue;
        }
        public static long Base62FromString(string sBase)
        {

            long dec = 0;
            int b = 0;
            long iProduct = 1;

            for (int i = sBase.Length - 1; i >= 0; i--, iProduct *= baseNum)
            {
                string sValue = sBase[i].ToString();
                b = baseDigits.IndexOf(sBase[i]);
                dec += (b * iProduct);
            }
            return dec;

        }
        #endregion

        #region string orperate
        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="input">The string to examine.</param>
        /// <param name="newValue">The value to replace.</param>
        /// <param name="oldValue">The new value to be inserted</param>
        /// <returns>A string</returns>
        public static string CaseInsenstiveReplace(string input,
           string newValue, string oldValue)
        {
            Regex regEx = new Regex(oldValue,
               RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(input, newValue);
        }
        /// <summary>
        /// Replaces the first occurence of a string with the replacement value. The Replace
        /// is case senstive
        /// </summary>
        /// <param name="input">The string to examine</param>
        /// <param name="oldValue">The value to replace</param>
        /// <param name="newValue">the new value to be inserted</param>
        /// <returns>A string</returns>
        public static string ReplaceFirst(string input, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.Multiline);
            return regEx.Replace(input, newValue, 1);
        }

        /// <summary>
        /// Replaces the last occurence of a string with the replacement value.
        /// The replace is case senstive.
        /// </summary>
        /// <param name="input">The string to examine</param>
        /// <param name="oldValue">The value to replace</param>
        /// <param name="newValue">the new value to be inserted</param>
        /// <returns>A string</returns>
        public static string ReplaceLast(string input, string oldValue, string newValue)
        {
            int index = input.LastIndexOf(oldValue);
            if (index < 0)
            {
                return input;
            }
            else
            {
                StringBuilder sb = new StringBuilder(input.Length - oldValue.Length + newValue.Length);
                sb.Append(input.Substring(0, index));
                sb.Append(newValue);
                sb.Append(input.Substring(index + oldValue.Length,
                   input.Length - index - oldValue.Length));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Left pads the passed input using the HTML 
        /// non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="totalSpaces">The total number 
        /// to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeftHtmlSpaces(string input,
           int totalSpaces)
        {
            string space = "&nbsp;";
            return PadLeft(input, space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not 
        /// cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to 
        /// pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeft(string input,
           string pad, int totalWidth)
        {
            return StringUtil.PadLeft(input, pad, totalWidth, false);
        }

        /// <summary>
        /// Left pads the passed input using the passed pad string
        /// for the total number of spaces.  It will 
        /// cut-off the pad so that  
        /// the string does not exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to 
        /// pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadLeft(string input, string pad,
           int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            int padCount = pad.Length;
            string paddedString = input;

            while (paddedString.Length < totalWidth)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth);

            return paddedString;
        }

        /// <summary>
        /// Right pads the passed input using the HTML 
        /// non-breaking string entity (&nbsp;)
        /// for the total number of spaces.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="totalSpaces">The total number 
        /// to pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRightHtmlSpaces(string input,
           int totalSpaces)
        {
            string space = "&nbsp;";
            return PadRight(input, space, totalSpaces * space.Length);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will not 
        /// cut-off the pad even if it 
        /// causes the string to exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to 
        /// pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRight(string input,
           string pad, int totalWidth)
        {
            return StringUtil.PadRight(input, pad, totalWidth, false);
        }

        /// <summary>
        /// Right pads the passed input using the passed pad string
        /// for the total number of spaces.  It will cut-off
        /// the pad so that  
        /// the string does not exceed the total width.
        /// </summary>
        /// <param name="input">The string to pad.</param>
        /// <param name="pad">The string to uses as padding.</param>
        /// <param name="totalSpaces">The total number to 
        /// pad the string.</param>
        /// <returns>A padded string.</returns>
        public static string PadRight(string input, string pad,
           int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            string paddedString = string.Empty;

            while (paddedString.Length < totalWidth - input.Length)
            {
                paddedString += pad;
            }

            // trim the excess.
            if (cutOff)
                paddedString = paddedString.Substring(0,
                   totalWidth - input.Length);

            paddedString += input;

            return paddedString;
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return (\r) symbols.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <returns>A string</returns>
        public static string RemoveNewLines(string input)
        {
            return StringUtil.RemoveNewLines(input, false);
        }

        /// <summary>
        /// Removes the new line (\n) and carriage return 
        /// (\r) symbols.
        /// </summary>
        /// <param name="input">The string to search.</param>
        /// <param name="addSpace">If true, adds a space 
        /// (" ") for each newline and carriage
        /// return found.</param>
        /// <returns>A string</returns>
        public static string RemoveNewLines(string input,
           bool addSpace)
        {
            string replace = string.Empty;
            if (addSpace)
                replace = " ";

            string pattern = @"[\r|\n]";
            Regex regEx = new Regex(pattern,
               RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return regEx.Replace(input, replace);
        }

        /// <summary>
        /// Reverse a string.
        /// </summary>
        /// <param name="input">The string to reverse</param>
        /// <returns>A string</returns>
        /// <remarks>Thanks to  Alois Kraus for pointing out an issue
        /// with an earlier version of this method and to Justin Roger's 
        /// site (http://weblogs.asp.net/justin_rogers/archive/2004/06/10/153175.aspx)
        /// for helping me to improve that previous method.</remarks>
        public static string Reverse(string input)
        {
            char[] reverse = new char[input.Length];
            for (int i = 0, k = input.Length - 1; i < input.Length; i++, k--)
            {
                if (char.IsSurrogate(input[k]))
                {
                    reverse[i + 1] = input[k--];
                    reverse[i++] = input[k];
                }
                else
                {
                    reverse[i] = input[k];
                }
            }
            return new System.String(reverse);
        }

        /// <summary>
        /// Converts a string to sentence case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SentenceCase(string input)
        {
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() +
               sentence.Substring(1);
        }

        /// <summary>
        /// Converts all spaces to HTML non-breaking spaces
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string</returns>
        public static string SpaceToNbsp(string input)
        {
            string space = "&nbsp;";
            return input.Replace(" ", space);
        }

        public static string StripTagsAndSpaceFromNbsp(string input)
        {
            string triped = StripTags(input);
            return triped.Replace("&nbsp;", " ");
        }

        /// <summary>
        /// Removes all HTML tags from the passed string
        /// </summary>
        /// <param name="input">The string whose 
        /// values should be replaced.</param>
        /// <returns>A string.</returns>
        public static string StripTags(string input)
        {
            Regex stripTags = new Regex("<(.|\n)+?>");
            return stripTags.Replace(input, "");
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string.</returns>
        public static string TitleCase(string input)
        {
            return TitleCase(input, true);
        }

        /// <summary>
        /// Converts a string to title case.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <param name="ignoreShortWords">If true, 
        /// does not capitalize words like
        /// "a", "is", "the", etc.</param>
        /// <returns>A string.</returns>
        public static string TitleCase(string input,
           bool ignoreShortWords)
        {
            List<string> ignoreWords = null;
            if (ignoreShortWords)
            {
                //TODO: Add more ignore words?
                ignoreWords = new List<string>();
                ignoreWords.Add("a");
                ignoreWords.Add("is");
                ignoreWords.Add("was");
                ignoreWords.Add("the");
            }

            string[] tokens = TrimIntraWords(input).Split(' ');
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (string s in tokens)
            {
                if (ignoreShortWords == true
                    && s != tokens[0]
                    && ignoreWords.Contains(s.ToLower()))
                {
                    sb.Append(s + " ");
                }
                else
                {
                    sb.Append(s[0].ToString().ToUpper());
                    sb.Append(s.Substring(1).ToLower());
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Removes multiple spaces between words
        /// </summary>
        /// <param name="input">The string to trim.</param>
        /// <returns>A string.</returns>
        public static string TrimIntraWords(string input)
        {
            Regex regEx = new Regex(@"[\s]+");
            return regEx.Replace(input, " ");
        }

        /// <summary>
        /// Converts new line(\n) and carriage return(\r) symbols to
        /// HTML line breaks.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>A string.</returns>
        public static string NewLineToBreak(string input)
        {
            Regex regEx = new Regex(@"[\n|\r]+");
            return regEx.Replace(input, "<br />");
        }

        /// <summary>
        /// Wraps the passed string at the 
        /// at the next whitespace on or after the 
        /// total charCount has been reached
        /// for that line.  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of characters 
        /// per line.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(string input, int charCount)
        {
            return StringUtil.WordWrap(input, charCount,
               false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string at the total 
        /// number of characters (if cuttOff is true)
        /// or at the next whitespace (if cutOff is false).
        /// Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of characters 
        /// per line.</param>
        /// <param name="cutOff">If true, will break in 
        /// the middle of a word.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(string input,
           int charCount, bool cutOff)
        {
            return StringUtil.WordWrap(input, charCount,
               cutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string at the total number 
        /// of characters (if cuttOff is true)
        /// or at the next whitespace (if cutOff is false).
        /// Uses the passed breakText
        /// for lineBreaks.
        /// </summary>
        /// <param name="input">The string to wrap.</param>
        /// <param name="charCount">The number of 
        /// characters per line.</param>
        /// <param name="cutOff">If true, will break in 
        /// the middle of a word.</param>
        /// <param name="breakText">The line break text to use.</param>
        /// <returns>A string.</returns>
        public static string WordWrap(string input, int charCount,
           bool cutOff, string breakText)
        {
            StringBuilder sb = new StringBuilder(input.Length + 100);
            int counter = 0;

            if (cutOff)
            {
                while (counter < input.Length)
                {
                    if (input.Length > counter + charCount)
                    {
                        sb.Append(input.Substring(counter, charCount));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(input.Substring(counter));
                    }
                    counter += charCount;
                }
            }
            else
            {
                string[] strings = input.Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    // added one to represent the space.
                    counter += strings[i].Length + 1;
                    if (i != 0 && counter > charCount)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }
            // to get rid of the extra space at the end.
            return sb.ToString().TrimEnd();
        }
        public static string[] RemoveDups(string[] items, bool sort)
        {
            ArrayList noDups = new ArrayList();
            for (int i = 0; i < items.Length; i++)
            {

                if ((!noDups.Contains(items[i].Trim())) && ((items[i].Trim() != "")))
                {
                    noDups.Add(items[i].Trim());
                }
            }
            if (sort) noDups.Sort();  //sorts list alphabetically
            string[] uniqueItems = new String[noDups.Count];
            noDups.CopyTo(uniqueItems);
            return uniqueItems;
        }
        #endregion

        /// <summary>   
        ///  获得某个字符串在另个字符串中出现的次数   
        /// </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static int GetStrCount(string strOriginal, string strSymbol)
        {
            int count = 0;
            for (int i = 0; i < (strOriginal.Length - strSymbol.Length + 1); i++)
            {
                if (strOriginal.Substring(i, strSymbol.Length) == strSymbol)
                {
                    count = count + 1;
                }
            }
            return count;
        }
        /// <summary>   
        /// 获得某个字符串在另个字符串第一次出现时前面所有字符   
        /// </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static string GetFirstString(string strOriginal, string strSymbol)
        {
            int strPlace = strOriginal.IndexOf(strSymbol);
            if (strPlace != -1)
                strOriginal = strOriginal.Substring(0, strPlace);
            return strOriginal;
        }
        /// <summary>   
        /// 获得某个字符串在另个字符串最后一次出现时后面所有字符   
        /// </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strSymbol">符号</param>   
        /// <returns>返回值</returns>   
        public static string GetLastString(string strOriginal, string strSymbol)
        {
            int strPlace = strOriginal.LastIndexOf(strSymbol) + strSymbol.Length;
            strOriginal = strOriginal.Substring(strPlace);
            return strOriginal;
        }
        /// <summary>   
        /// 获得两个字符之间第一次出现时前面所有字符   
        /// </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strFirst">最前哪个字符</param>   
        /// <param name="strLast">最后哪个字符</param>   
        /// <returns>返回值</returns>   
        public static string GetTwoMiddleFirstStr(string strOriginal, string strFirst, string strLast)
        {
            strOriginal = GetFirstString(strOriginal, strLast);
            strOriginal = GetLastString(strOriginal, strFirst);
            return strOriginal;
        }

        /// <summary>   
        ///  获得两个字符之间最后一次出现时的所有字符   
        /// </summary>   
        /// <param name="strOriginal">要处理的字符</param>   
        /// <param name="strFirst">最前哪个字符</param>   
        /// <param name="strLast">最后哪个字符</param>   
        /// <returns>返回值</returns>   
        public static string GetTwoMiddleLastStr(string strOriginal, string strFirst, string strLast)
        {
            strOriginal = GetFirstString(strOriginal, strFirst);
            strOriginal = GetLastString(strOriginal, strLast);
            return strOriginal;
        }

        /// <summary>   
        /// 截取左边规定字数字符串,超过字数用endStr结束   
        /// </summary>   
        /// <param name="str">需截取字符串</param>   
        /// <param name="length">截取字数</param>   
        /// <param name="endStr">超过字数，结束字符串，如"..."</param>   
        /// <returns>返回截取字符串</returns>   
        public static string GetLeftStr(string str, int length, string endStr)
        {
            string reStr;
            if (length < GetStrLength(str))
            {
                reStr = str.Substring(0, length) + endStr;
            }
            else
            {
                reStr = str;
            }
            return reStr;
        }
        /// <summary>   
        /// 截取左边规定字数字符串,超过字数用...结束   
        /// </summary>   
        /// <param name="str">需截取字符串</param>   
        /// <param name="length">截取字数</param>   
        /// <returns>返回截取字符串</returns>   
        public static string GetLeftStr(string str, int length)
        {
            string reStr;
            if (length < str.Length)
            {
                reStr = str.Substring(0, length) + "...";
            }
            else
            {
                reStr = str;
            }
            return reStr;
        }
        /// <summary>   
        /// 截取左边规定字数字符串,超过字数用...结束   
        /// </summary>   
        /// <param name="str">需截取字符串</param>   
        /// <param name="length">截取字数</param>   
        /// <param name="subcount">若超过字数右边减少的字符长度</param>   
        /// <returns>返回截取字符串</returns>   
        public static string GetLeftStr(string str, int length, int subcount)
        {
            string reStr;
            if (length < str.Length)
            {
                reStr = str.Substring(0, length - subcount) + "...";
            }
            else
            {
                reStr = str;
            }
            return reStr;
        }

        /// <summary>   
        /// 获得双字节字符串的字节数   
        /// </summary>   
        /// <param name="str">要检测的字符串</param>   
        /// <returns>返回字节数</returns>   
        public static int GetStrLength(string str)
        {
            ASCIIEncoding n = new ASCIIEncoding();
            byte[] b = n.GetBytes(str);
            int l = 0;  // l 为字符串之实际长度   
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] == 63)  //判断是否为汉字或全脚符号   
                {
                    l++;
                }
                l++;
            }
            return l;
        }

        /// <summary>   
        /// 返回一个 bool 值，指明提供的值是不是整数   
        /// </summary>   
        /// <param name="obj">要判断的值</param>   
        /// <returns>true[是整数]false[不是整数]</returns>   
        /// <remarks>   
        ///  isNumber　只能判断正(负)整数，如果 obj 为小数则返回 false;   
        /// </remarks>   
        /// <example>   
        /// 下面的示例演示了判断 obj 是不是整数：<br />   
        /// <code>   
        ///  bool flag;   
        ///  flag = isNumber("200");   
        /// </code>   
        /// </example>   
        public static bool isNumber(object obj)
        {
            //为指定的正则表达式初始化并编译 Regex 类的实例   
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^-?(\d*)$");
            //在指定的输入字符串中搜索 Regex 构造函数中指定的正则表达式匹配项   
            System.Text.RegularExpressions.Match mc = rg.Match(obj.ToString());
            //指示匹配是否成功   
            return (mc.Success);
        }

        /// <summary>
        /// 是否为0-9的数字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isDigitl(object obj)
        {
            //为指定的正则表达式初始化并编译 Regex 类的实例   
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^(\d*)$");
            //在指定的输入字符串中搜索 Regex 构造函数中指定的正则表达式匹配项   
            System.Text.RegularExpressions.Match mc = rg.Match(obj.ToString());
            //指示匹配是否成功   
            return (mc.Success);
        }

        /// <summary>   
        /// 移除字符串首尾某些字符   
        /// </summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="startStr">要在字符串首部移除的字符串</param>   
        /// <param name="endStr">要在字符串尾部移除的字符串</param>   
        /// <returns>string</returns>   
        public static string RemoveStartOrEndStr(string strOriginal, string startStr, string endStr)
        {
            char[] start = startStr.ToCharArray();
            char[] end = endStr.ToCharArray();
            return strOriginal.TrimStart(start).TrimEnd(end);
        }
        /// <summary>   
        /// 删除指定位置指定长度字符串   
        /// </summary>   
        /// <param name="strOriginal">要操作的字符串</param>   
        /// <param name="startIndex">开始删除字符的位置</param>   
        /// <param name="count">要删除的字符数</param>   
        /// <returns>string</returns>   
        public static string RemoveStr(string strOriginal, int startIndex, int count)
        {
            return strOriginal.Remove(startIndex, count);
        }



        #region GUID
        /// <summary>
        /// 获取新的Guid
        /// </summary>
        /// <returns></returns>
        public static string GetGuidString()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "").ToLower();
        }

        /// <summary>
        /// 获取Guid并转换为长整型
        /// </summary>
        /// <returns></returns>
        public static long GetGuidToLong()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 获取guid 短连接
        /// </summary>
        /// <returns></returns>
        public static string GetGuidToShort()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        #endregion

        #region datetime
        /// <summary>
        /// 返回相差指定秒数的时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="seconds">毫秒数</param>
        /// <returns></returns>
        public static DateTime GetDateTimeByMilliSeconds(DateTime startTime, string seconds)
        {
            long tick = 0;
            DateTime backTime = DateTime.Now;
            if (long.TryParse(seconds, out tick))
            {
                backTime = startTime.AddMilliseconds(tick);  // new DateTime(startTime.Ticks + tick);
            }
            return backTime;
        }

        /// <summary>
        /// 返回相差指定秒数的时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="seconds">毫秒数</param>
        /// <returns></returns>
        public static DateTime GetDateTimeBySeconds(DateTime startTime, string seconds)
        {
            long tick = 0;
            DateTime backTime = DateTime.Now;
            if (long.TryParse(seconds, out tick))
            {
                backTime = startTime.AddSeconds(tick);  // new DateTime(startTime.Ticks + tick);
            }
            return backTime;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="seconds">毫秒数</param>
        /// <returns></returns>
        public static long GetDateTimeSeconds(DateTime date)
        {
            return (long)(date - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        #endregion

        #region for mysql utf-8 mb4
        public static byte[] ParseHexString(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            int shift = 4;
            int offset = 0;
            foreach (char c in hex)
            {
                int b = (c - '0') % 32;
                if (b > 9) b -= 7;
                bytes[offset] |= (byte)(b << shift);
                shift ^= 4;
                if (shift != 0) offset++;
            }
            return bytes;
        }

        public static byte[] DecodeHexString(string str)
        {
            uint num = (uint)(str.Length / 2);
            byte[] buffer = new byte[num];
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                buffer[i] = (byte)((HexToByte(str[num2]) << 4) | HexToByte(str[num2 + 1]));
                num2 += 2;
            }
            return buffer;
        }

        private static byte HexToByte(char val)
        {
            if ((val <= '9') && (val >= '0'))
            {
                return (byte)(val - '0');
            }
            if ((val >= 'a') && (val <= 'f'))
            {
                return (byte)((val - 'a') + 10);
            }
            if ((val >= 'A') && (val <= 'F'))
            {
                return (byte)((val - 'A') + 10);
            }
            return 0xff;
        }

        public static string DecodeHex(string input)
        {
            if (input.Length % 2 == 1)
                throw new ArgumentException("Invalid hex encoded string.");

            int len = input.Length / 2;
            StringBuilder output = new StringBuilder(len);
            for (int c = 0; c < len; ++c)
                output.Append((char)System.Convert.ToByte(input.Substring(c * 2, 2), 16));

            return output.ToString();
        }


        #endregion


        #region  生成短链后缀
        /// <summary>
        ///  自动生成短串
        /// </summary>
        /// <returns></returns>
        public static string GetShortUrlName()
        {
            long guidNum = GetGuidToLong();
            return GetShortUrlName(guidNum);
        }


        /// <summary>
        /// 根据数字生成短串
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetShortUrlName(long num)
        {
            char[] chars = new char[] { 
              '0' , '1' , '2' , '3' , '4' , '5' , '6' , '7' , '8' , '9' , 
              'A' , 'B' , 'C' , 'D' , 'E' , 'F' , 'G' , 'H' ,'I' , 'J' , 
              'K' , 'L' , 'M' , 'N' , 'O' , 'P' , 'Q' , 'R' , 'S' , 'T' ,
              'U' , 'V' , 'W' , 'X' , 'Y' , 'Z'
            };
            StringBuilder strUrlName = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                long index = 0x0000001F & num;
                strUrlName.Append(chars[index]);
                num = num >> 5;
            }
            return strUrlName.ToString();
        }
        #endregion


        #region  加密相关处理
        /// <summary>
        /// 生成指定长度的字符串
        /// </summary>
        /// <param name="count"></param>
        /// <param name="type">10 - count个纯数字，    16 - count 16进制符   32 - 数字加26字母 </param>
        /// <returns></returns>
        public static string GetRandomString(int count,int type)
        {
            char[] chars = new char[] {  
              '0' , '1' , '2' , '3' , '4' , '5' , '6' , '7' , '8' , '9' , 
              'A' , 'B' , 'C' , 'D' , 'E' , 'F' , 'G' , 'H' ,'I' , 'J' , 
              'K' , 'L' , 'M' , 'N' , 'O' , 'P' , 'Q' , 'R' , 'S' , 'T' ,
              'U' , 'V' , 'W' , 'X' , 'Y' , 'Z' };
            StringBuilder strBuilder = new StringBuilder();
            Random ran = new Random();
            for (int i = 0; i < count; i++)
            {
                int c = ran.Next(type);
                strBuilder.Append(chars[c]);
            }
            return strBuilder.ToString();
        }
        #endregion


        #region  地图坐标转换
        private const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;  
        /// <summary>
        /// 中国正常坐标系GCJ02协议的坐标，转到 百度地图对应的 BD09 协议坐标
        /// </summary>
        /// <param name="lat">维度</param>
        /// <param name="lng">经度</param>
        public static void Convert_GCJ02_To_BD09(ref double lat,ref double lng)
        {
            double x = lng, y = lat;
            double z =Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            lng = z * Math.Cos(theta) + 0.0065;
            lat = z * Math.Sin(theta) + 0.006; 
        }
        /// <summary>
        /// 百度地图对应的 BD09 协议坐标，转到 中国正常坐标系GCJ02协议的坐标
        /// </summary>
        /// <param name="lat">维度</param>
        /// <param name="lng">经度</param>
        public static void Convert_BD09_To_GCJ02(ref double lat, ref double lng)
        {
            double x = lng - 0.0065, y = lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            lng = z * Math.Cos(theta);
            lat = z * Math.Sin(theta);
        }  
        #endregion
    }
}
