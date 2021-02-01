## 第六周 总结

### 动态规划

#### 相关概念

  ​	 动态规划(Dynamic Programming)=分治+剪枝，用递归实现。动态规划的一般形式就是**求最值**，比如求最长递增子序列，最小编辑距离等。求解动态规划的**核心问题是穷举**，因为要求最值，肯定要吧所有可能的答案穷举出来，然后在其中找最值。因为这类问题存在**重叠子问题**，如果暴力穷举的话，效率低下，所以需要**备忘录**或者**DP table**来优化穷举过程，避免不必要的计算。动态规划一定会具备**最优子结构**，才能通过子问题的最值得到原问题的最值。动态规划的核心思想是穷举求最值，但是问题可以千变万化，穷举所有可行解其实并不是一件容易的事，只有列出正确的**状态转移方程**才能正确的穷举。

**解题思路：** ①确定base case， ②分解子问题， ③归纳DP方程， ④ 迭代/递归，⑤剪枝

**三要素 ：** ①重叠子问题 ② 最优子结构 ③ 状态转移方程(DP方程)。

- **重叠子问题：** 很多时候动态规划的穷举存在很多重叠，比如斐波那契树中，存在大量的重复计算。此时就需要剪枝，实现每个子问题不再重叠。

- **最优子结构：** 分解的子问题必须相互独立，互不影响。所有独立的子问题就是最优子结构。

- **状态转移方程（DP方程）** ：如下图的斐波那契数列，f(n)相当于`状态n`，这个n是由`状态n-1`和`状态n-2`相加转移而来，这就叫状态转移。

![fib](https://github.com/HolaAmigoV5/Images/raw/master/fib.png)

​	

>如何思考状态转移方程：**明确 base case ­­­­­­­­→ 明确「状态」→ 明确「选择」 → 定义 dp 数组/函数的含义**

```java
# 初始化 base case
dp[0][0][...] = base
# 进行状态转移
for 状态1 in 状态1的所有取值：
    for 状态2 in 状态2的所有取值：
        for ...
            dp[状态1][状态2][...] = 求最值(选择1，选择2...)
```

> **状态压缩** ：如果我们发现每次状态转移只需要DP table中的一部分，那么可以尝试用状态压缩来缩小DP table的大小，只记录必要的数据。一般来说是把一个二维的DP table压缩成一维，即把空间复杂度从O(n²)压缩到O(n)。比如斐波那契的状态只和之前的两个状态相关，不需要用一个数组DP table来存储所有的状态只要想办法存储之前的两个状态就行了。	

> PS：但凡遇到需要递归的问题，最好都画出递归树，这对你分析算法的复杂度，寻找算法低效的原因都有巨大帮助。递归算法的时间复杂度=子问题个数×子问题需要的时间。迭代是自底向上，递归是自顶向下。

#### 例题分析

​	 凑领钱问题：给你 `k` 种面值的硬币，面值分别为 `c1, c2 ... ck`，每种硬币的数量无限，再给一个总金额 `amount`，问你**最少**需要几枚硬币凑出这个金额，如果不可能凑出，算法返回 -1 。

1. 确定base case：目标金额`amount`为0时，算法返回0，因为不需要任何硬币就已经凑出目标金额了；

2. 分解子问题：求凑11块钱最少需要几枚硬币，只需要知道凑10元最少需要几枚硬币+1(1个硬币，这个硬币可能是1元，2元或5元)即可，不断递推到base case（凑0元最少需要0枚硬币）；

3. 归纳DP方程：由简单到一般，归纳总结出DP方程（参考"如何列出状态转移方程"）；

4. 迭代/递归：使用迭代（自底向上）或者递归（自顶向下）方式确定写代码思路；

   <img src="https://github.com/HolaAmigoV5/Images/raw/master/coinchange1.png" style="zoom:70%;" />

5. 剪枝：对于重复进行删除。通过**备忘录**或者**dp数组**消除重复。

**如何列出状态转移方程：** 

1. **确定 base case：** 目标金额`amount`为0时，算法返回0，因为不需要任何硬币就已经凑出目标金额了。

2. **确定「状态」：** 即原问题和子问题中会变化的变量。目标金额会不断地向base case靠近，所以唯一的「状态」就是目标金额`amount`。

3. **确定「选择」：** 即导致「状态」产生变化的行为。所以硬币的面值，就是你的「选择」

4. **明确 `dp`函数/数组的定义：** 递归解题时需要一个递归的dp函数，一般来说函数的参数就是状态转移中会变化的量，也就是上面说到的「选择」；函数的返回值就是题目要求我们计算的量。就凑领钱来说，**`dp(n)`的定义：输入一个目标金额`n`，返回凑出目标金额`n`的最少硬币数量**。迭代解题时需要明确dp数组的定义，把「状态」也就是目标金额作为变量，不过`dp函数`体现在函数参数，而`dp数组`体现在数组索引。就凑领钱来说，**dp数组的定义：当目标金额为i时，至少需要dp[i]枚硬币凑出**。

   <img src="https://github.com/HolaAmigoV5/Images/raw/master/coinchange.png" alt="coinchange" style="zoom:70%;" />

   **凑硬币解法：**

   1. 暴力解法：

      ```c++
      def coinChange(coins: List[int], amount: int):
      
          def dp(n):
              # base case
              if n == 0: return 0
              if n < 0: return -1
              # 求最小值，所以初始化为正无穷
              res = float('INF')
              for coin in coins:
                  subproblem = dp(n - coin)
                  # 子问题无解，跳过
                  if subproblem == -1: continue
                  res = min(res, 1 + subproblem)
      
              return res if res != float('INF') else -1
      
          return dp(amount)
      ```

      

   2. 递归解法（带备忘录， dp函数）：

      ```c++
      def coinChange(coins: List[int], amount: int):
          # 备忘录
          memo = dict()
          def dp(n):
              # 查备忘录，避免重复计算
              if n in memo: return memo[n]
              # base case
              if n == 0: return 0
              if n < 0: return -1
              res = float('INF')
              for coin in coins:
                  subproblem = dp(n - coin)
                  if subproblem == -1: continue
                  res = min(res, 1 + subproblem)
      
              # 记入备忘录
              memo[n] = res if res != float('INF') else -1
              return memo[n]
      
          return dp(amount)
      ```

      

   3. 迭代解法（带dp table, dp数组）：

      ```c++
      int coinChange(vector<int>& coins, int amount) {
          // 数组大小为 amount + 1，初始值也为 amount + 1
          vector<int> dp(amount + 1, amount + 1);
          // base case
          dp[0] = 0;
          // 外层 for 循环在遍历所有状态的所有取值
          for (int i = 0; i < dp.size(); i++) {
              // 内层 for 循环在求所有选择的最小值
              for (int coin : coins) {
                  // 子问题无解，跳过
                  if (i - coin < 0) continue;
                  dp[i] = min(dp[i], 1 + dp[i - coin]);
              }
          }
          return (dp[amount] == amount + 1) ? -1 : dp[amount];
      }
      ```

   **解决两个字符串的动态规划问题，一般都是用两个指针** **`i,j`** **分别指向两个字符串的最后，然后一步步往前走，缩小问题的规模**。

### 课后刷题

| 题号 | 名称                                                         | 难度   | 分类   | 解法     |
| ---- | ------------------------------------------------------------ | ------ | ------ | -------- |
| 72   | [72. 编辑距离](https://leetcode-cn.com/problems/edit-distance/) | 🟢 困难 | 字符串 | 动态规划 |
| 91   | [91. 解码方法](https://leetcode-cn.com/problems/decode-ways/) | 🟡 中等 |        | 动态规划 |
| 213  | [213. 打家劫舍 II](https://leetcode-cn.com/problems/house-robber-ii/) | 🟡 中等 |        | 动态规划 |
| 221  | [221. 最大正方形](https://leetcode-cn.com/problems/maximal-square/) | 🟡 中等 |        | 动态规划 |
| 322  | [322. 零钱兑换](https://leetcode-cn.com/problems/coin-change/) | 🟡 中等 |        | 动态规划 |
| 621  | [621. 任务调度器](https://leetcode-cn.com/problems/task-scheduler/) | 🟡 中等 |        | 动态规划 |
| 647  | [647. 回文子串](https://leetcode-cn.com/problems/palindromic-substrings/) | 🟡 中等 | 字符串 | 动态规划 |

