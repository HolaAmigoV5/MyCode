/*
 * @lc app=leetcode.cn id=84 lang=csharp
 *
 * [84] 柱状图中最大的矩形
 */

// @lc code=start
public class Solution {
    public int LargestRectangleArea(int[] heights) {
        if(heights==null || heights.Length==0)
            return 0;
        
        int len=heights.Length, area=0;

        //M1:中心扩散
        // for(int i=0; i<len; i++){
        //     int left=i, right=i;
        //     while(left>=0 && heights[left]>=heights[i])
        //         left--;
        //     while(right<len && heights[right]>=heights[i])
        //         right++;
            
        //     area=Math.Max(area, (right-left-1)*heights[i]);
        // }
        // return area;

        //M2:单调递增栈。扩展当前柱体左右两边低的柱体，栈记录低的柱体。
        int[] tmp=new int[len+2];
        Array.Copy(heights,0,tmp,1,len);
        var stack=new Stack<int>();
        for(int i=0; i<len+2; i++){
            while(stack.Count>0 && tmp[stack.Peek()]>tmp[i]){
                var h=tmp[stack.Pop()];
                area=Math.Max(area, (i-stack.Peek()-1)*h);
            }
            stack.Push(i);
        }
        return area;
    }
}
// @lc code=end

