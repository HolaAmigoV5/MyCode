using System;

namespace ConsoleSort
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 3, 44, 38, 5, 47, 15, 36, 26, 27, 2, 46, 4, 19, 50, 48 };
            int[] arr2 = { 5, 4, 3, 2, 1 };
            int[] arr3 = { 1, 2, 3, 4, 5 };
            int[] arr4 = { 3, 4, 5, 1, 2 };
            //var arr1=SelectionSort(arr);
            //var res0= BubbleSort(arr3);
            //var res = InsertionSort(arr2);
            QuickSort(arr4, 0, arr4.Length - 1);
            //MergeSort(arr2, 0, arr2.Length-1);
            //HeapSort(arr);

            Console.ReadLine();
        }

        private static int[] SelectionSort(int[] arr)
        {
            //O(n^2), O(1)
            var len = arr.Length;
            for (int i=0; i<len-1; i++)
            {
                int minIndex = i;
                for (int j=i+1; j<len; j++)
                {
                    if (arr[j] < arr[minIndex]) //寻找最小值
                        minIndex = j;//保存最小值索引
                }

                int temp = arr[i];
                arr[i] = arr[minIndex];
                arr[minIndex] = temp;
            }
            return arr;
        }

        private static int[] BubbleSort(int[] arr)
        {
            //O(n^2), O(1)
            var len = arr.Length;
            for (var i=0; i<len-1; i++)
            {
                bool isSort = true;
                for(var j=0; j<len-i-1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        var tmp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = tmp;
                        isSort = false;
                    }
                }
                if (isSort)
                    break;
            }
            return arr;
        }

        private static int[] InsertionSort(int[] arr)
        {
            var len = arr.Length;
            for (int i=1; i<len; i++)
            {
                int preIndex = i - 1;
                int current = arr[i];
                while (preIndex>=0 && arr[preIndex] > current)
                {
                    arr[preIndex + 1] = arr[preIndex];
                    preIndex--;
                }
                arr[preIndex + 1] = current;
            }
            return arr;
        }

        #region MergeSort
        private static void MergeSort(int[] arr, int left, int right)
        {
            if (left >= right) return;
            int mid = (left + right) >> 1;  //(left+right)/2

            MergeSort(arr, left, mid);
            MergeSort(arr, mid+1, right);
            Merge(arr, left, mid, right);
        }

        /// <summary>
        /// 归并
        /// </summary>
        /// <param name="arr">排序数组</param>
        /// <param name="left">归并起始位置</param>
        /// <param name="mid">归并中间位置</param>
        /// <param name="right">归并结束位置</param>
        private static void Merge(int[] arr, int left, int mid, int right)
        {
            int[] temp = new int[right - left + 1]; //the array of middle
            int i = left, j = mid + 1, k = 0;

            while (i <= mid && j <= right)
                temp[k++] = arr[i] <= arr[j] ? arr[i++] : arr[j++];

            while (i <= mid)
                temp[k++] = arr[i++];

            while (j <= right)
                temp[k++] = arr[j++];

            //复制要合并的数据
            Array.Copy(temp, 0, arr, left, temp.Length);
            //for (int p = 0; p < temp.Length; p++)
            //    arr[left + p] = temp[p];
        } 
        #endregion

        #region QuickSort
        private static void QuickSort(int[] arr, int startIndex, int endIndex)
        {
            if (endIndex <= startIndex) 
                return;
            int pivotIndex = Partition(arr, startIndex, endIndex);  //start partition
            QuickSort(arr, startIndex, pivotIndex - 1); //sort the left elements of pivotIndex
            QuickSort(arr, pivotIndex + 1, endIndex);   //Sort the right elements of pivotIndex
        }
        private static int Partition(int[] arr, int start, int end)
        {
            // pivot: the index of pivot，counter: count the num. of elements which's less than arr[pivot]
            int pivot = end, counter = start;
            for (int i = start; i < end; i++)
            {
                //将比基准值小的值交换到基准位置最左边
                if (arr[i] < arr[pivot])
                {
                    Swap(arr, i, counter);
                    counter++;
                }
            }
            //将基准值交换调整到最后一个小于基准值位置的后一位
            Swap(arr, pivot, counter);
            return counter;
        }

        private static void Swap(int[] arr, int i, int j)
        {
            if (i == j)
                return;
            var tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
        #endregion

        #region HeapSort
        private static void HeapSort(int[] arr)
        {
            int len = arr.Length;
            if (len == 0) return;

            //创建堆
            for (int i = len / 2 - 1; i >= 0; i--)  
                Heapify(arr, i, len);  //从最底层最后一个根节点开始调整

            for (int i = len - 1; i >= 0; i--)
            {
                Swap(arr, i, 0);    //将堆顶元素依次与无序区的最后一位交换
                Heapify(arr, 0, i);     //重新调整堆
            }
        }

        /// <summary>
        /// 调整堆
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="length">数组范围</param>
        /// <param name="i">调整位置</param>
        private static void Heapify(int[] arr, int i, int len)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int largest = i;

            if (left < len && arr[left] > arr[largest])  //与左子节点比较
                largest = left;

            if (right < len && arr[right] > arr[largest])  //与右子节点比较
                largest = right;

            if (largest != i)  //largest发生了变化(即：左右子节点有大于根节点的情况)
            {
                Swap(arr, i, largest);  //将左右节点中大者与根节点进行交换(即：实现局部大顶堆)
                Heapify(arr, largest, len);  //重新调整堆
            }
        } 
        #endregion
    }
}
