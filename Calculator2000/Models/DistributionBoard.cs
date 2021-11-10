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
    public class DistributionBoard : Node, INotifyPropertyChanged, IDataErrorInfo
    {
        private List<int> cableDiametersAllowed = new List<int>()
        {
            10, 16, 20, 25, 35, 50, 70, 95
        };

        private List<int> fuseCurrentAllowed = new List<int>()
        {
            10, 16, 20, 25, 32, 40, 50, 63, 80, 100, 125
        };

        public double Voltage { get { return voltage; } set { voltage = value; UpdateProperties(); } }
        public double SimultanetyFactor { get => simultanetyFactor; set { simultanetyFactor = value; UpdateProperties(); } }
        public double GrowthFactor { get => growthFactor; set { growthFactor = value; UpdateProperties(); } }
        public double ReserveFactor { get => reserveFactor; set { reserveFactor = value; UpdateProperties(); } }
        public double CableLength { get => cableLength; set { cableLength = value; UpdateProperties(); } }
        public MaterialProperty CableMaterialProperty { get => cableMaterialProperty; set { cableMaterialProperty = value; UpdateProperties(); } }
        public int Phase { get => phase; set => phase = value; }
        public double MaximumVoltageDropAllowed { get => maximumVoltageDropAllowed; set { maximumVoltageDropAllowed = value; UpdateProperties(); } }

        private double voltage = 400;
        private int cableDiameter = 10;
        private double simultanetyFactor = 0.6;
        private double growthFactor = 1.3;
        private double reserveFactor = 1;
        private double cableLength = 0;
        private MaterialProperty cableMaterialProperty = MaterialProperties.COPPER;
        private int phase = 3;
        private double maximumVoltageDropAllowed = 0.8;

        [JsonIgnore]
        public int CableDiameter
        {
            get
            {
                cableDiameter = 10;
                while (VoltageDrop > MaximumVoltageDropAllowed || FuseCurrent * 0.8 > CableMaterialProperty.MaxCurrent(cableDiameter))
                {
                    int index = cableDiametersAllowed.IndexOf(cableDiameter) + 1;
                    if (index > cableDiametersAllowed.Count-1)
                        break;
                    cableDiameter = cableDiametersAllowed[index];
                }
                OnPropertyChanged(new PropertyChangedEventArgs("VoltageDrop"));
                return cableDiameter;
            }
        }

        [JsonIgnore]
        public double VoltageDrop
        {
            get
            {
                double drop;
                if (Phase == 3)
                    drop = (Math.Sqrt(3) * FuseCurrent * CableLength) / (cableDiameter * CableMaterialProperty.SpecificConductivity);
                else
                    drop = (2 * FuseCurrent * CableLength) / (cableDiameter * CableMaterialProperty.SpecificConductivity);
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
                while (curr * 0.8 < ScaledCurrent)
                {
                    int index = fuseCurrentAllowed.IndexOf(curr) + 1;
                    if (index > fuseCurrentAllowed.Count - 1)
                        break;
                    curr = fuseCurrentAllowed[index];
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

        public string Error => null;

        public string this[string columnName] 
        {
            get
            {
                if (columnName == "VoltageDrop" && VoltageDrop > MaximumVoltageDropAllowed)
                    return "Túl nagy feszültség esés!";
                if (columnName == "FuseCurrent" && ScaledCurrent > FuseCurrent)
                    return "Nem elég nagy biztosíték!";
                return null;
            }
        }

        public DistributionBoard() : base()
        {
            Name = "E.x";
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Current"));
            OnPropertyChanged(new PropertyChangedEventArgs("ScaledCurrent"));
            OnPropertyChanged(new PropertyChangedEventArgs("FuseCurrent"));
            OnPropertyChanged(new PropertyChangedEventArgs("Power"));
            OnPropertyChanged(new PropertyChangedEventArgs("ScaledPower"));
            OnPropertyChanged(new PropertyChangedEventArgs("CableDiameter"));
            OnPropertyChanged(new PropertyChangedEventArgs("VoltageDrop"));
        }
    }
}
