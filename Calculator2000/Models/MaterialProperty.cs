using Calculator2000.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2000.Models
{
    public class MaterialProperty
    {
        public MaterialProperty(string chemicalSymbol, string name, double specificConductivity)
        {
            ChemicalSymbol = chemicalSymbol;
            Name = name;
            SpecificConductivity = specificConductivity;
        }

        public string ChemicalSymbol { get; }
        public string Name { get; }
        public double SpecificConductivity { get; }

        public int MaxCurrent(int diameter)
        {
            if (ChemicalSymbol == "Cu")
                return CableMaxCurrentDB.COPPER[diameter];
            return CableMaxCurrentDB.ALUMINIUM[diameter];
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
