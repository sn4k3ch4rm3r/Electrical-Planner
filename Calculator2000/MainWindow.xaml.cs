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
        private string currentFile;
        private bool saved = true;
        public static Dictionary<string, List<Room>> Floors;
        public static readonly RoutedCommand InsertDistributionCommand = new RoutedCommand();
        public static readonly RoutedCommand InsertRoomCommand = new RoutedCommand();
        public static readonly RoutedCommand InsertConsumerCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            Defaults.Init();
            Hierarchy.SelectedItemChanged += TreeViewItem_Selected;
            Hierarchy.AllowDrop = true;
            CreateNewFile();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saved && !ConfirmExit()) e.Cancel = true;
        }

        private bool ConfirmExit()
        {
            MessageBoxResult result = MessageBox.Show("A nem mentett változtatások el fognak veszni!\nBiztosan folytatod?", "Megerősítés", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
            return result == MessageBoxResult.OK;
        }

        private void CreateNewFile(Node root = null)
        {
            if (!saved && !ConfirmExit()) return; 
            rootNode = root == null ? new RootNode() : root;
            (rootNode as RootNode).Updated = FileChanged;

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

            currentFile = null;
            saved = true;
            Title = "Ismeretlen";
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

        private void TreeViewItem_Selected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if ((TreeViewItem)Hierarchy.SelectedItem == null) return;
            Node selectedNode = FindNode((TreeViewItem)Hierarchy.SelectedItem);
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

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json fájl (*.json)|*.json|Minden fájl (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                this.currentFile = saveFileDialog.FileName;
                Save(saveFileDialog.FileName);
            }
        }

        private void Save(string fileName)
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(rootNode));
            this.saved = true;
            this.Title = this.currentFile;
        }

        private void InsertNode(string type)
        {
            TreeViewItem selected = (TreeViewItem)Hierarchy.SelectedItem;
            Node child = null;
            Node parentNode = selected == null ? rootNode : FindNode(selected);
            switch (type)
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
                    if (parentNode.GetType() == typeof(Room))
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
                    if (parentNode.GetType() == typeof(Consumer))
                    {
                        parentNode = parentNode.Parent;
                        selected = selected.Parent as TreeViewItem;
                    }
                    else if (selected == null)
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
            FileChanged();
        }

        private void EditDefaults_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "Resources");
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
                if(FindNode(draggedItem).AllowedParentTypes.Contains(FindNode(item).GetType()))
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

        private void FileChanged()
        {
            if (saved)
                this.Title += " *";
            this.saved = false;
        }


        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            CreateNewFile();
        }
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            if (this.currentFile != null)
                Save(this.currentFile);
            else
                SaveAs();
        }
        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            SaveAs();
        }
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json fájl (*.json)|*.json|Minden fájl (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string rawJson = File.ReadAllText(openFileDialog.FileName);
                Node root = JsonConvert.DeserializeObject<Node>(rawJson);
                root.SetupAfterLoad();
                CreateNewFile(root);
                this.currentFile = openFileDialog.FileName;
                this.Title = currentFile;
            }
        }
        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e) { }

        private void DeleteSelected()
        {
            if (this.currentFile == null && this.saved) return;
            TreeViewItem selected = (TreeViewItem)Hierarchy.SelectedItem;
            Node selectedNode = FindNode(selected);
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
            FileChanged();
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            DeleteSelected();
        }

        private void InsertDistributionCommand_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            InsertNode("Elosztó");
        }
        private void InsertRoomCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertNode("Szoba");
        }
        private void InsertConsumerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertNode("Fogyasztó");
        }

        private void Copy()
        {
            Clipboard.Clear();
            Clipboard.SetData("NodeJSON", JsonConvert.SerializeObject(FindNode(Hierarchy.SelectedItem as TreeViewItem)));
        }

        private Node GetNodeFromClipboard()
        {
            if (!Clipboard.ContainsData("NodeJSON")) return null;
            return JsonConvert.DeserializeObject<Node>(Clipboard.GetData("NodeJSON").ToString());
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
        }
        private void CutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Copy();
            DeleteSelected();
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TreeViewItem selected = Hierarchy.SelectedItem as TreeViewItem;
            Node selectedNode = FindNode(selected);

            Node data = GetNodeFromClipboard();
            
            selectedNode.AddChild(data);
            selectedNode.SetupAfterLoad();
            selected.Items.Add(data.ToTreeViewItem());
        }

        private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeViewItem selected = Hierarchy.SelectedItem as TreeViewItem;
            e.CanExecute = selected != null && FindNode(selected).GetType() != typeof(RootNode);
        }
        private void PasteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeViewItem selected = Hierarchy.SelectedItem as TreeViewItem;
            Node node = GetNodeFromClipboard();
            e.CanExecute = selected != null && node != null && node.AllowedParentTypes.Contains(FindNode(selected).GetType());
        }
    }
}
