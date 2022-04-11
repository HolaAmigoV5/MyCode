## 第二周

### 哈希表、映射、集合

​	哈希表(Hash table)，也叫散列表，根据关键码值(key value)而直接进行访问的数据结构。它通过把关键码值映射到表中一个位置来访问记录，以加快查找的速度。这个映射函数叫做散列函数(Hash Function)，存放记录的数组叫做哈希表。**实质是通过哈希函数把值映射到一个位置(index)。查询，添加和删除都是O(1)。**  可能会出现哈希碰撞。

​	DotNet中，hashtable内部实现依靠一个叫**private struct bucket（成员有key, val, hash_coll）的数组(private bucket[] buckets)**维护。由于会出现装箱或拆箱等耗时操作，**微软建议使用Dictionary<Tkey, TValue>泛型**替代hashtable使用。Dictionary<Tkey, TValue>内部实现依靠一个叫**private struct Entry(成员有next, key, value)的数组实现 。提供Add, Clear, ContainsKey, ContainsValue等方法。通过拉链法解决哈希冲突** 哈希函数是**GetHashCode()** 。

**Dictionary<Tkey,TValue>实现原理分析：**

* Add操作：① 通过key计算hashCode；② 对hashCode取余运算，计算该hashCode落在哪个buckets桶中(例如buckets[2])；③ 将hashCode，key，value等信息存入entries[index]中；④ 将entries[index]中的index赋值为对应下标的bucket，如buckets[2]=index；⑤ 最后version++。**只有增加，替换和删除才会version++，version用于保证遍历Dictionary时，又改变Dictionary时能及时抛出异常**
* Resize操作(扩容)
  * 扩容的两种情况：① buckets，entries两个数组满了会扩容； ② Dictionary发生碰撞次数太多，会严重影响性能，也会触发扩容。HashCollisionThreshold=100。
  * 扩容步骤：① buckets, entries翻倍 ② 元素拷贝到新entries ③ Hash碰撞扩容时，用新哈希函数重写计算Hash值 ④对entries每个元素确定新buckets位置 ⑤ 重建hash链。

### 树、二叉搜索树

​	链表(Linked List)是特殊化的树(Tree)，树(Tree)是特殊化的图(Graph)。链表生枝即是树，树中套环变成图。二叉树的遍历：① 前序(pre-order)：根-左-右； ②中序(In-order)：左-根-右；③ 后序(Post-order)：左-右-根

**二叉搜索树** 

​	二叉搜索树，也称二叉排序树、有序二叉树(Ordered Binary Tree)、排序二叉树(Sorted Binary Tree)，查询，插入，删除都是O(logn)，指一棵空树或者具有下列性质的二叉树：

1. 左子树上**所有节点** 的值均小于它的根节点的值；
  2. 右子树上**所有节点** 的值均大于它的根节点的值；
  3. 以此类推：左，右子树也分别为二叉搜索树（这就是重复性！）

中序遍历：升序排序。

### 堆、二叉堆

​	堆(Heap)：可以迅速找到一堆数中最大或最小的数据结构。将根节点最大的堆叫做大顶堆或大根堆，根节点最小的堆叫小顶堆或小根堆。常见的堆有二叉堆、斐波那契堆，严格斐波那契堆(性能最好)等。

假设是大顶堆，常见操作(API)：

1. find-max: O(1)
2. delete-max: O(logN)
3. insert(create): O(logN) or O(1)

二叉堆性质：通过完全二叉树来实现（注意：不是二叉搜索树）；二叉堆(大顶)它满足的性质(① 是一棵完全树，② 树中任意节点的值总是>=其子节点的值)。**二叉堆实现相对容易，时间复杂度刚刚及格** 

二叉堆实现细节

1. 二叉堆一般通过“数组”来实现
2. 根节点是a[0]；索引为i的左孩子索引是(2\*i+1)，索引为i的右孩子索引是(2\*i+2)，索引为i的父节点索引floor((i-1)2)。

Insert 插入操作—O(logN)

1. 新元素一律先插入到堆的尾部
2. 依次向上调整整个堆的结构(一直到根即可)——HeapifyUp

Delete Max 删除堆顶操作——O(logN)

1. 将堆尾元素替换到顶部（即堆顶被替代删除掉）
2. 依次从根部向下调整整个堆的结构(一直到堆尾即可)——heapifyDown

### 图的实现和特性

​	图的属性：Graph(V,E)

* V-vertex 点
  1. 度-入度和出度
  2. 点与点之间：连通与否
* E-edge 边
  1. 有向和无向(单行线)
  2. 权重(边长)

### 课后刷题

| 题号                                                         | 名称                                                         | 难度   | 分类               | 解法                           |
| ------------------------------------------------------------ | ------------------------------------------------------------ | ------ | ------------------ | ------------------------------ |
| [242](https://leetcode.com/problems/valid-anagram/discuss/?currentPage=1&orderBy=most_votes&query=) | [有效的字母异位词](https://leetcode-cn.com/problems/valid-anagram/) | 🟢 简单 | 哈希表、映射、集合 | ① Sort ②Dic/int[]哈希          |
| [49](https://leetcode.com/problems/group-anagrams/discuss/?currentPage=1&orderBy=most_votes&query=) | [字母异位词分组](https://leetcode-cn.com/problems/group-anagrams/) | 🟡 中等 | 哈希表、映射、集合 | ① 暴力 ②Dic哈希                |
| [1](https://leetcode.com/problems/two-sum/discuss/?currentPage=1&orderBy=most_votes&query=) | [两数之和](https://leetcode-cn.com/problems/two-sum/)        | 🟢 简单 | 哈希表、映射、集合 | ①暴力 ②Dic哈希                 |
| [144](https://leetcode-cn.com/problems/binary-tree-preorder-traversal/) | [二叉树的前序遍历](https://leetcode-cn.com/problems/binary-tree-preorder-traversal/) | 🟡 中等 | 二叉树             | ①递归 ②迭代(模板)              |
| [94](https://leetcode-cn.com/problems/binary-tree-inorder-traversal/) | [二叉树的中序遍历](https://leetcode-cn.com/problems/binary-tree-inorder-traversal/) | 🟡 中等 | 二叉树             | ①递归 ②迭代(模板)              |
| [145](https://leetcode-cn.com/problems/binary-tree-postorder-traversal/) | [二叉树的中序遍历](https://leetcode-cn.com/problems/binary-tree-postorder-traversal/) | 🟡 中等 | 二叉树             | ①递归 ②迭代(模板)              |
| [589](https://leetcode.com/problems/n-ary-tree-preorder-traversal/discuss/?currentPage=1&orderBy=most_votes&query=) | [N叉树的前序遍历](https://leetcode-cn.com/problems/n-ary-tree-preorder-traversal/) | 🟢 简单 | N叉树              | ①递归 ②迭代(模板)              |
| [429](https://leetcode.com/problems/n-ary-tree-level-order-traversal/discuss/?currentPage=1&orderBy=most_votes&query=) | [N叉树的层序遍历](https://leetcode-cn.com/problems/n-ary-tree-level-order-traversal/) | 🟡 中等 | N叉树              | ①递归 ②迭代(Queue) ③ Layer遍历 |
| [264](https://leetcode.com/problems/ugly-number-ii/discuss/?currentPage=1&orderBy=most_votes&query=) | [丑数](https://leetcode-cn.com/problems/ugly-number-ii/)     | 🟡 中等 | 动态规划，小顶堆   | ⭐️ 懵逼                         |
| [347](https://leetcode.com/problems/top-k-frequent-elements/discuss/?currentPage=1&orderBy=most_votes&query=) | [前 K 个高频元素](https://leetcode-cn.com/problems/top-k-frequent-elements/) | 🟡 中等 | 小顶堆             | ① 桶排序 ②Linq                 |
|                                                              |                                                              |        |                    |                                |

