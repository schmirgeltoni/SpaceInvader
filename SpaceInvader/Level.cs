using System.Xml;
using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{    

    internal class Level
    {
        public Space _Space = new Space();
        public Barrier _VerticalBarrier = new Barrier('║');
        public Barrier _HorizontalBarrier = new Barrier('═');

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
        public int FieldLengthX { get; set; }
        public int FieldLengthY { get; set; }
        public Level()
        {
            Field = new Entity[24, 24];
            FieldLengthY = Field.GetLength(0);
            FieldLengthX = Field.GetLength(1);
            //fill playing field
            for (int i = 0; i < FieldLengthY; i++)
            {
                for (int j = 0; j < FieldLengthX; j++)
                {
                    if (i == 0 || i == FieldLengthY - 1)
                        Field[i, j] = _HorizontalBarrier;
                    else if (j == 0 || j == FieldLengthX - 1)
                        Field[i, j] = _VerticalBarrier;
                    else
                        Field[i, j] = _Space;
                }
            }
            //4 corners
            Field[0, 0] = new Barrier('╔');
            Field[FieldLengthY - 1, 0] = new Barrier('╚');
            Field[0, FieldLengthX - 1] = new Barrier('╗');
            Field[FieldLengthX - 1, FieldLengthY - 1] = new Barrier('╝');

            GameProgress = 0;
            EnemyLocationY = 1;
            PlayerLocationY = FieldLengthY - 2;
            PlayerLocationX = FieldLengthX / 2;

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
            FieldLengthY = Field.GetLength(0);
            FieldLengthX = Field.GetLength(1);
            //fill playing field
            for (int i = 0; i < FieldLengthY; i++)
            {
                for (int j = 0; j < FieldLengthX; j++)
                {
                    if (i == 0 || i == FieldLengthY - 1)
                        Field[i, j] = _HorizontalBarrier;
                    else if (j == 0 || j == FieldLengthX - 1)
                        Field[i, j] = _VerticalBarrier;
                    else
                        Field[i, j] = _Space;
                }
            }
            //4 corners
            Field[0, 0] = new Barrier('╔');
            Field[FieldLengthY - 1, 0] = new Barrier('╚');
            Field[0, FieldLengthX - 1] = new Barrier('╗');
            Field[FieldLengthX - 1, FieldLengthY - 1] = new Barrier('╝');

            Field[PlayerLocationY, PlayerLocationX] = new Player(GameProgress);
        }
        public void LevelProgress(ConsoleKey consoleKey) //i = y (vertical), j = x (horizontal)
        {
            GameProgress++;
            EnemyExists = false;
            //prevents player from spamming
            if (PlayerWeapon.Cooldown < PlayerWeapon.MaxCooldown)
                PlayerWeapon.Cooldown++;
            //if special weapon is out of ammo, reset to standard weapon
            if (PlayerWeapon.Type != WeaponType.Standard && PlayerWeapon.Ammo <= 0)
                PlayerWeapon = new Weapon(WeaponType.Standard);
            //main loop
            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    #region Player  TODO: Upgrades durch reinfahren collecten
                    if (Field[i, j] is Player && Field[i, j].GameProgress != GameProgress)
                    {
                        //AimBarrier at (0, PlayerPositon)
                        Field[0, j] = new AimBarrier(GameProgress);
                        //Minigun toggle
                        if (PlayerWeapon.Type == WeaponType.Minigun && consoleKey == ConsoleKey.Spacebar)   
                        {
                            PlayerWeapon.Toggle = !PlayerWeapon.Toggle;
                        }
                        //Minigun shot (automatic)
                        if (PlayerWeapon.Type == WeaponType.Minigun && PlayerWeapon.Toggle && Field[i - 1, j] is not Barrier) 
                        {
                            Field[i - 1, j] = new FriendlyProjectile(GameProgress);
                            PlayerWeapon.Ammo--;
                        }
                        //remove health of destructable barriers/meteors if they're in front of the player and they shoot
                        if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is DestructableMeteor TemporaryDestructableMeteor1)
                        {
                            TemporaryDestructableMeteor1.Health--;
                        }
                        else if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is DestructableBarrier TemporaryDestructableBarrier)
                        {
                            TemporaryDestructableBarrier.Health--;
                        }
                        else if (consoleKey == ConsoleKey.Spacebar && Field[i - 1, j] is not Barrier)
                        {
                            //spawn projectile
                            if (PlayerWeapon.Type == WeaponType.Standard && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                Field[i - 1, j] = new FriendlyProjectile(GameProgress);
                            }
                            else if (PlayerWeapon.Type == WeaponType.Rocket && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                PlayerWeapon.Ammo--;
                                Field[i - 1, j] = new Rocket(GameProgress);
                            }
                            else if (PlayerWeapon.Type == WeaponType.Laser && PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                            {
                                PlayerWeapon.Cooldown = 0;
                                PlayerWeapon.Ammo--;
                                for (int k = i - 1; Field[k, j] is not Barrier; k--)
                                {
                                    if (Field[k, j] is Enemy)
                                        Kill();
                                    if (Field[k, j] is Upgrade TemporaryUpgrade)
                                        PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                                    Field[k, j] = new Laser(0);
                                }
                            }
                        }
                        //player goes left
                        else if ((consoleKey == ConsoleKey.LeftArrow || consoleKey == ConsoleKey.A) && (Field[i, j - 1] is not Barrier && Field[i, j - 1] is not DestructableBarrier))
                        {
                            if (Field[i, j - 1] is Enemy)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i, j - 1] is EnemyProjectile)
                            {
                                PlayerLifes--;
                            }
                            //recieve upgrade
                            else if (Field[i, j - 1] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            //make new player
                            Field[i, j - 1] = new Player(GameProgress);
                            //delete old aimbarrier
                            Field[0, j] = _HorizontalBarrier;
                            Field[i, j] = _Space;
                        }
                        //player goes right, stuff will be a bit repeatitive :P
                        else if ((consoleKey == ConsoleKey.RightArrow || consoleKey == ConsoleKey.D) && (Field[i, j + 1] is not Barrier && Field[i, j + 1] is not DestructableBarrier))
                        {
                            if (Field[i, j + 1] is Enemy)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i, j + 1] is EnemyProjectile)
                            {
                                PlayerLifes--;
                            }
                            else if (Field[i, j + 1] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i, j + 1] = new Player(GameProgress);
                            Field[0, j] = _HorizontalBarrier;
                            Field[i, j] = _Space;
                        }
                        //player goes up
                        else if ((consoleKey == ConsoleKey.UpArrow || consoleKey == ConsoleKey.W) && (Field[i - 1, j] is not Barrier && Field[i - 1, j] is not DestructableBarrier))
                        {
                            if (Field[i - 1, j] is Enemy)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if (Field[i - 1, j] is EnemyProjectile)
                            {
                                PlayerLifes--;
                            }
                            else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i - 1, j] = new Player(GameProgress);
                            Field[i, j] = _Space;
                            //no aimbarrier change here since movement is vertical
                        }
                        //player goes down
                        else if ((consoleKey == ConsoleKey.DownArrow || consoleKey == ConsoleKey.S) && (Field[i + 1, j] is not Barrier && Field[i + 1, j] is not DestructableBarrier))
                        {
                            if (Field[i + 1, j] is Enemy)
                            {
                                PlayerLifes--;
                                Kill();
                            }
                            else if(Field[i + 1, j] is EnemyProjectile)
                            {
                                PlayerLifes--;
                            }
                            else if (Field[i + 1, j] is Upgrade TemporaryUpgrade)
                            {
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            Field[i + 1, j] = new Player(GameProgress);
                            Field[i, j] = _Space;
                        }
                    }
                    #endregion

                    #region Explosion
                    else if (Field[i, j] is Explosion TemporaryExplosion)
                    {   
                        //explosions are on the field for 3 game ticks
                        if (TemporaryExplosion.GameProgress >= 3)
                        {
                            Field[i, j] = _Space;
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
                        //lasers are on the field for 3 game ticks too
                        if (TemporaryLaser.GameProgress >= 3)
                            Field[i, j] = _Space;
                        else
                            TemporaryLaser.GameProgress++;
                    }
                    #endregion

                    #region Rocket
                    else if (Field[i, j] is Rocket && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is not Barrier)
                    {

                        if (Field[i - 1, j] is Space)
                        {   
                            //delete old rocket
                            Field[i, j] = _Space; 
                            //new projectile is created one space above (it flies)
                            Field[i - 1, j] = new Rocket(GameProgress);
                        }
                        else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                        {
                            //collect new weapon
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            MakeExplosion(i, j, 1);
                        }
                        else
                        {
                            MakeExplosion(i, j, 1);
                        }
                    }
                    //collision with a barrier
                    else if (Field[i, j] is Rocket && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is Barrier) 
                    {
                        MakeExplosion(i, j, 1);
                    }
                    #endregion

                    #region FriendlyProjectile
                    //no barrier above projectile
                    else if (Field[i, j] is FriendlyProjectile && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is not Barrier)
                    {
                        if (Field[i - 1, j] is Space)
                        {
                            //projectile keeps going
                            Field[i, j] = _Space;    
                            Field[i - 1, j] = new FriendlyProjectile(GameProgress);
                        }
                        else if (Field[i - 1, j] is EnemyProjectile) 
                        {
                            //delete both projectiles and create an explosion
                            Field[i, j] = _Space;  
                            Field[i - 1, j] = new Explosion(); 
                        }
                        else if (Field[i - 1, j] is Enemy)
                        {
                            //delete both projectiles, create an explosion and update score
                            Field[i, j] = _Space;
                            Field[i - 1, j] = new Explosion();
                            Kill();
                        }
                        else if (Field[i - 1, j] is DestructableMeteor TemporaryDestructableMeteor2)
                        {   
                            //reduce hp and check for destruction
                            TemporaryDestructableMeteor2.Health--;
                            if (TemporaryDestructableMeteor2.Health <= 0)
                            {
                                Field[i, j] = new Explosion();
                            }
                        }
                        else if (Field[i - 1, j] is Upgrade TemporaryUpgrade)
                        {
                            //delete both entities and collect upgrade
                            Field[i, j] = _Space;
                            Field[i - 1, j] = _Space;
                            PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                        }
                        else if (Field[i - 1, j] is DestructableBarrier TemporaryDestructableBarrier)
                        {   
                            //delete the projectile
                            Field[i, j] = _Space;
                            TemporaryDestructableBarrier.Health--;
                            if (TemporaryDestructableBarrier.Health <= 0)
                            {
                                Field[i - 1, j] = new Explosion();
                            }
                        }
                    }
                    //collision with a barrier
                    else if (Field[i, j] is FriendlyProjectile && Field[i, j].GameProgress != GameProgress && Field[i - 1, j] is Barrier)
                    {
                        Field[i, j] = _Space;
                    }
                    #endregion

                    #region Meteor
                    else if (Field[i, j] is Meteor TemporaryMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        //space below meteor
                        if (Field[i + 1, j] is Space && TemporaryMeteor.Move)
                        {
                            //delete old
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new Meteor(GameProgress);
                        }
                        else if (Field[i + 1, j] is Rocket && TemporaryMeteor.Move)
                        {
                            MakeExplosion(i + 1, j, 1);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile && TemporaryMeteor.Move)
                        {   
                            //projectile gets deleted since meteor is undestructable
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new Meteor(GameProgress);
                        }
                        else if (Field[i + 1, j] is Player && TemporaryMeteor.Move)
                        {

                            Field[i, j] = _Space;
                            PlayerLifes--;
                        }
                        else if (Field[i + 1, j] is DestructableBarrier TemporaryDestructableBarrier && TemporaryMeteor.Move)
                        {
                            Field[i, j] = _Space;
                            //test if this will work
                            //TemporaryDestructableBarrier.Health--;
                            Field[i + 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                        else if (!TemporaryMeteor.Move)
                        {
                            //meteor moves every other tick, rework implementation with int for adjustable speeds
                            TemporaryMeteor.Move = true;
                        }
                    }
                    else if (Field[i, j] is Meteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        //delete meteor
                        Field[i, j] = _Space;
                    }
                    #endregion

                    #region DestructableMeteor
                    else if (Field[i, j] is DestructableMeteor TemporaryDestructableMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        if (TemporaryDestructableMeteor.Health <= 0)
                        {
                            //destr. meteor can only be destroyed by projectiles => explosion instead of space
                            Field[i, j] = new Explosion();
                        }
                        else if (Field[i + 1, j] is Space && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor.Health);
                        }
                        else if (Field[i + 1, j] is Rocket && TemporaryDestructableMeteor.Move)
                        {
                            MakeExplosion(i + 1, j , 1);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new DestructableMeteor(GameProgress, TemporaryDestructableMeteor.Health - 1);
                        }
                        else if (Field[i + 1, j] is Player && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = _Space;
                            PlayerLifes--;
                        }
                        else if (Field[i + 1, j] is DestructableBarrier TemporaryDestructableBarrier && TemporaryDestructableMeteor.Move)
                        {
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new DestructableBarrier(GameProgress, TemporaryDestructableBarrier.Health - 1);
                        }
                        else if (!TemporaryDestructableMeteor.Move)
                        {
                            TemporaryDestructableMeteor.Move = true;
                        }
                    }
                    else if (Field[i, j] is DestructableMeteor && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = _Space;
                    }
                    #endregion

                    #region EnemyProjectile
                    else if (Field[i, j] is EnemyProjectile && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {   
                        //enemies can destroy upgrades (for now)
                        if (Field[i + 1, j] is Space || Field[i + 1, j] is Upgrade)
                        {
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new EnemyProjectile(GameProgress);
                        }
                        else if (Field[i + 1, j] is Rocket)
                        {
                            //projectiles collide
                            MakeExplosion(i + 1, j, 1);
                        }
                        else if (Field[i + 1, j] is FriendlyProjectile || Field[i + 1, j] is Enemy)
                        {
                            Field[i, j] = _Space;
                            Field[i + 1, j] = new Explosion();
                        }
                        else if (Field[i + 1, j] is Player)
                        {
                            Field[i, j] = new Explosion();
                            PlayerLifes--;
                        }
                        //since projectiles are faster they can hit meteors even though they go the same direction
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
                        Field[i, j] = _Space;
                    }
                    #endregion

                    #region DestructableBarrier
                    else if (Field[i, j] is DestructableBarrier TemoraryDestructableBarrier)
                    {
                        if (TemoraryDestructableBarrier.Health <= 0)
                        {
                            //health of destructablebarriers doesn't get checked in other interactions so it is here
                            Field[i, j] = _Space;
                        }
                    }
                    #endregion

                    #region Upgrade
                    else if (Field[i, j] is Upgrade TemporaryUpgrade && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is not Barrier)
                    {
                        if (TemporaryUpgrade.Move >= 4)
                        {
                            if (Field[i + 1, j] is Space || Field[i + 1, j] is Enemy)
                            {
                                //upgrades destroy enemies so it's easier to get them
                                Field[i + 1, j] = new Upgrade(GameProgress, TemporaryUpgrade.UpgradeType);
                                Field[i, j] = _Space;
                            }
                            else if (Field[i + 1, j] is Rocket)
                            {
                                MakeExplosion(i + 1, j, 1);
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            else if (Field[i + 1, j] is FriendlyProjectile)
                            {
                                //no explosion here, upgrade just gets collected
                                Field[i, j] = _Space;  
                                Field[i + 1, j] = _Space; 
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            else if (Field[i + 1, j] is Player)
                            {
                                //collect upgrade
                                Field[i, j] = _Space;
                                PlayerWeapon = new Weapon(TemporaryUpgrade.UpgradeType);
                            }
                            //i think this can't even happen, but who knows 
                            else if (Field[i + 1, j] is DestructableMeteor)
                            {
                                Field[i, j] = _Space;
                            }
                            else if (Field[i + 1, j] is DestructableBarrier)
                            {
                                Field[i, j] = _Space;                         
                            }
                        }
                        else if (TemporaryUpgrade.Move < 4)
                        {
                            //upgrades move pretty slow
                            TemporaryUpgrade.Move++;
                        }
                    }
                    else if (Field[i, j] is Upgrade && Field[i, j].GameProgress != GameProgress && Field[i + 1, j] is Barrier)
                    {
                        Field[i, j] = _Space;                   
                    }
                    #endregion

                    #region Enemy
                    else if (Field[i, j] is Enemy TemporaryEnemy)
                    {
                        //for random enemy spawn
                        EnemyExists = true;
                        TemporaryEnemy.LifeTime++;
                        
                        //enemies move down very slow
                        TemporaryEnemy.LifeTimeForVerticalMovement++;
                        //vertical movement
                        if (TemporaryEnemy.LifeTimeForVerticalMovement > 67)
                        {
                            //upgrades can destroy enemies, but enemies can destroy upgrades too
                            if (Field[i + 1, j] is Space || Field[i + 1, j] is Upgrade)
                            {
                                Field[i, j] = _Space;
                                Field[i + 1, j] = new Enemy(GameProgress);
                            }
                            else if (Field[i + 1, j] is Player || Field[i + 1, j] is Barrier)
                            {
                                PlayerLifes--;
                                Kill();
                                Field[i, j] = _Space;
                            }
                            else if (Field[i + 1, j] is Rocket)
                            {
                                MakeExplosion(i + 1, j, 1);
                            }
                            else if (Field[i + 1, j] is FriendlyProjectile)
                            {
                                Field[i, j] = new Explosion();
                                Field[i + 1, j] = _Space;
                            }
                        }
                        //horizontal movement
                        else if (TemporaryEnemy.LifeTime > 20)
                        {
                            //decide if enemy goes left or right
                            int move = Random.Next(2);
                            //go left if there is no barrier or other enemy
                            if (move == 0 && Field[i, j - 1] is not Barrier && Field[i, j - 1] is not Enemy)
                            {
                                Field[i, j - 1] = new Enemy(GameProgress, TemporaryEnemy.LifeTimeForVerticalMovement);
                                Field[i, j] = _Space;
                            }
                            //go right if there is no barrier or other enemy
                            else if (move == 1 && Field[i, j + 1] is not Barrier && Field[i, j + 1] is not Enemy)
                            {
                                Field[i, j + 1] = new Enemy(GameProgress, TemporaryEnemy.LifeTimeForVerticalMovement);
                                Field[i, j] = _Space;
                            }
                            //if he can not go left or right, just shoot
                            else if (Field[i + 1, j] is not Enemy && Field[i + 1, j] is not Barrier)
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
                        //text disappears after it's lifetime. currently only used for the tutorial
                        if (TemporaryText.GameProgress >= TemporaryText.Lifetime)
                            Field[i, j] = _Space;
                        else
                            TemporaryText.GameProgress++;
                    }
                    #endregion
                }
            }
        }
        public void RandomEnemySpawn() //spawn function is seperate from game update
        {
            EnemySpawnCounter++;
            if (EnemySpawnCounter > EnemySpawnrate || EnemyExists == false)
            {
                //set this to true since one is about to be created
                EnemyExists = true;
                bool EnemyNotSpawned = true;
                while (EnemyNotSpawned)
                {
                    //find space on the vertical axis
                    int RandomSpawn = Random.Next(1, FieldLengthX - 2);
                    //if there is space, create enemy
                    if (Field[EnemyLocationY, RandomSpawn] is Space)
                    {
                        Field[EnemyLocationY, RandomSpawn] = new Enemy(GameProgress);
                        EnemyNotSpawned = false;
                    }
                }
                //reset the counter
                EnemySpawnCounter = 0;
            }
        }
        public void RandomMeteorSpawn(int Spawnrate)
        {
            MeteorSpawnCounter++;
            if (MeteorSpawnCounter > Spawnrate)
            {
                bool MeteorNotSpawned = true;
                while (MeteorNotSpawned)
                {
                    //get spawnpoint
                    int RandomSpawn = Random.Next(1, FieldLengthX - 2);
                    //decide which meteor type it is
                    int WhichMeteor = Random.Next(2);
                    if (Field[EnemyLocationY, RandomSpawn] is Space)
                    {
                        if (WhichMeteor == 0)
                        {
                            Field[EnemyLocationY, RandomSpawn] = new DestructableMeteor(GameProgress, 3);
                        }
                        else
                        {
                            Field[EnemyLocationY, RandomSpawn] = new Meteor(GameProgress);
                        }
                        MeteorNotSpawned = false;
                    }
                }
                MeteorSpawnCounter = 0;
            }
        }
        public void RandomUpgradeSpawn(int Spawnrate)
        {   
            //i don't know why % operator here, but hell yeah
            if (GameProgress % Spawnrate == 0)
            {
                bool UpgradeNotSpawned = true;
                int WhichUpgrade = Random.Next(3);

                while (UpgradeNotSpawned)
                { 
                    int RandomSpawn = Random.Next(1, Field.GetLength(1) - 2);
                    if (Field[EnemyLocationY, RandomSpawn] is Space || Field[EnemyLocationY, RandomSpawn] is FriendlyProjectile || Field[EnemyLocationY, RandomSpawn] is Enemy)
                    {
                        //should be improvable if C# enmus are just integers underneath
                        if (WhichUpgrade == 0)
                        {
                            Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Laser);
                        }
                        else if (WhichUpgrade == 1)
                        {
                            Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Minigun);
                        }
                        else if (WhichUpgrade == 2)
                        {
                            Field[EnemyLocationY, RandomSpawn] = new Upgrade(GameProgress, WeaponType.Rocket);
                        }
                        UpgradeNotSpawned = false;
                    }
                }
            }
        }         
        public void PrintLevel()
        {
            for (int i = 0; i < FieldLengthY; i++)
            {
                for (int j = 0; j < FieldLengthX; j++)
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
            //calculates padding for smooth UI
            int AmountOfPadding = FieldLengthX - 14 /* 7 characters for "Lifes: " and 7 for "Score: " => 14 */ - Convert.ToString(PlayerLifes).Length - Convert.ToString(Score).Length;
            Write(new string(' ', AmountOfPadding));
            ForegroundColor = Yellow;
            WriteLine($"Score: {Score}");           
        }
        public void PrintWeaponStatus()
        {
            //clear the line so new one can be drawn when weapon is ready to fire
            if (PlayerWeapon.Cooldown == 0)
            {
                ClearLine();
            }

            //reload bar color based on ammo left
            double WeaponPercentage = Convert.ToDouble(PlayerWeapon.Ammo) / Convert.ToDouble(PlayerWeapon.MaxAmmo);
            if (WeaponPercentage >= 0.0 && WeaponPercentage <= 0.333)
                ForegroundColor = Red;
            else if (WeaponPercentage > 0.333 && WeaponPercentage <= 0.666)
                ForegroundColor = ConsoleColor.DarkYellow;
            else
                ForegroundColor= Green;

            //standard weapon has different color scheme
            if (PlayerWeapon.Type == WeaponType.Standard)
            {
                //red if not loaded
                if (PlayerWeapon.Cooldown < PlayerWeapon.MaxCooldown)
                    ForegroundColor = Red;
                //weapon loaded
                else
                    ForegroundColor = Green;
            }

            if (PlayerWeapon.Type == WeaponType.Minigun)
            {
                //calculate the number of █ characters to display based on current ammo
                int NumCharacters = (int)Math.Round((double)PlayerWeapon.Ammo / PlayerWeapon.MaxAmmo * FieldLengthX);

                //prevent string from going out of bounds or being wonky
                if (NumCharacters > FieldLengthX || PlayerWeapon.IsAmmoFull())
                    NumCharacters = FieldLengthX;

                Write(new string('█', NumCharacters));

                //this might still be wonky in a few edge cases but MaxAmmo should be an even and nicely divisible integer anyway so it shouldn't happen
            }
            else if (PlayerWeapon.Type != WeaponType.Minigun)

                //weapon is ready to fire                
                if (PlayerWeapon.Cooldown >= PlayerWeapon.MaxCooldown)
                {
                    Write(new string('█', FieldLengthX));
                }
                else
                {
                    //reloading weapon status for all weapons except minigun
                    Write(new string('█', FieldLengthX / PlayerWeapon.MaxCooldown * PlayerWeapon.Cooldown));
                }

            ForegroundColor = DarkCyan;
            WriteLine();
            ClearLine();
            Write($"Weapon: {PlayerWeapon.Type}");  

            //weapons with ammo
            if (!(PlayerWeapon.Type == WeaponType.Standard))
            {
                {
                    //╚═════════════════════════╝
                    //Weapon: Minigun   Ammo: 100
                    //---8----          --6---
                    int AmountOfPadding = FieldLengthX - (8 + PlayerWeapon.Type.ToString().Length + 6 + Convert.ToString(PlayerWeapon.Ammo).Length);
                    Write(new string(' ', AmountOfPadding));
                    
                }
                ForegroundColor = Cyan;
                WriteLine($"Ammo: {PlayerWeapon.Ammo}");
            }
            else
                WriteLine();
        }
        public void PrintEnemySpawnrate() //this is a pure debug tool
        {
            ClearLine();
            WriteLine($"Enemy Spawnrate: {EnemySpawnrate}");
        } 
        public void PrintGameProgress() //this is a pure debug tool
        {
            WriteLine($"Gameprogress: {GameProgress}");
        }
        public void InsertTextInLevel(string Text, int LineX, int LineY, int Lifetime, ConsoleColor Color)
        {
            char[] Array = Text.ToCharArray();
            for (int i = 0; i < Array.Length; i++)
            {   
                //make sure Player doesn't get deleted
                if (Field[LineY, i + LineX] is Player)
                    Field[LineY + 1, i + LineX] = new Player(GameProgress);
                if (Array[i] == ' ')
                    Field[LineY, i + LineX] = _Space;
                else
                    Field[LineY, i + LineX] = new Text(Array[i], Color, GameProgress, Lifetime + GameProgress);
            }
        }
        public void MakeExplosion(int I, int J, int Radius)
        {   
            //go to the left by Radius amount of cells
            for (int j2 = J - Radius; j2 < J + Radius + 1; j2++)
            {
                //go up by Radius amount of cells
                for (int i2 = I - Radius; i2 < I + Radius + 1; i2++)
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
            Write(new string(' ', 50));
            CursorLeft = 0;
        }
        public void Kill() 
        {
            /*
            modulo can work if score is only increased by 100, but more enemy types giving different score will mess this up inevitably
            Score += 100;
            if (Score % 1000 == 0)
                EnemySpawnrate -= 10;
            */
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
        }
    }
}