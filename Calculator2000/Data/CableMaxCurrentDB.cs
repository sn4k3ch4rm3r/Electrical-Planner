using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator2000.Data
{
    public static class CableMaxCurrentDB
    {
        public static Dictionary<int, int> COPPER = new Dictionary<int, int>()
        {
            {10, 48},
            {16, 63},
            {20, 83},
            {25, 110},
            {35, 140},
            {50, 175},
            {70, 215},
            {95, 255}
        };

        public static Dictionary<int, int> ALUMINIUM = new Dictionary<int, int>()
        {
            {10, 36},
            {16, 51},
            {20, 65},
            {25, 86},
            {35, 110},
            {50, 140},
            {70, 175},
            {95, 205}
        };
    }
}
