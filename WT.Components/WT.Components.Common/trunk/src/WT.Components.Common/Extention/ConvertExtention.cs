using System;

namespace WT.Components.Common.Extention
{

    public static class ConvertExtention
    {
        /// <summary>
        /// object转换Int扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt32(this object obj)
        {
            return obj == null ? 0 : obj.ToString().ToInt32();
        }

        /// <summary>
        /// string转Int扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt32(this string obj)
        {
            int intReturn;
            return int.TryParse(obj, out intReturn) ? intReturn : 0;
        }
        /// <summary>
        /// object转换Int64扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ToInt64(this object obj)
        {
            return obj == null ? 0 : obj.ToString().ToInt64();
        }
        /// <summary>
        /// string转Int64扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ToInt64(this string obj)
        {
            long intReturn;
            return long.TryParse(obj, out intReturn) ? intReturn : 0;
        }


        /// <summary>
        /// object转换Int64扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Double ToDouble(this object obj)
        {
            return obj == null ? 0 : obj.ToString().ToDouble();
        }
        /// <summary>
        /// string转Int64扩展方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Double ToDouble(this string obj)
        {
            Double doubleReturn;
            return Double.TryParse(obj, out doubleReturn) ? doubleReturn : 0;
        }
    }
}
