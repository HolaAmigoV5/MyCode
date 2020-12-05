using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace ConsoleLeetCodeTest
{
    public partial class Solution
    {
        //Merge Sorted Array
        public void Merge(int[] nums1, int m, int[] nums2, int n)
        {
            //M1:
            /*  Array.Copy(nums2,0,nums1,m,n);
             Array.Sort(nums1); */

            //M2:
            /* for (int i = 0; i < nums2.Length; i++)
            {
                nums1[m+i]=nums2[i];
            }
            Array.Sort(nums1); */

            //M3:double pointers
            int p1 = m - 1, p2 = n - 1, p = m + n - 1;
            while (p2 >= 0)
            {
                //nums1[p--] = p1 >= 0 && nums1[p1] > nums2[p2] ? nums1[p1--] : nums2[p2--];
                if (p1 >= 0 && nums1[p1] > nums2[p2])
                {
                    nums1[p--] = nums1[p1--];
                    ShowResult(nums1);
                }

                else
                {
                    nums1[p--] = nums2[p2--];
                    ShowResult(nums1);
                }

            }

            //printout
            ShowResult(nums1);
        }

        public int Trap(int[] height)
        {
            //M1:
            //int sum = 0;
            ////最两端的列不用考虑，因为一定不会有雨水。所以下标从1到Length-2
            //for (int i = 1; i < height.Length - 1; i++)
            //{
            //    int max_left = 0;
            //    //找出左边最高
            //    for (int j = i - 1; j >= 0; j--)
            //    {
            //        if (height[j] > max_left)
            //        {
            //            max_left = height[j];
            //        }
            //    }

            //    int max_right = 0;
            //    //找出右边最高
            //    for (int j = i + 1; j < height.Length; j++)
            //    {
            //        if (height[j] > max_right)
            //            max_right = height[j];
            //    }

            //    //找出两端较小的
            //    int min = Math.Min(max_left, max_right);
            //    //只有较小的一段大于当前列的高度才会有水，其他情况不会有水
            //    if (min > height[i])
            //        sum += (min - height[i]);
            //}


            //M2:
            //int sum = 0;
            //int[] max_left = new int[height.Length];
            //int[] max_right = new int[height.Length];

            //for (int i = 1; i < height.Length - 1; i++)
            //    max_left[i] = Math.Max(max_left[i - 1], height[i - 1]);

            //for (int i = height.Length - 2; i >= 0; i--)
            //    max_right[i] = Math.Max(max_right[i + 1], height[i + 1]);
            //ShowResult(max_left);
            //ShowResult(max_right);
            //for (int i = 1; i < height.Length - 1; i++)
            //{
            //    int min = Math.Min(max_left[i], max_right[i]);
            //    if (min > height[i])
            //        sum += (min - height[i]);
            //}

            //M3:
            int max_left = 0, max_right = 0, sum = 0;
            int left = 0, right = height.Length - 1;

            //for(int i=1;i<height.Length-1; i++)
            //{
            //    if (height[left - 1] < height[right + 1])
            //    {
            //        max_left = Math.Max(max_left, height[left - 1]);
            //        int min = max_left;
            //        if (min > height[left])
            //            sum += min - height[left];
            //        left++;
            //    }
            //    else
            //    {
            //        max_right = Math.Max(max_right, height[right + 1]);
            //        int min = max_right;
            //        if (min > height[right])
            //            sum += min - height[right];
            //        right--;
            //    }
            //}

            while (left <= right)
            {
                if (max_left < max_right)
                {
                    sum += Math.Max(0, max_left - height[left]);
                    max_left = Math.Max(max_left, height[left]);
                    left++;
                }
                else
                {
                    sum += Math.Max(0, max_right - height[right]);
                    max_right = Math.Max(max_right, height[right]);
                    right--;
                }
            }
            return sum;
        }

        public void Rotate(int[] nums, int k)
        {
            //M1: violence
            int len = nums.Length;
            for (int i = 0; i < k; i++)
            {
                int pre = nums[len - 1];
                for (int j = 0; j < len; j++)
                {
                    int tem = nums[j];
                    nums[j] = pre;
                    pre = tem;
                }
            }

            //M2:user a new Array
            //int len = nums.Length;
            //int[] arr = new int[len];
            //k %= len;
            //for (int i = 0; i < len; i++)
            //{
            //    arr[(k + i) % len] = nums[i];
            //}
            ////Array.Copy(arr, 0, nums, 0, len);
            //arr.CopyTo(nums, 0);
            //ShowResult(nums);

            //int len = nums.Length;
            //k %= len;
            //int count = 0;
            //for (int start = 0; count< len; start++)
            //{
            //    int cur = start;
            //    int pre = nums[start];
            //    do
            //    {
            //        int next = (cur + k) % len;
            //        int tem = nums[next];
            //        nums[next] = pre;
            //        pre = tem;

            //        cur = next;
            //        count++;
            //        ShowResult(nums);
            //    } while (start != cur);
            //}
        }

        public int[] TwoSum(int[] nums, int target)
        {
            if (nums == null || nums.Length < 2) return null;
            var dic = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                if (dic.ContainsKey(nums[i]))
                    return new int[] { dic[nums[i]], i };
                else
                    dic[target - nums[i]] = i;
            }
            return null;
        }
        public IList<IList<int>> FourSum(int[] nums, int target)
        {
            //verification
            var res = new List<IList<int>>();
            if (nums == null || nums.Length < 4)
                return res;

            Array.Sort(nums);

            for (int i = 0; i < nums.Length - 3; i++)
            {

                if (i > 0 && nums[i] == nums[i - 1])
                    continue;
                for (int j = i + 1; j < nums.Length - 2; j++)
                {

                    int begin = j + 1;
                    int end = nums.Length - 1;
                    if (j > i + 1 && nums[j] == nums[j - 1])
                        continue;
                    while (begin < end)
                    {
                        int sum = nums[i] + nums[j] + nums[begin] + nums[end];
                        if (sum == target)
                        {
                            res.Add(new List<int>() { nums[i], nums[j], nums[begin], nums[end] });
                            while (begin < end && nums[begin] == nums[begin + 1])
                                begin++;
                            while (begin < end && nums[end] == nums[end - 1])
                                end--;
                            begin++;
                            end--;
                        }
                        else if (sum > target)
                            end--;
                        else if (sum < target)
                            begin++;
                    }

                }
            }
            return res;
        }

        public int RemoveDuplicates(int[] nums)
        {
            if (nums == null || nums.Length == 0) return 0;

            //int p = 0, q = 1;
            //while (q < nums.Length)
            //{
            //    if (nums[p] != nums[q])
            //    {
            //        if (q - p > 1)
            //            nums[p + 1] = nums[q];
            //        p++;
            //    }
            //    q++;
            //}
            //return p + 1;

            //M2
            //int p = 1;
            //for (int i = 0; i < nums.Length - 1; i++)
            //{
            //    if (nums[i] != nums[i + 1])
            //    {
            //        if (p != i + 1)
            //        {
            //            nums[p] = nums[i + 1];
            //        }
            //        p++;
            //    }
            //}
            //return p;

            //M3
            //int i = 0;
            //foreach(int n in nums)
            //{
            //    if (i < 1 || n > nums[i - 1])
            //        nums[i++] = n;
            //}
            //return i;
            
            //allow duplicates up to k(k=2)
            int i = 0;
            foreach (int n in nums)
            {
                if (i < 2 || n > nums[i - 2])
                    nums[i++] = n;
            }
            return i;
        }

        public bool isAnagram(string s, string t)
        {
            if (s == null || t == null)
                return false;
            if (s.Length != t.Length) return false;

            int[] alpha = new int[26];
            for (int i = 0; i < s.Length; i++)
            {
                alpha[s[i] - 'a']++;
                alpha[t[i] - 'a']--;
            }

            return !alpha.Any(item => item != 0);

            /*var dic = new Dictionary<char, int>();
            foreach (char cs in s)
            {
                if (dic.ContainsKey(cs))
                    dic[cs]++;
                else
                    dic[cs] = 1;
            }
            foreach (char ct in t)
            {
                if (dic.ContainsKey(ct))
                    if (dic[ct] > 1)
                        dic[ct]--;
                    else
                        dic.Remove(ct);
                else
                    return false;
            }
            return true;*/


            /*char[] schar = s.ToCharArray();
            char[] tchar = t.ToCharArray();
            Array.Sort(schar);
            Array.Sort(tchar);*/

            //return new string(schar) == new string(tchar);
        }

        public IList<IList<string>> GroupAnagrams(string[] strs)
        {
            //M1
            /* List<IList<string>> ans = new List<IList<string>>();
             bool[] used = new bool[strs.Length];
             for (int i = 0; i < strs.Length; i++)
             {
                 if (!used[i])
                 {
                     List<string> temp = new List<string> { strs[i] };
                     //两两比较判断字符串是否符合
                     for (int j = i + 1; j < strs.Length; j++)
                     {
                         if (!used[j] && isAnagram(strs[i], strs[j]))
                         {
                             used[j] = true;
                             temp.Add(strs[j]);
                         }
                     }
                     if (temp != null)
                         ans.Add(temp);
                 }
             }
             return ans;*/

            //M2
            var dic = new Dictionary<string, IList<string>>();
            foreach (var item in strs)
            {
                var map = GetWordMap(item);
                if (dic.ContainsKey(map))
                    dic[map].Add(item);
                else
                    dic[map] = new List<string>() { item };
            }
            return dic.Values.ToList();

        }

        public IList<string> FizzBuzz(int n)
        {
            //
            List<string> res = new List<string>();
            if (n < 1) return res;

            for (int i = 1; i <= n; i++)
            {
                if (i % 3 == 0 && i % 5 == 0)
                    res.Add("FizzBuzz");
                else if (i % 3 == 0)
                    res.Add("Fizz");
                else if (i % 5 == 0)
                    res.Add("Buzz");
                else
                    res.Add(i.ToString());
            }
            return res;
        }

        public int AddDigits(int num)
        {
            /*while (num>=10)
            {
                int next = 0;
                while (num!=0)
                {
                    next = next + num % 10;
                    num /= 10;
                }
                num = next;
            }
            return num;*/
            Console.WriteLine(-1 % 9);

            while (num > 9)
            {
                num = num / 10 + num % 10;
            }
            return num;
            //return (num - 1) % 9 + 1;
        }

        public int MaxDepth(TreeNode root)
        {
            //Recursion
            if (root == null) return 0;
            //int leftMaxDepth = MaxDepth(root.left);
            //int rightMaxDepth = MaxDepth(root.right);
            //return Math.Max(leftMaxDepth, rightMaxDepth) + 1;


            List<TreeNode> curlayer = new List<TreeNode>();
            curlayer.Add(root);
            int depth = 0;
            while (curlayer.Any())
            {
                var nextlayer = new List<TreeNode>();
                depth++;
                foreach (TreeNode node in curlayer)
                {
                    if (node.left == null && node.right == null)
                        return depth;
                    if (node.left != null)
                        nextlayer.Add(node.left);
                    if (node.right != null)
                        nextlayer.Add(node.right);
                }

                curlayer = nextlayer;
            }
            return depth;
        }

        public int[] TopKFrequent(int[] nums, int k)
        {
            //return nums.GroupBy(r => r).OrderByDescending(r => r.Count()).Take(k).Select(r => r.Key).ToArray();

            //Bucket Sort
            var dic = new Dictionary<int, int>();
            var list = new List<int>[nums.Length + 1];
            List<int> res = new List<int>();
            foreach (int num in nums)
            {
                if (dic.ContainsKey(num))
                    dic[num]++;
                else
                    dic[num] = 1;
            }

            foreach (int key in dic.Keys)
            {
                int i = dic[key];
                if (list[i] == null)
                    list[i] = new List<int>();
                list[i].Add(key);
            }

            for (int i = list.Length - 1; i >= 0 && res.Count < k; i--)
            {
                if (list[i] == null) continue;
                res.AddRange(list[i]);
            }
            return res.ToArray();
        }

        public int NthUglyNumber(int n)
        {
            // SortedSet. Small top heap
            // SortedSet<double> list=new SortedSet<double>();
            // int counter=1;
            // list.Add(1);
            // while(counter<n){
            //     double min=list.Min;
            //     list.Remove(min);
            //     list.Add(min*2);
            //     list.Add(min*3);
            //     list.Add(min*5);
            //     counter++;
            // }
            // return (int)(list.Min);

            //dynamic programing
            int[] dp = new int[n];
            dp[0] = 1;
            int index2 = 0, index3 = 0, index5 = 0;
            for (int i = 1; i < n; i++)
            {
                dp[i] = Math.Min(Math.Min(dp[index2] * 2, dp[index3] * 3), dp[index5] * 5);

                if (dp[i] == dp[index2] * 2) index2++;
                if (dp[i] == dp[index3] * 3) index3++;
                if (dp[i] == dp[index5] * 5) index5++;
            }

            return dp[n - 1];
        }

        public void MoveZeroes(int[] nums)
        {
            //双指针，慢指针记录第一个0位置，快指针不断遍历
            //1. Verification. 2. coding. 3 checking
            if (nums == null || nums.Length == 0) return;

            //M1:O(n), O(1)
            // int count=0;
            // for(int i=0; i<nums.Length; i++){
            //     if(nums[i]!=0){
            //         nums[count++]=nums[i];
            //     }
            // }

            // for(int j=count; j<nums.Length; j++){
            //     nums[j]=0;
            // }

            //M2:
            // int count=0;
            // for(int i=0; i<nums.Length; i++){
            //     if(nums[i]!=0){
            //          if(i!=count){
            //             nums[count]=nums[i];
            //             nums[i]=0;
            //         }
            //         count++;
            //     }
            // }

            //M3: use new Array
            /* int[] arr = new int[nums.Length];
             int index = 0;
             foreach (int num in nums)
             {
                 if (num != 0)
                     arr[index++] = num;
             }
             arr.CopyTo(nums, 0);*/

            //移动0至头部
            //int p = nums.Length - 1;
            //for (int i = nums.Length - 1; i >= 0; i--)
            //{
            //    if (nums[i] != 0)
            //    {
            //        if (i != p)
            //        {
            //            nums[p] = nums[i];
            //            nums[i] = 0;
            //        }
            //        p--;
            //    }
            //}

            int zeronum = nums.Length - 1;
            for (int i = nums.Length - 1; i >= 0; i--)
            {
                if (nums[i] != 0)
                    nums[zeronum--] = nums[i];
            }


            for (int j = 0; j <= zeronum; j++)
                nums[j] = 0;
        }

        public bool IsValid(string s)
        {
            //Verification
            if (s == null) return false;
            var stack = new Stack<char>();
            foreach (var item in s)
            {
                switch (item)
                {
                    case '(':
                        stack.Push(')');
                        break;
                    case '{':
                        stack.Push('}');
                        break;
                    case '[':
                        stack.Push(']');
                        break;
                    default:
                        if (stack.Count == 0 || item != stack.Pop())
                            return false;
                        break;
                }
            }
            return stack.Count == 0;

        }

        public ListNode ReverseList(ListNode head)
        {
            ////verification
            //if (head == null || head.next == null)
            //    return head;

            //Stack<ListNode> stack = new Stack<ListNode>();
            //ListNode pre = new ListNode(0);
            //ListNode theHead = pre;

            //while (head != null)
            //{
            //    stack.Push(head);
            //    head = head.next;
            //}

            //while (stack.Any())
            //{
            //    pre.next =new ListNode(stack.Pop().val);
            //    pre = pre.next;
            //}
            //return theHead.next;

            // ListNode pre=null;
            // ListNode cur=head;

            // while(cur!=null){
            //     var tem=cur.next;
            //     cur.next=pre;

            //     //pre and cur move one node
            //     pre=cur;
            //     cur=tem;
            // }
            // return pre;

            // ListNode cur=ReverseList(head.next);
            // head.next.next=head;
            // head.next=null;
            // return cur;

            if (head == null)
                return head;
            Stack<int> stack = new Stack<int>();
            ListNode theHead = head;
            while (head != null)
            {
                stack.Push(head.val);
                head = head.next;
            }
            head = theHead;
            while (stack.Any() && head != null)
            {
                head.val = stack.Pop();
                head = head.next;
            }
            return theHead;
        }

        public ListNode SwapPairs(ListNode head)
        {
            //Recursion
            if (head == null || head.next == null)
                return head;
            var next = head.next;
            head.next = SwapPairs(next.next);
            next.next = head;
            return next;

            //Iteration
            //ListNode pre = new ListNode(0);
            //ListNode cur = pre;
            //pre.next = head;
            //while (cur.next != null && cur.next.next != null)
            //{
            //    var start = cur.next;
            //    var end = cur.next.next;
            //    cur.next = end;

            //    start.next = end.next;
            //    end.next = start;
            //    cur = start;
            //}
            //return pre.next;
        }

        public int[] GetLeastNumbers(int[] arr, int k)
        {
            if (arr == null || arr.Length == 0)
                return new int[0];

            // Array.Sort(arr);
            // int[] res=new int[k];
            // for(int i=0; i<k; i++){
            //     res[i]=arr[i];
            // }
            // return res;

            return arr.OrderBy(item => item).Take(k).ToArray();
        }

        public int ClimbStairs(int n)
        {
            /*if (n < 2)
                return 1;
            int[] arr = new int[n + 1];
            arr[0] = 1;
            arr[1] = 1;
            for (int i = 2; i <= n; i++)
                arr[i] = arr[i - 1] + arr[i - 2];
            return arr[n];*/

            /* double sqrt_5 = Math.Sqrt(5);
             double fib_n = Math.Pow((1 + sqrt_5) / 2, n) - Math.Pow((1 - sqrt_5) / 2, n);
             return (int)(fib_n / sqrt_5);*/

            int p = 0, q = 0, r = 1;
            for (int i = 1; i <= n; i++)
            {
                p = q;
                q = r;
                r = p + q;
            }
            return r;
        }

        #region 前序遍历
        /// <summary>
        /// 前序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        List<int> list = new List<int>();
        public IList<int> Preorder(Node root)
        {
            //M1:Recursion
            if (root == null)
                return list;
            //list.Add(root.val);

            // foreach(Node cnode in root.children){
            //     Preorder(cnode);
            // }
            // return list;

            //M2:Iteration
            /*var stack = new Stack<Node>();
            stack.Push(root);
            while (stack.Any())
            {
                var temp = stack.Pop();
                list.Add(temp.val);

                if (temp.children != null)
                {
                    for (int i = temp.children.Count - 1; i >= 0; i--)
                        stack.Push(temp.children[i]);
                }
            }
            list.ForEach(item => Console.Write(item));
            return list;*/

            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);
            while (stack.Any())
            {
                var node = stack.Pop();
                if (node != null)
                {
                    if (node.children != null)
                    {
                        int count = node.children.Count();
                        for (int i = count - 1; i >= 0; i--)
                        {
                            stack.Push(node.children[i]);
                        }
                    }
                    stack.Push(node);
                    stack.Push(null);
                }
                else
                    list.Add(stack.Pop().val);
            }
            return list;
        }
        #endregion

        #region N叉树遍历
        /// <summary>
        /// 层序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        List<IList<int>> result = new List<IList<int>>();
        public IList<IList<int>> LevelOrder(Node root)
        {
            //M1:Recursion.
            /* if (root != null)
                 TraverseNode(root, 0);
             return result;*/

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(root);

            while (queue.Any())
            {
                //var size = queue.Count;
                var templist = new List<int>();
                for (int i = 0; i < queue.Count; i++)
                {
                    var cur = queue.Dequeue();
                    templist.Add(cur.val);
                    if (cur.children != null)
                    {
                        foreach (var child in cur.children)
                            queue.Enqueue(child);
                    }
                }
                result.Add(templist);
            }
            return result;

            /*List<Node> previousLayer = new List<Node>();
            previousLayer.Add(root);
            while (previousLayer.Any())
            {
                List<Node> currentLayer = new List<Node>();
                List<int> previousVals = new List<int>();
                previousLayer.ForEach(node =>
                {
                    previousVals.Add(node.val);
                    if (node.children != null)
                        currentLayer.AddRange(node.children);
                });
                result.Add(previousVals);
                previousLayer = currentLayer;
            }
            return result;*/
        }

        /// <summary>
        /// N叉树后序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public IList<int> Postorder(Node root)
        {
            //Verification
            List<int> res = new List<int>();
            if (root == null) return res;

            //M1:Recursion. left-right-root. O(n), O(n)/O(logn)
            // NTreePostorder(root, res);
            // return res;

            //M2:Iteration. O(n), O(n)
            Stack<Node> stack = new Stack<Node>();
            stack.Push(root);
            while (stack.Any())
            {
                var node = stack.Pop();
                if (node != null)
                {
                    stack.Push(node);
                    stack.Push(null);
                    if (node.children != null)
                    {
                        int count = node.children.Count;
                        for (int i = count - 1; i >= 0; i--)
                        {
                            stack.Push(node.children[i]);
                        }
                    }
                }
                else
                    res.Add(stack.Pop().val);
            }
            return res;
        }

        private void TraverseNode(Node node, int level)
        {
            if (result.Count <= level)
                result.Add(new List<int>());
            result[level].Add(node.val);
            if (node.children != null)
            {
                foreach (Node child in node.children)
                    TraverseNode(child, level + 1);
            }

        }
        #endregion

        #region 滑动窗口
        public int[] MaxSlidingWindow(int[] nums, int k)
        {
            if (nums.Length == 0 || k == 0) return new int[0];

            //int len = nums.Length;
            //int[] ans = new int[len - k + 1];

            //存数组下标
            //LinkedList<int> q = new LinkedList<int>();

            //for (int i = 1 - k, j = 0; j < nums.Length; i++, j++)
            //{
            //    if (i > 0 && q.First.Value == nums[i - 1])
            //        q.RemoveFirst();
            //    while (q.Any() && q.Last.Value < nums[j])
            //        q.RemoveLast();
            //    q.AddLast(nums[j]);

            //    if (i >= 0) ans[i] = q.First.Value;

            //}
            //return ans;
            

            //Brute force
            /*int len = nums.Length;
            if (len == 0)
                return nums;
            int[] res = new int[len - k + 1];
            for (int i = 0; i < res.Length; i++)
            {
                int max = int.MinValue;
                for (int j = i; j < i + k; j++)
                    max = Math.Max(max, nums[j]);

                res[i] = max;
            }
            return res;*/

            MonotonicQueue windws = new MonotonicQueue();
            int[] ans = new int[nums.Length - k + 1];
            int index = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                if (i < k - 1)
                    windws.Push(nums[i]);
                else
                {
                    windws.Push(nums[i]);
                    ans[index++] = windws.Max;
                    windws.Pop(nums[i - k + 1]);
                }
            }
            return ans;
        }

        /// <summary>
        /// 单调队列
        /// </summary>
        private class MonotonicQueue
        {
            LinkedList<int> deque;
            public MonotonicQueue() => deque = new LinkedList<int>();

            public int Max => deque.Max();
           
            public void Pop(int num)
            {
                if (deque.Any() && deque.First() == num)
                    deque.RemoveFirst();
                var s = new Stack<int>();
            }

            public void Push(int num)
            {
                while (deque.Any() && deque.Last() < num)
                {
                    deque.RemoveLast();
                }
                deque.AddLast(num);
            }
        }
        #endregion

        #region 组合
        List<IList<int>> res = new List<IList<int>>();
        public IList<IList<int>> Combine(int n, int k)
        {
            //Backtracking
            if (n <= 0 || k <= 0 || n < k)
                return res;
            FindCombinations(n, k, 1, new List<int>());
            return res;
        }

        private void FindCombinations(int n, int k, int start, List<int> list)
        {
            //terminator
            if (list.Count == k)
            {
                res.Add(new List<int>(list));
                return;
            }

            // for(int i=start; i<=n; i++){
            //     list.Add(i);
            //     CombineHelper(n,k,i+1,list);
            //     list.Remove(i);
            // }
            for (; start <= n - (k - list.Count) + 1; start++)
            {
                list.Add(start);
                FindCombinations(n, k, start + 1, list);
                list.Remove(start);
            }
            // while(start<=n){
            //     list.Add(start++);
            //     CombineHelper(n,k,start,list);
            //     list.Remove(start-1);
            // }
        }
        #endregion

        #region 全排列
        public IList<IList<int>> Permute(int[] nums)
        {
            int len = nums.Length;

            if (len == 0)
                return res;

            //bool[] used = new bool[len];
            //PermuteHelper(nums, len, 0, new List<int>(), used);
            //return res;

            //记录路径
            LinkedList<int> track = new LinkedList<int>();
            BackTrack(nums, track);
            return res;
        }

        private void BackTrack(int[] nums, LinkedList<int> track)
        {
            //触发结束条件
            if (track.Count == nums.Length)
            {
                res.Add(new List<int>(track));
                return;
            }

            for (int i = 0; i < nums.Length; i++)
            {
                if (track.Contains(nums[i]))
                    continue;
                //做选择
                track.AddLast(nums[i]);
                //进入下一层决策树
                BackTrack(nums, track);
                //取消选择
                track.RemoveLast();
            }
        }

        private void PermuteHelper(int[] nums, int len, int depth, List<int> list, bool[] used)
        {
            if (depth == len)
            {
                res.Add(new List<int>(list));
                return;
            }

            for (int i = 0; i < len; i++)
            {
                if (!used[i])
                {
                    list.Add(nums[i]);
                    used[i] = true;

                    PermuteHelper(nums, len, depth + 1, list, used);
                    //状态恢复
                    used[i] = false;
                    list.Remove(nums[i]);
                }
            }
        }
        #endregion

        #region 全排列2
        int[] the_nums = null;
        public IList<IList<int>> PermuteUnique(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return res;

            var list = new LinkedList<int>();
            Array.Sort(nums);
            the_nums = nums;

            var used = new bool[nums.Length];
            PermuteUniqueHelper(list, used);
            return res;
        }

        private void PermuteUniqueHelper(LinkedList<int> list, bool[] used)
        {
            if (list.Count == the_nums.Length)
            {
                res.Add(new List<int>(list));
                return;
            }

            for (int i = 0; i < the_nums.Length; i++)
            {
                if (used[i])
                    continue;
                if (i > 0 && the_nums[i] == the_nums[i - 1] && !used[i - 1])
                    continue;

                list.AddLast(the_nums[i]);
                used[i] = true;
                PermuteUniqueHelper(list, used);
                used[i] = false;
                list.RemoveLast();
            }
        }
        #endregion

        #region 电话号码的字母组合
        string[] letter_map = { "", "*", "abc", "def", "ghi", "jkl", "mno", "pqrs", "tuv", "wxyz" };
        List<string> resStr = new List<string>();
        public IList<string> LetterCombinations(string digits)
        {
            //take care the Boundary
            if (digits == null || digits.Length == 0)
                return new List<string>();

            IterStr(digits, "", 0);
            return resStr;

            //Queue<string> ans = new Queue<string>();
            //ans.Enqueue("");
            //while (ans.Peek().Length != digits.Length)
            //{
            //    string remove = ans.Dequeue();
            //    string map = letter_map[digits[remove.Length] - '0'];
            //    foreach (char c in map)
            //    {
            //        ans.Enqueue(remove + c);
            //    }
            //}
            //return ans.ToList();
        }

        private void IterStr(string str, string letter, int index)
        {
            if (index == str.Length)
            {
                resStr.Add(letter);
                return;
            }

            char c = str[index];
            int pos = c - '0';
            string map_string = letter_map[pos];
            for (int i = 0; i < map_string.Length; i++)
                IterStr(str, letter + map_string[i], index + 1);
        }
        #endregion

        #region 子集
        //List<IList<int>> result = new List<IList<int>>();
        public IList<IList<int>> Subsets(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return res;

            GetSubSets(nums, 0, new List<int>());
            return res;
        }

        private void GetSubSets(int[] nums, int index, List<int> list)
        {
            res.Add(new List<int>(list));

            //for (int i = index; i < nums.Length; i++)
            //{
            //    list.Add(nums[i]);
            //    GetSubSets(nums, i + 1, list);
            //    list.RemoveAt(list.Count - 1);
            //}

            for (; index < nums.Length; index++)
            {
                list.Add(nums[index]);
                GetSubSets(nums, index + 1, list);
                list.RemoveAt(list.Count - 1);
            }
        }
        #endregion

        #region N皇后
        List<IList<string>> output = new List<IList<string>>();
        public IList<IList<string>> SolveNQueens(int n)
        {
            //初始化棋盘
            char[][] chess = new char[n][];
            for (int i = 0; i < n; i++)
            {
                chess[i] = new char[n];
                for (int j = 0; j < n; j++)
                {
                    chess[i][j] = '.';
                }
            }
            Solve(chess, 0);
            return output;
        }

        private void Solve(char[][] chess, int row)
        {
            //output
            if (row == chess.Length)
            {
                output.Add(Construct(chess));
                return;
            }

            for (int col = 0; col < chess.Length; col++)
            {
                if (valid(chess, row, col))
                {
                    //begin choose
                    chess[row][col] = 'Q';
                    //go to next
                    Solve(chess, row + 1);
                    //backtrace
                    chess[row][col] = '.';
                }
            }
        }

        private bool valid(char[][] chess, int row, int col)
        {
            //check all cols
            for (int i = 0; i < row; i++)
            {
                if (chess[i][col] == 'Q')
                    return false;
            }
            //check 45 degree. right-top
            for (int i = row - 1, j = col + 1; i >= 0 && j < chess.Length; i--, j++)
            {
                if (chess[i][j] == 'Q')
                    return false;
            }

            //check 135 degree. left-top
            for (int i = row - 1, j = col - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (chess[i][j] == 'Q')
                    return false;
            }
            return true;

        }

        private IList<string> Construct(char[][] chess)
        {
            var path = new List<string>();
            for (int i = 0; i < chess.Length; i++)
            {
                path.Add(new string(chess[i]));
            }
            return path;
        }
        #endregion

        #region 岛屿问题
        public int NumIslands(char[][] grid)
        {
            //verification
            if (grid == null || grid.Length == 0)
                return 0;

            int num_island = 0;
            for (int row = 0; row < grid.Length; row++)
            {
                for (int col = 0; col < grid.Length; col++)
                {
                    if (grid[row][col] == '1')
                    {
                        num_island++;
                        DFS(grid, row, col);
                    }
                }
            }
            return num_island;
        }

        private void DFS(char[][] grid, int row, int col)
        {
            if (row < 0 || col < 0 || row >= grid.Length ||
                col >= grid[0].Length || grid[row][col] == '0')
                return;

            grid[row][col] = '0';
            DFS(grid, row - 1, col);
            DFS(grid, row + 1, col);
            DFS(grid, row, col - 1);
            DFS(grid, row, col + 1);
        }
        #endregion

        #region 模拟机器人行走
        public int RobotSim(int[] commands, int[][] obstacles)
        {
            // 朝向，根据命令修改朝向计数
            // 北/东/南/西
            int[] dx = new int[] { 0, 1, 0, -1 };
            int[] dy = new int[] { 1, 0, -1, 0 };
            HashSet<int> obSet = new HashSet<int>();
            foreach (int[] obs in obstacles)
            {
                obSet.Add(EncodeObs(obs[0], obs[1]));
            }

            int x = 0, y = 0, di = 0, ans = 0;
            foreach (int cmd in commands)
            {
                switch (cmd)
                {
                    case -2://向左转 90 度
                        di = (di + 3) % 4;
                        break;
                    case -1://向右转 90 度
                        di = (di + 1) % 4;
                        break;
                    default:
                        // 一步一步走，看是否碰到障碍物，遇到则停下
                        for (int i = 0; i < cmd; i++)
                        {
                            int nx = x + dx[di];
                            int ny = y + dy[di];

                            if (!obSet.Contains(EncodeObs(nx, ny)))
                            {
                                x = nx;
                                y = ny;
                                ans = Math.Max(ans, x * x + y * y);
                            }
                        }
                        break;
                }
            }
            return ans;
        }

        public int RobotSim3(int[] commands, int[][] obstacles)
        {
            //verification
            if (commands == null || commands.Length == 0)
                return 0;

            //prepare data
            int x = 0, y = 0, direction = 0, res = 0;
            HashSet<int> obs = new HashSet<int>();
            foreach (int[] obstacle in obstacles)
            {
                obs.Add(GetHashCode(obstacle[0], obstacle[1]));
            }
            foreach (var cmd in commands)
            {
                if (cmd < 0)
                {
                    direction = (direction + (cmd == -1 ? 1 : 3)) % 4;
                    continue;
                }
                int step = cmd;
                while (step-- > 0)
                {
                    var newx = x + (direction == 1 ? 1 : direction == 3 ? -1 : 0);
                    var newy = y + (direction == 0 ? 1 : direction == 2 ? -1 : 0);
                    if (!obs.Contains(GetHashCode(newx, newy)))
                    {
                        x = newx;
                        y = newy;
                    }
                }
                res = Math.Max(res, x * x + y * y);
            }
            return res;
        }

        private int GetHashCode(int x, int y)
        {
            //return (x + 30000) << 16 + y + 30000;  表示x的(16+y=30000)次方
            return ((x + 30000) << 16) + y + 30000;
        }

        private int GetDirection(int command, int pre)
        {
            switch (command)
            {
                case -1:
                    pre += 1;
                    break;
                case -2:
                    pre += 3;
                    break;
            }
            return pre % 4;
        }

        private (int, int) GetNewPosition(int direction, int x, int y)
        {
            switch (direction)
            {
                case 0:
                    y += 1;
                    break;
                case 1:
                    x += 1;
                    break;
                case 2:
                    y -= 1;
                    break;
                default:
                    x -= 1;
                    break;
            }
            return (x, y);
        }

        private int EncodeObs(int x, int y)
        {
            return ((x + 30000) << 16) + (y + 30000);
        }
        #endregion

        #region 单词接龙
        public int LadderLength(string beginWord, string endWord, IList<string> wordList)
        {
            //O(mn), O(mn)
            var wordSet = new HashSet<string>(wordList);
            if (wordSet.Count == 0 || !wordSet.Contains(endWord))
                return 0;

            //BFS
            // bool[] visited=new bool[wordList.Count];
            // int idx=wordList.IndexOf(beginWord);
            // if(idx!=-1)
            //     visited[idx]=true;

            var visited = new HashSet<string>();
            var toVisited = new HashSet<string>();

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(beginWord);

            int count = 0;
            while (queue.Any())
            {
                int size = queue.Count;
                count++;
                while (size-- > 0)
                {
                    string start = queue.Dequeue();
                    if (start.Equals(endWord))
                        return count;
                    var neighbors = GetNeighbors(start, wordSet);
                    foreach (var neighbor in neighbors)
                        queue.Enqueue(neighbor);
                }
            }
            return 0;
        }

        /// <summary>
        /// Two-ended BFS
        /// </summary>
        /// <param name="beginWord"></param>
        /// <param name="endWord"></param>
        /// <param name="wordList"></param>
        /// <returns></returns>
        public int LadderLength2(string beginWord, string endWord, IList<string> wordList)
        {
            wordSet = new HashSet<string>(wordList);
            if (wordSet.Count == 0 || !wordSet.Contains(endWord))
                return 0;

            HashSet<string> visited = new HashSet<string>();
            //var toVisited = new HashSet<string>();
            //toVisited.Add(beginWord);

            //分别用左边和右边扩散的哈希表代替单向BFS里的队列
            HashSet<string> beginVisited = new HashSet<string>
            {
                beginWord
            };

            HashSet<string> endVisited = new HashSet<string>
            {
                endWord
            };

            int step = 1;
            while (beginVisited.Any())
            {
                //visited.UnionWith(toVisited);
                //toVisited.Clear();

                step++;
                //优先选择小的哈希表进行扩散，考虑到的情况更少
                if (beginVisited.Count > endVisited.Count)
                {
                    //互换
                    var tmp = beginVisited;
                    beginVisited = endVisited;
                    endVisited = tmp;
                }

                //保证beginVisited是相对较小的集合
                //nextLevelVisited在扩散完成以后，会成为新的beginVisted
                var nextLevelVisited = new HashSet<string>();
                foreach (var word in beginVisited)
                {
                    var neighbors = GetNeighbors(word, wordSet);
                    foreach (var neighbor in neighbors)
                    {
                        if (endVisited.Contains(neighbor))
                            return step;
                        if (!visited.Contains(neighbor))
                        {
                            //toVisited.Add(neighbor);
                            nextLevelVisited.Add(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
                //这一行表示从begin这一侧向外扩散一层
                beginVisited = nextLevelVisited;
                //beginVisited = new HashSet<string>(toVisited);

            }
            return 0;
        }
        #endregion

        #region 单词接龙2
        List<IList<string>> res2 = new List<IList<string>>(); //存储结果
        string end = string.Empty;
        string begin = string.Empty;
        HashSet<string> wordSet = null;
        //记录当前节点和所有对应的子节点
        Dictionary<string, List<string>> nodeNeighbors = new Dictionary<string, List<string>>();
        public IList<IList<string>> FindLadders2(string beginWord, string endWord, IList<string> wordList)
        {
            wordSet = new HashSet<string>(wordList);
            if (wordSet.Count == 0 || !wordSet.Contains(endWord))
                return res2;

            end = endWord;
            begin = beginWord;
            wordSet.Remove(begin);//删除第一个元素，防止"["hot", "dog", "dot"]"时，又回到了hot，环

            if (!BFSMapAllNeighbors())
                return res2;
            DFS(begin, new List<string>());
            return res2;
        }

        private bool BFSMapAllNeighbors()
        {
            var visited = new HashSet<string>();
            var toVisit = new HashSet<string>();

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(begin);
            toVisit.Add(begin);
            bool foundEnd = false;

            while (queue.Any())
            {
                //foreach (var item in toVisit)
                //    visited.Add(item);
                visited.UnionWith(toVisit);
                toVisit.Clear();

                int count = queue.Count();
                while (count-- > 0)
                {
                    string cur = queue.Dequeue();
                    var neighbors = GetNeighbors(cur, wordSet);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.Equals(end))
                            foundEnd = true;
                        if (!visited.Contains(neighbor))
                        {
                            if (!nodeNeighbors.ContainsKey(cur))
                                nodeNeighbors[cur] = new List<string>();
                            nodeNeighbors[cur].Add(neighbor);

                            if (!toVisit.Contains(neighbor))
                            {
                                queue.Enqueue(neighbor);
                                toVisit.Add(neighbor);
                            }
                        }
                    }
                }
                if (foundEnd)
                    break;
            }
            return foundEnd;
        }

        //DFS:output all paths
        private void DFS(string cur, List<string> path)
        {
            path.Add(cur);

            if (end.Equals(cur))
                res2.Add(new List<string>(path));

            else if (nodeNeighbors.ContainsKey(cur))
            {
                foreach (var next in nodeNeighbors[cur])
                    DFS(next, path);
            }

            path.RemoveAt(path.Count - 1);
        }

        #region My Two-ended BFS
        List<IList<string>> res1 = new List<IList<string>>();
        string begin1, end1;
        //HashSet<string> wordSet1;
        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>(); //这里注意要用HashSet，自动去重
        public IList<IList<string>> FindLadders(string beginWord, string endWord, IList<string> wordList)
        {
            // 第 1 步：使用双向广度优先遍历得到后继结点列表 dict
            // key：字符串，value：广度优先遍历过程中 key 的后继结点列表
            // 第 2 步：基于后继结点列表 dict ，使用回溯算法得到所有最短路径列表

            //先将 wordList 放到哈希表里，便于判断某个单词是否在 wordList 里
            wordSet = new HashSet<string>(wordList);
            if (wordSet.Count == 0 || !wordSet.Contains(endWord))
                return res1;

            begin1 = beginWord;
            end1 = endWord;
            //wordSet.Remove(begin1); //移除开始元素，否则会出现无限循环

            bool found = BFS();
            if (!found)
                return res1;
            LinkedList<string> path = new LinkedList<string>();
            DFS(begin1, path);

            return res1;
        }

        private bool BFS()
        {
            var beginVisited = new HashSet<string>();
            beginVisited.Add(begin1);
            var endVisited = new HashSet<string>();
            endVisited.Add(end1);

            // 记录访问过的单词
            var visited = new HashSet<string>();
            visited.Add(begin1);
            visited.Add(end1);

            bool forward = true;
            bool found = false;

            // 在保证了 beginVisited 总是较小（可以等于）大小的集合前提下，&& endVisited.Any() 可以省略
            while (beginVisited.Any() && endVisited.Any())
            {
                // 一直保证 beginVisited 是相对较小的集合，方便后续编码
                if (beginVisited.Count > endVisited.Count)
                {
                    var temp = beginVisited;
                    beginVisited = endVisited;
                    endVisited = temp;

                    // 只要交换，就更改方向，以便维护 dict 的定义
                    forward = !forward;
                }
                var nextVisited = new HashSet<string>();
                // 默认 beginVisited 是小集合，因此从 beginVisited 出发
                foreach (var curWord in beginVisited)
                {
                    var neighbors = GetNeighbors(curWord);
                    foreach (var n in neighbors)
                    {
                        if (endVisited.Contains(n))
                        {
                            found = true;
                            // 在另一侧找到单词以后，还需把这一层关系添加到「后继结点列表」
                            AddToSuccessor(forward, curWord, n);
                        }
                        if (!visited.Contains(n))
                        {
                            nextVisited.Add(n);
                            AddToSuccessor(forward, curWord, n);
                        }
                    }

                }
                beginVisited = nextVisited;
                visited.UnionWith(nextVisited);
                if (found)
                    break;
            }
            return found;
        }

        private void DFS(string cur, LinkedList<string> path)
        {
            path.AddLast(cur);
            if (cur.Equals(end1))
            {
                res1.Add(new List<string>(path));
            }
            else if (dict.ContainsKey(cur))
            {
                foreach (var next in dict[cur])
                    DFS(next, path);
            }
            path.RemoveLast();
        }

        private void DFS2(string cur, List<string> path)
        {
            path.Add(cur);
            if (cur.Equals(end))
            {
                res1.Add(new List<string>(path));
                //return;
            }
            else if (dict.ContainsKey(cur))
            {
                foreach (var next in dict[cur])
                    DFS2(next, path);
            }
            path.RemoveAt(path.Count - 1);
        }

        private void AddToSuccessor(bool forward, string curWord, string nextWord)
        {
            if (!forward)
            {
                string temp = curWord;
                curWord = nextWord;
                nextWord = temp;
            }
            if (!dict.ContainsKey(curWord))
                dict[curWord] = new List<string>();
            dict[curWord].Add(nextWord);
        }
        private List<string> GetNeighbors(string cur)
        {
            var ans = new List<string>();
            if (string.IsNullOrEmpty(cur))
                return ans;
            char[] chars = cur.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char old = chars[i];
                for (char c = 'a'; c <= 'z'; c++)
                {
                    if (c == old)
                        continue;
                    chars[i] = c;
                    string next = new string(chars);
                    if (wordSet.Contains(next))
                        ans.Add(next);
                }
                chars[i] = old;
            }
            return ans;
        }
        #endregion


        //M1:遍历wordList来判断每个单词和当前单词是否只有一个字母不同.O(mn)
        private bool CanConvert(string beginWord, string curWord)
        {
            int count = 0;
            for (int i = 0; i < beginWord.Length; i++)
            {
                if (beginWord[i] != curWord[i])
                    count++;
                if (count == 2)
                    return false;
            }
            return count == 1;
        }

        //M2:将要找的节点单词每个位置换一个字符，然后看更改后的字符是否在wordList中.O(26n)
        private List<string> GetNeighbors(string cur, HashSet<string> dict)
        {
            var ans = new List<string>();
            char[] chars = cur.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char old = chars[i];
                //考虑变成其他所有的字母
                for (char c = 'a'; c <= 'z'; c++)
                {
                    if (c == old)
                        continue;
                    chars[i] = c;
                    string next = new string(chars);

                    if (dict.Contains(next))
                        ans.Add(next);
                }
                chars[i] = old;
            }
            if (!dict.Contains(end))
                dict.Add(end);
            return ans;
        }
        #endregion

        public double MyPow(double x, long n)
        {
            //double res = 1;
            //for (int i = 0; i < (n > 0 ? n : -n); i++)
            //    res *= x;

            //return n > 0 ? res : 1 / res;

            if (n == 0)
                return 1;

            if (n < 0)
            {
                n = -n;
                x = 1 / x;
            }
            var y = MyPow(x, n / 2);
            return n % 2 == 0 ? y * y : x * y * y;

            //return n % 2 == 0 ? MyPow(x * x, n / 2) : x * MyPow(x * x, n / 2);
        }

        private string GetWordMap(string str)
        {
            //str.Split("[],".ToCharArray(),StringSplitOptions.RemoveEmptyEntries)
            var arr = new int[26];
            foreach (var item in str)
            {
                arr[item - 'a']++;
            }
            return string.Join(",", arr);
        }

        #region Find the Majority Element
        public int MajorityElement(int[] nums)
        {
            
            //求众数
            //M1: Sort O(nlogn) O(logn)
            // Array.Sort(nums);
            // return nums[nums.Length/2];
            //return nums.GroupBy(i=>i).OrderBy(i=>i.Count()).Select(i=>i.Key).Last();

            //M2:Dictionary. O(n), O(n)
            // var dic=new Dictionary<int, int>();
            // var n=nums.Length/2;
            // foreach(var num in nums){
            //     if(dic.ContainsKey(num)){
            //         dic[num]++;
            //     }
            //     else{
            //         dic[num]=1;
            //     }
            //     if(dic[num]>n)
            //         return num;
            // }
            // return -1;

            //M3:Moore Vote. O(n), O(1)
            // int candidate=0, count=0;
            // for(int i=0; i<nums.Length; i++){
            //     if(count==0)
            //         candidate=nums[i];
            //     count+=nums[i]==candidate?1:-1;
            // }
            // return candidate;

            //divide and Conquer
            return MajorityElementRec(nums, 0, nums.Length - 1);
        }

        private int MajorityElementRec(int[] nums, int lo, int hi)
        {
            if (lo == hi)
                return nums[lo];
            int mid = lo + (hi - lo) / 2;
            int left = MajorityElementRec(nums, lo, mid);
            int right = MajorityElementRec(nums, mid + 1, hi);

            if (left == right)
                return left;

            int leftCount = CountInRange(nums, left, lo, hi);
            int rightCount = CountInRange(nums, right, lo, hi);

            return leftCount > rightCount ? left : right;
        }

        private int CountInRange(int[] nums, int num, int lo, int hi)
        {
            int count = 0;
            for (int i = lo; i <= hi; i++)
            {
                if (nums[i] == num)
                    count++;
            }
            return count;
        }
        #endregion

        public void ShowResult(int[] nums)
        {
            Array.ForEach(nums, k => Console.Write(k));
            Console.WriteLine();
            //nums.ToHashSet<int>();
        }
    }
}
