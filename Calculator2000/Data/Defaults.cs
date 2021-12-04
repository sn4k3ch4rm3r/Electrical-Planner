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

        public static void Init()
        {
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
