# 第一周 学习笔记

## 知识点

1. **精通一个领域的方法：**  ① 知识点切碎 ② 碎片训练 ③ 反馈修正
2. **刷题技巧：** 五遍刷题法
   * 第一遍：5-10分钟读题+思考，看最优解法，背诵和默写解法
   * 第二遍：自己写代码，LeetCode提交通过
   * 第三遍：24小时候，再写一遍
   * 第四遍：一周后再练习相同的题目
   * 第五遍：面前前一个星期，恢复练习
3. **数据结构：**
   * 一维：array, linked list, stack, queue, deque, set, map
   * 二维：tree, graph, binary search tree, heap, disjoint set, Trie
   * 特殊：Bitwise, BloomFilter, LRU Cache
4. **算法：** 跳转(branch)，循环(iteration)，递归(Recursion)，搜索(Search)，动态规划(Dynamic Programing)，二分查找(Binary Search)，贪心(Greedy)，数学(Math)
5. **编程习惯：** 自顶向下编程，快捷键的刻意训练
6. **时间复杂度：** O(1), O(log n), O(n), O(n²), O(n³), O(2^N), O(n!)
   * 二分查找(Binary Search)：O(log n)
   * 二叉树遍历(Binary tree traversal)：O(n)，每个节点有且访问一次，故时间复杂度为O(n)
   * 最优排序矩阵查找(Optimal sorted matrix seach)：O(n)
   * 归并排序(Merge sort)：O(nlog n)
7. **空间复杂度：** ① 数组的长度 ② 递归的深度
8. **数组、链表、跳表、栈、队列、双端队列、优先队列**
   1. 数组(Array)：一段连续的地址空间，通过索引访问。prepend→O(1), apend(end)→O(1), lookup→O(1), insert→O(n), delete→O(n)。insert和delete时都涉及到移位，故O(n)。prepend时，头部预留空间处理，故O(1)
   2. 链表(LinkedList)：一个Value和一个指针。添加和删除不需要移位，故O(1)；查询时需要遍历，故O(n)。prepend→O(1), apend(end)→O(1), lookup→O(n), insert→O(1), delete→O(1)
   3. 跳表(skip List)：1989年代出现，对标平衡树(AVL Tree)和二分查找，是一种插入/删除/搜索都是**O(log n)** 的数据结构。热门项目如Redis、LevelDB等用跳表替代平衡树。**注意：只能用于元素有序的情况。** 现实中跳表增加和删除时，因为要维护索引，所以成本比较高。空间复杂度O(n)。工程上应用Redis。
   4. 栈(Stack)：先入后出容器结构；添加删除皆为O(1)，查询O(n)，因为无序，查询需要遍历。
   5. 队列(Queue)：先进先出容器结构；添加删除皆为O(1)，查询O(n)，因为无序，查询需要遍历。
   6. 双端队列(Deque)：栈和队列的结合体，本质还是一个队列，只是可在前端和尾端进行Push和Pop操作。插入和删除都是O(1)，因为无序需遍历，查询还是O(n)。
   7. 优先队列(Priority Queue)：插入O(1)；取出操作O(log n)，按照元素优先级取出。底层具体实现的数据结构较为多样和复杂(heap，bst，treap)

## 刷题

1. 数组：两数之和，删除重复项，接雨水，加一，合并数组，旋转数组，移动零，山脉数组
2. 链表：合并链表，设计循环双端队列

## 总结

1. 精通一个领域的方法：有点像“破而后立”，先把一个个知识点分解，逐个击破（单项训练），练到一定程度也就内化吸收了。由此又想到一句话Practice makes perfect!
2. 刷题技巧：五毒神掌。五遍只是一个概数，对于我来说，我想5遍基本是不够的，需要更多遍数扎实基本功。
3. 刷题所遇问题：① 过于追求LeetCode里的Runtime，死磕了不少时间。实际直接Copy别人代码运行，Runtime跟别人不一样；② 写完题后，习惯性的直接执行，经常出现单词拼写错误，“==”写成“=”等，还是需要写完养成检查一遍的习惯。
4. Coding三步法：① 参数验证 ② 解题 ③ 检查
5. **本周不足：** ① 没来得刷老师讲的例题，后面找时间需要补上； ② 已刷的题目强化不足，同时还没来得及研习国际站的Discuss部分。