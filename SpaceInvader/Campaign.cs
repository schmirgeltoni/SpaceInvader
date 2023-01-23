using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    internal class CampaignLevel : Level
    {

    }
    internal class CampaignLevelMenuOption : MenuOption
    {       
        public CampaignLevel Level { get; set; }
        public CampaignLevelMenuOption(string Name, string Description, ConsoleColor Color, CampaignLevel Level) : base(Name, Description, Color)
        {           
            this.Level = Level;
        }
    }
    internal class Campaign : Menu
    {               
        public Campaign(CampaignLevelMenuOption[] Options) : base(Options, 50, "Campaign", Yellow) { }      
    }
}