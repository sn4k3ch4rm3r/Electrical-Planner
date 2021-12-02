using Calculator2000.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2000.Models
{
    public class RootNode : Node
    {
        private string cablePart = "Mért fővezeték";
        private double voltage = 400;

        public int UnmeasuredDistance { get; set; } = 0;
        public int MeasuredDistance { get; set; } = 0;
        public MaterialProperty UnmeasuredMaterial { get; set; } = MaterialProperties.COPPER;
        public MaterialProperty MeasuredMaterial { get; set; } = MaterialProperties.COPPER;
        public int UnmeasuredDiameter { get; set; } = 16;
        public int MeasuredDiameter { get; set; } = 16;
        public double UnmeasuredDrop { get; set; } = 0;
        public double MeasuredDrop { get; set; } = 0;

        [JsonIgnore]
        public string CablePart { get => cablePart; 
            set { 
                cablePart = value; 
                UpdateProperties();
                OnPropertyChanged("Distance");
                OnPropertyChanged("CableMaterialProperty");
                OnPropertyChanged("CableDiameter");
                OnPropertyChanged("VoltageDrop");
            } 
        }

        [JsonIgnore]
        public int Distance
        {
            set
            {
                if (CablePart == "Mért fővezeték")
                    MeasuredDistance = value;
                else
                    UnmeasuredDistance = value;
                UpdateProperties();
            }
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MeasuredDistance;
                else
                    return UnmeasuredDistance;
            }
        }

        [JsonIgnore]
        public MaterialProperty CableMaterialProperty
        {
            set
            {
                if (CablePart == "Mért fővezeték")
                    MeasuredMaterial = value;
                else
                    UnmeasuredMaterial = value;
                UpdateProperties();
            }
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MeasuredMaterial;
                else
                    return UnmeasuredMaterial;
            }
        }

        [JsonIgnore]
        public int CableDiameter
        {
            set
            {
                if (CablePart == "Mért fővezeték")
                    MeasuredDiameter = value;
                else
                    UnmeasuredDiameter = value;
                UpdateProperties();
            }
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MeasuredDiameter;
                else
                    return UnmeasuredDiameter;
            }
        }

        [JsonIgnore]
        public double VoltageDrop
        {
            set
            {
                if (CablePart == "Mért fővezeték")
                    MeasuredDrop = value;
                else
                    UnmeasuredDrop = value;
                UpdateProperties();
            }
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MeasuredDrop;
                else
                    return UnmeasuredDrop;
            }
        }

        public double Voltage { get => voltage; set { voltage = value; UpdateProperties(); } }
        
        [JsonIgnore]
        public double Current { get => UsedPower / (Math.Sqrt(3) * Voltage * 1); }

        [JsonIgnore]
        public override double UsedPower 
        { 
            get
            {
                return Children.Select(x => x as DistributionBoard).Sum(x => x.ScaledPower * x.SimultanetyFactor);
            } 
        }

        public RootNode()
        {
            this.Name = "Főelosztó";
        }
        private void UpdateProperties()
        {
            OnPropertyChanged("Current");
            OnPropertyChanged("TotalPower");
            OnPropertyChanged("UsedPower");
        }
    }
}
