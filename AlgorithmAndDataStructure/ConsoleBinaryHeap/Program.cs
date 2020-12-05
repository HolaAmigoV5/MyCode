using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBinaryHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryHeap maxHeap = new BinaryHeap(10);
            maxHeap.Insert(10);
            maxHeap.Insert(4);
            maxHeap.Insert(9);
            maxHeap.Insert(1);
            maxHeap.Insert(7);
            maxHeap.Insert(5);
            maxHeap.Insert(3);

            maxHeap.PrintHeap();
            maxHeap.Delete(5);
            maxHeap.PrintHeap();
            maxHeap.Delete(2);
            maxHeap.PrintHeap();


            Console.WriteLine();
        }
    }

    public class BinaryHeap
    {
         static int d = 2;  //d叉树
         int[] heap;
         int heapSize;

        public BinaryHeap(int capacity)
        {
            heapSize = 0;
            heap = new int[capacity + 1];
        }

        public bool isEmpty()
        {
            return heapSize == 0;
        }

        public bool isFull()
        {
            return heapSize == heap.Length;
        }

        private int Parent(int i)
        {
            return (i - 1) / d;
        }

        private int kthChild(int i, int k)
        {
            return d * i + k;
        }

        /// <summary>
        /// Inserts new element into heap.
        /// Complexity:O(logN)
        /// As worst case scenario, we need to traverse till the root.
        /// </summary>
        /// <param name="x"></param>
        public void Insert(int x)
        {
            if (isFull())
                throw new Exception("Heap is full, No space to insert new element.");

            //尾部插入，堆大小增加，调整
            heap[heapSize] = x;
            heapSize++;
            HeapifyUp(heapSize - 1);
        }

        /// <summary>
        /// Deletes element at index x
        /// Complexity:O(logN)
        /// </summary>
        /// <param name="x">index</param>
        /// <returns></returns>
        public int Delete(int x)
        {
            if (isEmpty())
                throw new Exception("Heap is empty, No element to delete!");

            int maxElement = heap[x];
            heap[x] = heap[heapSize - 1];
            heapSize--;
            HeapifyDown(x);
            return maxElement;
        }

        public int FindMax()
        {
            if (isEmpty())
                throw new Exception("Heap is empty.");
            return heap[0];
        }

        public void PrintHeap()
        {
            Console.WriteLine("nHeap = ");
            for (int i = 0; i < heapSize; i++)
            {
                Console.Write(heap[i] + " ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Maintains the heap property while deleting an element.
        /// </summary>
        /// <param name="x"></param>
        private void HeapifyDown(int x)
        {
            int temp = heap[x];
            while (kthChild(x, 1) < heapSize)
            {
                int child = MaxChild(x);
                if (temp >= heap[child])
                    break;
                heap[x] = heap[child];
                x = child;
            }
            heap[x] = temp;
        }

        private int MaxChild(int x)
        {
            int leftChild = kthChild(x, 1);
            int rightChild = kthChild(x, 2);

            return heap[leftChild] > heap[rightChild] ? leftChild : rightChild;
        }

        /// <summary>
        /// Maintains the heap property while inserting an element.
        /// </summary>
        /// <param name="v">the index of heap tail</param>
        private void HeapifyUp(int v)
        {
            int insertValue = heap[v];
            while (v > 0 && insertValue > heap[Parent(v)])
            {
                heap[v] = heap[Parent(v)];
                v = Parent(v);
            }
            heap[v] = insertValue;
        }
    }

    public class BinaryHeapExample
    {
        int[] arr;
        int sizeOfTree;
        public BinaryHeapExample(int size)
        {
            arr = new int[size + 1];
            this.sizeOfTree = 0;
            Console.WriteLine("Empty heap has been created Successfully.");
        }

        //get the first item of Heap.
        public int PeekOfHeap()
        {
            if (sizeOfTree == 0)
                return 0;
            else
                return arr[1];
        }

        public int SizeOfHeap()
        {
            return sizeOfTree;
        }

        public void InsertElementInHeap(int value)
        {
            if (sizeOfTree <=0)
                Console.WriteLine("Tree is empty");
            else
            {
                arr[++sizeOfTree] = value; //insert to the end of Heap.
                HeapfyBottomToTop(sizeOfTree);
                Console.WriteLine("Inserted" + value + " successfully in Heap!");
            }
        }

        public int ExtractHeapOfHeap()
        {
            if (sizeOfTree == 0)
            {
                Console.WriteLine("Heap is empty!");
                return -1;
            }
            else
            {
                Console.WriteLine("Head of Heap is: " + arr[1]);
                Console.WriteLine("Extracting it now ...");
                arr[1] = arr[sizeOfTree];  //Replacing with the last one of the array.
                sizeOfTree--;
                HeapfyTopToButtom(1);
                return arr[1];
            }
        }

        private void HeapfyTopToButtom(int index)
        {
            int left = index * 2;
            int right = index * 2 + 1;
            int smallestChild;
            if (sizeOfTree < left)
                return;
            else if(sizeOfTree==left)  // only left child
            {
                if (arr[index] > arr[left])
                {
                    int tem = arr[index];
                    arr[index] = arr[left];
                    arr[left] = tem;
                }
                return;
            }
            else  //both left and right children
            {
                smallestChild = arr[left] < arr[right] ? left : right;
                if (arr[index] > arr[smallestChild])
                {
                    int tem = arr[index];
                    arr[index] = arr[smallestChild];
                    arr[smallestChild] = tem;
                }
            }
            HeapfyTopToButtom(smallestChild);
        }

        private void HeapfyBottomToTop(int index)
        {
            if (index <= 1) //root of the tree. Hence no more Heapifying.
                return;
            int parent = index / 2;
            if (arr[index] < arr[parent])
            {
                int tem = arr[index];
                arr[index] = arr[parent];
                arr[parent] = tem;
            }
            HeapfyBottomToTop(parent);
        }
    }


}
