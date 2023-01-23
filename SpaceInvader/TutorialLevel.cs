using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    internal class TutorialLevel : Level
    {
        public static int FirstLineOfText = 9;
        public bool TutorialNotCompleted { get; set; }
        
        #region MoveTutorial
        private bool MoveTutorialText;        
        public void TutorialMove()
        {
            if (MoveTutorialText)
            {
                InsertTextInLevel("Use W,A,S,D", 7, FirstLineOfText, 100, Yellow);    
                InsertTextInLevel("or the arrow keys", 4, FirstLineOfText + 1, 100, Yellow);
                InsertTextInLevel("to move", 8, Field.GetLength(0) / 2 - 1, 100, Yellow);
                MoveTutorialText = false;
            }
            /*
            MoveTutorialNotCompleted = false;
            if (key == ConsoleKey.UpArrow || key == ConsoleKey.W || key == ConsoleKey.DownArrow || key == ConsoleKey.S || key == ConsoleKey.LeftArrow || key == ConsoleKey.A || key == ConsoleKey.RightArrow || key == ConsoleKey.D)
                MoveTutorialNotCompleted = true;
            */
        }
        #endregion

        #region ShootTutorial
        private bool ShootTutorialText;
        public void TutorialShoot()
        {
            if (ShootTutorialText)
            {
                InsertTextInLevel("Press Spacebar", 5, FirstLineOfText, 100, Yellow);
                InsertTextInLevel("to shoot", 8, FirstLineOfText + 1, 100, Yellow);
                ShootTutorialText = false;
            }
            /*
            if (key == ConsoleKey.Spacebar)
                AmountOfShots++;
            if (AmountOfShots > 0)
                ShootTutorialNotCompleted = false;
            else
                ShootTutorialNotCompleted = true;
            */
        }
        #endregion

        #region EnemyTutorial
        public bool EnemyTutorialNotCompleted { get; set; }
        private bool EnemyTutorialText;
        public bool SpawnEnemy;
        public void TutorialStageEnemy()
        {
            if (EnemyTutorialText)
            {
                InsertTextInLevel("Destroy the enemy", 3, FirstLineOfText, 50, Yellow);
                EnemyTutorialText = false;
            }

            if (GameProgress > 325)
                EnemyTutorialNotCompleted = false;
            else if (GameProgress <= 325)
                EnemyTutorialNotCompleted = true;

            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    if (Field[i, j] is Enemy)
                        EnemyTutorialNotCompleted = true;                    
                }
            }           
        }         
        #endregion
        public TutorialLevel() : base()
        {
            TutorialNotCompleted = true;

            MoveTutorialText = true;
            
            ShootTutorialText = true;

            EnemyTutorialNotCompleted = true;
            EnemyTutorialText = true;
            SpawnEnemy = true;
        }        
    }
}