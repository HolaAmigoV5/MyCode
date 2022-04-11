/*
 * @lc app=leetcode.cn id=589 lang=csharp
 *
 * [589] N叉树的前序遍历
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

    public Node(int _val,IList<Node> _children) {
        val = _val;
        children = _children;
    }
}
*/

public class Solution {
    IList<int> res=new List<int>();
    public IList<int> Preorder(Node root) {
        //M1:Recursive
        // if(root==null) return res;
        // res.Add(root.val);
        // foreach(var i in root.children)
        //     Preorder(i);
        // return res;

        //M2:Iteration
        if(root==null) return res;

        var stackTemp = new Stack<Node>();
        stackTemp.Push(root);
        while (stackTemp.Any())
        {
            var node = stackTemp.Pop();
            res.Add(node.val);

            for(int i=node.children!.Count-1; i>=0; i--)
                stackTemp.Push(node.children[i]);
        }
        return res;
    }
}
// @lc code=end

