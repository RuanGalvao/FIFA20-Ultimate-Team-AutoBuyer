using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFA20_Ultimate_Team_Autobuyer.Methods
{
    public static class ChemistryStyle
    {
        private static readonly IEnumerable<Models.Filter> allChemistryStyles;

        static ChemistryStyle()
        {
            var s = new List<Models.Filter>
            {
                new Models.Filter { ID = 250, ChemistryStyle = "Basic" },
                new Models.Filter { ID = 251, ChemistryStyle = "Sniper" },
                new Models.Filter { ID = 252, ChemistryStyle = "Finisher" },
                new Models.Filter { ID = 253, ChemistryStyle = "Deadeye" },
                new Models.Filter { ID = 254, ChemistryStyle = "Marksman" },
                new Models.Filter { ID = 255, ChemistryStyle = "Hawk" },
                new Models.Filter { ID = 256, ChemistryStyle = "Artist" },
                new Models.Filter { ID = 257, ChemistryStyle = "Architect" },
                new Models.Filter { ID = 258, ChemistryStyle = "Powerhouse" },
                new Models.Filter { ID = 259, ChemistryStyle = "Maestro" },
                new Models.Filter { ID = 260, ChemistryStyle = "Engine" },
                new Models.Filter { ID = 261, ChemistryStyle = "Sentinel" },
                new Models.Filter { ID = 262, ChemistryStyle = "Guardian" },
                new Models.Filter { ID = 263, ChemistryStyle = "Gladiator" },
                new Models.Filter { ID = 264, ChemistryStyle = "Backbone" },
                new Models.Filter { ID = 265, ChemistryStyle = "Anchor" },
                new Models.Filter { ID = 266, ChemistryStyle = "Hunter" },
                new Models.Filter { ID = 267, ChemistryStyle = "Catalyst" },
                new Models.Filter { ID = 268, ChemistryStyle = "Shadow" },
                new Models.Filter { ID = 269, ChemistryStyle = "Wall" },
                new Models.Filter { ID = 270, ChemistryStyle = "Shield" },
                new Models.Filter { ID = 271, ChemistryStyle = "Cat" },
                new Models.Filter { ID = 272, ChemistryStyle = "Glove" },
                new Models.Filter { ID = 273, ChemistryStyle = "GK Basic" }
            };
            allChemistryStyles = s;
        }

        public static int GetID(string chemistryStyleName)
        {
            return allChemistryStyles.Where(player => string.Equals(player.ChemistryStyle, chemistryStyleName, StringComparison.OrdinalIgnoreCase)).Select(i => i.ID).FirstOrDefault();
        }

        public static string GetName(int chemistryStyleID)
        {
            return allChemistryStyles.Where(i => i.ID == chemistryStyleID).Select(i => i.ChemistryStyle).FirstOrDefault();
        }

        public static List<string> ReturnAllChemistrystyles()
        {
            return allChemistryStyles.Select(p => p.ChemistryStyle).ToList();
        }
    }
}
