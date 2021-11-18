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
        private ConsumerProperties properties = new ConsumerProperties("Dugaszoló aljzat", 600, 230, 0.60);

        public override string Name
        {
            set
            {
                Properties.Name = value;
                TreeViewItem.Header = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Properties.Name");
            }
            get
            {
                return Properties.Name;
            }
        }
        public ConsumerProperties Properties { get => properties; 
            set 
            {
                properties = value;
                OnPropertyChanged("Properties");
                OnPropertyChanged("Properties.Name");
                OnPropertyChanged("Properties.Voltage");
                OnPropertyChanged("Properties.Power");
                OnPropertyChanged("Properties.SimultanetyFactor");
            } 
        }
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
