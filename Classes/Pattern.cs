using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ACSE.Classes.Utilities;

namespace ACSE
{
    /// <summary>
    /// GameCube Pattern Data Write-up
    /// Patterns consist of a 15-color palette.
    /// There are 16 palettes to select from, but you can only use one palette at a time.
    /// To save space, the AC Devs chose to use the C4 image format. This format uses a palette map with each nibble equating to a pixel. FF = White, White | 1F = Red, White
    /// The blocks are 4 bytes (8 pixels) wide by 8 bytes deep (so each block is 32 bytes [0x20 bytes])!
    /// Each Pattern is 32x32 pixels.
    /// Pattern Structure:
    ///     [String] Name: 0x10 bytes
    ///     [Byte] Palette: 0x1 bytes
    ///     [Byte Array] Alignment Bytes?: 0xF bytes
    ///     [Byte Array] Pattern Data: 0x200 bytes
    /// </summary>
    class PatternData
    {
        #region Palettes

        public static uint[][] AC_Palette_Data = new uint[16][]
        {
            new uint[15]
            {
                0xFFCD4A4A, 0xFFDE8341, 0xFFE6AC18, 0xFFE6C520, 0xFFD5DE18, 0xFFB4E618, 0xFF83D552, 0xFF39C56A, 0xFF29ACC5, 0xFF417BEE, 0xFF6A4AE6, 0xFF945ACD, 0xFFBD41B4, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFFFF8B8B, 0xFFFFCD83, 0xFFFFE65A, 0xFFFFF662, 0xFFFFFF83, 0xFFDEFF52, 0xFFB4FF83, 0xFF7BF6AC, 0xFF62E6F6, 0xFF83C5FF, 0xFFA49CFF, 0xFFD59CFF, 0xFFFF9CF6, 0xFF8B8B8B, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF9C1818, 0xFFAC5208, 0xFFB47B00, 0xFFB49400, 0xFFA4AC00, 0xFF83B400, 0xFF52A431, 0xFF089439, 0xFF007B94, 0xFF104ABD, 0xFF3918AC, 0xFF5A2994, 0xFF8B087B, 0xFF080808, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF41945A, 0xFF73C58B, 0xFF94E6AC, 0xFF008B7B, 0xFF5AB4AC, 0xFF83C5C5, 0xFF2073A4, 0xFF4A9CCD, 0xFF6AACDE, 0xFF7383BD, 0xFF6A73AC, 0xFF525294, 0xFF39397B, 0xFF181862, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF9C8352, 0xFFBD945A, 0xFFD5BD83, 0xFF9C5252, 0xFFCD7362, 0xFFEE9C8B, 0xFF8B6283, 0xFFA483B4, 0xFFDEB4DE, 0xFFBD8383, 0xFFAC736A, 0xFF945252, 0xFF7B3939, 0xFF621810, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFFEE5A00, 0xFFFF9C41, 0xFFFFCD83, 0xFFFFEEA4, 0xFF8B4A29, 0xFFB47B5A, 0xFFE6AC8B, 0xFFFFDEBD, 0xFF318BFF, 0xFF62B4FF, 0xFF9CDEFF, 0xFFC5E6FF, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF39B441, 0xFF62DE5A, 0xFF8BEE83, 0xFFB4FFAC, 0xFF2020C5, 0xFF5252F6, 0xFF8383FF, 0xFFB4B4FF, 0xFFCD3939, 0xFFDE6A6A, 0xFFE68B9C, 0xFFEEBDBD, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF082000, 0xFF415A39, 0xFF6A8362, 0xFF9CB494, 0xFF5A2900, 0xFF7B4A20, 0xFFA4734A, 0xFFD5A47B, 0xFF947B00, 0xFFB49439, 0xFFCDB46A, 0xFFDED59C, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF2020FF, 0xFFFF2020, 0xFFD5D500, 0xFF6262FF, 0xFFFF6262, 0xFFD5D562, 0xFF9494FF, 0xFFFF9494, 0xFFD5D594, 0xFFACACFF, 0xFFFFACAC, 0xFFE6E6AC, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFF20A420, 0xFF39ACFF, 0xFF9C52EE, 0xFF52BD52, 0xFF5AC5FF, 0xFFB49CFF, 0xFF6AD573, 0xFF8BE6FF, 0xFFCDB4FF, 0xFF94DEAC, 0xFFBDF6FF, 0xFFD5CDFF, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFFD50000, 0xFFFFBD00, 0xFFEEF631, 0xFF4ACD41, 0xFF299C29, 0xFF528BBD, 0xFF414AAC, 0xFF9452D5, 0xFFF67BDE, 0xFFA49439, 0xFF9C4141, 0xFF5A3139, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new uint[15]
            {
                0xFFE6CD18, 0xFF20C518, 0xFFFF6A00, 0xFF0000FF, 0xFF9400BD, 0xFFE6CD18, 0xFF00A400, 0xFFCD4100, 0xFF0000D5, 0xFF5A008B, 0xFF9C8B18, 0xFF008300, 0xFFA42000, 0xFF0000A4, 0xFF4A005A
            },
            new uint[15]
            {
                0xFFFF2020, 0xFFE6D500, 0xFFF639BD, 0xFF00D59C, 0xFF107310, 0xFFC52020, 0xFFBDA400, 0xFFCD3994, 0xFF009C6A, 0xFF204A20, 0xFF8B2020, 0xFF836A00, 0xFF941862, 0xFF00734A, 0xFF183918
            },
            new uint[15]
            {
                0xFFEED5D5, 0xFFDEC5C5, 0xFFCDB4B4, 0xFFBDA4A4, 0xFFAC9494, 0xFF9C8383, 0xFF8B7373, 0xFF7B6262, 0xFF6A5252, 0xFF5A4141, 0xFF4A3131, 0xFF392020, 0xFF291010, 0xFF180000, 0xFF100000
            },
            new uint[15]
            {
                0xFFEEEEEE, 0xFFDEDEDE, 0xFFCDCDCD, 0xFFBDBDBD, 0xFFACACAC, 0xFF9C9C9C, 0xFF8B8B8B, 0xFF7B7B7B, 0xFF6A6A6A, 0xFF5A5A5A, 0xFF4A4A4A, 0xFF393939, 0xFF292929, 0xFF181818, 0xFF101010
            },
            new uint[15]
            {
                0xFFEE7B7B, 0xFFD51818, 0xFFF69418, 0xFFE6E652, 0xFF006A00, 0xFF39B439, 0xFF0039B4, 0xFF399CFF, 0xFF940094, 0xFFFF6AFF, 0xFF944108, 0xFFEE9C5A, 0xFFFFC594, 0xFF000000, 0xFFFFFFFF
            },
        };

        public static byte[] WW_Pattern_Offsets = new byte[16]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0xA, 0xC, 0xB, 0xD, 0xE, 0xF //Not in order??
        };

        public static uint[][] WW_Palette_Data = new uint[16][]
        {
            new uint [15]
            {
                0xFFFF0000, 0xFFFF7331, 0xFFFFAD00, 0xFFFFFF00, 0xFFADFF00, 0xFF52FF00, 0xFF00FF00, 0xFF00AD52, 0xFF0052AD, 0xFF0000FF, 0xFF5200FF, 0xFFAD00FF, 0xFFFF00FF, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFFF7B7B, 0xFFFFB57B, 0xFFFFE77B, 0xFFFFFF7B, 0xFFDEFF7B, 0xFFADFF7B, 0xFF7BFF7B, 0xFF52AD84, 0xFF5284AD, 0xFF7B7BFF, 0xFFB57BFF, 0xFFE77BFF, 0xFFFF7BFF, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFA50000, 0xFFA53100, 0xFFA57300, 0xFFA5A500, 0xFF73A500, 0xFF31A500, 0xFF00A500, 0xFF005221, 0xFF002152, 0xFF0000A5, 0xFF3100A5, 0xFF7300A5, 0xFFA500A5, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF009C00, 0xFF5ACE6B, 0xFFB5FFDE, 0xFF009C6B, 0xFF52CEA5, 0xFFADFFD6, 0xFF0052AD, 0xFF2984D6, 0xFF5AADFF, 0xFF0000FF, 0xFF4A6BFF, 0xFF314ADE, 0xFF1821B5, 0xFF00008C, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFAD7300, 0xFFD6AD42, 0xFFFFDE8C, 0xFFFF0839, 0xFFFF4A6B, 0xFFFF949C, 0xFFAD00FF, 0xFFD663FF, 0xFFFFCEFF, 0xFFFFBD9C, 0xFFDE9473, 0xFFBD634A, 0xFF9C3921, 0xFF7B1000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFFF0000, 0xFFFF5200, 0xFFFFB55A, 0xFFFFEFAD, 0xFF7B1000, 0xFFA54A31, 0xFFD6846B, 0xFFFFBD9C, 0xFF5AADFF, 0xFF84C6FF, 0xFFADE7FF, 0xFFD6FFFF, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF00FF00, 0xFF42FF42, 0xFF8CFF8C, 0xFFD6FFD6, 0xFF0000FF, 0xFF4242FF, 0xFF8C8CFF, 0xFFD6D6FF, 0xFFFF0000, 0xFFFF4242, 0xFFFF8C8C, 0xFFFFD6D6, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF003100, 0xFF426342, 0xFF849C84, 0xFFC6D6C6, 0xFF7B1000, 0xFFA54A29, 0xFFD68C5A, 0xFFFFC68C, 0xFFD6B500, 0xFFE7CE39, 0xFFF7DE7B, 0xFFFFF7BD, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF0000FF, 0xFFFF0000, 0xFFFFFF00, 0xFF4242FF, 0xFFFF4242, 0xFFFFFF42, 0xFF8C8CFF, 0xFFFF8C8C, 0xFFFFFF8C, 0xFFD6D6FF, 0xFFFFD6D6, 0xFFFFFFD6, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF00FF00, 0xFF0000FF, 0xFFFF00FF, 0xFF42FF42, 0xFF4242FF, 0xFFFF42FF, 0xFF8CFF8C, 0xFF8C8CFF, 0xFFFF8CFF, 0xFFD6FFD6, 0xFFD6D6FF, 0xFFFFD6FF, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFFF0000, 0xFFFF7B00, 0xFFFFFF00, 0xFF84FF00, 0xFF00FF00, 0xFF00847B, 0xFF0000FF, 0xFF7B00FF, 0xFFFF94FF, 0xFFD6B500, 0xFFBD1000, 0xFF5A1000, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFF109463, 0xFF087B52, 0xFF108C39, 0xFF319C31, 0xFFCEA54A, 0xFFCE9439, 0xFFBD8C4A, 0xFFD68C31, 0xFFAD734A, 0xFF8C5A31, 0xFF6B4229, 0xFF84EFFF, 0xFF31CEEF, 0xFF00A5C6, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFD6DEE7, 0xFFB5CEDE, 0xFFE7EFEF, 0xFFF7F7F7, 0xFF84737B, 0xFF948C6B, 0xFF847B63, 0xFF9C845A, 0xFF739CB5, 0xFFFF2929, 0xFFFFFF00, 0xFF9421FF, 0xFF009CBD, 0xFF000000, 0xFFFFFFFF
            },
            new uint [15]
            {
                0xFFFFFFFF, 0xFFF7EFEF, 0xFFE7DEDE, 0xFFD6CECE, 0xFFC6B5B5, 0xFFB5A5A5, 0xFFA59494, 0xFF9C8484, 0xFF8C6B6B, 0xFF7B5A5A, 0xFF6B4A4A, 0xFF5A3131, 0xFF4A2121, 0xFF421010, 0xFF310000
            },
            new uint [15]
            {
                0xFFFFFFFF, 0xFFEFEFEF, 0xFFDEDEDE, 0xFFCECECE, 0xFFB5B5B5, 0xFFA5A5A5, 0xFF949494, 0xFF848484, 0xFF6B6B6B, 0xFF5A5A5A, 0xFF4A4A4A, 0xFF313131, 0xFF212121, 0xFF101010, 0xFF000000
            },
            new uint [15]
            {
                0xFFFF8C7B, 0xFFFF0000, 0xFFFF7B00, 0xFFFFFF00, 0xFF008400, 0xFF00FF00, 0xFF0000FF, 0xFF009CFF, 0xFFD600FF, 0xFFFF6BFF, 0xFF9C0000, 0xFFFF9400, 0xFFFFBD94, 0xFF000000, 0xFFFFFFFF
            },
        }; 

        public static uint[][] CF_Palette_Data = new uint[16][]
        {
            new uint [15]
            {
                0xFFFF0000, 0xFF008000, 0xFF0000FF, 0xFF804000, 0xFF000000, 0xFFFF8000, 0xFF80FF00, 0xFF00EAEA, 0xFF800080, 0xFF808080, 0xFFFFCAE4, 0xFFFFFF00, 0xFFACCCFD, 0xFFFF66B3, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFFFD561E, 0xFF00C400, 0xFF3982FB, 0xFFAC6802, 0xFF000000, 0xFFFFA54A, 0xFF9FFF40, 0xFF51A8FF, 0xFFEE70FC, 0xFF808080, 0xFFFFC993, 0xFFFFFF80, 0xFFC1E0FF, 0xFFFFC4E1, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF804040, 0xFF006633, 0xFF4B4BA9, 0xFF4B2F21, 0xFF000000, 0xFFAC6802, 0xFF00A854, 0xFF317BF7, 0xFF5F3E86, 0xFF969696, 0xFFDFB9AC, 0xFFA4A400, 0xFF9CBACF, 0xFFB06FB0, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFFB02902, 0xFFA29F4F, 0xFF008282, 0xFF022B68, 0xFF000000, 0xFFFA7403, 0xFFE6C947, 0xFF04C67D, 0xFF5A5ACD, 0xFF808080, 0xFFFED7AF, 0xFFFFFFA8, 0xFFC4EBC2, 0xFF94B4FE, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFFD05B9C, 0xFFEAB602, 0xFF2291FF, 0xFF8000FF, 0xFF000000, 0xFFFF9FCF, 0xFFF4F400, 0xFF7EC8FE, 0xFFB376C7, 0xFF808080, 0xFFEECCCC, 0xFFFFFFB0, 0xFFC1F4F3, 0xFFE7CFEF, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF400040, 0xFF424505, 0xFF006262, 0xFF2D0059, 0xFF000000, 0xFF981F5F, 0xFF9D6B4D, 0xFF377D95, 0xFF7143BC, 0xFF808080, 0xFFEC95C5, 0xFFCEC68A, 0xFF80AECA, 0xFFB56AFF, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF710071, 0xFF950000, 0xFF438507, 0xFF005984, 0xFF000000, 0xFFD9006C, 0xFFFF8000, 0xFF66CC00, 0xFF049788, 0xFF808080, 0xFFFF93FF, 0xFFF7BA02, 0xFFDCD838, 0xFF00FFFF, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF6D6D36, 0xFFA4A4A4, 0xFF371C00, 0xFF002D00, 0xFF000000, 0xFFAD8301, 0xFFB4B4B4, 0xFF5C3F23, 0xFF23673A, 0xFF808080, 0xFFD2CC5B, 0xFFE1E1E1, 0xFFB08364, 0xFF66938A, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF804040, 0xFF666666, 0xFF003737, 0xFF83745C, 0xFF000000, 0xFFB3757E, 0xFFC3C3C3, 0xFF233641, 0xFFA39E65, 0xFF808080, 0xFFD7A8A8, 0xFFD0CDB9, 0xFF5F5F6D, 0xFFC9BE98, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFFAB5436, 0xFF3D140A, 0xFF6B3F8F, 0xFF793431, 0xFF000000, 0xFFD69769, 0xFF6A390D, 0xFF5050AF, 0xFFD1423F, 0xFF808080, 0xFFFABEBE, 0xFFCBAB69, 0xFF286249, 0xFFFF8B17, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF800080, 0xFF800040, 0xFFB35900, 0xFF8C8C00, 0xFF000000, 0xFFDF00DF, 0xFFE4013A, 0xFFFF8000, 0xFFFFFF00, 0xFF808080, 0xFFFF8CFF, 0xFFD68585, 0xFFFECF78, 0xFFFFFF80, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF4E009B, 0xFF0000A0, 0xFF538BB5, 0xFF008040, 0xFF000000, 0xFF8000FF, 0xFF5F22FD, 0xFF58CFFC, 0xFF029D5B, 0xFF84846F, 0xFFD896FC, 0xFF9595FF, 0xFF93FFFF, 0xFF43FEA5, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF9E8B49, 0xFF8B96A7, 0xFFB78786, 0xFF432121, 0xFF000000, 0xFFBF7E3E, 0xFFAFB0BC, 0xFFDDC9BF, 0xFF562945, 0xFF5B5B5B, 0xFFBBA85B, 0xFFB1C1D1, 0xFFE0CFA9, 0xFF8A5353, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFF003535, 0xFF00974B, 0xFF487776, 0xFFF5F5F5, 0xFF8C4600, 0xFF00773C, 0xFF02B78E, 0xFF95B5BF, 0xFFEAF1F2, 0xFFCF7625, 0xFF02B08D, 0xFF64AB03, 0xFFE1E1E1, 0xFFFFFFFF, 0xFFE6AD59
            },

            new uint [15]
            {
                0xFFA5ABBC, 0xFF002828, 0xFFCBA0BE, 0xFF392837, 0xFF000000, 0xFFBDC8D2, 0xFF44526A, 0xFFDBB0CF, 0xFF593349, 0xFF808080, 0xFFC6DDDD, 0xFF687D93, 0xFFE9C7C7, 0xFF8D6376, 0xFFFFFFFF
            },

            new uint [15]
            {
                0xFFBDA98A, 0xFF222200, 0xFFBCBCBC, 0xFF232332, 0xFF000000, 0xFFE6D6BD, 0xFF5C5C2E, 0xFFD2D7DB, 0xFF424D55, 0xFF808080, 0xFFEFE7E7, 0xFF7B7C56, 0xFFE9EAED, 0xFF6B6B6B, 0xFFFFFFFF
            },
        };

        //Should probably convert other palette data to be like this, and just do palette * 16 + color_idx to find the color
        public static uint[] NL_Palette_Data = new uint[256]
        {
            0xFFFFEFFF, 0xFFFF9AAD, 0xFFEF559C, 0xFFFF65AD, 0xFFFF0063, 0xFFBD4573, 0xFFCE0052, 0xFF9C0031, 0xFF522031, 0xFF000009, 0xFF00000A, 0xFF00000B, 0xFF00000C, 0xFF00000D, 0xFF00000E, 0xFFFFFFFF,
            0xFFFFBACE, 0xFFFF7573, 0xFFDE3010, 0xFFFF5542, 0xFFFF0000, 0xFFCE6563, 0xFFBD4542, 0xFFBD0000, 0xFF8C2021, 0xFF000019, 0xFF00001A, 0xFF00001B, 0xFF00001C, 0xFF00001D, 0xFF00001E, 0xFFECECEC,
            0xFFDECFBD, 0xFFFFCF63, 0xFFDE6521, 0xFFFFAA21, 0xFFFF6500, 0xFFBD8A52, 0xFFDE4500, 0xFFBD4500, 0xFF633010, 0xFF000029, 0xFF00002A, 0xFF00002B, 0xFF00002C, 0xFF00002D, 0xFF00002E, 0xFFDADADA,
            0xFFFFEFDE, 0xFFFFDFCE, 0xFFFFCFAD, 0xFFFFBA8C, 0xFFFFAA8C, 0xFFDE8A63, 0xFFBD6542, 0xFF9C5531, 0xFF8C4521, 0xFF000039, 0xFF00003A, 0xFF00003B, 0xFF00003C, 0xFF00003D, 0xFF00003E, 0xFFC8C8C8,
            0xFFFFCFFF, 0xFFEF8AFF, 0xFFCE65DE, 0xFFBD8ACE, 0xFFCE00FF, 0xFF9C659C, 0xFF8C00AD, 0xFF520073, 0xFF310042, 0xFF000049, 0xFF00004A, 0xFF00004B, 0xFF00004C, 0xFF00004D, 0xFF00004E, 0xFFB6B6B6,
            0xFFFFBAFF, 0xFFFF9AFF, 0xFFDE20BD, 0xFFFF55EF, 0xFFFF00CE, 0xFF8C5573, 0xFFBD009C, 0xFF8C0063, 0xFF520042, 0xFF000059, 0xFF00005A, 0xFF00005B, 0xFF00005C, 0xFF00005D, 0xFF00005E, 0xFFA3A3A3,
            0xFFDEBA9C, 0xFFCEAA73, 0xFF734531, 0xFFAD7542, 0xFF9C3000, 0xFF733021, 0xFF522000, 0xFF311000, 0xFF211000, 0xFF000069, 0xFF00006A, 0xFF00006B, 0xFF00006C, 0xFF00006D, 0xFF00006E, 0xFF919191,
            0xFFFFFFCE, 0xFFFFFF73, 0xFFDEDF21, 0xFFFFFF00, 0xFFFFDF00, 0xFFCEAA00, 0xFF9C9A00, 0xFF8C7500, 0xFF525500, 0xFF000079, 0xFF00007A, 0xFF00007B, 0xFF00007C, 0xFF00007D, 0xFF00007E, 0xFF7F7F7F,
            0xFFDEBAFF, 0xFFBD9AEF, 0xFF6330CE, 0xFF9C55FF, 0xFF6300FF, 0xFF52458C, 0xFF42009C, 0xFF210063, 0xFF211031, 0xFF000089, 0xFF00008A, 0xFF00008B, 0xFF00008C, 0xFF00008D, 0xFF00008E, 0xFF6D6D6D,
            0xFFBDBAFF, 0xFF8C9AFF, 0xFF3130AD, 0xFF3155EF, 0xFF0000FF, 0xFF31308C, 0xFF0000AD, 0xFF101063, 0xFF000021, 0xFF000099, 0xFF00009A, 0xFF00009B, 0xFF00009C, 0xFF00009D, 0xFF00009E, 0xFF5B5B5B,
            0xFF9CEFBD, 0xFF63CF73, 0xFF216510, 0xFF42AA31, 0xFF008A31, 0xFF527552, 0xFF215500, 0xFF103021, 0xFF002010, 0xFF0000A9, 0xFF0000AA, 0xFF0000AB, 0xFF0000AC, 0xFF0000AD, 0xFF0000AE, 0xFF484848,
            0xFFDEFFBD, 0xFFCEFF8C, 0xFF8CAA52, 0xFFADDF8C, 0xFF8CFF00, 0xFFADBA9C, 0xFF63BA00, 0xFF529A00, 0xFF316500, 0xFF0000B9, 0xFF0000BA, 0xFF0000BB, 0xFF0000BC, 0xFF0000BD, 0xFF0000BE, 0xFF363636,
            0xFFBDDFFF, 0xFF73CFFF, 0xFF31559C, 0xFF639AFF, 0xFF1075FF, 0xFF4275AD, 0xFF214573, 0xFF002073, 0xFF001042, 0xFF0000C9, 0xFF0000CA, 0xFF0000CB, 0xFF0000CC, 0xFF0000CD, 0xFF0000CE, 0xFF242424,
            0xFFADFFFF, 0xFF52FFFF, 0xFF008ABD, 0xFF52BACE, 0xFF00CFFF, 0xFF429AAD, 0xFF00658C, 0xFF004552, 0xFF002031, 0xFF0000D9, 0xFF0000DA, 0xFF0000DB, 0xFF0000DC, 0xFF0000DD, 0xFF0000DE, 0xFF121212,
            0xFFCEFFEF, 0xFFADEFDE, 0xFF31CFAD, 0xFF52EFBD, 0xFF00FFCE, 0xFF73AAAD, 0xFF00AA9C, 0xFF008A73, 0xFF004531, 0xFF0000E9, 0xFF0000EA, 0xFF0000EB, 0xFF0000EC, 0xFF0000ED, 0xFF0000EE, 0xFF000000,
            0xFFADFFAD, 0xFF73FF73, 0xFF63DF42, 0xFF00FF00, 0xFF21DF21, 0xFF52BA52, 0xFF00BA00, 0xFF008A00, 0xFF214521, 0xFF0000F9, 0xFF0000FA, 0xFF0000FB, 0xFF0000FC, 0xFF0000FD, 0xFF0000FE, 0xFF0000FF
        };

        #endregion

        private static Color ColorFromUInt32(uint color)
        {
            return Color.FromArgb(0xFF, (byte)(color >> 16), (byte)(color >> 8), (byte)(color));
        }

        public static byte ClosestColorHSV(uint color, uint[] paletteData)
        {
            uint closestColor = paletteData[0];
            double diff = double.MaxValue;
            Color c = ColorFromUInt32(color);
            float targetHue = c.GetHue();
            float targetSat = c.GetSaturation();
            float targetBri = c.GetBrightness();

            foreach (uint validColor in paletteData)
            {
                Color checkColor = ColorFromUInt32(validColor);
                float currentHue = checkColor.GetHue();
                float currentSat = checkColor.GetSaturation();
                float currentBri = checkColor.GetBrightness();

                double currentDiff = Math.Pow(targetHue - currentHue, 2) + Math.Pow(targetSat - currentSat, 2) + Math.Pow(targetBri - currentBri, 2);

                if (currentDiff < diff)
                {
                    diff = currentDiff;
                    closestColor = validColor;
                }
            }

            return (byte)(Array.IndexOf(paletteData, closestColor) + 1);
        }

        public static byte ClosestColorRGB(uint Color, uint[] PaletteData)
        {
            double Distance = double.MaxValue;
            byte ClosestPaletteIndex = 0;
            double R = Color & 0xFF;
            double G = (Color >> 8) & 0xFF;
            double B = (Color >> 16) & 0xFF;

            for (int i = 0; i < PaletteData.Length; i++)
            {
                uint PaletteColor = PaletteData[i];
                double pR = PaletteColor & 0xFF;
                double pG = (PaletteColor >> 8) & 0xFF;
                double pB = (PaletteColor >> 16) & 0xFF;

                double ThisDistance = Math.Sqrt(Math.Pow(pR - R, 2) + Math.Pow(pG - G, 2) + Math.Pow(pB - B, 2));
                if (ThisDistance == 0)
                {
                    // Perfect match
                    return (byte)(i + 1);
                }
                else if (ThisDistance < Distance)
                {
                    Distance = ThisDistance;
                    ClosestPaletteIndex = (byte)(i + 1);
                }
            }

            return ClosestPaletteIndex;
        }
    }

    public class Pattern
    {
        private Save Save_File;
        private int Offset = 0;
        public int Index;
        public byte[] patternBitmapBuffer = new byte[4 * 32 * 32];
        public byte[] DecodedData;
        public string Name;
        public string CreatorName;
        public string TownName;
        public byte Palette;
        public byte Concept;
        public Bitmap Pattern_Bitmap;
        public uint[] PaletteData;

        public Pattern(int patternOffset, int index, Save save = null)
        {
            Offset = patternOffset;
            Save_File = save;
            Read(index);
        }

        public uint[][] GetPaletteArray(SaveGeneration Save_Generation)
        {
            switch (Save_Generation)
            {
                case SaveGeneration.GCN:
                default:
                    return PatternData.AC_Palette_Data;
                case SaveGeneration.NDS:
                    return PatternData.WW_Palette_Data;
                case SaveGeneration.Wii:
                    return PatternData.CF_Palette_Data;
            }
        }

        // AC / CF
        public void GeneratePatternBitmap(byte[] Import_Data = null, bool Decode = true)
        {
            byte[] patternRawData = Import_Data ?? (Save_File.Save_Type == SaveType.City_Folk ? Save_File.ReadByteArray(Offset, 0x200) : Save_File.ReadByteArray(Offset + 0x20, 0x200));
            uint[][] Palette_Data = Save_File.Save_Type == SaveType.City_Folk ? PatternData.CF_Palette_Data : PatternData.AC_Palette_Data;

            if (Decode)
            {
                DecodedData = PatternUtility.DecodeC4(patternRawData);
            }

            Pattern_Bitmap = PatternUtility.C4PaletteMapToBitmap(DecodedData, Palette_Data[Palette], 32, 32);
        }

        public void GenerateWWPatternBitmap(byte[] Import_Data = null, bool Decode = true)
        {
            byte[] Raw_Data = Import_Data ?? Save_File.ReadByteArray(Offset, 0x200);

            if (Decode)
            {
                DecodedData = new byte[0x400];

                for (int i = 0; i < 0x200; i++)
                {
                    DecodedData[i * 2] = (byte)(Raw_Data[i] & 0x0F);  //Left is Right
                    DecodedData[i * 2 + 1] = (byte)((Raw_Data[i] & 0xF0) >> 4); //Right is Left
                }
            }

            uint[] Palette_Array = PatternData.WW_Palette_Data[Palette];
            
            for (int i = 0; i < 0x400; i++)
            {
                int color_idx = Math.Max(0, DecodedData[i] - 1);
                Buffer.BlockCopy(BitConverter.GetBytes(Palette_Array[color_idx]), 0, patternBitmapBuffer, i * 4, 4);
            }
            Pattern_Bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = Pattern_Bitmap.LockBits(new Rectangle(0, 0, 32, 32), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(patternBitmapBuffer, 0, bitmapData.Scan0, patternBitmapBuffer.Length);
            Pattern_Bitmap.UnlockBits(bitmapData);
        }

        //NL Patterns have a custom palette, created by the user by choosing 15 colors
        public void GenerateNLPatternBitmap(byte[] Import_Data = null, bool Decode = true)
        {
            //Add decoding of "Pro" patterns
            byte[] Raw_Data = Import_Data ?? Save_File.ReadByteArray(Offset + 0x6C, 0x200); //32x32 doubled up pixels
            byte[] Custom_Palette = Save_File.ReadByteArray(Offset + 0x58, 15); //New Leaf user selected palette data
            PaletteData = new uint[15];

            // Generate Palatte Data
            for (int i = 0; i < 15; i++)
                PaletteData[i] = PatternData.NL_Palette_Data[Custom_Palette[i]];

            if (Decode)
            {
                //Expand data for working with

                DecodedData = new byte[0x400]; //32x32 expanded pixel buffer
                for (int i = 0; i < 0x200; i++)
                {
                    DecodedData[i * 2] = (byte)(Raw_Data[i] & 0x0F);
                    DecodedData[i * 2 + 1] = (byte)((Raw_Data[i] >> 4) & 0x0F);
                }
            }

            //Convert palette color index to argb color
            for (int i = 0; i < 0x400; i++)
                Buffer.BlockCopy(BitConverter.GetBytes(PaletteData[DecodedData[i]]), 0, patternBitmapBuffer, i * 4, 4);
            
            //Create new bitmap
            Pattern_Bitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = Pattern_Bitmap.LockBits(new Rectangle(0, 0, 32, 32), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(patternBitmapBuffer, 0, bitmapData.Scan0, patternBitmapBuffer.Length);
            Pattern_Bitmap.UnlockBits(bitmapData);
        }

        public void Read(int Index)
        {
            this.Index = Index;
            if (Save_File.Save_Type == SaveType.Animal_Crossing)
            {
                Name = new ACString(Save_File.ReadByteArray(Offset, 0x10), Save_File.Save_Type).Trim();
                Palette = Save_File.ReadByte(Offset + 0x10);
                PaletteData = PatternData.AC_Palette_Data[Palette];
                GeneratePatternBitmap();
            }
            else if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus || Save_File.Save_Type == SaveType.Doubutsu_no_Mori
                || Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus)
            {
                Name = new ACString(Save_File.ReadByteArray(Offset, 0xA), Save_File.Save_Type).Trim();
                Palette = Save_File.ReadByte(Offset + 0xA);
                PaletteData = PatternData.AC_Palette_Data[Palette];
                GeneratePatternBitmap();
            }
            else if (Save_File.Save_Type == SaveType.Wild_World)
            {
                TownName = new ACString(Save_File.ReadByteArray(Offset + 0x202, 8), SaveType.Wild_World).Trim();
                CreatorName = new ACString(Save_File.ReadByteArray(Offset + 0x20C, 8), SaveType.Wild_World).Trim();
                Name = new ACString(Save_File.ReadByteArray(Offset + 0x216, 16), SaveType.Wild_World).Trim();
                Palette = (byte)((Save_File.ReadByte(Offset + 0x226) & 0xF0) >> 4);
                Concept = (byte)(Save_File.ReadByte(Offset + 0x226) & 0x0F);
                PaletteData = PatternData.WW_Palette_Data[Palette];
                GenerateWWPatternBitmap();
            }
            else if (Save_File.Save_Type == SaveType.City_Folk)
            {
                TownName = new ACString(Save_File.ReadByteArray(Offset + 0x822, 16), SaveType.City_Folk).Trim();
                CreatorName = new ACString(Save_File.ReadByteArray(Offset + 0x838, 16), SaveType.City_Folk).Trim();
                Name = new ACString(Save_File.ReadByteArray(Offset + 0x84C, 32), SaveType.City_Folk).Trim();
                Palette = Save_File.ReadByte(Offset + 0x86F);
                PaletteData = PatternData.CF_Palette_Data[Palette];
                GeneratePatternBitmap();
            }
            else if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
            {
                Name = new ACString(Save_File.ReadByteArray(Offset, 0x2A), SaveType.New_Leaf).Trim();
                CreatorName = new ACString(Save_File.ReadByteArray(Offset + 0x2C, 0x14), SaveType.New_Leaf).Trim();
                TownName = new ACString(Save_File.ReadByteArray(Offset + 0x42, 0x14), SaveType.New_Leaf).Trim();
                //No specific palette in NL/WA
                GenerateNLPatternBitmap();
            }
            else
                MessageBox.Show("Patterns: Unknown Save Type");
        }

        public void RedrawBitmap()
        {
            switch (Save_File.Save_Generation)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    GeneratePatternBitmap(DecodedData, false);
                    Write(PatternUtility.EncodeC4(DecodedData, 32, 32));
                    break;
                case SaveGeneration.NDS:
                    GenerateWWPatternBitmap(DecodedData, false);
                    Write(PatternUtility.CondenseNonBlockPattern(DecodedData));
                    break;
                case SaveGeneration.N3DS:
                    GenerateNLPatternBitmap(DecodedData, false);
                    Write(PatternUtility.CondenseNonBlockPattern(DecodedData));
                    break;
            }
        }

        public void Import(uint[] Bitmap_Buffer)
        {
            // Convert to nibble map array of bytes
            byte[] Pattern_Buffer = new byte[Bitmap_Buffer.Length / 2];

            if (Save_File.Save_Generation == SaveGeneration.NDS || Save_File.Save_Generation == SaveGeneration.N3DS)
            {
                for (int i = 0; i < Pattern_Buffer.Length; i++)
                {
                    int idx = i * 2;
                    Pattern_Buffer[i] = (byte)((PatternData.ClosestColorRGB(Bitmap_Buffer[idx + 1], PaletteData) << 4) | PatternData.ClosestColorRGB(Bitmap_Buffer[idx], PaletteData)); // these are reversed
                }
            }
            else
            {
                byte[] ConvertedBuffer = new byte[Bitmap_Buffer.Length];
                for (int i = 0; i < ConvertedBuffer.Length; i++)
                {
                    ConvertedBuffer[i] = PatternData.ClosestColorRGB(Bitmap_Buffer[i], PaletteData);
                }
                Pattern_Buffer = PatternUtility.EncodeC4(ConvertedBuffer);
            }

            switch (Save_File.Save_Generation)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    GeneratePatternBitmap(Pattern_Buffer);
                    break;
                case SaveGeneration.NDS:
                    GenerateWWPatternBitmap(Pattern_Buffer);
                    break;
                case SaveGeneration.N3DS:
                    GenerateNLPatternBitmap(Pattern_Buffer);
                    break;
            }

            Write(Pattern_Buffer);
        }

        public void Write(byte[] New_Pattern_Data)
        {
            if (Save_File.Save_Generation == SaveGeneration.GCN)
            {
                int PatternNameSize = Save_File.Save_Type == SaveType.Animal_Crossing ? 0x10 : 0x0A;
                Save_File.Write(Offset, ACString.GetBytes(Name, PatternNameSize));
                Save_File.Write(Offset + PatternNameSize, Palette);
                Save_File.Write(Offset + 0x20, New_Pattern_Data);
            }
            else if (Save_File.Save_Type == SaveType.Wild_World)
            {
                Save_File.Write(Offset, New_Pattern_Data);
                // TODO: Town Name & Creator Name (Also for City Folk, New Leaf)
                Save_File.Write(Offset + 0x216, ACString.GetBytes(Name, 0x10));
                Save_File.Write(Offset + 0x226, (byte)(((Palette & 0x0F) << 4) | (Concept & 0x0F)));
            }
            else if (Save_File.Save_Type == SaveType.City_Folk)
            {
                Save_File.Write(Offset, New_Pattern_Data);
                Save_File.Write(Offset + 0x84C, ACString.GetBytes(Name, 0x20));
                Save_File.Write(Offset + 0x86F, Palette);
            }
            else if (Save_File.Save_Generation == SaveGeneration.N3DS)
            {
                Save_File.Write(Offset, ACString.GetBytes(Name, 0x2A));
                Save_File.Write(Offset + 0x6C, New_Pattern_Data);
                // TODO: Write Palette (since it's customizable)
            }
        }
    }
}
