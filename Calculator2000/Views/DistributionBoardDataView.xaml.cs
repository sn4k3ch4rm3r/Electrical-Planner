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
using Calculator2000.Models;
using Calculator2000.Data;

namespace Calculator2000.Views
{
    /// <summary>
    /// Interaction logic for DistributionBoardDataView.xaml
    /// </summary>
    public partial class DistributionBoardDataView : Page
    {
        public DistributionBoard DistributionBoard { get; }
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

        public DistributionBoardDataView(DistributionBoard distributionBoard)
        {
            InitializeComponent();
            DistributionBoard = distributionBoard;
            DataContext = this;
        }
    }
}
