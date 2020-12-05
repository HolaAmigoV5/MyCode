using System;
using System.Collections.Generic;

namespace ConsoleLeetCodeTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Solution solution = new Solution();
            //var res= solution.Combine(2, 2);
            int[] nums = { -2, 1, -3, 4, -1, 2, 1, -5, 4 };
            //var result = solution.Permute(nums);
            //var ll = solution.PermuteUnique(nums);
            //var re = solution.TopKFrequent(nums, 2);
            ListNode node = GenerateListNode();
            //solution.ReverseList(node);
            //var res=solution.SwapPairs(node);
            //solution.LetterCombinations("2");
            //var num= solution.MyPow(2, -2147483648);
            //solution.SwapPairs(GenerateListNode());
            //solution.Subsets(new int[] { 1, 2, 3 });
            //solution.Permute(new int[] { 1, 2, 3 });
            //solution.SolveNQueens(4);
            //solution.MaxDepth(GenerateBinaryTree());
            //solution.LetterCombinations("234");
            char[][] grid = {
                new char[]{ '1', '1', '1', '1', '0' },
                new char[]{'1', '1', '0', '1', '0'},
                new char[]{'1', '1', '0', '0', '0'},
                new char[]{'0', '0', '0', '0', '0'}};
            //solution.NumIslands(grid);
            int[] commands = { 4, -1, 4, -2, 4 };
            int[][] obstacles = { new int[]{2,4 }
                //new int[] {-2,1},
                //new int[] {0,-1},
                //new int[] {-2,4},
                //new int[] {-1,0},
                //new int[] {-2,-3},
                //new int[] {0,-3},
                //new int[] {4,4},
                //new int[] {-3,3},
                //new int[] {2,2 } 
            };
            //solution.RobotSim(commands, obstacles);
            List<string> wordlist = new List<string>() { "hot", "dot", "dog", "lot", "log", "cog" };
            List<string> wordlist2 = new List<string>() { "hot", "dog", "dot" };
            //solution.LadderLength("hot", "dog", wordlist2);
            //solution.LadderLength2("hit", "cog", wordlist);
            //solution.FindLadders2("hot", "dog", wordlist2);
            //solution.FindLadders("a", "c", new List<string>() { "a", "b", "c" });
            //solution.FindLadders("hit", "cog", wordlist);
            //solution.FindLadders2("red", "tax", new List<string>() { "ted", "tex", "red", "tax", "tad", "den", "rex", "pee" });
            //var res= solution.MyPow(2, -2147483648);
            solution.Rotate(new int[] { 1, 2, 3, 4, 5, 6, 7 }, 3);
            //solution.MoveZeroes(new int[] { 0, 1, 0, 3, 12 });
            //solution.SolveNQueens(4);
            //solution.MajorityElement(new int[] { 2, 2, 1, 1, 1, 2, 2 });
            //solution.CanJump(new int[] { 0, 2, 3 });
            //solution.Jump(new int[] { 2, 3, 1, 1, 4 });
            //solution.FindTarget(new int[] { 2, 3, 4, 5, 6, 7 }, 2);
            //solution.SubarraySum(new int[] { 1, 2, 3 }, 3);
            //solution.RobotSim3(commands, obstacles);
            //solution.MaxSubArray(nums);
            //solution.FindLadders("a", "b", new List<string>() { "a", "b", "c" });
            //solution.LongestCommonSubsequence("abcde", "ace");
            //solution.FindMin(new int[] { 2, 1 });
            //solution.CountSubstrings("abc");
            //solution.CoinChange(new int[] { 1,2, 5 }, 11);
            //solution.MinDistance("horse", "ros");
            char[][] board = {
                new char[] { 'o', 'a', 'a', 'n' },
                new char[]{ 'e', 't', 'a', 'e' },
                new char[]{ 'i', 'h', 'k', 'r' },
                new char[]{ 'i', 'f', 'l', 'v' } };
            string[] words = new string[] { "oath", "pea", "eat", "rain" };
            //var r = solution.FindWords(board, words);
            //solution.Rob(new int[] { 1, 2, 3, 1 });
            //solution.MaxProfit(1, new int[] { 1, 2 });
            //solution.Change(11, new int[] { 1, 2, 5 });
            //solution.UniquePaths(3, 2);
            //solution.LargestValues(GenerateBinaryTree());
            //solution.GenerateParenthesis(3);
            //solution.LeastInterval(new char[] { 'A', 'A', 'C', 'C', 'D', 'D', 'F' }, 2);
            string[] gens = { "AAAAAAAA", "AAAAAAAC", "AAAAAACC", "AAAAACCC", "AAAACCCC", "AACACCCC", "ACCACCCC", "ACCCCCCC", "CCCCCCCA", "CCCCCCCC" };
            //solution.MinMutation("AAAAAAAA", "CCCCCCCC", gens);
            //solution.NumDecodings("226");
            //Solution.MinStack ms = new Solution.MinStack();
            //var min= ms.GetMin();
            //ms.Push(2);
            //ms.Push(0);
            //ms.Push(3);
            //ms.Push(0);
            //min = ms.GetMin();
            //ms.Pop();
            //min = ms.GetMin();
            //ms.Pop();
            //min = ms.GetMin();
            //ms.Pop();
            //min = ms.GetMin();
            //solution.JudgeSquareSum(5);
            //solution.Rob2(new int[] { 2, 3, 2 });
            //Console.WriteLine((char)(1 + '0'));
            //Console.WriteLine('1' - '0');
            //solution.CountBits(2);
            int[] a1 = { 474, 83, 404, 3 };
            int[] a2 = { 2, 1, 4, 3, 9, 6 };
            //solution.RelativeSortArray(a1, a2);
            //solution.ReverseOnlyLetters("a-bC-dEf-ghIj");
            //solution.IsIsomorphic("ab", "aa");
            //solution.NumSquares(12);
            //solution.CoinChange(a1, 264);
            int[][] M = {
                new int[] {1,1,0},
                new int[] {1,1,0},
                new int[] {0,0,1}};
            //solution.FindCircleNum(M);
            //solution.ReverseKGroup(GenerateListNode(), 2);
            //solution.SlidingWindowTemplate("cbaebabacd", "abc");
            //solution.LengthOfLongestSubstring("abcabcbb");
            //solution.ReverseWords("Let's take LeetCode contest");
            //solution.FindAnagrams("cbaebabacd", "abc");
            //solution.RemoveDuplicates(new int[] { 1, 1, 1, 2, 2, 3 });
            //solution.LargestRectangleArea(new int[] { 2, 1, 5, 6, 2, 3 });
            //solution.LengthOfLIS(new int[] { 10, 9, 2, 5, 3, 7, 101, 18 });
            //solution.LengthOfLastWord("a ");
            //solution.LongestCommonPrefix(new string[] { "c", "acc", "ccc" });
            //solution.ReversePairs(new int[] { 12, 10, 1, 3, 5 });
            //solution.SortList(GenerateListNode2());
            var list = new List<IList<string>>();
            list.Add(new List<string>() { "John", "johnsmith@mail.com", "john_newyork@mail.com" });
            list.Add(new List<string>() { "John", "johnsmith@mail.com", "john00@mail.com" });
            list.Add(new List<string>() { "Mary", "mary@mail.com" });
            list.Add(new List<string>() { "John", "johnnybravo@mail.com" });

            //solution.AccountsMerge(list);
            //var points = new int[2][];
            //points[0] = new int[2] { 0, 1 };
            //points[1] = new int[2] { 1, 0 };
            //solution.KClosest(points, 2);

            Console.ReadLine();
        }

        #region Node
        private static TreeNode GenerateBinaryTree()
        {
            TreeNode root = new TreeNode(1);
            root.left = new TreeNode(2);
            root.right = new TreeNode(3, new TreeNode(6));
            return root;
        }

        private static Node GenerateNodeTree()
        {
            Node root = new Node(1);
            List<Node> nodes = new List<Node>() {
                new Node(2,new List<Node>(){new Node(5),new Node(6) }),new Node(3, new List<Node>(){ }), new Node(4)
            };
            root.children = nodes;
            return root;
        }

        private static ListNode GenerateListNode()
        {
            ListNode listNode = new ListNode(1);
            ListNode head = listNode;

            for (int i = 1; i < 5; i++)
            {
                listNode.next = new ListNode(i + 1);
                listNode = listNode.next;
            }
            return head;
        }
        private static ListNode GenerateListNode2()
        {
            ListNode head = new ListNode(0);
            ListNode listNode = new ListNode(4);
            head.next = listNode;
            listNode.next = new ListNode(2);
            listNode.next.next = new ListNode(1);
            listNode.next.next.next = new ListNode(3);
            return head.next;
        }
        private static char[][] BuildCharGrid(int m, int n)
        {
            char[][] chargrid = new char[m][];
            for(int i=0;i<m; i++)
            {
                chargrid[i] = new char[n];
                for(int j=0; j<n; j++)
                {
                    chargrid[i][j] = '1';
                }
            }
            return chargrid;
        }
        #endregion
    }

    public class Node
    {
        public int val;
        public IList<Node> children;

        public Node() { }

        public Node(int _val)
        {
            val = _val;
        }

        public Node(int _val, IList<Node> _children)
        {
            val = _val;
            children = _children;
        }
    }

    public class TreeNode
    {
        public int val = 0;
        public TreeNode left = null;
        public TreeNode right = null;
        public TreeNode(int value)
        {
            this.val = value;
        }
        public TreeNode(int val=0, TreeNode left=null, TreeNode right=null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }

    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int x) { val = x; }
    }

    public class MineNode
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MineNode(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
