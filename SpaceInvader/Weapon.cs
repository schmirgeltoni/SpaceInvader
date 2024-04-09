using static System.Console;
using static System.ConsoleColor;

namespace SpaceInvader
{
    public enum WeaponType { Standard, Laser, Rocket, Minigun }
    public class Weapon
    {
        public int Cooldown { get; set; }
        public int MaxCooldown { get; set; }
        public int Ammo { get; set; }
        public int MaxAmmo { get; set; }
        public WeaponType Type { get; set; }
        //toggle for minigun
        public bool Toggle { get; set; }
        public int MiniGunRateOfFire { get; set; }
        public Weapon(WeaponType Type)
        {
            this.Type = Type;            
            if (Type == WeaponType.Laser)
            {
                MaxCooldown = 25;
                MaxAmmo = 10;
                Ammo = MaxAmmo;
            }
            else if (Type == WeaponType.Rocket)
            {
                MaxCooldown = 8;
                MaxAmmo = 15;
                Ammo = MaxAmmo;
            }
            else if (Type == WeaponType.Minigun)    
            {
                MaxAmmo = 100;
                Ammo = MaxAmmo;
                Toggle = false;
                MiniGunRateOfFire = 2;
            }
            else if (Type == WeaponType.Standard)
            {
                MaxCooldown = 5;
                //prevent DivideByZero
                MaxAmmo = 1;
            }
            Cooldown = MaxCooldown;
        }
        public Weapon(WeaponType Type, int Ammo, int MaxCooldown, int Cooldown, int MaxAmmo)
        {
            this.Type= Type;
            this.Ammo = Ammo;
            this.MaxCooldown = MaxCooldown;
            this.Cooldown = Cooldown;
            this.MaxAmmo = MaxAmmo;
            Toggle = false;
        }
        public bool IsAmmoFull() => MaxAmmo == Ammo;
    }
}