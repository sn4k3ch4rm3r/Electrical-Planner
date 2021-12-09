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
        public override string Name { get => name; set { name = value; UpdateHeader(); } }

        public int Number 
        { 
            get 
            {
                return MainWindow.Floors[floor].IndexOf(this) + 1;
            } 
        }

        public string Floor { 
            get => floor; 
            set
            {
                MainWindow.Floors[floor].Remove(this);
                foreach (Room room in MainWindow.Floors[floor])
                {
                    room.UpdateHeader();
                }
                if (!MainWindow.Floors.Keys.Contains(value))
                    MainWindow.Floors[value] = new List<Room>();
                MainWindow.Floors[value].Add(this);
                
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

        public void UpdateHeader()
        {
            this.TreeViewItem.Header = this.ToString();
            OnUpdate();
        }
    }
}
