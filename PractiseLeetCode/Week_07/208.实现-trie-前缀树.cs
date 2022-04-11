/*
 * @lc app=leetcode.cn id=208 lang=csharp
 *
 * [208] 实现 Trie (前缀树)
 */

// @lc code=start
public class Trie {
    // TrieNode root;
    private Trie[] next;
    private bool isEnd=false;
    /** Initialize your data structure here. */
    public Trie() {
        //root=new TrieNode();
        next=new Trie[26];
    }
    
    /** Inserts a word into the trie. */
    public void Insert(string word) {
        if(word==null || word.Length==0) return;
        
        //TrieNode node=root;
        // foreach(char ch in word){
        //     if(node.Next[ch-'a']==null)
        //         node.Next[ch-'a']=new TrieNode();
        //     node=node.Next[ch-'a'];
        // }
        // node.IsEnd=true;

        Trie node=this;
        foreach(var ch in word){
            int index=ch-'a';
            if(node.next[index]==null)
                node.next[index]=new Trie();
            node=node.next[index];
        }
        node.isEnd=true;
    }
    
    /** Returns if the word is in the trie. */
    public bool Search(string word) {
        // TrieNode node=root;
        // foreach(char ch in word){
        //     node=node.Next[ch-'a'];
        //     if(node==null)
        //         return false;
        // }
        // return node.IsEnd;

        // Trie node=this;
        // foreach(var ch in word){
        //     int index=ch-'a';
        //     node=node.next[index];
        //     if(node==null)
        //         return false;
        // }
        // return node.isEnd;

        var node=SearchWord(word);
        return node!=null && node.isEnd;
    }
    
    /** Returns if there is any word in the trie that starts with the given prefix. */
    public bool StartsWith(string prefix) {
        // TrieNode node=root;
        // foreach(char ch in prefix){
        //     node=node.Next[ch-'a'];
        //     if(node==null)
        //         return false;
        // }
        // return true;

        // Trie node=this;
        // foreach (var ch in prefix)
        // {
        //     int index=ch-'a';
        //     node=node.next[index];
        //     if(node==null)
        //         return false;
        // }
        // return true;

        return SearchWord(prefix)!=null;
    }

    private Trie SearchWord(string word){
        var node=this;
        foreach (var ch in word)
        {
            var index=ch-'a';
            node=node.next[index];
            if(node==null)
                return null;
        }
        return node;
    }

    class TrieNode{
        public bool IsEnd;
        public TrieNode[] Next;

        public TrieNode(){
            IsEnd=false;
            Next=new TrieNode[26];
        }
    }
}

/**
 * Your Trie object will be instantiated and called as such:
 * Trie obj = new Trie();
 * obj.Insert(word);
 * bool param_2 = obj.Search(word);
 * bool param_3 = obj.StartsWith(prefix);
 */
// @lc code=end

