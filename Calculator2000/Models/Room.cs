using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Calculator2000.Models
{
    public class Room : Node
    {
        private string floor = "0";

        private string name = "Szoba";
        public override string Name { get => name; set { name = value; OnUpdate(); } }

        public int Number 
        { 
            get 
            {
                if(!MainWindow.Floors.ContainsKey(floor)) return -1;
                return MainWindow.Floors[floor].IndexOf(this) + 1;
            } 
        }

        public string Floor { 
            get => floor; 
            set
            {
                if(MainWindow.Floors[floor].Contains(this)) {
                    MainWindow.Floors[floor].Remove(this);
                    foreach (Room room in MainWindow.Floors[floor])
                    {
                        room.OnUpdate();
                    }
                    if (!MainWindow.Floors.Keys.Contains(value))
                        MainWindow.Floors[value] = new List<Room>();
                    MainWindow.Floors[value].Add(this);
                
                }
                floor = value;
                TreeViewItem.Header = ToString();
            }
        }

        [JsonIgnore]
        public double ReactivePower { get; }

        public override string ToString()
        {
            return $"{Floor}.{Number} {Name}";
        }

        public override void RemoveRooms()
        {
            MainWindow.Floors[floor].Remove(this);
            foreach (Room room in MainWindow.Floors[floor])
            {
                room.OnUpdate();
            }
        }

        public Room()
        {
            AllowedParentTypes = new List<Type> { typeof(DistributionBoard) };
        }
    }
}
