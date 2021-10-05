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
        public int Number { get; set; }
        public string Floor { get; set; }

        [JsonIgnore]
        public double ReactivePower { get; }

        public Room() : base()
        {
            Name = "Szoba";
        }
    }
}
