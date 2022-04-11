/*
 * @lc app=leetcode.cn id=429 lang=csharp
 *
 * [429] N叉树的层序遍历
 */

// @lc code=start
/*
// Definition for a Node.
public class Node {
    public int val;
    public IList<Node> children;

    public Node() {}

    public Node(int _val) {
        val = _val;
    }

    public Node(int _val, IList<Node> _children) {
        val = _val;
        children = _children;
    }
}
*/

public class Solution {
    List<IList<int>> result=new List<IList<int>>();
    public IList<IList<int>> LevelOrder(Node root) {
        //M1:Recursion. O(n), O(log n), O(n)
        if(root!=null)
            TraverseNode(root,0);
        return result;

        //M2:Iteration.O(n), O(n)
        if(root==null) return result;
        // Queue<Node> queue = new Queue<Node>();
        // queue.Enqueue(root);

        // while (queue.Any())
        // {
        //     var size = queue.Count;
        //     var templist = new List<int>();
        //     for (int i = 0; i < size; i++)
        //     {
        //         var cur = queue.Dequeue();
        //         templist.Add(cur.val);
        //         if (cur.children != null)
        //         {
        //             foreach (var child in cur.children)
        //                 queue.Enqueue(child);
        //         }

        //     }
        //     result.Add(templist);
        // }
        // return result;

        //M3:O(n), O(n)
        // List<Node> previousLayer=new List<Node>();
        // previousLayer.Add(root);
        // while (previousLayer.Any())
        // {
        //     List<Node> currentLayer=new List<Node>();
        //     List<int> previousVals=new List<int> ();
        //     previousLayer.ForEach(node=>{
        //         previousVals.Add(node.val);
        //         if(node.children!=null)
        //             currentLayer.AddRange(node.children);
        //     });
        //     result.Add(previousVals);
        //     previousLayer=currentLayer;
        // }
        // return result;
    }

    private void TraverseNode(Node node, int level){
        if(result.Count<=level)
            result.Add(new List<int>());
        result[level].Add(node.val);
        foreach(Node child in node.children)
            TraverseNode(child, level+1);
    }
}
// @lc code=end

