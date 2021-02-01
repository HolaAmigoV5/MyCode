/*
 * @lc app=leetcode.cn id=297 lang=csharp
 *
 * [297] 二叉树的序列化与反序列化
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
public class Codec {

    // Encodes a tree to a single string.
    //TreeNode d;
    public string serialize(TreeNode root) {
        // d=root;
        // return " ";

        //DFS
        // var sb= new StringBuilder();
        // ToSerialize(root, sb);
        //sb.Remove(sb.Length-1,1);  //remove the last ','
        // return sb.ToString();

        //BFS
        if(root==null)
            return "";
        var sb=new StringBuilder();
        var queue=new Queue<TreeNode>();
        queue.Enqueue(root);
        while(queue.Any()){
            var node=queue.Dequeue();
            if(node==null)
                sb.Append("#,");
            else{
                sb.Append($"{node.val},");
                queue.Enqueue(node.left);
                queue.Enqueue(node.right);
            }
        }
        sb.Remove(sb.Length-1,1);  //remove the last ','
        return sb.ToString();
    }

    private void ToSerialize(TreeNode node, StringBuilder sb){
        if(node==null){
            sb.Append("#,");
            return;
        }
        
        sb.Append($"{node.val},");
        ToSerialize(node.left, sb);
        ToSerialize(node.right, sb);
    }

    // Decodes your encoded data to tree.
    public TreeNode deserialize(string data) {
        //return d;
        if(string.IsNullOrEmpty(data))
            return null;

        //DFS
        // Queue<string> queue = new Queue<string>();
        // foreach (var node in data.Split(',')){
        //     //Console.WriteLine(node);
        //     queue.Enqueue(node);
        // }
        // return Deserialize(queue);

        //BFS
        var list=data.Split(',');
        var root=new TreeNode(int.Parse(list[0]));
        var queue=new Queue<TreeNode>();
        queue.Enqueue(root);

        int index=1;
        while(queue.Any()){
            var node=queue.Dequeue();

            if(index<list.Length){
                node.left=list[index]=="#"?null:new TreeNode(int.Parse(list[index]));
                if(node.left!=null)
                    queue.Enqueue(node.left);
                index++;
            }

            if(index<list.Length){
                node.right=list[index]=="#"?null:new TreeNode(int.Parse(list[index]));
                if(node.right!=null)
                    queue.Enqueue(node.right);
                
                index++;
            }
        }
        return root;

    }
    private TreeNode Deserialize(Queue<string> queue)
    {
        if (queue.Count == 0)
            return null;

        string node = queue.Dequeue();
        if (node == "#")
            return null;

        TreeNode root = new TreeNode(int.Parse(node));
        root.left = Deserialize(queue);
        root.right = Deserialize(queue);

        return root;
    }
}

// Your Codec object will be instantiated and called as such:
// Codec codec = new Codec();
// codec.deserialize(codec.serialize(root));
// @lc code=end

