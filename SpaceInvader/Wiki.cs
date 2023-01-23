using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public class Wiki : Menu
    {
        public Wiki(WikiMenuOption[] Options) : base(Options, 50, "Wiki", Blue)
        {

        }
    }
    public class WikiMenuOption : MenuOption
    {
        public WikiMenuOption(string Name, string Description, ConsoleColor Color) : base(Name, Description, Color)
        {

        }
    }

}
