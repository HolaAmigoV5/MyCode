## 第三周 总结

### 泛型递归、树的递归

递归(Recursion)：本质是循环，通过循环体进行的循环。一层层下，一层层回来。

1. recursion terminator 递归终结
2. process logic in current level 处理当前层逻辑
3. drill down 下探下层
4. reverse the current level status if needed.  清理当前层

代码模板(Java)：

```java
// Java
public void recursion(int level, int param) { 
  // terminator 
  if (level > MAX_LEVEL) { 
    // process result 
    return; 
  }
  // process current logic 
  process(level, param); 
  // drill down 
  recursion( level: level + 1, newParam); 
  // restore current status 
}
```

**思维要点：** 

1. 不要人肉进行递归（最大误区）
2. 找到最近最简方法，拆解成可重复解决的问题（重复子问题）
3. 数学归纳法思维

### 分治、回溯

分治：问题拆机成小问题处理后，组合结果后返回。

回溯：也叫“回溯搜索”算法，主要用于在一个庞大的空间搜索我们需要问题的解。解决一个回溯问题，实际就是一个决策树的遍历过程。

1. 路径：已经做出的选择。
2. 选择列表：当前可以做的选择。
3. 结束条件：到达决策树底层，无法再做选择的条件。

```java
//回溯算法框架
result = []
def backtrack(路径, 选择列表):
    if 满足结束条件:
        result.add(路径)
        return
    
    for 选择 in 选择列表:
        做选择
        backtrack(路径, 选择列表)
        撤销选择
```

回溯算法框架：**核心是for循环里面的递归，在递归调用之前【做选择】，递归调用后【撤销选择】。写backtrack函数时，需要维护走过的【路径】和当前可以走的【选择列表】，当触发【结束条件】时，将【路径】记入结果集中。**

### 课后刷题

| 题号                                                         | 名称                                                         | 难度   | 分类 | 解法           |
| ------------------------------------------------------------ | ------------------------------------------------------------ | ------ | ---- | -------------- |
| [46](https://leetcode-cn.com/problems/permutations/)         | [全排列](https://leetcode-cn.com/problems/permutations/)     | 🟡 中等 | 回溯 | ① backTracing. |
| [47](https://leetcode-cn.com/problems/permutations-ii/)      | [全排列 2](https://leetcode-cn.com/problems/permutations-ii/) | 🟡 中等 | 回溯 | ① backTracing. |
| [77](https://leetcode-cn.com/problems/combinations/)         | [组合](https://leetcode-cn.com/problems/combinations/)       | 🟡 中等 | 回溯 | ① backTracing. |
| [105](https://leetcode-cn.com/problems/construct-binary-tree-from-preorder-and-inorder-traversal/) | [从前序与中序遍历序列构造二叉树](https://leetcode-cn.com/problems/construct-binary-tree-from-preorder-and-inorder-traversal/) | 🟡 中等 | 分治 | ①分治          |
| [236](https://leetcode-cn.com/problems/lowest-common-ancestor-of-a-binary-tree/) | [二叉树的最近公共祖先](https://leetcode-cn.com/problems/lowest-common-ancestor-of-a-binary-tree/) | 🟡 中等 | 分治 | ①分治          |
