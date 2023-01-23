using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public class Display
    {
        public static bool DisplayTwoOptions(SimpleMenuOption[] Menu, int LineTop, int LineBottom, bool IsEscapeSupposedToReturnFalse)
        {
            CursorVisible = false;
            SetCursorPosition(0, LineTop);
            while (true)
            {
                ConsoleKey RestartKey = ReadKey(true).Key;
                if (RestartKey == ConsoleKey.UpArrow || RestartKey == ConsoleKey.W || RestartKey == ConsoleKey.DownArrow || RestartKey == ConsoleKey.S)
                {
                    if (GetCursorPosition().Top == LineTop)
                    {
                        CursorLeft = 0;
                        ForegroundColor = Menu[0].Color; BackgroundColor = Black; Write(Menu[0].Name);
                        SetCursorPosition(0, LineBottom);
                        BackgroundColor = Menu[1].Color; ForegroundColor = Black; Write(Menu[1].Name);
                    }
                    else if (GetCursorPosition().Top == LineBottom)
                    {
                        CursorLeft = 0;
                        ForegroundColor = Menu[1].Color; BackgroundColor = Black; Write(Menu[1].Name);
                        SetCursorPosition(0, LineTop);
                        BackgroundColor = Menu[0].Color; ForegroundColor = Black; Write(Menu[0].Name);
                    }
                }
                else if (RestartKey == ConsoleKey.Escape && IsEscapeSupposedToReturnFalse)
                {
                    BackgroundColor = Black;
                    return false;
                }
                else if (RestartKey == ConsoleKey.Escape && !IsEscapeSupposedToReturnFalse)
                {
                    BackgroundColor = Black;
                    return true;
                }
                else if (RestartKey == ConsoleKey.Enter || RestartKey == ConsoleKey.Spacebar)
                {
                    if (GetCursorPosition().Top == LineTop)
                    {
                        BackgroundColor = Black;
                        return false;
                    }
                    else if (GetCursorPosition().Top == LineBottom)
                    {
                        BackgroundColor = Black;
                        return true;
                    }
                }
            }
        }
        public static string[] ScreenStringArray(string filename)
        {
            StreamReader Screen = new(filename);
            int lineCount = 0;
            while (!Screen.EndOfStream)
            {
                Screen.ReadLine();
                lineCount++;
            }
            Screen.Close();

            Screen = new(filename);
            string[] lines = new string[lineCount];
            int index = 0;
            while (!Screen.EndOfStream)
            {
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
                lines[index++] = Screen.ReadLine();
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
            }
            Screen.Close();
            return lines;
        }
        public static void DisplayTitleScreen()
        {
            string[] firstTitle = ScreenStringArray(@"..\..\..\Title1.txt");
            string[] secondTitle = ScreenStringArray(@"..\..\..\Title2.txt");
            while (!KeyAvailable)
            {
                ScreenUpDown(firstTitle, true, true);
                Thread.Sleep(1000);
                SetCursorPosition(0, 0);
                ScreenUpDown(secondTitle, false, true);
                Thread.Sleep(1000);
                SetCursorPosition(0, 0);
            }
            Clear();
        }
        public static void ScreenUpDown(string[] lines, bool UpToDown, bool Colorswitch)
        {
            int ColorSwitch;
            if (UpToDown && Colorswitch)
            {
                ColorSwitch = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    ForegroundColor = Yellow;
                    if (ColorSwitch > 8)
                        ForegroundColor = DarkMagenta;
                    WriteLine(lines[i]);
                    ForegroundColor = White;
                    ColorSwitch++;
                    Thread.Sleep(20);
                }
            }
            else if (Colorswitch)
            {
                ColorSwitch = lines.Length - 1;
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    SetCursorPosition(0, i);
                    ForegroundColor = Yellow;
                    if (ColorSwitch < 8)
                        ForegroundColor = DarkMagenta;
                    WriteLine(lines[i]);
                    ForegroundColor = White;
                    ColorSwitch--;
                    Thread.Sleep(20);
                }
            }
            else if (!Colorswitch)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    WriteLine(lines[i]);
                    Thread.Sleep(50);
                }
            }
        }
        public static void DisplayMainMenu() //Mit Cursor durch Main Menu, invertierte Farben, Enter bestätigen
        {
            ForegroundColor = White;
            WriteLine("Select what you want to do by pressing the corresponding key"); WriteLine(); ForegroundColor = Green; Thread.Sleep(50);
            WriteLine("(T) Tutorial"); WriteLine(); ForegroundColor = Red; Thread.Sleep(50);
            WriteLine("(E) Endless Mode"); WriteLine(); ForegroundColor = Yellow; Thread.Sleep(50);
            WriteLine("(L) Levels"); WriteLine(); ForegroundColor = Blue; Thread.Sleep(50);
            WriteLine("(W) Wiki"); WriteLine(); ForegroundColor = Gray; Thread.Sleep(50);
            WriteLine("(Esc) Quit");
        }
        public static void DisplayExitScreen(string[] lines)
        {
            int ColorSwitch = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                ForegroundColor = Yellow;
                if (ColorSwitch > 5)
                    ForegroundColor = DarkMagenta;
                WriteLine(lines[i]);
                ForegroundColor = White;
                ColorSwitch++;
                Thread.Sleep(20);
            }
        }

    }
}