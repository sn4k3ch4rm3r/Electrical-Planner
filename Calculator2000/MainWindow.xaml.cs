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

namespace Calculator2000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Node rootNode;
        public MainWindow()
        {
            InitializeComponent();
            rootNode = new Node();
            Hierarchy.SelectedItemChanged += TreeViewItem_Selected;
        }

        private void TreeViewItem_Selected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Node selectedNode = rootNode.FindNode(x => x.GUID.ToString() == ((TreeViewItem)Hierarchy.SelectedItem).Tag.ToString());
            if(selectedNode.GetType() == typeof(DistributionBoard))
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
                List<Node> nodes = JsonConvert.DeserializeObject<List<Node>>(rawJson);
                rootNode.Children = nodes;
                rootNode.SetupAfterLoad();
                Hierarchy.Items.Clear();
                foreach (Node child in rootNode.Children)
                {
                    Hierarchy.Items.Add(child.TreeViewItem);
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("test.json", JsonConvert.SerializeObject(rootNode.Children));
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
                    if (selected == null)
                        child.Name = $"E.{rootNode.Children.Count + 1}";
                    else if (parentNode.GetType() != typeof(DistributionBoard))
                        return;
                    else
                        child.Name = $"{parentNode.Name}.{parentNode.Children.Count + 1}";
                    break;
                case "Szoba":
                    if (parentNode.GetType() != typeof(DistributionBoard))
                        return;
                    child = new Room();
                    break;
                case "Fogyasztó":
                    if(selected == null || parentNode.GetType() == typeof(Consumer))
                        return;
                    child = new Consumer();
                    break;
            }

            TreeViewItem childTreeViewItem = child.TreeViewItem;

            if (selected == null)
            {
                rootNode.AddChild(child);
                Hierarchy.Items.Add(childTreeViewItem);
            }
            else
            {
                parentNode.AddChild(child);
                selected.Items.Add(childTreeViewItem);
                selected.IsExpanded = true;
            }
        }
    }
}
