using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleLeetCodeTest
{
    public partial class Solution
    {
        #region 529.扫雷游戏
        int[] dx = { -1, -1, 0, 1, 1, 1, 0, -1 };  //相邻位置（左，左上，上，右上，右，右下，下，左下）
        int[] dy = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int row = 0, col = 0;

        #region DFS
        public char[][] UpdateBoard(char[][] board, int[] click)
        {
            row = board.Length;
            col = board[0].Length;


            //MinesDFS(board, click[0], click[1]);
            //return board;

            //BFS
            Queue<MineNode> queue = new Queue<MineNode>();
            queue.Enqueue(new MineNode(click[0], click[1]));
            while (queue.Count > 0)
            {
                MineNode top = queue.Dequeue();
                int x = top.X;
                int y = top.Y;
                if (board[x][y] == 'E')
                {
                    board[x][y] = 'B';
                    int count = Judge(board, x, y);
                    if (count == 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int newX = x + dx[i];
                            int newY = y + dy[i];
                            if (newX < 0 || newX >= row || newY < 0 || newY >= col)
                                continue;
                            queue.Enqueue(new MineNode(newX, newY));
                        }
                    }
                    else
                        board[x][y] = (char)(count + '0');
                }
                else if (board[x][y] == 'M')
                    board[x][y] = 'X';

            }
            return board;
        }

        private void MinesDFS(char[][] board, int x, int y)
        {
            if (x < 0 || x >= row || y < 0 || y >= col)
                return;

            if (board[x][y] == 'E')
            {
                //如果当前为E，才进行判断是否要递归相邻节点
                board[x][y] = 'B';
                int count = Judge(board, x, y);
                if (count == 0)
                {
                    //如果为0则进行递归
                    for (int i = 0; i < 8; i++)
                        MinesDFS(board, x + dx[i], y + dy[i]);
                }
                else
                {
                    board[x][y] = (char)(count + '0');
                }
            }
            else if (board[x][y] == 'M')
            {
                //注意不要用else，否则会递归改掉已经是数字的位置
                board[x][y] = 'X';
            }
        }

        //统计雷的数量
        private int Judge(char[][] board, int x, int y)
        {
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];
                if (newX < 0 || newX >= row || newY < 0 || newY >= col)
                    continue;
                if (board[newX][newY] == 'M')
                    count++;
            }
            return count;
        }
        #endregion


        #endregion

        public bool CanJump(int[] nums)
        {
            int k = 0;
            for (int i = 0; i < nums.Length; i++)
            {
                if (i > k)
                    return false;
                k = Math.Max(k, nums[i] + i);
            }
            return true;

            //int dis = 0;
            //for (int i = 0; i <nums.Length-1; i++)
            //{
            //    dis = Math.Max(dis, i + nums[i]);
            //    if (dis >= nums.Length - 1)
            //        return true;
            //}
            //return false;
        }

        public int Jump(int[] nums)
        {
            int end = 0;
            int maxPosition = 0;
            int steps = 0;
            for (int i = 0; i < nums.Length - 1; i++)
            {
                maxPosition = Math.Max(maxPosition, nums[i] + i);
                if (i == end)
                {
                    end = maxPosition;
                    steps++;
                }
            }
            return steps;

            //int ans = 0;
            //int start = 0;
            //int end = 1;
            //while (end < nums.Length)
            //{
            //    int maxPos = 0;
            //    for (int i = start; i < end; i++)
            //    {
            //        maxPos = Math.Max(maxPos, i + nums[i]);
            //    }
            //    start = end;
            //    end = maxPos + 1;
            //    ans++;
            //}

            //return ans;
        }

        public int FindTarget(int[] nums, int target)
        {
            int left = 0, right = nums.Length - 1;
            while (left <= right)
            {
                var mid = (right + left) / 2;
                if (nums[mid] == target)
                    return mid;
                if (nums[mid] > target)
                    right = mid - 1;
                else
                    left = mid + 1;
            }
            return -1;
        }

        public int FindMin(int[] nums)
        {
            //brute force
            // int min=nums[0];
            // foreach(var num in nums){
            //     min=Math.Min(min, num);
            // }
            // return min;

            //use api
            //return nums.Min();

            //binary search
            int left = 0, right = nums.Length - 1;
            while (left < right)
            {
                if (nums[left] < nums[right])
                    return nums[left];

                var mid = left + (right - left) / 2;
                if (nums[mid] > nums[left])
                    left = mid + 1;
                else
                    right = mid;
            }
            return nums[left];
        }


        public int SubarraySum(int[] nums, int k)
        {
            //int count = 0;
            //for (int i = 0; i < nums.Length; i++)
            //{
            //    for (int j = i; j < nums.Length; j++)
            //    {
            //        int sum = 0;
            //        for (int m = i; m <= j; m++)
            //            sum += nums[m];
            //        if (sum == k)
            //            count++;
            //    }
            //}

            //O(n*n) O(1)
            //for(int i=0; i<nums.Length; i++)
            //{
            //    int sum = 0;
            //    for(int j=i; j<nums.Length; j++)
            //    {
            //        sum += nums[j];
            //        if (sum == k)
            //            count++;
            //    }
            //}
            //return count;

            if (nums == null || nums.Length == 0)
                return 0;

            //前缀和
            int len = nums.Length;
            int[] preSum = new int[len + 1];
            preSum[0] = 0;
            for (int i = 0; i < len; i++)
            {
                preSum[i + 1] = preSum[i] + nums[i];
            }

            int count = 0;
            for (int left = 0; left < len; left++)
            {
                for (int right = left; right < len; right++)
                {
                    if (preSum[right + 1] - preSum[left] == k)
                        count++;
                }
            }
            return count;
        }

        public int MaxSubArray(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0;
            int res = nums[0];
            int sum = 0;
            foreach (int num in nums)
            {
                if (sum > 0)
                    sum += num;
                else
                    sum = num;
                res = Math.Max(res, sum);
            }

            return res;
        }

        public int LongestCommonSubsequence(string text1, string text2)
        {
            if (text1.Length == 0 || text2.Length == 0)
                return 0;
            int[,] dp = new int[text1.Length, text2.Length];
            for (int i = 1; i < text1.Length + 1; i++)
            {
                for (int j = 1; j < text2.Length + 1; j++)
                {
                    if (text1[i - 1] == text2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }

            return dp[text1.Length, text2.Length];
        }

        int count = 0;
        public int CountSubstrings(string s)
        {
            if (s == null || s.Length == 0)
                return 0;

            for (int i = 0; i < s.Length; i++)
            {
                extendPalindrome(s, i, i);  //odd length
                extendPalindrome(s, i, i + 1); //even length
            }
            return count;
        }

        private void extendPalindrome(string s, int left, int right)
        {
            while (left >= 0 && right < s.Length && s[left] == s[right])
            {
                count++;
                left--;
                right++;
            }
        }

        public int Rob(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0;
            int n = nums.Length;
            int[] dp = new int[n + 1];

            //base case
            dp[0] = 0; dp[1] = nums[0];
            for (int i = 2; i <= n; i++)
            {
                dp[i] = Math.Max(dp[i - 2] + nums[i - 1], dp[i - 1]);
            }
            return dp[n];
        }

        public int Rob2(int[] nums)
        {
            int len = nums.Length;
            if (len == 0) return 0;
            if (len == 1) return nums[0];

            int m1 = HouseRob(nums, 0, len - 2);
            int m2 = HouseRob(nums, 1, len - 1);
            return Math.Max(m1, m2);
        }

        private int HouseRob(int[] nums, int start, int end)
        {
            // int pre=0, cur=0;
            // for(int i=start; i<=end; i++){
            //     var tmp=Math.Max(pre+nums[i], cur);
            //     pre=cur;
            //     cur=tmp;
            // }
            // return cur;

            int len = nums.Length;
            int[] dp = new int[len + 1];
            if (start == 0)
            {
                dp[0] = nums[0];
                dp[1] = Math.Max(nums[0], nums[1]);
            }
            else
                dp[1] = nums[1];

            for (int i = 2; i <= end; i++)
                dp[i] = Math.Max(dp[i - 2] + nums[i], dp[i - 1]);

            return dp[end];
        }


        public int UniquePaths(int m, int n)
        {
            //O(mn), O(mn)
            // int[,] dp=new int[m,n];
            // for(int i=0;i<n; i++)
            //     dp[0,i]=1;
            // for(int j=0;j<m; j++)
            //     dp[j,0]=1;
            // for(int i=1; i<m; i++){
            //     for(int j=1; j<n;j++){
            //         dp[i,j]=dp[i-1,j]+dp[i,j-1];
            //     }
            // }
            // return dp[m-1,n-1];

            //dynamic programming2：O(mn), O(n)
            int[] cur = new int[n];
            //Array.Fill(cur, 1);
            for (int i = 0; i < n; i++)
                cur[i] = 1;

            for (int i = 1; i < m; i++)
            {
                for (int j = 1; j < n; j++)
                    cur[j] += cur[j - 1];
            }

            return cur[n - 1];
        }

        public List<int> LargestValues(TreeNode root)
        {
            var res = new List<int>();
            if (root == null)
                return res;
            //DFS
            LargestValuesHelper(root, res, 0);
            return res;
        }

        private void LargestValuesHelper(TreeNode root, List<int> res, int level)
        {
            if (root == null)
                return;

            if (level == res.Count)
                res.Add(root.val);
            else
                res[level] = Math.Max(res[level], root.val);

            LargestValuesHelper(root.left, res, level + 1);
            LargestValuesHelper(root.right, res, level + 1);
        }

        public IList<string> GenerateParenthesis(int n)
        {
            List<string> res = new List<string>();
            if (n < 1)
                return res;

            DFS(res, "", n, n);
            return res;
        }
        private void DFS(List<string> res, string path, int left, int right)
        {
            if (left == 0 && right == 0)
            {
                res.Add(path);
                return;
            }
            if (left > 0)
                DFS(res, path + "(", left - 1, right);
            if (right > left)
                DFS(res, path + ")", left, right - 1);

        }

        public int LeastInterval(char[] tasks, int n)
        {
            if (tasks.Length <= 0 || n < 1)
                return tasks.Length;

            int[] counts = new int[26];
            for (int i = 0; i < tasks.Length; i++)
                counts[tasks[i] - 'A']++;

            Array.Sort(counts);
            int maxCount = counts[25];
            int retCount = (maxCount - 1) * (n + 1) + 1;
            int k = 24;
            while (k >= 0 && counts[k] == maxCount)
            {
                retCount++;
                k--;
            }
            return Math.Max(retCount, tasks.Length);
        }

        public int MinMutation(string start, string end, string[] bank)
        {
            //two-ended BFS
            var bankSet = bank.ToHashSet<string>();
            if (bankSet.Count == 0 || !bankSet.Contains(end))
                return -1;

            bankSet.Remove(start);
            var startVisited = new HashSet<string>() { start };
            var endVisited = new HashSet<string>() { end };
            var visited = new HashSet<string>();

            int count = 0;
            while (startVisited.Any() && endVisited.Any())
            {
                count++;
                //switch
                if (startVisited.Count > endVisited.Count)
                {
                    var tmp = startVisited;
                    startVisited = endVisited;
                    endVisited = tmp;
                }

                var nextVisited = new HashSet<string>();
                foreach (var str in startVisited)
                {
                    var neighbors = GetNeighbors2(str, bankSet);
                    foreach (var n in neighbors)
                    {
                        if (endVisited.Contains(n))
                            return count;
                        if (!visited.Contains(n) && !nextVisited.Contains(n))
                        {
                            nextVisited.Add(n);
                        }
                    }
                }
                visited.UnionWith(nextVisited);
                startVisited = nextVisited;
            }
            return -1;
        }

        private List<string> GetNeighbors2(string str, HashSet<string> bankSet)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(str))
                return list;
            var chars = str.ToCharArray();
            var gen = "ACGT";
            for (int i = 0; i < str.Length; i++)
            {
                var old = chars[i];
                foreach (var ch in gen)
                {
                    if (ch == old)
                        continue;
                    chars[i] = ch;
                    var newstr = new string(chars);
                    if (bankSet.Contains(newstr))
                        list.Add(newstr);
                }

                chars[i] = old;
            }
            return list;
        }

        public int NumDecodings(string s)
        {
            int n = s.Length;
            if (n == 0) return 0;

            int[] dp = new int[n + 1];
            //base case
            dp[0] = 1;
            dp[1] = s[0] != '0' ? 1 : 0;
            for (int i = 2; i <= n; i++)
            {
                int first = int.Parse(s.Substring(i - 1, 1));
                int second = int.Parse(s.Substring(i - 2, 2));
                if (first >= 1 && first <= 9)
                    dp[i] += dp[i - 1];
                if (second >= 10 && second <= 26)
                    dp[i] += dp[i - 2];
            }
            StringBuilder sb = new StringBuilder("abc");
            //sb.Insert()

            return dp[n];
        }

        public bool JudgeSquareSum(int c)
        {
            for (long i = 0; i * i <= c; i++)
            {
                var res = Math.Sqrt(c - i * i);
                //Console.WriteLine(res == (int)res);
                if (res == (int)res)
                    return true;
            }
            return false;
        }

        public int[] CountBits(int num)
        {
            int[] res = new int[num + 1];
            for (int i = 1; i <= num; i++)
            {
                int count = 0;
                var tmp = i;
                while (tmp != 0)
                {
                    count++;
                    tmp &= (tmp - 1);
                }
                res[i] = count;
            }
            return res;
        }

        public int[] RelativeSortArray(int[] arr1, int[] arr2)
        {
            //int[] arr = new int[20];
            //int[] res = new int[arr1.Length];

            //int index = 0;
            ////将arr1中的数记录下来
            //foreach (var item in arr1)
            //    arr[item]++;

            ////先安排arr2中的数
            //foreach (var item in arr2)
            //{
            //    while (arr[item]-- > 0)
            //    {
            //        res[index++] = item;
            //    }
            //}

            ////再安排剩下的元素
            //for (int i = 0; i < 20; i++)
            //{
            //    while (arr[i]-- > 0)
            //    {
            //        res[index++] = i;
            //    }
            //}
            //return res;

            // var arr=new int[1001];
            // foreach(var num in arr1)
            //     arr[num]++;

            // int index=0;
            // foreach(var num in arr2){
            //     while(arr[num]-->0){
            //         arr1[index++]=num;
            //     }
            // }

            // for(int i=0; i<1001; i++){
            //     while(arr[i]-->0)
            //         arr1[index++]=i;
            // }
            // return arr1;

            var dic = new Dictionary<int, int>();
            Array.Sort(arr1);

            foreach (var num in arr1)
            {
                if (dic.ContainsKey(num))
                    dic[num]++;
                else
                    dic[num] = 1;
            }

            int index = 0;
            foreach (var num in arr2)
            {
                while (dic[num]-- > 0)
                    arr1[index++] = num;
            }

            foreach (var num in dic.Keys)
            {
                int n = dic[num];
                while (n-- > 0)
                    arr1[index++] = num;

                //while (dic[num]-- > 0)  //这里不能修改集合值，修改后，无法遍历了
                //    arr1[index++] = num;
            }
            return arr1;
        }

        public string ReverseOnlyLetters(string S)
        {
            int l = 0, r = S.Length - 1;
            var cs = S.ToCharArray();
            while (l < r)
            {
                while (!IsLetter(cs[l]) && l < r) l++;
                while (!IsLetter(cs[r]) && l < r) r--;
                if (l < r)
                {
                    char c = cs[l];
                    cs[l++] = cs[r];
                    cs[r--] = c;
                }
            }

            return new string(cs);
        }

        private bool IsLetter(char c)
        {
            return ((c >= 65 && c <= 90) || (c >= 97 && c <= 122));
        }

        public bool IsIsomorphic(string s, string t)
        {
            //int n = s.Length;
            //if (n == 0) return true;
            //var dic = new Dictionary<char, char>();

            //for (int i = 0; i < n; i++)
            //{
            //    var ch1 = s[i];
            //    var ch2 = t[i];

            //    if (dic.ContainsKey(ch1))
            //    {
            //        if (dic[ch1] != ch2)
            //            return false;
            //    }
            //    else if (dic.ContainsValue(ch2))
            //        return false;
            //    else
            //        dic[ch1] = ch2;
            //}
            //return true;

            int[] m = new int[512];
            for (int i = 0; i < s.Length; i++)
            {
                if (m[s[i]] != m[t[i] + 256])
                    return false;
                m[s[i]] = m[t[i] + 256] = i + 1;

            }
            return true;
        }

        public int NumSquares(int n)
        {
            //dynamic programming. O(n*Sqrt(n)), O(n)
            // int[] dp=new int[n+1];
            // for(int i=1; i<=n; i++){
            //     dp[i]=i;
            //     for(int j=1; i-j*j>=0; j++)
            //         dp[i]=Math.Min(dp[i], dp[i-j*j]+1);
            // }
            // return dp[n];

            //M2:BFS
            var queue = new Queue<int>();
            queue.Enqueue(n);

            var visited = new HashSet<int>() { n };
            int level = 0;
            while (queue.Any())
            {
                var size = queue.Count;
                level++;
                while (size-- > 0)
                {
                    var cur = queue.Dequeue();
                    for (int i = 0; cur - i * i >= 0; i++)
                    {
                        var next = cur - i * i;
                        if (next == 0)
                            return level;

                        if (!visited.Contains(next))
                        {
                            visited.Add(next);
                            queue.Enqueue(next);
                        }
                    }
                }
            }
            return -1;
        }

        public ListNode ReverseKGroup(ListNode head, int k)
        {
            ListNode cur = head;
            int count = 0;
            while (cur != null && count != k)
            {
                cur = cur.next;
                count++;
            }
            if (count == k)
            {
                cur = ReverseKGroup(cur, k);
                while (count-- > 0)
                {
                    ListNode tmp = head.next;
                    head.next = cur;
                    cur = head;
                    head = tmp;
                }
                head = cur;
            }
            return head;
        }

        public string LongestCommonPrefix(string[] strs)
        {
            if (strs.Length == 0) return "";
            string ans = strs[0];

            // for(int i=1; i<strs.Length; i++){
            //     int j=0;
            //     for(; j<ans.Length && j<strs[i].Length; j++){
            //         if(ans[j]!=strs[i][j])
            //             break;
            //     }
            //     ans=ans.Substring(0,j);
            //     if(ans.Equals(""))
            //         return ans;
            // }
            // return ans;

            for (int i = 1; i < strs.Length; i++)
            {
                while (strs[i].IndexOf(ans) != 0)
                {
                    Console.WriteLine($"strs[i]={strs[i]}, ans={ans}, index={strs[i].IndexOf(ans)}");
                    ans = ans.Substring(0, ans.Length - 1);
                }
            }
            return ans;
        }

        public string ReverseWords(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length < 2)
                return s;

            //split then reverse
            // var strs=s.Split(' ');
            // var sb=new StringBuilder();
            // foreach(var str in strs){
            //     var chars=str.ToCharArray();
            //     Array.Reverse(chars);
            //     sb.Append(new string(chars) + " ");
            // }
            // return sb.ToString().Trim();

            //two pointers
            var chars = s.ToCharArray();
            int len = chars.Length;

            for (int i = 0; i < len; i++)
            {
                if (s[i] != ' ')
                {
                    int end = i;
                    while (end + 1 < len && s[end + 1] != ' ')
                        end++;
                    Reverse(chars, i, end);
                    i = end;
                }
            }
            return new string(chars);
        }
        private void Reverse(char[] chars, int left, int right)
        {
            while (left < right)
            {
                var tmp = chars[left];
                chars[left++] = chars[right];
                chars[right--] = tmp;
            }
        }

        public int MyAtoi(string str)
        {
            str = str.Trim();
            Match match = Regex.Match(str, "^[\\+\\-]?\\d+");
            int ans = 0;
            if (match.Success)
            {
                if (int.TryParse(match.Value, out ans))
                    return ans;
                else
                    ans = str[0] == '-' ? int.MinValue : int.MaxValue;
            }
            return ans;
        }

        public int[][] Merge(int[][] intervals)
        {
            if (intervals.Length <= 1)
                return intervals;
            Array.Sort(intervals, (v1, v2) => v1[0] - v2[0]);

            List<int[]> result = new List<int[]>(intervals.Length);
            int[] newInterval = intervals[0];
            result.Add(newInterval);

            foreach (var interval in intervals)
            {
                if (interval[0] <= newInterval[1])
                    newInterval[1] = Math.Max(newInterval[1], interval[1]);
                else
                {
                    newInterval = interval;
                    result.Add(newInterval);
                }
            }

            return result.ToArray();
        }

        public int LargestRectangleArea(int[] heights)
        {
            if (heights == null || heights.Length == 0)
                return 0;

            int len = heights.Length, area = 0;

            //brute force
            // for(int i=0; i<len; i++){
            //     var cur=heights[i];
            //     var left=i;
            //     int right=i;

            //     while(left>=0 && heights[left]>=cur)
            //         left--;

            //     while(right<len && heights[right]>=cur)
            //         right++;

            //     area=Math.Max(area, (right-left-1)*heights[i]);
            // }
            // return area;

            //M2
            int[] tmp = new int[len + 2];
            Array.Copy(heights, 0, tmp, 1, len);
            var stack = new Stack<int>();

            for (int i = 0; i < len + 2; i++)
            {
                while (stack.Count > 0 && tmp[i] < tmp[stack.Peek()])
                {
                    int h = tmp[stack.Pop()];
                    area = Math.Max(area, (i - stack.Peek() - 1) * h);
                }
                stack.Push(i);
            }
            return area;
        }

        public int LengthOfLIS(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0;

            //int max= 1, len = nums.Length;
            //dp
            // int[] dp=new int[len];
            // Array.Fill(dp, 1);

            // for(int i=1; i<len; i++){
            //     for(int j=0; j<i; j++){
            //         if(nums[j]<nums[i]){
            //             dp[i]=Math.Max(dp[i], dp[j]+1);
            //             max=Math.Max(max, dp[i]);
            //         }
            //     }
            // }
            // return max;

            //patience sorting
            int len = nums.Length;
            int piles = 0;
            int[] top = new int[len];
            foreach (var num in nums)
            {
                int left = 0, right = piles;
                while (left < right)
                {
                    var mid = (left + right) >> 1;
                    if (top[mid] >= num)
                        right = mid;
                    else
                        left = mid + 1;
                }
                if (left == piles)
                    piles++;
                top[left] = num;
            }
            return piles;
        }

        public int LengthOfLastWord(string s)
        {
            return s.Trim().Split(' ').Last().Length;
        }
        public int[][] KClosest(int[][] points, int K)
        {
            Array.Sort(points, (p1, p2)=>p1[0]*p1[0]+p1[1]*p1[1]-p2[0]*p2[0]-p2[1]*p2[1]);
            // int[][] res=new int[K][];
            // Array.Copy(points, 0, res,0,K);
            // return res;

            var res = new List<int[]>();
            while (K-- > 0)
                res.Add(points[K]);
            return res.ToArray();
        }

        #region 账户合并
        public IList<IList<string>> AccountsMerge(IList<IList<string>> accounts)
        {
            var result = new List<IList<string>>();
            if (accounts == null || accounts.Count == 0)
                return result;

            var emailToName = new Dictionary<string, string>();  //eamil-->username
            var graph = new Dictionary<string, HashSet<string>>(); //email-->neighbors

            foreach (var account in accounts)
            {
                string name = account[0];
                for (int i = 1; i < account.Count; i++)
                {
                    if (!graph.ContainsKey(account[i]))
                        graph[account[i]] = new HashSet<string>();
                    emailToName[account[i]] = name;

                    if (i != 1)
                    {
                        graph[account[i]].Add(account[i - 1]);
                        graph[account[i - 1]].Add(account[i]);
                    }
                }

            }

            var visited = new HashSet<string>();
            foreach (var email in graph.Keys)
            {
                if (!visited.Contains(email))
                {
                    visited.Add(email);
                    var newList = BFS(graph, visited, email);
                    // var newList=new List<string>();
                    // DFS(newList, graph, visited, email);
                    newList.Sort((left, right) => { return string.CompareOrdinal(left, right); });
                    newList.Insert(0, emailToName[newList[0]]);
                    result.Add(newList);
                }
            }
            return result;
        }

        public List<string> BFS(Dictionary<string, HashSet<string>> graph,
            HashSet<string> visited, string startPoint)
        {
            var newList = new List<string>();
            var queue = new Queue<string>();
            queue.Enqueue(startPoint);
            while (queue.Any())
            {
                int size = queue.Count;
                while (size-- > 0)
                {
                    string curEmail = queue.Dequeue();
                    newList.Add(curEmail);
                    var neighbors = graph[curEmail];
                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return newList;
        } 
        #endregion

        #region 翻转对
        public int ReversePairs(int[] nums)
        {
            if (nums == null || nums.Length == 0)
                return 0;

            //int count = 0, len = nums.Length;

            //burte force. timeout
            // for(int i=0; i<len; i++){
            //     for(int j=i+1; j<len; j++){
            //         if(nums[i]/2.0>nums[j])
            //             count++;
            //     }
            // }
            // return count;

            //MergeSort
            return MergeSort(nums, 0, nums.Length - 1);
        }

        private int MergeSort(int[] nums, int left, int right)
        {
            if (left >= right)
                return 0;

            int mid = (left + right) >> 1;
            int cnt = MergeSort(nums, left, mid) + MergeSort(nums, mid + 1, right);

            for (int i = left, j = mid + 1; i <= mid; i++)
            {
                while (j <= right && nums[i] / 2.0 > nums[j])
                    j++;
                cnt += j - (mid + 1);
            }
            Array.Sort(nums, left, right - left + 1);
            return cnt;
        }
        #endregion

        #region 排序链表
        public ListNode SortList(ListNode head)
        {
            if (head == null || head.next == null)
                return head;

            //cut
            ListNode fast = head.next, slow = head;
            while (fast != null && fast.next != null)
            {
                slow = slow.next;
                fast = fast.next.next;
            }
            ListNode tmp = slow.next;
            slow.next = null;

            ListNode left = SortList(head);
            ListNode right = SortList(tmp);

            //Merge
            ListNode h = new ListNode(0);
            ListNode res = h;
            while (left != null && right != null)
            {
                if (left.val < right.val)
                {
                    h.next = left;
                    left = left.next;
                }
                else
                {
                    h.next = right;
                    right = right.next;
                }
                h = h.next;
            }
            h.next = left ?? right;
            return res.next;

            //Bottom-to-up(no recurring)
            //ListNode dummy = new ListNode(0);
            //dummy.next = head;
            ////先统计长度
            //int n = 0;
            //while (head != null)
            //{
            //    head = head.next;
            //    n++;
            //}

            ////循环开始切割和合并
            //for (int size = 1; size < n; size <<= 1)
            //{
            //    ListNode prev = dummy;
            //    ListNode cur = dummy.next;
            //    while (cur != null)
            //    {
            //        ListNode left = cur;
            //        ListNode right = Cut(left, size);  //链表切掉size, 剩下返还给right
            //        cur = Cut(right, size); //链表切掉size剩下返还为cur
            //        prev = Merge(left, right, prev);
            //    }
            //}
            //return dummy.next;
        }

        private ListNode Cut(ListNode head, int size)
        {
            if (head == null)
                return null;
            for (int i = 1; head.next != null && i < size; i++)
            {
                head = head.next;
            }
            ListNode next = head.next;
            head.next = null;
            return next;
        }

        private ListNode Merge(ListNode left, ListNode right, ListNode prev)
        {
            ListNode cur = prev;
            while (left != null && right != null)
            {
                if (left.val < right.val)
                {
                    cur.next = left;
                    left = left.next;
                }
                else
                {
                    cur.next = right;
                    right = right.next;
                }
                cur = cur.next;
            }
            cur.next = left == null ? right : left;

            while (cur.next != null)
                cur = cur.next; //保持返还最尾端
            return cur;
        } 
        #endregion

        #region SlidingWindow

        public List<int> FindAnagrams(string s, string p)
        {
            List<int> result = new List<int>();
            if (p.Length > s.Length)
                return result;

            //int[] needs = new int[26];
            //int[] window = new int[26];

            //foreach (var ch in p)
            //{
            //    needs[ch - 'a']++;
            //}

            //int left = 0, right = 0;
            ////右窗口不断向右移动
            //while (right < s.Length)
            //{
            //    int rIndex = s[right] - 'a';
            //    window[rIndex]++;
            //    right++;

            //    //当window数组中，rIndex比needs数组中对应元素的个数要多时，移动左窗口
            //    while (window[rIndex] > needs[rIndex])
            //    {
            //        int lIndex = s[left] - 'a';
            //        window[lIndex]--;
            //        left++;
            //    }

            //    if (right - left == p.Length)
            //        res.Add(left);
            //}
            //return res;

            //int[] hash = new int[256];
            //foreach (char ch in p)
            //    hash[ch]++;
            //int left = 0, right = 0;
            //int count = p.Length;

            //while (right < s.Length)
            //{
            //    //move right everytime, if the character exists in p's hash, decrease the count
            //    //current hash value >= 1 means the character is existing in p
            //    if (hash[s[right++]]-- >= 1)
            //        count--;

            //    //when the count is down to 0, means we found the right anagram
            //    //then add window's left to result list
            //    if (count == 0)
            //        res.Add(left);

            //    //if we find the window's size equals to p, 
            //    //then we have to move left (narrow the window) to find the new match window
            //    //++ to reset the hash because we kicked out the left
            //    //only increase the count if the character is in p
            //    //the count >= 0 indicate it was original in the hash, cuz it won't go below 0
            //    if (right - left == p.Length && hash[s[left++]]++ >= 0)
            //        count++;
            //}
            //return res;

            //count the char num. in P
            var dic = new Dictionary<char, int>();
            foreach (var ch in p)
            {
                if (dic.ContainsKey(ch))
                    dic[ch]++;
                else
                    dic[ch] = 1;
            }

            var window = new Dictionary<char, int>();
            int left = 0, right = 0, len = p.Length;
     
            //build window.
            while (right < s.Length)
            {
                if (dic.ContainsKey(s[right]))
                {
                    if (window.ContainsKey(s[right]))
                        window[s[right]]++;
                    else
                        window[s[right]] = 1;
                    if (window[s[right]] <= dic[s[right]])
                        len--;
                }

                //shrink the left
                while (len == 0)
                {
                    if (right - left + 1 == p.Length)
                        result.Add(left);
                    if (dic.ContainsKey(s[left]))
                    {
                        window[s[left]]--;
                        if (window[s[left]] < dic[s[left]])
                            len++;
                    }
                    left++;
                }
                right++;
            }
            return result;
        }

        public List<int> SlidingWindowTemplate(string s, string t)
        {
            //init a collection or int value to save the result according the question.
            List<int> result = new List<int>();
            if (t.Length > s.Length) return result;

            //create a hashmap to save the Characters of the target substring.
            var map = new Dictionary<char, int>();
            foreach (char c in t)
            {
                if (map.ContainsKey(c))
                    map[c]++;
                else
                    map[c] = 1;
            }

            //maintain a counter to check whether match the target string.
            int counter = map.Count;//must be the map size, NOT the string size because the char may be duplicate.

            //Two Pointers: begin - left pointer of the window; end - right pointer of the window
            int begin = 0, end = 0;

            //the length of the substring which match the target string.
            //int len = int.MaxValue;

            //loop at the begining of the source string
            while (end < s.Length)
            {
                char c = s[end];//get a character
                if (map.ContainsKey(c))
                {
                    map[c]--;   // plus or minus one
                    //modify the counter according the requirement(different condition).
                    if (map[c] == 0) counter--;
                }
                end++;

                //increase begin pointer to make it invalid/valid again
                /* counter condition. different question may have different condition */
                while (counter == 0)
                {
                    //***be careful here: choose the char at begin pointer, NOT the end pointer
                    char tempc = s[begin];
                    if (map.ContainsKey(tempc))
                    {
                        map[tempc]++;   //plus or minus one
                        //modify the counter according the requirement(different condition).
                        if (map[tempc] > 0) counter++;
                    }

                    /* save / update(min/max) the result if find a target*/
                    // result collections or result int value
                    if (end - begin == t.Length)
                        result.Add(begin);

                    begin++;
                }
            }
            return result;
        }

        public int LengthOfLongestSubstring(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            int max = 0, left = 0;
            var dic = new Dictionary<char, int>();
            for (int i = 0; i < s.Length; i++)
            {
                if (dic.ContainsKey(s[i]))
                    left = Math.Max(left, dic[s[i]]);
                dic[s[i]] = i + 1;
                max = Math.Max(max, i - left + 1);
            }
            return max;
        }
        #endregion

        #region 朋友圈
        public int FindCircleNum(int[][] M)
        {
            int[] visited = new int[M.Length];
            int ans = 0;
            for (int i = 0; i < M.Length; i++)
            {
                if (visited[i] == 0)
                {
                    ans++;
                    DFS(M, i, visited);
                }
            }
            return ans;
        }
        private void DFS(int[][] M, int i, int[] visited)
        {
            for (int j = 0; j < M.Length; j++)
            {
                if (M[i][j] == 1 && visited[j] == 0)
                {
                    Console.WriteLine($"i={i},  j={j}");
                    visited[j] = 1;
                    DFS(M, j, visited);
                }

            }
        }

        #endregion

        #region 股票问题
        public int MaxProfit(int k, int[] prices)
        {
            if (prices == null || prices.Length == 0)
                return 0;

            int len = prices.Length;
            if (k > len / 2)
                return MaxProfit_k_inf(prices);

            int[,,] dp = new int[len, k + 1, 2];

            for (int i = 0; i < len; i++)
            {
                for (int j = k; j >= 1; j--)
                {
                    if (i == 0)
                    {
                        //base case
                        dp[0, j, 0] = 0;
                        dp[0, j, 1] = int.MinValue;
                        continue;
                    }
                    dp[i, j, 0] = Math.Max(dp[i - 1, j, 0], dp[i - 1, j, 1] + prices[i]);
                    dp[i, j, 1] = Math.Max(dp[i - 1, j, 1], dp[i - 1, j - 1, 0] - prices[i]);
                }
            }
            return dp[len - 1, k, 0];
        }

        public int MaxProfit_k_inf(int[] prices)
        {
            if (prices == null || prices.Length == 0)
                return 0;

            int len = prices.Length;
            int dp_i_0 = 0, dp_i_1 = int.MinValue;
            for (int i = 0; i < len; i++)
            {
                int temp = dp_i_0;
                dp_i_0 = Math.Max(dp_i_0, dp_i_1 + prices[i]);
                dp_i_1 = Math.Max(dp_i_1, temp - prices[i]);
            }
            return dp_i_0;

            //greedy
            // int profit=0;
            // for(int i=1; i<prices.Length; i++)
            //     profit+=Math.Max(0,prices[i]-prices[i-1]);
            // return profit;
        }
        #endregion

        #region 零钱兑换
        int ans = int.MaxValue;
        public int CoinChange(int[] coins, int amount)
        {
            Array.Sort(coins);
            CoinChangeHelper(coins.Length - 1, coins, 0, amount);
            return ans == int.MaxValue ? -1 : ans;
        }

        private void CoinChangeHelper(int index, int[] coins, int count, int needAmount)
        {
            if (needAmount == 0)
            {
                ans = Math.Min(count, ans);
                return;
            }
            if (index < 0)
                return;
            int i = needAmount / coins[index];
            for (int k = i; k >= 0 && count + k < ans; k--)
                CoinChangeHelper(index - 1, coins, count + k, needAmount - k * coins[index]);
        }

        //零钱兑换2
        public int Change(int amount, int[] coins)
        {
            //dynamic programming. O(mn), O(m)
            int[] dp = new int[amount + 1];
            dp[0] = 1;
            foreach (var coin in coins)
            {
                for (int i = 1; i <= amount; i++)
                {
                    if (i < coin)
                        continue;
                    dp[i] += dp[i - coin];
                }
            }
            return dp[amount];
        }
        #endregion

        #region 编辑距离
        public int MinDistance(string word1, string word2)
        {
            int m = word1.Length, n = word2.Length;
            int[,] dp = new int[m + 1, n + 1];

            //base case
            for (int i = 1; i <= m; i++)
                dp[i, 0] = i;
            for (int j = 1; j <= n; j++)
                dp[0, j] = j;

            //from bottom to up
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (word1[i - 1] == word2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                    {
                        dp[i, j] = Math.Min(dp[i - 1, j] + 1, Math.Min(dp[i, j - 1] + 1, dp[i - 1, j - 1] + 1));
                    }
                }
            }
            return dp[m, n];
        }
        #endregion

        #region 前缀树
        List<string> re = new List<string>();
        char[][] _board = null;
        public IList<string> FindWords(char[][] board, string[] words)
        {
            if (board.Length == 0 || board[0].Length == 0 || words.Length == 0)
                return re;
            this._board = board;

            //build Trie Tree
            Trie root = new Trie();
            foreach (var w in words)
                root.Insert(w);

            //begin to DFS
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[0].Length; j++)
                {
                    if (root.Next[board[i][j] - 'a'] != null)
                        DFS(i, j, root);
                }
            }
            return re;
        }

        private void DFS(int x, int y, Trie p)
        {
            if (!Legal(x, y) || p.Next[_board[x][y] - 'a'] == null)
                return;

            char letter = _board[x][y];
            Trie cur = p.Next[letter - 'a'];
            if (cur.IsEnd)
            {
                re.Add(cur.Word);
                cur.Word = null;
                cur.IsEnd = false;
            }

            _board[x][y] = '#';
            for (int i = 0; i < 4; i++)
            {
                DFS(x + (i == 1 ? 1 : i == 3 ? -1 : 0), y + (i == 0 ? 1 : i == 2 ? -1 : 0), cur);
            }
            _board[x][y] = letter;
            if (cur.IsLeaf())
                p.Next[letter - 'a'] = null;
        }

        private bool Legal(int x, int y)
        {
            return x >= 0 && x < _board.Length && y >= 0 && y < _board[0].Length && _board[x][y] != '#';
        }

        public class Trie
        {
            public Trie[] Next;
            public bool IsEnd;
            public string Word;

            /* Initialize your data structure here. */
            public Trie()
            {
                IsEnd = false;
                Next = new Trie[26];
            }

            /* Inserts a word into the trie. */
            public void Insert(string word)
            {
                if (string.IsNullOrEmpty(word))
                    return;

                Trie node = this;
                foreach (var ch in word)
                {
                    int index = ch - 'a';
                    if (node.Next[index] == null)
                        node.Next[index] = new Trie();
                    node = node.Next[index];
                }
                node.IsEnd = true;
                node.Word = word;
            }

            /* Returns if the word is in the trie. */
            public bool Search(string word)
            {
                Trie node = SearchPrefix(word);
                return node != null && node.IsEnd;
            }

            /* Returns if there is any word in the trie that starts with the given prefix. */
            public bool StartsWith(string prefix)
            {
                Trie node = SearchPrefix(prefix);
                return node != null;
            }

            private Trie SearchPrefix(string word)
            {
                Trie node = this;
                foreach (var ch in word)
                {
                    node = node.Next[ch - 'a'];
                    if (node == null)
                        return null;
                }
                return node;
            }

            public bool IsLeaf()
            {
                foreach (Trie sub in this.Next)
                {
                    if (sub != null)
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region 最小栈
        public class MinStack
        {

            /** initialize your data structure here. */
            Stack<int> stack;
            int min = int.MaxValue;
            //int min = 0;
            //Stack<int> minStack;
            public MinStack()
            {
                //M1. double Stack
                //M2. One Stack
                //M3. Node

                stack = new Stack<int>();
                //minStack=new Stack<int>();
            }

            public void Push(int x)
            {
                //stack.Push(x);
                // if(minStack.Any())
                //     minStack.Push(Math.Min(x,minStack.Peek()));
                // else
                //     minStack.Push(x);

                // if(minStack.Count==0 || minStack.Peek()>=x)
                //     minStack.Push(x);

                if (x <= min)
                {
                    stack.Push(min);
                    min = x;
                }
                stack.Push(x);
            }

            public void Pop()
            {
                //stack.Pop();
                //minStack.Pop();

                // if(stack.Pop()==minStack.Peek())
                //     minStack.Pop();

                if (stack.Pop() == min)
                    min = stack.Pop();

            }

            public int Top()
            {
                return stack.Peek();
            }

            public int GetMin()
            {
                //return minStack.Peek();
                return min;
            }
        }
        #endregion
    }
}
