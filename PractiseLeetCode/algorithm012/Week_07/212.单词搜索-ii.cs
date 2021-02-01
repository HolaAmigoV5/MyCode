/*
 * @lc app=leetcode.cn id=212 lang=csharp
 *
 * [212] 单词搜索 II
 */

// @lc code=start
public class Solution {
    List<string> res=new List<string>();
    char[][] _board=null;
    public IList<string> FindWords(char[][] board, string[] words) {
        if(board.Length==0 || board[0].Length==0 || words.Length==0)
            return res;
        this._board=board;

        //build Trie Tree
        Trie root=new Trie();
        foreach(var w in words)
            root.Insert(w);
        
        //begin to DFS
        for(int i=0; i<board.Length; i++){
            for(int j=0; j<board[0].Length; j++){
                DFS(i,j, root);
            } 
        }
        return res;
    }

    private void DFS(int x, int y, Trie p){
        if(!Legal(x,y) || p.Next[_board[x][y]-'a']==null)
            return;

        char letter=_board[x][y];
        Trie cur=p.Next[letter-'a'];

        if(cur.Word!=null){
            res.Add(cur.Word);
            cur.Word=null;
        }

        _board[x][y]='#';
        for(int i=0; i<4; i++){
            DFS(x+(i==1?1:i==3?-1:0), y+(i==0?1:i==2?-1:0),cur);
        }
        _board[x][y]=letter;
    }

    private bool Legal(int x, int y){
        return x>=0 && x<_board.Length && y>=0 && y<_board[0].Length && _board[x][y]!='#';
    }

    public class Trie
    {
        public Trie[] Next;
        public string Word;

        public Trie(){
            Next = new Trie[26];
        }

        //Insert a word into the trie.
        public void Insert(string word){
            if(string.IsNullOrEmpty(word))
                return;

            Trie node=this;
            foreach(var ch in word){
                int index=ch-'a';
                if(node.Next[index]==null)
                    node.Next[index]=new Trie();
                node = node.Next[index];
            }
            node.Word=word;
        }
    }
}


// @lc code=end

