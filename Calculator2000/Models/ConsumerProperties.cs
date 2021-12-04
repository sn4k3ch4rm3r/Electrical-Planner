using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2000.Models
{
    public class ConsumerProperties
    {
        public ConsumerProperties(string name, double power, int voltage, double simultanetyFactor)
        {
            Name = name;
            Voltage = voltage;
            Power = power;
            SimultanetyFactor = simultanetyFactor;
        }

        public string Name { get; set; }
        public int Voltage { get; set; }
        public double Power { get; set; }
        public double SimultanetyFactor { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public ConsumerProperties Clone()
        {
            return new ConsumerProperties(this.Name, this.Power, this.Voltage, this.SimultanetyFactor);
        }
    }
}
