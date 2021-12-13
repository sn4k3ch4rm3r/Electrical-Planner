using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Calculator2000.Models;

namespace Calculator2000.Data
{
    public class Defaults
    {
        public static List<string> RoomNames { get; private set; }
        public static List<ConsumerProperties> Consumers { get; private set; }

        public static List<int> CableDiameters { get; } = new List<int>() { 10, 16, 20, 25, 35, 50, 70, 95 };
        public static Dictionary<int, string> CableDiameterPicker { get; private set; } = new Dictionary<int, string>()
        {
            { -1, "Automatikus" },
        };

        public static List<int> FuseCurrents { get; } = new List<int>()
        {
            10, 16, 20, 25, 32, 40, 50, 63, 80, 100, 125
        };
        public static Dictionary<int, string> FusePicker { get; private set; } = new Dictionary<int, string>()
        {
            { -1, "Automatikus" }
        };

        public static void Init()
        {
            foreach (int diameter in CableDiameters)
            {
                CableDiameterPicker[diameter] = diameter.ToString();
            }

            foreach (int fuse in FuseCurrents)
            {
                FusePicker[fuse] = fuse.ToString();
            }

            string[] rooms = File.ReadAllLines("Resources/szobanevek.csv");
            RoomNames = rooms.ToList();
            RoomNames.RemoveAt(0);

            string[] consumers = File.ReadAllLines("Resources/fogyasztok.csv");
            Consumers = new List<ConsumerProperties>();
            for (int i = 1; i < consumers.Length; i++)
            {
                string[] data = consumers[i].Split(';');
                Consumers.Add(
                    new ConsumerProperties(
                        data[0],
                        double.Parse(data[1]),
                        int.Parse(data[2]),
                        double.Parse(data[3])
                    )    
                );
            }
        }
    }
}
