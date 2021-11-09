using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Calculator2000.Data;
using Newtonsoft.Json;

namespace Calculator2000.Models
{
    public class DistributionBoard : Node
    {
        private List<int> cableDiametersAllowed = new List<int>()
        {
            10, 16, 20, 25, 35, 50, 70, 95
        };

        private List<int> fuseCurrentAllowed = new List<int>()
        {
            10, 16, 20, 25, 32, 40, 50, 63, 80, 100, 125
        };

        private double _Voltage = 400;
        public double Voltage { get { return _Voltage; } set { _Voltage = value; } }
        public double SimultanetyFactor { get; set; } = 0.6;
        public double GrowthFactor { get; set; } = 1.3;
        public double ReserveFactor { get; set; } = 1;
        public double CableLength { get; set; } = 0;
        public MaterialProperty CableMaterialProperty { get; set; } = MaterialProperties.COPPER;
        public int Phase { get; set; } = 3;
        public double MaximumVoltageDropAllowed { get; set; } = 0.8;

        private int _cableDiameter = 10;
        
        [JsonIgnore]
        public int CableDiameter 
        {
            get
            {
                _cableDiameter = 10;
                while (VoltageDrop > MaximumVoltageDropAllowed || FuseCurrent * 0.8 > CableMaterialProperty.MaxCurrent(_cableDiameter))
                    _cableDiameter = cableDiametersAllowed[cableDiametersAllowed.IndexOf(_cableDiameter) + 1];
                return _cableDiameter;
            }
        }

        [JsonIgnore]
        public double VoltageDrop
        {
            get
            {
                double drop;
                if (Phase == 3)
                    drop = (Math.Sqrt(3) * FuseCurrent * CableLength) / (_cableDiameter * CableMaterialProperty.SpecificConductivity);
                else
                    drop = (2 * FuseCurrent * CableLength) / (_cableDiameter * CableMaterialProperty.SpecificConductivity);
                return drop * 100 / Voltage;
            }
        }

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
        public double FuseCurrent
        {
            get
            {
                int curr = 10;
                while(curr * 0.8 < ScaledCurrent)
                {
                    curr = fuseCurrentAllowed[fuseCurrentAllowed.IndexOf(curr) + 1];
                }
                return curr;
            }
        }
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
