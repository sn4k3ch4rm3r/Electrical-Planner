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
                ConsumerProperties consumer = DefaultConsumers.Find(x => x.Name == value);
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
        public List<ConsumerProperties> DefaultConsumers { get; set; } = new List<ConsumerProperties>()
        {
            new ConsumerProperties("Dugaszoló aljzat", 600, 230, 0.60),
            new ConsumerProperties("Duralamp BT101N", 36, 230, 0.40),
            new ConsumerProperties("Duralamp LC001", 20, 230, 0.40),
            new ConsumerProperties("Eglo Turcona 60x60 felületre szerelhető", 35, 230, 0.40),
            new ConsumerProperties("Gymled", 286, 230, 0.90),
            new ConsumerProperties("Helios elszívó ventilátor DX200", 34, 230, 0.40),
            new ConsumerProperties("Helios elszívó ventilátor DX400", 71, 230, 0.40),
            new ConsumerProperties("Led szalag RGBW", 18, 230, 0.40),
            new ConsumerProperties("Légkondícionáló", 800, 230, 0.40),
            new ConsumerProperties("Menekülési útvonaljelző", 10, 230, 1.00),
            new ConsumerProperties("Opple LEDpanel" , 30, 230, 0.40),
            new ConsumerProperties("Trillux 74R WD2", 19, 230, 0.40),
            new ConsumerProperties("Trilux 8851 RB/950-730 2G1S kültéri álló lámpa", 10, 230, 0.40),
            new ConsumerProperties("Trilux Belviso D", 29, 230, 0.40),
            new ConsumerProperties("Trilux SWFlow D2-L", 27, 230, 0.40),
            new ConsumerProperties("Tükörvilágító lámpa", 40, 230, 0.40)
        };

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
