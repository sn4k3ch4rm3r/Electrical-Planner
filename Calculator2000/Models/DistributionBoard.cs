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
        private List<int> cableDiametersAllowed { get => Defaults.CableDiameters; }
        private List<int> fuseCurrentAllowed {get => Defaults.FuseCurrents; }

        public double Voltage { get { return voltage; } set { voltage = value; UpdateProperties(); } }
        public double SimultanetyFactor { get => simultanetyFactor; set { simultanetyFactor = value; UpdateProperties(); } }
        public double GrowthFactor { get => growthFactor; set { growthFactor = value; UpdateProperties(); } }
        public double ReserveFactor { get => reserveFactor; set { reserveFactor = value; UpdateProperties(); } }
        public double CableLength { get => cableLength; set { cableLength = value; UpdateProperties(); } }
        public string CableMaterial { get => cableMaterial; set { cableMaterial = value; UpdateProperties(); } }
        [JsonIgnore]
        public MaterialProperty CableMaterialProperty { get => MaterialProperties.Values[cableMaterial]; set { cableMaterial = value.ChemicalSymbol; UpdateProperties(); } }
        public int Phase { get => phase; set { phase = value; UpdateProperties(); } }
        public double MaximumVoltageDropAllowed { get => maximumVoltageDropAllowed; set { maximumVoltageDropAllowed = value; UpdateProperties(); } }

        private double voltage = 400;
        private int cableDiameter = 10;
        private int fuseCurrent = 10;
        private double simultanetyFactor = 0.6;
        private double growthFactor = 1.3;
        private double reserveFactor = 1;
        private double cableLength = 0;
        private string cableMaterial = "Cu";
        private int phase = 3;
        private double maximumVoltageDropAllowed = 0.8;


        private int getCableDiameter()
        {
            int diameter = cableDiametersAllowed[0];
            double drop = getVoltageDrop(diameter, FuseCurrent);

            while (drop > MaximumVoltageDropAllowed || FuseCurrent * 0.8 > CableMaterialProperty.MaxCurrent(diameter))
            {
                int index = cableDiametersAllowed.IndexOf(diameter) + 1;
                if (index > cableDiametersAllowed.Count - 1)
                    break;
                diameter = cableDiametersAllowed[index];
                drop = getVoltageDrop(diameter, FuseCurrent);
            }
            return diameter;
        }

        private int getFuseCurrent()
        {
            int current = fuseCurrentAllowed[0];
            while (current * 0.8 < ScaledCurrent)
            {
                int index = fuseCurrentAllowed.IndexOf(current) + 1;
                if (index > fuseCurrentAllowed.Count - 1)
                    break;
                current = fuseCurrentAllowed[index];
            }
            return current;
        }

        private double getVoltageDrop(int cableDiameter, int fuseCurrent)
        {
            double drop;
            if (Phase == 3)
                drop = (Math.Sqrt(3) * fuseCurrent * CableLength) / (cableDiameter * CableMaterialProperty.SpecificConductivity);
            else
                drop = (2 * fuseCurrent * CableLength) / (cableDiameter * CableMaterialProperty.SpecificConductivity);

            if (Parent.GetType() == typeof(RootNode))
                drop += (Parent as RootNode).UnmeasuredDrop + (Parent as RootNode).MeasuredDrop;
            else
                drop += (Parent as DistributionBoard).VoltageDropV;
            return drop * 100 / Voltage;
        }

        public int CableDiameter
        {
            get
            {
                if(!AutoCable) return cableDiameter;

                cableDiameter = getCableDiameter();
                OnPropertyChanged("VoltageDrop");
                return cableDiameter;
            }
            set
            {
                AutoCable = value == -1;
                cableDiameter = value;
                UpdateProperties();
            }
        }

        public int FuseCurrent
        {
            get
            {
                if (!AutoFuse) return fuseCurrent;

                return getFuseCurrent();
            }
            set
            {
                AutoFuse = value == -1;
                fuseCurrent = value;
                UpdateProperties();
            }
        }

        public bool AutoCable { get; set; } = true;
        public bool AutoFuse { get; set; } = true;

        [JsonIgnore]
        public double VoltageDrop
        {
            get
            {
                return getVoltageDrop(cableDiameter, FuseCurrent);
            }
        }

        [JsonIgnore]
        public double VoltageDropV
        {
            get { return VoltageDrop * Voltage / 100; }
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
                if (columnName == "CableDiameter" && CableDiameter < getCableDiameter())
                    return "Túl kicsi keresztmetszet!";
                if (columnName == "FuseCurrent" && FuseCurrent < getFuseCurrent())
                    return "Túl kicsi biztosíték!";
                return null;
            }
        }

        public DistributionBoard() : base()
        {
            Name = "E.x";
            AllowedParentTypes = new List<Type> { typeof(RootNode), typeof(DistributionBoard) };
        }

        private void UpdateProperties()
        {
            OnPropertyChanged("Current");
            OnPropertyChanged("ScaledCurrent");
            OnPropertyChanged("FuseCurrent");
            OnPropertyChanged("Power");
            OnPropertyChanged("ScaledPower");
            OnPropertyChanged("CableDiameter");
            OnPropertyChanged("VoltageDrop");
            OnPropertyChanged("AutoCable");
            OnPropertyChanged("AutoFuse");
            OnUpdate();
        }
    }
}
