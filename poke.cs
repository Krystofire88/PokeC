using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

public class Player
{
    public int direction = 4;

    public void Menu()
    {
        string[] options = { "Trainer", "Bag", "Pokemon" };
        int selectedIndex = 0;

        while (true)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{(i == selectedIndex ? ">" : " ")}  {options[i]}");
            }
            Console.WriteLine();

            var move = Console.ReadKey().Key;

            if (move == ConsoleKey.S)
                selectedIndex = (selectedIndex + 1) % options.Length;
            else if (move == ConsoleKey.W)
                selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;

            for (int i = 10 * 2 + 1; i < 10 * 1 + 8; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(new string(' ', 20));
            }
            Console.SetCursorPosition(0, 10 * 2 + 1);
        }
    }
}

public class NPC { }

public class Port
{
    public int mapID { get; }
    public int playerX { get; }
    public int playerY { get; }
    private bool[] triggers = { false, false, false, false, false };

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

    private int viewXSize = 10;
    private int viewYSize = 10;

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
    private string machineSprite = "§§";
    private string compSprite = "PC";

    public Map(int mapID, string[] map, int playerX, int playerY, List<Port> ports)
    {
        this.mapID = mapID;
        this.map = map;
        this.playerX = playerX;
        this.playerY = playerY;
        this.ports = ports;
    }

    public Map(int mapID, string[] map, List<Port> ports)
    {
        this.mapID = mapID;
        this.map = map;
        this.ports = ports;
    }

    public void UpdateMap()
    {
        Console.Clear();
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
                        else if (tile == 'n') line += npcSprite;
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
    }

    public static void Loading()
    {
        Console.Clear();
        for (int viewY = 0; viewY < 20; viewY++)
        {
            string line = "";
            for (int viewX = 0; viewX < 20; viewX++)
                line += "xx";
            Console.WriteLine(line);
        }
    }

    public Map MovePlayer(int dir)
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
        char tile = map[newY][newX];

        if (passable.Contains(tile))
        {
            playerX = newX;
            playerY = newY;
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

        char[] interactable = new char[] { 'n', 'P', 'j' };
        char tile = map[newY][newX];

        if (interactable.Contains(tile))
        {
            switch (tile)
            {
                case 'n':
                    Console.WriteLine("negr");
                    break;
                case 'P':
                    break;
                case 'j':
                    break;
                default:
                    return;
            }
        }
        else return;
    }
}

class Program
{
    public static string[] levelData = {
        "w"
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
        "w          w",
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
        "1                  pppp  ppwwwwpp  w",
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
    public static string[] levelData12 = levelData;

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

    public static string[] levelData16 = levelData;

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
        "ww        ww",
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

    public static Map Loading = new Map(0, levelData, null);

    public static Port portAshTopToBottom = new Port(2, 7, 1);

    public static Port portAshBottomToTop = new Port(1, 7, 1);
    public static Port portAshToPallet = new Port(3, 5, 8);

    public static Port portPalletToAsh = new Port(2, 4, 6);
    public static Port portPalletToDaisy = new Port(5, 4, 6);
    public static Port portPalletToLab = new Port(4, 4, 9);
    public static Port portPalletToRoute1 = new Port(6, 7, 35);

    public static Port portDaisyToPallet = new Port(3, 13, 8);

    public static Port portLabToPallet = new Port(3, 12, 14);

    public static Port portRoute1ToPallet = new Port(3, 10, 1);
    public static Port portRoute1ToViridian = new Port(7, 19, 32);

    public static Port portViridianToRoute1 = new Port(6, 7, 2);
    public static Port portViridianToPokeCenter = new Port(8, 4, 8);
    public static Port portViridianToPokeMart = new Port(9, 4, 9);
    public static Port portViridianToVHouse1 = new Port(10, 4, 6);
    public static Port portViridianToVHouse2 = new Port(11, 4, 6);
    public static Port portViridianToRoute22 = new Port(12, 7, 2);
    public static Port portViridianToRoute2 = new Port(14, 9, 73);

    public static Port portPokeCenterToViridian = new Port(7, 7, 2);

    public static Port portPokeMartToViridian = new Port(7, 7, 2);

    public static Port portVHouse1ToViridian = new Port(7, 7, 2);

    public static Port portVhouse2ToViridian = new Port(7, 7, 2);

    public static Port portRoute22ToViridian = new Port(7, 7, 2);
    public static Port portRoute22ToVictoryRoadCheck = new Port(13, 7, 2);

    public static Port portVictoryRoadCheckToRoute22 = new Port(12, 7, 2);

    public static Port portRoute2ToViridian = new Port(7, 17, 2);
    public static Port portRoute2ToViridianForestBottom = new Port(15, 12, 14);
    public static Port portRoute2ToViridianForestTop = new Port(17, 12, 14);
    public static Port portRoute2ToPewter = new Port(18, 12, 14);

    public static Port portViridianForestBottomToRoute2 = new Port(14, 12, 14);
    public static Port portViridianForestBottomToViridianForest = new Port(16, 12, 14);

    public static Port portViridianForestToViridianForestBottom = new Port(15, 12, 14);
    public static Port portViridianForestToVidianForestTop = new Port(17, 12, 14);

    public static Port portViridianForestTopToViridianForest = new Port(16, 12, 14);
    public static Port portViridianForestTopToRoute2 = new Port(14, 12, 14);

    public static Port portPewterToRoute2 = new Port(14, 12, 14);
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

    public static Map AshTop = new Map(1, levelData1, 4, 5, new List<Port> { portAshTopToBottom });
    public static Map AshBottom = new Map(2, levelData2, new List<Port> { portAshBottomToTop, portAshToPallet });
    public static Map PalletTown = new Map(3, levelData3, 5, 8, new List<Port> { portPalletToAsh, portPalletToDaisy, portPalletToLab, portPalletToRoute1 });
    public static Map Lab = new Map(4, levelData4, new List<Port> { portLabToPallet });
    public static Map Daisy = new Map(5, levelData5, 4, 6, new List<Port> { portDaisyToPallet });
    public static Map Route1 = new Map(6, levelData6, 7, 35, new List<Port> { portRoute1ToPallet, portRoute1ToViridian });
    public static Map Viridian = new Map(7, levelData7, 19, 32, new List<Port> { portViridianToRoute1, portViridianToRoute22, portViridianToPokeCenter, portViridianToPokeMart, portViridianToVHouse1, portViridianToVHouse2, portViridianToRoute2 });
    public static Map PokeCenter = new Map(8, levelData8, new List<Port> { portPokeCenterToViridian });
    public static Map PokeMart = new Map(9, levelData9, new List<Port> { portPokeMartToViridian });
    public static Map VHouse1 = new Map(10, levelData10, new List<Port> { portVHouse1ToViridian });
    public static Map Vhouse2 = new Map(11, levelData11, new List<Port> { portVhouse2ToViridian });
    public static Map Route22 = new Map(12, levelData12, new List<Port> { portRoute22ToViridian, portRoute22ToVictoryRoadCheck });
    public static Map VictoryRoadCheck = new Map(13, levelData13, new List<Port> { portVictoryRoadCheckToRoute22 });
    public static Map Route2 = new Map(14, levelData14, 9, 73, new List<Port> { portRoute2ToViridian, portRoute2ToViridianForestBottom, portRoute2ToViridianForestTop, portRoute2ToPewter });
    public static Map ViridianForestBottom = new Map(15, levelData15, new List<Port> { portViridianForestBottomToRoute2, portViridianForestBottomToViridianForest });
    public static Map ViridianForest = new Map(16, levelData16, new List<Port> { portViridianForestToViridianForestBottom, portViridianForestToVidianForestTop });
    public static Map ViridianForestTop = new Map(17, levelData17, new List<Port> { portViridianForestTopToViridianForest, portViridianForestTopToRoute2 });
    public static Map Pewter = new Map(18, levelData18, 11, 7, new List<Port> { portPewterToRoute2, portPewterToPewterPokeCenter, portPewterToPewterPokeMart, portPewterToGym, portPewterToPHouse1, portPewterToPHouse2, portPewterToMuseumBottom });
    public static Map PewterPokeCenter = new Map(19, levelData19, new List<Port> { portPewterPokeCenterToPewter });
    public static Map PewterPokeMart = new Map(20, levelData20, new List<Port> { portPewterPokeMartToPewter });
    public static Map Gym = new Map(21, levelData21, new List<Port> { portGymToPewter });
    public static Map PHouse1 = new Map(22, levelData22, new List<Port> { portPHouse1ToPewter });
    public static Map PHouse2 = new Map(23, levelData23, new List<Port> { portPHouse2ToPewter });
    public static Map MuseumBottom = new Map(24, levelData24, new List<Port> { portMuseumBottomToPewter, portMuseumBottomToMuseumTop });
    public static Map MuseumTop = new Map(25, levelData25, new List<Port> { portMuseumTopToMuseumBottom });

    public static List<Map> mapList = new List<Map>
    {
        Loading, AshTop, AshBottom, PalletTown, Lab, Daisy, Route1, Viridian,
        PokeCenter, PokeMart, VHouse1, Vhouse2, Route22, VictoryRoadCheck,
        Route2, ViridianForestBottom, ViridianForest, ViridianForestTop, Pewter,
        PewterPokeCenter, PewterPokeMart, Gym, PHouse1, PHouse2, MuseumBottom, MuseumTop
    };

    static void Main()
    {
        Map currentMap = mapList[14];
        currentMap.UpdateMap();
        int direction = 0;
        Player playa = new Player();

        while (true)
        {
            Thread.Sleep(100);
            var input = Console.ReadKey().Key;
            direction = playa.direction;
            bool move = true;

            switch (input)
            {
                case ConsoleKey.W: direction = 4; break;
                case ConsoleKey.A: direction = 2; break;
                case ConsoleKey.S: direction = 3; break;
                case ConsoleKey.D: direction = 1; break;
                case ConsoleKey.Escape: return;
                case ConsoleKey.Z: move = false; break;
                case ConsoleKey.X: move = false; break;
            }

            if (move)
            {
                currentMap = currentMap.MovePlayer(direction);
                currentMap.UpdateMap();
            }
            else if (input == ConsoleKey.Z)
            {
                currentMap.Interact(playa);
            }
            else if (input == ConsoleKey.X)
            {
                playa.Menu();
            }

            playa.direction = direction;
        }
    }
}