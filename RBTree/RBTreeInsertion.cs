using System;
using System.Collections.Generic;
using System.Linq;

namespace RBTree
{
    partial class RBTree<T> where T : IComparable<T>
    {
        RBTreeNode<T> GetInsertionNode(T val)
        {
            RBTreeNode<T> ReturnNode = null;
            if (Root == null) Root = ReturnNode = new RBTreeNode<T>(null, val);
            else
            {
                RBTreeNode<T> CurrentNode = Root;

                bool NodeFound = false;
                while (!NodeFound)
                {
                    if (val.CompareTo(CurrentNode.Val) < 0)
                    {
                        if (CurrentNode.Left == null)
                        {
                            ReturnNode = CurrentNode.Left = new RBTreeNode<T>(CurrentNode, val);
                            NodeFound = true;
                        }
                        else
                            CurrentNode = CurrentNode.Left;
                    }
                    else
                    {
                        if (CurrentNode.Right == null)
                        {
                            ReturnNode = CurrentNode.Right = new RBTreeNode<T>(CurrentNode, val);
                            NodeFound = true;
                        }
                        else
                            CurrentNode = CurrentNode.Right;
                    }
                }
            }

            return ReturnNode;
        }

        void InsertNodeFinal(RBTreeNode<T> Node, T val)
        {
            RBTreeNode<T> GrandFather = GetGrandFather(Node);

            //Теперь свойство постоянства чёрной высоты дерева выполняется. Осталось, чтобы дети красных были чёрными.
            //Выполним поворот дерева "на деда", заодно перекрасив некоторые узлы
            Node.Parent.Color = NodeColor.NODE_BLACK;
            GrandFather.Color = NodeColor.NODE_RED;

            LinkMode mode = LinkMode.NO;
            if (GrandFather.Parent != null)
            {
                if (GrandFather == GrandFather.Parent.Right) mode = LinkMode.RIGHT;
                else if (GrandFather == GrandFather.Parent.Left) mode = LinkMode.LEFT;
            }

            if (Node == Node.Parent.Left && Node.Parent == GrandFather.Left)
            {
                RotateRight(GrandFather, mode);
            }
            else if (Node == Node.Parent.Right && Node.Parent == GrandFather.Right)
            {
                RotateLeft(GrandFather, mode);
            }
        }

        void InsertNodeMaybeNeedRotation(RBTreeNode<T> Node, T val)
        {
            RBTreeNode<T> Uncle = GetUncle(Node);
            RBTreeNode<T> Next = Node;

            //Если батя красный, но дядя - чёрный, то придётся проводить балансировку
            if (Node.Parent.Color == NodeColor.NODE_RED && (Uncle == null || Uncle.Color == NodeColor.NODE_BLACK))
            {
                RBTreeNode<T> GrandFather = GetGrandFather(Node);

                //Теперь повращаем немного. Если надо, поменяем местами узел с его батей
                if (Node == Node.Parent.Right && Node.Parent == GrandFather.Left)
                {
                    RotateLeft(Node.Parent, LinkMode.LEFT);
                    Next = Node.Left;
                }
                else if (Node == Node.Parent.Left && Node.Parent == GrandFather.Right)
                {
                    RotateRight(Node.Parent, LinkMode.RIGHT);
                    Next = Node.Right;
                }
            }

            InsertNodeFinal(Next, val);
        }

        void InsertNodeMaybeNeedRepaint(RBTreeNode<T> Node, T val)
        {
            RBTreeNode<T> Uncle = GetUncle(Node);
            //Если батя и дядя - красные, то перекрашиваем их в чёрный, а деда - в красный
            if (Node.Parent.Color == NodeColor.NODE_RED && Uncle != null && Uncle.Color == NodeColor.NODE_RED)
            {
                Node.Parent.Color = NodeColor.NODE_BLACK;
                Uncle.Color = NodeColor.NODE_BLACK;
                Uncle.Parent.Color = NodeColor.NODE_RED;

                //На случай, если дед нарушит свойства красно-чёрного дерева, повторим процедуру заново
                InsertNodeMaybeRoot(Uncle.Parent, val);
            }
            else
            {
                InsertNodeMaybeNeedRotation(Node, val);
            }
        }

        void InsertNodeMaybeBlackFather(RBTreeNode<T> Node, T val)
        {
            //Если папаша - чёрный, то ничего перекрашивать не надо
            if (Node.Parent.Color == NodeColor.NODE_RED)
            {
                InsertNodeMaybeNeedRepaint(Node, val);
            }
        }

        void InsertNodeMaybeRoot(RBTreeNode<T> Node, T val)
        {
            //1й случай: вставляем корень. Тогда перекрашиваем его в чёрный
            if (Node == Root)
            {
                Node.Color = NodeColor.NODE_BLACK;
            }
            else
            {
                InsertNodeMaybeBlackFather(Node, val);
            }
        }

        public void InsertNode(T val)
        {
            if (Find(val) == null)
            {
                RBTreeNode<T> Node = GetInsertionNode(val);
                InsertNodeMaybeRoot(Node, val);
            }
            else { Console.WriteLine(val); }
        }
    }
}