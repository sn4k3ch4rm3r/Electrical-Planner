using Calculator2000.Data;
using Calculator2000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator2000.Views
{
    /// <summary>
    /// Interaction logic for RootNodeDataView.xaml
    /// </summary>
    public partial class RootNodeDataView : Page
    {
        public RootNode RootNode { get; }
        public List<double> Voltages { get; } = new List<double>()
            {
                400, 230
            };
        public List<MaterialProperty> Materials { get; } = new List<MaterialProperty>()
            {
                MaterialProperties.COPPER, MaterialProperties.ALUMINIUM
            };
        public List<int> Phases { get; } = new List<int>()
            {
                3, 1
            };
        public List<string> CablePart { get; } = new List<string>() 
            {
                "Mért fővezeték",
                "Méretlen fővezeték"
            };

        public RootNodeDataView(RootNode rootNode)
        {
            InitializeComponent();
            RootNode = rootNode;
            DataContext = this;
        }
    }
}
