## 第七周 总结

### 字典树

 ​	 字典树， 即Trie树，又称单词查找树或键树，是一种树形结构。典型应用是用于统计和排序大量的字符串（但不限于字符串），所以经常被搜索引擎系统应用于文本词频统计。它的优点是最大限度地减少无谓的字符串比较，查询效率比哈希表高。字典树的核心思想是空间换时间。利用字符串的公共前缀来降低查询时间的开销以达到提高效率的目的。

基本性质：

1. 结点本身不存完整单词
2. 从根结点到某一个结点，路径上经过的字符串连接起来，为该结点对应的字符串。
3. 每个结点的所有子结点路径代表的字符都不相同。

### 并查集

 ​	 并查集（Disjoint Set），用于处理组团和配对问题。

基本操作

1. makeSet(s):建立一个新的并查集，其中包含s个单元集合
2. unionSet(x,y)：把元素x和元素y所在集合合并，要求x和y所在的集合不相交，如果相交则不合并。
3. find(x)：找到元素x所在集合的代表，该操作也可以用于判断两个元素是否位于同一个集合，只要将它们各自代表比较一下就可以了。

### 高级搜索

#### 剪枝

 ​	去掉重复的分支，通过缓存或者预先判断的方式实现。

#### 双端BFS

 ​	BFS的扩展，从两端开始搜索，中间相遇。

代码模板：

```c#
 public int MinMutation(string start, string end, string[] bank) {
     var bankSet=new HashSet<string>(bank);
     if(start==null || end ==null || !bankSet.Contains(end))
         return -1;

     // two-ended BFS
     var beginVisited=new HashSet<string>();
     var endVisited=new HashSet<string>();
     var visited=new HashSet<string>();
     int count=0;

     beginVisited.Add(start);
     endVisited.Add(end);
     visited.Add(start);
     visited.Add(end);

     while(beginVisited.Any()){
         count++;
         //choose the smaller to visit
         if(beginVisited.Count>endVisited.Count){
             //change
             var temp=endVisited;
             endVisited=beginVisited;
             beginVisited=temp;
         }

         var nextVisited=new HashSet<string>();
         foreach(string gen in beginVisited){
             var nextGens=GetNeighbors(gen, bankSet);
             foreach(var nGen in nextGens){
                 if(endVisited.Contains(nGen))
                     return count;
                 if(!visited.Contains(nGen)){
                     nextVisited.Add(nGen);
                 }
             }
             beginVisited= nextVisited;
             visited.UnionWith(nextVisited);
         }
     }
     return -1;
  }
```



#### 启发式搜索

 ​	 启发式搜索（Heuristic Search），又称A*算法。启发式函数，又称估价函数，h(n)用来评价哪些结点最有希望的是一个我们要找的节点，h(n)会返回一个非负实数，也可以认为是从结点n的目标结点路径的估计成本。启发式函数是一种告知搜索方向的方法。它提供了一种明智的方法来猜测哪个邻居结点会导向下一个目标。

### AVL树和红黑树

#### AVL树

1. AVL树，发明者G.M.Adelson-Velsky和Evgenii Landis。平衡二叉搜索树

2. 平衡因子(Balance Factor)， 左子树的高度减去右子树的高度（有时想反），balance factor={-1,0,1}
3. 通过旋转操作来进行平衡（左旋，右旋，左右旋，右左旋）
4. 不足：结点需要存储额外信息，且调整次数频繁。

#### 红黑树

 ​	红黑树（Red-black Tree）,是一种**近似平衡的二叉搜索树(Binary Search Tree)**，它能够确保任何一个结点的左右子树的**高度差小于两倍**。具体来说，红黑树是满足如下条件的二叉搜索树：

1. 每个结点要么是红色，要么是黑色

2. 根结点是黑色
3. 每个叶结点(空节点)是黑色
4. 不能有相邻接的两个红色节点
5. 从任一结点到其每个叶子的所有路径都包含相同数目的黑色节点

#### 对比(重点)

1. AVL trees provide **faster lookups** than Read Black Trees because they are **more strictly balanced**.
    - AVL树查询更快，因为是严格平衡树
2. Red Black Trees provide **faster insertion and removal**  operations than AVL trees as fewer rotations are done due to relatively relaxed balancing.
    - 红黑树插入和删除更快，因为不用频繁进行旋转操作
3. AVL trees store **balance factors or heights** with each node, thus requires storage for an integer per node whereas Red Black Tree requires only 1 bit of information per node.
    - AVL树每个节点会存储平衡因子或高度，因而每个节点需要存储一个整数， 而红黑树每个节点只用1位。
4. Red Black Trees are used in most of the **language libraries** like map, multimap, multiset in C++ whereas AVL trees are used in **databases** where faster retrievals are required.
    - 红黑树在大多数语言库（例如C ++中的map，multimap，multiset）中使用，而AVL树用在需要更快检索的databases 中使用。
