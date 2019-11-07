using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Cryptography;

namespace HashConsoleApp
{

    interface IHashFunction
    {
        byte[] ComputeHash(string s);
    }

    class H1: IHashFunction
    {
        private SHA256 sha256Hash;
        public H1()
        {
            sha256Hash = SHA256.Create();
        }
        public byte[] ComputeHash(string s)
        {
            return this.sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }

    class H2 : IHashFunction
    {
        private SHA384 sha384;
        public H2()
        {
            sha384 = SHA384.Create();
        }
        public byte[] ComputeHash(string s)
        {
            return this.sha384.ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }

    class H3 : IHashFunction
    {
        private MD5 md5;
        public H3()
        {
            md5 = MD5.Create();
        }
        public byte[] ComputeHash(string s)
        {
            return this.md5.ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }

    class BloomFilter<T>
    {
        private int[] filter;
        private List<IHashFunction> hashes;
        private int length;
        public int capacity;

        public BloomFilter()
        {
            H1 h1 = new H1();
            H2 h2 = new H2();
            H3 h3 = new H3();
            hashes = new List<IHashFunction>();
            hashes.Add(h1);
            hashes.Add(h2);
            hashes.Add(h3);
            filter = new int[1 << 20];
            capacity = (1 << 25) - 1;
            length = 25;
        }

        public void add(T rawData)
        {
            string data = rawData.ToString();
            foreach (IHashFunction h in hashes)
            {
                byte[] bytes = h.ComputeHash(data);
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = bytes.Length - 4; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                int hex = int.Parse(builder.ToString(), System.Globalization.NumberStyles.HexNumber) & capacity;
                int index = hex >> 5;
                int rest = 1 << (hex & 31);
                filter[index] |= rest;
            }

        }

        public bool contains(T rawData)
        {
            string data = rawData.ToString();
            foreach (IHashFunction h in hashes)
            {
                byte[] bytes = h.ComputeHash(data);
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = bytes.Length - 4; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                int hex = int.Parse(builder.ToString(), System.Globalization.NumberStyles.HexNumber) & capacity;
                int index = hex >> 5;
                int rest = 1 << (hex & 31);
                if ((filter[index] & rest) == 0)
                {
                    return false;
                }
            }

            return true;
        }

    }
    public class Program
    {
        static void Main(string[] args)
        {
            BloomFilter<int> b = new BloomFilter<int>();
            HashSet<int> h = new HashSet<int>();
            for (int i = 0; i < 1000000; i++)
            {
                b.add(i);
                h.Add(i);
            }


        }
    }

}
