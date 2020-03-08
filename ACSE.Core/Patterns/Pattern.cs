using System;
using System.Drawing;
using System.Drawing.Imaging;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Patterns
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
    public static class PatternData
    {
        #region Palettes

        public static readonly uint[][] AcPaletteData = {
            new[]
            {
                0xFFCD4A4A, 0xFFDE8341, 0xFFE6AC18, 0xFFE6C520, 0xFFD5DE18, 0xFFB4E618, 0xFF83D552, 0xFF39C56A, 0xFF29ACC5, 0xFF417BEE, 0xFF6A4AE6, 0xFF945ACD, 0xFFBD41B4, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFFF8B8B, 0xFFFFCD83, 0xFFFFE65A, 0xFFFFF662, 0xFFFFFF83, 0xFFDEFF52, 0xFFB4FF83, 0xFF7BF6AC, 0xFF62E6F6, 0xFF83C5FF, 0xFFA49CFF, 0xFFD59CFF, 0xFFFF9CF6, 0xFF8B8B8B, 0xFFFFFFFF
            },
            new[]
            {
                0xFF9C1818, 0xFFAC5208, 0xFFB47B00, 0xFFB49400, 0xFFA4AC00, 0xFF83B400, 0xFF52A431, 0xFF089439, 0xFF007B94, 0xFF104ABD, 0xFF3918AC, 0xFF5A2994, 0xFF8B087B, 0xFF080808, 0xFFFFFFFF
            },
            new[]
            {
                0xFF41945A, 0xFF73C58B, 0xFF94E6AC, 0xFF008B7B, 0xFF5AB4AC, 0xFF83C5C5, 0xFF2073A4, 0xFF4A9CCD, 0xFF6AACDE, 0xFF7383BD, 0xFF6A73AC, 0xFF525294, 0xFF39397B, 0xFF181862, 0xFFFFFFFF
            },
            new[]
            {
                0xFF9C8352, 0xFFBD945A, 0xFFD5BD83, 0xFF9C5252, 0xFFCD7362, 0xFFEE9C8B, 0xFF8B6283, 0xFFA483B4, 0xFFDEB4DE, 0xFFBD8383, 0xFFAC736A, 0xFF945252, 0xFF7B3939, 0xFF621810, 0xFFFFFFFF
            },
            new[]
            {
                0xFFEE5A00, 0xFFFF9C41, 0xFFFFCD83, 0xFFFFEEA4, 0xFF8B4A29, 0xFFB47B5A, 0xFFE6AC8B, 0xFFFFDEBD, 0xFF318BFF, 0xFF62B4FF, 0xFF9CDEFF, 0xFFC5E6FF, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF39B441, 0xFF62DE5A, 0xFF8BEE83, 0xFFB4FFAC, 0xFF2020C5, 0xFF5252F6, 0xFF8383FF, 0xFFB4B4FF, 0xFFCD3939, 0xFFDE6A6A, 0xFFE68B9C, 0xFFEEBDBD, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF082000, 0xFF415A39, 0xFF6A8362, 0xFF9CB494, 0xFF5A2900, 0xFF7B4A20, 0xFFA4734A, 0xFFD5A47B, 0xFF947B00, 0xFFB49439, 0xFFCDB46A, 0xFFDED59C, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF2020FF, 0xFFFF2020, 0xFFD5D500, 0xFF6262FF, 0xFFFF6262, 0xFFD5D562, 0xFF9494FF, 0xFFFF9494, 0xFFD5D594, 0xFFACACFF, 0xFFFFACAC, 0xFFE6E6AC, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF20A420, 0xFF39ACFF, 0xFF9C52EE, 0xFF52BD52, 0xFF5AC5FF, 0xFFB49CFF, 0xFF6AD573, 0xFF8BE6FF, 0xFFCDB4FF, 0xFF94DEAC, 0xFFBDF6FF, 0xFFD5CDFF, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFD50000, 0xFFFFBD00, 0xFFEEF631, 0xFF4ACD41, 0xFF299C29, 0xFF528BBD, 0xFF414AAC, 0xFF9452D5, 0xFFF67BDE, 0xFFA49439, 0xFF9C4141, 0xFF5A3139, 0xFF6A6A6A, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFE6CD18, 0xFF20C518, 0xFFFF6A00, 0xFF0000FF, 0xFF9400BD, 0xFFE6CD18, 0xFF00A400, 0xFFCD4100, 0xFF0000D5, 0xFF5A008B, 0xFF9C8B18, 0xFF008300, 0xFFA42000, 0xFF0000A4, 0xFF4A005A
            },
            new[]
            {
                0xFFFF2020, 0xFFE6D500, 0xFFF639BD, 0xFF00D59C, 0xFF107310, 0xFFC52020, 0xFFBDA400, 0xFFCD3994, 0xFF009C6A, 0xFF204A20, 0xFF8B2020, 0xFF836A00, 0xFF941862, 0xFF00734A, 0xFF183918
            },
            new[]
            {
                0xFFEED5D5, 0xFFDEC5C5, 0xFFCDB4B4, 0xFFBDA4A4, 0xFFAC9494, 0xFF9C8383, 0xFF8B7373, 0xFF7B6262, 0xFF6A5252, 0xFF5A4141, 0xFF4A3131, 0xFF392020, 0xFF291010, 0xFF180000, 0xFF100000
            },
            new[]
            {
                0xFFEEEEEE, 0xFFDEDEDE, 0xFFCDCDCD, 0xFFBDBDBD, 0xFFACACAC, 0xFF9C9C9C, 0xFF8B8B8B, 0xFF7B7B7B, 0xFF6A6A6A, 0xFF5A5A5A, 0xFF4A4A4A, 0xFF393939, 0xFF292929, 0xFF181818, 0xFF101010
            },
            new[]
            {
                0xFFEE7B7B, 0xFFD51818, 0xFFF69418, 0xFFE6E652, 0xFF006A00, 0xFF39B439, 0xFF0039B4, 0xFF399CFF, 0xFF940094, 0xFFFF6AFF, 0xFF944108, 0xFFEE9C5A, 0xFFFFC594, 0xFF000000, 0xFFFFFFFF
            },
        };

        public static readonly uint[][] WwPaletteData = {
            new[]
            {
                0xFFFF0000, 0xFFFF7331, 0xFFFFAD00, 0xFFFFFF00, 0xFFADFF00, 0xFF52FF00, 0xFF00FF00, 0xFF00AD52, 0xFF0052AD, 0xFF0000FF, 0xFF5200FF, 0xFFAD00FF, 0xFFFF00FF, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFFF7B7B, 0xFFFFB57B, 0xFFFFE77B, 0xFFFFFF7B, 0xFFDEFF7B, 0xFFADFF7B, 0xFF7BFF7B, 0xFF52AD84, 0xFF5284AD, 0xFF7B7BFF, 0xFFB57BFF, 0xFFE77BFF, 0xFFFF7BFF, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFA50000, 0xFFA53100, 0xFFA57300, 0xFFA5A500, 0xFF73A500, 0xFF31A500, 0xFF00A500, 0xFF005221, 0xFF002152, 0xFF0000A5, 0xFF3100A5, 0xFF7300A5, 0xFFA500A5, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF009C00, 0xFF5ACE6B, 0xFFB5FFDE, 0xFF009C6B, 0xFF52CEA5, 0xFFADFFD6, 0xFF0052AD, 0xFF2984D6, 0xFF5AADFF, 0xFF0000FF, 0xFF4A6BFF, 0xFF314ADE, 0xFF1821B5, 0xFF00008C, 0xFFFFFFFF
            },
            new[]
            {
                0xFFAD7300, 0xFFD6AD42, 0xFFFFDE8C, 0xFFFF0839, 0xFFFF4A6B, 0xFFFF949C, 0xFFAD00FF, 0xFFD663FF, 0xFFFFCEFF, 0xFFFFBD9C, 0xFFDE9473, 0xFFBD634A, 0xFF9C3921, 0xFF7B1000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFFF0000, 0xFFFF5200, 0xFFFFB55A, 0xFFFFEFAD, 0xFF7B1000, 0xFFA54A31, 0xFFD6846B, 0xFFFFBD9C, 0xFF5AADFF, 0xFF84C6FF, 0xFFADE7FF, 0xFFD6FFFF, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF00FF00, 0xFF42FF42, 0xFF8CFF8C, 0xFFD6FFD6, 0xFF0000FF, 0xFF4242FF, 0xFF8C8CFF, 0xFFD6D6FF, 0xFFFF0000, 0xFFFF4242, 0xFFFF8C8C, 0xFFFFD6D6, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF003100, 0xFF426342, 0xFF849C84, 0xFFC6D6C6, 0xFF7B1000, 0xFFA54A29, 0xFFD68C5A, 0xFFFFC68C, 0xFFD6B500, 0xFFE7CE39, 0xFFF7DE7B, 0xFFFFF7BD, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF0000FF, 0xFFFF0000, 0xFFFFFF00, 0xFF4242FF, 0xFFFF4242, 0xFFFFFF42, 0xFF8C8CFF, 0xFFFF8C8C, 0xFFFFFF8C, 0xFFD6D6FF, 0xFFFFD6D6, 0xFFFFFFD6, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF00FF00, 0xFF0000FF, 0xFFFF00FF, 0xFF42FF42, 0xFF4242FF, 0xFFFF42FF, 0xFF8CFF8C, 0xFF8C8CFF, 0xFFFF8CFF, 0xFFD6FFD6, 0xFFD6D6FF, 0xFFFFD6FF, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFFF0000, 0xFFFF7B00, 0xFFFFFF00, 0xFF84FF00, 0xFF00FF00, 0xFF00847B, 0xFF0000FF, 0xFF7B00FF, 0xFFFF94FF, 0xFFD6B500, 0xFFBD1000, 0xFF5A1000, 0xFF6B6B6B, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFF109463, 0xFF087B52, 0xFF108C39, 0xFF319C31, 0xFFCEA54A, 0xFFCE9439, 0xFFBD8C4A, 0xFFD68C31, 0xFFAD734A, 0xFF8C5A31, 0xFF6B4229, 0xFF84EFFF, 0xFF31CEEF, 0xFF00A5C6, 0xFFFFFFFF
            },
            new[]
            {
                0xFFD6DEE7, 0xFFB5CEDE, 0xFFE7EFEF, 0xFFF7F7F7, 0xFF84737B, 0xFF948C6B, 0xFF847B63, 0xFF9C845A, 0xFF739CB5, 0xFFFF2929, 0xFFFFFF00, 0xFF9421FF, 0xFF009CBD, 0xFF000000, 0xFFFFFFFF
            },
            new[]
            {
                0xFFFFFFFF, 0xFFF7EFEF, 0xFFE7DEDE, 0xFFD6CECE, 0xFFC6B5B5, 0xFFB5A5A5, 0xFFA59494, 0xFF9C8484, 0xFF8C6B6B, 0xFF7B5A5A, 0xFF6B4A4A, 0xFF5A3131, 0xFF4A2121, 0xFF421010, 0xFF310000
            },
            new[]
            {
                0xFFFFFFFF, 0xFFEFEFEF, 0xFFDEDEDE, 0xFFCECECE, 0xFFB5B5B5, 0xFFA5A5A5, 0xFF949494, 0xFF848484, 0xFF6B6B6B, 0xFF5A5A5A, 0xFF4A4A4A, 0xFF313131, 0xFF212121, 0xFF101010, 0xFF000000
            },
            new[]
            {
                0xFFFF8C7B, 0xFFFF0000, 0xFFFF7B00, 0xFFFFFF00, 0xFF008400, 0xFF00FF00, 0xFF0000FF, 0xFF009CFF, 0xFFD600FF, 0xFFFF6BFF, 0xFF9C0000, 0xFFFF9400, 0xFFFFBD94, 0xFF000000, 0xFFFFFFFF
            },
        }; 

        public static readonly uint[][] CfPaletteData = {
            new[]
            {
                0xFFFF0000, 0xFF008000, 0xFF0000FF, 0xFF804000, 0xFF000000, 0xFFFF8000, 0xFF80FF00, 0xFF00EAEA, 0xFF800080, 0xFF808080, 0xFFFFCAE4, 0xFFFFFF00, 0xFFACCCFD, 0xFFFF66B3, 0xFFFFFFFF
            },

            new[]
            {
                0xFFFD561E, 0xFF00C400, 0xFF3982FB, 0xFFAC6802, 0xFF000000, 0xFFFFA54A, 0xFF9FFF40, 0xFF51A8FF, 0xFFEE70FC, 0xFF808080, 0xFFFFC993, 0xFFFFFF80, 0xFFC1E0FF, 0xFFFFC4E1, 0xFFFFFFFF
            },

            new[]
            {
                0xFF804040, 0xFF006633, 0xFF4B4BA9, 0xFF4B2F21, 0xFF000000, 0xFFAC6802, 0xFF00A854, 0xFF317BF7, 0xFF5F3E86, 0xFF969696, 0xFFDFB9AC, 0xFFA4A400, 0xFF9CBACF, 0xFFB06FB0, 0xFFFFFFFF
            },

            new[]
            {
                0xFFB02902, 0xFFA29F4F, 0xFF008282, 0xFF022B68, 0xFF000000, 0xFFFA7403, 0xFFE6C947, 0xFF04C67D, 0xFF5A5ACD, 0xFF808080, 0xFFFED7AF, 0xFFFFFFA8, 0xFFC4EBC2, 0xFF94B4FE, 0xFFFFFFFF
            },

            new[]
            {
                0xFFD05B9C, 0xFFEAB602, 0xFF2291FF, 0xFF8000FF, 0xFF000000, 0xFFFF9FCF, 0xFFF4F400, 0xFF7EC8FE, 0xFFB376C7, 0xFF808080, 0xFFEECCCC, 0xFFFFFFB0, 0xFFC1F4F3, 0xFFE7CFEF, 0xFFFFFFFF
            },

            new[]
            {
                0xFF400040, 0xFF424505, 0xFF006262, 0xFF2D0059, 0xFF000000, 0xFF981F5F, 0xFF9D6B4D, 0xFF377D95, 0xFF7143BC, 0xFF808080, 0xFFEC95C5, 0xFFCEC68A, 0xFF80AECA, 0xFFB56AFF, 0xFFFFFFFF
            },

            new[]
            {
                0xFF710071, 0xFF950000, 0xFF438507, 0xFF005984, 0xFF000000, 0xFFD9006C, 0xFFFF8000, 0xFF66CC00, 0xFF049788, 0xFF808080, 0xFFFF93FF, 0xFFF7BA02, 0xFFDCD838, 0xFF00FFFF, 0xFFFFFFFF
            },

            new[]
            {
                0xFF6D6D36, 0xFFA4A4A4, 0xFF371C00, 0xFF002D00, 0xFF000000, 0xFFAD8301, 0xFFB4B4B4, 0xFF5C3F23, 0xFF23673A, 0xFF808080, 0xFFD2CC5B, 0xFFE1E1E1, 0xFFB08364, 0xFF66938A, 0xFFFFFFFF
            },

            new[]
            {
                0xFF804040, 0xFF666666, 0xFF003737, 0xFF83745C, 0xFF000000, 0xFFB3757E, 0xFFC3C3C3, 0xFF233641, 0xFFA39E65, 0xFF808080, 0xFFD7A8A8, 0xFFD0CDB9, 0xFF5F5F6D, 0xFFC9BE98, 0xFFFFFFFF
            },

            new[]
            {
                0xFFAB5436, 0xFF3D140A, 0xFF6B3F8F, 0xFF793431, 0xFF000000, 0xFFD69769, 0xFF6A390D, 0xFF5050AF, 0xFFD1423F, 0xFF808080, 0xFFFABEBE, 0xFFCBAB69, 0xFF286249, 0xFFFF8B17, 0xFFFFFFFF
            },

            new[]
            {
                0xFF800080, 0xFF800040, 0xFFB35900, 0xFF8C8C00, 0xFF000000, 0xFFDF00DF, 0xFFE4013A, 0xFFFF8000, 0xFFFFFF00, 0xFF808080, 0xFFFF8CFF, 0xFFD68585, 0xFFFECF78, 0xFFFFFF80, 0xFFFFFFFF
            },

            new[]
            {
                0xFF4E009B, 0xFF0000A0, 0xFF538BB5, 0xFF008040, 0xFF000000, 0xFF8000FF, 0xFF5F22FD, 0xFF58CFFC, 0xFF029D5B, 0xFF84846F, 0xFFD896FC, 0xFF9595FF, 0xFF93FFFF, 0xFF43FEA5, 0xFFFFFFFF
            },

            new[]
            {
                0xFF9E8B49, 0xFF8B96A7, 0xFFB78786, 0xFF432121, 0xFF000000, 0xFFBF7E3E, 0xFFAFB0BC, 0xFFDDC9BF, 0xFF562945, 0xFF5B5B5B, 0xFFBBA85B, 0xFFB1C1D1, 0xFFE0CFA9, 0xFF8A5353, 0xFFFFFFFF
            },

            new[]
            {
                0xFF003535, 0xFF00974B, 0xFF487776, 0xFFF5F5F5, 0xFF8C4600, 0xFF00773C, 0xFF02B78E, 0xFF95B5BF, 0xFFEAF1F2, 0xFFCF7625, 0xFF02B08D, 0xFF64AB03, 0xFFE1E1E1, 0xFFFFFFFF, 0xFFE6AD59
            },

            new[]
            {
                0xFFA5ABBC, 0xFF002828, 0xFFCBA0BE, 0xFF392837, 0xFF000000, 0xFFBDC8D2, 0xFF44526A, 0xFFDBB0CF, 0xFF593349, 0xFF808080, 0xFFC6DDDD, 0xFF687D93, 0xFFE9C7C7, 0xFF8D6376, 0xFFFFFFFF
            },

            new[]
            {
                0xFFBDA98A, 0xFF222200, 0xFFBCBCBC, 0xFF232332, 0xFF000000, 0xFFE6D6BD, 0xFF5C5C2E, 0xFFD2D7DB, 0xFF424D55, 0xFF808080, 0xFFEFE7E7, 0xFF7B7C56, 0xFFE9EAED, 0xFF6B6B6B, 0xFFFFFFFF
            },
        };

        //Should probably convert other palette data to be like this, and just do palette * 16 + color_idx to find the color
        public static readonly uint[] NlPaletteData = {
            0xFFFFEEFF, 0xFFFF99AA, 0xFFEE5599, 0xFFFF66AA, 0xFFFF0066, 0xFFBB4477, 0xFFCC0055, 0xFF990033, 0xFF552233, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
            0xFFFFBBCC, 0xFFFF7777, 0xFFDD3311, 0xFFFF5544, 0xFFFF0000, 0xFFCC6666, 0xFFBB4444, 0xFFBB0000, 0xFF882222, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFEEEEEE,
            0xFFDDCCBB, 0xFFFFCC66, 0xFFDD6622, 0xFFFFAA22, 0xFFFF6600, 0xFFBB8855, 0xFFDD4400, 0xFFBB4400, 0xFF663311, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFDDDDDD,
            0xFFFFEEDD, 0xFFFFDDCC, 0xFFFFCCAA, 0xFFFFBB88, 0xFFFFAA88, 0xFFDD8866, 0xFFBB6644, 0xFF995533, 0xFF884422, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFCCCCCC,
            0xFFFFCCFF, 0xFFEE88FF, 0xFFCC66DD, 0xFFBB88CC, 0xFFCC00FF, 0xFF996699, 0xFF8800AA, 0xFF550077, 0xFF330044, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFBBBBBB,
            0xFFFFBBFF, 0xFFFF99FF, 0xFFDD22BB, 0xFFFF55EE, 0xFFFF00CC, 0xFF885577, 0xFFBB0099, 0xFF880066, 0xFF550044, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFAAAAAA,
            0xFFDDBB99, 0xFFCCAA77, 0xFF774433, 0xFFAA7744, 0xFF993300, 0xFF773322, 0xFF552200, 0xFF331100, 0xFF221100, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF999999,
            0xFFFFFFCC, 0xFFFFFF77, 0xFFDDDD22, 0xFFFFFF00, 0xFFFFDD00, 0xFFCCAA00, 0xFF999900, 0xFF887700, 0xFF555500, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF888888,
            0xFFDDBBFF, 0xFFBB99EE, 0xFF6633CC, 0xFF9955FF, 0xFF6600FF, 0xFF554488, 0xFF440099, 0xFF220066, 0xFF221133, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF777777,
            0xFFBBBBFF, 0xFF8899FF, 0xFF3333AA, 0xFF3355EE, 0xFF0000FF, 0xFF333388, 0xFF0000AA, 0xFF111166, 0xFF000022, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF666666,
            0xFF99EEBB, 0xFF66CC77, 0xFF226611, 0xFF44AA33, 0xFF008833, 0xFF557755, 0xFF225500, 0xFF113322, 0xFF002211, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF555555,
            0xFFDDFFBB, 0xFFCCFF88, 0xFF88AA55, 0xFFAADD88, 0xFF88FF00, 0xFFAABB99, 0xFF66BB00, 0xFF559900, 0xFF336600, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF444444,
            0xFFBBDDFF, 0xFF77CCFF, 0xFF335599, 0xFF6699FF, 0xFF1177FF, 0xFF4477AA, 0xFF224477, 0xFF002277, 0xFF001144, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF333333,
            0xFFAAFFFF, 0xFF55FFFF, 0xFF0088BB, 0xFF55BBCC, 0xFF00CCFF, 0xFF4499AA, 0xFF006688, 0xFF004455, 0xFF002233, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF222222,
            0xFFCCFFEE, 0xFFAAEEDD, 0xFF33CCAA, 0xFF55EEBB, 0xFF00FFCC, 0xFF77AAAA, 0xFF00AA99, 0xFF008877, 0xFF004433, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFF000000,
            0xFFAAFFAA, 0xFF77FF77, 0xFF66DD44, 0xFF00FF00, 0xFF22DD22, 0xFF55BB55, 0xFF00BB00, 0xFF008800, 0xFF224422, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF
        };

        #endregion

        private static Color ColorFromUInt32(uint color)
        {
            return Color.FromArgb(0xFF, (byte)(color >> 16), (byte)(color >> 8), (byte)(color));
        }

        public static byte ClosestColorHsv(uint color, uint[] paletteData)
        {
            var closestColor = paletteData[0];
            var diff = double.MaxValue;
            var c = ColorFromUInt32(color);
            var targetHue = c.GetHue();
            var targetSat = c.GetSaturation();
            var targetBri = c.GetBrightness();

            foreach (var validColor in paletteData)
            {
                var checkColor = ColorFromUInt32(validColor);
                var currentHue = checkColor.GetHue();
                var currentSat = checkColor.GetSaturation();
                var currentBri = checkColor.GetBrightness();

                var currentDiff = Math.Pow(targetHue - currentHue, 2) + Math.Pow(targetSat - currentSat, 2) + Math.Pow(targetBri - currentBri, 2);

                if (!(currentDiff < diff)) continue;
                diff = currentDiff;
                closestColor = validColor;
            }

            return (byte)(Array.IndexOf(paletteData, closestColor) + 1);
        }

        public static byte ClosestColorRgb(uint color, uint[] paletteData, bool gen1 = false)
        {
            var distance = double.MaxValue;
            byte closestPaletteIndex = 0;
            double r = color & 0xFF;
            double g = (color >> 8) & 0xFF;
            double b = (color >> 16) & 0xFF;

            for (var i = 0; i < paletteData.Length; i++)
            {
                var paletteColor = paletteData[i];
                double pR = paletteColor & 0xFF;
                double pG = (paletteColor >> 8) & 0xFF;
                double pB = (paletteColor >> 16) & 0xFF;

                var thisDistance = Math.Sqrt(Math.Pow(pR - r, 2) + Math.Pow(pG - g, 2) + Math.Pow(pB - b, 2));
                if (Math.Abs(thisDistance) < double.Epsilon)
                {
                    // Perfect match
                    return gen1 ? (byte)(i + 1) : (byte) i;
                }

                if (!(thisDistance < distance)) continue;

                distance = thisDistance;
                closestPaletteIndex = gen1 ? (byte)(i + 1) : (byte)i;
            }

            return closestPaletteIndex;
        }
    }

    public sealed class Pattern
    {
        private readonly int _offset;
        public readonly int Index;
        public byte[] PatternBitmapBuffer = new byte[4 * 32 * 32];
        public byte[] DecodedData;
        public string Name;
        public string CreatorName;
        public string TownName;
        public byte Palette;
        public byte Concept;
        public Bitmap PatternBitmap;
        public uint[] PaletteData;

        public Pattern(int patternOffset, int index)
        {
            _offset = patternOffset;
            Index = index;
            Read(index);
        }

        public uint[][] GetPaletteArray(SaveGeneration saveGeneration)
        {
            switch (saveGeneration)
            {
                default:
                    return PatternData.AcPaletteData;
                case SaveGeneration.NDS:
                    return PatternData.WwPaletteData;
                case SaveGeneration.Wii:
                    return PatternData.CfPaletteData;
            }
        }

        // AC / CF
        public void GeneratePatternBitmap(byte[] importData = null, bool decode = true)
        {
            var patternRawData = importData ?? (Save.SaveInstance.SaveType == SaveType.CityFolk
                                     ? Save.SaveInstance.ReadByteArray(_offset, 0x200)
                                     : Save.SaveInstance.ReadByteArray(_offset + 0x20, 0x200));

            var paletteData = Save.SaveInstance.SaveType == SaveType.CityFolk
                ? PatternData.CfPaletteData
                : PatternData.AcPaletteData;

            if (decode)
            {
                DecodedData = PatternUtility.DecodeC4(patternRawData);
            }

            PatternBitmap = PatternUtility.C4PaletteMapToBitmap(DecodedData, paletteData[Palette]);
        }

        public void GenerateWwPatternBitmap(byte[] importData = null, bool decode = true)
        {
            var rawData = importData ?? Save.SaveInstance.ReadByteArray(_offset, 0x200);

            if (decode)
            {
                DecodedData = new byte[0x400];

                for (var i = 0; i < 0x200; i++)
                {
                    DecodedData[i * 2] = (byte)(rawData[i] & 0x0F);  //Left is Right
                    DecodedData[i * 2 + 1] = (byte)((rawData[i] & 0xF0) >> 4); //Right is Left
                }
            }

            var paletteArray = PatternData.WwPaletteData[Palette];
            
            for (var i = 0; i < 0x400; i++)
            {
                var colorIdx = Math.Max(0, DecodedData[i] - 1);
                Buffer.BlockCopy(BitConverter.GetBytes(paletteArray[colorIdx]), 0, PatternBitmapBuffer, i * 4, 4);
            }
            PatternBitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            var bitmapData = PatternBitmap.LockBits(new Rectangle(0, 0, 32, 32), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(PatternBitmapBuffer, 0, bitmapData.Scan0, PatternBitmapBuffer.Length);
            PatternBitmap.UnlockBits(bitmapData);
        }

        //NL Patterns have a custom palette, created by the user by choosing 15 colors
        public void GenerateNlPatternBitmap(byte[] importData = null, bool decode = true)
        {
            //Add decoding of "Pro" patterns
            var rawData = importData ?? Save.SaveInstance.ReadByteArray(_offset + 0x6C, 0x200); //32x32 doubled up pixels
            var customPalette = Save.SaveInstance.ReadByteArray(_offset + 0x58, 16); //New Leaf user selected palette data
            PaletteData = new uint[15];

            // Generate Palatte Data
            for (var i = 0; i < 15; i++)
                PaletteData[i] = PatternData.NlPaletteData[customPalette[i]];

            if (decode)
            {
                //Expand data for working with

                DecodedData = new byte[0x400]; //32x32 expanded pixel buffer
                for (var i = 0; i < 0x200; i++)
                {
                    DecodedData[i * 2] = (byte)(rawData[i] & 0x0F);
                    DecodedData[i * 2 + 1] = (byte)((rawData[i] >> 4) & 0x0F);
                }
            }

            //Convert palette color index to argb color
            for (var i = 0; i < 0x400; i++)
                Buffer.BlockCopy(BitConverter.GetBytes(PaletteData[DecodedData[i] % 15]), 0, PatternBitmapBuffer, i * 4, 4);
            
            //Create new bitmap
            PatternBitmap = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
            BitmapData bitmapData = PatternBitmap.LockBits(new Rectangle(0, 0, 32, 32), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(PatternBitmapBuffer, 0, bitmapData.Scan0, PatternBitmapBuffer.Length);
            PatternBitmap.UnlockBits(bitmapData);
        }

        public void Read(int index)
        {
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.AnimalCrossing:
                    Name = new AcString(Save.SaveInstance.ReadByteArray(_offset, 0x10), Save.SaveInstance.SaveType).Trim();
                    Palette = Save.SaveInstance.ReadByte(_offset + 0x10);
                    PaletteData = PatternData.AcPaletteData[Palette];
                    GeneratePatternBitmap();
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalForestEPlus:
                    Name = new AcString(Save.SaveInstance.ReadByteArray(_offset, 0xA), Save.SaveInstance.SaveType).Trim();
                    Palette = Save.SaveInstance.ReadByte(_offset + 0xA);
                    PaletteData = PatternData.AcPaletteData[Palette];
                    GeneratePatternBitmap();
                    break;
                case SaveType.WildWorld:
                    TownName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x202, 8), SaveType.WildWorld).Trim();
                    CreatorName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x20C, 8), SaveType.WildWorld).Trim();
                    Name = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x216, 16), SaveType.WildWorld).Trim();
                    Palette = (byte)((Save.SaveInstance.ReadByte(_offset + 0x226) & 0xF0) >> 4);
                    Concept = (byte)(Save.SaveInstance.ReadByte(_offset + 0x226) & 0x0F);
                    PaletteData = PatternData.WwPaletteData[Palette];
                    GenerateWwPatternBitmap();
                    break;
                case SaveType.CityFolk:
                    TownName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x822, 16), SaveType.CityFolk).Trim();
                    CreatorName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x838, 16), SaveType.CityFolk).Trim();
                    Name = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x84C, 32), SaveType.CityFolk).Trim();
                    Palette = Save.SaveInstance.ReadByte(_offset + 0x86F);
                    PaletteData = PatternData.CfPaletteData[Palette];
                    GeneratePatternBitmap();
                    break;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    Name = new AcString(Save.SaveInstance.ReadByteArray(_offset, 0x2A), SaveType.NewLeaf).Trim();
                    CreatorName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x2C, 0x14), SaveType.NewLeaf).Trim();
                    TownName = new AcString(Save.SaveInstance.ReadByteArray(_offset + 0x42, 0x14), SaveType.NewLeaf).Trim();
                    //No specific palette in NL/WA
                    GenerateNlPatternBitmap();
                    break;
            }
        }

        public void RedrawBitmap()
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    GeneratePatternBitmap(DecodedData, false);
                    Write(PatternUtility.EncodeC4(DecodedData));
                    break;
                case SaveGeneration.NDS:
                    GenerateWwPatternBitmap(DecodedData, false);
                    Write(PatternUtility.CondenseNonBlockPattern(DecodedData));
                    break;
                case SaveGeneration.N3DS:
                    GenerateNlPatternBitmap(DecodedData, false);
                    Write(PatternUtility.CondenseNonBlockPattern(DecodedData));
                    break;
            }
        }

        public void Import(uint[] bitmapBuffer)
        {
            // Convert to nibble map array of bytes
            var patternBuffer = new byte[bitmapBuffer.Length / 2];

            if (Save.SaveInstance.SaveGeneration == SaveGeneration.NDS || Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
            {
                for (var i = 0; i < patternBuffer.Length; i++)
                {
                    var idx = i * 2;
                    patternBuffer[i] = (byte) ((PatternData.ClosestColorRgb(bitmapBuffer[idx + 1], PaletteData) << 4) |
                                               PatternData.ClosestColorRgb(bitmapBuffer[idx], PaletteData));
                }
            }
            else
            {
                var convertedBuffer = new byte[bitmapBuffer.Length];
                for (var i = 0; i < convertedBuffer.Length; i++)
                {
                    convertedBuffer[i] = PatternData.ClosestColorRgb(bitmapBuffer[i], PaletteData,
                        Save.SaveInstance.SaveGeneration == SaveGeneration.GCN);
                }
                patternBuffer = PatternUtility.EncodeC4(convertedBuffer);
            }

            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    GeneratePatternBitmap(patternBuffer);
                    break;
                case SaveGeneration.NDS:
                    GenerateWwPatternBitmap(patternBuffer);
                    break;
                case SaveGeneration.N3DS:
                    GenerateNlPatternBitmap(patternBuffer);
                    break;
            }

            Write(patternBuffer);
        }

        public void Write(byte[] newPatternData)
        {
            if (Save.SaveInstance.SaveGeneration == SaveGeneration.GCN)
            {
                var patternNameSize = Save.SaveInstance.SaveType == SaveType.AnimalCrossing ? 0x10 : 0x0A;
                Save.SaveInstance.Write(_offset, AcString.GetBytes(Name, patternNameSize));
                Save.SaveInstance.Write(_offset + patternNameSize, Palette);
                Save.SaveInstance.Write(_offset + 0x20, newPatternData);
            }
            else switch (Save.SaveInstance.SaveType)
            {
                case SaveType.WildWorld:
                    Save.SaveInstance.Write(_offset, newPatternData);
                    // TODO: Town Name & Creator Name (Also for City Folk, New Leaf)
                    Save.SaveInstance.Write(_offset + 0x216, AcString.GetBytes(Name, 0x10));
                    Save.SaveInstance.Write(_offset + 0x226, (byte)(((Palette & 0x0F) << 4) | (Concept & 0x0F)));
                    break;
                case SaveType.CityFolk:
                    Save.SaveInstance.Write(_offset, newPatternData);
                    Save.SaveInstance.Write(_offset + 0x84C, AcString.GetBytes(Name, 0x20));
                    Save.SaveInstance.Write(_offset + 0x86F, Palette);
                    break;
                default:
                    if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                    {
                        Save.SaveInstance.Write(_offset, AcString.GetBytes(Name, 0x2A));
                        Save.SaveInstance.Write(_offset + 0x6C, newPatternData);
                        // TODO: Write Palette (since it's customizable)
                    }

                    break;
            }
        }

        public void Write()
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    Write(PatternUtility.EncodeC4(DecodedData));
                    break;
                case SaveGeneration.NDS:
                case SaveGeneration.N3DS:
                    Write(PatternUtility.CondenseNonBlockPattern(DecodedData));
                    break;
            }
        }
    }
}
