using System;

namespace RBTree
{
    enum NodeColor { NODE_BLACK, NODE_RED };
    enum LinkMode { RIGHT, LEFT, NO };

    class RBTreeNode<T> where T : IComparable<T>
    {
        internal T Val;
        internal NodeColor Color;
        internal RBTreeNode<T> Parent;
        internal RBTreeNode<T> Left;
        internal RBTreeNode<T> Right;

        internal bool IsLeaf() { return Left == null && Right == null; }

        internal RBTreeNode(RBTreeNode<T> parent, T val)
        {
            this.Parent = parent;
            this.Val = val;
            Color = NodeColor.NODE_RED;
            Left = Right = null;
        }
    }

    partial class RBTree<T> where T : IComparable<T>
    {
        RBTreeNode<T> Root;

        public RBTreeNode<T> Find(T val)
        {
            RBTreeNode<T> CurrentNode = Root;

            bool found = false;
            while (!found)
            {
                if (CurrentNode == null) found = true;
                else if (val.CompareTo(CurrentNode.Val) == 0) found = true;
                else if (val.CompareTo(CurrentNode.Val) < 0) CurrentNode = CurrentNode.Left;
                else CurrentNode = CurrentNode.Right;
            }

            return CurrentNode;
        }

        RBTreeNode<T> GetGrandFather(RBTreeNode<T> node)
        {
            if (node != null && node.Parent != null)
                return node.Parent.Parent;
            else
                return null;
        }

        RBTreeNode<T> GetUncle(RBTreeNode<T> node)
        {
            RBTreeNode<T> GrandFather = GetGrandFather(node);
            if (GrandFather == null) return null;
            else
            {
                if (node.Parent == GrandFather.Left)
                    return GrandFather.Right;
                else
                    return GrandFather.Left;
            }
        }

        RBTreeNode<T> GetSibling(RBTreeNode<T> Node)
        {
            if (Node == Node.Parent.Left) return Node.Parent.Right;
            else return Node.Parent.Left;
        }

        void RotateLeft(RBTreeNode<T> Node, LinkMode Mode)
        {
            RBTreeNode<T> Grand = Node.Parent;
            RBTreeNode<T> Pivot = Node.Right;

            //Выполняем вращение            
            Node.Right = Pivot.Left;
            if (Node.Right != null) Node.Right.Parent = Node;
            Pivot.Left = Node;

            switch (Mode)
            {
                case LinkMode.NO: Root = Pivot; break;
                case LinkMode.LEFT: Grand.Left = Pivot; break;
                case LinkMode.RIGHT: Grand.Right = Pivot; break;
            }

            //Выставляем корректно предков
            Pivot.Parent = Grand;
            Node.Parent = Pivot;
        }

        void RotateRight(RBTreeNode<T> Node, LinkMode Mode)
        {
            RBTreeNode<T> Grand = Node.Parent;
            RBTreeNode<T> Pivot = Node.Left;

            //Выполняем вращение            
            Node.Left = Pivot.Right;
            if (Node.Left != null) Node.Left.Parent = Node;
            Pivot.Right = Node;

            switch (Mode)
            {
                case LinkMode.NO: Root = Pivot; break;
                case LinkMode.LEFT: Grand.Left = Pivot; break;
                case LinkMode.RIGHT: Grand.Right = Pivot; break;
            }

            //Выставляем корректно предков
            Pivot.Parent = Grand;
            Node.Parent = Pivot;
        }        
    }
}