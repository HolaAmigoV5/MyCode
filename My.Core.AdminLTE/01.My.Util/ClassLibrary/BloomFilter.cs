using System;
using System.Collections;
using System.Collections.Generic;

namespace My.Util
{
    /// <summary>
    /// 描述：布隆过滤器
    /// 作者：wby 2019/10/18 9:30:57
    /// </summary>
    public class BloomFilter<T>
    {
        #region 成员属性
        Random _random;
        BitArray _bitArray;

        public int BitSize { get; set; }
        public int NumberOfHashes { get; set; }
        public int SetSize { get; set; }

        //计算基于布隆过滤器散列的最佳数量
        private int OptimalNumberOfHashes(int bitSize, int setSize)
        {
            return (int)Math.Ceiling(bitSize / setSize * Math.Log(2.0));
        }

        private int Hash (T item)
        {
            return item.GetHashCode();
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化bloom过滤器并设置hash散列的最佳数目
        /// </summary>
        /// <param name="bitSize">布隆过滤器的大小(m)默认为10E消耗100M内存</param>
        /// <param name="setSize">集合的大小(n)默认为1000W</param>
        public BloomFilter(int bitSize = 1000000000, int setSize = 10000000)
        {
            BitSize = bitSize;
            SetSize = setSize;
            _bitArray = new BitArray(bitSize);
            NumberOfHashes = OptimalNumberOfHashes(bitSize, setSize);
        }

        /// <summary>
        /// 初始化bloom过滤器并设置hash散列的最佳数目
        /// </summary>
        /// <param name="bitSize">布隆过滤器的大小(m)默认为10E消耗100M内存</param>
        /// <param name="setSize">集合的大小(n)默认为1000W</param>
        /// <param name="numberOfHashes">hash散列函数的数量(k)</param>
        public BloomFilter(int bitSize, int setSize, int numberOfHashes)
        {
            _bitArray = new BitArray(bitSize);
            BitSize = bitSize;
            SetSize = setSize;
            NumberOfHashes = numberOfHashes;
        }
        #endregion

        #region 公有方法
        public void Add(T item)
        {
            _random = new Random(Hash(item));
            for (int i = 0; i < NumberOfHashes; i++)
            {
                _bitArray[_random.Next(BitSize)] = true;
            }
        }

        public bool Contains(T item)
        {
            _random = new Random(Hash(item));
            for (int i = 0; i < NumberOfHashes; i++)
            {
                if (!_bitArray[_random.Next(BitSize)])
                    return false;
            }

            return true;
        }

        //检查列表中的任何项是否可能是在集合。
        //如果布隆过滤器包含列表中的任何一项，返回真
        public bool ContainsAny(List<T> items)
        {
            foreach (T item in items)
            {
                if (Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检查列表中的所有项目是否都在集合
        /// </summary>
        /// <param name="items">集合对象</param>
        /// <returns></returns>
        public bool ContainsAll(List<T> items)
        {
            foreach (T item in items)
            {
                if (!Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 计算遇到误检率的概率
        /// </summary>
        /// <returns></returns>
        public double FalsePositiveProbability()
        {
            return Math.Pow(1 - Math.Exp(NumberOfHashes * SetSize / (double)BitSize), NumberOfHashes);
        } 
        #endregion
    }

    /// <summary>
    /// 共享内存布隆过滤器
    /// </summary>
    /// <typeparam name="T">泛型数据类型</typeparam>
    public class BloomFilterWithShareMemory<T>
    {
        #region 成员属性
        Random _random;
        ShareMemory sm;
        public int NumberOfHashes { get; set; }
        public int SetSize { get; set; }
        public int BitSize { get; set; }

        /// <summary>
        /// 计算基于布隆过滤器散列的最佳数量
        /// </summary>
        /// <param name="bitSize">过滤器大小</param>
        /// <param name="setSize">集合大小</param>
        /// <returns></returns>
        private int OptimalNumberOfHashes(int bitSize, int setSize)
        {
            return (int)Math.Ceiling(bitSize / setSize * Math.Log(2.0));
        }

        /// <summary>
        /// 哈希计算
        /// </summary>
        /// <param name="item">哈希对象</param>
        /// <returns></returns>
        private int Hash(T item)
        {
            return item.GetHashCode();
        }
        #endregion

        #region 构造函数
        public BloomFilterWithShareMemory(string bloomName, int _bitSize = 1000000000, int _setSize = 10000000)
        {
            sm = new ShareMemory(bloomName, 1000000000);
            BitSize = _bitSize;
            SetSize = _setSize;
            NumberOfHashes = OptimalNumberOfHashes(_bitSize, _setSize);
        }
        #endregion

        #region 外部方法
        public void Add(T item)
        {
            _random = new Random(Hash(item));

            for (int i = 0; i < NumberOfHashes; i++)
            {
                int index = _random.Next(BitSize);
                int j, offSet = 0;
                if ((index + 1) % 8 == 0)
                    j = (index + 1) / 8 - 1;
                else
                {
                    j = (index + 1) / 8;
                    offSet = (index + 1) % 8 - 1;
                }

                byte[] getData = sm.Read(1, j);

                BitArray bitArray = new BitArray(getData);
                bitArray[offSet] = true;

                int tmp = 0;
                for (int k = 0; k < 8; k++)
                {
                    if (bitArray[k] == true)
                        tmp += (int)Math.Pow(2, k);
                }

                byte[] setData = new byte[1];
                setData[0] = (byte)tmp;
                sm.Write(setData, j);
            }
        }

        public bool Contains(T item)
        {
            _random = new Random(Hash(item));
            for (int i = 0; i < NumberOfHashes; i++)
            {
                int index = _random.Next(BitSize);
                int j, offSet = 0;
                if ((index + 1) % 8 == 0)
                    j = (index + 1) / 8 - 1;
                else
                {
                    j = (index + 1) / 8;
                    offSet = (index + 1) % 8 - 1;
                }

                byte[] getData = sm.Read(1, j);
                BitArray bitArray = new BitArray(getData);
                if (bitArray[offSet] == false)
                    return false;
            }

            return true;
        }

        public void close()
        {
            sm.Close();
        }

        public bool ContainsAny(List<T> items)
        {
            foreach (T item in items)
            {
                if (Contains(item))
                    return true;
            }
            return false;
        }

        public bool ContainsAll(List<T> items)
        {
            foreach (T item in items)
            {
                if (!Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 计算遇到误检率的概率
        /// </summary>
        /// <returns></returns>
        public double FalsePositiveProbability()
        {
            return Math.Pow(1 - Math.Exp(NumberOfHashes * SetSize / (double)BitSize), NumberOfHashes);
        } 
        #endregion
    }
}
