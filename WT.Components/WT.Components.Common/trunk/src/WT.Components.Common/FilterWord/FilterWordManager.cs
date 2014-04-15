using System.Collections.Generic;
using System.IO;

namespace WT.Components.Common.FilterWord
{
    public class FilterWordManager
    {
        private static object lockObject = new object();
        private static Node rootNode = null;

        /// <summary>
        /// 创建关键字树
        /// </summary>
        /// <param name="path"></param>
        private static void CreateTree(string path)
        {
            if (rootNode != null)
            {
                return;
            }

            lock (lockObject)
            {
                if (rootNode == null)
                {
                    rootNode = new Node('R');
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            string key = reader.ReadLine();
                            if (!string.IsNullOrEmpty(key))
                            {
                                char[] chars = key.ToCharArray();
                                rootNode.Insert(chars, 0);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 判断敏感词是不是在词库中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool HasKeyWord(string path, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return false;
            }

            CreateTree(path);

            //没有敏感词库
            if (rootNode == null || rootNode.Nodes.Count <= 0)
            {
                return false;
            }

            int index = 0;
            List<string> word = new List<string>();
            char[] chars = content.ToCharArray();
            Node node = rootNode;

            while (index < chars.Length)
            {
                node = node.FindNode(chars[index]);

                if (node == null)
                {
                    node = rootNode;
                    index = index - word.Count;
                    word.Clear();
                }
                else if (node.Flag == 1)
                {
                    return true;
                }
                else
                {
                    word.Add(chars[index].ToString());
                }

                index++;
            }

            return false;
        }
    }
}
