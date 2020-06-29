using System;
using System.Collections.Generic;
using System.Linq;

namespace FIFA20_Ultimate_Team_AutoBuyer.Methods
{
    public static class ChemistryStyle
    {
        private static readonly IEnumerable<ChemistryStyleItem> allChemistryStyles;

        static ChemistryStyle()
        {
            allChemistryStyles = new List<ChemistryStyleItem>
            {
                new ChemistryStyleItem { Id = 250, FriendlyName = "Basic" },
                new ChemistryStyleItem { Id = 251, FriendlyName = "Sniper" },
                new ChemistryStyleItem { Id = 252, FriendlyName = "Finisher" },
                new ChemistryStyleItem { Id = 253, FriendlyName = "Deadeye" },
                new ChemistryStyleItem { Id = 254, FriendlyName = "Marksman" },
                new ChemistryStyleItem { Id = 255, FriendlyName = "Hawk" },
                new ChemistryStyleItem { Id = 256, FriendlyName = "Artist" },
                new ChemistryStyleItem { Id = 257, FriendlyName = "Architect" },
                new ChemistryStyleItem { Id = 258, FriendlyName = "Powerhouse" },
                new ChemistryStyleItem { Id = 259, FriendlyName = "Maestro" },
                new ChemistryStyleItem { Id = 260, FriendlyName = "Engine" },
                new ChemistryStyleItem { Id = 261, FriendlyName = "Sentinel" },
                new ChemistryStyleItem { Id = 262, FriendlyName = "Guardian" },
                new ChemistryStyleItem { Id = 263, FriendlyName = "Gladiator" },
                new ChemistryStyleItem { Id = 264, FriendlyName = "Backbone" },
                new ChemistryStyleItem { Id = 265, FriendlyName = "Anchor" },
                new ChemistryStyleItem { Id = 266, FriendlyName = "Hunter" },
                new ChemistryStyleItem { Id = 267, FriendlyName = "Catalyst" },
                new ChemistryStyleItem { Id = 268, FriendlyName = "Shadow" },
                new ChemistryStyleItem { Id = 269, FriendlyName = "Wall" },
                new ChemistryStyleItem { Id = 270, FriendlyName = "Shield" },
                new ChemistryStyleItem { Id = 271, FriendlyName = "Cat" },
                new ChemistryStyleItem { Id = 272, FriendlyName = "Glove" },
                new ChemistryStyleItem { Id = 273, FriendlyName = "GK Basic" }
            };
        }

        public static int GetID(string chemistryStyleName)
        {
            if (chemistryStyleName == null) throw new ArgumentNullException("chemistryStyleName");
            if (string.IsNullOrWhiteSpace(chemistryStyleName)) throw new ArgumentException("chemistryStyleName");
            return allChemistryStyles.Where(player => string.Equals(player.FriendlyName, chemistryStyleName, StringComparison.OrdinalIgnoreCase)).Select(i => i.Id).FirstOrDefault();
        }

        public static string GetName(int chemistryStyleID)
        {
            if (chemistryStyleID <= 0) throw new ArgumentException("chemistryStyleName");
            return allChemistryStyles.Where(i => i.Id == chemistryStyleID).Select(i => i.FriendlyName).FirstOrDefault();
        }

        public static List<string> ReturnAllChemistrystyles()
        {
            return allChemistryStyles.Select(p => p.FriendlyName).ToList();
        }
    }
}
