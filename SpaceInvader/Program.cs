using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader //x und y sind vertauscht
{
    internal class Program
    {
        private static void Main()
        {
            CursorVisible = false;

            //Display.DisplayTitleScreen(); 
            //Thread.Sleep(3000); //3 Sekunden sleepen, damit nicht direkt nach dem TitleScreen etwas ausgewählt wird
            ClearKeyBuffer();
            
            Campaign Campaign = new(InitialiseCampaign());

            Menu MainMenu = new(InitialiseMainMenu(), 30, "Main Menu", White);
            
            MainMenu:            
            
            Clear();
            int SelectedOptionMainMenu = 0;
            MainMenu.PrintMenu(SelectedOptionMainMenu);
            while (true)
            {
                if (KeyAvailable)
                {                
                    ConsoleKey MainMenuKey = ReadKey(true).Key;                   
                    SelectedOptionMainMenu = MainMenu.GoThroughMenu(MainMenuKey, SelectedOptionMainMenu);
                    
                    MainMenu.PrintMenu(SelectedOptionMainMenu);
                    
                    if (MainMenuKey == ConsoleKey.Enter || MainMenuKey == ConsoleKey.Spacebar)
                    {
                        if (SelectedOptionMainMenu == 0) //Tutorial
                        {
                            BackgroundColor = Black;
                            bool RestartTutorial = Tutorial();
                            if (RestartTutorial)
                            {
                                Clear();
                                goto MainMenu;
                            }
                        }
                        else if (SelectedOptionMainMenu == 1) //Endless Mode
                        {
                        EndlessMode:
                            BackgroundColor = Black;
                            bool RestartEndlessmode = EndlessMode();
                            if (!RestartEndlessmode)
                            {
                                Clear();
                                goto MainMenu;
                            }
                            else if (RestartEndlessmode)
                            {
                                goto EndlessMode;
                            }
                        }
                        else if (SelectedOptionMainMenu == 2) //Levels
                        {
                            BackgroundColor = Black;
                            Clear();
                            int SelectedOptionCampaign = 0;
                            Campaign.PrintMenu(SelectedOptionCampaign);
                            while (true)
                            {
                                if (KeyAvailable)
                                {
                                    ConsoleKey CampaignKey = ReadKey(true).Key;
                                    SelectedOptionCampaign = Campaign.GoThroughMenu(CampaignKey, SelectedOptionCampaign);
                                    
                                    Campaign.PrintMenu(SelectedOptionCampaign);
                                    
                                    if (CampaignKey == ConsoleKey.Escape)
                                    {
                                        goto MainMenu;
                                    }
                                }
                            }
                        }
                        else if (SelectedOptionMainMenu == 3) //Wiki
                        {
                            BackgroundColor = Black;
                            Clear();

                            ForegroundColor = Blue;
                            WriteLine("Coming Soon!");
                            ForegroundColor = White;
                            WriteLine();
                            WriteLine("Press Escape to return to main menu");
                            int SelectedOptionWiki = 0;
                            //Wiki.PrintMenu(SelectedOptionWiki);
                            while (true)
                            {
                                if (KeyAvailable)
                                {
                                    ConsoleKey WikiKey = ReadKey(true).Key;
                                    //SelectedOptionWiki = Wiki.GoThroughMenu(WikiKey, SelectedOptionWiki);

                                    //Wiki.PrintMenu(SelectedOptionWiki);

                                    if (WikiKey == ConsoleKey.Escape)
                                    {
                                        goto MainMenu;
                                    }
                                }
                            }
                        }                       
                    }
                    else if (MainMenuKey == ConsoleKey.Escape)
                    {
                        BackgroundColor = Black;
                        ForegroundColor = White;
                        if (Quit())
                        {
                            Clear();
                            Display.DisplayExitScreen(Display.ScreenStringArray(@"..\..\..\ExitScreen.txt"));
                            break;
                        }
                        else
                        {
                            Clear();
                            goto MainMenu;
                        }
                    }
                }
            }                      
        }
        private static bool Quit()
        {
            Clear();

            WriteLine("Are you sure you want to quit?");

            SimpleMenuOption[] RestartMenu = new SimpleMenuOption[] { new SimpleMenuOption("Quit", Green),
                                                                      new SimpleMenuOption("Return", Red) };

            WriteLine();

            BackgroundColor = RestartMenu[0].Color; ForegroundColor = Black; WriteLine(RestartMenu[0].Name); //3
            WriteLine();
            ForegroundColor = RestartMenu[1].Color; BackgroundColor = Black; WriteLine(RestartMenu[1].Name); //5

            SetCursorPosition(0, 2);
            return !Display.DisplayTwoOptions(RestartMenu, 2, 4, true);
        }
        private static bool Tutorial()
        {
            TutorialLevel Level = new();
            CursorVisible = false;
            Clear();
            Level.InsertTextInLevel("Welcome to", 7, TutorialLevel.FirstLineOfText, 100, Yellow);
            Level.InsertTextInLevel("Space Fighters", 5, TutorialLevel.FirstLineOfText + 1, 100, Magenta);
            while (Level.TutorialNotCompleted)
            {
                if (Level.GameProgress > 325 && Level.SpawnEnemy)
                {
                    Level.Field[Level.EnemyLocationY, Level.PlayerLocationX] = new Enemy(Level.GameProgress);
                    Level.SpawnEnemy = false;
                }
                ConsoleKey key;
                if (KeyAvailable)
                {
                    key = ReadKey(true).Key;
                    ClearKeyBuffer();
                }
                else
                {
                    key = ConsoleKey.End;
                }
                if (Level.GameProgress > 100)
                    Level.TutorialMove();
                if (Level.GameProgress > 200)
                    Level.TutorialShoot();
                if (Level.GameProgress > 300 && Level.EnemyTutorialNotCompleted)
                    Level.TutorialStageEnemy();
                
                if (!Level.EnemyTutorialNotCompleted)
                        Level.TutorialNotCompleted = false;
                
                Level.LevelProgress(key);
                Level.PrintLevel();
                /*                
                ForegroundColor = White;
                Level.PrintGameProgress();             
                WriteLine($"Enemy Tutorial Not Completed: {Level.EnemyTutorialNotCompleted}");
                WriteLine($"WaitForEnemyToSpawn: {Level.SpawnEnemy}");
                */
                SetCursorPosition(0, 0);
            }
            for (int i = 0; i < 40; i++) //Auslauf
            {
                ConsoleKey key;
                if (KeyAvailable)
                {
                    key = ReadKey(true).Key;
                    ClearKeyBuffer();
                }
                else
                {
                    key = ConsoleKey.End;
                }
                Level.LevelProgress(key);
                Level.PrintLevel();
                SetCursorPosition(0, 0);
            }
            Clear();

            ForegroundColor = Yellow;
            WriteLine("Congratulations! You completed the Tutorial");
            ForegroundColor = Magenta;
            WriteLine("Welcome to Space Fighters");
            WriteLine();
            ForegroundColor = Gray;
            WriteLine("Press any key to return to Main Menu");
            BackgroundColor = Black;
            Thread.Sleep(2000);
            ClearKeyBuffer();
            while (!KeyAvailable) { }
            ClearKeyBuffer();
            return true;
        }
        private static bool EndlessMode()
        {
            Level Level = new(25, 25);
            CursorVisible = false;
            Clear();
            DateTime StartEndlessMode = DateTime.Now;
            while (Level.PlayerLifes > 0)
            {
                ConsoleKey key;
                if (KeyAvailable)
                {
                    key = ReadKey(true).Key;
                    ClearKeyBuffer();
                }
                else
                {
                    key = ConsoleKey.End;
                }
                Level.RandomEnemySpawn();
                Level.RandomMeteorSpawn(10);
                Level.RandomUpgradeSpawn(500);
                Level.LevelProgress(key);

                Level.PrintLevel();
                Level.PrintLifesAndScore();
                Level.PrintWeaponStatus();
                //Level.PrintEnemySpawnrate();
                //Level.PrintGameProgress();

                SetCursorPosition(0, 0);
            }
            TimeSpan Survived = DateTime.Now.Subtract(StartEndlessMode);
            Clear();
            ForegroundColor = DarkRed;
            Display.ScreenUpDown(Display.ScreenStringArray(@"..\..\..\YouLost.txt"), true, false);
            ForegroundColor = White;
            ForegroundColor = Blue; WriteLine($"Score: {Level.Score}");
            ForegroundColor = Yellow; WriteLine($"Time survived: {Survived}");
            
            SimpleMenuOption[] RestartMenu = new SimpleMenuOption[] { new SimpleMenuOption("Restart",   Green), new SimpleMenuOption("Main Menu", Red)  };
            SetCursorPosition(0, 8);
            BackgroundColor = RestartMenu[0].Color; ForegroundColor = Black; WriteLine(RestartMenu[0].Name); //10
            WriteLine();
            ForegroundColor = RestartMenu[1].Color; BackgroundColor = Black; WriteLine(RestartMenu[1].Name); //12
            return !Display.DisplayTwoOptions(RestartMenu, 8, 10, false);
        }
        private static void ClearKeyBuffer() //KEY Buffer Clear
        {
            while (KeyAvailable)
            {
                ReadKey(true);
            }
        }
        public static CampaignLevelMenuOption[] InitialiseCampaign()
        {
            CampaignLevelMenuOption[] Options = new CampaignLevelMenuOption[10];

            CampaignLevel Level1 = new();
            Options[0] = new CampaignLevelMenuOption("First Flight", "A criminal organisation called the \"Reapers\" has taken massive influence in the galaxy. Take flight for the first time and defeat the first wave of enemies. Can you take them all or will you succumb to their strength?", Yellow, Level1);

            CampaignLevel Level2 = new();
            Options[1] = new CampaignLevelMenuOption("Asteroids? No, meteors! Or?", "\"Sir, the possibility of successfully navigating an asteroid field is approximately 3720 to 1.\"                                                      \"Never tell me the odds\" ", Yellow, Level2);

            CampaignLevel Level3 = new();
            Options[2] = new CampaignLevelMenuOption("Upgrades people, Upgrades!", "Collect the purple Upgrade-Cores and improve your ship with powerful weapons to fight against the incoming asteroids and massive amount of enemies", Yellow, Level3);

            CampaignLevel Level4 = new();
            Options[3] = new CampaignLevelMenuOption("A new foe enters the battle", "What's that? It seems the Reapers have built spaceships with new weapons and abilities.     Can you figure out how to stop them?", Yellow, Level4);

            CampaignLevel Level5 = new();
            Options[4] = new CampaignLevelMenuOption("You're much more of a Reaper", "You have been taking out Reaper forces faster than they can recruit them. The Reaper Commanding Officer has seen to it personally to take you out ", Yellow, Level5); //First Boss

            CampaignLevel Level6 = new();
            Options[5] = new CampaignLevelMenuOption("Coming soon!", "", Yellow, Level6);

            CampaignLevel Level7 = new();
            Options[6] = new CampaignLevelMenuOption("Coming soon!", "", Yellow, Level7);

            CampaignLevel Level8 = new();
            Options[7] = new CampaignLevelMenuOption("Coming soon!", "", Yellow, Level8);

            CampaignLevel Level9 = new();
            Options[8] = new CampaignLevelMenuOption("Coming soon!", "", Yellow, Level9);

            CampaignLevel Level10 = new();
            Options[9] = new CampaignLevelMenuOption("Coming soon!", "", Yellow, Level10);

            return Options;
        }
        public static MenuOption[] InitialiseMainMenu()
        {
            MenuOption[] Options = new MenuOption[4] { new MenuOption("Tutorial", "Learn the game mechanics in this simple tutorial", Green),
                                                       new MenuOption("Endless Mode", "Try to hold your best at an ever increasing amount of enemys in the endless mode", Red),
                                                       new MenuOption("Campaign", "Battle your way through 10 unique levels in the campaign", Yellow),
                                                       new MenuOption("Wiki", "Read through the wiki and inform yourself about different enemies or upgrades to get better", Blue), };
            return Options;
        }
        public static WikiMenuOption[] InitialiseWiki()
        {
            WikiMenuOption[] Options = new WikiMenuOption[10];

            return Options;
        }
    }
}