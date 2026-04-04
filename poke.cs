using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel.Design;
using System.Xml.Linq;
public enum Status
{
    None = 0,
    Burn = 1,
    Freeze = 2,
    Paralysis = 3,
    Poison = 4,
    Sleep = 5,
    Confusion = 6,
    Flinch = 7,     
    EscapePrevent = 8,
    Bound = 9,
}
public enum Split
{
    Physical = 1,
    Special = 2,
    Status = 3
}
public enum Stat
{
    None = 0,
    Atk = 1,
    Def = 2,
    Spa = 3,
    Spd = 4,
    Spe = 5,
    Acc = 6,
    Eva = 7
}
public enum Type
{
    None = 0,
    Normal = 1,
    Fire = 2,
    Water = 3,
    Electric = 4,
    Grass = 5,
    Fighting = 7,
    Poison = 8,
    Ground = 9,
    Flying = 10,
    Psychic = 11,
    Bug = 12,
    Rock = 13,
}
public class Species
{
    public string name { get; }
    public Type type1 { get; }
    public Type type2 { get; }
    public int Hp { get; }
    public int Atk { get; }
    public int Def { get; }
    public int Spa { get; }
    public int Spe { get; }
    public int ratio { get; }
    public int catchRate { get; }
    public int expYield { get; }
    public bool expGroup { get; }
    public int[] evYield { get; }
    public List<(int level, int moveId)> learnSet { get; set; } = new();
    public Species(string name, Type type1, Type type2, int Hp, int Atk, int Def, int Spa, int Spe, int ratio, int catchRate, int expYield, bool expGroup, int[] evYield, List<(int level, int moveId)> learnSet)
    {
        this.name = name;
        this.type1 = type1;
        this.type2 = type2;
        this.Hp = Hp;
        this.Atk = Atk;
        this.Def = Def;
        this.Spa = Spa;
        this.Spe = Spe;
        this.ratio = ratio;
        this.catchRate = catchRate;
        this.expYield = expYield;
        this.expGroup = expGroup;
        this.evYield = evYield;
        this.learnSet = learnSet;
    }
}
public class Pokemon
{
    public Species species { get; set; }
    public string name { get; set; }
    public bool gender { get; }
    public int level { get; private set; }
    public int exp { get; set; }
    public int maxHP { get; private set; }
    public int hp { get; set; }
    public Status statusNonVol { get; set; } = Status.None;
    public List<Status> statusVol { get; set; } = new List<Status>();
    public int HpIV, HpEV, AtkIV, AtkEV, DefIV, DefEV, SpaIV, SpaEV, SpeIV, SpeEV;
    public int AtkMod, DefMod, SpaMod, SpeMod, AccMod, EvaMod;
    public int confusionTimer { get; set; } = 0;
    public int sleepTimer { get; set; } = 0;
    public int boundCounter { get; set; } = 0;
    public int critRatio { get; set; } = 24;
    public bool chargingMove { get; set; } = false;
    public bool moveFirst { get; set; } = false;
    public Move lastMove { get; set; } = null;
    public Move selectedMove { get; set; } = null;
    public Move[] moveSet = new Move[4];
    public int moveNum { get; private set; } = 0;
    public Pokemon(Species species, int level)
    {
        this.species = species;
        this.name = species.name;
        int gender = Random.Shared.Next(1, 101);
        bool g = false;
        if (gender <= species.ratio)
        {
            g = true;
        }
        this.gender = g;
        this.level = level;
        exp = this.ExpToLevel(0);
        this.HpIV = Random.Shared.Next(0, 15);
        this.HpEV = 0;
        this.AtkIV = Random.Shared.Next(0, 15);
        this.AtkEV = 0;
        this.DefIV = Random.Shared.Next(0, 15);
        this.DefEV = 0;
        this.SpaIV = Random.Shared.Next(0, 15);
        this.SpaEV = 0;
        this.SpeIV = Random.Shared.Next(0, 15);
        this.SpeEV = 0;
        this.AtkMod = 0;
        this.DefMod = 0;
        this.SpaMod = 0;
        this.SpeMod = 0;
        this.AccMod = 0;
        this.EvaMod = 0;
        this.maxHP = CalcHp();
        this.hp = maxHP;
    }
    public void AddMove(Move move)
    {
        if (move == null || move.moveB == null) return;
        if (moveNum < 4)
        {
            moveSet[moveNum] = move;
            moveNum++;
        }
    }
    public void ClearMods()
    {
        AtkMod = 0;
        DefMod = 0;
        SpaMod = 0;
        SpeMod = 0;
        AccMod = 0;
        EvaMod = 0;
    }
    public void Heal()
    {
        hp = maxHP;
        HealConditions();
        ClearMods();
        foreach (Move m in moveSet)
        {
            if (m != null)
            {
                m.PP = m.moveB.maxPP;
            }
        }
        lastMove = null;
        chargingMove = false;
    }
    public void HealConditions()
    {
        statusNonVol = 0;
        statusVol.Clear();
        confusionTimer = 0;
    }
    public void PokeInfo()
    {
        if (name != species.name)
        {
            Console.Write(name);
            Console.Write($"({species.name})");
        }
        else
        {
            Console.Write(species.name);
        }   
        if (gender == true) Console.Write(" (M) ");
        else Console.Write(" (F) ");
        Console.WriteLine();
        Console.WriteLine($"Hp: {hp}/{maxHP}");
        Console.WriteLine($"Level: {level} Exp:{+ exp} Exp to next level: {ExpToLevel(1) - exp}");

        var evParts = new List<string>();
        if (HpEV != 0) evParts.Add($"{HpEV} HP");
        if (AtkEV != 0) evParts.Add($"{AtkEV} Atk");
        if (DefEV != 0) evParts.Add($"{DefEV} Def");
        if (SpaEV != 0) evParts.Add($"{SpaEV} Spa");
        if (SpeEV != 0) evParts.Add($"{SpeEV} Spe");

        if (evParts.Count > 0)
        {
            Console.WriteLine($"EVs: {string.Join(" / ", evParts)}");
        }


        var ivParts = new List<string>();
        if (HpIV != 0) ivParts.Add($"{HpIV} HP");
        if (AtkIV != 0) ivParts.Add($"{AtkIV} Atk");
        if (DefIV != 0) ivParts.Add($"{DefIV} Def");
        if (SpaIV != 0) ivParts.Add($"{SpaIV} Spa");
        if (SpeIV != 0) ivParts.Add($"{SpeIV} Spe");

        if (ivParts.Count > 0)
        {
            Console.WriteLine($"IVs: {string.Join(" / ", ivParts)}");
        }

        for (int i = 0; i < moveSet.Length; i++)
        {
            if (moveSet[i] == null)
            {
                break;
            }
            else
            {
                Console.WriteLine($"-{moveSet[i].moveB.name} PP: {moveSet[i].PP}/{moveSet[i].moveB.maxPP}");
            }
        }
        Console.WriteLine();
    }
    public double GetMod(int value)
    {
        return value switch
        {
            -6 => 0.25,
            -5 => 0.28,
            -4 => 0.33,
            -3 => 0.4,
            -2 => 0.5,
            -1 => 0.67,
            0 => 1.0,
            1 => 1.5,
            2 => 2.0,
            3 => 2.5,
            4 => 3.0,
            5 => 3.5,
            6 => 4.0,
            _ => 0.0
        };
    }
    public int CalcStat(int i)
    {
        int hp = Convert.ToInt32(Math.Floor((((i + HpIV) * 2 + Math.Floor((double)Math.Sqrt(HpEV) / 4)) * level) / 100) + level + 5);
        return hp;
    }
    public int CalcHp()
    {
        int hp = Convert.ToInt32(Math.Floor(((2 * species.Hp + HpIV + Math.Floor((double)HpEV / 4)) * level) / 100) + level + 10);
        return hp;
    }
    public Move PickMove()
    {
        int i = 0;
        foreach (Move m in moveSet)
        {
            if (m != null)
            {
                i++;
                Console.WriteLine($"[{i}] {m.moveB.name} PP: {m.PP}");
            }
        }
        Console.WriteLine("Pick a move");
        int move = Convert.ToInt32(Console.ReadLine());
        return moveSet[move - 1];
    }
    public bool DoIMove()
    {
        if (statusNonVol == Status.Sleep)
        {
            if (sleepTimer > 0)
            {
                sleepTimer--;
                Console.WriteLine($"{name} is asleep and can't move!");
                return false;
            }
            else
            {
                statusNonVol = Status.None;
                Console.WriteLine($"{name} woke up!");
                return true;
            }
        }
        else if (statusNonVol == Status.Freeze)
        {
            int thaw = Random.Shared.Next(0, 5);
            if (thaw == 0)
            {
                statusNonVol = Status.None;
                Console.WriteLine($"{name} thawed out!");
                return true;
            }
            else
            {
                Console.WriteLine($"{name} is frozen solid and can't move!");
                return false;
            }
        }
        else if (statusNonVol == Status.Paralysis)
        {
            int chance = Random.Shared.Next(0, 4);
            if (chance == 0)
            {
                Console.WriteLine($"{name} is paralyzed and can't move!");
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            foreach (Status statusN in statusVol)
            {
                switch (statusN)
                {
                    case Status.Confusion:
                        if (confusionTimer > 0)
                        {
                            confusionTimer--;
                            int check = Random.Shared.Next(0, 3);
                            if (check == 0)
                            {
                                Console.WriteLine($"{name} is confused and hurt itself in its confusion!");
                                int damage = Convert.ToInt32(Math.Floor((double)(((((2 * level) / 5) + 2) * 40 * CalcStat(species.Atk)) / CalcStat(species.Def)) / 50) + 2);
                                hp -= damage;
                                if (hp < 0) hp = 0;
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            statusVol.Remove(Status.Confusion);
                            Console.WriteLine($"{name} snapped out of its confusion!");
                            return true;
                        }
                    default:
                        return true;
                }
            }
            return true;
        }
    }
    public bool IsImmune(Status st)
    {
        switch (st)
        {
            case Status.Burn:
                if (species.type1 == Type.Fire || species.type2 == Type.Fire) return true;
                return false;
            case Status.Paralysis:
                if (species.type1 == Type.Electric || species.type2 == Type.Electric) return true;
                return false;
            case Status.Poison:
                if (species.type1 == Type.Poison || species.type2 == Type.Poison) return true;
                return false;
            default:
                return false;
        }
    }
    public int ExpToLevel(int i)
    {
        if(species.expGroup)
        {
            return Convert.ToInt32(Math.Max(Math.Floor((Math.Pow((level + i), 3) * 6/5) - (15 * Math.Pow((level + i), 2)) + (100 * (level + i)) - 140), 0));
        }
        else
        {
            return Convert.ToInt32(Math.Max(Math.Floor(Math.Pow((level + 1), 3)), 0));
        }
    }
    public void CheckLevelUp()
    {
        while (exp >= ExpToLevel(0))
        {
            level++;
            Console.WriteLine($"{name} grew to level {level}!");
            Console.ReadLine();
            Species oldSpecies = species;
            if (level == 16 && (species == Program.AllPokemon[0] || species == Program.AllPokemon[3] || species == Program.AllPokemon[6] || species == Program.AllPokemon[26] || species == Program.AllPokemon[24])) 
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            else if (level == 36 && (species == Program.AllPokemon[1] || species == Program.AllPokemon[4] || species == Program.AllPokemon[7] || species == Program.AllPokemon[16]))
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            else if (level == 7 && (species == Program.AllPokemon[9] || species == Program.AllPokemon[12]))
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            else if (level == 10 && (species == Program.AllPokemon[10] || species == Program.AllPokemon[13]))
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            else if (level == 18 && (species == Program.AllPokemon[15]))
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            else if (level == 20 && (species == Program.AllPokemon[18] || species == Program.AllPokemon[20]))
            {
                species = Program.AllPokemon[Array.IndexOf(Program.AllPokemon, species) + 1];
            }
            if (oldSpecies != species)
            {
                Console.WriteLine("Oh?");
                Console.ReadLine();
                Console.WriteLine($"{name} evolved into {species.name}!");
                Console.ReadLine();
            }
            maxHP = CalcHp();
            foreach (var move in species.learnSet)
            {
                if (move.level == level)
                {
                    AddMove(new Move(Program.AllMoves[move.moveId]));
                    Console.WriteLine($"{name} learned {Program.AllMoves[move.moveId].name}!");
                }
            }
        }
    }
}
public class MoveB
{
    public string name { get; }
    public Type type { get; set; }
    public int power { get; set; }
    public int acc { get; }
    public Split split { get; }
    public int maxPP { get; }
    public List<MoveEffect> effectList { get; private set; }
    public MoveB(string name, Type type, int power, int acc, Split split, int maxPP, List<MoveEffect> effectList)
    {
        this.name = name;
        this.type = type;
        this.power = power;
        this.acc = acc;
        this.split = split;
        this.maxPP = maxPP;
        this.effectList = effectList;
    }
}
public class MoveEffect
{
    public Status effectStatus { get; }
    public Stat effectStat { get; }
    public int effectChance { get; }
    public int effectPower { get; }
    public MoveEffect(Status effectStatus, Stat effectStat, int effectChance, int effectPower)
    {
        this.effectStatus = effectStatus;
        this.effectStat = effectStat;
        this.effectChance = effectChance;
        this.effectPower = effectPower;
    }
}
public class Move
{
    public MoveB moveB { get; private set; }
    public int PP { get; set; }
    public Move(MoveB moveB)
    {
        if (moveB == null) return;
        this.moveB = moveB;
        this.PP = moveB.maxPP;
    }
    public Move(MoveB moveB, int PP)
    {
        if (moveB == null) return;
        this.moveB = moveB;
        this.PP = PP;
    }
 
}
public class Item
{
    public string name { get; }
    public int count { get; set; } = 0;
    public Item(string name)
    {
        this.name = name;
    }
}
public class Trainer
{
    public string name { get; }
    int numPoke = 0;
    public List<Pokemon> team = new List<Pokemon>();
    public int wins { get; set; } = 0;
    public Trainer(string name)
    {
        this.name = name;
    }
    public void AddPokemon(Pokemon pokemon)
    {
        if (pokemon.moveNum == 0 || pokemon == null || pokemon.species == null) return;
        if (numPoke < 6)
        {
            team.Add(pokemon);
            numPoke++;
        }
    }
    public void RemovePokemon(Pokemon pokemon)
    {
        if (team.Contains(pokemon))
        {
            team.Remove(pokemon);
            numPoke--;
        }
    }
    public void HealTeam()
    {
        foreach (Pokemon p in team)
        {
            p.Heal();
        }
    }
    public Pokemon ShouldSwitch(Pokemon active)
    {
        double highestScore = 0;
        int availablePk = team.Count();
        List<Pokemon> availablePokemon = new List<Pokemon>();
        for (int i = 0; i < availablePk; i++)
        {
            if (team[i].hp > 0 && team[i] != active)
            {
                availablePokemon.Add(team[i]);
            }
        }
        int pkmnNum = 0;
        foreach (Pokemon pk in availablePokemon)
        {
            pkmnNum++;
            Console.WriteLine($"[{pkmnNum}] {pk.name}");
        }
        Console.WriteLine("Who to switch");
        int pkmn = Convert.ToInt32(Console.ReadLine());
        return availablePokemon[pkmn - 1];
    }
    public bool AbleToBattle()
    {
        bool able = false;
        foreach (Pokemon p in team)
        {
            if (p.hp > 0)
            {
                able = true; break;
            }
        }
        return able;
    }
    public bool LastInBattle()
    {
        bool last = true;
        bool check = false;
        foreach (Pokemon p in team)
        {
            if (p.hp > 0)
            {
                if (check) last = false;
                check = true;
                break;
            }
        }
        return last;
    }
}
public class Player : Trainer
{
    public int direction = 4;
    public int encounterImunity = 3;
    public int money = 0;
    public List<Item> bag = new List<Item> { 
            new Item("Potion"),
            new Item("Super Potion"),
            new Item("Ether"),
            new Item("Revive"),
            new Item("Repel"),
            new Item("Pokeball"),
            new Item("Antidote"),
            new Item("Paralyze Heal"),
            new Item("Awakening"),
            new Item("Burn Heal"),
            new Item("Rare candy"),
            new Item("Pokedex"),
        };
    public bool[] progressFlags = { false, false, false, false, false, false };
    public Port lastPokeCenter = Program.portPalletToAsh;
    public Player(string name) : base(name) {}
    public void Menu()
    {
        string[] options = { "Trainer", "Bag", "Pokemon" };
        int selectedIndex = 0;        
        while (true)
        {
            for (int i = 6; i < 18; i++) // clears previous menu lines
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(new string(' ', 40));
            }

            Console.SetCursorPosition(0, 6);
            for (int i = 0; i < options.Length; i++)
                Console.WriteLine($"{(i == selectedIndex ? ">" : " ")}  {options[i]}");

            var move = Console.ReadKey(true).Key;

            if (move == ConsoleKey.Z)
            {
                bool empty = false;
                for (int i = 6; i < 18; i++) // clears previous menu lines
                {
                    Console.SetCursorPosition(0, i);
                    Console.WriteLine(new string(' ', 40));
                }
                Console.SetCursorPosition(0, 6);
                switch (selectedIndex)
                {
                    case 0:
                        Console.WriteLine($"Name: {name}");
                        Console.WriteLine($"Money: ${money}");
                        break;
                    case 1:
                        {
                            foreach (var item in bag)
                            {
                                if (item.count > 0)
                                {
                                    Console.WriteLine($"{item.name}: {item.count}");
                                    empty = true;
                                }
                            }

                            if (!empty)
                                Console.WriteLine("(Empty bag)");
                            break;
                        }

                    case 2:
                        {
                            foreach (var pk in team)
                            {
                                if (pk != null)
                                {
                                    pk.PokeInfo();
                                    empty = true;
                                }
                            }

                            if (!empty)
                                Console.WriteLine("(No pokemon)");
                            break;
                        }
                }
                while (true)
                {
                    move = Console.ReadKey(true).Key;
                    if (move == ConsoleKey.X || move == ConsoleKey.D) break;
                }
            }
            else if (move == ConsoleKey.DownArrow)
                selectedIndex = (selectedIndex + 1) % options.Length;
            else if (move == ConsoleKey.UpArrow)
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
            else if (move == ConsoleKey.X || move == ConsoleKey.D)
            { 
                for (int i = 6; i < 18; i++) // clears previous menu lines
                {
                    Console.SetCursorPosition(0, i);
                    Console.WriteLine(new string(' ', 40));
                    Console.SetCursorPosition(0, 6);
                }
                return;
            }

        }
    }
    public void AddItem(string item, int count)
    {
        for(int i = 0; i < bag.Count; i++)
        {
            if (bag[i].name == item)
            {
                bag[i].count += count;
                return;
            }
        }
    }
}
public class Port
{
    public int mapID { get; }
    public int playerX { get; }
    public int playerY { get; }

    public Port(int mapID, int playerX, int playerY)
    {
        this.mapID = mapID;
        this.playerX = playerX;
        this.playerY = playerY;
    }
}

class Map
{
    private int mapID;
    private string[] map;
    private int playerX;
    private int playerY;
    private List<Port> ports;
    private List<(int pk, int[] lvlRange, int rate)> encounterTable;

    private int viewXSize = 5;
    private int viewYSize = 2;

    private string playerSprite = "()";
    private string npcSprite = "@@";
    private string chairSprite = "[]";
    private string wallSprite = "##";
    private string airSprite = "  ";
    private string nothingSprite = "xx";
    private string waterSprite = "ss";
    private string portSprite = "$$";
    private string grassSprite = "GG";
    private string signSprite = "TT";
    private string patchSprite = "++";
    private string ledgeSprite = "ww";
    private string treeSprite = "AA";
    private string branchSprite = "ff";
    private string machineSprite = "%%";
    private string compSprite = "PC";

    private bool foe1 = false;
    private bool foe2 = false;
    private bool foe3 = false;
    private bool foe4 = false;

    public Map(int mapID, string[] map, int playerX, int playerY, List<Port> ports, List<(int pk, int[] lvlRange, int rate)> encounterTable)
    {
        this.mapID = mapID;
        this.map = map;
        this.playerX = playerX;
        this.playerY = playerY;
        this.ports = ports;
        this.encounterTable = encounterTable;
    }

    public Map(int mapID, string[] map, List<Port> ports, List<(int pk, int[] lvlRange, int rate)> encounterTable)
    {
        this.mapID = mapID;
        this.map = map;
        this.ports = ports;
    }

    public void UpdateMap(Player playa, int oak)
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.ForegroundColor = ConsoleColor.Black;
        int oakOffsetX = 1;
        int oakOffsetY = 5;
        if(oak == 9)
        {
            oakOffsetX = 0;
            oakOffsetY = 10;
        }
        for (int viewY = 0; viewY < viewYSize * 2 + 1; viewY++)
        {
            string line = "";
            char[] ports = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int viewX = 0; viewX < viewXSize * 2 + 1; viewX++)
            {
                if (viewX == viewXSize && viewY == viewYSize)
                {
                    line += playerSprite;
                }
                else if (oak != -1 && viewX == viewXSize - oakOffsetX && viewY == viewYSize + oakOffsetY - oak)
                {
                    line += npcSprite;
                }
                else if (oak == 11 && viewX == viewXSize - 2 && viewY == viewYSize)
                {
                    line += npcSprite;
                }
                else
                {
                    int tileY = viewY + playerY - viewYSize;
                    int tileX = viewX + playerX - viewXSize;

                    if (tileX >= 0 && tileX < map[0].Length && tileY >= 0 && tileY < map.Length)
                    {
                        char tile = map[tileY][tileX];
                        if (tile == 'w') line += wallSprite;
                        else if (tile == 's') line += waterSprite;
                        else if (ports.Contains(tile)) line += portSprite;
                        else if (tile == 'c') line += chairSprite;
                        else if (tile == 'n' && (mapID != 4 || playa.progressFlags[0])) line += npcSprite;
                        else if (tile == 'p') line += patchSprite;
                        else if (tile == 'g') line += grassSprite;
                        else if (tile == 't') line += signSprite;
                        else if (tile == 'v') line += ledgeSprite;
                        else if (tile == 'a') line += treeSprite;
                        else if (tile == 'b') line += branchSprite;
                        else if (tile == 'P') line += compSprite;
                        else if (tile == 'm') line += machineSprite;
                        else if (tile == ' ') line += airSprite;
                    }
                    else
                    {
                        line += nothingSprite;
                    }
                }
            }
            Console.WriteLine(line);
        }
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        if(mapID == 9 && !playa.progressFlags[3])
        {
            PlayerRecieveParcel(playa);
        }
    }
    public static void Loading(bool pk)
    {
        int sizeX = 5;
        int sizeY = 2;
        int viewXSize = sizeX * 2 + 1;
        int viewYSize = sizeY * 2 + 1;
        int offset = 0;
        int delay = 400;
        Console.Clear();
        if (pk)
        {
            for (int viewY = 0; viewY < viewYSize; viewY++)
            {
                string line = "";
                int lineNum = viewY;
                if (viewY % 2 == 0)
                {
                    lineNum = viewY - viewY / 2;
                    offset++;
                    delay = 0;
                }
                else
                {
                    lineNum = viewYSize - offset;
                    delay = 400;
                }
                Console.SetCursorPosition(0, lineNum);
                for (int viewX = 0; viewX < viewXSize; viewX++)
                    line += "xx";
                Console.WriteLine(line);
                Thread.Sleep(delay);
            }
        }
        else
        {
            int center = viewYSize / 2;
            for (int viewY = 0; viewY < viewYSize; viewY++)
            {
                string line = "";
                for (int viewX = 0; viewX < viewXSize; viewX++)
                    line += "xx";
                Console.WriteLine(line);
            }
            Thread.Sleep(delay);
            for (int step = 0; step < viewYSize; step++)
            {
                int lineNum;

                if (step % 2 == 0)
                {
                    lineNum = center + offset;
                    delay = 0;
                }
                else
                {
                    lineNum = center - offset;
                    offset++;
                    delay = 400;
                }

                if (lineNum >= 0 && lineNum < viewYSize)
                {
                    Console.SetCursorPosition(0, lineNum);
                    Console.Write(new string(' ', viewXSize * 2)); // erase line
                }

                Thread.Sleep(delay);
            }
        }
    }
    public static void Loading()
    {
        Console.Clear();
        int sizeX = 5;
        int sizeY = 2;
        int viewXSize = sizeX * 2 + 1;
        int viewYSize = sizeY * 2 + 1;
        for (int viewY = 0; viewY < viewYSize; viewY++)
        {
            string line = "";
            for (int viewX = 0; viewX < viewXSize; viewX++)
                line += "xx";
            Console.WriteLine(line);
        }
    }
    public Map MovePlayer(int dir, Player playa)
    {
        int dirX = 0, dirY = 0;
        switch (dir)
        {
            case 1: dirX = 1; break;
            case 2: dirX = -1; break;
            case 3: dirY = 1; break;
            case 4: dirY = -1; break;
        }

        int newX = playerX + dirX;
        int newY = playerY + dirY;

        char[] passable = new char[] { ' ', 'c', 'p', 'g' };
        char tile = ' ';
        try
        {
            tile = map[newY][newX];
        }
        catch
        {
            Console.Clear();
            if (playa.progressFlags[playa.progressFlags.Length-1])
            {   
                Console.WriteLine("you won");
            }
            Console.ReadKey();
            Environment.Exit(0);
        }

        if (passable.Contains(tile))
        {      
            playerX = newX;
            playerY = newY;
            if(mapID == 16)
            {
                if((!foe1) && playerX == 31 && playerY == 33)
                {
                    Trainer foe = new Trainer("Bug Catcher");
                    Pokemon weedle = new Pokemon(Program.AllPokemon[12], 6);
                    weedle.AddMove(new Move(Program.AllMoves[11]));
                    weedle.AddMove(new Move(Program.AllMoves[28]));
                    Pokemon caterpie = new Pokemon(Program.AllPokemon[9], 6);
                    caterpie.AddMove(new Move(Program.AllMoves[9]));
                    caterpie.AddMove(new Move(Program.AllMoves[28]));
                    foe.AddPokemon(weedle);
                    foe.AddPokemon(caterpie);
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("Hey. you have pokemon right? Cmon, lets battle em!");
                    Console.ReadLine();
                    Program.Battle(playa, foe, false);
                    if (foe.AbleToBattle())
                    {
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("You recived 60$!");
                        playa.money += 60;
                        Console.ReadLine();
                        foe1 = true;
                    }
                }
                else if ((!foe2) && playerY == 20 && (playerX == 26 || playerX == 27 || playerX == 28 || playerX == 29 || playerX == 30))
                {
                    Trainer foe = new Trainer("Bug Catcher");
                    Pokemon weedle = new Pokemon(Program.AllPokemon[12], 7);
                    weedle.AddMove(new Move(Program.AllMoves[11]));
                    weedle.AddMove(new Move(Program.AllMoves[28]));
                    Pokemon kakuna = new Pokemon(Program.AllPokemon[13], 7);
                    kakuna.AddMove(new Move(Program.AllMoves[37]));
                    foe.AddPokemon(weedle);
                    foe.AddPokemon(kakuna);
                    foe.AddPokemon(weedle);
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("Yo, you cant jam out if youre a pokemon trainer!");
                    Console.ReadLine();
                    Program.Battle(playa, foe, false);
                    if (foe.AbleToBattle())
                    {
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("You recived 70$!");
                        playa.money += 70;
                        Console.ReadLine();
                        foe2 = true;
                    }
                }
                else if ((!foe3) && playerX == 2 && playerY == 19)
                {
                    Trainer foe = new Trainer("Bug Catcher");
                    Pokemon weedle = new Pokemon(Program.AllPokemon[12], 9);
                    weedle.AddMove(new Move(Program.AllMoves[11]));
                    weedle.AddMove(new Move(Program.AllMoves[28]));
                    foe.AddPokemon(weedle);
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("Yo, wait up! Whats the hurry?");
                    Console.ReadLine();
                    Program.Battle(playa, foe, false);
                    if (foe.AbleToBattle())
                    {
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("You recived 90$!");
                        playa.money += 90;
                        Console.ReadLine();
                        foe3 = true;
                    }                 
                }
            }
            else if(mapID == 21 && (!foe4) && playerY == 7 && (playerX == 5 || playerX == 6 || playerX == 7 || playerX == 8))
            {
                Trainer foe = new Trainer("Jr. Trainer");
                Pokemon diglett = new Pokemon(Program.AllPokemon[28], 11);
                diglett.AddMove(new Move(Program.AllMoves[0]));
                diglett.AddMove(new Move(Program.AllMoves[14]));
                Pokemon sandshrew = new Pokemon(Program.AllPokemon[23], 11);
                sandshrew.AddMove(new Move(Program.AllMoves[0]));
                sandshrew.AddMove(new Move(Program.AllMoves[6]));
                foe.AddPokemon(diglett);
                foe.AddPokemon(sandshrew);
                Console.SetCursorPosition(0, 5);
                Console.WriteLine("You are light-years away from facing Brock");
                Console.ReadLine();
                Program.Battle(playa, foe, false);
                if (foe.AbleToBattle())
                {
                    Console.SetCursorPosition(0, 5);
                    Console.WriteLine("You recived 220$!");
                    playa.money += 220;
                    Console.ReadLine();
                    foe4 = true;
                }
            }
            else if (tile == 'g')
            {
                if (mapID == 3 && (!playa.progressFlags[0] || !playa.progressFlags[1]))
                {
                    OakEncounter(playa);
                    playa.progressFlags[0] = true;
                }
                CheckEncounter(playa);
            }
        }
        if (!playa.AbleToBattle() && playa.progressFlags[1])
        {
            Console.Clear();
            Console.WriteLine("You blacked out...");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("You rushed to the nearest pokemon center");
            Console.ReadLine();
            playa.HealTeam();
            Program.mapList[playa.lastPokeCenter.mapID].Port(playa.lastPokeCenter);
            return Program.mapList[playa.lastPokeCenter.mapID];
        }
        if (tile == 'v' && dirY == 1)
        {
            if (!passable.Contains(map[newY + 1][newX])) return this;
            playerX = newX;
            playerY = newY + 1;
        }
        if (tile >= '0' && tile <= '9')
        {
            int index = tile - '0';
            if(index == 6 && !playa.progressFlags[4])
            {
                Console.SetCursorPosition(0, 5);
                Console.WriteLine("Try take a look around the town first");
                Console.ReadLine();
                Console.SetCursorPosition(0, 5);
                Console.WriteLine("                                       ");
                return this;
            }
            if (index < ports.Count())
            {
                var port = ports[index];
                Program.mapList[port.mapID].Port(port);
                return Program.mapList[port.mapID];
            }
        }
        return this;
    }
    public void Port(Port port)
    {
        Map.Loading();
        Thread.Sleep(500);
        playerX = port.playerX;
        playerY = port.playerY;
    }
    public void CheckEncounter(Player playa)
    {
        if (playa.encounterImunity < 1)
        {
            Random check = new Random();
            if (25 > check.Next(0, 256))
            {
                Encounter(playa);
                playa.encounterImunity = 3;
            }
        }
        else playa.encounterImunity--;
    }
    public void Encounter(Player playa)
    {
        if (encounterTable == null) return;
        Map.Loading(true);
        Console.Clear();
        Random pkmn = new Random();
        int roll = pkmn.Next(1, 101);
        int encounterRate = 0;
        foreach (var encounter in encounterTable)
        {
            encounterRate += encounter.rate;
            if (encounterRate >= roll)
            {
                Trainer wild = new Trainer("Wild " + Program.AllPokemon[encounter.pk].name);
                Pokemon wld = new Pokemon(Program.AllPokemon[encounter.pk], pkmn.Next(encounter.lvlRange[0], encounter.lvlRange[1] + 1));
                foreach (var move in wld.species.learnSet)
                {
                    if (move.level <= wld.level)
                    {
                        wld.AddMove(new Move(Program.AllMoves[move.moveId]));
                    }
                }
                wild.AddPokemon(wld);
                Program.Battle(playa, wild, true);
                Map.Loading(false);
                return;
            }
        }
    }
    public void Interact(Player playa)
    {
        int dir = playa.direction;
        int dirX = 0, dirY = 0;
        switch (dir)
        {
            case 1: dirX = 1; break;
            case 2: dirX = -1; break;
            case 3: dirY = 1; break;
            case 4: dirY = -1; break;
        }

        int newX = playerX + dirX;
        int newY = playerY + dirY;

        char[] interactable = new char[] { 'n', 'P', 'w' };
        char tile = map[newY][newX];

        if (interactable.Contains(tile))
        {
            switch (tile)
            {
                case 'n':
                    if (mapID == 4 && playa.progressFlags[0] && !playa.progressFlags[1])
                    {
                        Player gary = ChooseStarter(playa);
                        playa.progressFlags[1] = true;
                        RivalBattle(playa, gary);
                        playa.progressFlags[2] = true;
                        playa.HealTeam();
                    }
                    else if (mapID == 4 && playa.progressFlags[3] && !playa.progressFlags[4])
                    {
                        OakReciveParcel(playa);
                    }
                    else if (mapID == 4 && playa.progressFlags[4])
                    {
                        OakBasic(playa);
                    }
                    else if (mapID == 21 && playerY == 3 && !playa.progressFlags[5])
                    {
                        BrockFight(playa);
                        UpdateMap(playa, -1);
                        if (playa.AbleToBattle())
                        {
                            Console.SetCursorPosition(0, 5);
                            Console.WriteLine($"Brock: Huh, not bad for a beginner! v");
                            Console.ReadLine();
                            Console.SetCursorPosition(0, 5);
                            Console.WriteLine($"Here, take this badge and some money! v");
                            Console.ReadLine();
                            Console.SetCursorPosition(0, 5);
                            Console.WriteLine($"{playa.name} recieved the Boulder Badge! v");
                            Console.ReadLine();
                            Console.SetCursorPosition(0, 5);
                            Console.WriteLine($"{playa.name} recieved 1386$ v               ");
                            Console.ReadLine();
                            playa.progressFlags[5] = true;
                            playa.money += 1386;
                        }
                    }
                    else if (mapID == 21 && playerY == 3 && playa.progressFlags[5])
                    {
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine($"Brock: Huh, not bad for a beginner! v");
                        Console.ReadLine();
                    }
                    break;
                case 'P':
                    break;
                case 'w':
                    try
                    {
                        tile = map[newY + dirY][newX + dirX];
                    }
                    catch
                    { break; }
                    if (interactable.Contains(tile))
                    {
                        if (tile == 'n' && (mapID == 8 || mapID == 19))
                        {
                            PokeCenter(playa);
                        }
                        else if (tile == 'n' && (mapID == 9 || mapID == 20))
                        {
                            PokeMart(playa);
                        }
                    }
                    break;
                default:
                    return;
            }
        }
        else return;
    }
    public void OakEncounter(Player playa)
    {
        UpdateMap(playa, -1);
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("Wait! v");
        Console.ReadKey();
        for (int i = 0; i < 5; i++)
        {
            UpdateMap(playa, i);
            Thread.Sleep(500);
        }
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("Its dangerous to go into the tall grass without a pokemon! v");
        Console.ReadKey();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("                                                                    ");
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("Follow me! v");
        Console.ReadKey();
        playerX -= 2;
        for (int i = 0; i < 10; i++)
        {
            playerY++;
            UpdateMap(playa, 9);
            Thread.Sleep(500);
        }
    }
    public Player ChooseStarter(Player playa)
    {
        UpdateMap(playa, 11);
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("Oak: I got three pokemon here for you! v");
        Console.ReadKey();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("Charmander, Bulbasaur, and Squirtle! v  ");
        Console.ReadKey();
        bool pok = true;
        Player gary = new Player("Gary");
        while (pok)
        {
            Console.WriteLine($"Which one do you choose {playa.name}? (1/2/3) v                                     ");
            int pkmn = Convert.ToInt32(Console.ReadLine());           
            Pokemon charmander = new Pokemon(Program.AllPokemon[3], 5);
            charmander.AddMove(new Move(Program.AllMoves[0]));
            charmander.AddMove(new Move(Program.AllMoves[14]));
            Pokemon bulbasaur = new Pokemon(Program.AllPokemon[0], 5);
            bulbasaur.AddMove(new Move(Program.AllMoves[9]));
            bulbasaur.AddMove(new Move(Program.AllMoves[14]));
            Pokemon squirtle = new Pokemon(Program.AllPokemon[6], 5);
            squirtle.AddMove(new Move(Program.AllMoves[9]));
            squirtle.AddMove(new Move(Program.AllMoves[10]));
            switch (pkmn)
            {
                case 1:
                    Console.WriteLine($"Do you want to choose the Fire-Type pokemon Charmander?");
                    Console.WriteLine("Yes");
                    Console.WriteLine("No");
                    string keyInfo = Console.ReadKey().Key.ToString().ToLower();
                    if (!(keyInfo == "y" || keyInfo == "enter"))
                    {
                        continue;
                    }
                    playa.AddPokemon(charmander);
                    gary.AddPokemon(squirtle);
                    pok = false;
                    break;
                case 2:
                    Console.WriteLine($"Do you want to choose the Grass-Type pokemon Bulbasaur?");
                    Console.WriteLine("Yes");
                    Console.WriteLine("No");
                    keyInfo = Console.ReadKey().Key.ToString().ToLower();
                    if (!(keyInfo == "y" || keyInfo == "enter"))
                    {
                        continue;
                    }
                    playa.AddPokemon(bulbasaur);
                    gary.AddPokemon(charmander);
                    pok = false;
                    break;
                case 3:
                    Console.WriteLine($"Do you want to choose the Water-Type pokemon Squirtle?");
                    Console.WriteLine("Yes");
                    Console.WriteLine("No");
                    keyInfo = Console.ReadKey().Key.ToString().ToLower();
                    if (!(keyInfo == "y" || keyInfo == "enter"))
                    {
                        continue;
                    }
                    playa.AddPokemon(squirtle);
                    gary.AddPokemon(bulbasaur);
                    pok = false;
                    break;
                default:
                    Console.WriteLine("Invalid Optinon                                            ");
                    break;
            }
        }
        for (int i = 5; i < 15; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.WriteLine("                                                                 ");
        }
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"{playa.name} recived a {playa.team[0].name}!                    ");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Gary: Hey I want one too!                                       ");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Gary recived a {gary.team[0].name}!                            ");
        Console.ReadLine();
        return gary;
    }
    public void RivalBattle(Player playa, Player gary)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Gary: Where do you think you are going! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Lets have a battle with our new pokemon! v");
        Console.ReadLine();
        Program.Battle(playa, gary, false);
        UpdateMap(playa, 11);
        if (playa.AbleToBattle())
        {
            Console.SetCursorPosition(0, 5);
            Console.WriteLine($"Gary: That was only beginners luck {playa.name}, ill beat you next time! v");
            Console.ReadLine();
        }
        else
        {
            Console.SetCursorPosition(0, 5);
            Console.WriteLine($"Gary: Haha, I win! v                                                       ");
            Console.ReadLine();
        }
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Smell ya' later! v                                                             ");
        Console.ReadLine();
    }
    public void PlayerRecieveParcel(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Clerk: Oh, hey are you from Pallet town? v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"This parcel came for proffesor oak! v         ");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Can you please deliver it to him? v            ");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"{playa.name} recived Oaks parcel! v            ");
        Console.ReadLine(); 
        playa.progressFlags[3] = true;
        Console.SetCursorPosition(0, 5);
        Console.WriteLine("                                                  ");
    }
    public void OakReciveParcel(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Oak: Oh, hey {playa.name}! Thanks for bringing me my parcel! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Here, take this as a reward! v                                   ");
        Console.ReadLine();
        playa.AddItem("Pokedex", 1);
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"{playa.name} recived a Pokedex! v                                  ");
        Console.ReadLine();
        playa.progressFlags[4] = true;
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"And also, take this as to help with your pokedex! v                 ");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"{playa.name} recived 5 Pokeballs! v                                  ");
        Console.ReadLine();
        playa.AddItem("Pokeball", 5);
    }
    public void OakBasic(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Oak: Hey {playa.name}, I see you are doing well on your pokemon journey! v");
        Console.ReadLine();
    }
    public void BrockFight(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Brock: I am Brock! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"I am Pewters gym leader! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Show me your best! v         ");
        Console.ReadLine();
        Trainer brock = new Trainer("Brock");
        Pokemon geodude = new Pokemon(Program.AllPokemon[29], 12);
        geodude.AddMove(new Move(Program.AllMoves[9]));
        geodude.AddMove(new Move(Program.AllMoves[39]));
        Pokemon onix = new Pokemon(Program.AllPokemon[30], 14);
        onix.AddMove(new Move(Program.AllMoves[9]));
        onix.AddMove(new Move(Program.AllMoves[36]));
        brock.AddPokemon(geodude);
        brock.AddPokemon(onix);
        Program.Battle(playa, brock, false);      
    }
    public void PokeCenter(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Nurse: Welcome to the Pokemon Center! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Nurse: We heal your pokemon for free! v");
        Console.ReadLine();
        playa.HealTeam();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Your pokemon have been healed! v             ");
        Console.ReadLine();
    }
    public void PokeMart(Player playa)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Shopkeeper: Welcome to the Poke Mart! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Shopkeeper: We sell all kinds of items for your pokemon journey! v");
        Console.ReadLine();
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Shopkeeper: But unfortunately I haven't implemented the shop yet! v");
        Console.ReadLine();
    }
}
class Program
{
    public static double MatchUp(Type attackType, Type defenseType)
    {
        if (defenseType == 0) return 1.0;

        double eff = 1.0;

        switch (attackType)
        {
            case Type.Normal:
                //Normal
                if (defenseType == Type.Rock) eff = 0.5; //vs Rock, Steel
                break;

            case Type.Fire:
                // Fire
                if (defenseType == Type.Grass ||  defenseType == Type.Bug) eff = 2.0; //vs Grass, Ice, Bug, Steel
                else if (defenseType == Type.Fire || defenseType == Type.Water || defenseType == Type.Rock ) eff = 0.5; //vs Fire, Water, Rock, Dragon
                break;

            case Type.Water:
                // Water
                if (defenseType == Type.Fire || defenseType == Type.Ground || defenseType == Type.Rock) eff = 2.0;// vs Fire, Ground, Rock
                else if (defenseType == Type.Water || defenseType == Type.Grass) eff = 0.5;// vs Water, Grass, Dragon
                break;

            case Type.Electric:
                // Electric
                if (defenseType == Type.Water || defenseType == Type.Flying) eff = 2.0; //vs Water, Flying
                else if (defenseType == Type.Electric || defenseType == Type.Grass) eff = 0.5;// vs Electric, Grass, Dragon
                else if (defenseType == Type.Ground) eff = 0.0;// vs Ground
                break;

            case Type.Grass:
                // Grass
                if (defenseType == Type.Water || defenseType == Type.Ground || defenseType == Type.Rock) eff = 2.0;// vs Water, Ground, Rock
                else if (defenseType == Type.Fire || defenseType == Type.Grass || defenseType == Type.Poison || defenseType == Type.Flying || defenseType == Type.Bug) eff = 0.5; //vs Fire, Grass, Poison, Flying, Bug, Dragon, Steel
                break;

            case Type.Fighting:
                //   Fighting
                if (defenseType == Type.Normal || defenseType == Type.Rock) eff = 2.0; //vs Normal, Ice, Rock, Dark, Steel
                else if (defenseType == Type.Poison || defenseType == Type.Flying || defenseType == Type.Psychic || defenseType == Type.Bug) eff = 0.5; //vs Poison, Flying, Psychic, Bug, Fairy
                break;

            case Type.Poison:
                //  Poison
                if (defenseType == Type.Grass) eff = 2.0;// vs Grass, Fairy
                else if (defenseType == Type.Poison || defenseType == Type.Ground || defenseType == Type.Rock) eff = 0.5; //vs Poison, Ground, Rock, Ghost
                break;

            case Type.Ground:
                //Ground
                if (defenseType == Type.Fire || defenseType == Type.Electric || defenseType == Type.Poison || defenseType == Type.Rock) eff = 2.0;// vs Fire, Electric, Poison, Rock, Steel
                else if (defenseType == Type.Grass || defenseType == Type.Bug) eff = 0.5; //vs Grass, Bug
                else if (defenseType == Type.Flying) eff = 0.0; //vs Flying
                break;

            case Type.Flying:
                // Flying
                if (defenseType == Type.Grass || defenseType == Type.Fighting || defenseType == Type.Bug) eff = 2.0; //vs Grass, Fighting, Bug
                else if (defenseType == Type.Electric || defenseType == Type.Rock) eff = 0.5; //vs Electric, Rock, Steel
                break;

            case Type.Psychic:
                //Psychic
                if (defenseType == Type.Fighting || defenseType == Type.Poison) eff = 2.0;// vs Fighting, Poison
                else if (defenseType == Type.Psychic) eff = 0.5;// vs Psychic, Steel
                break;

            case Type.Bug:
                //  Bug
                if (defenseType == Type.Grass || defenseType == Type.Psychic) eff = 2.0;// vs Grass, Psychic, Dark
                else if (defenseType == Type.Fire || defenseType == Type.Fighting || defenseType == Type.Poison || defenseType == Type.Flying) eff = 0.5; //vs Fire, Fighting, Poison, Flying, Ghost, Steel, Fairy
                break;

            case Type.Rock:
                //  Rock
                if (defenseType == Type.Fire || defenseType == Type.Flying || defenseType == Type.Bug) eff = 2.0;// vs Fire, Ice, Flying, Bug
                else if (defenseType == Type.Fighting || defenseType == Type.Ground) eff = 0.5; //vs Fighting, Ground, Steel
                break;

        }

        return eff;
    }
    public static int FindBestMove(Pokemon atk, Pokemon opp)
    {
        int HighestEffect = 0;
        for (int i = 0; i < atk.moveNum; i++)
        {
            if (HighestEffect < Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcStat(atk.species.Atk), opp.CalcStat(opp.species.Def) * opp.GetMod(opp.DefMod), true))
            {
                HighestEffect = Damage(atk, opp, atk.moveSet[i], atk.moveSet[i].moveB.power, atk.CalcStat(atk.species.Atk), opp.CalcStat(atk.species.Atk) * opp.GetMod(opp.DefMod), true);
            }
        }
        return HighestEffect;
    }
    public static bool CheckAcc(Move move, Pokemon pokemonA, Pokemon pokemonD)
    {
        if (move.moveB.acc == 101) return true;

        int check = Random.Shared.Next(1, 101);
        double gravity = 1.00;
        if (check <= (move.moveB.acc * gravity * pokemonA.GetMod(pokemonA.AccMod) * pokemonD.GetMod(pokemonD.EvaMod)))
        {
            return true;
        }
        return false;
    }
    public static void Move(Pokemon pokemonA, Pokemon pokemonD, Move move)
    {
        if (move.PP <= 0)
        {
            Console.WriteLine($"{pokemonA.species.name} has no PP left for {move.moveB.name}!");
            MoveB struggle = new MoveB("Struggle", 0, 50, 101, Split.Status, 100, new List<MoveEffect>());
            move = new Move(struggle);
            Console.WriteLine($"{pokemonA.species.name} used Struggle!");
            pokemonD.hp -= Damage(pokemonA, pokemonD, move, 50, (pokemonA.CalcStat(pokemonA.species.Atk) * pokemonA.GetMod(pokemonA.AtkMod)), (pokemonD.CalcStat(pokemonA.species.Def) * pokemonD.GetMod(pokemonD.DefMod)), false);
            if (pokemonD.hp < 0) pokemonD.hp = 0;
            pokemonA.hp -= Convert.ToInt32(Math.Floor(pokemonA.maxHP / 4.0));
            if (pokemonA.hp < 0) pokemonA.hp = 0;
            Console.WriteLine($"{pokemonA.species.name} is hit with recoil");
            Console.WriteLine($"recoil brought to: {pokemonA.hp}");
            return;
        }
        move.PP--;

        if (CheckAcc(move, pokemonA, pokemonD) == true || pokemonA.chargingMove)
        {
            if (move.moveB.split == Split.Status)
            {
                pokemonA.lastMove = move;
                foreach (MoveEffect effect in move.moveB.effectList)
                {
                    InflictStatus(pokemonD, effect);
                }
            }
            else
            {
                double attack = 0.0;
                double defense = 0.0;
                pokemonA.critRatio = 0;
                int power = move.moveB.power;
                if (move.moveB.split == Split.Physical)
                {
                    attack = (pokemonA.CalcStat(pokemonA.species.Atk) * pokemonA.GetMod(pokemonA.AtkMod));
                    defense = (pokemonD.CalcStat(pokemonD.species.Def) * pokemonD.GetMod(pokemonD.DefMod));
                }
                else if (move.moveB.split == Split.Special)
                {
                    attack = (pokemonA.CalcStat(pokemonA.species.Spa) * pokemonA.GetMod(pokemonA.SpaMod));
                    defense = (pokemonD.CalcStat(pokemonD.species.Spa) * pokemonD.GetMod(pokemonD.SpaMod));
                }
                List<string> critMoves = new List<string> { "Razor Leaf", "Slash"};
                if (critMoves.Contains(move.moveB.name))
                {
                    pokemonA.critRatio++;
                }
                if (move.moveB.name == "Endeavor")
                {
                    if (pokemonD.hp < pokemonA.hp)
                    {
                        return;
                    }
                    else
                        pokemonD.hp = pokemonA.hp;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.lastMove = move;
                    return;

                }
                if (move.moveB.name == "Super Fang")
                {
                    int halfHp = Convert.ToInt32(Math.Floor((double)pokemonD.hp / 2));
                    pokemonD.hp -= halfHp;
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    pokemonA.lastMove = move;
                    return;
                }
                if (pokemonA.lastMove != null && pokemonA.lastMove.moveB.name == "Charge" && move.moveB.type == Type.Electric)
                {
                    power *= 2;
                }
                List<string> charge = new List<string> { "Solar Beam",  "Skull Bash" };
                if (charge.Contains(move.moveB.name) && !pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = true;
                    Console.WriteLine($"{pokemonA.name} is charging up for {move.moveB.name}!");
                    pokemonA.lastMove = move;
                    return;
                }
                else if (charge.Contains(move.moveB.name) && pokemonA.chargingMove)
                {
                    pokemonA.chargingMove = false;
                }

                pokemonA.lastMove = move;
                int numHits = 1;
                if (move.moveB.effectList != null && move.moveB.effectList.Count > 0) numHits = 1;
                for (int i = 0; i < numHits; i++)
                { 
                    pokemonD.hp -=  Damage(pokemonA, pokemonD, move, power, attack, defense, false);
                    if (pokemonD.hp < 0) pokemonD.hp = 0;
                    if (move.moveB.effectList != null)
                    {
                        foreach (MoveEffect effect in move.moveB.effectList)
                        {                          
                                InflictStatus(pokemonD, effect);
                        }
                    }
                }
                pokemonA.critRatio = 0;
            }
        }
        else
        {
            Console.WriteLine("haha you missed");
            MoveB mis = new MoveB("Miss", Type.Normal, 0, 0, Split.Status, 0, null);
            Move miss = new Move(mis);
            pokemonA.lastMove = miss;
        }
    }
    public static int Damage(Pokemon pokemonA, Pokemon pokemonD, Move move, int power, double atk, double def, bool test)
    {
        Type pkAtype1 = pokemonA.species.type1;
        Type pkAtype2 = pokemonA.species.type2;
        Type pkDtype1 = pokemonD.species.type1;
        Type pkDtype2 = pokemonD.species.type2;


        double stab = 1;

        if (pkAtype1 == move.moveB.type || pkAtype2 == move.moveB.type)
        {
            stab = 1.5;
        }

        double status = 1.00;
        if (pokemonA.statusNonVol == Status.Burn && move.moveB.split == Split.Physical)
        {
            status = 0.50;
        }

        double eff1 = MatchUp(move.moveB.type, pkDtype1);
        double eff2 = MatchUp(move.moveB.type, pkDtype2);
        



        double crit = 1;
        int rcrit = 24;
        if (Random.Shared.Next(0, rcrit) == 0 && !test)
        {
            crit = 2;
            Console.Write("Critical hit!");
        }

        double ran = Random.Shared.Next(85, 101) / 100.0;
        if (test) ran = 9.2;
        int dmg = Convert.ToInt32(Math.Round(((((((((2 * pokemonA.level * crit) / 5) + 2) * power * ((double)atk / def)) / 50)) + 2) * stab * eff1 * eff2 * ran * status), 0));// too complicated check https:bulbapedia.bulbagarden.net / wiki / Damage
        if (pokemonD.hp < dmg && !test)
        {
            Console.WriteLine($" It did {pokemonD.hp} damage!");
        }
        else if (!test)
        {
            Console.WriteLine($" It did {dmg} damage!");
        }

        return dmg;
    }
    public static void InflictStatus(Pokemon pk, MoveEffect effect)
    {
        if (effect.effectChance < 101)
        {
            int check = Random.Shared.Next(1, 101);

            if (check <= effect.effectChance)
            {
                if (effect.effectStat != Stat.None && !(effect.effectPower < 0 ))
                {
                    switch (effect.effectStat)
                    {
                        case Stat.Atk:
                            if (pk.AtkMod < 6 && pk.AtkMod > -6)
                            {
                                pk.AtkMod += effect.effectPower;
                                if (pk.AtkMod > 6) pk.AtkMod = 6;
                                else if (pk.AtkMod < -6) pk.AtkMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Def:
                            if (pk.DefMod < 6 && pk.DefMod > -6)
                            {
                                pk.DefMod += effect.effectPower;
                                if (pk.DefMod > 6) pk.DefMod = 6;
                                else if (pk.DefMod < -6) pk.DefMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Spa:
                            if (pk.SpaMod < 6 && pk.SpaMod > -6)
                            {
                                pk.SpaMod += effect.effectPower;
                                if (pk.SpaMod > 6) pk.SpaMod = 6;
                                else if (pk.SpaMod < -6) pk.SpaMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;                     
                        case Stat.Acc:
                            if (pk.AccMod < 6 && pk.AccMod > -6)
                            {
                                pk.AccMod += effect.effectPower;
                                if (pk.AccMod > 6) pk.AccMod = 6;
                                else if (pk.AccMod < -6) pk.AccMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Eva:
                            if (pk.EvaMod < 6 && pk.EvaMod > -6)
                            {
                                pk.EvaMod += effect.effectPower;
                                if (pk.EvaMod > 6) pk.EvaMod = 6;
                                else if (pk.EvaMod < -6) pk.EvaMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        case Stat.Spe:
                            if (pk.SpeMod < 6 && pk.SpeMod > -6)
                            {
                                pk.SpeMod += effect.effectPower;
                                if (pk.SpeMod > 6) pk.SpeMod = 6;
                                else if (pk.SpeMod < -6) pk.SpeMod = -6;
                            }
                            else
                            {
                                Console.WriteLine("It cant go higher");
                            }
                            break;
                        default:
                            break;
                    }
                }
                List<Status> nonVolitile = new List<Status> { Status.Confusion };
                if (effect.effectStatus != Status.None)
                {
                    if ( pk.statusNonVol == Status.None && !nonVolitile.Contains(effect.effectStatus) && !pk.IsImmune(effect.effectStatus))
                    {
                        pk.statusNonVol = effect.effectStatus;
                    }
                    else if (!pk.statusVol.Contains(effect.effectStatus) && effect.effectStatus != Status.Flinch && !pk.IsImmune(effect.effectStatus))
                    {                      
                        List<Status> trap = new List<Status> { Status.Bound};
                        if (pk.statusVol.Any(s => trap.Contains(s)))
                        {
                            return;
                        }
                        pk.statusVol.Add(effect.effectStatus);
                    }
                }
            }
        }
    }
    public static void Battle(Player team1, Trainer team2, bool wild)
    {
        Console.Clear();
        team2.HealTeam();
        int tm2index = 0;
        Random r = new Random();
        Pokemon currentPokemon1 = team1.team[0];
        Pokemon currentPokemon2 = team2.team[tm2index];
        if (wild) Console.WriteLine($"A wild {currentPokemon2.species.name} appeared!");
        else Console.WriteLine($"Trainer {team2.name} challanged you to a battle!");
        Console.WriteLine($"{team1.name} sent out {currentPokemon1.name}");
        if (!wild) Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
        while (team1.AbleToBattle() && team2.AbleToBattle())
        {
            currentPokemon1.moveFirst = false;
            currentPokemon2.moveFirst = false;
            currentPokemon1.selectedMove = null;
            currentPokemon2.selectedMove = null;
            double spe1 = 0;
            double spe2 = 0;

            if(currentPokemon2.hp > 0)
            {
                double para = 1;
                if (currentPokemon2.statusNonVol == Status.Paralysis) para = 0.5;
                spe1 = currentPokemon2.CalcStat(currentPokemon2.species.Spe) * currentPokemon2.GetMod(currentPokemon2.SpeMod) * para;
                currentPokemon2.selectedMove = currentPokemon2.moveSet[r.Next(0, currentPokemon2.moveNum)];
            }
            if (currentPokemon1.hp > 0)
            {
                Console.WriteLine("Choose your action");
                Console.WriteLine("[1] Fight");
                Console.WriteLine("[2] Pokemon");
                Console.WriteLine("[3] Bag");
                Console.WriteLine("[4] Run");
                int input = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                double para = 1;
                if (currentPokemon1.statusNonVol == Status.Paralysis) para = 0.5;
                spe1 = currentPokemon1.CalcStat(currentPokemon1.species.Spe) * currentPokemon1.GetMod(currentPokemon1.SpeMod) * para;
                if (input == 1)
                {
                    if (currentPokemon1.chargingMove)
                    {
                        currentPokemon1.selectedMove = currentPokemon1.lastMove;
                    }
                    else
                    {
                        currentPokemon1.selectedMove = currentPokemon1.PickMove();
                    }
                }
                else if (input == 2)
                {
                    currentPokemon1 = team1.ShouldSwitch(currentPokemon1);
                }
                else if (input == 3)
                {
                    bool empty = false;
                    int index = 1;
                    foreach (var item in team1.bag)
                    {
                        if (item.count > 0)
                        {
                            Console.WriteLine($"[{index}] {item.name}: {item.count}");
                            empty = true;
                            index++;
                        }
                    }

                    if (!empty)
                        Console.WriteLine("(Empty bag)");

                    Console.WriteLine("\n what item to use? (back)");
                    string itemNum = Console.ReadLine().ToLower();
                    if (itemNum != "back")
                    {
                        Console.WriteLine();
                        int iNum = Convert.ToInt32(itemNum);
                        var visibleItems = team1.bag.Where(item => item.count > 0).ToList();
                        int foundItem = visibleItems.FindIndex(item => item.name == "Pokeball");

                        Console.WriteLine(foundItem + 1);
                        if (iNum == foundItem + 1 && wild)
                        {
                            bool caught = CatchPokemon(team1, currentPokemon2);
                            team1.bag[foundItem].count--;
                            if (caught) return;
                        }
                        else
                        {
                            Console.WriteLine("You cant use that here!");
                        }
                    }
                }
                else if (input == 4)
                {
                    if (wild)
                    {
                        if (spe1 > spe2 || r.Next(0, 2) == 0)
                        {
                            Console.WriteLine("You got away safely!");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("You failed to run away!");
                            currentPokemon2.moveFirst = true;
                            currentPokemon1.selectedMove = null;
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You can't run from a trainer battle!");
                        continue;
                    }
                }       
            }

            int priority1 = 0;
            int priority2 = 0;
            if (currentPokemon1.selectedMove != null && currentPokemon1.selectedMove.moveB.name == "Quick Attack") { priority1 = 1; }
            if (currentPokemon2.selectedMove != null && currentPokemon2.selectedMove.moveB.name == "Quick Attack") { priority1 = 1; }


            if (priority1 > priority2)
            {
               currentPokemon2.moveFirst = true;
            }
            else if (priority1 < priority2)
            {
               currentPokemon1.moveFirst = true;
            }
            else
            {
                if (spe1 > spe2)
                {
                   currentPokemon2.moveFirst = true;
                }
                else if (spe1 < spe2)
                {
                    currentPokemon1.moveFirst = true;
                }
                else
                {
                    int tie = Random.Shared.Next(0, 2);
                    if (tie == 0)
                    {
                        currentPokemon2.moveFirst = true;
                    }
                    else
                    {
                       currentPokemon1.moveFirst = true;
                    }
                }
            }


            if (currentPokemon1.moveFirst)
            {
                if (currentPokemon1.selectedMove != null && currentPokemon1.DoIMove())
                {
                    ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove);
                }
                if (currentPokemon2.selectedMove != null && currentPokemon2.hp > 0)
                {
                    if (currentPokemon2.DoIMove())
                    {
                        if (currentPokemon1.hp <= 0 && currentPokemon2.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon2.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove);
                    }
                }
            }
            else
            {
                if (currentPokemon2.selectedMove != null && currentPokemon2.DoIMove())
                {
                    ExecuteMove(currentPokemon2, currentPokemon1, currentPokemon2.selectedMove);
                }
                if (currentPokemon1.selectedMove != null && currentPokemon1.hp > 0)
                {
                    if (currentPokemon1.DoIMove())
                    {
                        if (currentPokemon2.hp <= 0 && currentPokemon1.selectedMove.moveB.split != Split.Status)
                            Console.WriteLine($"{currentPokemon1.selectedMove.moveB.name} failed");
                        else
                            ExecuteMove(currentPokemon1, currentPokemon2, currentPokemon1.selectedMove);
                    }
                }
            }


            PostTurnPokemonCheck(currentPokemon1);
            PostTurnPokemonCheck(currentPokemon2);

            if (currentPokemon1.hp <= 0 && team1.AbleToBattle())
            {
                currentPokemon1 = team1.ShouldSwitch(currentPokemon1);
            }
            if (currentPokemon2.hp <= 0 && team2.AbleToBattle())
            {
                if (!wild) currentPokemon2 = team2.team[++tm2index];
                if (!wild) Console.WriteLine($"{team2.name} sent out {currentPokemon2.name}");
            }

        }
        if (team1.AbleToBattle())
        {
            Console.WriteLine($"{team1.name} wins the battle!");
            currentPokemon1.exp += team2.team[tm2index].species.expYield;
            currentPokemon1.CheckLevelUp();
            for (int i = 0; i < 5; i++)
            {
                switch(i)
                {
                    case 0:
                        currentPokemon1.HpEV += team2.team[tm2index].species.evYield[i];
                        break;
                    case 1:
                        currentPokemon1.AtkEV += team2.team[tm2index].species.evYield[i];
                        break;
                    case 2:
                        currentPokemon1.DefEV += team2.team[tm2index].species.evYield[i];
                        break;
                    case 3:
                        currentPokemon1.SpaEV += team2.team[tm2index].species.evYield[i];
                        break;
                    case 4:
                        currentPokemon1.SpeEV += team2.team[tm2index].species.evYield[i];
                        break;
                }
            }
        }
    }
    public static void ExecuteMove(Pokemon atk, Pokemon def, Move move)
    {
        Console.WriteLine($"{atk.name} used {move.moveB.name} against {def.name}");
        Move(atk, def, move);

        Console.WriteLine(def.hp);
        if (def.hp <= 0)
        {
            Console.WriteLine($"{def.name} fainted");  
        }
        if (atk.hp <= 0)
        {
            Console.WriteLine($"{atk.name} fainted");
        }
    }
    public static void PostTurnPokemonCheck(Pokemon pk)
    {
        if (pk == null) return;
        if (pk.hp <= 0) return;
        switch (pk.statusNonVol)
        {
            case Status.Poison:
                int dmg = Convert.ToInt32(Math.Round(pk.maxHP / 8.0, 0));
                pk.hp -= dmg;
                Console.WriteLine($"{pk.name} is hurt by poison and lost {dmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
            case Status.Burn:
                int burnDmg = Convert.ToInt32(Math.Round(pk.maxHP / 16.0, 0));
                pk.hp -= burnDmg;
                Console.WriteLine($"{pk.name} is hurt by its burn and lost {burnDmg} HP!");
                if (pk.hp < 0) pk.hp = 0;
                break;
        }
        if (pk.statusVol.Contains(Status.Bound))
        {
            double chip = 8.0;
            if (pk.boundCounter == 0) pk.boundCounter = 1;
            if (pk.boundCounter == 4)
            {
                pk.statusVol.Remove(Status.Bound);
                pk.statusVol.Remove(Status.EscapePrevent);
                Console.WriteLine($"{pk.name} is no longer bound!");
                return;
            }

            int dmg = Convert.ToInt32(Math.Round(pk.maxHP / chip, 0));
            pk.hp -= dmg;
            Console.WriteLine($"{pk.name} is hurt by poison and lost {dmg} HP!");
            pk.boundCounter++;
            if (pk.hp < 0) pk.hp = 0;
        }
    }
    public static bool CatchPokemon(Player playa, Pokemon pokemon)
    {
        if (pokemon.hp <= 0)
            return false;

        int maxHP = pokemon.maxHP;
        int hp = pokemon.hp;
        int catchRate = pokemon.species.catchRate;

        int a = ((3 * maxHP - 2 * hp) * catchRate) / (3 * maxHP);
        if (a > 255) a = 255;

        int r = Random.Shared.Next(0, 256);
        if (r <= a)
        {
            Console.WriteLine($"You caught {pokemon.species.name}!");
            playa.AddPokemon(pokemon);
            Console.ReadLine();
            return true;
        }
        else
        {
            Console.WriteLine($"{pokemon.species.name} broke free!");
            return false;
        }
    }
    #region maps
    public static string[] levelData = {
        ""
    };

    public static string[] levelData1 = {
        "wwwwwwwwww",
        "wwww    0w",
        "w        w",
        "w   w    w",
        "w   w    w",
        "ww     w w",
        "ww     w w",
        "wwwwwwwwww"
    };

    public static string[] levelData2 = {
        "wwwwwwwwww",
        "www w   0w",
        "w        w",
        "w  cwwn  w",
        "w  cwwc  w",
        "w        w",
        "w        w",
        "www11wwwww"
    };

    public static string[] levelData3 = new string[]
    {
        "         w33w       ",
        "         wggw       ",
        "         wggw       ",
        "wwwwwwwwwwggwwwwwwww",
        "wp                pw",
        "wp  wwww    wwww  pw",
        "wp  wwww    wwww  pw",
        "wp tw0ww   tw1ww  pw",
        "wp                pw",
        "wp                pw",
        "wp        wwwwww  pw",
        "wp  wwwt  wwwwww  pw",
        "wp  pppp  wwwwww  pw",
        "wp  pppp  ww2www  pw",
        "wp                pw",
        "wp        wwwtww  pw",
        "wpppssss  pppppp  pw",
        "wpppssss  pppppp  pw",
        "wpppssss          pw",
        "wwppsssswwwwwwwwwwww",
        "wwwwsssswwwwwwwwwwww"
    };

    public static string[] levelData4 = {
        "wwwwwwwwwwww",
        "wwwww  wwwww",
        "w     n    w",
        "w      www w",
        "w          w",
        "w          w",
        "wwwww  wwwww",
        "wwwww  wwwww",
        "w          w",
        "w          w",
        "w          w",
        "w          w",
        "wwwww00wwwww"
    };

    public static string[] levelData5 = {
        "wwwwwwwwww",
        "www     ww",
        "w        w",
        "w  nwwc  w",
        "w  cwwc  w",
        "w        w",
        "ww      ww",
        "ww      ww",
        "www00wwwww"
    };

    public static string[] levelData6 = {
        "      w  w      ",
        "      w11w      ",
        "      w  w      ",
        "wwwwwww  wwwwwww",
        "wpppppp  ppppppw",
        "wpppppp  ppppppw",
        "wpppppa        w",
        "wvvvvvavvvv    w",
        "wpppppaggggggggw",
        "wpppppaggggggggw",
        "wpppppaggggggggw",
        "wvvvvvaggggggggw",
        "wpppppppp      w",
        "wpppppppp      w",
        "wpppppppp  ggggw",
        "waavvvvaaaaggggw",
        "wpppppppp  ggggw",
        "wpppppppp  ggggw",
        "wpp        ppppw",
        "wpp        ppppw",
        "wpp  ppppppppppw",
        "wv vvv vvvvvvvvw",
        "wpp            w",
        "wpp            w",
        "wppppppppgggg  w",
        "waaaaaaaaggggvvw",
        "wppppppppgggg  w",
        "wppppppppgggg  w",
        "w              w",
        "wvv   tvvvvvvvvw",
        "wppgggg  ppggggw",
        "wppgggg  ppggggw",
        "wgggg    ggggppw",
        "wgggg    ggggppw",
        "wwwwwwwggwwwwwww",
        "      wggw      ",
        "      w00w      ",
        "      wggw      "
    };

    public static string[] levelData7 = {
        "               w   a                ",
        "               w666a                ",
        "  wwwwwwwwwwaaaw  taaaawwwwwwwwwwwww",
        "  waaaaaaaaaaaaw   aaaa            w",
        "  waaaaaaaaaaaaw   aaaa            w",
        "  wwwppppppppb             wwwwww  w",
        "  wwwppwwwwwwaa            wwwwww  w",
        "  wwwpwaaaaaaaaw   w       wwwwww  w",
        "  wwwpwaaaaaaaaw   w      twwww9w  w",
        "  wwwpwaaaaaaaaw   wwww            w",
        "  wwwpwaaaaaaaaw   w5wwvvvvvvvvvvvvw",
        "  wwwpwaaaaaaaaw                   w",
        "  wwwpwaaaaaaaaw                   w",
        "  wwwpwaaaaaaaaw                   w",
        "wwwwwpwaaaaaaaaw   wwwwwwwwwwwwwwwww",
        "1pppppwaaaaaaaaw   wwww            w",
        "1pppppwaaaaaaaaw   w4ww            w",
        "1    ppaaaaaaaa          ppwwwwpp  w",
        "1    pppppppppp t  wwww  ppwwwwpp  w",
        "www                pppp  ppwwwwpp  w",
        "www                pppp  ppw3wwpp  w",
        "  w  pppppppppp                    w",
        "  waaaapppppppp                    w",
        "  w  ppbppppppp  pp  wwww  pppppp  w",
        "  w  ppaapppppp  pp  wwww  pppppp  w",
        "  w  ppsssssspp  pp  wwww  pppppp  w",
        "  w  ppsssssspp  pp  w2ww  pppppp  w",
        "  w  ppsssssspp  pp                w",
        "  wvvvvssssssvpvvv vvvvvvvvvvvvvvvvw",
        "  w  pppppppppp  pp  pppppp  pppp  w",
        "  w  pppppppppp  pp tpppppp  pppp  w",
        "  w                                w",
        "  wwwwwwwwwwwwwwwww  wwwwwwwwwwwwwww",
        "                  w  w              ",
        "                  w  w              ",
        "                  w00w              ",
        "                  w  w              "
    };

    public static string[] levelData8 = {
        "wwwwwwwwwww9w9ww",
        "wwmm   wmm     w",
        "wwmmn  wmm     w",
        "wwwwwwwwwwwwnwww",
        "w             Pw",
        "ww             w",
        "ww             w",
        "www    ww    www",
        "www    ww    www",
        "wwww00wwwwwwwwww"
    };

    public static string[] levelData9 = {
        "wwwwwwwwww",
        "wwwwwwwwww",
        "w        w",
        "www  wwwww",
        "www  wwwww",
        "wnw      w",
        "www      w",
        "w        w",
        "wwww00wwww"
    };

    public static string[] levelData10 = {
        "wwwwwwwwww",
        "www     ww",
        "w        w",
        "w  nwwc  w",
        "w  cwwc  w",
        "w        w",
        "ww      ww",
        "ww      ww",
        "www00wwwww"
    };

    public static string[] levelData11 = {
        "wwwwwwwwww",
        "www     ww",
        "w        w",
        "w  nwwc  w",
        "w  cwwc  w",
        "w        w",
        "ww      ww",
        "ww      ww",
        "www00wwwww"
    };

    public static string[] levelData12 = {
    "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww  ",
    "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwppppppwww  ",
    "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwvvvpvvwww  ",
    "wwwwwwwwwwwwwww                    www  ",
    "wwwwwww1wwwwwww                    wwwww",
    "w            ww      sssswwwwppppwwpppp0",
    "w            wwvvvvvvsssswwwwvpvvwppppp0",
    "wppppppppppppwwggggggsssswwwwggggwp    0",
    "wvvvvvvvvvpvvwwggggggsssswwwwggggwp    0",
    "w            wwggggggppppwwwwggggwp  www",
    "w    wtwwwwwwwwggggggppppwwwwggggwp  w  ",
    "wppppppppppppppppppppppppwwwwppppppppw  ",
    "wvvvvvvvvvvvvvvvvvvvvvvvvwwwwvvvpvvvvw  ",
    "w                                    w  ",
    "w                                    w  ",
    "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww  "
};

    public static string[] levelData13 = {
    "wwwww99wwwww",
    "w   w  w   w",
    "w      n   w",
    "w          w",
    "w          w",
    "w          w",
    "w          w",
    "w   w  w   w",
    "wwwww00wwwww"
};

    public static string[] levelData14 = {
    "        a  a          ",
    "        a33a          ",
    "        a  a          ",
    "aaaaaaaaa  aaaaaaaaaaa",
    "agggggggg  w         a",
    "agggggggg  w         a",
    "agggggggg  w         a",
    "agggggggg  w         a",
    "agggggggg  wwwwwwww  a",
    "agggggggg  wwwwwwww  a",
    "a          wwwwwwww  a",
    "a          ww9wwwww  a",
    "aaaapabaaaa    ppppppa",
    "appa2appppp t  ppppppa",
    "appwwwwpppp          a",
    "appwwwwpppp          a",
    "appwwwwpppppppppppp  a",
    "awww9wwwwwwwwwwpppp  a",
    "a        aaaaaapppp  a",
    "a        aaaaaapppp  a",
    "a        aaaaaawwww  a",
    "avvvvvvvvaaaaaaw9ww  a",
    "agg  ggggaaaaaa      a",
    "agg  ggggaaaaaa      a",
    "aaaaaaaaaaaaaaaabaaaaa",
    "aaaaaaaaaaaaaaaapppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaavvvpvva",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaavvvpvva",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaappppppa",
    "aaaaaaaaaaaaaaawwppwwa",
    "aaaaaaaaaaaaaaawwwwwwa",
    "aaaaaaaaaaaaaaawwwwwwa",
    "aaaapabaaaaaaaawwwwwwa",
    "appapapppppaaaaw9wwwwa",
    "appwwwwppppaaaappppppa",
    "appwwwwppppaaaappppppa",
    "appwwwwppppaaaappppppa",
    "awww1wwwwwwaaaavpvvvva",
    "a          pappppppppa",
    "a          pappppppppa",
    "a          pappppppppa",
    "avvvvvvvv  pappppppppa",
    "app  ggggggpappppppppa",
    "app  ggggggpavvvpvvvva",
    "app  ggggggpappppppppa",
    "app  ggggggpappppppppa",
    "app  ppppppppbpppppppa",
    "app  ppaaaaaaaappppppa",
    "app  paaaaaaaapppppppa",
    "app  paaaaaaaapppppppa",
    "app  ppaaaaaaapppppppa",
    "app  ppppppppapppppppa",
    "app        ppapppppppa",
    "app        ppapppppppa",
    "aaapppppp  ppbpppppppa",
    "aaavvvvvpvvvvaavpvvvva",
    "aaapppppp  ppapppppppa",
    "aaapppppp  ppapppppppa",
    "aaa        ppapppppppa",
    "aaa   t    ppapppppppa",
    "aaa  pppp  ppapppppppa",
    "aaa  pppp  ppapppppppa",
    "aaa        ppbpppppppa",
    "aaa        aaaaaaaaaaa",
    "aaaaaaaw   aaaaaaaaaaa",
    "aaaaaaaw   aaaaaaaaaaa",
    "aaaaaaaw000aaaaaaaaaaa",
    "aaaaaaaw   aaaaaaaaaaa",
};

    public static string[] levelData15 = {
    "wwwwww1wwwww",
    "w          w",
    "ww     w www",
    "ww     w www",
    "ww        ww",
    "ww     w www",
    "ww     w www",
    "ww        ww",
    "wwwww00wwwww"
};

    public static string[] levelData16 = {
    "aw11waaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "aw  waawwwwwwwwaawwwwwwwwwwwwwwwwwwa",
    "aw twaa        aa                 wa",
    "aw  waa        aa                 wa",
    "aw  waa        aa    wwwwwwwwww   wa",
    "aw  waa   ww   aa   waaaaaaaaaaw  wa",
    "aw  waa   ww   aa   waaaaaaaaaaw  wa",
    "awggwaagggwwgggaagggwaaaaaaaaaaw  wa",
    "awggwaagggwwgggaagggwaaaaaaaaaaw  wa",
    "awggwaagggwwgggaagggggggg         wa",
    "awggwaagggwwgggaagggggggg         wa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwgggaagggwaaaaw  waagggwa",
    "awggwaagggwwggg  gggwaaaaw  waagggwa",
    "awggwaagggwwggg  gggwaaaaw twaagggwa",
    "awgnwaagggww        waaaaw        wa",
    "awggwaagggww        waaaaw     n  wa",
    "awggwaagggwaaaaaaaaaaaaaawgggaaw  wa",
    "awggwaagggwaaaaaaaaaaaaaawgggaaw  wa",
    "awggg  gggwaaaaaaaaaaaaaawgggaaw  wa",
    "awggg  gggwaaaaaaaaaaaaaawgggaaw  wa",
    "aw   t        waaaaaaaaaawgggaaw  wa",
    "aw            waaaaaaaaaawgggaaw  wa",
    "aaaaaaaaaaaw  waaaaaaaaaawgggaaw  wa",
    "aaaaaaaaaaaw  waaaaaaaaaawgggaaw  wa",
    "aaaaaaaaaaaw  waaaaaaaaaawgggaaw  wa",
    "aaaaaaaaaaaw  waaaaaaaaaawgggaaw  wa",
    "awggggggggwaaaaaaaaaaaaaawgggaaw  wa",
    "awggggggggwaaaaaaaaaaaaaawgggaaw  wa",
    "aaaaaaa  ggggggggt gwaaaawg       wa",
    "aaaaaaa  gggggggg  gwaaaawg    n  wa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "aaaaaaa  gwaaaawg  gwaaaawg  aaaaaaa",
    "awggggg  gggggggg  ggggggtg  gggggwa",
    "awggggg  ggggggggaagggggggg  gggggwa",
    "awggggg          aa          gggggwa",
    "awggggg                      gggggwa",
    "aaaaaaaaaaaaaaaw    waaaaaaaaaaaaaaa",
    "aaaaaaaaaaaaaaaw   twaaaaaaaaaaaaaaa",
    "aaaaaaaaaaaaaaaw    waaaaaaaaaaaaaaa",
    "aaaaaaaaaaaaaaaw    waaaaaaaaaaaaaaa",
    "aaaaaaaaaaaaaaaw0000waaaaaaaaaaaaaaa"
};

    public static string[] levelData17 = {
    "wwwwww1wwwww",
    "w          w",
    "ww     w www",
    "ww     w www",
    "ww        ww",
    "ww     w www",
    "ww     w www",
    "ww        ww",
    "wwwww00wwwww"
};

    public static string[] levelData18 = {
        "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwaa     ",
        "w  ppppwwwwwwwwwwwwwwpppppppppppa     ",
        "w  ppppwwwwwwwwwwwwwwaaaapppppppa     ",
        "w  ppppwwwwwwwwwwwwwwppbppppppppa     ",
        "w  ppppwwwwwwwww9wwwwppaapppppppa     ",
        "w  ppppwwwwwwww  pppppppapppppppa     ",
        "wvvv vvwwww6wwwvvvvvvvvvavvv vvva     ",
        "w                pppppppppppppppa     ",
        "w           t    pppppppppppppppa     ",
        "w  pppppppppppp                wwwwwww",
        "w  pppppppppppp                wwwwwww",
        "w                ppppppppwwww  wwwwwww",
        "w                ppppppppw5ww  wwwwwww",
        "w  pppp  wwwwww    wwww        wwwwwww",
        "w  pppp  wwwwww    wwww        wwwwwww",
        "w  pppp  wwwwww  ppwwwwpppppp         ",
        "w  pppp twwww3w  ppw2wwpppppp         ",
        "w              w                      ",
        "w              w              t       ",
        "w  pppp  ppppppw pppppppppppp  wwwwwww",
        "waaaaaaaaaaaaaaw pppppppppppp  wwwwwww",
        "w        wwww                  wwwwwww",
        "w        wwww      wwwtwwww    wwwwwww",
        "w  ppppppwwwwpp  pappppppppap  wwwwwww",
        "w  ppppppw1wwpp  pappppppppap  wwwwwww",
        "w                pappppppppap  pw     ",
        "w                pappppppppap  pw     ",
        "w  wwwwpppppppp  pappppppppap  pw     ",
        "w  w4wwpppppppp tpapvvv vvvap  pw     ",
        "w                              pw     ",
        "w                              pw     ",
        "wwwwwwwwwaaaaaa  aaaaaawwwwwwwwww     ",
        "              a  a                    ",
        "              a00a                    ",
        "              a  a                    "
    };

    public static string[] levelData19 = {
        "wwwwwwwwwww9w9ww",
        "wwmm   wmm     w",
        "wwmmn  wmm     w",
        "wwwwwwwwwwwwnwww",
        "w             Pw",
        "ww             w",
        "ww             w",
        "www    ww    www",
        "www    ww    www",
        "wwww00wwwwwwwwww"
    };

    public static string[] levelData20 = {
        "wwwwwwwwww",
        "wwwwwwwwww",
        "w        w",
        "www  wwwww",
        "www  wwwww",
        "wnw      w",
        "www      w",
        "w        w",
        "wwww00wwww"
    };

    public static string[] levelData21 = {
        "wwwwwwwwwwww",
        "wwwwwwwwwwww",
        "ww   n    ww",
        "ww        ww",
        "wwwww  wwwww",
        "ww        ww",
        "ww w  www ww",
        "ww  n     ww",
        "ww w  www ww",
        "ww        ww",
        "wwwww  wwwww",
        "w   w  wn  w",
        "w          w",
        "w          w",
        "w          w",
        "wwwww00wwwww"
    };

    public static string[] levelData22 = {
        "wwwwwwwwww",
        "www     ww",
        "w        w",
        "w  nwwc  w",
        "w  cwwc  w",
        "w        w",
        "ww      ww",
        "ww      ww",
        "www00wwwww"
    };

    public static string[] levelData23 = {
        "wwwwwwwwww",
        "www     ww",
        "w        w",
        "w  nwwc  w",
        "w  cwwc  w",
        "w        w",
        "ww      ww",
        "ww      ww",
        "www00wwwww"
    };

    public static string[] levelData24 = {
        "wwwwwwwwwwwwwwwwwwwwww",
        "w           wwwwwwwwww",
        "w wwww      wc  nm   w",
        "w wwww      wc       w",
        "w           wn       w",
        "w wwww   w  wwww     w",
        "w wwww   w    ww    ww",
        "w       1w    ww    ww",
        "wwwwwwwwwww00wwww99www"
    };

    public static string[] levelData25 = {
        "wwwwwwwwwwwwwwww",
        "w          www w",
        "w          www w",
        "w              w",
        "w wwww         w",
        "w wwww   w     w",
        "w        w     w",
        "w       0w     w",
        "wwwwwwwwwwwwwwww"
    };

    public static Map Loading = new Map(0, levelData, null, null);

    public static Port portAshTopToBottom = new Port(2, 7, 1);

    public static Port portAshBottomToTop = new Port(1, 7, 1);
    public static Port portAshToPallet = new Port(3, 5, 8);

    public static Port portPalletToAsh = new Port(2, 4, 6);
    public static Port portPalletToDaisy = new Port(5, 4, 7);
    public static Port portPalletToLab = new Port(4, 6, 11);
    public static Port portPalletToRoute1 = new Port(6, 7, 35);

    public static Port portDaisyToPallet = new Port(3, 13, 8);

    public static Port portLabToPallet = new Port(3, 12, 14);

    public static Port portRoute1ToPallet = new Port(3, 10, 1);
    public static Port portRoute1ToViridian = new Port(7, 19, 34);

    public static Port portViridianToRoute1 = new Port(6, 7, 2);
    public static Port portViridianToPokeCenter = new Port(8, 4, 8);
    public static Port portViridianToPokeMart = new Port(9, 4, 7);
    public static Port portViridianToVHouse1 = new Port(10, 3, 7);
    public static Port portViridianToVHouse2 = new Port(11, 3, 7);
    public static Port portViridianToRoute22 = new Port(12, 38, 7);
    public static Port portViridianToRoute2 = new Port(14, 9, 73);

    public static Port portPokeCenterToViridian = new Port(7, 22, 27);

    public static Port portPokeMartToViridian = new Port(7, 28, 21);

    public static Port portVHouse1ToViridian = new Port(7, 20, 17);

    public static Port portVhouse2ToViridian = new Port(7, 20, 11);

    public static Port portRoute22ToViridian = new Port(7, 1, 16);
    public static Port portRoute22ToVictoryRoadCheck = new Port(13, 5, 7);

    public static Port portVictoryRoadCheckToRoute22 = new Port(12, 7, 5);

    public static Port portRoute2ToViridian = new Port(7, 17, 2);
    public static Port portRoute2ToViridianForestBottom = new Port(15, 5, 7);
    public static Port portRoute2ToViridianForestTop = new Port(17, 6, 1);
    public static Port portRoute2ToPewter = new Port(18, 15, 32);

    public static Port portViridianForestBottomToRoute2 = new Port(14, 4, 46);
    public static Port portViridianForestBottomToViridianForest = new Port(16, 17, 48);

    public static Port portViridianForestToViridianForestBottom = new Port(15, 6, 1);
    public static Port portViridianForestToVidianForestTop = new Port(17, 5, 7);

    public static Port portViridianForestTopToViridianForest = new Port(16, 2, 1);
    public static Port portViridianForestTopToRoute2 = new Port(14, 4, 12);

    public static Port portPewterToRoute2 = new Port(14, 9, 2);
    public static Port portPewterToPewterPokeCenter = new Port(19, 4, 8);
    public static Port portPewterToPewterPokeMart = new Port(20, 4, 7);
    public static Port portPewterToGym = new Port(21, 5, 14);
    public static Port portPewterToPHouse1 = new Port(22, 3, 7);
    public static Port portPewterToPHouse2 = new Port(23, 3, 7);
    public static Port portPewterToMuseumBottom = new Port(24, 11, 7);

    public static Port portPewterPokeCenterToPewter = new Port(18, 10, 25);

    public static Port portPewterPokeMartToPewter = new Port(18, 20, 17);

    public static Port portGymToPewter = new Port(18, 13, 17);

    public static Port portPHouse1ToPewter = new Port(18, 4, 29);

    public static Port portPHouse2ToPewter = new Port(18, 26, 13);

    public static Port portMuseumBottomToPewter = new Port(18, 11, 7);
    public static Port portMuseumBottomToMuseumTop = new Port(25, 7, 7);

    public static Port portMuseumTopToMuseumBottom = new Port(24, 7, 7);

    public static List<(int pk, int[] lvlRange, int rate)> encounterTable1 = new List<(int pk, int[] lvlRange, int rate)>
    {
        (15, new int[] { 2, 5 }, 50),
        (18, new int[] { 2, 4 }, 50)
    };
    public static List<(int pk, int[] lvlRange, int rate)> encounterTable2 = new List<(int pk, int[] lvlRange, int rate)>
    {
        (15, new int[] { 3, 5 }, 40),
        (18, new int[] { 2, 5 }, 40),
        (9, new int[] { 3, 5 }, 10),
        (12, new int[] { 3, 5 }, 10)
    };
    public static List<(int pk, int[] lvlRange, int rate)> encounterTable22 = new List<(int pk, int[] lvlRange, int rate)>
    {
        (18, new int[] { 2, 4 }, 20),
        (20, new int[] { 3, 5 }, 20),
        (24, new int[] {3, 4 }, 30),
        (26, new int[] {3, 4 }, 30)
    };
    public static List<(int pk, int[] lvlRange, int rate)> encounterTableV = new List<(int pk, int[] lvlRange, int rate)>
    {
        (9, new int[] { 3, 5 }, 25),
        (10, new int[] { 4, 6 }, 20),
        (12, new int[] { 3, 5 }, 30),
        (13, new int[] { 4, 6 }, 20),
        (22, new int[] { 3, 5 }, 5),
    };

    public static Map AshTop = new Map(1, levelData1, 4, 5, new List<Port> { portAshTopToBottom }, null);
    public static Map AshBottom = new Map(2, levelData2, new List<Port> { portAshBottomToTop, portAshToPallet }, null);
    public static Map PalletTown = new Map(3, levelData3, 5, 8, new List<Port> { portPalletToAsh, portPalletToDaisy, portPalletToLab, portPalletToRoute1 }, null);
    public static Map Lab = new Map(4, levelData4, new List<Port> { portLabToPallet }, null);
    public static Map Daisy = new Map(5, levelData5, 4, 6, new List<Port> { portDaisyToPallet }, null);
    public static Map Route1 = new Map(6, levelData6, 7, 35, new List<Port> { portRoute1ToPallet, portRoute1ToViridian }, encounterTable1);
    public static Map Viridian = new Map(7, levelData7, 17, 2, new List<Port> { portViridianToRoute1, portViridianToRoute22, portViridianToPokeCenter, portViridianToPokeMart, portViridianToVHouse1, portViridianToVHouse2, portViridianToRoute2 }, null);
    public static Map PokeCenter = new Map(8, levelData8, new List<Port> { portPokeCenterToViridian }, null);
    public static Map PokeMart = new Map(9, levelData9, new List<Port> { portPokeMartToViridian }, null);
    public static Map VHouse1 = new Map(10, levelData10, new List<Port> { portVHouse1ToViridian }, null);
    public static Map Vhouse2 = new Map(11, levelData11, new List<Port> { portVhouse2ToViridian }, null);
    public static Map Route22 = new Map(12, levelData12, new List<Port> { portRoute22ToViridian, portRoute22ToVictoryRoadCheck }, encounterTable22);
    public static Map VictoryRoadCheck = new Map(13, levelData13, new List<Port> { portVictoryRoadCheckToRoute22 }, null);
    public static Map Route2 = new Map(14, levelData14, 4, 12, new List<Port> { portRoute2ToViridian, portRoute2ToViridianForestBottom, portRoute2ToViridianForestTop, portRoute2ToPewter }, encounterTable2);
    public static Map ViridianForestBottom = new Map(15, levelData15, new List<Port> { portViridianForestBottomToRoute2, portViridianForestBottomToViridianForest }, null);
    public static Map ViridianForest = new Map(16, levelData16, 17, 48,  new List<Port> { portViridianForestToViridianForestBottom, portViridianForestToVidianForestTop }, encounterTableV);
    public static Map ViridianForestTop = new Map(17, levelData17, new List<Port> { portViridianForestTopToViridianForest, portViridianForestTopToRoute2 }, null);
    public static Map Pewter = new Map(18, levelData18, 11, 7, new List<Port> { portPewterToRoute2, portPewterToPewterPokeCenter, portPewterToPewterPokeMart, portPewterToGym, portPewterToPHouse1, portPewterToPHouse2, portPewterToMuseumBottom }, null);
    public static Map PewterPokeCenter = new Map(19, levelData19, new List<Port> { portPewterPokeCenterToPewter }, null);
    public static Map PewterPokeMart = new Map(20, levelData20, new List<Port> { portPewterPokeMartToPewter }, null);
    public static Map Gym = new Map(21, levelData21, 6, 12, new List<Port> { portGymToPewter }, null);
    public static Map PHouse1 = new Map(22, levelData22, new List<Port> { portPHouse1ToPewter }, null);
    public static Map PHouse2 = new Map(23, levelData23, new List<Port> { portPHouse2ToPewter }, null);
    public static Map MuseumBottom = new Map(24, levelData24, new List<Port> { portMuseumBottomToPewter, portMuseumBottomToMuseumTop }, null);
    public static Map MuseumTop = new Map(25, levelData25, new List<Port> { portMuseumTopToMuseumBottom }, null);

    public static List<Map> mapList = new List<Map>
    {
        Loading, AshTop, AshBottom, PalletTown, Lab, Daisy, Route1, Viridian,
        PokeCenter, PokeMart, VHouse1, Vhouse2, Route22, VictoryRoadCheck,
        Route2, ViridianForestBottom, ViridianForest, ViridianForestTop, Pewter,
        PewterPokeCenter, PewterPokeMart, Gym, PHouse1, PHouse2, MuseumBottom, MuseumTop
    };
    #endregion
    public static readonly Species[] AllPokemon = {
        new Species("Bulbasaur", (Type)5, (Type)8, 45, 49, 49, 65, 45, 87,
            45, 64, true, new int[] {0,0,0,1,0},
            new List<(int,int)> {
                (1,14), (1,9), (7,4), (13,25), (20,26), (27,23), (34,24), (41,27)
            }),
        new Species("Ivysaur", (Type)5, (Type)8, 60, 62, 63, 80, 60, 87,
            45, 141, true, new int[] {0,0,0,1,1},
            new List<(int,int)> {
                (1,14), (1,9), (1,4), (13,25), (22,26), (30,23), (38,24), (46,27)
            }),
        new Species("Venusaur", (Type)5, (Type)8, 80, 82, 83, 100, 80, 87,
            45, 208, true, new int[] {0,0,0,2,1},
            new List<(int,int)> {
                (1,14), (1,9), (1,4), (13,25), (22,26), (30,23), (43,24), (55,27)
            }),
        new Species("Charmander", (Type)2, (Type)0, 39, 52, 43, 60, 65, 87,
            45, 62, false, new int[] {0,0,0,0,1},
            new List<(int,int)> {
                (1,0), (1,12), (9,16), (15,51), (30,17), (38,29)
            }),
        new Species("Charmeleon", (Type)2, (Type)0, 58, 64, 58, 80, 80, 87,
            45, 142, false, new int[] {0,0,0,1,1},
            new List<(int,int)> {
                (1,0), (1,12), (1,16), (15,51), (31,17), (42,29)
            }),
        new Species("Charizard", (Type)2, (Type)10, 78, 84, 78, 109, 100, 87,
            45, 209, false, new int[] {0,0,0,3,0},
            new List<(int,int)> {
                (1,0), (1,12), (1,16), (15,51), (36,17), (46,29)
            }),
        new Species("Squirtle", (Type)3, (Type)0, 44, 48, 65, 50, 43, 87,
            45, 63, false, new int[] {0,0,1,0,0},
            new List<(int,int)> {
                (1,9), (1,10), (8,18), (15,45), (42,19)
            }),
        new Species("Wartortle", (Type)3, (Type)0, 59, 63, 80, 65, 58, 87,
            45, 142, false, new int[] {0,0,1,1,0},
            new List<(int,int)> {
                (1,9), (1,10), (1,18), (15,45), (47,19)
            }),
        new Species("Blastoise", (Type)3, (Type)0, 79, 83, 100, 85, 78, 87,
            45, 210, false, new int[] {0,0,3,0,0},
            new List<(int,int)> {
                (1,9), (1,10), (1,18), (15,45), (52,19)
            }),
        new Species("Caterpie", (Type)12, (Type)0, 45, 30, 35, 20, 45, 50,
            255, 53, false, new int[] {1,0,0,0,0},
            new List<(int,int)> {
                (1,9), (1,28)
            }),
        new Species("Metapod", (Type)12, (Type)0, 50, 20, 55, 25, 30, 50,
            120, 72, false, new int[] {0,0,2,0,0},
            new List<(int,int)> {
                (1,38)
            }),
        new Species("Butterfree", (Type)12, (Type)10, 60, 45, 50, 90, 70, 50,
            45, 160, false, new int[] {0,0,0,2,1},
            new List<(int,int)> {
                (1,15), (10,33), (13,20)
            }),
        new Species("Weedle", (Type)12, (Type)8, 40, 35, 30, 20, 50, 50,
            255, 52, false, new int[] {0,1,0,0,0},
            new List<(int,int)> {
                (1,11), (1,28)
            }),
        new Species("Kakuna", (Type)12, (Type)8, 45, 25, 50, 25, 35, 50,
            120, 71, false, new int[] {0,0,2,0,0},
            new List<(int,int)> {
                (1,38)
            }),
        new Species("Beedrill", (Type)12, (Type)8, 65, 90, 40, 45, 75, 50,
            45, 159, false, new int[] {0,2,0,0,0},
            new List<(int,int)> {
                (1,11), (1,38)
            }),
        new Species("Pidgey", (Type)1, (Type)10, 40, 45, 40, 35, 56, 50,
            255, 55, false, new int[] {0,0,0,0,1},
            new List<(int,int)> {
                (1,9), (5,12), (12,1), (19,2)
            }),
        new Species("Pidgeotto", (Type)1, (Type)10, 63, 60, 55, 50, 71, 50,
            120, 113, false, new int[] {0,0,0,0,2},
            new List<(int,int)> {
                (1,9), (1,12), (1,1), (21,2)
            }),
        new Species("Pidgeot", (Type)1, (Type)10, 83, 80, 75, 70, 101, 50,
            45, 172, false, new int[] {0,0,0,0,3},
            new List<(int,int)> {
                (1,9), (1,12), (1,1), (21,2)
            }),
        new Species("Rattata", (Type)1, (Type)0, 30, 56, 35, 25, 72, 50,
            255, 57, false, new int[] {0,0,0,0,1},
            new List<(int,int)> {
                (1,9), (1,14), (7,37), (14,13)
            }),
        new Species("Raticate", (Type)1, (Type)0, 55, 81, 60, 50, 97, 50,
            127, 116, false, new int[] {0,0,0,0,2},
            new List<(int,int)> {
                (1,9), (1,14), (1,37), (14,13)
            }),
        new Species("Spearow", (Type)1, (Type)10, 40, 60, 30, 31, 70, 50,
            255, 58, false, new int[] {0,1,0,0,0},
            new List<(int,int)> {
                (1,21), (1,12), (9,7), (15,22)
            }),
        new Species("Fearow", (Type)1, (Type)10, 65, 90, 65, 61, 100, 50,
            90, 162, false, new int[] {0,2,0,0,0},
            new List<(int,int)> {
                (1,21), (1,12), (1,7), (15,22)
            }),
        new Species("Pikachu", (Type)4, (Type)0, 35, 55, 40, 50, 90, 50,
            190, 82, false, new int[] {0,0,0,0,2},
            new List<(int,int)> {
                (1,31), (1,14), (9,36), (26,32), (43,33)
            }),
        new Species("Sandshrew", (Type)9, (Type)0, 50, 75, 85, 20, 40, 50,
            255, 93, false, new int[] {0,0,1,0,0},
            new List<(int,int)> {
                (1,38), (10,6), (17,45)
            }),
        new Species("Nidoran F", (Type)8, (Type)0, 55, 47, 52, 40, 41, 0,
            235, 59, false, new int[] {1,0,0,0,0},
            new List<(int,int)> {
                (1,11), (8,6), (14,26)
            }),
        new Species("Nidorina", (Type)8, (Type)0, 70, 62, 67, 55, 56, 0,
            120, 117, false, new int[] {0,0,2,0,0},
            new List<(int,int)> {
                (1,11), (1,6), (14,26)
            }),
        new Species("Nidoran M", (Type)8, (Type)0, 46, 57, 40, 40, 50, 100,
            235, 60, false, new int[] {0,1,0,0,0},
            new List<(int,int)> {
                (1,11), (8,6), (14,7)
            }),
        new Species("Nidorino", (Type)8, (Type)0, 61, 72, 57, 55, 65, 100,
            120, 118, false, new int[] {0,2,0,0,0},
            new List<(int,int)> {
                (1,11), (1,6), (14,7)
            }),
        new Species("Diglett", (Type)9, (Type)0, 10, 55, 25, 35, 95, 50,
            255, 81, false, new int[] {0,0,0,0,1},
            new List<(int,int)> {
                (1,6), (15,51)
            }),
        new Species("Geodude", (Type)13, (Type)9, 40, 80, 100, 30, 20, 50,
            255, 86, false, new int[] {0,0,1,0,0},
            new List<(int,int)> {
                (1,9), (1,38), (11,45)
            }),
        new Species("Onix", (Type)13, (Type)9, 35, 45, 160, 30, 70, 50,
            45, 77, false, new int[] {0,0,1,0,0},
            new List<(int,int)> {
                (1,9), (1,38), (15,45)
            })
};
    public static readonly MoveB[] AllMoves = {
        new MoveB("Scratch", Type.Normal, 40, 100, Split.Physical, 35, new List<MoveEffect>()),
        new MoveB("Gust", Type.Flying, 40, 100, Split.Physical, 35, new List<MoveEffect>()),
        new MoveB("Wing Attack", Type.Flying, 60, 100, Split.Physical, 35, new List<MoveEffect>()),
        new MoveB("Whirlwind", Type.Normal, 0, 101, Split.Status, 20, new List<MoveEffect>()),
        new MoveB("Vine Whip", Type.Grass, 45, 100, Split.Special, 25, new List<MoveEffect>()),
        new MoveB("Double Kick", Type.Fighting, 30, 100, Split.Physical, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.None, 0, 0)
        }),
        new MoveB("Sand Attack", Type.Ground, 0, 100, Split.Status, 15, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Acc, 100, -1)
        }),
        new MoveB("Horn Attack", Type.Normal, 65, 100, Split.Physical, 25, new List<MoveEffect>()),
        new MoveB("Horn Drill", Type.Normal, -1, 30, Split.Physical, 5, new List<MoveEffect>()),
        new MoveB("Tackle", Type.Normal, 40, 100, Split.Physical, 35, new List<MoveEffect>()),
        new MoveB("Tail Whip", Type.Normal, 0, 100, Split.Status, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, -1)
        }),
        new MoveB("Poison Sting", Type.Poison, 15, 100, Split.Physical, 35, new List<MoveEffect>
        {
            new MoveEffect(Status.Poison, Stat.None, 30, 0)
        }),
        new MoveB("Leer", Type.Normal, 0, 100, Split.Status, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, -1)
        }),
        new MoveB("Bite", Type.Normal, 60, 100, Split.Physical, 25, new List<MoveEffect>
        {
            new MoveEffect(Status.Flinch, Stat.None, 30, 0)
        }),
        new MoveB("Growl", Type.Normal, 0, 100, Split.Status, 40, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Atk, 100, -1)
        }),
        new MoveB("Supersonic", Type.Normal, 0, 55, Split.Status, 20, new List<MoveEffect>
        {
            new MoveEffect(Status.Confusion, Stat.None, 100, 0)
        }),
        new MoveB("Ember", Type.Fire, 40, 100, Split.Special, 25, new List<MoveEffect>
        {
            new MoveEffect(Status.Burn, Stat.None, 10, 0)
        }),
        new MoveB("Flamethrower", Type.Fire, 90, 100, Split.Special, 15, new List<MoveEffect>
        {
            new MoveEffect(Status.Burn, Stat.None, 10, 0)
        }),
        new MoveB("Water Gun", Type.Water, 40, 100, Split.Special, 25, new List<MoveEffect>()),
        new MoveB("Hydro Pump", Type.Water, 110, 80, Split.Special, 5, new List<MoveEffect>()),
        new MoveB("Psybeam", Type.Psychic, 65, 100, Split.Special, 20, new List<MoveEffect>
        {
            new MoveEffect(Status.Confusion, Stat.None, 10, 0)
        }),
        new MoveB("Peck", Type.Flying, 35, 100, Split.Physical, 35, new List<MoveEffect>()),
        new MoveB("Drill Peck", Type.Flying, 80, 100, Split.Physical, 20, new List<MoveEffect>()),
        new MoveB("Growth", Type.Normal, 0, 101, Split.Status, 20, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Atk, 100, 1),
            new MoveEffect(Status.None, Stat.Spa, 100, 1)
        }),
        new MoveB("Razor Leaf", Type.Grass, 55, 95, Split.Special, 25, new List<MoveEffect>()),
        new MoveB("Poison Powder", Type.Poison, 0, 75, Split.Status, 35, new List<MoveEffect>
        {
            new MoveEffect(Status.Poison, Stat.None, 100, 0)
        }),
        new MoveB("Stun Spore", Type.Grass, 0, 75, Split.Status, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.Paralysis, Stat.None, 100, 0)
        }),
        new MoveB("Sleep Powder", Type.Grass, 0, 75, Split.Status, 15, new List<MoveEffect>
        {
            new MoveEffect(Status.Sleep, Stat.None, 100, 0)
        }),
        new MoveB("String Shot", Type.Bug, 0, 95, Split.Status, 40, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Spe, 100, -2)
        }),
        new MoveB("Fire Spin", Type.Fire, 35, 85, Split.Special, 15, new List<MoveEffect>
        {
            new MoveEffect(Status.EscapePrevent, Stat.None, 100, 0),
            new MoveEffect(Status.Bound, Stat.None, 100, 0)
        }),
        new MoveB("Thunder Shock", Type.Electric, 40, 100, Split.Special, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.Paralysis, Stat.None, 10, 0)
        }),
        new MoveB("Thunder Wave", Type.Electric, 0, 90, Split.Status, 20, new List<MoveEffect>
        {
            new MoveEffect(Status.Paralysis, Stat.None, 100, 0)
        }),
        new MoveB("Thunder", Type.Electric, 110, 70, Split.Special, 10, new List<MoveEffect>
        {
            new MoveEffect(Status.Paralysis, Stat.None, 10, 0)
        }),
        new MoveB("Confusion", Type.Psychic, 50, 100, Split.Special, 25, new List<MoveEffect>
        {
            new MoveEffect(Status.Confusion, Stat.None, 10, 0)
        }),
        new MoveB("Agility", Type.Psychic, 0, 101, Split.Status, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Spe, 100, 2)
        }),
        new MoveB("Quick Attack", Type.Normal, 40, 100, Split.Physical, 30, new List<MoveEffect>()),
        new MoveB("Screech", Type.Normal, 0, 85, Split.Status, 40, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, -2)
        }),
        new MoveB("Harden", Type.Normal, 0, 101, Split.Status, 30, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, 1)
        }),
        new MoveB("Withdraw", Type.Water, 0, 101, Split.Status, 40, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, 1)
        }),
        new MoveB("Defense Curl", Type.Normal, 0, 101, Split.Status, 40, new List<MoveEffect>
        {
            new MoveEffect(Status.None, Stat.Def, 100, 1)
        }),
        new MoveB("Swift", Type.Normal, 60, 101, Split.Physical, 20, new List<MoveEffect>()),
        new MoveB("Skull Bash", Type.Normal, 130, 100, Split.Physical, 10, new List<MoveEffect>()),
        new MoveB("Super Fang", Type.Normal, 0, 90, Split.Status, 10, new List<MoveEffect>()),
        new MoveB("Slash", Type.Normal, 70, 100, Split.Physical, 20, new List<MoveEffect>())
    };
    public static Player Intro()
    {
        Console.WriteLine("  Welcome to Pokemon Arduino!");
        Console.WriteLine("             Start");
        Console.ReadKey();
        Console.Clear();
        Console.WriteLine("Welcome to the wonderful world of Pokemon! v");
        Console.ReadKey();
        Console.Clear();
        string name = "Ash";
        while (true)
        {
            Console.Write("What is your name: ");
            name = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"So your name is {name}?");
            Console.WriteLine("Yes");
            Console.WriteLine("No");
            string keyInfo = Console.ReadKey().Key.ToString().ToLower();
            if (keyInfo == "y" || keyInfo == "enter")
            {
                break;
            }
            Console.Clear();
        }
        Console.Clear();
        Console.WriteLine("Your journey begins now! v");
        Console.ReadKey();
        Console.Clear();
        Thread.Sleep(500);
        return new Player(name);
    }
    public static void Main()
    {
        Player playa = Intro();
        Map currentMap = mapList[1];
        currentMap.UpdateMap(playa, -1);
        int direction = 0;
        while (true)
        {
            Thread.Sleep(100);
            ConsoleKey key = ConsoleKey.NoName;
            while (Console.KeyAvailable)
            {
                key = Console.ReadKey(true).Key;
            }

            if (key == ConsoleKey.NoName) continue;

            var input = key;
            direction = playa.direction;
            bool move = true;

            switch (input)
            {
                case ConsoleKey.UpArrow: direction = 4; break;
                case ConsoleKey.LeftArrow: direction = 2; break;
                case ConsoleKey.DownArrow: direction = 3; break;
                case ConsoleKey.RightArrow: direction = 1; break;
                case ConsoleKey.Escape: return;
                case ConsoleKey.Z: move = false; break;
                case ConsoleKey.X: move = false; break;
                case ConsoleKey.D: move = false; break;
            }

            if (move)
            {
                currentMap = currentMap.MovePlayer(direction, playa);
                currentMap.UpdateMap(playa, -1);
            }
            else if (input == ConsoleKey.Z)
            {
                currentMap.Interact(playa);
            }
            else if (input == ConsoleKey.D)
            {
                playa.Menu();
            }

            playa.direction = direction;
        }
    }
}
