/*
 * @lc app=leetcode.cn id=515 lang=csharp
 *
 * [515] 在每个树行中找最大值
 */

// @lc code=start
/**
 * Definition for a binary tree node.
 * public class TreeNode {
 *     public int val;
 *     public TreeNode left;
 *     public TreeNode right;
 *     public TreeNode(int x) { val = x; }
 * }
 */
public class Solution {
    List<int> res=new List<int>();
    public IList<int> LargestValues(TreeNode root) {
        if(root==null)
            return res;

        //BFS
        // var queue=new Queue<TreeNode>();
        // queue.Enqueue(root);
        // while(queue.Any()){
        //     var size=queue.Count();
        //     var max=int.MinValue;
        //     while(size-->0){
        //         var node=queue.Dequeue();
        //         max=Math.Max(node.val, max);
        //         if(node.left!=null)
        //             queue.Enqueue(node.left);
        //         if(node.right!=null)
        //             queue.Enqueue(node.right);
        //     }
        //     res.Add(max);
        // }
        // return res;

        //DFS
        DFS(root,0);
        return res;
    }

    private void DFS(TreeNode node, int depth){
        if(node==null)
            return;
        if(depth==res.Count)
            res.Add(node.val);
        else
            res[depth]=Math.Max(res[depth],node.val);

        DFS(node.left, depth+1);
        DFS(node.right, depth+1);
    }
}
// @lc code=end

