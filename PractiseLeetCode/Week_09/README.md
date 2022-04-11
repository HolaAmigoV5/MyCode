## 第九周 总结

### 高级动态规划

1. 复杂问题分解
2. 最优子结构
3. 状态顺推

**参考第六周总结动态规划详情。**

### 字符串算法

​	C#中字符串是不可变的，每次操作字符串都会生成新的字符串。C#中字符串比较“\==”和Equals是一样，对于值类型，两者没有区别，对于引用类型来说，“\==”比较的是引用地址，Equals比较的是内容。字符串问题一般都是用**双指针**或**哈希映射**解决。

**字符串问题常用代码**

```c#
//分割字符串
string[] strs=s.Split(' ');
//可变字符串
var sb=new StringBuilder();
sb.Append(strs[len]+" ");

//字符操作
char.IsLetterOrDigit(s[left]); //判定是否是字母或数字
char.ToLower(s[left++])!=char.ToLower(s[right--]); //字符转小写比较

//字符哈希映射
int[] m=new int[512];
for(int i=0; i<s.Length; i++){
    if(m[s[i]]!=m[t[i]+256])
        return false;
    m[s[i]]=m[t[i]+256]=i+1;
}
return true;

//反转字符串
int left=0, right=s.Length-1;
while(left<right){
    var tmp=s[left];
    s[left++]=s[right];
    s[right--]=tmp;
}
```

**字符串匹配算法**(了解原理)

1. Rabin-Karp算法，暴力算法基础上，进行哈希运算。将目标字符串(长度N)txt中子串(长度M)pat，全部哈希运算，比较哈希值，如果值不同，肯定不匹配，如果相同还需要使用朴素算法再次判断。——类似布隆过滤器。
2. KMP算法(Knuth-Morris-Pratt)，最长公共前后缀个数+字母=前缀表。通过前缀表进行匹配。
3. Boyer-Moore算法：各种编辑器查找功能大多采用此算法。德克萨斯大学的Robert S. Boyer教授和J Strother Moore教授发明了这种算法。
4. Sunday算法