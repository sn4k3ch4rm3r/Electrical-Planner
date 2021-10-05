using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator2000.Data;

namespace Calculator2000.Models
{
    public class Cable
    {
        public double Length { get; set; } = 0;
        public int Diameter { get; set; } = 10;
        public MaterialProperty MaterialProperty { get; set; } = MaterialProperties.COPPER;
    }
}
