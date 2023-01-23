using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public class SimpleMenuOption
    {
        public string Name { get; set; }
        public ConsoleColor Color { get; set; }
        public SimpleMenuOption(string Text, ConsoleColor Color)
        {
            this.Name = Text;
            this.Color = Color;
        }
    }
}