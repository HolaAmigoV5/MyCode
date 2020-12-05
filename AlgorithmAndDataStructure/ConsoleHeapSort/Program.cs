using System;
using System.Net.Http.Headers;

namespace ConsoleHeapSort
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] arr = { 12, 11, 13, 5, 6, 7 };

            HeapSort ob = new HeapSort();
            ob.Sort(arr);

            Console.WriteLine("Sorted array is");
            HeapSort.PrintArray(arr);
        }
    }

    public class HeapSort
    {
        public void Sort(int[] arr)
        {
            int n = arr.Length;

            //Build heap(rearrange array)
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(arr, n, i);

            //One by one extract an element from heap
            for (int i = n - 1; i > 0; i--)
            {
                //Move current root to end
                int temp = arr[0];
                arr[0] = arr[i];
                arr[i] = temp;

                //call max heapify on the reduced heap
                Heapify(arr, i, 0);
            }
        }

        //To heapify a subtree rooted with node i which is an 
        // index in arr[]. n is size of heap
        private void Heapify(int[] arr, int n, int i)
        {
            int largest = i;  //Initialize largest as root
            int left = 2 * i + 1;  //left=2*i+1;
            int right = 2 * i + 2;  //right=2*i+2;

            //If left child is larger than root
            if (left < n && arr[left] > arr[largest])
                largest = left;

            //If right child is larger than largest so far
            if (right < n && arr[right] > arr[largest])
                largest = right;

            //If largest is not root
            if (largest != i)
            {
                int swap = arr[i];
                arr[i] = arr[largest];
                arr[largest] = swap;

                //Recursively heapify the affected sub-tree
                Heapify(arr, n, largest);
            }
        }

        public static void PrintArray(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n; i++)
                Console.WriteLine(arr[i] + " ");
            Console.Read();
        }
    }

    /// <summary>
    /// 大根堆。小根堆实现方法相同
    /// </summary>
    public class MaxHeap
    {
        int[] heap = new int[1000];
        int heapSize = 0;

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="x"></param>
        public void Push(int x)
        {
            //每次将元素放到最后，再进行调整。从下到上，比较父元素
            heap[++heapSize] = x;
            int now = heapSize, nxt = 0;
            while (now>1)
            {
                nxt = now / 2;
                if (heap[now] <= heap[nxt]) break;
                Swap(ref heap[now], ref heap[nxt]);
                now = nxt;
            }
        }

        public int Pop()
        {
            int res = heap[1];
            int now = 1, nxt = 0;
            while (now*2<=heapSize)
            {
                nxt = now * 2;
                if (heap[nxt + 1] > heap[nxt] && nxt + 1 <= heapSize) nxt++;
                if (heap[nxt] <= heap[now])
                    return res;
                Swap(ref heap[nxt], ref heap[now]);
                now = nxt;
            }
            return res;
        }

        public void Clear()
        {
            heap = new int[1000];
            heapSize = 0;
        }

        private void Swap(ref int a, ref int b)
        {
            a ^= b;
            b ^= a;
            a ^= b;
        }
    }
}
