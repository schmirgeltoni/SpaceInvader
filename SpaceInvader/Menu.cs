using static System.Console;
using static System.ConsoleColor;
/*  Boxdraw Characters:
 *  ═ ║ ╒ ╓ ╔ ╕ ╖ ╗ ╘ ╙ ╚ ╛ ╜ ╝ ╞ ╟ ╠ ╡ ╢ ╣ ╤ ╥ ╦ ╧ ╨ ╩ ╪ ╫ ╬

        ╔═══╗       :O 
        ║   ║
        ║   ║
        ╚═══╝
 */


namespace SpaceInvader
{
    public class Menu
    {
        public MenuOption[] Options { get; set; }       //Array of Options to be displayed
        private ConsoleColor MenuColor { get; set; }    //Main Color of the Menu
        public string MenuName { get; set; }
        public int LongestLevelName { get; set; }       //Lenght of longest name for menu width
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
            for (int i = 0; i < Options.Length; i++)    //Get longest level name from options
            {
                if (Options[i].Name.Length > LongestLevelName)
                    LongestLevelName = Options[i].Name.Length;
            }
            PositionOfFirstVerticalLine = LongestLevelName + 3;             //This is actually the second vertical line of a menu since the first line is trivial (Left = 0)
            LeftPositionOfDescription = PositionOfFirstVerticalLine + 2;    //Description Starts two spaces to the left of the line
            TopPositionOfDescription = 5;                                   //to leave space for name and lines
            RightPositionOfDescription = PositionOfFirstVerticalLine + DescriptionSize; //right border so the description can be 'boxed'
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
            PrintHorizontalLine(RightPositionOfDescription + 3, 0, 1, new KeyValuePair<int, char>[] {
                                                                       new KeyValuePair<int, char>(0, '╔'),
                                                                       new KeyValuePair<int, char>(PositionOfFirstVerticalLine, '╦'),
                                                                       new KeyValuePair<int, char>(RightPositionOfDescription + 2, '╗') });
            
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
            PrintHorizontalLine(RightPositionOfDescription + 3, 0, Options.Length * 2 + 3, new KeyValuePair<int, char>[] {
                                                                                            new KeyValuePair<int, char>(0, '╚'),
                                                                                            new KeyValuePair<int, char>(PositionOfFirstVerticalLine, '╩'),
                                                                                            new KeyValuePair<int, char>(RightPositionOfDescription + 2, '╝') });
            PrintMenuOptionNames(SelectedOption);          
        }
        private void PrintMenuOptionNames(int SelectedOption)
        {
            CursorTop = 3;
            for (int i = 0; i < Options.Length; i++)
            {
                CursorLeft = 2;
                ForegroundColor = Options[i].Color;
                if (i == SelectedOption) //Highlight the selected option
                {
                    ForegroundColor = Black;
                    BackgroundColor = Options[i].Color;
                }
                WriteLine(Options[i].Name);
                if (i == SelectedOption) //change background color
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
            for (int i = 0; i < Description.Length; i++) //reset cursor so it stays in the menu box
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
        static private void PrintVerticalLine(int LengthOfLine, int StartPositionX, int StartPositionY, params KeyValuePair<int, char>[] exceptions)
        {
            //the exceptions array contains the position and special boxdraw character that should be used in a line to make a clean menu
            ForegroundColor = Gray;
            SetCursorPosition(StartPositionX, StartPositionY);
            for (int i = StartPositionY; i < LengthOfLine; i++)
            {
                SetCursorPosition(StartPositionX, i);
                Write('║');
            }
            foreach (KeyValuePair<int, char> pair in exceptions)
            {
                CursorLeft = pair.Key;
                Write(pair.Value);
            }
        }
        static private void PrintHorizontalLine(int LengthOfLine, int StartPositionX, int StartPositionY, params KeyValuePair<int,char>[] exceptions)
        //the exceptions array contains the position and special boxdraw character that should be used in a line to make a clean menu
        {
            ForegroundColor = Gray;

            SetCursorPosition(StartPositionX, StartPositionY);
            for (int i = 0; i < LengthOfLine; i++)
                Write('═');
            foreach (KeyValuePair<int,char> pair in exceptions)
            {
                CursorLeft = pair.Key;
                Write(pair.Value);
            }
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