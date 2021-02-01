/*
 * @lc app=leetcode.cn id=22 lang=csharp
 *
 * [22] 括号生成
 */

// @lc code=start
public class Solution {
    List<string> res=new List<string>();
    public IList<string> GenerateParenthesis(int n) {
        if(n==0)
            return res;
        // DFS(n, n, "");
        // return res;

        //BFS
        // var queue=new Queue<Node>();
        // queue.Enqueue(new Node("",n,n));
        // while(queue.Any()){
        //     var node=queue.Dequeue();
        //     if(node.left==0&& node.right==0)
        //         res.Add(node.str);
        //     if(node.left>0)
        //         queue.Enqueue(new Node(node.str+"(", node.left-1, node.right));
        //     if(node.right>node.left)
        //         queue.Enqueue(new Node(node.str+")", node.left, node.right-1));
        // }
        // return res;

        //Dp
        var dp=new List<List<string>>();
        var dp0=new List<string>();
        dp0.Add("");
        dp.Add(dp0);

        for(int i=1; i<=n; i++){
            var cur=new List<string>();
            for(int j=0; j<i; j++){
                var str1=dp[j];
                var str2=dp[i-1-j];
                foreach(var s1 in str1){
                    foreach(var s2 in str2)
                        cur.Add("("+s1+")"+s2);
                }
            }
            dp.Add(cur);
        }
        return dp[n];
    }

    private void DFS(int left, int right, string curStr){
        if(left==0 && right==0){
            res.Add(curStr);
            return;
        }

        if(left>0)
            DFS(left-1, right, curStr+"(");
        if(right>left)
            DFS(left, right-1, curStr+")");
    }

    class Node{
        public string str;
        public int left;
        public int right;
        public Node(string str, int left, int right){
            this.str=str;
            this.left=left;
            this.right=right;
        }
    }
}
// @lc code=end

