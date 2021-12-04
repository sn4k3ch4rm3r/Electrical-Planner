using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;
using Calculator2000.Models;
using Calculator2000.Views;
using Calculator2000.Data;

namespace Calculator2000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Node rootNode;
        public static Dictionary<string, List<Room>> Floors;

        public MainWindow()
        {
            InitializeComponent();
            Defaults.Init();
            Hierarchy.SelectedItemChanged += TreeViewItem_Selected;
            Hierarchy.AllowDrop = true;
            CreateNewFile();
        }

        private void CreateNewFile(Node root = null)
        {
            rootNode = root == null ? new RootNode() : root;
            Hierarchy.Items.Clear();
            Floors = new Dictionary<string, List<Room>>() { {"0", new List<Room>()} };
            TreeViewItem item = rootNode.ToTreeViewItem();
            item.IsExpanded = true;
            
            if(root == null)
                SetEventHandlers(item);
            else
            {
                SetEventHandlersRecursively(item);
                SetupRooms(rootNode);
            }
            
            Hierarchy.Items.Add(item);
            DataInputView.Content = null;   
        }

        private void SetEventHandlers(TreeViewItem item)
        {

            item.DragOver += ChildTreeViewItem_DragOver;
            item.Drop += ChildTreeViewItem_Drop;
            item.MouseMove += ChildTreeViewItem_MouseMove;
            item.MouseDown += ChildTreeViewItem_MouseDown;
        }

        private void SetEventHandlersRecursively(TreeViewItem item)
        {
            SetEventHandlers(item);
            foreach (TreeViewItem child in item.Items)
            {
                SetEventHandlersRecursively(child);
            }
        }

        private void SetupRooms(Node node)
        {
            if (node.GetType() == typeof(Room))
            {
                Room room = (Room)node;
                Floors[room.Floor].Add(room);
                room.UpdateHeader();
            }
            foreach (Node child in node.Children)
            {
                SetupRooms(child);
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            CreateNewFile();
        }

        private void TreeViewItem_Selected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((TreeViewItem)Hierarchy.SelectedItem == null) return;
            Node selectedNode = rootNode.FindNode(x => x.GUID.ToString() == ((TreeViewItem)Hierarchy.SelectedItem).Tag.ToString());
            if(selectedNode.GetType() == typeof(RootNode))
            {
                DataInputView.Navigate(new RootNodeDataView((RootNode) selectedNode));
            }
            else if(selectedNode.GetType() == typeof(DistributionBoard))
            {
                DataInputView.Navigate(new DistributionBoardDataView((DistributionBoard) selectedNode));
            }
            else if(selectedNode.GetType() == typeof(Room))
            {
                DataInputView.Navigate(new RoomDataView((Room)selectedNode));
            }
            else if (selectedNode.GetType() == typeof(Consumer))
            {
                DataInputView.Navigate(new ConsumerDataView((Consumer)selectedNode));
            }
        }
        
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json fájl (*.json)|*.json|Minden fájl (*.*)|*.*";
            if(openFileDialog.ShowDialog() == true)
            {
                string rawJson = File.ReadAllText(openFileDialog.FileName);
                Node root= JsonConvert.DeserializeObject<Node>(rawJson);
                root.SetupAfterLoad();
                CreateNewFile(root);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json fájl (*.json)|*.json|Minden fájl (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(rootNode));
            }
        }
        
        private void EditDefaults_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "Resources");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem) Hierarchy.SelectedItem;
            Node child = null;
            Node parentNode = selected == null ? rootNode : rootNode.FindNode(x => x.GUID.ToString() == selected.Tag.ToString());
            switch (((MenuItem) sender).Header)
            {
                case "Elosztó":
                    child = new DistributionBoard();
                    if (selected == null || parentNode == rootNode)
                        child.Name = $"E.{rootNode.Children.Count + 1}";
                    else if (parentNode.GetType() != typeof(DistributionBoard))
                        return;
                    else
                        child.Name = $"{parentNode.Name}.{parentNode.Children.Count + 1}";
                    break;
                case "Szoba":
                    if(parentNode.GetType() == typeof(Room))
                    {
                        parentNode = parentNode.Parent;
                        selected = selected.Parent as TreeViewItem;
                    }
                    else if (parentNode.GetType() != typeof(DistributionBoard))
                        return;
                    
                    child = new Room();
                    Floors["0"].Add(child as Room);
                    (child as Room).UpdateHeader();
                    break;
                case "Fogyasztó":
                    if(parentNode.GetType() == typeof(Consumer))
                    {
                        parentNode = parentNode.Parent;
                        selected = selected.Parent as TreeViewItem;
                    }
                    else if(selected == null)
                        return;
                    child = new Consumer();
                    break;
            }

            TreeViewItem childTreeViewItem = child.TreeViewItem;
            SetEventHandlers(childTreeViewItem);

            if (selected == null || parentNode == rootNode)
            {
                rootNode.AddChild(child);
                (Hierarchy.Items[0] as TreeViewItem).Items.Add(childTreeViewItem);
                (Hierarchy.Items[0] as TreeViewItem).IsExpanded = true;
            }
            else
            {
                parentNode.AddChild(child);
                selected.Items.Add(childTreeViewItem);
                selected.IsExpanded = true;
            }
            childTreeViewItem.IsSelected = true;
        }

        private Point _lastMouseDown;
        private TreeViewItem draggedItem;
        private TreeViewItem target;
        private void ChildTreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _lastMouseDown = e.GetPosition(Hierarchy);
        }

        private void ChildTreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(Hierarchy);
                if(Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0 || Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0)
                {
                    draggedItem = Hierarchy.SelectedItem as TreeViewItem;
                    if(draggedItem != null)
                    {
                        if (FindNode(draggedItem).GetType() == typeof(RootNode)) return;
                        
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(Hierarchy, Hierarchy.SelectedValue, DragDropEffects.Move);
                        if(finalDropEffect == DragDropEffects.Move && target != null && draggedItem.Tag != target.Tag)
                        {
                            Node draggedNode = FindNode(draggedItem);
                            Node parentNode = FindNode(target);

                            draggedNode.Parent.Children.Remove(draggedNode);
                            parentNode.AddChild(draggedNode);

                            (draggedItem.Parent as TreeViewItem).Items.Remove(draggedItem);
                            target.Items.Add(draggedItem);
                            target.IsExpanded = true;

                            target = null;
                            draggedItem = null;
                        }
                    }
                }
            }
        }

        private Node FindNode(TreeViewItem treeViewItem)
        {
            return rootNode.FindNode(x => x.GUID.ToString() == treeViewItem.Tag.ToString());
        }

        private void ChildTreeViewItem_Drop(object sender, DragEventArgs e)
        {
            Point currentPosition = e.GetPosition(Hierarchy);
            e.Effects = DragDropEffects.None;
            if (Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0 || Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0)
            {
                TreeViewItem item = sender as TreeViewItem;
                if(FindNode(item).GetType() == FindNode(draggedItem).Parent.GetType())
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            e.Handled = true;
        }

        private void ChildTreeViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            TreeViewItem targetItem = sender as TreeViewItem;
            if(targetItem != null && draggedItem != null)
            {
                this.target = targetItem;
                e.Effects = DragDropEffects.Move;
            }
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selected = (TreeViewItem)Hierarchy.SelectedItem;
            Node selectedNode = rootNode.FindNode(x => x.GUID.ToString() == selected.Tag.ToString());
            if (selectedNode == rootNode) return;
            if (selectedNode.GetType() == typeof(Room))
            {
                string floor = (selectedNode as Room).Floor;
                Floors[floor].Remove(selectedNode as Room);
                foreach (Room room in Floors[floor])
                {
                    room.UpdateHeader();
                }
            }
            (selected.Parent as TreeViewItem).Items.Remove(selected);
            selectedNode.Parent.Children.Remove(selectedNode);
            DataInputView.Content = null;
        }
    }
}
