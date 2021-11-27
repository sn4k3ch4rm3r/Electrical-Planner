using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using Newtonsoft.Json;
using JsonKnownTypes;
using System.ComponentModel;

namespace Calculator2000.Models
{
    [JsonConverter(typeof(JsonKnownTypesConverter<Node>))]
    [JsonKnownType(typeof(Node), "node")]
    [JsonKnownType(typeof(RootNode), "root")]
    [JsonKnownType(typeof(DistributionBoard), "distribution")]
    [JsonKnownType(typeof(Room), "room")]
    [JsonKnownType(typeof(Consumer), "consumer")]
    public class Node : INotifyPropertyChanged
    {
        public const int COSFI = 1;

        private string _Name;
        public virtual string Name { get { return _Name; } set { _Name = value; TreeViewItem.Header = ToString(); } }
        [JsonIgnore]
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }

        [JsonIgnore]
        public virtual double TotalPower
        {
            get
            {
                return Children.Sum(x => x.TotalPower);
            }
        }

        [JsonIgnore]
        public virtual double UsedPower
        {
            get
            {
                return this.Children.Sum(x => x.UsedPower);
            }
        }

        private TreeViewItem _TreeViewItem;
        [JsonIgnore]
        public TreeViewItem TreeViewItem
        {
            get
            {
                if (_TreeViewItem == null)
                    _TreeViewItem = ToTreeViewItem();
                return _TreeViewItem;
            }
            set
            {
                _TreeViewItem = value;
            }
        }
        [JsonIgnore]
        public Guid GUID { get; }
        [JsonIgnore]
        public bool IsRoot 
        { 
            get
            {
                return this.Parent == null;
            }
        }
        [JsonIgnore]
        public bool IsLeaf
        {
            get
            {
                return this.Children.Count == 0;
            }
        }

        public Node()
        {
            Children = new List<Node>();
            GUID = System.Guid.NewGuid();

            this.ElementsIndex = new LinkedList<Node>();
            this.ElementsIndex.Add(this);
        }

        public Node AddChild(Node child)
        {
            child.Parent = this;
            this.Children.Add(child);
            this.RegisterChildForSearch(child);
            return child;
        }

        public void SetupAfterLoad()
        {
            foreach (Node child in Children)
            {
                child.Parent = this;
                RegisterChildForSearch(child);
                child.SetupAfterLoad();
            }
            if (!IsRoot)
            {
                _TreeViewItem = ToTreeViewItem();
                _TreeViewItem.IsExpanded = true;
            }
        }

        public TreeViewItem ToTreeViewItem()
        {
            TreeViewItem treeViewItem = new TreeViewItem()
            {
                Header = ToString(),
                Tag = GUID.ToString()
            };
            foreach (Node child in Children)
            {
                treeViewItem.Items.Add(child.ToTreeViewItem());
            }
            this.TreeViewItem = treeViewItem;
            return treeViewItem;
        }

        private ICollection<Node> ElementsIndex { get; set; }

        private void RegisterChildForSearch(Node node)
        {
            ElementsIndex.Add(node);
            if (Parent != null)
                Parent.RegisterChildForSearch(node);
        }

        public Node FindNode(Func<Node, bool> predicate)
        {
            return this.ElementsIndex.FirstOrDefault(predicate);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
