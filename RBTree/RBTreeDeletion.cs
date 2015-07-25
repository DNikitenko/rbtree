using System;
using System.Collections.Generic;
using System.Linq;

namespace RBTree
{
    partial class RBTree<T> where T : IComparable<T>
    {        
        /// <summary>
        /// Возвращает минимальный элемент дерева с корнем в узле Node
        /// </summary>
        /// <param name="Node">Корень дерева</param>
        RBTreeNode<T> GetMinimalNode(RBTreeNode<T> Node)
        {
            RBTreeNode<T> MinimalNode = Node;
            while (MinimalNode.Left != null) MinimalNode = MinimalNode.Left;
            return MinimalNode;
        }

        void UpdateParent(RBTreeNode<T> Node, RBTreeNode<T> NewValue)
        {
            if (Node.Parent != null)
            {
                if (Node == Node.Parent.Right) Node.Parent.Right = NewValue;
                else Node.Parent.Left = NewValue;
            }
            else
            {
                Root = NewValue;
            }
        }

        void DeleteNodeFinalRotation(RBTreeNode<T> Node)
        {
            RBTreeNode<T> Sibling = GetSibling(Node);

            //А теперь - последняя трансформация, которая, раз уж сюда пришли, восстановит все свойства дерева. Вот и всё :)
            Sibling.Color = Node.Parent.Color;
            Node.Parent.Color = NodeColor.NODE_BLACK;

            LinkMode mode = LinkMode.NO;
            if (Node.Parent.Parent != null)
            {
                if (Node.Parent == Node.Parent.Parent.Right) mode = LinkMode.RIGHT;
                else if (Node.Parent == Node.Parent.Parent.Left) mode = LinkMode.LEFT;
            }
 
            if (Node == Node.Parent.Left)
            {
                Sibling.Right.Color = NodeColor.NODE_BLACK;
                RotateLeft(Node.Parent, mode);
            }
            else
            {
                Sibling.Left.Color = NodeColor.NODE_BLACK;
                RotateRight(Node.Parent, mode);
            }
        }

        void DeleteNodeNeedRightRotation(RBTreeNode<T> Node)
        {
            RBTreeNode<T> Sibling = GetSibling(Node);

            //Очередной случай, но теперь малой кровью не обойтись, придётся вращать поддеревья
            LinkMode mode = LinkMode.NO;
            if (Sibling.Parent != null)
            {
                if (Sibling == Sibling.Parent.Right) mode = LinkMode.RIGHT;
                else if (Sibling == Sibling.Parent.Left) mode = LinkMode.LEFT;
            }

            if ((Sibling == Sibling.Parent.Right) && (Sibling.Right == null || Sibling.Right.Color == NodeColor.NODE_BLACK))
            {
                Sibling.Color = NodeColor.NODE_RED;
                Sibling.Left.Color = NodeColor.NODE_BLACK;
                RotateRight(Sibling, mode);
            }
            else if ((Sibling == Sibling.Parent.Left) && (Sibling.Left == null || Sibling.Left.Color == NodeColor.NODE_BLACK))
            {
                Sibling.Color = NodeColor.NODE_RED;
                Sibling.Right.Color = NodeColor.NODE_BLACK;
                RotateLeft(Sibling, mode);
            }

            DeleteNodeFinalRotation(Node);
        }

        void DeleteNodeNewParentIsRed(RBTreeNode<T> Node)
        {
            RBTreeNode<T> Sibling = GetSibling(Node);

            //Очередной случай: папаша красный, а все дети - чёрные. Просто тогда меняем цвета у отца и брата.
            //Это добавит 1 черный узел к путям через Node, как раз чтобы "отыграть потерю"
            if ((Node.Parent.Color == NodeColor.NODE_RED) &&
                (Sibling.Color == NodeColor.NODE_BLACK) &&
                (Sibling.Left == null || Sibling.Left.Color == NodeColor.NODE_BLACK) &&
                (Sibling.Right == null || Sibling.Right.Color == NodeColor.NODE_BLACK))
            {
                Sibling.Color = NodeColor.NODE_RED;
                Node.Parent.Color = NodeColor.NODE_BLACK;
            }
            else
            {
                DeleteNodeNeedRightRotation(Node);
            }
        }

        void DeleteNodeEveryoneIsBlack(RBTreeNode<T> Node)
        {
            RBTreeNode<T> Sibling = GetSibling(Node);

            //Если всё чёрные, то мы выравниваем баланс, перекрашивая брата в красный. Но теперь все проходящие через отца
            //пути содержат на 1 узел меньше, чем те, что не проходят. Поэтому применим рекурсивно процедуру балансировки
            //к оставшемуся дереву, начиная с отца
            if ((Node.Parent.Color == NodeColor.NODE_BLACK) &&
                (Sibling.Color == NodeColor.NODE_BLACK) &&
                (Sibling.Left == null || Sibling.Left.Color == NodeColor.NODE_BLACK) &&
                (Sibling.Right == null || Sibling.Right.Color == NodeColor.NODE_BLACK))
            {
                Sibling.Color = NodeColor.NODE_RED;
                DeleteNodeMaybeRoot(Node.Parent);
            }
            else
            {
                DeleteNodeNewParentIsRed(Node);
            }
        }

        void DeleteNodeSiblingMaybeRed(RBTreeNode<T> Node)
        {
            RBTreeNode<T> Sibling = GetSibling(Node);

            //Здесь мы обеспечиваем, чтобы у Node был чёрный брат и красный отец. Тогда в дальнейшем можно всё сделать красиво
            if (Sibling != null && Sibling.Color == NodeColor.NODE_RED)
            {
                Node.Parent.Color = NodeColor.NODE_RED;
                Sibling.Color = NodeColor.NODE_BLACK;

                LinkMode mode = LinkMode.NO;
                if (Node.Parent.Parent != null)
                {
                    if (Node.Parent == Node.Parent.Parent.Right) mode = LinkMode.RIGHT;
                    else if (Node.Parent == Node.Parent.Parent.Left) mode = LinkMode.LEFT;
                }

                if (Node == Node.Parent.Left)
                    RotateLeft(Node.Parent, mode);
                else
                    RotateRight(Node.Parent, mode);
            }

            DeleteNodeEveryoneIsBlack(Node);
        }

        void DeleteNodeMaybeRoot (RBTreeNode<T> Node)
        {
            //Если так сложилось, что потомок стал новым корнем, то всё ОК
            if (Root != Node) DeleteNodeSiblingMaybeRed(Node);
        }        

        void PerformDeletion(RBTreeNode<T> toDelete)
        {            
            RBTreeNode<T> NextElement = null;
            NodeColor DeletedNodeColor = toDelete.Color;
            bool UpdateLinksToNull = false;

            //Если удаляемая вершина - лист, то дополнительных действий выполнять не нужно
            if (toDelete.IsLeaf())
            {                
                UpdateLinksToNull = true;
                NextElement = toDelete;
            }
            //Если у вершины есть только один потомок, то надо подставить его вместо удаляемой вершины (т.к. у него нет листьев)
            else if (toDelete.Left == null)
            {
                toDelete.Right.Parent = toDelete.Parent;
                UpdateParent(toDelete, toDelete.Right);
                NextElement = toDelete = toDelete.Right;                   
            }
            else if (toDelete.Right == null)
            {
                toDelete.Left.Parent = toDelete.Parent;
                UpdateParent(toDelete, toDelete.Left);
                NextElement = toDelete = toDelete.Left;
            }
            //Если узел имеет двух нелистовых потомков, то найдём наименьший элемент правого поддерева
            //(с таким же успехом можно было искать максимальный элемент левого)
            else
            {
                NextElement = GetMinimalNode(toDelete.Right);
                DeletedNodeColor = NextElement.Color;

                //Теперь значение вершины, которое надо удалить, заменится значением минимального элемента,
                //а сам элемент исчезнет
                toDelete.Val = NextElement.Val;                
            }

            //Если мы херакнули красную вершину, то дерево осталось сбалансированным. Если нет, то поехали веселиться
            //(ща перекурю и допишу)
            if (Root != null && DeletedNodeColor == NodeColor.NODE_BLACK)
            {
                if (NextElement.IsLeaf())
                {
                    //Ещё один простой случай: потомок стал красным. Поэтому просто перекрашиваем потомка - и voile :)
                    if (NextElement.Color == NodeColor.NODE_RED) NextElement.Color = NodeColor.NODE_BLACK;
                    //Ну а если оба чернявые - то начинается хардкор, потому что требуется балансировка
                    //(потому как на один чёрный узел в пути стала меньше, и свойство чёрной высоты нарушилось)
                    else
                    {
                        DeleteNodeMaybeRoot(NextElement);
                        UpdateLinksToNull = true;
                    }
                }
                else
                {
                    UpdateParent(NextElement, NextElement.Right);
                    NextElement.Right.Parent = NextElement.Parent;

                    if (NextElement.Right.Color == NodeColor.NODE_RED) NextElement.Right.Color = NodeColor.NODE_BLACK;
                    else DeleteNodeMaybeRoot(NextElement);
                }
            }

            //А это сделано потому, что у меня пустые листы не представляются как отдельные объекты, а алгоритм этого требует.
            //Поэтому узел как бы не удаляется и алгоритм начинает с него, а после можно наконец спокойно удалить уже
            if (UpdateLinksToNull)
            {
                if (toDelete == Root) Root = null;
                else UpdateParent(NextElement, null);
            }
        }

        public void DeleteNode(T val)
        {
            //Вначале найдём вершину, которую трубется удалить
            RBTreeNode<T> toDelete = Find(val);
            if (toDelete != null) PerformDeletion(toDelete);
        }
    }
}