using System;
using System.Drawing;
using System.Windows.Forms;

namespace URPGDesktopCalc.Classes
{
    public class BattlePokemon : Pokemon
    {
        public double CurrentHP { get; set; }
        public double Percentage { get; set; }
        public bool Sub { get; set; }
        public double SubHP { get; set; }

        public char Which { get; set; }

        public string Gender { get; set; }
        public string Ability { get; set; }
        public string Status { get; set; }
        public string Effect { get; set; }
        public string Item { get; set; }

        public double BasePower { get; set; }
        public TypeObject AttackType { get; set; }
        public string AttackClass { get; set; }

        public double ATTmod { get; set; }
        public double DEFmod { get; set; }
        public double SPAmod { get; set; }
        public double SPDmod { get; set; }
        public double SPEmod { get; set; }

        public bool isDynamaxed { get; set; }

        public BattlePokemon()
        {
        }

        public BattlePokemon(Pokemon P)
        {
            Name = P.Name;
            Type1 = P.Type1;
            Type2 = P.Type2;
            MaxHP = P.MaxHP;
            ATT = P.ATT;
            DEF = P.DEF;
            SPA = P.SPA;
            SPD = P.SPD;
            SPE = P.SPE;

            CurrentHP = MaxHP;
            Percentage = 100.0;
            Sub = false;
            SubHP = 0.0;

            ATTmod = 1.0;
            DEFmod = 1.0;
            SPAmod = 1.0;
            SPDmod = 1.0;
            SPEmod = 1.0;
        }

        public BattlePokemon(BattlePokemon P)
        {
            Name = P.Name;
            Type1 = P.Type1;
            Type2 = P.Type2;
            MaxHP = P.MaxHP;
            ATT = P.ATT;
            DEF = P.DEF;
            SPA = P.SPA;
            SPD = P.SPD;
            SPE = P.SPE;

            CurrentHP = P.CurrentHP;
            Percentage = P.Percentage;
            Sub = P.Sub;
            SubHP = P.SubHP;

            ATTmod = P.ATTmod;
            DEFmod = P.DEFmod;
            SPAmod = P.SPAmod;
            SPDmod = P.SPDmod;
            SPEmod = P.SPEmod;
        }

        public void SetHealth(double newHP)
        {
            CurrentHP = Math.Min(newHP, MaxHP);
            Percentage = Math.Round(CurrentHP/MaxHP*100, 2);
        }

        public void ChangeHealth(double amount)
        {
            //Takes amount as a percentage for Sixteenths function
            amount /= 100;

            if (amount > 0)
            {
                amount = Math.Floor(amount * MaxHP);
                CurrentHP = Math.Min(CurrentHP + amount, MaxHP);
            }
            else
            {
                amount = Math.Ceiling(amount * MaxHP);
                CurrentHP = Math.Max(CurrentHP + amount, 0);
            }

            Percentage = Math.Round(CurrentHP / MaxHP * 100, 2);
        }

        public void SmackSelf(double Damage)
        {
            CurrentHP = Math.Max(CurrentHP + Damage, 0);
            Percentage = Math.Round(CurrentHP / MaxHP * 100, 2);
        }

        public void TakeDamage(double Damage)
        {
            if (Sub)
            {
                SubHP = Math.Max(SubHP + Damage, 0);

                if (SubHP == 0)
                {
                    MessageBox.Show("Substitute broke.");
                    Sub = false;
                }
            }
            else
            {
                CurrentHP = Math.Max(CurrentHP + Damage, 0);
                Percentage = Math.Round(CurrentHP / MaxHP * 100, 2);
            }
        }

        public void SetPercentage(double newPer)
        {
            Percentage = Math.Min(newPer, 100.0);
            CurrentHP = Math.Round(newPer / 100 * MaxHP);
        }

        public void CreateSub()
        {
            SubHP = Math.Floor(MaxHP/4);
            Sub = true;
            SetHealth(CurrentHP - SubHP);
        }

        public void SetSubHP(double health)
        {
            SubHP = health;

            Sub = SubHP != 0;
        }

        public void ToggleDynamax(Button butt)
        {
            if (isDynamaxed)
            {
                MaxHP /= 2;
                CurrentHP /= 2;
                butt.FlatAppearance.BorderColor = Color.Empty;
            }
            else
            {
                MaxHP *= 2;
                CurrentHP *= 2;
                butt.FlatAppearance.BorderColor = Color.Red;
            }

            isDynamaxed = !isDynamaxed;
        }

        public void ResetMods()
        {
            ATTmod = 1;
            DEFmod = 1;
            SPAmod = 1;
            SPDmod = 1;
            SPEmod = 1;
        }
    }
}
