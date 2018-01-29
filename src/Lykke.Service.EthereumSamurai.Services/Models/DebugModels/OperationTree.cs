using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Services.Models.DebugModels
{
    public class TreeNodeLeveller<T>
    {
        public Dictionary<int, List<TreeNode<T>>> Levels { get; set; }
         
        public TreeNodeLeveller(TreeNode<T> initNode)
        {
            Levels = new Dictionary<int, List<TreeNode<T>>>();
            Levels.Add(0, new List<TreeNode<T>>() { initNode });
        }

        public void AddToNode(TreeNode<T> parent, TreeNode<T> child)
        {
            parent.AddChild(child);

        }
    }

    public class TreeNode<T>
    {
        public int Depth {get; private set;}
        public T Data { get; set; }
        public TreeNode<T> Parent { get; set; }
        public List<TreeNode<T>> Childs { get; set; }

        public TreeNode(T data, int depth)
        {
            Data = data;
            Childs = new List<TreeNode<T>>();
            Depth = depth;
        }

        public void AddChild(TreeNode<T> node)
        {
            node.Parent = this;
            Childs.Add(node);
        }

        public void RecursivelyProcessAllNodes(Action<T> action)
        {
            action(this.Data);

            if (this.Childs.Count == 0)
            {
                return;
            }

            foreach (var child in this.Childs)
            {
                child.RecursivelyProcessAllNodes(action);
            }
        }
    }
}
