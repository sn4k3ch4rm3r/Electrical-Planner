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
    /// Interaction logic for RoomDataView.xaml
    /// </summary>
    public partial class RoomDataView : Page
    {
        public Room Room { get; set; }

        public List<string> DefaultRoomNames { get; set; } = new List<string>()
        {
            "Szoba",
            "Háló",
            "Előkészítő",
            "Előtér",
            "Konyha",
            "Közlekedő",
            "Mosdó",
            "Mosókonyha",
            "Öltöző",
            "Raktár",
            "WC",
            "Zuhanyzó"
        };
        public RoomDataView(Room room)
        {
            InitializeComponent();
            Room = room;
            DataContext = this;
        }
    }
}
