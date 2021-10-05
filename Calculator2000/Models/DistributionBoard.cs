using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Calculator2000.Models
{
    public class DistributionBoard : Node
    {
        private double _Voltage = 400;
        public double Voltage { get { return _Voltage; } set { _Voltage = value; } }
        public double SimultanetyFactor { get; set; } = 0.6;
        public double GrowthFactor { get; set; } = 1.3;
        public double ReserveFactor { get; set; } = 1;
        public Cable Cable { get; set; } = new Cable();
        public int Phase { get; set; } = 3;
        public double MaximumVoltageDropAllowed { get; set; } = 0.8;

        [JsonIgnore]
        public double Current
        {
            get
            {
                if (Phase == 3)
                    return UsedPower / (Math.Sqrt(3) * Voltage * 1);
                else
                    return UsedPower / (Voltage * 2 * 1);
            }
        }
        [JsonIgnore]
        public double ScaledCurrent 
        { 
            get
            {
                if (Phase == 3)
                    return ScaledPower / (Math.Sqrt(3) * Voltage * 1);
                else
                    return ScaledPower / (Voltage * 2 * 1);
            }
        }
        [JsonIgnore]
        public double FuseCurrent { get; }
        [JsonIgnore]
        public double ReactivePower { get; }

        [JsonIgnore]
        public double ScaledPower
        {
            get
            {
                return UsedPower * GrowthFactor * ReserveFactor;
            }
        }

        public DistributionBoard() : base()
        {
            Name = "E.x";
        }
    }
}
