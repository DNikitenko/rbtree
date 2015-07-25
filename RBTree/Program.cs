using System;
using System.Collections.Generic;
using System.Linq;

namespace RBTree
{    
    class Program
    {
        static void Main(string[] args)
        {
            RBTree<int> Tree = new RBTree<int>();
            Random gen = new Random();
            List<int> lst = new List<int>();

            for (int i = 0; i < 1000000; ++i)
            {
                int tmp = gen.Next(10000000);
                Tree.InsertNode(tmp);
                lst.Add(tmp);
            }

            lst = (from int i in lst orderby Guid.NewGuid() select i).ToList();
            foreach (int i in lst)
            {
                Tree.DeleteNode(i);
            }

            //foreach (int i in new int[] { 881, 58, 270, 366, 879, 429, 845, 582, 832, 235 })
            //{
            //    Tree.InsertNode(i);
            //}

            //foreach (int i in new int[] { 235, 582, 881, 845, 832, 58, 270, 879, 429, 366 })
            //{
            //    Tree.DeleteNode(i);
            //}

            Console.WriteLine("Готово");
            Console.ReadLine();
        }
    }
}
