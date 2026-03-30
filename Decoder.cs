using System.Buffers.Text;
using System.Data;
using System.Numerics;
using System.Security.Cryptography;
namespace PokeC_Decoder
{
    internal class Program
    {
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
    "awggwaagggww        waaaaw        wa",
    "awggwaagggww        waaaaw        wa",
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
    "aaaaaaa  gggggggg  gwaaaawg       wa",
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
        public static string[][] maps = new string[][]
        {
            levelData1,
            levelData2,
            levelData3,
            levelData4,
            levelData5,
            levelData6,
            levelData7,
            levelData8,
            levelData9,
            levelData10,
            levelData11,
            levelData12,
            levelData13,
            levelData14,
            levelData15,
            levelData16,
            levelData17,
            levelData18,
            levelData19,
            levelData20,
            levelData21,
            levelData22,
            levelData23,
            levelData24,
            levelData25
        };
        public static string Encode(string[] map)
        {
            string result = "";
            result += map.Length;
            result += ',';
            result += map[0].Length;
            result += ',';
            char[] charSet = new char[] { };
            foreach (string line in map)
            {
                foreach (char c in line)
                {
                    if (!charSet.Contains(c))
                    {
                        if (c == ' ') continue;
                        result += c;
                        charSet = charSet.Append(c).ToArray();
                    }
                }
            }
            result += ',';
            
            foreach (char c in charSet)
            {
                bool[] charMap = new bool[map.Length * map[0].Length];
                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < map[0].Length; j++)
                    {
                        if (map[i][j] == c)
                        {
                            charMap[i * map[0].Length + j] = true;
                        }
                    }
                }
                BigInteger value = 0;

                for (int i = 0; i < charMap.Length; i++)
                {
                    if (charMap[i])
                    {
                        value |= (BigInteger.One << i);
                    }
                }
                result += ToBase64(value);
                result += ',';
            }
            result = result.TrimEnd(',');
            return result;
        }
        public static string ToBase64(BigInteger value)
        {
            byte[] bytes = value.ToByteArray();
            return Convert.ToBase64String(bytes);
        }
        public static string[] Decode(string encoded)
        {
            string[] parts = encoded.Split(',');

            int height = int.Parse(parts[0]);
            int width = int.Parse(parts[1]);
            string charSet = parts[2];

            string[] result = new string[height];

            // Initialize map with spaces
            for (int i = 0; i < height; i++)
            {
                result[i] = new string(' ', width);
            }

            for (int sIndex = 0; sIndex < charSet.Length; sIndex++)
            {
                string base64 = parts[3 + sIndex];
                char c = charSet[sIndex];

                byte[] bytes = Convert.FromBase64String(base64);
                BigInteger value = new BigInteger(bytes, isUnsigned: true, isBigEndian: false);

                int bitCount = width * height;

                int index = 0;
                for (int y = 0; y < height; y++)
                {
                    char[] line = result[y].ToCharArray();

                    for (int x = 0; x < width; x++)
                    {
                        if (((value >> index) & BigInteger.One) == BigInteger.One)
                        {
                            line[x] = c;
                        }
                        index++;
                    }

                    result[y] = new string(line);
                }
            }

            return result;
        }
        static void Main(string[] args)
        {
            foreach (string[] map in maps)
           {
                string encoded = Encode(map);
                Console.WriteLine(encoded);
                string[] decoded = Decode(encoded);
                foreach (string line in decoded)
                {
                    Console.WriteLine(line);
                }
            }      
        }
    }
}
