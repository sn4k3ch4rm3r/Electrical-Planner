using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator2000.Models;

namespace Calculator2000.Data
{
    public class MaterialProperties
    {
        public static MaterialProperty COPPER = new MaterialProperty("Cu", "Réz", 56);
        public static MaterialProperty ALUMINIUM = new MaterialProperty("Al", "Aluminium", 35);

        public static Dictionary<string, MaterialProperty> Values = new Dictionary<string, MaterialProperty>()
        {
            {"Cu", COPPER},
            {"Al", ALUMINIUM}
        };

    }
}
