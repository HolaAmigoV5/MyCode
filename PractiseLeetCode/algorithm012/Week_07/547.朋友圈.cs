/*
 * @lc app=leetcode.cn id=547 lang=csharp
 *
 * [547] 朋友圈
 */

// @lc code=start
public class Solution {
    public int FindCircleNum(int[][] M)
    {
        if(M==null || M.Length==0) return 0;
        int len = M.Length;

        //M1: Disjoint Set
        // UnionFind uf = new UnionFind(len);
        // for (int i = 0; i < len; i++)
        // {
        //     for (int j = 0; j < i; j++)
        //     {
        //         if (M[i][j] == 1)
        //             uf.Union(i, j);
        //     }
        // }
        // return uf.Count();

        //DFS && BFS
        var visited=new bool[len];
        int count=0;
        var queue=new Queue<int>();

        for(int i=0; i<len; i++){
            if(!visited[i]){
                count++;
                //DFS
                //DFS(M,i, visited);

                //BFS
                queue.Enqueue(i);
                while (queue.Any())
                {
                    var cur = queue.Dequeue();
                    for (int j = 0; j < len; j++)
                    {
                        if (M[cur][j] == 1 && !visited[j]){
                            queue.Enqueue(j);
                            visited[j]=true;
                        }
                            
                    }
                }
            }
        }
        return count;
    }

    private void DFS(int[][] M, int i, int[] visited){
        for(int j=0; j<M.Length; j++){
            if(M[i][j]==1 && visited[j]==0){
                visited[j]=1;
                DFS(M, j, visited);
            }
                
        }
    }

    class UnionFind
    {
        private int count; //记录连通分量
        private int[] parent; //记录每个节点的父节点
        private int[] size; //记录树的重量

        //构造，n为图的节点总数
        public UnionFind(int n)
        {
            this.count = n; //一开始互不联通
            parent = new int[n];  //父节点指针初始指向自己
            size = new int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
        }

        //将p和q连接
        public void Union(int p, int q)
        {
            int rootP = Find(p);
            int rootQ = Find(q);
            if (rootP == rootQ)
                return;

            //小树接到大树下面，较平衡
            if (size[rootP] > size[rootQ])
            {
                parent[rootQ] = rootP;
                size[rootP] += size[rootQ];
            }
            else
            {
                parent[rootP] = rootQ;
                size[rootQ] += size[rootP];
            }
            count--;
        }

        //判断p和q是否连通
        public bool Connected(int p, int q)
        {
            int rootP = Find(p);
            int rootQ = Find(q);
            return rootP == rootQ;
        }

        //返回某个节点x的根节点,O(n)
        public int Find(int x)
        {
            while (parent[x] != x)
            {
                parent[x] = parent[parent[x]];  //进行路径压缩
                x = parent[x];
            }
            return x;
        }

        //返回图中有多少个连通分量
        public int Count()
        {
            return count;
        }
    }
}
// @lc code=end

