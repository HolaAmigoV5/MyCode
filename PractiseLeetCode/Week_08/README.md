## 第八周 总结

### 位运算

**位运算符**

1. 左移(<<)：位向左移动，前面出去，后面补0
  2. 右移(>>)：位向右移动，前面补0
  3. 按位或(|)：有1或1 
  4. 按位与(&)：有0与0
  5. 按位取反(~)：1变成0，0变成1
  6. 按位异或( ^)：相同为0，不同为1。也可以理解为“不进位加法”
       - x^0=x
       - x^1s=~x    //1s为全部为1， 1s=~0
       - x^(~x)=1s

       - x^x=0

       - a=10, b=20, 执行交换：a=a^b, b=a^b, a=a^b;

       - a^c=a^(b^c)=(a^b)^c

**指定位置的位运算**

1. 将x最右边的n位清零：x&(~0<<n)
2. 获取x的第n位值(0或1)：(x>>n)&1
3. 获取x的第n位幂值：x&(1<<n)
4. 仅将第n位置为1：x|(1<<n)
5. 仅将第n位置为0：x&(~(1<<n))
6. 将x最高位至第n位(含)清零：x$((1<<n)-1)

**实战位运算要点**

- 判断奇偶：x%2\==1 —— (x&1)\==1， x%2\==0 —— （x&1）\==0
- x>>1 —— x/2 ：即x=x/2; —— x=x>>1
- x=x&(x-1) 清零最低位的1
- x&-x 得到最低位的1。**0表示‘+’号(正数)，1表示'-'号(负数)。负数以原码的补码形式表示**
- x&~x  = 0
- a=10, b=20，交换a, b：执行如下：
  1. a=a^b：此时a=30
  2. b=a^b：此时b=10
  3. a=a^b：此时a=20

**基础补充**

计算机机器数真正的值称为真值。因为机器数最高位是符号位，计算真值要区分。如`10000101`不表示133， 表示-5。

原码=符号位+真值

反码：正数的反码与原码相同； 负数的反码是对其原码取反，但符号位除外

补码：正数的补码与原码相同，负数的补码等于其反码的末位加1

- 补码运算时，其符号位与数值部分一起参加运算

- 补码的符号位相加后，如果有进位出现，要把这个进位舍去

### 布隆过滤器

​	布隆过滤器(Bloom Filter)，是一个占用空间很小，效率很高的随机数据结构。由一个很长的二进制向量(bit数组)和一系列随机映射函数(Hash算法)构成。布隆过滤器可以用于检索一个元素是否在一个集合中。优点是**空间效率和查询时间效率都远远超过一般的算法** ， 缺点是**有一定的误识别率和删除困难**。 

布隆过滤器添加元素：

- 将待添加元素给k个哈希函数进行哈希运算

- 得到对应位数组上的k个位置 

- 将这k个位置设1

布隆过滤器查询元素：

- 将要查询的元素给k个哈希函数 进行哈希运算
- 得到对应位上的k个位置 
- 如果k个位置有一个为0，则肯定不在集合中 
- 如果k个位置全部为1，则可能在集合中。

### LRU缓存

​	LRU缓存(Least recently used cache)，最少最近被使用的元素会被淘汰。大小和替换策略，使用Hash Table+Double LinkedList实现，能在O(1)的时间复杂度实现查询，修改和更新。

### 排序算法

比较类排序：通过比较来决定元素的相对次序，由于其时间复杂度不能突破O(nlogn)，因此也称为非线性时间比较类排序。

非比较类排序：不通过比较来决定元素间的相对次序，可以突破基于比较排序的时间下界，以线性时间运行，因此也称线性时间比较类排序。

**初级排序——O(n^2)**

1. 选择排序(Selection Sort)，每次找最小值，然后放到待排序数组的起始位置。代码如下

   ```c#
   public int[] SelectionSort(int[] arr){
       int len=arr.Length;
       for(int i=0; i<len-1; i++){
           int minIndex=i;
           for(int j=i+1; j<len; j++){
               if(arr[j]<arr[minIndex])
                   minIndex=j;
           }
           int tmp=arr[i];
           arr[i]=arr[minIndex];
           arr[minIndex]=tmp;
       }
       return arr;
   }
   ```

   

2. 插入排序(Insertion Sort)，依次从未排序序列中抽取元素插入已排序序列合适位置。代码如下：

   ```c#
   public int[] InsertionSort(int[] arr){
       int len=arr.Length;
       for(int i=1; i<len; i++){
           int preIndex=i-1;
           int current=arr[i];
           while(preIndex>=0 && arr[preIndex]>current){
               arr[preIndex+1]=arr[preIndex];
               preIndex--;
           }
           arr[preIndex+1]=current;
       }
       return arr;
   }
   ```

   

3. 冒泡排序(Bubble Sort)， 嵌套循环，每次查看相邻的元素，如果逆序，则交换。代码如下：

   ```c#
   public int[] BubbleSort(int[] arr){
   	int len=arr.Length;
   	for(int i=0; i<len-1; i++){
           int isSort=true;
   		for(int j=0; j=len-i-1; j++){
   			if(arr[j]>arr[j+1]){
   				var tmp=arr[j+1];
   				arr[j+1]=arr[j];
   				arr[j]=tmp;
                   isSort=false;
   			}
   		}
           if(isSort)
               break;
   	}
   	return arr;
   }
   ```

   

**高级排序——O(N*logN)——重点，手写，原理**

1. 快速排序(Quick Sort)，数组取标杆pivot，将小元素放pivot左边，大元素放右边，然后依次对左边和右边的子数组继续快排，以达到整个序列有序。代码如下：

   ```c#
   public void QuickSort(int[] arr, int beginIndex, int endIndex){
       if(endIndex<=beginIndex) return;
       int pivotIndex=Partition(arr, beginIndex, endIndex);
       QuickSort(arr, beginIndex, pivotIndex-1);
       QuickSort(arr, pivotIndex+1, endIndex);
   }
   private int Partition(int[] arr, int begin, int end){
       int pivot=end, counter=begin;
       for(int i=begin; i< end; i++){
           if(arr[i]<arr[pivot]){
               Swap(arr, i, counter);
               counter++;
           }
       }
       Swap(arr, counter, pivot);
       return counter;
   }
   private void Swap(int[] arr, int i, in j){
       if(i==j) return;
       var tmp=arr[i];
       arr[i]=arr[j];
       arr[j]=tmp;
   }
   ```

   

2. 归并排序(Merge Sort)，①不断把长度为n的序列分成两个长度为n/2的子序列 ②对这个子序列分别采用归并排序 ③将两个排序好的子序列合并成一个最终的排序序列——先分后合——分治。代码如下：

   ```c#
   public void MergeSort(int[] arr, int left, int right){
       if(left>=right) return;
       int mid=(left+right)>>1;
       
       MergeSort(arr, left, mid);
       MergeSort(arr, mid+1, right);
       Merge(arr, left, mid, right);
   }
   private void Merge(int[] arr, int left, int mid, int right){
       int[] tmp=new int[right-left+1];
       int i=left, j=mid+1, k=0;
       
       while(i<=mid && j<=right)
           tmp[k++]=arr[i]<=arr[j]?arr[i++]:arr[j++];
      	while(i<=mid)
           tmp[k++]=arr[i++];
      	while(j<=right)
           tmp[k++]=arr[j++];
       
       Array.Copy(tmp,0,arr,left, tmp.Length);
   }
   ```

   

3. 堆排序(Heap Sort)，①数组元素建立小(大)顶堆，②依次取堆顶元素并删除。代码如下：

   ```C#
   public void HeapSort(int[] arr){
       int len=arr.Length;
       if(len==0) return;
       
       //build Heap
       //begin to adjust from the last root at the bottom.
       for(int i=len/2-1; i>=0; i--)
           Heapify(arr,i, len);
       
       for(int i=len-1; i>=0; i--){
           //Exchange the top element with the last one.
           Swap(arr,i,0);
           Heapify(arr,0,i);
       }
   }
   private void Heapify(int[] arr, int i, int len){
       int left=2*i+1;
       int right 2*i+2;
       int largest=i;
       
       if(left<len && arr[left]>arr[largest])
           largest=left;
       if(right<len && arr[right]>arr[largest])
           largest=right;
       //the largest one changed.
       if(largest!=i){ 
           Swap(arr, i, largest);
           Heapify(arr, largest, len);
       }
   }
   ```

   堆插入O(logn)， 取最小/大值 O(1)。

**特殊排序——O(n)**

1. 计数排序(Counting Sort)，计数排序要求输入的数据必须是有确定范围的整数。将输入的数据值转为键存储在额外开辟的数组空间中，然后依次把计数大于1的填充回数组。
2. 桶排序(Bucket Sort)，假设输入数据服从均匀分布，将数据分到有限数量的通里，每个桶再分别排序（有可能再使用别的排序算法或以递归方式继续使用桶排序进行排）。
3. 基数排序(Radix Sort)，基数排序是按照低位先排序，然后收集； 再按高位排序，然后再收集； 依次类推，直到最高位。有时候有些属性是有优先级顺序的，先按低优先级排序，再按高优先级排序。