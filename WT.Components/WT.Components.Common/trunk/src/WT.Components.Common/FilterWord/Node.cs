using System.Collections.Generic;

namespace WT.Components.Common.FilterWord
{
    internal class Node
    {
        private char c;
        private int flag;//1表示终结，0表示延续
        private List<Node> nodes = new List<Node>();

        /// <summary>
        /// 单词
        /// </summary>
        public char C
        {
            get
            {
                return c;
            }
        }

        /// <summary>
        /// 1表示终结，0表示延续
        /// </summary>
        public int Flag
        {
            get
            {
                return flag;
            }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<Node> Nodes
        {
            get
            {
                return nodes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public Node(char c)
        {
            this.c = c;
            this.flag = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="flag"></param>
        public Node(char c, int flag)
        {
            this.c = c;
            this.flag = flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="p"></param>
        internal void Insert(char[] chars, int index)
        {
            Node node = FindNode(chars[index]);
            if (node == null)
            {
                node = new Node(chars[index]);
                nodes.Add(node);
            }

            if (index == (chars.Length - 1))
            {
                node.flag = 1;
            }

            index++;
            {
                if (index < chars.Length)
                {
                    Insert(chars, index);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal Node FindNode(char c)
        {
            Node node = null;
            foreach (Node n in nodes)
            {
                if (n.c == c)
                {
                    node = n;
                    break;
                }
            }
            return node;
        }
    }
}
