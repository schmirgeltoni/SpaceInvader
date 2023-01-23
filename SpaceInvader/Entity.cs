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
        public int LifeTime { get; set; }
        public int LifeTimeForVerticalMovement { get; set; }
        public Enemy(int GameProgress, char Icon, ConsoleColor Color) : base(Icon, Color, GameProgress) { }
        public Enemy(int GameProgress, int LifeTimeForVerticalMovement) : base('V', Red, GameProgress) { LifeTime = 0; this.LifeTimeForVerticalMovement = LifeTimeForVerticalMovement; }
        public Enemy(int GameProgress) : base('V', Red, GameProgress) { LifeTime = 0; LifeTimeForVerticalMovement = 0; }

    }
    public class Turret : Enemy
    {
        public Turret(int GameProgress) : base(GameProgress,'U', Red) { }
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
    public class Meteor : Barrier
    {
        public bool Move { get; set; }
        public Meteor(int GameProgress) : base(GameProgress)
        {
            Icon = '*';
            Color = White;
            Move = false;
        }
    }
    public class DestructableMeteor : DestructableBarrier
    {
        public bool Move { get; set; }
#pragma warning disable CS0108 // Element blendet vererbte Element aus; fehlendes 'new'-Schlüsselwort
        public int Health { set; get; }
#pragma warning restore CS0108 // Element blendet vererbte Element aus; fehlendes 'new'-Schlüsselwort
        public DestructableMeteor(int GameProgress, int Health) : base(GameProgress, Health)
        {
            Icon = '*';
            Color = Blue;
            this.Health = Health;
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
            Icon = '█';
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