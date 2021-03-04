using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using URPGDesktopCalc.Classes;

/* URPG Desktop Calc
 * Version: 1.6
 * Written by Monbrey
 */

namespace URPGDesktopCalc
{
    public partial class Calc : Form
    {
        BindingList<Pokemon> PokemonList;
        BindingList<Object> Abilities, Genders, Attacks, Weathers, Statuses, Effects;
        BindingList<TypeObject> Types;
        BindingList<StatMod> StatMods;

        enum TypeCode : int { NO, B, DK, DR, E, FA, FI, FR, FL, GH, GR, GD, I, NM, PO, PS, R, S, W };

        BattlePokemon[] StoredPokemon = new BattlePokemon[12];
        TextBox[] StorageBoxes;

        BattlePokemon BattlePokemonA;
        BattlePokemon BattlePokemonB;

        bool ChangeFlag = true;
        //I use this to override some default functionality

        public Calc()
        {
            InitializeComponent();

            CreateBindingLists();
            AssignBindingLists();

            InitializeComboBoxes();
            InitializeModBoxesA();
            InitializeModBoxesB();
            InitializeStorage();
        }

        #region Setup Functions

        protected void CreateBindingLists()
        {
            //Pokemon
            PokemonList = new BindingList<Pokemon>();
            string[] FileLines = Properties.Resources.Pokemon.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string S in FileLines)
            {
                string[] values = S.Split(',');
                PokemonList.Add(new Pokemon(values));
            }

            //Abilities
            Abilities = new BindingList<Object>();
            FileLines = URPGDesktopCalc.Properties.Resources.Abilities.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string S in FileLines)
            {
                string[] values = S.Split(',');
                Abilities.Add(new Object(values[0], values[1]));
            }

            //Types
            Types = new BindingList<TypeObject>();
            FileLines = URPGDesktopCalc.Properties.Resources.Types.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string S in FileLines)
            {
                string[] values = S.Split(',');
                Types.Add(new TypeObject(values[0], values[1]));
            }

            //Compatibilities
            int TypeIndex = 1;
            FileLines = URPGDesktopCalc.Properties.Resources.Compatibilities.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string S in FileLines)
            {
                string[] sections = S.Split(',');
                Types[TypeIndex].Compatibility = new Compat(sections[1].Split('|'), sections[2].Split('|'), sections[3].Split('|'));
                TypeIndex++;
            }

            Genders = new BindingList<Object>
            {
                new Object("M", "Male"),
                new Object("F", "Female"),
                new Object("X", "Genderless")
            };

            Attacks = new BindingList<Object>
            {
                new Object("PHYS", "Physical"),
                new Object("SPEC", "Special"),
                new Object("SAvD", "SpA vs Def")
            };

            Weathers = new BindingList<Object>
            {
                new Object("NO", "(none)"),
                new Object("S", "Sun"),
                new Object("HS", "Harsh Sunlight"),
                new Object("R", "Rain"),
                new Object("HR", "Heavy Rain"),
                new Object("SS", "Sandstorm"),
                new Object("H", "Hail"),
                new Object("MAC", "Mysterious Air Current"),
                new Object("F", "Fog")
            };

            Statuses = new BindingList<Object>
            {
                new Object("NO", "(none)"),
                new Object("BRN", "Burn"),
                new Object("FRZ", "Freeze"),
                new Object("PAR", "Paralysis"),
                new Object("PSN", "Poison"),
                new Object("TOX", "Toxic"),
                new Object("SLP", "Sleep")
            };

            Effects = new BindingList<Object>
            {
                new Object("NO", "(none)"),
                new Object("R", "Reflect"),
                new Object("LS", "L Screen"),
                new Object("MF", "Me First")
            };

            StatMods = new BindingList<StatMod>
            {
                new StatMod(4.0f, "+6"),
                new StatMod(3.5f, "+5"),
                new StatMod(3.0f, "+4"),
                new StatMod(2.5f, "+3"),
                new StatMod(2.0f, "+2"),
                new StatMod(1.5f, "+1"),
                new StatMod(1.0f, "0"),
                new StatMod((2.0 / 3.0), "-1"),
                new StatMod((2.0 / 4.0), "-2"),
                new StatMod((2.0 / 5.0), "-3"),
                new StatMod((2.0 / 6.0), "-4"),
                new StatMod((2.0 / 7.0), "-5"),
                new StatMod((2.0 / 8.0), "-6"),
            };
        }

        protected void AssignBindingLists()
        {
            PokemonA.DataSource = PokemonList;
            PokemonA.SelectedIndex = -1;
            PokemonA.SelectedIndexChanged += PokemonAChanged;

            PokemonB.DataSource = new BindingList<Pokemon>(PokemonList);
            PokemonB.SelectedIndex = -1;
            PokemonB.SelectedIndexChanged += PokemonBChanged;

            AttModA.DataSource = StatMods;
            AttModB.DataSource = new BindingList<StatMod>(StatMods);
            DefModA.DataSource = new BindingList<StatMod>(StatMods);
            DefModB.DataSource = new BindingList<StatMod>(StatMods);
            SpaModA.DataSource = new BindingList<StatMod>(StatMods);
            SpaModB.DataSource = new BindingList<StatMod>(StatMods);
            SpdModA.DataSource = new BindingList<StatMod>(StatMods);
            SpdModB.DataSource = new BindingList<StatMod>(StatMods);
            SpeModA.DataSource = new BindingList<StatMod>(StatMods);
            SpeModB.DataSource = new BindingList<StatMod>(StatMods);

            TypeA1.DataSource = Types;
            TypeA2.DataSource = new BindingList<TypeObject>(Types);
            TypeB1.DataSource = new BindingList<TypeObject>(Types);
            TypeB2.DataSource = new BindingList<TypeObject>(Types);

            AttackTypeA.DataSource = new BindingList<TypeObject>(Types);
            AttackTypeB.DataSource = new BindingList<TypeObject>(Types);

            GenderA.DataSource = Genders;
            GenderB.DataSource = new BindingList<Object>(Genders);

            AbilityA.DataSource = Abilities;
            AbilityB.DataSource = new BindingList<Object>(Abilities);

            AttackClassA.DataSource = Attacks;
            AttackClassB.DataSource = new BindingList<Object>(Attacks);

            WeatherBox.DataSource = Weathers;

            StatusA.DataSource = Statuses;
            StatusB.DataSource = new BindingList<Object>(Statuses);

            EffectA.DataSource = Effects;
            EffectB.DataSource = new BindingList<Object>(Effects);
        }

        protected void InitializeComboBoxes()
        {
            TypeA1.SelectedIndex = 0;
            TypeA2.SelectedIndex = 0;
            TypeB1.SelectedIndex = 0;
            TypeB2.SelectedIndex = 0;

            GenderA.SelectedIndex = 0;
            GenderB.SelectedIndex = 0;

            AbilityA.SelectedIndex = 0;
            AbilityB.SelectedIndex = 0;

            HItemA.SelectedIndex = 0;
            HItemB.SelectedIndex = 0;

            CritA.SelectedIndex = 1;
            CritB.SelectedIndex = 1;

            AccMod.SelectedIndex = 6;
            EvaMod.SelectedIndex = 6;

            WeatherBox.SelectedIndex = 0;
            OtherBox.SelectedIndex = 0;

            StatusA.SelectedIndex = 0;
            StatusB.SelectedIndex = 0;

            EffectA.SelectedIndex = 0;
            EffectB.SelectedIndex = 0;
        }

        protected void InitializeModBoxesA()
        {
            AttModA.SelectedIndex = 6;
            DefModA.SelectedIndex = 6;
            SpaModA.SelectedIndex = 6;
            SpdModA.SelectedIndex = 6;
            SpeModA.SelectedIndex = 6;
        }

        protected void InitializeModBoxesB()
        {
            AttModB.SelectedIndex = 6;
            DefModB.SelectedIndex = 6;
            SpaModB.SelectedIndex = 6;
            SpdModB.SelectedIndex = 6;
            SpeModB.SelectedIndex = 6;
        }

        protected void InitializeStorage()
        {
            TextBox[] temp = { SP1A, SP2A, SP3A, SP4A, SP5A, SP6A, SP1B, SP2B, SP3B, SP4B, SP5B, SP6B };
            StorageBoxes = temp;
        }

        #endregion Setup Functions

        #region Display Updaters

        public void UpdateHealthDisplay()
        {
            if (BattlePokemonA != null)
            {
                MaxHPA.Text = BattlePokemonA.MaxHP.ToString();
                HPResultA.Text = BattlePokemonA.CurrentHP.ToString();
                PercentResultA.Text = $"{BattlePokemonA.Percentage:F2}%";
                SubHealthA.Text = BattlePokemonA.Sub ? BattlePokemonA.SubHP.ToString() : "None";
            }
            if (BattlePokemonB != null)
            {
                MaxHPB.Text = BattlePokemonB.MaxHP.ToString();
                HPResultB.Text = BattlePokemonB.CurrentHP.ToString();
                PercentResultB.Text = $"{BattlePokemonB.Percentage:F2}%";
                SubHealthB.Text = BattlePokemonB.Sub ? BattlePokemonB.SubHP.ToString() : "None";
            }
        }

        public void UpdateStatDisplay()
        {
            if (BattlePokemonA != null)
            {
                AttackA.Text = (Math.Floor(BattlePokemonA.ATT * BattlePokemonA.ATTmod)).ToString();
                DefenceA.Text = (Math.Floor(BattlePokemonA.DEF * BattlePokemonA.DEFmod)).ToString();
                SpAttackA.Text = (Math.Floor(BattlePokemonA.SPA * BattlePokemonA.SPAmod)).ToString();
                SpDefenceA.Text = (Math.Floor(BattlePokemonA.SPD * BattlePokemonA.SPDmod)).ToString();
            }
            if (BattlePokemonB != null)
            {
                AttackB.Text = (Math.Floor(BattlePokemonB.ATT * BattlePokemonB.ATTmod)).ToString();
                DefenceB.Text = (Math.Floor(BattlePokemonB.DEF * BattlePokemonB.DEFmod)).ToString();
                SpAttackB.Text = (Math.Floor(BattlePokemonB.SPA * BattlePokemonB.SPAmod)).ToString();
                SpDefenceB.Text = (Math.Floor(BattlePokemonB.SPD * BattlePokemonB.SPDmod)).ToString();
            }

            CalculateSpeed();
        }

        public void UpdateTeamDisplay()
        {
            for (int i = 0; i < 12; i++)
            {
                if (StoredPokemon[i] != null)
                {
                    StorageBoxes[i].Text = $"Name: {StoredPokemon[i].Name}\r\n";
                    StorageBoxes[i].Text += "Ability: ";
                    foreach (Object A in Abilities)
                    {
                        if (A.Code == StoredPokemon[i].Ability)
                            StorageBoxes[i].Text += A.Name;
                    }
                    StorageBoxes[i].Text += $"\r\nItem: {StoredPokemon[i].Item}\r\n\r\n";
                    StorageBoxes[i].Text += $"[{StoredPokemon[i].Percentage}%] ";
                    if (StoredPokemon[i].Status != "NO" && StoredPokemon[i].Status != null)
                        StorageBoxes[i].Text += $"[{StoredPokemon[i].Status}] ";
                }
            }
        }

        #endregion

        #region HP and Stat Modifiers

        protected void DynamaxPokemon(object sender, EventArgs e)
        {
            Button butt = (Button)sender;
            char Which = butt.Name[butt.Name.Length - 1];

            switch (Which)
            {
                case 'A':
                    BattlePokemonA?.ToggleDynamax(butt);
                    break;
                case 'B':
                    BattlePokemonB?.ToggleDynamax(butt);
                    break;
            }

            UpdateHealthDisplay();
        }

        protected void ManualHPMod(object sender, EventArgs e)
        {
            Button butt = (Button)sender;
            char Attacker = butt.Name[butt.Name.Length - 1];

            double changeHealth;

            if (Attacker == 'A' && ManualA.Text != "")
            {
                if (butt.Name == "ManHPA")
                    if (!Regex.IsMatch(ManualA.Text, @"^[0-9-]+$"))
                    {
                        MessageBox.Show("Entry should be a whole number.", "Error");
                        return;
                    }
                if (butt.Name == "ManPercentA")
                    if (!Regex.IsMatch(ManualA.Text, @"^[0-9.-]+$"))
                    {
                        MessageBox.Show("Entry should be a whole number or decimal.", "Error");
                        return;
                    }

                changeHealth = Convert.ToDouble(ManualA.Text);

                if (butt.Name == "ManPercentA")
                    changeHealth = Math.Round((changeHealth / 100) * BattlePokemonA.MaxHP);

                BattlePokemonA.SetHealth(BattlePokemonA.CurrentHP + changeHealth);
            }
            if (Attacker == 'B' && ManualB.Text != "")
            {
                if (butt.Name == "ManHPB")
                    if (!Regex.IsMatch(ManualB.Text, @"^[0-9-]+$"))
                    {
                        MessageBox.Show("Entry should be a whole number.", "Error");
                        return;
                    }
                if (butt.Name == "ManPercentB")
                    if (!Regex.IsMatch(ManualB.Text, @"^[0-9.-]+$"))
                    {
                        MessageBox.Show("Entry should be a whole number or decimal.", "Error");
                        return;
                    }

                changeHealth = Convert.ToDouble(ManualB.Text);

                if (butt.Name == "ManPercentB")
                    changeHealth = Math.Round((changeHealth / 100) * BattlePokemonB.MaxHP);

                BattlePokemonB.SetHealth(BattlePokemonB.CurrentHP + changeHealth);
            }

            UpdateHealthDisplay();
        }

        protected void ManualStatEntry(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                e.Handled = true;
            else
                return;

            TextBox stat = (TextBox)sender;
            char Attacker = stat.Name[stat.Name.Length - 1];
            string statName = stat.Name.Substring(0, 3);

            if (Attacker == 'A' && BattlePokemonA != null)
            {
                if (!Regex.IsMatch(stat.Text, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole, positive number.", "Error");
                    return;
                }

                switch (statName)
                {
                    case "Max": BattlePokemonA.MaxHP = Convert.ToDouble(stat.Text);
                        break;
                    case "Att": BattlePokemonA.ATT = Convert.ToDouble(stat.Text);
                        break;
                    case "Def": BattlePokemonA.DEF = Convert.ToDouble(stat.Text);
                        break;
                    case "SpA": BattlePokemonA.SPA = Convert.ToDouble(stat.Text);
                        break;
                    case "SpD": BattlePokemonA.SPD = Convert.ToDouble(stat.Text);
                        break;
                    case "Spe": BattlePokemonA.SPE = Convert.ToDouble(stat.Text);
                        break;
                }
            }
            if (Attacker == 'B' && BattlePokemonB != null)
            {
                if (!Regex.IsMatch(stat.Text, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole, positive number.", "Error");
                    return;
                }

                switch (statName)
                {
                    case "Max": BattlePokemonB.MaxHP = Convert.ToDouble(stat.Text);
                        break;
                    case "Att": BattlePokemonB.ATT = Convert.ToDouble(stat.Text);
                        break;
                    case "Def": BattlePokemonB.DEF = Convert.ToDouble(stat.Text);
                        break;
                    case "SpA": BattlePokemonB.SPA = Convert.ToDouble(stat.Text);
                        break;
                    case "SpD": BattlePokemonB.SPD = Convert.ToDouble(stat.Text);
                        break;
                    case "Spe": BattlePokemonB.SPE = Convert.ToDouble(stat.Text);
                        break;
                }
            }

            UpdateStatDisplay();
            UpdateHealthDisplay();
            this.ActiveControl = PokemonHeading;
        }

        protected void ManualHPEntry(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                e.Handled = true;
            else
                return;

            string box = ((TextBox)sender).Name;
            string boxValue = ((TextBox)sender).Text;

            if (box[box.Length - 1] == 'A')
                if (BattlePokemonA == null)
                {
                    MessageBox.Show("There is no Pokemon selected.", "Error");
                    return;
                }
            if (box[box.Length - 1] == 'B')
                if (BattlePokemonB == null)
                {
                    MessageBox.Show("There is no Pokemon selected.", "Error");
                    return;
                }

            if (box == "HPResultA")
            {
                if (!Regex.IsMatch(boxValue, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole number.", "Error");
                    return;
                }

                BattlePokemonA.SetHealth(Convert.ToDouble(boxValue));
            }
            if (box == "PercentResultA")
            {
                if (boxValue[boxValue.Length - 1] == '%')
                    boxValue = boxValue.Substring(0, boxValue.Length - 1);

                if (!Regex.IsMatch(boxValue, @"^[0-9.%]+$"))
                {
                    MessageBox.Show("Entry should be a whole number or decimal.", "Error");
                    return;
                }

                BattlePokemonA.SetPercentage(Convert.ToDouble(boxValue));
            }
            if (box == "SubHealthA")
            {
                if (!Regex.IsMatch(boxValue, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole number.", "Error");
                    return;
                }

                BattlePokemonA.SetSubHP(Convert.ToDouble(boxValue));
            }
            if (box == "HPResultB")
            {
                if (!Regex.IsMatch(boxValue, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole number.", "Error");
                    return;
                }

                BattlePokemonB.SetHealth(Convert.ToDouble(boxValue));
            }
            if (box == "PercentResultB")
            {
                if (boxValue[boxValue.Length - 1] == '%')
                    boxValue = boxValue.Substring(0, boxValue.Length - 1);

                if (!Regex.IsMatch(boxValue, @"^[0-9.%]+$"))
                {
                    MessageBox.Show("Entry should be a whole number or decimal.", "Error");
                    return;
                }

                BattlePokemonB.SetPercentage(Convert.ToDouble(boxValue));
            }
            if (box == "SubHealthB")
            {
                if (!Regex.IsMatch(boxValue, @"^[0-9]+$"))
                {
                    MessageBox.Show("Entry should be a whole number.", "Error");
                    return;
                }

                BattlePokemonB.SetSubHP(Convert.ToDouble(boxValue));
            }

            UpdateHealthDisplay();
            this.ActiveControl = PokemonHeading;
        }

        protected void SubButton(object sender, EventArgs e)
        {
            char Attacker = ((Button)sender).Name[((Button)sender).Name.Length - 1];

            if (Attacker == 'A' && BattlePokemonA != null)
                if (BattlePokemonA.Sub)
                    MessageBox.Show("That Pokemon already has a Substitute.", "Error");
                else
                    BattlePokemonA.CreateSub();
            if (Attacker == 'B' && BattlePokemonB != null)
                if (BattlePokemonB.Sub)
                    MessageBox.Show("That Pokemon already has a Substitute.", "Error");
                else
                    BattlePokemonB.CreateSub();

            UpdateHealthDisplay();
        }

        protected void Sixteenths(object sender, EventArgs e)
        {
            PictureBox button = (PictureBox)sender;

            char first, last;
            string amount;

            first = button.Name[0];
            last = button.Name[button.Name.Length - 1];

            amount = button.Name.Substring(button.Name.Length - 4, 3);

            if (last == 'A')
            {
                if (BattlePokemonA == null)
                {
                    MessageBox.Show("There is no Pokemon entered.", "Error");
                    return;
                }

                if (first == 'P')
                {
                    if (amount == "625")
                        BattlePokemonA.ChangeHealth(6.25);
                    else
                        BattlePokemonA.ChangeHealth(12.5);
                }
                if (first == 'M')
                {
                    if (amount == "625")
                        BattlePokemonA.ChangeHealth(-6.25);
                    else
                        BattlePokemonA.ChangeHealth(-12.5);
                }
            }
            else if (last == 'B')
            {
                if (BattlePokemonB == null)
                {
                    MessageBox.Show("There is no Pokemon entered.", "Error");
                    return;
                }

                if (first == 'P')
                {
                    if (amount == "625")
                        BattlePokemonB.ChangeHealth(6.25);
                    else
                        BattlePokemonB.ChangeHealth(12.5);
                }
                if (first == 'M')
                {
                    if (amount == "625")
                        BattlePokemonB.ChangeHealth(-6.25);
                    else
                        BattlePokemonB.ChangeHealth(-12.5);
                }
            }

            UpdateHealthDisplay();
        }

        #endregion

        #region ComboBox OnChange Functions

        private void PokemonAChanged(object sender, EventArgs e)
        {
            if (PokemonA.SelectedItem == null || PokemonA.SelectedIndex == -1)
            {
                BattlePokemonA = null;
                return;
            }

            int i = 0;

            if (ChangeFlag == true)
                BattlePokemonA = new BattlePokemon((Pokemon)PokemonA.SelectedItem);

            PAttackLabel1.Text = BattlePokemonA.Name;
            PAttackLabel3.Text = BattlePokemonA.Name;
            DoAttackB.Text = "<-- Attack " + BattlePokemonA.Name;
            foreach (Object T in TypeA1.Items)
            {
                if (BattlePokemonA.Type1 == T.Code)
                {
                    TypeA1.SelectedIndex = i;
                }
                else i++;
            }
            i = 0;
            foreach (Object T in TypeA2.Items)
            {
                if (BattlePokemonA.Type2 == T.Code)
                {
                    TypeA2.SelectedIndex = i;
                }
                else i++;
            }

            MaxHPA.Text = BattlePokemonA.MaxHP.ToString();
            AttackA.Text = BattlePokemonA.ATT.ToString();
            DefenceA.Text = BattlePokemonA.DEF.ToString();
            SpAttackA.Text = BattlePokemonA.SPA.ToString();
            SpDefenceA.Text = BattlePokemonA.SPD.ToString();
            SpeedA.Text = BattlePokemonA.SPE.ToString();

            if (ChangeFlag == true)
            {
                HPResultA.Text = BattlePokemonA.MaxHP.ToString();
                PercentResultA.Text = BattlePokemonA.Percentage.ToString() + "%";
                SubHealthA.Text = "None";

                AbilityA.SelectedIndex = 0;
                BattlePokemonA.Ability = "NO";
                GenderA.SelectedIndex = 0;
                BattlePokemonA.Gender = "NO";
                HItemA.SelectedIndex = 0;
                BattlePokemonA.Item = "(none)";
                StatusA.SelectedIndex = 0;
                BattlePokemonA.Status = "NO";
            }

            InitializeModBoxesA();
        }

        private void PokemonBChanged(object sender, EventArgs e)
        {
            if (PokemonB.SelectedItem == null)
            {
                BattlePokemonB = null;
                return;
            }

            int i = 0;

            if(ChangeFlag == true)
                BattlePokemonB = new BattlePokemon((Pokemon)PokemonB.SelectedItem);

            PAttackLabel2.Text = BattlePokemonB.Name;
            PAttackLabel4.Text = BattlePokemonB.Name;
            DoAttackA.Text = "Attack " + BattlePokemonB.Name + " -->";
            foreach (Object T in TypeB1.Items)
            {
                if (BattlePokemonB.Type1 == T.Code)
                {
                    TypeB1.SelectedIndex = i;
                }
                else i++;
            }
            i = 0;
            foreach (Object T in TypeB2.Items)
            {
                if (BattlePokemonB.Type2 == T.Code)
                {
                    TypeB2.SelectedIndex = i;
                }
                else i++;
            }

            MaxHPB.Text = BattlePokemonB.MaxHP.ToString();
            AttackB.Text = BattlePokemonB.ATT.ToString();
            DefenceB.Text = BattlePokemonB.DEF.ToString();
            SpAttackB.Text = BattlePokemonB.SPA.ToString();
            SpDefenceB.Text = BattlePokemonB.SPD.ToString();
            SpeedB.Text = BattlePokemonB.SPE.ToString();

            if (ChangeFlag == true)
            {
                HPResultB.Text = BattlePokemonB.MaxHP.ToString();
                PercentResultB.Text = BattlePokemonB.Percentage.ToString() + "%";
                SubHealthB.Text = "None";

                AbilityB.SelectedIndex = 0;
                BattlePokemonB.Ability = "NO";
                GenderB.SelectedIndex = 0;
                BattlePokemonB.Gender = "NO";
                HItemB.SelectedIndex = 0;
                BattlePokemonB.Item = "(none)";
                StatusB.SelectedIndex = 0;
                BattlePokemonB.Status = "NO";
            }

            InitializeModBoxesB();
        }

        private void BattlePokemonAChanged()
        {
            ChangeFlag = false;

            foreach(Pokemon P in PokemonList)
            {
                if(P.Name == BattlePokemonA.Name)
                    PokemonA.SelectedItem = P;
            }

            foreach (Object A in Abilities)
            {
                if (A.Code == BattlePokemonA.Ability)
                    AbilityA.SelectedItem = A;
            }

            foreach (Object S in Statuses)
            {
                if (S.Code == BattlePokemonA.Status)
                    StatusA.SelectedItem = S;
            }

            HItemA.SelectedItem = BattlePokemonA.Item;

            UpdateHealthDisplay();

            ChangeFlag = true;
        }

        private void BattlePokemonBChanged()
        {
            ChangeFlag = false;

            foreach (Pokemon P in PokemonList)
            {
                if (P.Name == BattlePokemonB.Name)
                    PokemonB.SelectedItem = P;
            }

            foreach (Object A in Abilities)
            {

                if (A.Code == BattlePokemonB.Ability)
                    AbilityB.SelectedItem = A;
            }

            foreach (Object S in Statuses)
            {
                if (S.Code == BattlePokemonB.Status)
                    StatusB.SelectedItem = S;
            }

            HItemB.SelectedItem = BattlePokemonB.Item;

            UpdateHealthDisplay();

            ChangeFlag = true;
        }

        private void ModChanged(object sender, EventArgs e)
        {
            ComboBox modBox = (ComboBox)sender;
            double modifier = ((StatMod)modBox.SelectedItem).Modifier;
            char Attacker = modBox.Name[modBox.Name.Length - 1];
            string modName = modBox.Name.Substring(0, 3);

            if (Attacker == 'A' && BattlePokemonA != null)
            {
                switch (modName)
                {
                    case "Att": BattlePokemonA.ATTmod = modifier;
                        break;
                    case "Def": BattlePokemonA.DEFmod = modifier;
                        break;
                    case "Spa": BattlePokemonA.SPAmod = modifier;
                        break;
                    case "Spd": BattlePokemonA.SPDmod = modifier;
                        break;
                    case "Spe": BattlePokemonA.SPEmod = modifier;
                        break;
                }
            }
            if (Attacker == 'B' && BattlePokemonB != null)
            {
                switch (modName)
                {
                    case "Att": BattlePokemonB.ATTmod = modifier;
                        break;
                    case "Def": BattlePokemonB.DEFmod = modifier;
                        break;
                    case "Spa": BattlePokemonB.SPAmod = modifier;
                        break;
                    case "Spd": BattlePokemonB.SPDmod = modifier;
                        break;
                    case "Spe": BattlePokemonB.SPEmod = modifier;
                        break;
                }
            }

            UpdateStatDisplay();
        }

        private void StatusChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string Name = box.Name;

            if (Name == "StatusA" && BattlePokemonA != null)
                BattlePokemonA.Status = ((Object)StatusA.SelectedItem).Code;
            if (Name == "StatusB" && BattlePokemonB != null)
                BattlePokemonB.Status = ((Object)StatusB.SelectedItem).Code;

            CalculateSpeed();
        }

        private void AbilityChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string Name = box.Name;

            if (Name == "AbilityA" && BattlePokemonA != null)
                BattlePokemonA.Ability = ((Object)AbilityA.SelectedItem).Code;
            if (Name == "AbilityB" && BattlePokemonB != null)
                BattlePokemonB.Ability = ((Object)AbilityB.SelectedItem).Code;

            CalculateSpeed();
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string Name = box.Name;

            if (Name == "HItemA" && BattlePokemonA != null)
                BattlePokemonA.Item = box.Text;
            if (Name == "HItemB" && BattlePokemonB != null)
                BattlePokemonB.Item = box.Text;

            CalculateSpeed();
        }

        private void WeatherChanged(object sender, EventArgs e)
        {
            CalculateSpeed();
        }

        private void TypeChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            string BoxName = box.Name;

            if (BattlePokemonA != null)
            {
                if (BoxName == "TypeA1")
                    BattlePokemonA.Type1 = ((Object)box.SelectedItem).Code;
                if (BoxName == "TypeA2")
                    BattlePokemonA.Type2 = ((Object)box.SelectedItem).Code;
            }
            if (BattlePokemonB != null)
            {
                if (BoxName == "TypeB1")
                    BattlePokemonB.Type1 = ((Object)box.SelectedItem).Code;
                if (BoxName == "TypeB2")
                    BattlePokemonB.Type2 = ((Object)box.SelectedItem).Code;
            }
        }

        private void CalculateSpeed()
        {
            double modA = 1.0, modB = 1.0;
            string Weather = "NO";

            if (WeatherBox.SelectedItem != null)
                Weather = ((Object)WeatherBox.SelectedItem).Code;

            if (BattlePokemonA != null)
            {
                if (BattlePokemonA.Status == "PAR" && BattlePokemonA.Ability != "QF")
                    modA *= 0.50;

                if (BattlePokemonB == null || (BattlePokemonB.Ability != "CN" && BattlePokemonB.Ability != "AI"))
                {
                    if (BattlePokemonA.Ability == "CL" && (Weather == "S" || Weather == "HS"))
                        modA *= 2.0;
                    if (BattlePokemonA.Ability == "SWS" && (Weather == "R" || Weather == "HR"))
                        modA *= 2.0;
                    if (BattlePokemonA.Ability == "SLR" && Weather == "H")
                        modA *= 2.0;
                    if (BattlePokemonA.Ability == "SRH" && Weather == "SS")
                        modA *= 2.0;
                }

                if (BattlePokemonA.Ability == "QF" && BattlePokemonA.Status != "NO")
                    modA = 1.5;

                if (BattlePokemonA.Item == "Choice Scarf" && BattlePokemonA.Ability != "KL")
                    modA *= 1.5;

                if (BattlePokemonA.Item == "Iron Ball" || BattlePokemonA.Item == "Macho Brace" ||
                    BattlePokemonA.Item == "Power Anklet" || BattlePokemonA.Item == "Power Band" ||
                    BattlePokemonA.Item == "Power Belt" || BattlePokemonA.Item == "Power Brace" ||
                    BattlePokemonA.Item == "Power Lens" || BattlePokemonA.Item == "Power Weight")
                    modA = .5;

                SpeedA.Text = Math.Floor(BattlePokemonA.SPE * BattlePokemonA.SPEmod * modA).ToString();
            }
            if (BattlePokemonB != null)
            {
                if (BattlePokemonB.Status == "PAR" && BattlePokemonB.Ability != "QF")
                    modB *= 0.50;

                if (BattlePokemonA == null || (BattlePokemonA.Ability != "CN" && BattlePokemonA.Ability != "AI"))
                {
                    if (BattlePokemonB.Ability == "CL" && (Weather == "S" || Weather == "HS"))
                        modB *= 2.0;
                    if (BattlePokemonB.Ability == "SWS" && (Weather == "R" || Weather == "HR"))
                        modB *= 2.0;
                    if (BattlePokemonB.Ability == "SLR" && Weather == "H")
                        modB *= 2.0;
                    if (BattlePokemonB.Ability == "SRH" && Weather == "SS")
                        modB *= 2.0;
                }

                if (BattlePokemonB.Ability == "QF" && BattlePokemonB.Status != "NO")
                    modB = 1.5;

                if (BattlePokemonB.Item == "Choice Scarf" && BattlePokemonB.Ability != "KL")
                    modB *= 1.5;

                if (BattlePokemonB.Item == "Iron Ball" || BattlePokemonB.Item == "Macho Brace" ||
                    BattlePokemonB.Item == "Power Anklet" || BattlePokemonB.Item == "Power Band" ||
                    BattlePokemonB.Item == "Power Belt" || BattlePokemonB.Item == "Power Brace" ||
                    BattlePokemonB.Item == "Power Lens" || BattlePokemonB.Item == "Power Weight")
                    modB *= .5;

                SpeedB.Text = Math.Floor(BattlePokemonB.SPE * BattlePokemonB.SPEmod * modB).ToString();
            }
        }

        #endregion

        #region THE DAMAGE CALCULATION
        protected void CalcDamage(object sender, EventArgs e)
        {
            double STAB = 1.0;  //Same Type Attack Bonus
            double TE = 1.0;    //Type Effectiveness

            BattlePokemon Attacker, Defender;

            //Ensure two Pokemon are selected before calculation
            if (BattlePokemonA == null || BattlePokemonB == null)
            {
                MessageBox.Show("You must selected both Pokemon before calculating attacks.", "Error");
                return;
            }

            char last = ((Button)sender).Name[((Button)sender).Name.Length - 1];

            //Check that all attack information is included
            if (!ValidAttack(last))
                return;

            if (last == 'A')
            {
                Attacker = AssignAttacker('A');
                Defender = AssignDefender('B');
            }
            else
            {
                Attacker = AssignAttacker('B');
                Defender = AssignDefender('A');
            }

            #region Modifier Calculations
            //Base Power Modifiers - Items and Abilities that modify Base Power
            double BP1 = CalcAttackerItem(Attacker);
            double BP2 = CalcAttackerAbility(Attacker, Defender);
            double BP3 = CalcDefenderAbility(Attacker, Defender);

            Attacker.BasePower = Math.Floor(Attacker.BasePower * BP1 * BP2 * BP3);

            //Calculate Attack/Defence Stats
            CalculateStats(Attacker, Defender);

            //Damage Fomula Modifiers
            double ModOne = CalcModOne(Attacker, Defender);
            double ModTwo = CalcModTwo(Attacker);
            double ModThree = CalcModThree(Attacker, Defender);

            //Check Stab
            STAB = CheckStab(Attacker);
            //Check Type Effectiveness
            TE = CheckTypes(Attacker, Defender);
            #endregion

            #region THE DAMAGE FORMULA
            double Damage = 0.0;

            //Check for Wonder Room and swap first
            if (OtherBox.Text == "Wonder Room")
            {
                double temp = Defender.DEF;
                Defender.DEF = Defender.SPD;
                Defender.SPD = temp;
            }

            switch (Attacker.AttackClass)
            {
                case "PHYS":
                    Damage = Math.Floor(42 * Attacker.BasePower * Math.Floor(Attacker.ATT * Attacker.ATTmod) / 50);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage / Math.Floor(Defender.DEF * Defender.DEFmod));
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage * ModOne);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor((Damage + 2.0) * ModTwo);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage * 92.5);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage / 100);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage * STAB);
                    Console.WriteLine(Damage);
                    Damage = Math.Floor(Damage * TE);
                    Console.WriteLine(Damage);
                    Damage = -Math.Floor(Damage * ModThree);
                    Console.WriteLine(Damage);
                    break;
                case "SPEC":
                    Damage = Math.Floor(42 * Attacker.BasePower * Math.Floor(Attacker.SPA * Attacker.SPAmod) / 50);
                    Damage = Math.Floor(Damage / Math.Floor(Defender.SPD * Defender.SPDmod));
                    Damage = Math.Floor(Damage * ModOne);
                    Damage = Math.Floor((Damage + 2.0) * ModTwo);
                    Damage = Math.Floor(Damage * 92.5);
                    Damage = Math.Floor(Damage / 100);
                    Damage = Math.Floor(Damage * STAB);
                    Damage = Math.Floor(Damage * TE);
                    Damage = -Math.Floor(Damage * ModThree);
                    break;
                case "SAvD":
                    Damage = Math.Floor(42 * Attacker.BasePower * Math.Floor(Attacker.SPA * Attacker.SPAmod) / 50);
                    Damage = Math.Floor(Damage / Math.Floor(Defender.DEF * Defender.DEFmod));
                    Damage = Math.Floor(Damage * ModOne);
                    Damage = Math.Floor((Damage + 2.0)* ModTwo);
                    Damage = Math.Floor(Damage * 92.5);
                    Damage = Math.Floor(Damage / 100);
                    Damage = Math.Floor(Damage * STAB);
                    Damage = Math.Floor(Damage * TE);
                    Damage = -Math.Floor(Damage * ModThree);
                    break;
            }

            #endregion

            #region Abilities that modify or replace damage post-calculation

            //Fur Coat
            if (Defender.Ability == "FU" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TV" && Attacker.AttackClass == "PHYS")
                Damage = Math.Floor(Damage / 2.0);

            //Dry Skin heal on Water moves
            if (Defender.Ability == "DS" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "W")
            {
                Damage = Math.Floor(Defender.MaxHP / 4.0);
                MessageBox.Show("Dry Skin activated.");
            }

            //Volt Absorb heal
            if (Defender.Ability == "VA" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "E")
            {
                if (BattlePokemonB.Type1 == "GD" || BattlePokemonB.Type2 == "GD")
                    Damage = 0;
                else
                {
                    Damage = Math.Floor(Defender.MaxHP / 4.0);
                    MessageBox.Show("Volt Absorb activated.");
                }
            }

            //Water Absorb heal
            if (Defender.Ability == "WA" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "W")
            {
                Damage = Math.Floor(Defender.MaxHP / 4.0);
                MessageBox.Show("Water Absorb activated.");
            }

            //Water Bubble reduction
            if (Defender.Ability == "WB" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "FR")
                Damage = Math.Floor(Damage / 2);

            //Flash Fire negation
            if (Defender.Ability == "FF" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "FR")
            {
                Damage = 0;
                MessageBox.Show("Flash Fire activated.");
                if (Defender.Which == 'A') AbilityA.SelectedIndex = 33;
                else AbilityB.SelectedIndex = 33;
            }

            //Flash Fire On negation
            if (Defender.Ability == "FFO" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "FR")
            {
                Damage = 0;
                MessageBox.Show("Flash Fire prevents damage.");
            }

            //Sap Sipper negation
            if (Defender.Ability == "SPS" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "GR")
            {
                Damage = 0;
                MessageBox.Show("Sap Sipper activated.");
                if (Defender.Which == 'A') AttModA.SelectedIndex -= 1;
                else AttModB.SelectedIndex -= 1;
            }

            //Storm Drain negation
            if (Defender.Ability == "STD" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "W")
            {
                Damage = 0;
                MessageBox.Show("Storm Drain activated.");
                if (Defender.Which == 'A') SpaModA.SelectedIndex -= 1;
                else SpaModB.SelectedIndex -= 1;
            }

            //Lightningrod negation
            if (Defender.Ability == "LI" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "E")
            {
                if (Defender.Type1 != "GD" && Defender.Type2 != "GD")
                {
                    if (Defender.Which == 'A') SpaModA.SelectedIndex -= 1;
                    else SpaModB.SelectedIndex -= 1;
                }

                Damage = 0;
                MessageBox.Show("Lightningrod activated.");
            }

            //Levitate negation
            if (Defender.Ability == "LE" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && Attacker.AttackType.Code == "GD")
            {
                Damage = 0;
                MessageBox.Show("Levitate prevents damage.");
            }

            //Wonder Guard negation
            if (Defender.Ability == "WG" && Attacker.Ability != "MB" && Attacker.Ability != "TV" && Attacker.Ability != "TU" && TE < 2.0)
            {
                Damage = 0;
                MessageBox.Show("Wonder Guard prevents damage.");
            }
            #endregion

            if (Attacker.Which == 'A') ResultA.Text = Damage.ToString();
            else ResultB.Text = Damage.ToString();

            if (Defender.Which == 'A') BattlePokemonA.TakeDamage(Damage);
            else BattlePokemonB.TakeDamage(Damage);

            UpdateHealthDisplay();
            UpdateStatDisplay();

            if(TE > 1.0 && Defender.Item == "Weakness Policy")
            {
                MessageBox.Show("Weakness Policy activates");
                if (Defender.Which == 'A')
                {
                    AttModA.SelectedIndex -= 2;
                    SpaModA.SelectedIndex -= 2;
                    HItemA.SelectedIndex = 0;
                }
                else
                {
                    AttModB.SelectedIndex -= 2;
                    SpaModB.SelectedIndex -= 2;
                    HItemB.SelectedIndex = 0;
                }
            }
        }

        protected void ConfuseDamage(object sender, EventArgs e)
        {
            char Attacker = ((Button)sender).Name[((Button)sender).Name.Length - 1];

            double damage = 0.0;

            if (Attacker == 'A' && BattlePokemonA != null)
            {
                double Attack = BattlePokemonA.ATT * BattlePokemonA.ATTmod;
                double Defence = BattlePokemonA.DEF * BattlePokemonA.DEFmod;

                damage = Math.Floor(42.0 * Attack * 40.0 / Defence);
                damage = Math.Floor(damage * 1.0 / 50.0) + 2.0;
                damage = Math.Floor(damage * (1.0 * 1.0 * (((236.0 / 255.0) * 100.0) / 100.0)));
                damage *= -1.0;

                BattlePokemonA.SmackSelf(damage);
                ResultA.Text = damage.ToString();
            }
            if (Attacker == 'B' && BattlePokemonB != null)
            {
                double Attack = BattlePokemonB.ATT * BattlePokemonB.ATTmod;
                double Defence = BattlePokemonB.DEF * BattlePokemonB.DEFmod;

                damage = Math.Floor(42.0 * Attack * 40.0 / Defence);
                damage = Math.Floor(damage * 1.0 / 50.0) + 2.0;
                damage = Math.Floor(damage * (1.0 * 1.0 * (((236.0 / 255.0) * 100.0) / 100.0)));
                damage *= -1.0;

                BattlePokemonB.SmackSelf(damage);
                ResultB.Text = damage.ToString();
            }

            UpdateHealthDisplay();
        }
        #endregion

        #region Damage Calc preparation
        protected bool ValidAttack(char Attacker)
        {
            if (Attacker == 'A')
            {
                string message = "";
                if (AttackTypeA.Text == "")
                    message += "Please select an attack type.\n";

                if (BasePowerA.Text == "")
                    message += "Please enter a Base Power for the attack.\n";
                else if (!Regex.IsMatch(BasePowerA.Text, @"^[0-9]+$"))
                    message += "Base Power must be a positive integer.\n";

                if (AttackClassA.Text == "")
                    message += "Please select an attack class.\n";

                if (message != "")
                {
                    MessageBox.Show(message, "Error");
                    return false;
                }
            }
            if (Attacker == 'B')
            {
                string message = "";
                if (AttackTypeB.Text == "")
                    message += "Please select an attack type.\n";

                if (BasePowerB.Text == "")
                    message += "Please enter a Base Power for the attack.\n";
                else if (!Regex.IsMatch(BasePowerB.Text, @"^[0-9]+$"))
                    message += "Base Power must be a positive integer.\n";

                if (AttackClassB.Text == "")
                    message += "Please select an attack class.\n";

                if (message != "")
                {
                    MessageBox.Show(message, "Error");
                    return false;
                }
            }

            return true;
        }

        protected BattlePokemon AssignAttacker(char which)
        {
            BattlePokemon temp;

            switch (which)
            {
                case 'A':
                    temp = new BattlePokemon(BattlePokemonA);
                    temp.Which = 'A';
                    temp.Gender = ((Object)GenderA.SelectedItem).Code;
                    temp.Ability = ((Object)AbilityA.SelectedItem).Code;
                    temp.Status = ((Object)StatusA.SelectedItem).Code;
                    temp.Effect = ((Object)EffectA.SelectedItem).Code;
                    temp.Item = HItemA.Text;

                    temp.BasePower = Convert.ToDouble(BasePowerA.Text);
                    temp.AttackClass = ((Object)AttackClassA.SelectedItem).Code;
                    temp.AttackType = (TypeObject)AttackTypeA.SelectedItem;
                    break;
                case 'B':
                    temp = new BattlePokemon(BattlePokemonB);
                    temp.Which = 'B';
                    temp.Gender = ((Object)GenderB.SelectedItem).Code;
                    temp.Ability = ((Object)AbilityB.SelectedItem).Code;
                    temp.Status = ((Object)StatusB.SelectedItem).Code;
                    temp.Effect = ((Object)EffectB.SelectedItem).Code;
                    temp.Item = HItemB.Text;

                    temp.BasePower = Convert.ToDouble(BasePowerB.Text);
                    temp.AttackClass = ((Object)AttackClassB.SelectedItem).Code;
                    temp.AttackType = (TypeObject)AttackTypeB.SelectedItem;
                    break;
                default:
                    temp = null;
                    break;
            }

            return temp;
        }

        protected BattlePokemon AssignDefender(char which)
        {
            BattlePokemon temp;

            switch (which)
            {
                case 'A':
                    temp = new BattlePokemon(BattlePokemonA);
                    temp.Which = 'A';
                    temp.Gender = ((Object)GenderA.SelectedItem).Code;
                    temp.Ability = ((Object)AbilityA.SelectedItem).Code;
                    temp.Status = ((Object)StatusA.SelectedItem).Code;
                    temp.Effect = ((Object)EffectA.SelectedItem).Code;
                    temp.Item = HItemA.Text;
                    break;
                case 'B':
                    temp = new BattlePokemon(BattlePokemonB);
                    temp.Which = 'B';
                    temp.Gender = ((Object)GenderB.SelectedItem).Code;
                    temp.Ability = ((Object)AbilityB.SelectedItem).Code;
                    temp.Status = ((Object)StatusB.SelectedItem).Code;
                    temp.Effect = ((Object)EffectB.SelectedItem).Code;
                    temp.Item = HItemB.Text;
                    break;
                default:
                    temp = null;
                    break;
            }

            return temp;
        }
        #endregion

        #region Stab and Type Effectiveness functions
        protected double CheckStab(BattlePokemon A)
        {
            if (A.Ability == "NZ") A.AttackType = Types[(int)TypeCode.NM];
            if (A.Ability == "AE" && A.AttackType.Code == "NM") A.AttackType = Types[(int)TypeCode.FL];
            if (A.Ability == "PX" && A.AttackType.Code == "NM") A.AttackType = Types[(int)TypeCode.FA];
            if (A.Ability == "RF" && A.AttackType.Code == "NM") A.AttackType = Types[(int)TypeCode.I];
            if (A.Ability == "GV" && A.AttackType.Code == "NM") A.AttackType = Types[(int)TypeCode.E];


            if (A.Type1 == A.AttackType.Code || A.Type2 == A.AttackType.Code)
            {
                if (A.Ability == "AD")
                    return 2.0;
                else return 1.5;
            }

            return 1.0;
        }

        protected double CheckTypes(BattlePokemon A, BattlePokemon D)
        {
            //A = Attacker, D = Defender
            double result = 1.0;

            if(A.AttackType.Compatibility.SE.Contains(D.Type1))
                result *= 2.0;
            if(A.AttackType.Compatibility.SE.Contains(D.Type2))
                result *= 2.0;

            if(A.AttackType.Compatibility.NVE.Contains(D.Type1))
                result *= 0.5;
            if(A.AttackType.Compatibility.NVE.Contains(D.Type2))
                result *= 0.5;

            if (A.AttackType.Compatibility.I.Contains(D.Type1))
                result *= 0.0;
            if (A.AttackType.Compatibility.I.Contains(D.Type2))
                result *= 0.0;

            //Mysterious air current override - assumes the above would have already x2.0
            if(((Object)WeatherBox.SelectedItem).Code == "MAC")
            {
                if (A.AttackType.Compatibility.SE.Contains(D.Type1) && D.Type1 == "FL")
                    result *= 0.5;
                if (A.AttackType.Compatibility.SE.Contains(D.Type2) && D.Type2 == "FL")
                    result *= 0.5;
            }

            return result;
        }
        #endregion

        #region BasePower Modifier Checks

        protected double CalcAttackerItem(BattlePokemon A)
        {
            double mod = 1.0;

            if (A.Ability == "KL")
                return 1.0;

            //Phys/Spec boosters
            if (A.Item == "Muscle Band" && A.AttackClass == "PHYS") mod = 1.1;
            if (A.Item == "Wise Glasses" && (A.AttackClass == "SPEC" || A.AttackClass == "SAvD")) mod = 1.1;
            //Type Boosters
            if ((A.Item == "SilverPowder" || A.Item == "Insect Plate") && A.AttackType.Code == "B") mod = 1.2;
            if ((A.Item == "BlackGlasses" || A.Item == "Dread Plate") && A.AttackType.Code == "DK") mod = 1.2;
            if ((A.Item == "Dragon Fang" || A.Item == "Draco Plate") && A.AttackType.Code == "DR") mod = 1.2;
            if ((A.Item == "Magnet" || A.Item == "Zap Plate") && A.AttackType.Code == "E") mod = 1.2;
            if ((A.Item == "Black Belt" || A.Item == "Fist Plate") && A.AttackType.Code == "FI") mod = 1.2;
            if ((A.Item == "Charcoal" || A.Item == "Flame Plate") && A.AttackType.Code == "FR") mod = 1.2;
            if ((A.Item == "Sharp Beak" || A.Item == "Sky Plate") && A.AttackType.Code == "FL") mod = 1.2;
            if ((A.Item == "Spell Tag" || A.Item == "Spooky Plate") && A.AttackType.Code == "GH") mod = 1.2;
            if ((A.Item == "Miracle Seed" || A.Item == "Rose Incense" || A.Item == "Meadow Plate") && A.AttackType.Code == "GR") mod = 1.2;
            if ((A.Item == "Soft Sand" || A.Item == "Earth Plate") && A.AttackType.Code == "GD") mod = 1.2;
            if ((A.Item == "NeverMeltIce" || A.Item == "Icicle Plate") && A.AttackType.Code == "A.Item") mod = 1.2;
            if ((A.Item == "Silk Scarf") && A.AttackType.Code == "NM") mod = 1.2;
            if ((A.Item == "Poison Barb" || A.Item == "Toxic Plate") && A.AttackType.Code == "PO") mod = 1.2;
            if ((A.Item == "TwistedSpoon" || A.Item == "Odd Incense" || A.Item == "Mind Plate") && A.AttackType.Code == "PS") mod = 1.2;
            if ((A.Item == "Hard Stone" || A.Item == "Rock Incense" || A.Item == "Stone Plate") && A.AttackType.Code == "R") mod = 1.2;
            if ((A.Item == "Metal Coat" || A.Item == "Iron Plate") && A.AttackType.Code == "S") mod = 1.2;
            if ((A.Item == "Mystic Water" || A.Item == "Sea Incense" || A.Item == "Wave Incense" || A.Item == "Splash Plate") && A.AttackType.Code == "W") mod = 1.2;
            if (A.Item == "Pixie Plate" && A.AttackType.Code == "FA") mod = 1.2;

            //Type Gems
            if (A.Item.Substring(A.Item.Length - 3, 3) == "Gem")
            {
                if (A.Item == "Bug Gem" && A.AttackType.Code == "B") mod = 1.3;
                if (A.Item == "Dark Gem" && A.AttackType.Code == "DK") mod = 1.3;
                if (A.Item == "Dragon Gem" && A.AttackType.Code == "DR") mod = 1.3;
                if (A.Item == "Electric Gem" && A.AttackType.Code == "E") mod = 1.3;
                if (A.Item == "Fighting Gem" && A.AttackType.Code == "FI") mod = 1.3;
                if (A.Item == "Fire Gem" && A.AttackType.Code == "FR") mod = 1.3;
                if (A.Item == "Flying Gem" && A.AttackType.Code == "FL") mod = 1.3;
                if (A.Item == "Ghost Gem" && A.AttackType.Code == "GH") mod = 1.3;
                if (A.Item == "Grass Gem" && A.AttackType.Code == "GR") mod = 1.3;
                if (A.Item == "Ground Gem" && A.AttackType.Code == "GD") mod = 1.3;
                if (A.Item == "Ice Gem" && A.AttackType.Code == "I") mod = 1.3;
                if (A.Item == "Normal Gem" && A.AttackType.Code == "NM") mod = 1.3;
                if (A.Item == "Poison Gem" && A.AttackType.Code == "PO") mod = 1.3;
                if (A.Item == "Psychic Gem" && A.AttackType.Code == "PS") mod = 1.3;
                if (A.Item == "Rock Gem" && A.AttackType.Code == "R") mod = 1.3;
                if (A.Item == "Steel Gem" && A.AttackType.Code == "S") mod = 1.3;
                if (A.Item == "Water Gem" && A.AttackType.Code == "W") mod = 1.3;

                if (mod == 1.3)
                {
                    MessageBox.Show("The gem was used.");
                    if (A.Which == 'A') HItemA.SelectedIndex = 0;
                    else HItemB.SelectedIndex = 0;
                }
            }

            return mod;
        }

        protected double CalcAttackerAbility(BattlePokemon A, BattlePokemon D)
        {
            double mod = 1.0;

            string Weather = ((Object)WeatherBox.SelectedItem).Code;

            //Activate Crits if Merciless/Poison
            if(A.Ability == "ME" && (D.Status == "PSN" || D.Status == "TOX"))
            {
                if (A.Which == 'A') CritA.Text = "Yes";
                if (A.Which == 'B') CritB.Text = "Yes";
            }

            if (A.Ability == "RI" && A.Gender == D.Gender) mod = 1.25;
            if (A.Ability == "RI" && A.Gender != D.Gender) mod = .75;
            if (A.Ability == "RI" && (A.Gender == "NO" || D.Gender == "NO")) mod = 1.0;
            if (A.Ability == "BL" && A.Percentage < 33.33 && A.AttackType.Code == "FR") mod = 1.5;
            if (A.Ability == "OG" && A.Percentage < 33.33 && A.AttackType.Code == "GR") mod = 1.5;
            if (A.Ability == "TO" && A.Percentage < 33.33 && A.AttackType.Code == "W") mod = 1.5;
            if (A.Ability == "SWA" && A.Percentage < 33.33 && A.AttackType.Code == "B") mod = 1.5;
            if (A.Ability == "TE" && A.BasePower <= 60.0) mod = 1.5;
            if (A.Ability == "SF" && Weather == "SS" && (A.AttackType.Code == "GD" || A.AttackType.Code == "R" || A.AttackType.Code == "S")) mod = 1.3;
            if (A.Ability == "WB" && A.AttackType.Code == "W") mod = 2.0;
            if (A.Ability == "STW" && A.AttackType.Code == "S") mod = 1.5;
            if (A.Ability == "DM" && A.AttackType.Code == "DR") mod = 1.5;
            if (A.Ability == "TS" && A.AttackType.Code == "E") mod = 1.5;

            if (A.Ability == "DK" && A.AttackType.Code == "DK") mod = (D.Ability != "AU") ? (4.0 / 3.0) : (2.0 / 3.0);
            if (A.Ability == "FA" && A.AttackType.Code == "FA") mod = (D.Ability != "AU") ? (4.0 / 3.0) : (2.0 / 3.0);

            if (A.Ability == "NZ" && A.AttackType.Code != "NM") mod = 1.2;
            if ((A.Ability == "AE" || A.Ability == "PX" || A.Ability == "RF" || A.Ability == "GV") && A.AttackType.Code == "NM") mod = 1.2;

            return mod;
        }

        protected double CalcDefenderAbility(BattlePokemon A, BattlePokemon D)
        {
            double mod = 1.0;

            if (A.Ability == "MB" || A.Ability == "TV" || A.Ability == "TU")
                return 1.0;

            if (D.Ability == "TH" && (A.AttackType.Code == "FR" || A.AttackType.Code == "I")) mod = 0.5;
            if (D.Ability == "HE" && A.AttackType.Code == "FR") mod = 0.5;
            if (D.Ability == "DS" && A.AttackType.Code == "FR") mod = 1.25;

            return mod;
        }

        #endregion

        #region Damage Formula Modifiers
        protected void CalculateStats(BattlePokemon A, BattlePokemon D)
        {
            //This function determines what modifier all stats should by multiplied by
            //Then does the multiplication

            string Weather = ((Object)WeatherBox.SelectedItem).Code;

            #region Abilities that effect the Stat Modifiers
            //Unaware
            if (A.Ability == "UB")
            {
                D.DEFmod = 1.0;
                D.SPDmod = 1.0;
            }
            if (D.Ability == "UB" && A.Ability != "MB" && A.Ability != "TV" && A.Ability != "TU")
            {
                A.ATTmod = 1.0;
                A.SPAmod = 1.0;
            }
            #endregion

            #region Crit Modifier
            if ((CritA.Text == "Yes" && A.Which == 'A') || (CritB.Text == "Yes" && A.Which == 'B'))
            {
                if (A.ATTmod < 1.0) A.ATTmod = 1.0;
                if (A.SPAmod < 1.0) A.SPAmod = 1.0;
                if (D.DEFmod > 1.0) D.DEFmod = 1.0;
                if (D.SPDmod > 1.0) D.SPDmod = 1.0;
            }
            #endregion

            #region Abilities that modify the Attack stat
            //Huge Power or Pure Power
            if (A.Ability == "HP" || A.Ability == "PU") A.ATTmod *= 2.0;
            //Guts
            if (A.Ability == "GU" && A.Status != "NO") A.ATTmod *= 1.5;
            //Toxic Boost
            if (A.Ability == "TB" && (A.Status == "PSN" || A.Status == "TOX")) A.ATTmod *= 1.5;
            //Flower Gift
            if (A.Ability == "FG" && (Weather == "S" || Weather == "HS") && D.Ability != "CN" && D.Ability != "AI") A.ATTmod *= 1.5;
            //Hustle
            if (A.Ability == "HU" || A.Ability == "GT") A.ATTmod *= 1.5;
            //Defeatist
            if (A.Ability == "DE" && A.Percentage < 50.0) A.ATTmod *= 0.5;
            #endregion

            #region Abilities that modify the Special Attack stat
            if (A.Ability == "SP" && (Weather == "S" || Weather == "HS") && D.Ability != "CN" && D.Ability != "AI")
                A.SPAmod *= 1.5;
            if (A.Ability == "FT" && A.Status == "BRN")
                A.SPAmod *= 1.5;
            if (A.Ability == "DE" && A.Percentage < 50.0)
                A.SPAmod *= 0.5;
            #endregion

            #region Items that modify the Attack stat
            if (A.Ability != "KL")
            {
                if (A.Item == "Choice Band")
                    A.ATTmod *= 1.5;
                if (A.Item == "Light Ball" && A.Name == "Pikachu")
                    A.ATTmod *= 2.0;
                if (A.Item == "Thick Club" && (A.Name == "Cubone" || A.Name == "Marowak" || A.Name == "Marowak-Alolan"))
                    A.ATTmod *= 2.0;
            }
            #endregion

            #region Items that modify the Special Attack stat
            if (A.Ability != "KL")
            {
                if (A.Item == "Choice Specs")
                    A.SPAmod *= 1.5;
                if (A.Item == "Light Ball" && A.Name == "Pikachu")
                    A.SPAmod *= 2;
                if (A.Item == "DeepSeaTooth" && A.Name == "Clamperl")
                    A.SPAmod *= 2;
            }
            #endregion

            #region Other modifiers
            //Marvel Scale modifies Defence
            if (D.Ability == "MS" && D.Status != "NO")
                D.DEFmod *= 1.5;
            //Sandstorm modifies Special Defence
            if (Weather == "SS" && (A.Ability != "CN" || D.Ability != "CN") &&
               (A.Ability != "AI" || D.Ability != "AI") &&
               (D.Type1 == "R" || D.Type2 == "R"))
                D.SPDmod *= 1.5;
            //Flower Gift modifies Special Defence
            if (D.Ability == "FG" && A.Ability != "MB" && A.Ability != "TV" && A.Ability != "TU"
                && ((Weather == "S" || Weather == "HS") && A.Ability != "CN" && A.Ability != "AI"))
                D.SPDmod *= 1.5;
            //DeepSeaScale modifies Special Defence
            if (D.Item == "DeepSeaScale" && D.Ability != "KL" && D.Name == "Clamperl")
                D.SPDmod *= 2.0;
            //Assault Vest modifies Special Defence
            if (D.Item == "Assault Vest" && D.Ability != "KL")
                D.SPDmod *= 1.5;
            //Eviolite
            if (D.Item == "Eviolite" && D.Ability != "KL")
            {
                D.DEFmod *= 1.5;
                D.SPDmod *= 1.5;
            }
            #endregion
        }

        protected double CalcModOne(BattlePokemon A, BattlePokemon D)
        {
            double mod = 1.0;

            string Weather = ((Object)WeatherBox.SelectedItem).Code;

            if (A.Status == "BRN" && A.AttackClass == "PHYS" && A.Ability != "GU")    //Burn Reduction
                mod *= 0.5;
            if (D.Effect == "R" && A.AttackClass == "PHYS" && A.Ability != "II" && CritA.Text != "Yes")   //Reflect
                mod *= 0.5;
            if (D.Effect == "LS" && A.AttackClass == "SPEC" && A.Ability != "II" && CritA.Text != "Yes")  //Lightscreen
                mod *= 0.5;
            //Sunny Weather Fire/Water Mod
            if ((Weather == "S" || Weather == "HS") && (A.Ability != "CN" || D.Ability != "CN") &&
               (A.Ability != "AI" && A.Ability != "AI") && A.AttackType.Code == "FR")
                mod *= 1.5;
            if ((Weather == "S" || Weather == "HS") && (A.Ability != "CN" || D.Ability != "CN") &&
               (A.Ability != "AI" && A.Ability != "AI") && A.AttackType.Code == "W")
                mod *= 0.5;
            //Rainy Weather
            if ((Weather == "R" || Weather == "HR") && (A.Ability != "CN" || D.Ability != "CN") &&
               (A.Ability != "AI" && A.Ability != "AI") && A.AttackType.Code == "W")
                mod *= 1.5;
            if ((Weather == "R" || Weather == "HR") && (A.Ability != "CN" || D.Ability != "CN") &&
               (A.Ability != "AI" && A.Ability != "AI") && A.AttackType.Code == "FR")
                mod *= 0.5;

            //Harsh sun
            if (Weather == "HS" && A.AttackType.Code == "W")
                mod *= 0;
            if (Weather == "HS" && A.AttackType.Code == "FR")
                mod *= 1.5;
            //Heavy rain
            if (Weather == "HR" && A.AttackType.Code == "FR")
                mod *= 0;
            if (Weather == "HR" && A.AttackType.Code == "W")
                mod *= 1.5;

            if (A.Ability == "FFO" && A.AttackType.Code == "FR")  //Flash Fire Boost
                mod *= 1.5;

            if (D.Ability == "MT" && D.Percentage == 100.0)   //Multiscale
                mod *= 0.5;
            if (D.Ability == "SSH" && (D.CurrentHP == D.MaxHP))
                mod *= 0.5;

            return mod;
        }

        protected double CalcModTwo(BattlePokemon A)
        {
            double mod = 1.0;

            if (A.Item == "Life Orb" && A.Ability != "KL")
                mod *= 1.3;

            if (A.Effect == "MF")
            {
                mod *= 1.5;
                EffectA.SelectedIndex = 0;
            }

            return mod;
        }

        protected double CalcModThree(BattlePokemon A, BattlePokemon D)
        {
            double mod = 1.0;

            double TE = CheckTypes(A, D);

            // Tinted Lens
            if (A.Ability == "TL" && TE < 1.0)
                mod *= 2.0;

            //Solid Rock/Filter
            if ((D.Ability == "SR" || D.Ability == "FI") && A.Ability != "MB" && A.Ability != "TV" && A.Ability != "TU" && TE > 1.0)
                mod *= 0.75;
            if (D.Ability == "PA" && TE > 1.0)
                mod *= 0.75;

            if (A.Item == "Expert Belt" && TE > 1.0)
                mod *= 1.2;

            //Type-reducing berries
            bool Berry = false;
            if (A.Ability != "UV" && D.Ability != "KL" && !D.Sub)
            {
                if (D.Item == "Tanga Berry" && A.AttackType.Code == "B" && TE > 1.0) Berry = true;
                if (D.Item == "Colbur Berry" && A.AttackType.Code == "DK" && TE > 1.0) Berry = true;
                if (D.Item == "Haban Berry" && A.AttackType.Code == "DR" && TE > 1.0) Berry = true;
                if (D.Item == "Wacan Berry" && A.AttackType.Code == "E" && TE > 1.0) Berry = true;
                if (D.Item == "Roseli Berry" && A.AttackType.Code == "FA" && TE > 1.0) Berry = true;
                if (D.Item == "Chople Berry" && A.AttackType.Code == "FI" && TE > 1.0) Berry = true;
                if (D.Item == "Occa Berry" && A.AttackType.Code == "FR" && TE > 1.0) Berry = true;
                if (D.Item == "Coba Berry" && A.AttackType.Code == "FL" && TE > 1.0) Berry = true;
                if (D.Item == "Kasib Berry" && A.AttackType.Code == "GH" && TE > 1.0) Berry = true;
                if (D.Item == "Rindo Berry" && A.AttackType.Code == "GR" && TE > 1.0) Berry = true;
                if (D.Item == "Shuca Berry" && A.AttackType.Code == "GD" && TE > 1.0) Berry = true;
                if (D.Item == "Yache Berry" && A.AttackType.Code == "I" && TE > 1.0) Berry = true;
                if (D.Item == "Chilan Berry" && A.AttackType.Code == "NM") Berry = true;
                if (D.Item == "Kebia Berry" && A.AttackType.Code == "PO" && TE > 1.0) Berry = true;
                if (D.Item == "Payapa Berry" && A.AttackType.Code == "PS" && TE > 1.0) Berry = true;
                if (D.Item == "Charti Berry" && A.AttackType.Code == "R" && TE > 1.0) Berry = true;
                if (D.Item == "Babiri Berry" && A.AttackType.Code == "S" && TE > 1.0) Berry = true;
                if (D.Item == "Passho Berry" && A.AttackType.Code == "W" && TE > 1.0) Berry = true;

                if (Berry)
                {
                    mod *= 0.5;
                    MessageBox.Show("The berry was used.");
                    if (D.Which == 'A')
                        HItemA.SelectedIndex = 0;
                    else
                        HItemB.SelectedIndex = 0;
                }
            }

            if ((CritA.Text == "Yes" && A.Which == 'A') || (CritB.Text == "Yes" && A.Which == 'B'))
            {
                if (A.Ability == "SN")
                    mod *= 2.25;
                else
                    mod *= 1.5;

                if (A.Which == 'A') CritA.SelectedIndex = 1; else CritB.SelectedIndex = 1;

            }

            return mod;
        }
        #endregion

        #region Menu Functions

        private void ClearCalc(object sender, EventArgs e)
        {
            PokemonA.SelectedItem = null;
            PokemonB.SelectedItem = null;

            MaxHPA.Text = "";
            AttackA.Text = "";
            DefenceA.Text = "";
            SpAttackA.Text = "";
            SpDefenceA.Text = "";
            SpeedA.Text = "";
            HPResultA.Text = "";
            PercentResultA.Text = "";
            SubHealthA.Text = "";
            MaxHPB.Text = "";
            AttackB.Text = "";
            DefenceB.Text = "";
            SpAttackB.Text = "";
            SpDefenceB.Text = "";
            SpeedB.Text = "";
            HPResultB.Text = "";
            PercentResultB.Text = "";
            SubHealthB.Text = "";

            PAttackLabel1.Text = "Pokemon A:";
            PAttackLabel3.Text = "Pokemon A:";
            PAttackLabel2.Text = "Pokemon B:";
            PAttackLabel4.Text = "Pokemon B:";

            DoAttackA.Text = "Attack -->";
            DoAttackB.Text = "<-- Attack";

            InitializeComboBoxes();
            InitializeModBoxesA();
            InitializeModBoxesB();

            AttackClassA.SelectedIndex = 0;
            AttackClassB.SelectedIndex = 0;
            AttackTypeA.SelectedIndex = 0;
            AttackTypeB.SelectedIndex = 0;

            BasePowerA.Text = "";
            BasePowerB.Text = "";
            AccResultBox.Text = "";
            ManualA.Text = "";
            ManualB.Text = "";
        }

        private void ClearStorage(object sender, EventArgs e)
        {
            StoredPokemon = new BattlePokemon[12];

            for(int i = 0; i < 12; i++)
            {
                StorageBoxes[i].Text = "";
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            var Version = Assembly.GetEntryAssembly().GetName().Version;
            var VersionString = Version.Major + "." + Version.Minor + "." + Version.Build;
            string Message = "URPG Desktop Calculator "+VersionString+"\n";
            Message += "Written by Monbrey\n";
            Message += "Based on the HTML/Javascript Calculator by Pikachu\n";
            Message += "Credit also to DaRkUmBrEoN, Monbrey, Espeon and WebMaster\n";

            MessageBox.Show(Message, "About:");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Accuracy Function

        protected void CalcAcc(object sender, EventArgs e)
        {
            int ACCtop = 0;
            int ACCbottom = 0;

            int EVAtop = 0;
            int EVAbottom = 0;

            int ACC = Convert.ToInt32(AccMod.Text);
            int EVA = Convert.ToInt32(EvaMod.Text);

            if (ACC < 0)
            {
                ACCtop = 3;
                ACCbottom = 3 - ACC;
            }
            else
            {
                ACCtop = 3 + ACC;
                ACCbottom = 3;
            }

            if (EVA < 0)
            {
                EVAtop = 3 - EVA;
                EVAbottom = 3;
            }
            else
            {
                EVAtop = 3;
                EVAbottom = 3 + EVA;
            }

            int resultTop = ACCtop * EVAtop;
            int resultBottom = ACCbottom * EVAbottom;

            Console.WriteLine(resultTop + " " + resultBottom);

            int a = resultTop, b = resultBottom;

            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }

            if (a != resultTop && a != resultBottom)
            {
                resultTop = resultTop / a;
                resultBottom = resultBottom / a;
            }

            AccResultBox.Text = resultTop + "\\" + resultBottom;
        }
        #endregion

        #region Other Compatibility Functions

        private void StealFocus(object sender, EventArgs e)
        {
            this.ActiveControl = PokemonHeading;
        }

        private void ComboBoxTypedIn(object sender, KeyPressEventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            if (box.DroppedDown)
                box.DroppedDown = false;
        }

        protected void Leaver(object sender, EventArgs e)
        {
            KeyPressEventArgs key = new KeyPressEventArgs((char)Keys.Return);

            TextBox which = (TextBox)sender;

            if (which.Name.Contains("Result") || which.Name.Contains("Sub"))
                ManualHPEntry(sender, key);
            else
                ManualStatEntry(sender, key);
        }
        #endregion

        #region Team Storage Functions
        protected void StorePokemon(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            char side = button.Name[button.Name.Length - 1];
            string slot = button.Name[button.Name.Length - 2].ToString();
            int index = Convert.ToInt32(slot) - 1;

            if (side == 'A')
            {
                if (BattlePokemonA == null)
                {
                    MessageBox.Show("There is no Pokemon selected on side A to store.", "Error");
                    return;
                }

                if(BattlePokemonA.isDynamaxed) BattlePokemonA.ToggleDynamax(DynamaxA);
                StoredPokemon[index] = BattlePokemonA;
            }
            if (side == 'B')
            {
                if (BattlePokemonB == null)
                {
                    MessageBox.Show("There is no Pokemon selected on side B to store.", "Error");
                    return;
                }

                if (BattlePokemonB.isDynamaxed) BattlePokemonB.ToggleDynamax(DynamaxB);
                StoredPokemon[index+6] = BattlePokemonB;
            }

            UpdateTeamDisplay();
        }

        protected void LoadPokemon(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            char side = button.Name[button.Name.Length - 1];
            string slot = button.Name[button.Name.Length - 2].ToString();
            int index = Convert.ToInt32(slot) - 1;

            if (side == 'A')
            {
                if(StoredPokemon[index] == null)
                {
                    MessageBox.Show("There is no Pokemon stored here to load.", "Error");
                    return;
                }

                StoredPokemon[index].ResetMods();
                StoredPokemon[index].SetSubHP(0);
                BattlePokemonA = StoredPokemon[index];

                BattlePokemonAChanged();
            }
            if (side == 'B')
            {
                if(StoredPokemon[index+6] == null)
                {
                    MessageBox.Show("There is no Pokemon stored here to load.", "Error");
                    return;
                }

                StoredPokemon[index+6].ResetMods();
                StoredPokemon[index+6].SetSubHP(0);
                BattlePokemonB = StoredPokemon[index+6];

                BattlePokemonBChanged();
            }

            TabController.SelectedIndex = 0;
        }
        #endregion

        private void TabChanged(object sender, TabControlEventArgs e)
        {
            if (((TabControl)sender).SelectedTab == TeamStorageTab)
                UpdateTeamDisplay();
        }
    }
}
