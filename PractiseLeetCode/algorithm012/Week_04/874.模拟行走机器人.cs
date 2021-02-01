/*
 * @lc app=leetcode.cn id=874 lang=csharp
 *
 * [874] 模拟行走机器人
 */

// @lc code=start
public class Solution{
    public int RobotSim(int[] commands, int[][] obstacles){
        if(commands==null || commands.Length==0)
            return 0;

        //O(m+k), O(k) m is the num. of 'commands', k is the nums. of 'obstacles'
        int x=0, y=0, direction=0, ans=0;
        var obstacleSet=new HashSet<int>();
        foreach(var obstrcle in obstacles)
            obstacleSet.Add(GetHashCode(obstrcle[0], obstrcle[1]));
        
        foreach(var command in commands){
            if(command<0){
                direction=(direction+(command==-1?1:3))%4;
                continue;
            }
            var step=command;
            while(step-->0){
                var newx=x+(direction==1?1:direction==3?-1:0);
                var newy=y+(direction==0?1:direction==2?-1:0);
                if(obstacleSet.Contains(GetHashCode(newx,newy)))
                    break;
                x=newx; y=newy;
                    
            }
            ans=Math.Max(ans, x*x+y*y);
        }
        return ans;
    }

    private int GetHashCode(int x, int y){
        return ((x+30000)<<16)+ y+30000;
    }
}
// @lc code=end

