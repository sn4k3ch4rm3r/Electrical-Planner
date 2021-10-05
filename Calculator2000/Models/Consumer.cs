using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Calculator2000.Models
{
    public class Consumer : Node
    {
        public override string Name
        {
            set
            {
                Properties.Name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
            get
            {
                return Properties.Name;
            }
        }
        public ConsumerProperties Properties { get; set; } = new ConsumerProperties("Dugaszoló aljzat", 600, 230, 0.60);
        public int Count { get; set; } = 1;

        [JsonIgnore]
        public override double UsedPower
        {
            get
            {
                return TotalPower * Properties.SimultanetyFactor;
            }
        }
        public override double TotalPower
        {
            get
            {
                return this.Count * Properties.Power;
            }
        }

        public Consumer() : base()
        {
            Name = "Dugaszoló aljzat";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
