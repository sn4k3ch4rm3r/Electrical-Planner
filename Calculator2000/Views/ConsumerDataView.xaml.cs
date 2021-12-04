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
using Calculator2000.Data;
using Calculator2000.Models;

namespace Calculator2000.Views
{
    /// <summary>
    /// Interaction logic for ConsumerDataView.xaml
    /// </summary>
    public partial class ConsumerDataView : Page
    {
        public Consumer Consumer { get; set; }

        public string SelectedName
        {
            get
            {
                return Consumer.Name;
            }
            set
            {
                Consumer.Name = value;
                ConsumerProperties consumer = DefaultConsumers.Find(x => x.Name == value).Clone();
                if (consumer != null)
                {
                    Consumer.Properties = consumer;
                }
            }
        }

        public List<int> Voltages { get; } = new List<int>()
            {
                400, 230
            };
        public List<ConsumerProperties> DefaultConsumers
        {
            get
            {
                return Defaults.Consumers;
            }
        }

        public List<string> DefaultConsumerNames
        {
            get
            {
                return DefaultConsumers.Select(x => x.Name).ToList();
            }
        }

        public ConsumerDataView(Consumer consumer)
        {
            InitializeComponent();
            Consumer = consumer;
            DataContext = this;
        }
    }
}
