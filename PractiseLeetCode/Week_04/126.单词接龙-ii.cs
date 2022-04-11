/*
 * @lc app=leetcode.cn id=126 lang=csharp
 *
 * [126] 单词接龙 II
 */

// @lc code=start
public class Solution
{
    List<IList<string>> res = new List<IList<string>>();
    string end = string.Empty;
    string begin = string.Empty;
    HashSet<string> wordSet = null;
    Dictionary<string, List<string>> nodeNeighbors = new Dictionary<string, List<string>>();

    public IList<IList<string>> FindLadders(string beginWord, string endWord, IList<string> wordList)
    {
        wordSet = new HashSet<string>(wordList);
        if (wordSet.Count == 0 || !wordSet.Contains(endWord))
            return res;

        //wordSet.Remove(begin);
        begin = beginWord;
        end = endWord;

        if(!BFS()) 
            return res;
        // if(!TwoEndedBFS())
        //     return res;
        DFS(begin, new List<string>());
        return res;
    }

    private bool TwoEndedBFS(){
        var beginVisited=new HashSet<string>(){begin};
        var endVisited=new HashSet<string>(){end};
        var visited=new HashSet<string>();
        visited.Add(begin);
        visited.Add(end);

        bool isFound=false;
        bool IsSwop=false;
        while(beginVisited.Any()){
            //swop
            if(beginVisited.Count>endVisited.Count){
                var tmp=beginVisited;
                beginVisited=endVisited;
                endVisited=tmp;
                IsSwop=!IsSwop;
            }
            var nextVisited=new HashSet<string>();
            foreach(var str in beginVisited){
                var neighbors=GetNeighbors(str);
                foreach(var n in neighbors){
                    if(endVisited.Contains(n)){
                        isFound=true;
                        AddToSuccessors(IsSwop, str, n);
                    }
                    if(!visited.Contains(n)){
                        nextVisited.Add(n);
                        AddToSuccessors(IsSwop, str, n);
                    }
                }
            }
            beginVisited=nextVisited;
            visited.UnionWith(nextVisited);
            if(isFound)
                break;
        }
        return isFound;
    }

    private void AddToSuccessors(bool IsSwop, string cur, string next){
        if(IsSwop){
            var tmp=cur;
            cur=next;
            next=tmp;
        }
        if(!nodeNeighbors.ContainsKey(cur))
            nodeNeighbors[cur]=new List<string>();
        nodeNeighbors[cur].Add(next);
    }

    private bool BFS()
    {
        var queue = new Queue<string>();
        queue.Enqueue(begin);
        var visited = new HashSet<string>(){begin};
        var toVisited = new HashSet<string>();
        toVisited.Add(begin);
        bool isFound = false;

        while (queue.Any())
        {
            // foreach (var item in toVisited)
            //     visited.Add(item);
            visited.UnionWith(toVisited);
            toVisited.Clear();

            int size = queue.Count;
            while (size-- > 0)
            {
                var cur = queue.Dequeue();
                if (cur.Equals(end))
                    isFound = true;
                var neighbors = GetNeighbors(cur);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        if (!nodeNeighbors.ContainsKey(cur))
                            nodeNeighbors[cur] = new List<string>();
                        nodeNeighbors[cur].Add(neighbor);

                        if (!toVisited.Contains(neighbor))
                        {
                            toVisited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }

                        // if(!visited.Contains(neighbor)){
                        //     visited.Add(neighbor);
                        //     queue.Enqueue(neighbor);
                        // }
                    }
                }
            }
            if (isFound)
                break;
        }
        return isFound;
    }

    //DFS:output all paths with the shortest distance
    private void DFS(string cur, List<string> path)
    {
        path.Add(cur);
        if (end.Equals(cur))
            res.Add(new List<string>(path));
        else if (nodeNeighbors.ContainsKey(cur))
        {
            foreach (var next in nodeNeighbors[cur])
                DFS(next, path);
        }
        path.RemoveAt(path.Count - 1);
    }

    private List<string> GetNeighbors(string str)
    {
        var ans = new List<string>();
        if(string.IsNullOrEmpty(str))
            return ans;
        char[] chars = str.ToCharArray();
        for (int i = 0; i < str.Length; i++)
        {
            char old = str[i];
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                if (ch == old)
                    continue;
                chars[i] = ch;
                var newstr = new string(chars);
                if (wordSet.Contains(newstr))
                    ans.Add(newstr);
            }
            chars[i] = old;
        }
        return ans;
    }
}
// @lc code=end

