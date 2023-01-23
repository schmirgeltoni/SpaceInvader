using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{    
    internal class Level
    {
        private static Random Random = new();
        public Entity[,] Field { get; set; }       
        public int PlayerLocationY { get; set; }
        public int PlayerLocationX { get; set; }
        public int GameProgress { get; set; }
        public int PlayerLifes { get; set; }
        public int Score { get; set; }
        public int MeteorSpawnCounter { get; set; }
        public int EnemySpawnCounter { get; set; }
        public int EnemyLocationY { get; set; }
        public int EnemySpawnrate { get; set; }
        public bool EnemyExists { get; set; }
        public Weapon PlayerWeapon { get; set; }
        public Level()
        {
            Field = new Entity[24, 24];
            for (int i = 0; i < Field.GetLength(0); i++)    //Feld befüllen
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    if (i == 0 || i == Field.GetLength(0) - 1 || j == 0 || j == Field.GetLength(1) - 1)
                        Field[i, j] = new Barrier(GameProgress);
                    else
                        Field[i, j] = new Space();
                }
            }
            GameProgress = 0;
            EnemyLocationY = 1;
            PlayerLocationY = Field.GetLength(0) - 2;
            PlayerLocationX = Field.GetLength(1) / 2;

            Field[PlayerLocationY, PlayerLocationX] = new Player(GameProgress);
            PlayerWeapon = new Weapon(WeaponType.Standard);
            EnemyExists = false;
        }
        public Level(int Height, int Width)
        {
            GameProgress = 1;
            Score = 0;
            EnemySpawnCounter = 0;
            MeteorSpawnCounter = 0;
            PlayerLifes = 3;
            PlayerLocationY = Height - 2;
            PlayerLocationX = Width/2;
            EnemyLocationY = 1;
            EnemySpawnrate = 100;
            EnemyExists = false;
            PlayerWeapon = new Weapon(WeaponType.Standard);

            Field = new Entity[Height, Width];
            for (int i = 0; i < Field.GetLength(0); i++)    //Feld befüllen
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                        Field[i, j] = new Barrier(GameProgress);                   
                    else 
                        Field[i, j] = new Space();
                }
            }           
            Field[PlayerLocationY, PlayerLocationX] = new Player(GameProgress);
        }
        public void LevelProgress(ConsoleKey consoleKey)
        {
            GameProgress++;
            EnemyExists = false;
                      
            if (PlayerWeapon.Cooldown < PlayerWeapon.MaxCooldown)
                PlayerWeapon.Cooldown++;

            if (PlayerWeapon.Type != WeaponType.Standard && PlayerWeapon.Uses <= 0)
                PlayerWeapon = new Weapon(WeaponType.Standard);
           
            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    #region Player  TODO: Upgrades durch reinfahren collecten
                    if (Field[i, j] is Player && Field[i, j].GameProgress != GameProgress)
                    {
                        Field[0, j] = new AimBarrier(GameProgress); //AimBarrier an (0, PlayerPositon)

                        if (PlayerWeapon.Type == WeaponType.Minigun && consoleKey == ConsoleKey.Spacebar)   //Minigun toggle
                        {
                            PlayerWeapon.Toggle = !PlayerWeapon.Toggle;
                        }

                        if (PlayerWeapon.Type == WeaponType.Minigun && PlayerWeapon.Toggle && Field[i - 1, j] is not Barrier) //MinigunSchuss (automatisch)
                        {
                            Field[i - 1, j] = new FriendlyProjectile(GameProgress);
                            PlayerWeapon.Uses--;
                        }

                        if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is DestructableMeteor TemporaryDestructableMeteor1)
                        {
                            Field[i - 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor1.Health - 1);
                        }
                        else if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is DestructableBarrier TemporaryDestructableBarrier)
                        {
                            Field[i - 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                        else if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is not Barrier)
                        {
                            if (PlayerWeapon.Type == WeaponType.Standard && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                Field[i - 1, j] = new FriendlyProjectile(GameProgress);
                            }
                            else if (PlayerWeapon.Type == WeaponType.Rocket && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                PlayerWeapon.Uses--;
                                Field[i - 1, j] = new Rocket(GameProgress);
                            }
                            else if (PlayerWeapon.Type == WeaponType.Laser && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                PlayerWeapon.Uses--;
                                for (int k = i - 1; Field[k, j] is not Barrier || Field[k, j] is Meteor; k--)
                                {
                                    if (Field[k, j] is Enemy)
                                        Kill();
                                    if (Field[k, j] is Upgrade TemporaryUpgrade)
                                        PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                                    Field[k, j] = new Laser(0);
                                }
                            }
                        }
                        else if ((consoleKey == ConsoleKey.LeftArrow || consoleKey == ConsoleKey.A) && (Field[i, j - 1] is not Barrier && Field[i, j - 1] is not DestructableBarrier))
                        {
                            if (Field[i, j - 1] is Enemy || Field[i, j - 1] is EnemyProjectile)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i, j - 1] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i, j - 1] = new Player(GameProgress);
                            Field[0, j] = new Barrier(GameProgress); //alte AimBarrier wird gelöscht
                            Field[i, j] = new Space();
                        }
                        else if ((consoleKey == ConsoleKey.RightArrow || consoleKey == ConsoleKey.D) && (Field[i, j + 1] is not Barrier && Field[i, j + 1] is not DestructableBarrier))
                        {
                            if (Field[i, j + 1] is Enemy || Field[i, j + 1] is EnemyProjectile)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i, j + 1] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i, j + 1] = new Player(GameProgress);
                            Field[0, j] = new Barrier(GameProgress); //alte AimBarrier wird gelöscht
                            Field[i, j] = new Space();
                        }
                        else if ((consoleKey == ConsoleKey.UpArrow || consoleKey == ConsoleKey.W) && (Field[i - 1, j] is not Barrier && Field[i - 1, j] is not DestructableBarrier))
                        {
                            if (Field[i - 1, j] is Enemy || Field[i - 1, j] is EnemyProjectile)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i - 1, j] = new Player(GameProgress);
                            Field[i, j] = new Space();
                        }
                        else if ((consoleKey == ConsoleKey.DownArrow || consoleKey == ConsoleKey.S) && (Field[i + 1, j] is not Barrier && Field[i + 1, j] is not DestructableBarrier))
                        {
                            if (Field[i + 1, j] is Enemy || Field[i + 1, j] is EnemyProjectile)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i + 1, j] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i + 1, j] = new Player(GameProgress);
                            Field[i, j] = new Space();
                        }
                    }
                    #endregion

                    #region Explosion
                    else if (Field[i, j] is Explosion TemporaryExplosion)
                    {
                        if (TemporaryExplosion.GameProgress >= 3)
                        {
                            Field[i, j] = new Space();
                        }
                        else
                        {
                            TemporaryExplosion.GameProgress++;
                        }
                    }
                    #endregion

                    #region Laser
                    else if (Field[i, j] is Laser TemporaryLaser)
                    {
                        if (TemporaryLaser.GameProgress >= 3)
                            Field[i, j] = new Space();
                        else
                            TemporaryLaser.GameProgress++;
                    }
                    #endregion

                    #region Rocket
                    else if (Field[i, j] is Rocket && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is not Barrier)
                    {

                        if (Field[i - 1, j] is Space)
                        {
                            Field[i, j] = new Space();      //Altes Projectile wird geslöscht (Space)
                            Field[i - 1, j] = new Rocket(GameProgress); // dann wird darüber ein neues Projectile gesetzt (es fliegt weiter)
                        }
                        else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                        {
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            RocketExplosion(i, j);
                        }
                        else
                        {
                            RocketExplosion(i, j);
                        }
                    }
                    else if (Field[i, j] is Rocket && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is Barrier)  //Collision mit Barrier
                    {
                        RocketExplosion(i, j);
                    }
                    #endregion

                    #region FriendlyProjectile
                    else if (Field[i, j] is FriendlyProjectile && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is not Barrier) //Wenn über dem Projektil keine Barriere ist
                    {
                        if (Field[i - 1, j] is Space)
                        {
                            Field[i, j] = new Space();    //Altes Projectile wird geslöscht (Space)
                            Field[i - 1, j] = new FriendlyProjectile(GameProgress); // dann wird darüber ein neues Projectile gesetzt (es fliegt weiter)
                        }
                        else if (Field[i - 1, j] is EnemyProjectile)  //Collision mit GegnerProjektil
                        {
                            Field[i, j] = new Space();  //Projektil wird zu Space
                            Field[i - 1, j] = new Explosion(); //EnemyProjectile wird zu Explosion
                        }
                        else if (Field[i - 1, j] is Enemy)    //Collision mit Gegner
                        {
                            Field[i, j] = new Space();
                            Field[i - 1, j] = new Explosion();
                            Kill();
                        }
                        else if (Field[i - 1, j] is DestructableMeteor TemporaryDestructableMeteor2)
                        {
                            if (TemporaryDestructableMeteor2.Health <= 0)
                            {
                                Field[i, j] = new Explosion();
                            }
                            else
                            {
                                Field[i - 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor2.Health - 1);
                            }
                        }
                        else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                        {
                            Field[i, j] = new Space();
                            Field[i - 1, j] = new Space();
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                        }
                        else if (Field[i - 1, j] is DestructableBarrier TemporaryDestructableBarrier)
                        {
                            Field[i, j] = new Space();
                            Field[i - 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                            if (TemporaryDestructableBarrier.Health <= 0)
                            {
                                Field[i - 1, j] = new Explosion();
                            }
                        }
                    }
                    else if (Field[i, j] is FriendlyProjectile && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is Barrier)  //Collision mit Barrier
                    {
                        Field[i, j] = new Space();
                    }
                    #endregion

                    #region Meteor
                    else if (Field[i, j] is Meteor TemporaryMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        if (Field[i + 1, j] is Space && TemporaryMeteor.Move) //Wenn unter dem Meteor nichts ist
                        {
                            Field[i, j] = new Space();    //Alter Meteor wird gelöscht (Space)
                            Field[i + 1, j] = new Meteor(GameProgress); //dann wird darunter ein neues Projectile hesetzt (es fliegt weiter)
                        }
                        else if (Field[i + 1, j] is Rocket && TemporaryMeteor.Move)
                        {
                            RocketExplosion(i + 1, j);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile && TemporaryMeteor.Move)
                        {
                            Field[i, j] = new Space();  //Alter Meteor wird zu Space
                            Field[i + 1, j] = new Meteor(GameProgress); //FriendlyProjectile wird zu überschrieben (vernichtet)
                        }
                        else if (Field[i + 1, j] is Player && TemporaryMeteor.Move)
                        {
                            Field[i, j] = new Space();
                            PlayerLifes--;
                        }
                        else if (Field[i + 1, j] is DestructableBarrier TemporaryDestructableBarrier && TemporaryMeteor.Move)
                        {
                            Field[i, j] = new Space();
                            Field[i + 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                        else if (!TemporaryMeteor.Move)
                        {
                            TemporaryMeteor.Move = true;
                        }
                    }
                    else if (Field[i, j] is Meteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = new Space();
                    }
                    #endregion

                    #region DestructableMeteor
                    else if (Field[i, j] is DestructableMeteor TemporaryDestructableMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        if (TemporaryDestructableMeteor.Health <= 0)
                        {
                            Field[i, j] = new Explosion();
                        }
                        else if (Field[i + 1, j] is Space && TemporaryDestructableMeteor.Move) //Wenn unter dem Meteor nichts ist
                        {
                            Field[i, j] = new Space();    //Alter Meteor wird gelöscht (Space)
                            Field[i + 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor.Health); //dann wird darunter ein neues Projectile hesetzt (es fliegt weiter)                            
                        }
                        else if (Field[i + 1, j] is Rocket && TemporaryDestructableMeteor.Move)
                        {
                            RocketExplosion(i + 1, j);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = new Space();  //Alter Meteor wird zu Space
                            Field[i + 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor.Health - 1); //FriendlyProjectile wird zu überschrieben (vernichtet)
                        }
                        else if (Field[i + 1, j] is Player && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = new Space();
                            PlayerLifes--;
                        }
                        else if (Field[i + 1, j] is DestructableBarrier TemporaryDestructableBarrier && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = new Space();
                            Field[i + 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                        else if (!TemporaryDestructableMeteor.Move)
                        {
                            TemporaryDestructableMeteor.Move = true;
                        }
                    }
                    else if (Field[i, j] is DestructableMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = new Space();
                    }
                    #endregion

                    #region EnemyProjectile
                    else if (Field[i, j] is EnemyProjectile && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        if (Field[i + 1, j] is Space || Field[i + 1, j] is Upgrade) //Wenn unter dem Projektil nichts/Upgrade ist
                        {
                            Field[i, j] = new Space();    //Altes Projectile wird gelöscht (Space)
                            Field[i + 1, j] = new EnemyProjectile(GameProgress); //dann wird darunter ein neues Projectile hesetzt (es fliegt weiter)
                        }
                        else if (Field[i + 1, j] is Rocket)
                        {
                            RocketExplosion(i + 1, j);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile || Field[i + 1, j] is Enemy)
                        {
                            Field[i, j] = new Space();  //Projektil wird zu Space
                            Field[i + 1, j] = new Explosion(); //FriendlyProjectile wird zu Space
                        }
                        else if (Field[i + 1, j] is Player)
                        {
                            Field[i, j] = new Explosion();
                            PlayerLifes--;
                        }
                        else if (Field[i + 1, j] is DestructableMeteor TemporaryDestructableMeteor2)
                        {
                            Field[i, j] = new Explosion();
                            Field[i + 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor2.Health - 1);
                        }
                        else if (Field[i + 1, j] is DestructableBarrier TemporaryDestructableBarrier)
                        {
                            Field[i, j] = new Explosion();
                            Field[i + 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                    }
                    else if (Field[i, j] is EnemyProjectile && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = new Space();
                    }
                    #endregion

                    #region DestructableBarrier
                    else if (Field[i, j] is DestructableBarrier TemoraryDestructableBarrier)
                    {
                        if (TemoraryDestructableBarrier.Health <= 0)
                        {
                            Field[i, j] = new Space();
                        }
                    }
                    #endregion

                    #region Upgrade
                    else if (Field[i, j] is Upgrade TemporaryUpgrade && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {                      
                        if ((Field[i + 1, j] is Space || Field[i + 1, j] is Enemy) && TemporaryUpgrade.Move >= 4) //Wenn unter dem Upgrade nichts/Gegner ist
                        {
                            Field[i + 1, j] = new Upgrade(GameProgress, TemporaryUpgrade.UpgradeType); //dann wird darunter ein neues Upgrade gesetzt (es fliegt weiter)
                            Field[i, j] = new Space();    //Altes Projectile wird gelöscht (Space)
                        }
                        else if (Field[i + 1, j] is Rocket && TemporaryUpgrade.Move >= 4)
                        {
                            RocketExplosion(i + 1, j);
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile && TemporaryUpgrade.Move >= 4)
                        {
                            Field[i, j] = new Space();  
                            Field[i + 1, j] = new Space(); 
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                        }
                        else if (Field[i + 1, j] is Player && TemporaryUpgrade.Move >= 4)
                        {
                            Field[i, j] = new Space();
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                        }
                        else if (Field[i + 1, j] is DestructableMeteor && TemporaryUpgrade.Move >= 4)
                        {
                            Field[i, j] = new Space();
                        }
                        else if (Field[i + 1, j] is DestructableBarrier && TemporaryUpgrade.Move >= 4)
                        {
                            Field[i, j] = new Space();                         
                        }
                        else if (TemporaryUpgrade.Move < 4)
                        {
                            TemporaryUpgrade.Move++;
                        }
                    }
                    else if (Field[i, j] is Upgrade && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = new Space();                   
                    }
                    #endregion

                    #region Enemy
                    else if (Field[i, j] is Enemy TemporaryEnemy)
                    {
                        EnemyExists = true;
                        TemporaryEnemy.LifeTime++;
                        TemporaryEnemy.LifeTimeForVerticalMovement++;

                        if (TemporaryEnemy.LifeTimeForVerticalMovement > 67)
                        {
                            if (Field[i + 1, j] is Space || Field[i + 1, j] is Upgrade)
                            {
                                Field[i, j] = new Space();
                                Field[i + 1, j] = new Enemy(GameProgress);
                            }
                            else if (Field[i + 1, j] is Player || Field[i + 1, j] is Barrier)
                            {
                                PlayerLifes--;
                                Kill();
                                Field[i, j] = new Space();
                            }
                            else if (Field[i + 1, j] is Rocket)
                            {
                                RocketExplosion(i + 1, j);
                            }
                            else if (Field[i + 1, j] is FriendlyProjectile)
                            {
                                Field[i, j] = new Explosion();
                                Field[i + 1, j] = new Space();
                            }
                        }
                        else if (TemporaryEnemy.LifeTime > 20)
                        {
                            int move = Random.Next(5);
                            if (move == 0 && Field[i, j - 1] is not Barrier && Field[i, j - 1] is not Enemy) //nach Links
                            {
                                Field[i, j - 1] = new Enemy(GameProgress, TemporaryEnemy.LifeTimeForVerticalMovement);
                                Field[i, j] = new Space();
                            }
                            else if (move == 1 && Field[i, j + 1] is not Barrier && Field[i, j + 1] is not Enemy) //nach Rechts
                            {
                                Field[i, j + 1] = new Enemy(GameProgress, TemporaryEnemy.LifeTimeForVerticalMovement);
                                Field[i, j] = new Space();
                            }
                            else if (Field[i + 1, j] is not Enemy && Field[i + 1, j] is not Barrier)    //Shoot
                            {
                                if (Field[i + 1, j] is Player)
                                {
                                    PlayerLifes--;
                                }
                                else
                                {
                                    Field[i + 1, j] = new EnemyProjectile(GameProgress);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Text
                    else if(Field[i, j] is Text TemporaryText)
                    {
                        if (TemporaryText.GameProgress >= TemporaryText.Lifetime)
                            Field[i, j] = new Space();
                        else
                            TemporaryText.GameProgress++;
                    }
                    #endregion
                }
            }
        }
        public void RandomEnemySpawn()
        {
            EnemySpawnCounter++;
            if (EnemySpawnCounter > EnemySpawnrate || EnemyExists == false)
            {
            RollNewEnemySpawn:
                EnemyExists = true;
                int RandomSpawn = Random.Next(1, Field.GetLength(1) - 2);
                if (Field[EnemyLocationY, RandomSpawn] is Space)
                {
                    Field[EnemyLocationY, RandomSpawn] = new Enemy(GameProgress);
                }
                else
                {
                    goto RollNewEnemySpawn;
                }
            EnemySpawnCounter = 0;
            }
            EnemyExists = false;
        }
        public void RandomMeteorSpawn(int Spawnrate)
        {
            MeteorSpawnCounter++;
            if (MeteorSpawnCounter > Spawnrate)
            {
            RollNewMeteorSpawn:
                int RandomSpawn = Random.Next(1, Field.GetLength(1) - 2);
                int WhichMeteor = Random.Next(2);
                if ((Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile) && WhichMeteor == 0)
                {
                    Field[EnemyLocationY, RandomSpawn] = new DestructableMeteor(GameProgress, 3);
                }
                else if ((Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile) && WhichMeteor == 1)
                {
                    Field[EnemyLocationY, RandomSpawn] = new Meteor(GameProgress);
                }
                else
                {
                    goto RollNewMeteorSpawn;
                }
                MeteorSpawnCounter = 0;
            }
        }
        public void RandomUpgradeSpawn(int Spawnrate)
        {
            if (GameProgress % Spawnrate == 0)
            {
                int WhichUpgrade = Random.Next(3);
                RollNewUpgradeSpawn:
                int RandomSpawn = Random.Next(1, Field.GetLength(1) - 2);
                if ((Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile || Field[EnemyLocationY, RandomSpawn] is Enemy) && WhichUpgrade == 0)
                {
                    Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Laser);
                }
                else if ((Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile || Field[EnemyLocationY, RandomSpawn] is Enemy) && WhichUpgrade == 1)
                {
                    Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Minigun);
                }
                else if ((Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile || Field[EnemyLocationY, RandomSpawn] is Enemy) && WhichUpgrade == 2)
                {
                    Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Rocket);
                }
                else
                {
                    goto RollNewUpgradeSpawn;
                }
            }
        }         
        public void PrintLevel()
        {
            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    ForegroundColor = Field[i, j].Color;
                    Write(Field[i, j].Icon);
                }
                WriteLine();
            }            
            
        }
        public void PrintLifesAndScore()
        {
            ForegroundColor = Green;
            Write($"Lifes: {PlayerLifes}");
            ForegroundColor = Yellow;
            for (int i = 0; i < Field.GetLength(1) - 14 /*7 Zeichen von "Lifes: ", 7 von "Score: "*/ - Convert.ToString(PlayerLifes).Length - Convert.ToString(Score).Length; i++)
            {
                Write($" ");
            }
            WriteLine($"Score: {Score}");           
        }
        public void PrintWeaponStatus()
        {
            if (PlayerWeapon.Cooldown == 0)
            {
                ClearLine();
            }

            if (PlayerWeapon.Cooldown < PlayerWeapon.MaxCooldown) //Rot wenn nicht geladen
                ForegroundColor = Red;
            else if (PlayerWeapon.Type == WeaponType.Minigun)
                ForegroundColor = Green;
            else
                ForegroundColor = Green; //sonst Grün

            if (PlayerWeapon.Type == WeaponType.Minigun)
            {
                for (int i = PlayerWeapon.Uses / 4; i >= 0; i--)
                    Write("█");
            }
            else if (!(PlayerWeapon.Type == WeaponType.Minigun))

                if (PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                {
                    for (int i = 0; i < Field.GetLength(1); i++) //Reload Display for a loaded gun
                    {
                        Write("█");
                    }
                }
                else
                {
                    for (int i = 0; i < Field.GetLength(1) / PlayerWeapon.MaxCooldown * PlayerWeapon.Cooldown; i++) //Reload Display for all Weapons except Minigun
                    {
                        Write("█");
                    }
                }

            ForegroundColor = DarkYellow;
            WriteLine();
            ClearLine();
            Write($"Weapon: {PlayerWeapon.Type}");  //12 "Uses: " = 6 => 6 - Length
            if (!(PlayerWeapon.Type == WeaponType.Minigun || PlayerWeapon.Type == WeaponType.Standard)) //Für Waffen mit Uses wird es angezeigt
            {
                {
                    for (int i = 0; i < 6 - Convert.ToString(PlayerWeapon.Uses).Length; i++)
                        Write(" ");
                }
                WriteLine($"Ammo: {PlayerWeapon.Uses}");
            }
            else
                WriteLine();
        }
        public void PrintEnemySpawnrate()
        {
            ClearLine();
            WriteLine($"Enemy Spawnrate: {EnemySpawnrate}");
        } //Debugtool
        public void PrintGameProgress()
        {
            WriteLine($"Gameprogress: {GameProgress}"); //hier kein ClearLine, da der Progress nie kleiner wird
        } //Debugtool
        public void InsertTextInLevel(string Text, int LineX, int LineY, int Lifetime, ConsoleColor Color)
        {
            char[] Array = Text.ToCharArray();
            for (int i = 0; i < Array.Length; i++)
            {
                if (Field[LineY, i + LineX] is Player)
                    Field[LineY + 1, i + LineX] = new Player(GameProgress);
                if (Array[i] == ' ')
                    Field[LineY, i + LineX] = new Space();
                else
                    Field[LineY, i + LineX] = new Text(Array[i], Color, GameProgress, Lifetime + GameProgress);
            }
        }
        public void RocketExplosion(int i, int j)
        {
            for (int j2 = j - 1; j2 < j + 2; j2++)
            {
                for (int i2 = i - 1; i2 < i + 2; i2++)
                {
                    if (Field[i2, j2] is not Barrier)
                    {
                        if (Field[i2, j2] is Enemy)
                        {
                            Score += 100;
                            Field[i2, j2] = new Explosion();
                        }
                        else if (Field[i2, j2] is Player)
                        {
                            PlayerLifes--;
                        }
                        else
                        {
                            Field[i2, j2] = new Explosion();
                        }
                    }
                    else if (Field[i2, j2] is Meteor)
                    {
                        Field[i2, j2] = new Explosion();
                    }
                }
            }
        }
        static public void ClearLine()
        {
            Write("                                               ");
            CursorLeft = 0;
        }
        public void Kill() 
        {
            Score += 100;
            if (Score % 1000 == 0)
                EnemySpawnrate -= 10;
            /* //Indischer Code goes hard
            if (Score < 1000)
                EnemySpawnrate = 100;
            else if (Score >= 1000 && Score < 2000)
                EnemySpawnrate = 90;
            else if (Score >= 2000 && Score < 3000)
                EnemySpawnrate = 80;
            else if (Score >= 3000 && Score < 4000)
                EnemySpawnrate = 70;
            else if (Score >= 4000 && Score < 5000)
                EnemySpawnrate = 60;
            else if (Score >= 5000 && Score < 6000)
                EnemySpawnrate = 50;
            else if (Score >= 6000 && Score < 7000)
                EnemySpawnrate = 40;
            else if (Score >= 7000 && Score < 8000)
                EnemySpawnrate = 30;
            else if (Score >= 8000 && Score < 9000)
                EnemySpawnrate = 20;
            else if (Score >= 9000)
                EnemySpawnrate = 10;
            */
        }
    }
}