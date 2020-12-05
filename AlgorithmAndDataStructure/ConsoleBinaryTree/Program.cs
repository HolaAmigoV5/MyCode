using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleBinaryTree
{
    public class Program
    {
        static void Main(string[] args)
        {
            TreeNode tree = CreatFakeTree();
            //TreeNode tree = new TreeNode("A", new TreeNode("B"), new TreeNode("C"));
            //PreOrder(tree);
            //Console.WriteLine("\r\n");
            //PreOrderNoRecursion(tree);
            //Console.WriteLine("\r\n");
            /*Console.WriteLine("递归中序遍历：");
            InOrder(tree);
            Console.WriteLine();
            Console.WriteLine("非递归中序遍历");
            InOrderNoRecursion(tree);*/
            //PreOrderNoRecursion(tree);
            //PostOrderNoRecursion(tree);
            //LevelOrderNoRecursion(tree);
            var simpleTree = CreatSimpleTree();
            //PreOrderNoRecursion(simpleTree);
            //InOrderNoRecursion(simpleTree);
            PostOrderNoRecursion(simpleTree);
            Console.ReadLine();
        }

        public static TreeNode CreatFakeTree()
        {
            TreeNode tree = new TreeNode("A");
            tree.left = new TreeNode("B")
            {
                left = new TreeNode("D") { left = new TreeNode("G") },
                right = new TreeNode("E") { right = new TreeNode("H") }
            };
            tree.right = new TreeNode("C") { right = new TreeNode("F") };
            return tree;
        }

        public static TreeNode CreatSimpleTree()
        {
            TreeNode tree = new TreeNode("A");
            tree.left = new TreeNode("B");
            tree.right = new TreeNode("C");
            return tree;
        }


        /// <summary>
        /// 前序递归遍历
        /// </summary>
        /// <param name="node"></param>
        public static void PreOrder(TreeNode node)
        {
            if (node == null) return;

            Console.Write(node.val);
            PreOrder(node.left);
            PreOrder(node.right);
        }

        /// <summary>
        /// 前序迭代遍历
        /// </summary>
        /// <param name="node"></param>
        public static void PreOrderNoRecursion(TreeNode node)
        {
            if (node == null) return;

            //根->左->右
            Stack<TreeNode> stack = new Stack<TreeNode>();
            stack.Push(node);

            /*while (stack.Any())
            {
                //遍历根节点
                TreeNode tempNode = stack.Pop();
                Console.Write(tempNode.val);

                //右子树压栈
                if (tempNode.right != null)
                    stack.Push(tempNode.right);

                //左子树压栈
                if (tempNode.left != null)
                    stack.Push(tempNode.left);
            }*/

            //M2:统一模板。入栈顺序：右->左->根
            while (stack.Any())
            {
                var tempNode = stack.Pop();  //弹出节点并判断是否访问过
                //非空说明没有访问过，然后右节点入栈，左节点入栈，最后根节点入栈，并入栈一个空节点，表明当前节点已经访问过
                if (tempNode != null)
                {
                    if (tempNode.right != null)
                        stack.Push(tempNode.right);
                    if (tempNode.left != null)
                        stack.Push(tempNode.left);
                    stack.Push(tempNode);
                    stack.Push(null);
                }
                else
                    Console.Write(stack.Pop().val); //如果弹出节点为空节点，表明当前栈顶点已经访问过
            }
        }

        /// <summary>
        /// 中序递归遍历
        /// </summary>
        /// <param name="node"></param>
        public static void InOrder(TreeNode node)
        {
            if (node == null) return;
            InOrder(node.left);
            Console.Write(node.val);
            InOrder(node.right);
        }

        /// <summary>
        /// 中序迭代遍历
        /// </summary>
        /// <param name="node"></param>
        public static void InOrderNoRecursion(TreeNode node)
        {
            if (node == null) return;
            //左->根->右
            Stack<TreeNode> stack = new Stack<TreeNode>();
            /*while (node != null || stack.Any())
            {
                //All the left nodes push into the stack.
                if (node != null)
                {
                    //依次将所有左子树节点压栈
                    stack.Push(node);
                    node = node.left;
                }
                else
                {
                    //出栈遍历节点
                    var temp = stack.Pop();
                    Console.Write(temp.val);

                    //左子树遍历结束跳转到右子树
                    node = temp.right;
                }
            }*/

            //M2:统一模板。入栈顺序：右->根->左
            stack.Push(node);  //先将根节点入栈
            while (stack.Any())
            {
                var temp = stack.Pop();
                if (temp != null)
                {
                    if (temp.right != null)
                        stack.Push(temp.right);
                    stack.Push(temp); //在左节点之前重新插入该节点，以便在左节点之后处理
                    stack.Push(null); //空节点随之入栈，标识已经访问过，但还没有处理
                    if (temp.left != null)
                        stack.Push(temp.left);
                }
                else
                    Console.Write(stack.Pop().val);
            }
        }

        /// <summary>
        /// 后续递归遍历
        /// </summary>
        /// <param name="node"></param>
        public static void PostOrder(TreeNode node)
        {
            if (node == null) return;
            PostOrder(node.left);
            PostOrder(node.right);
            Console.Write(node.val);
        }

        /// <summary>
        /// 后续迭代遍历
        /// </summary>
        /// <param name="node"></param>
        public static void PostOrderNoRecursion(TreeNode node)
        {
            if (node == null) return;

            //两个栈，一个存储，一个输出
            Stack<TreeNode> stackIn = new Stack<TreeNode>();
            //Stack<TreeNode> stackOut = new Stack<TreeNode>();

            //根节点首先压栈
            stackIn.Push(node);

            //左->右->根
            /*while (stackIn.Any())
            {
                TreeNode curNode = stackIn.Pop();
                stackOut.Push(curNode);
                //左子树压栈
                if (curNode.left != null)
                    stackIn.Push(curNode.left);
                //右子树压栈
                if (curNode.right != null)
                    stackIn.Push(curNode.right);
            }

            while (stackOut.Any())
            {
                //依次遍历各节点
                TreeNode outNode = stackOut.Pop();
                Console.Write(outNode.val);
            }*/

            //入栈：根->右->左
            while (stackIn.Any())
            {
                var temp = stackIn.Pop();
                if (temp != null)
                {
                    stackIn.Push(temp);
                    stackIn.Push(null);
                    if (temp.right != null)
                        stackIn.Push(temp.right);
                    if (temp.left != null)
                        stackIn.Push(temp.left);
                }
                else
                    Console.Write(stackIn.Pop().val);
            }
        }

        /// <summary>
        /// 层序迭代遍历
        /// </summary>
        /// <param name="node"></param>
        public static void LevelOrderNoRecursion(TreeNode node)
        {
            if (node == null) return;
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(node);    //队尾入列
            while (queue.Any())
            {
                var item = queue.Dequeue(); //队头出列
                Console.Write(item.val);
                if (item.left != null)
                    queue.Enqueue(item.left);
                if (item.right != null)
                    queue.Enqueue(item.right);
            }
        }
    }

    public class TreeNode
    {
        public string val = string.Empty;
        public TreeNode left = null;
        public TreeNode right = null;
        public TreeNode(string  value)
        {
            this.val = value;
        }
        public TreeNode(string val, TreeNode left, TreeNode right)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }
}
