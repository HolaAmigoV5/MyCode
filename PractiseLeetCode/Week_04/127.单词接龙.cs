/*
 * @lc app=leetcode.cn id=127 lang=csharp
 *
 * [127] 单词接龙
 */

// @lc code=start
public class Solution {
    public int LadderLength(string beginWord, string endWord, IList<string> wordList) {
        //O(mn), O(mn)
        var wordSet=new HashSet<string>(wordList);
        if(wordSet.Count==0|| !wordSet.Contains(endWord))
            return 0;

        //BFS
        // bool[] visited=new bool[wordList.Count];
        // int idx=wordList.IndexOf(beginWord);
        // if(idx!=-1)
        //     visited[idx]=true;
        // Queue<string> queue=new Queue<string>();
        // queue.Enqueue(beginWord);

        // int count=0;
        // while(queue.Any()){
        //     int size=queue.Count;
        //     count++;
        //     while(size-->0){
        //         string start=queue.Dequeue();
        //         if(start.Equals(endWord))
        //             return count;
        //         var neighbors=GetNeighbors(start,wordSet);
        //         foreach(var neighbor in neighbors)
        //             queue.Enqueue(neighbor);

        //         // for(int i=0; i<wordList.Count; i++){
        //         //     if(visited[i])
        //         //         continue;
        //         //     string str=wordList[i];
        //         //     if(!CanConvert(start, str))
        //         //         continue;
        //         //     if(str.Equals(endWord))
        //         //         return count+1;
        //         //     visited[i]=true;
        //         //     queue.Enqueue(str);
        //         // }
        //     }
        // }
        // return 0;

        //two-ended BFS
        var beginVisited=new HashSet<string>();
        beginVisited.Add(beginWord);
        var endVisited=new HashSet<string>();
        endVisited.Add(endWord);
        var visited=new HashSet<string>();
        visited.Add(beginWord);
        visited.Add(endWord);

        int step=0;
        while(beginVisited.Any()){
            step++;
            //choose the smallest one
            if(beginVisited.Count>endVisited.Count){
                //change
                var temp=beginVisited;
                beginVisited=endVisited;
                endVisited=temp;
            }

            var nextLevelVisited=new HashSet<string>();
            foreach(var word in beginVisited){
                var neighbors=GetNeighbors(word, wordSet);
                foreach(var neighbor in neighbors){
                    if(endVisited.Contains(neighbor))
                        return step+1;
                    if(!visited.Contains(neighbor)){
                        //visited.Add(neighbor);
                        nextLevelVisited.Add(neighbor);
                    }
                }
                beginVisited=nextLevelVisited;
                visited.UnionWith(nextLevelVisited);
            }
        }
        return 0;
    }

    private bool CanConvert(string s1, string s2){
        int count=0;
        for(int i=0; i<s1.Length; i++){
            if(s1[i]!=s2[i]){
                count++;
                if(count>1)
                    return false;
            }
        }
        return count==1;
    }

    private List<string> GetNeighbors(string str, HashSet<string> wordSet){
        var neighbors=new List<string>();
        char[] chars=str.ToCharArray();
        for(int i=0; i<str.Length; i++){
            var old= chars[i];
            for(char ch='a'; ch<='z'; ch++){
                if(ch==old)
                    continue;
                chars[i]=ch;
                var newstr=new string(chars);
                // if(wordSet.Remove(newstr))
                //     neighbors.Add(newstr);

                if(wordSet.Contains(newstr))
                    neighbors.Add(newstr);
            }
            chars[i]=old;
        }
        return neighbors;
    }
}
// @lc code=end

