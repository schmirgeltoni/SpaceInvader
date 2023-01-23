using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public class Menu
    {
        public MenuOption[] Options { get; set; }
        private ConsoleColor MenuColor { get; set; }
        public string MenuName { get; set; }
        public int LongestLevelName { get; set; }
        public int PositionOfFirstVerticalLine { get; set; }
        public int LeftPositionOfDescription { get; set; }
        public int TopPositionOfDescription { get; set; }
        public int RightPositionOfDescription { get; set; }       
        public Menu(MenuOption[] Options, int DescriptionSize, string MenuName, ConsoleColor MenuColor)
        {
            this.Options = Options;
            this.MenuName = MenuName;
            this.MenuColor = MenuColor;
            LongestLevelName = 0;
            for (int i = 0; i < Options.Length; i++)
            {
                if (Options[i].Name.Length > LongestLevelName)
                    LongestLevelName = Options[i].Name.Length;
            }
            PositionOfFirstVerticalLine = LongestLevelName + 3;
            LeftPositionOfDescription = PositionOfFirstVerticalLine + 2;
            TopPositionOfDescription = 5;
            RightPositionOfDescription = PositionOfFirstVerticalLine + DescriptionSize;
        }
        public int GoThroughMenu(ConsoleKey Key, int SelectedOption)
        {
            if ((Key == ConsoleKey.UpArrow || Key == ConsoleKey.W) && SelectedOption != 0)
            {
                SelectedOption--;
            }
            else if ((Key == ConsoleKey.DownArrow || Key == ConsoleKey.S) && SelectedOption < Options.Length - 1)
            {
                SelectedOption++;
            }
            else if ((Key == ConsoleKey.UpArrow || Key == ConsoleKey.W) && SelectedOption == 0) //Select flips to the end
            {
                SelectedOption = Options.Length - 1;
            }
            else if ((Key == ConsoleKey.DownArrow || Key == ConsoleKey.S) && SelectedOption == Options.Length - 1)  //Select flips to the start
            {
                SelectedOption = 0;
            }
            return SelectedOption;
        }
        public void PrintMenu(int SelectedOption)
        {           
            PrintHorizontalLine(RightPositionOfDescription + 3, 0, Options.Length * 2 + 3);
            
            ClearMenuOptionDescription();
            
            WriteLine();
            WriteLine();

            PrintMenuOptionDescription(SelectedOption);

            PrintVerticalLine(Options.Length * 2 + 3, PositionOfFirstVerticalLine, 2);
            PrintVerticalLine(Options.Length * 2 + 3, RightPositionOfDescription + 2, 2);
            PrintVerticalLine(Options.Length * 2 + 3, 0, 2);
            
            ForegroundColor = MenuColor;
            SetCursorPosition(0,0);
            Write(MenuName);

            PrintHorizontalLine(RightPositionOfDescription + 3, 0, 1);
            PrintMenuOptionNames(SelectedOption);          
        }
        private void PrintMenuOptionNames(int SelectedOption)
        {
            CursorTop = 3;
            for (int i = 0; i < Options.Length; i++)
            {
                CursorLeft = 2;
                ForegroundColor = Options[i].Color;
                if (i == SelectedOption)
                {
                    ForegroundColor = Black;
                    BackgroundColor = Options[i].Color;
                }
                WriteLine(Options[i].Name);
                if (i == SelectedOption)
                {
                    BackgroundColor = Black;
                }
                WriteLine();
            }
        }
        private void PrintMenuOptionDescription(int SelectedOption)
        {
            ForegroundColor = White;
            SetCursorPosition(LeftPositionOfDescription, 3);

            ForegroundColor = Options[SelectedOption].Color;
            Write(Options[SelectedOption].Name);
            ForegroundColor = White;

            SetCursorPosition(LeftPositionOfDescription, TopPositionOfDescription);
            string[] Description = Options[SelectedOption].Description.Split(' ');
            for (int i = 0; i < Description.Length; i++)
            {
                if (Description[i].Length + CursorLeft > RightPositionOfDescription + 1)
                    SetCursorPosition(LeftPositionOfDescription, CursorTop + 1);
                Write(Description[i]);
                Write(' ');
            }
        }
        private void ClearMenuOptionDescription()
        {
            SetCursorPosition(LeftPositionOfDescription, 3);
            for (int i = CursorTop; i < Options.Length * 2 + 3; i++)
            {
                for (int j = 0; j < RightPositionOfDescription - LeftPositionOfDescription + 1; j++)
                {
                    Write(" ");
                }
                SetCursorPosition(LeftPositionOfDescription, i);
            }
        }
        static private void PrintVerticalLine(int LengthOfLine, int StartPositionX, int StartPositionY)
        {
            ForegroundColor = Gray;
            SetCursorPosition(StartPositionX, StartPositionY);
            for (int i = StartPositionY; i < LengthOfLine; i++)
            {
                SetCursorPosition(StartPositionX, i);
                Write('|');
            }
        }
        static private void PrintHorizontalLine(int LengthOfLine, int StartPositionX, int StartPositionY)
        {
            ForegroundColor = Gray;
            SetCursorPosition(StartPositionX, StartPositionY);
            for (int i = 0; i < LengthOfLine; i++)
                Write('-');
        }
    }
    public class MenuOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ConsoleColor Color { get; set; }
        public MenuOption(string Name, string Description, ConsoleColor Color)
        {
            this.Name = Name;
            this.Description = Description;
            this.Color = Color;
        }       
    }
}