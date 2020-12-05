using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建二维数组[2004...2009][1...4]
            int[] lowerBounds = { 2005, 1 };
            int[] lengths = { 5, 4 };
            decimal[,] quarterlyRevenue = (decimal[,])Array.CreateInstance(typeof(decimal), lengths, lowerBounds);

            Console.WriteLine("{0,4} {1,9} {2,9} {3,9} {4,9}", "Year", "Q1", "Q2", "Q3", "Q4");

            int firstYear = quarterlyRevenue.GetLowerBound(0);
            int lastYear = quarterlyRevenue.GetUpperBound(0);
            int firstQuarter = quarterlyRevenue.GetLowerBound(1);
            int lastQuarter = quarterlyRevenue.GetUpperBound(1);

            for (int year = firstYear; year <= lastYear; year++)
            {
                Console.Write(year + " ");
                for (int quarter = firstQuarter; quarter <= lastQuarter; quarter++)
                {
                    Console.Write("{0,9:C}", quarterlyRevenue[year, quarter]);
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}