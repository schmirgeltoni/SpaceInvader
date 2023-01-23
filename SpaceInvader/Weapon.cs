using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public enum WeaponType { Standard, Laser, Rocket, Minigun }
    public class Weapon
    {
        public int Cooldown { get; set; }
        public int MaxCooldown { get; set; }
        public int Uses { get; set; }
        public WeaponType Type { get; set; }
        public bool Toggle { get; set; }
        public Weapon(WeaponType Type)
        {
            this.Type = Type;            
            if (Type == WeaponType.Laser)
            {
                MaxCooldown = 25;
                Uses = 10;
            }
            else if (Type == WeaponType.Rocket)
            {
                MaxCooldown = 8;
                Uses = 15;
            }
            else if (Type == WeaponType.Minigun)    
            {
                Uses = 100;
                Toggle = false;
            }
            else if (Type == WeaponType.Standard)
            {
                MaxCooldown = 5;
            }
            Cooldown = MaxCooldown;
        }
        public Weapon(WeaponType Type, int Uses, int MaxCooldown, int Cooldown)
        {
            this.Type= Type;
            this.Uses = Uses;
            this.MaxCooldown = MaxCooldown;
            this.Cooldown = Cooldown;
            Toggle = false;
        }
    }
}