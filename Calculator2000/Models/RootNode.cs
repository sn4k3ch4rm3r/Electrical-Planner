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
        private int unmeasuredDistance = 0;
        private int measuredDistance = 0;
        private string unmeasuredMaterial = "Cu";
        private string measuredMaterial = "Cu";
        private int unmeasuredDiameter = 16;
        private int measuredDiameter = 16;

        public int UnmeasuredDistance { get => unmeasuredDistance; set { unmeasuredDistance = value; UpdateProperties(); } }
        public int MeasuredDistance { get => measuredDistance; set { measuredDistance = value; UpdateProperties(); } }
        public string UnmeasuredMaterial { get => unmeasuredMaterial; set { unmeasuredMaterial = value; UpdateProperties(); } }
        public string MeasuredMaterial { get => measuredMaterial; set { measuredMaterial = value; UpdateProperties(); } }
        public int UnmeasuredDiameter { get => unmeasuredDiameter; set { unmeasuredDiameter = value; UpdateProperties(); } }
        public int MeasuredDiameter { get => measuredDiameter; set { measuredDiameter = value; UpdateProperties(); } }

        [JsonIgnore]
        public double UnmeasuredDrop
        {
            get
            {
                if(unmeasuredDistance == 0) return 0;
                return (Math.Sqrt(3) * Current * UnmeasuredDistance) / (UnmeasuredDiameter * MaterialProperties.Values[UnmeasuredMaterial].SpecificConductivity);
            }
        }

        [JsonIgnore]
        public double MeasuredDrop
        {
            get
            {
                if (measuredDistance == 0) return 0;
                return (Math.Sqrt(3) * Current * MeasuredDistance) / (MeasuredDiameter * MaterialProperties.Values[MeasuredMaterial].SpecificConductivity);
            }
        }

        [JsonIgnore]
        public Action Updated { get; set; }

        [JsonIgnore]
        public string CablePart
        {
            get => cablePart;
            set
            {
                cablePart = value;
                UpdateProperties();
                OnUpdate();
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
                    MeasuredMaterial = value.ChemicalSymbol;
                else
                    UnmeasuredMaterial = value.ChemicalSymbol;
                UpdateProperties();
            }
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MaterialProperties.Values[MeasuredMaterial];
                else
                    return MaterialProperties.Values[UnmeasuredMaterial];
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
        public double PartVoltageDrop
        {
            get
            {
                if (cablePart == "Mért fővezeték")
                    return MeasuredDrop;
                else
                    return UnmeasuredDrop;
            }
        }

        [JsonIgnore]
        public double VoltageDrop
        {
            get
            {
                return MeasuredDrop + UnmeasuredDrop;
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
            OnPropertyChanged("Distance");
            OnPropertyChanged("CableMaterialProperty");
            OnPropertyChanged("CableDiameter");
            OnPropertyChanged("PartVoltageDrop");
            OnUpdate();
        }

        public override void OnUpdate()
        {
            if (Updated != null)
                Updated.Invoke();
            base.OnUpdate();
        }
    }
}
