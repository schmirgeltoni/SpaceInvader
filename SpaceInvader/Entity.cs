using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public class Entity
    {
        public char Icon { get; set; }
        public ConsoleColor Color { get; set; }
        public int GameProgress { get; set; }
        public Entity(char Icon, ConsoleColor Color, int GameProgress)
        {
            this.Icon = Icon;
            this.Color = Color;
            this.GameProgress = GameProgress;
        }
        public Entity(ConsoleColor Color, int GameProgress)
        {
            this.Color = Color;
            this.GameProgress = GameProgress;
        }
        public override string ToString() => $"Icon: {Icon}, Color: {Color}, Gameprogress: {GameProgress}";      
    }   
    public class Player : Entity
    {      
        public Player(int GameProgress) : base('A', Green, GameProgress) { }

    }
    public class Enemy : Entity
    {
        public int LifeTimeForHorizontalMovement { get; set; }
        public int LifeTimeForVerticalMovement { get; set; }
        public int LifeTimeForShooting { get; set; }
        public Enemy(int GameProgress, char Icon, ConsoleColor Color, int LifeTimeForShooting) : base(Icon, Color, GameProgress) { }
        public Enemy(int GameProgress, int LifeTimeForVerticalMovement, int LifeTimeForShooting) : base('V', Red, GameProgress) 
        {
            LifeTimeForHorizontalMovement = 0;
            this.LifeTimeForVerticalMovement = LifeTimeForVerticalMovement;
            this.LifeTimeForShooting = LifeTimeForShooting;
        }
        public Enemy(int GameProgress, int LifeTimeForShooting) : base('V', Red, GameProgress) 
        { 
            LifeTimeForHorizontalMovement = 0;
            LifeTimeForVerticalMovement = 0;
            this.LifeTimeForShooting = LifeTimeForShooting; 
        }
    }
    public class Turret : Enemy
    {
        public int ShotsFired { get; set; }
        public int ShotsFiredBeforeMovement { get; set; }
        public int AmountOfTilesToMove {  get; set; }
        public int TilesMoved { get; set; }
        public int Direction {  get; set; }
        public Turret(int GameProgress, int ShotsFired, int AmountOfTilesToMove, int TilesMoved, int Direction) : base(GameProgress, 'U', Red, 0)
        {
            this.ShotsFired = ShotsFired;
            ShotsFiredBeforeMovement = 25;
            this.AmountOfTilesToMove = AmountOfTilesToMove;
            this.TilesMoved = TilesMoved;
            this.Direction = Direction;
        }
    }
    public class FriendlyProjectile : Entity
    {        
        public FriendlyProjectile(int GameProgress) : base('|', Green, GameProgress) { }       
    }
    public class Rocket : FriendlyProjectile
    {
        public Rocket(int GameProgress) : base(GameProgress)
        {
            Icon = '┴';
        }
    }
    public class Laser : Entity
    {
        public Laser(int GameProgress) : base('|', Green, GameProgress) { }
    }
    public class EnemyProjectile : Entity
    {
        public ConsoleColor color = Red;
        public EnemyProjectile(int GameProgress) : base('|', Red, GameProgress) { }
    }
    public class Barrier : Entity
    {
        public Barrier(int GameProgress) : base('█', DarkGray, GameProgress) { }
        public Barrier(char Icon, ConsoleColor Color, int GameProgress) : base(Icon, Color, GameProgress) { }
        public Barrier(char Icon) : base(Icon, DarkGray, 0) { }
    }
    public class Space : Entity
    {
        public Space() : base(' ', White, 0) { }
    }
    public class DestructableBarrier : Entity
    {
        public int Health { get; set; }
        public DestructableBarrier(int GameProgress, int Health) : base(Blue, GameProgress)
        {            
            this.Health = Health;
            /*
               █ ▆ ▅ ▄ ▃ ▂      █
               ▆                ▓
               ▅                ▒
               ▄                ░
               ▃ 
               ▂ 
            */
            if (Health > 6) //== 8 || Health == 7 )            
                Icon = '█';            
            else if (Health == 6 || Health == 5)
                Icon = '▓';
            else if (Health == 4 || Health == 3)
                Icon = '▒';
            else if (Health == 2 || Health == 1)
                Icon = '░';           
        }
    }
    public class DestructableMeteor : Entity
    {
        public int LifeTime {  get; set; }
        public int Health { get; set; }

        public DestructableMeteor(int GameProgress, int Health) : base(Blue, GameProgress)
        {
            Icon = '*';
            this.Health = Health;
        }
    }
    public class Meteor : Barrier
    {
        public bool Move { get; set; }
        public int LifeTime { get; set; }
        public Meteor(int GameProgress) : base(GameProgress)
        {
            Icon = '*';
            Color = White;
            Move = false;
        }
    }
    public class Explosion : Entity
    {
        public Explosion() : base('X', DarkRed, 0) { }
    }
    public class AimBarrier : Barrier
    {
        public AimBarrier(int GameProgress) : base(GameProgress)
        {            
            Color = Yellow;
            Icon = '═';
        }
    }
    public class Text : Barrier
    {
        public int Lifetime { get; set; }
        public Text(char Icon, ConsoleColor Color, int GameProgress, int Lifetime) : base(Icon, Color, GameProgress)
        {
            this.Lifetime = Lifetime;
        }
    }
    public class Upgrade : Entity
    {
        public WeaponType UpgradeType { get; set; }
        public int Move { get; set; }
        public Upgrade(int GameProgress, WeaponType UpgradeType) : base('o', Magenta, GameProgress)
        {
            this.UpgradeType = UpgradeType;
            Move = 0;
        }
    }
}