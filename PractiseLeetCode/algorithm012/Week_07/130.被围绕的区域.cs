/*
 * @lc app=leetcode.cn id=130 lang=csharp
 *
 * [130] 被围绕的区域
 */

// @lc code=start
public class Solution {
    int m=0,n=0;
    public void Solve(char[][] board) {
        if(board==null || board.Length==0)
            return;

        m=board.Length;
        n=board[0].Length;

        UnionFind uf=new UnionFind(m*n+1);
        int dummy=m*n;

        for(int i=0; i<m; i++){
            for(int j=0; j<n; j++){
                //bool isEdge=(i==0 || j==0 || i==m-1 || j==n-1);
                // if(isEdge && board[i][j]=='O'){
                //     //将与O相连的O全部变为#
                //     //DFS(board, i,j);

                //      //BFS
                //     var queue=new Queue<int[]>();
                //     queue.Enqueue(new int[]{i,j});
                //     while(queue.Count>0){
                //         var p=queue.Dequeue();
                //         if(p[0]>=0 && p[0]<m && p[1]>=0 && p[1]<n && board[p[0]][p[1]]=='O'){
                //             board[p[0]][p[1]]='#';
                //             for(int k=0; k<4; k++){
                //                 var x=p[0]+(k==1?1:k==3?-1:0);
                //                 var y=p[1]+(k==0?1:k==2?-1:0);
                //                 if(x>=0 && x<m && y>=0 && y<n && board[x][y]=='O')
                //                     queue.Enqueue(new int[]{x,y});
                //             }
                //         }
                //     }
                // }

                 //Disjoint Set
                if(board[i][j]=='O'){
                    if(i==0 || j==0 || i==m-1 || j==n-1){
                        uf.Union(i*n+j,dummy);
                    }
                    else{
                        for(int k=0; k<4; k++){
                            var x=i+(k==1?1:k==3?-1:0);
                            var y=j+(k==0?1:k==2?-1:0);
                            if(x>=0 && x<m && y>=0 && y<n && board[x][y]=='O')
                                uf.Union(i*n+j, x*n+y);
                        }
                    }
                }
            }
        }

        //重新全部遍历，#变为O, O变为X
        // for(int i=0; i<m; i++){
        //     for(int j=0; j<n; j++){
        //         if(board[i][j]=='O')
        //             board[i][j]='X';
        //         else if(board[i][j]=='#')
        //             board[i][j]='O';
        //     }
        // }

        
        for(int i=0; i<m; i++){
            for(int j=0; j<n; j++){
                if(!uf.IsConnected(i*n+j, dummy))
                    board[i][j]='X';
            }
        }
    }

    private void DFS(char[][] board, int x, int y){
        if(x<0 || x>=board.Length || y<0 || y>=board[0].Length || 
            board[x][y]=='#' || board[x][y]=='X')
            return;
        
        //Console.WriteLine($"x={x}, y={y}");
        board[x][y]='#';

        for(int i=0; i<4; i++){
            DFS(board, x+(i==1?1:i==3?-1:0), y+(i==0?1:i==2?-1:0));
        }
    }

    class UnionFind{
        int[] parent;
        public UnionFind(int n){
            parent=new int[n];
            for(int i=0; i<n; i++)
                parent[i]=i;
        }

        public void Union(int p, int q){
            var rootP=Find(p);
            var rootQ=Find(q);
            if(rootP!=rootQ)
                parent[rootP]=rootQ;
        }

        public int Find(int x){
            while(parent[x]!=x){
                parent[x]=parent[parent[x]];
                x=parent[x];
            }
            return x;
        }

        public bool IsConnected(int p, int q){
            return Find(p)==Find(q);
        }
    }
}
// @lc code=end

