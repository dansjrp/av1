using System;
using System.Collections.Generic;
using System.Linq;

namespace Tree
{

    public class TreeNode
    {
        public int Value { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public TreeNode(int value)
        {
            Value = value;
            Left = null;
            Right = null;
        }

        public void PrintTree(string prefix = "", bool isLeft = true)
        {
            if (Right != null)
            {
                Right.PrintTree(prefix + (isLeft ? "│   " : "    "), false);
            }

            Console.WriteLine(prefix + (isLeft ? "└── " : "┌── ") + Value);

            if (Left != null)
            {
                Left.PrintTree(prefix + (isLeft ? "    " : "│   "), true);
            }
        }
    }

    // Classe principal para construir a árvore
    public class SpecialTreeBuilder
    {
        /// <summary>
        /// Constrói uma árvore a partir de um array seguindo as regras:
        /// - Raiz: maior valor do array
        /// - Galhos esquerdos: números à esquerda da raiz em ordem decrescente
        /// - Galhos direitos: números à direita da raiz em ordem decrescente
        /// </summary>
        /// <param name="array">Array de inteiros sem duplicatas</param>
        /// <returns>Raiz da árvore construída</returns>
        public TreeNode? BuildTree(int[] array)
        {
            if (array == null || array.Length == 0)
                return null;

            int maxValue = array.Max();
            int maxIndex = Array.IndexOf(array, maxValue);

            TreeNode root = new TreeNode(maxValue);

            int[] leftElements = array.Take(maxIndex).ToArray();

            int[] rightElements = array.Skip(maxIndex + 1).ToArray();

            root.Left = BuildSubtree(leftElements);
            root.Right = BuildSubtree(rightElements);

            return root;
        }

        /// <summary>
        /// Constrói uma subárvore recursivamente seguindo as mesmas regras
        /// </summary>
        /// <param name="elements">Array de elementos para construir a subárvore</param>
        /// <returns>Raiz da subárvore</returns>
        private TreeNode? BuildSubtree(int[] elements)
        {
            if (elements.Length == 0)
                return null;

            if (elements.Length == 1)
                return new TreeNode(elements[0]);

            int maxValue = elements.Max();
            int maxIndex = Array.IndexOf(elements, maxValue);

            TreeNode node = new TreeNode(maxValue);

            int[] leftElements = elements.Take(maxIndex).ToArray();

            int[] rightElements = elements.Skip(maxIndex + 1).ToArray();

            node.Left = BuildSubtree(leftElements);
            node.Right = BuildSubtree(rightElements);

            return node;
        }

        /// <summary>
        /// Percorre a árvore em ordem (in-order traversal)
        /// </summary>
        /// <param name="root">Raiz da árvore</param>
        /// <returns>Lista com os valores em ordem</returns>
        public List<int> InOrderTraversal(TreeNode? root)
        {
            List<int> result = new List<int>();
            InOrderHelper(root, result);
            return result;
        }

        private void InOrderHelper(TreeNode? node, List<int> result)
        {
            if (node != null)
            {
                InOrderHelper(node.Left, result);
                result.Add(node.Value);
                InOrderHelper(node.Right, result);
            }
        }

        /// <summary>
        /// Percorre a árvore em pré-ordem (pre-order traversal)
        /// </summary>
        /// <param name="root">Raiz da árvore</param>
        /// <returns>Lista com os valores em pré-ordem</returns>
        public List<int> PreOrderTraversal(TreeNode? root)
        {
            List<int> result = new List<int>();
            PreOrderHelper(root, result);
            return result;
        }

        private void PreOrderHelper(TreeNode? node, List<int> result)
        {
            if (node != null)
            {
                result.Add(node.Value);
                PreOrderHelper(node.Left, result);
                PreOrderHelper(node.Right, result);
            }
        }

        /// <summary>
        /// Calcula a altura da árvore
        /// </summary>
        /// <param name="root">Raiz da árvore</param>
        /// <returns>Altura da árvore</returns>
        public int GetHeight(TreeNode? root)
        {
            if (root == null)
                return 0;

            int leftHeight = GetHeight(root.Left);
            int rightHeight = GetHeight(root.Right);

            return Math.Max(leftHeight, rightHeight) + 1;
        }

        /// <summary>
        /// Conta o número total de nós na árvore
        /// </summary>
        /// <param name="root">Raiz da árvore</param>
        /// <returns>Número de nós</returns>
        public int CountNodes(TreeNode? root)
        {
            if (root == null)
                return 0;

            return CountNodes(root.Left) + CountNodes(root.Right) + 1;
        }
    }

    public class TreeNodeRequest
    {
        public int[] Array { get; set; }

        public TreeNodeRequest(int[] array)
        {
            Array = array;
        }
    }
}