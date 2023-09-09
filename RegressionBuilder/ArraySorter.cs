using System;
using System.Collections.Generic;

namespace RegressionBuilder
{
    class ArraySorter
    {
        /// <summary>
        /// Like matrix
        /// </summary>
        /// <returns></returns>
        public static T[][] ToArray<T>(List<List<T>> arrays)
        {
            var arr = new T[arrays[0].Count][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new T[arrays.Count];
                for (int j = 0; j < arr[i].Length; j++)
                {
                    arr[i][j] = arrays[j][i];
                }
            }

            return arr;
        }
        public static T[][] ToArray<T>(List<(T x, T y)> array)
        {
            var arr = new T[array.Count][];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new T[2];
                arr[i][0] = array[i].x;
                arr[i][1] = array[i].y;
            }

            return arr;
        }

        public static void SetFromArray<T>(List<List<T>> arrays,T[][] arr)
        {
            arrays.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    arrays.Add(new List<T>());
                    arrays[j].Add(arr[i][j]);
                }
            }
        }
        /// <summary>
        /// Sort non-transposed array (X and Y in the separated arrays)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays"></param>
        /// <param name="sortByColumn"></param>
        public static void SortBy<T>(List<List<T>> arrays,int sortByColumn)
        {
            var arr = ToArray(arrays);
            Array.Sort(arr, (x, y) => Comparer<T>.Default.Compare(x[sortByColumn], y[sortByColumn]));
            SetFromArray(arrays,arr);
        }

        /// <summary>
        /// fill from two-dimensional array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        public static void FillArray<T>(List<(T x, T y)> destination,T[][] source)
        {
            destination.Clear();
            for (int i = 0; i < source.Length; i++)
            {
                destination.Add((source[i][0], source[i][1]));
            }
        }

        public static void FillArray<T>(List<T> destination, T[] source)
        {
            destination.Clear();
            for (int i = 0; i < source.Length; i++)
            {
                destination.Add(source[i]);
            }
        }

        public static void SortBy<T>(List<(T x, T y)> array, int sortByColumn)
        {
            var arr = ToArray(array);
            Array.Sort(arr, (x, y) => Comparer<T>.Default.Compare(x[sortByColumn], y[sortByColumn]));
            FillArray(array, arr);
        }

    }
}
