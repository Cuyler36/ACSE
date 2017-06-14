using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ACSE.Properties;
using System.Resources;
using System.Globalization;
using System.Collections;

namespace ACSE
{
    class AcreData
    {
        /*
         * Acres usually contain both a Mid-level and Low-level variant.
         * They are offset (in < 0x03A0?) by 0x1 starting at the low variant.
         * The only exception are cliffs (unless they actually do have that 4th layer programmed in.)
         * Example:
             * 0x0278 = Empty Acre (Lower) (4)
             * 0x0279 = Empty Acre (Middle) (4)
             * 0x027A = Empty Acre (Upper) (4)
        
         * In >= 0x03A0 acres, they are all offset by 0x4. My guess is that these were added either in the transition from Animal Forest to Animal Crossing,
         * or at the end of the development period. It could also be that to figure out the level, their code does: int Level = Acre_ID % 4.
         * Lower Acres would set it to 0, Mid Acres would be 1, and Upper Acres would be 2. This still doesn't explain the lack of any fourth level acres.

         * Another possible explaination is that originally, they were planning on using up to four different levels.
         * This would explain why there are so many ocean acres. They would have no need for the extra ocean acre row after the islands if not.
         * This plan would have likely been changed due to the size of town item data on the memory card.
         * That would explain why there is no town item data for any of the ocean acres, except for H-4 & H-5 (the island acres.)
         * 
         * Just confirmed it. 0x01B6 (A "4th > 3rd" Cliff acre should have no model. It does. And it's a true 4th height. This means my hypothesis about their plan to
         * have a max of four acre heights is correct!
         * 
         * Figured out how this acre thing works all in one night! My theory was correct. They originally wanted to have up to 4 different acre heights.
         * Each acre is actually 0x4 away from the last. The acres between those are just the same acre, at a different height. This means that
         * They only needed to create 1/4th of the total acre count to get all these acres!
         */

        //99% Documented
        public static Dictionary<ushort, string> Acres = new Dictionary<ushort, string>()
        {
            {0x0000, "No Acre Data" },
            {0x0060, "River /w Islets & Stone Bridges (Left > Down) (Lowest)" },
            {0x0061, "River /w Islets & Stone Bridges (Left > Down) (Lower)" },
            {0x0062, "River /w Islets & Stone Bridges (Left > Down) (Middle)" },
            {0x0063, "River /w Islets & Stone Bridges (Left > Down) (Upper)" },
            //0x0064 (Invalid data)
            //0x0068 (Invalid Empty Acre)
            {0x006C, "Cliff (Right Corner) (Lowest)" },
            {0x006D, "Cliff (Right Corner) (Lower)" },
            {0x006E, "Cliff (Right Corner) (Middle)" },
            {0x006F, "Cliff (Right Corner) (Upper)" },
            {0x0070, "River w/ Train Track Bridge (1) (Lowest)" },
            {0x0071, "River w/ Train Track Bridge (1) (Lower)" },
            {0x0072, "River w/ Train Track Bridge (1) (Middle)" },
            {0x0073, "River w/ Train Track Bridge (1) (Upper)" },
            //0x0074 (Glitched Beta Acre (Grass))
            //0x0078 (Glitched Beta Acre (Ramp))
            //0x007C (Glitched Beta Acre (Lake))
            //0x0080 (Glitched Beta Acre (River))
            {0x0084, "Cliff (Horizontal) w/ Waterfall (Lowest)" },
            {0x0085, "Cliff (Horizontal) w/ Waterfall (Lower)" },
            {0x0086, "Cliff (Horizontal) w/ Waterfall (Middle)" },
            {0x0087, "Cliff (Horizontal) w/ Waterfall (Upper)" },
            {0x0088, "Ramp (Horizontal) (Lowest)" },
            {0x0089, "Ramp (Horizontal) (Lower)" },
            {0x008A, "Ramp (Horizontal) (Middle)" },
            {0x008B, "Ramp (Horizontal) (Upper)" },
            {0x008C, "River (Vertical) w/ Waterfall & Cliff (Lowest)" },
            {0x008D, "River (Vertical) w/ Waterfall & Cliff (Lower)" },
            {0x008E, "River (Vertical) w/ Waterfall & Cliff (Middle)" },
            {0x008F, "River (Vertical) w/ Waterfall & Cliff (Upper)" },
            {0x0090, "Cliff (East > North) w/ Waterfall (East) (Lowest)" },
            {0x0091, "Cliff (East > North) w/ Waterfall (East) (Lower)" },
            {0x0092, "Cliff (East > North) w/ Waterfall (East) (Middle)" },
            {0x0093, "Cliff (East > North) w/ Waterfall (East) (Upper)" },
            {0x0094, "Empty Acre (1) (Lowest)" },
            {0x0095, "Empty Acre (1) (Lower)" },
            {0x0096, "Empty Acre (1) (Middle)" },
            {0x0097, "Empty Acre (1) (Upper)" },
            {0x0098, "Empty Acre (2) (Lowest)" },
            {0x0099, "Empty Acre (2) (Lower)" },
            {0x009A, "Empty Acre (2) (Middle)" },
            {0x009B, "Empty Acre (2) (Upper)" },
            {0x009C, "Cliff (Horizontal) (Lowest)" },
            {0x009D, "Cliff (Horizontal) (Lower)" },
            {0x009E, "Cliff (Horizontal) (Middle)" },
            {0x009F, "Cliff (Horizontal) (Upper)" },
            {0x00A0, "Cliff (Horizontal) w/ River (East) (Lowest)" },
            {0x00A1, "Cliff (Horizontal) w/ River (East) (Lower)" },
            {0x00A2, "Cliff (Horizontal) w/ River (East) (Middle)" },
            {0x00A3, "Cliff (Horizontal) w/ River (East) (Upper)" },
            {0x00A4, "Cliff (Horizontal) w/ River (West) (Lowest)" },
            {0x00A5, "Cliff (Horizontal) w/ River (West) (Lower)" },
            {0x00A6, "Cliff (Horizontal) w/ River (West) (Middle)" },
            {0x00A7, "Cliff (Horizontal) w/ River (West) (Upper)" },
            {0x00A8, "Cliff (Left > Up) (Lowest)" },
            {0x00A9, "Cliff (Left > Up) (Lower)" },
            {0x00AA, "Cliff (Left > Up) (Middle)" },
            {0x00AB, "Cliff (Left > Up) (Upper)" },
            {0x00AC, "Cliff (Vertical) (Lowest)" },
            {0x00AD, "Cliff (Vertical) (Lower)" },
            {0x00AE, "Cliff (Vertical) (Middle)" },
            {0x00AF, "Cliff (Vertical) (Upper)" },
            {0x00B0, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x00B1, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x00B2, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x00B3, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x00B4, "Cliff (Left Corner) (Lowest)" },
            {0x00B5, "Cliff (Left Corner) (Lower)" },
            {0x00B6, "Cliff (Left Corner) (Middle)" },
            {0x00B7, "Cliff (Left Corner) (Upper)" },
            {0x00B8, "River (Vertical) w/ Corner Cliff (Lowest)" },
            {0x00B9, "River (Vertical) w/ Corner Cliff (Lower)" },
            {0x00BA, "River (Vertical) w/ Corner Cliff (Middle)" },
            {0x00BB, "River (Vertical) w/ Corner Cliff (Upper)" },
            {0x00BC, "Cliff (North > East) w/ River (East) (Lowest)" },
            {0x00BD, "Cliff (North > East) w/ River (East) (Lower)" },
            {0x00BE, "Cliff (North > East) w/ River (East) (Middle)" },
            {0x00BF, "Cliff (North > East) w/ River (East) (Upper)" },
            {0x00C0, "Cliff (Right Corner) (Lowest)" },
            {0x00C1, "Cliff (Right Corner) (Lower)" },
            {0x00C2, "Cliff (Right Corner) (Middle)" },
            {0x00C3, "Cliff (Right Corner) (Upper)" },
            {0x00C4, "Cliff (East > South) w/ River (East) (Lowest)" },
            {0x00C5, "Cliff (East > South) w/ River (East) (Lower)" },
            {0x00C6, "Cliff (East > South) w/ River (East) (Middle)" },
            {0x00C7, "Cliff (East > South) w/ River (East) (Upper)" },
            {0x00C8, "Cliff (Right > Down) w/ River (Lowest)" },
            {0x00C9, "Cliff (Right > Down) w/ River (Lower)" },
            {0x00CA, "Cliff (Right > Down) w/ River (Middle)" },
            {0x00CB, "Cliff (Right > Down) w/ River (Upper)" },
            {0x00CC, "Cliff (Vertical) (Lowest)" },
            {0x00CD, "Cliff (Vertical) (Lower)" },
            {0x00CE, "Cliff (Vertical) (Middle)" },
            {0x00CF, "Cliff (Vertical) (Upper)" },
            {0x00D0, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x00D1, "Cliff (Vertical) w/ River (South) (Lower)" },
            {0x00D2, "Cliff (Vertical) w/ River (South) (Middle)" },
            {0x00D3, "Cliff (Vertical) w/ River (South) (Upper)" },
            {0x00D4, "Cliff (Down > Right) (Lowest)" },
            {0x00D5, "Cliff (Down > Right) (Lower)" },
            {0x00D6, "Cliff (Down > Right) (Middle)" },
            {0x00D7, "Cliff (Down > Right) (Upper)" },
            {0x00D8, "River (Vertical) (Lowest)" },
            {0x00D9, "River (Vertical) (Lower)" },
            {0x00DA, "River (Vertical) (Middle)" },
            {0x00DB, "River (Vertical) (Upper)" },
            {0x00DC, "River (Horizontal) (Lowest)" },
            {0x00DD, "River (Horizontal) (Lower)" },
            {0x00DE, "River (Horizontal) (Middle)" },
            {0x00DF, "River (Horizontal) (Upper)" },
            {0x00E0, "River (Horizontal) (Lowest)" },
            {0x00E1, "River (Horizontal) (Lower)" },
            {0x00E2, "River (Horizontal) (Middle)" },
            {0x00E3, "River (Horizontal) (Upper)" },
            {0x00E4, "River /w Suspension Bridge (Down > Right) (Lowest)" },
            {0x00E5, "River /w Suspension Bridge (Down > Right) (Lower)" },
            {0x00E6, "River /w Suspension Bridge (Down > Right) (Middle)" },
            {0x00E7, "River /w Suspension Bridge (Down > Right) (Upper)" },
            {0x00E8, "River (Right > Down) (Lowest)" },
            {0x00E9, "River (Right > Down) (Lower)" },
            {0x00EA, "River (Right > Down) (Middle)" },
            {0x00EB, "River (Right > Down) (Upper)" },
            {0x00EC, "River (Down > Left) (Lowest)" },
            {0x00ED, "River (Down > Left) (Lower)" },
            {0x00EE, "River (Down > Left) (Middle)" },
            {0x00EF, "River (Down > Left) (Upper)" },
            {0x00F0, "River (Left > Down) (Lowest)" },
            {0x00F1, "River (Left > Down) (Lower)" },
            {0x00F2, "River (Left > Down) (Middle)" },
            {0x00F3, "River (Left > Down) (Upper)" },
            {0x00F4, "Cliff (Vertical) w/ Waterfall (East) (Lowest)" },
            {0x00F5, "Cliff (Vertical) w/ Waterfall (East) (Lower)" },
            {0x00F6, "Cliff (Vertical) w/ Waterfall (East) (Middle)" },
            {0x00F7, "Cliff (Vertical) w/ Waterfall (East) (Upper)" },
            {0x00F8, "Cliff (South > East) w/ Waterfall (West) (Lowest)" },
            {0x00F9, "Cliff (South > East) w/ Waterfall (West) (Lower)" },
            {0x00FA, "Cliff (South > East) w/ Waterfall (West) (Middle)" },
            {0x00FB, "Cliff (South > East) w/ Waterfall (West) (Upper)" },
            {0x00FC, "Cliff (Vertical) w/ Waterfall (West) (Lowest)" },
            {0x00FD, "Cliff (Vertical) w/ Waterfall (West) (Lower)" },
            {0x00FE, "Cliff (Vertical) w/ Waterfall (West) (Middle)" },
            {0x00FF, "Cliff (Vertical) w/ Waterfall (West) (Upper)" },
            {0x0100, "River (Vertical) w/ Stone Bridge (Lowest)" },
            {0x0101, "River (Vertical) w/ Stone Bridge (Lower)" },
            {0x0102, "River (Vertical) w/ Stone Bridge (Middle)" },
            {0x0103, "River (Vertical) w/ Stone Bridge (Upper)" },
            {0x0104, "River (East) w/ Islet & Stone Bridges (Lowest)" },
            {0x0105, "River (East) w/ Islet & Stone Bridges (Lower)" },
            {0x0106, "River (East) w/ Islet & Stone Bridges (Middle)" },
            {0x0107, "River (East) w/ Islet & Stone Bridges (Upper)" },
            {0x0108, "River (East > West) w/ Stone Bridge (Lowest)" },
            {0x0109, "River (East > West) w/ Stone Bridge (Lower)" },
            {0x010A, "River (East > West) w/ Stone Bridge (Middle)" },
            {0x010B, "River (East > West) w/ Stone Bridge (Upper)" },
            {0x010C, "River (Down > Right) w/ Stone Bridge (Lowest)" },
            {0x010D, "River (Down > Right) w/ Stone Bridge (Lower)" },
            {0x010E, "River (Down > Right) w/ Stone Bridge (Middle)" },
            {0x010F, "River (Down > Right) w/ Stone Bridge (Upper)" },
            {0x0110, "River (Down > Left) w/ Stone Bridge (Lowest)" },
            {0x0111, "River (Down > Left) w/ Stone Bridge (Lower)" },
            {0x0112, "River (Down > Left) w/ Stone Bridge (Middle)" },
            {0x0113, "River (Down > Left) w/ Stone Bridge (Upper)" },
            {0x0114, "River w/ Stone Bridge (Left > Down) (Lowest)" },
            {0x0115, "River w/ Stone Bridge (Left > Down) (Lower)" },
            {0x0116, "River w/ Stone Bridge (Left > Down) (Middle)" },
            {0x0117, "River w/ Stone Bridge (Left > Down) (Upper)" },
            {0x0118, "Dump (1) (Lowest)" },
            {0x0119, "Dump (1) (Lower)" },
            {0x011A, "Dump (1) (Middle)" },
            {0x011B, "Dump (1) (Upper)" },
            {0x011C, "Ramp (Horizontal) (Lowest)" },
            {0x011D, "Ramp (Horizontal) (Lower)" },
            {0x011E, "Ramp (Horizontal) (Middle)" },
            {0x011F, "Ramp (Horizontal) (Upper)" },
            {0x0120, "Ramp (Right > Up) (Lowest)" },
            {0x0121, "Ramp (Right > Up) (Lower)" },
            {0x0122, "Ramp (Right > Up) (Middle)" },
            {0x0123, "Ramp (Right > Up) (Upper)" },
            {0x0124, "Ramp (Vertical) (Right) (Lowest)" },
            {0x0125, "Ramp (Vertical) (Right) (Lower)" },
            {0x0126, "Ramp (Vertical) (Right) (Middle)" },
            {0x0127, "Ramp (Vertical) (Right) (Upper)" },
            {0x0128, "Corner Ramp (Up > Right) (Lowest)" },
            {0x0129, "Corner Ramp (Up > Right) (Lower)" },
            {0x012A, "Corner Ramp (Up > Right) (Middle)" },
            {0x012B, "Corner Ramp (Up > Right) (Upper)" },
            {0x012C, "Ramp (Right Corner) (Lowest)" },
            {0x012D, "Ramp (Right Corner) (Lower)" },
            {0x012E, "Ramp (Right Corner) (Middle)" },
            {0x012F, "Ramp (Right Corner) (Upper)" },
            {0x0130, "Ramp (Left) (Vertical Cliff) (Lowest)" },
            {0x0131, "Ramp (Left) (Vertical Cliff) (Lower)" },
            {0x0132, "Ramp (Left) (Vertical Cliff) (Middle)" },
            {0x0133, "Ramp (Left) (Vertical Cliff) (Upper)" },
            {0x0134, "Ramp (Down > Right) (Left Side) (Lowest)" },
            {0x0135, "Ramp (Down > Right) (Left Side) (Lower)" },
            {0x0136, "Ramp (Down > Right) (Left Side) (Middle)" },
            {0x0137, "Ramp (Down > Right) (Left Side) (Upper)" },
            {0x0138, "Cliff (Down > Right) w/ River (Lowest)" },
            {0x0139, "Cliff (Down > Right) w/ River (Lower)" },
            {0x013A, "Cliff (Down > Right) w/ River (Middle)" },
            {0x013B, "Cliff (Down > Right) w/ River (Upper)" },
            {0x013C, "Cliff (North > East) w/ River (West) (Lowest)" },
            {0x013D, "Cliff (North > East) w/ River (West) (Lower)" },
            {0x013E, "Cliff (North > East) w/ River (West) (Middle)" },
            {0x013F, "Cliff (North > East) w/ River (West) (Upper)" },
            //0x0140 (Invalid Map Data) (Crazy Redd's Shop Inside)
            //0x0144 (Invalid Map Data) (Post Office Inside)
            //0x0148 (Invalid Map Data) (Train Car Inside 1)
            //0x014C (Invalid Map Data) (Train Car Inside 2)
            //0x0150 (Invalid Map Data?) (Empty Acre, Corrupted Map Icon Index?)
            {0x0154, "Train Station (1) (Lowest)" },
            {0x0155, "Train Station (1) (Lower)" },
            {0x0156, "Train Station (1) (Middle)" },
            {0x0157, "Train Station (1) (Upper)" },
            //0x0158 (Empty Acre (No Map Index, so breaks map) | Invalid)
            {0x015C, "Cliff (Horizontal) (Lowest)" },
            {0x015D, "Cliff (Horizontal) (Lower)" },
            {0x015E, "Cliff (Horizontal) (Middle)" },
            {0x015F, "Cliff (Horizontal) (Upper)" },
            {0x0160, "Cliff (Horizontal) (Lowest)" },
            {0x0161, "Cliff (Horizontal) (Lower)" },
            {0x0162, "Cliff (Horizontal) (Middle)" },
            {0x0163, "Cliff (Horizontal) (Upper)" },
            {0x0164, "Cliff (Horizontal) (Lowest)" },
            {0x0165, "Cliff (Horizontal) (Lower)" },
            {0x0166, "Cliff (Horizontal) (Middle)" },
            {0x0167, "Cliff (Horizontal) (Upper)" },
            {0x0168, "Cliff (Horizontal) (Lowest)" },
            {0x0169, "Cliff (Horizontal) (Lower)" },
            {0x016A, "Cliff (Horizontal) (Middle)" },
            {0x016B, "Cliff (Horizontal) (Upper)" },
            {0x016C, "Cliff (Right > Up) (Lowest)" },
            {0x016D, "Cliff (Right > Up) (Lower)" },
            {0x016E, "Cliff (Right > Up) (Middle)" },
            {0x016F, "Cliff (Right > Up) (Upper)" },
            {0x0170, "River (Vertical) (Lowest)" },
            {0x0171, "River (Vertical) (Lower)" },
            {0x0172, "River (Vertical) (Middle)" },
            {0x0173, "River (Vertical) (Upper)" },
            {0x0174, "River (Vertical) (Lowest)" },
            {0x0175, "River (Vertical) (Lower)" },
            {0x0176, "River (Vertical) (Middle)" },
            {0x0177, "River (Vertical) (Upper)" },
            {0x0178, "River (Down > Right) (Lowest)" },
            {0x0179, "River (Down > Right) (Lower)" },
            {0x017A, "River (Down > Right) (Middle)" },
            {0x017B, "River (Down > Right) (Upper)" },
            {0x017C, "River (Right > Down) (Lowest)" },
            {0x017D, "River (Right > Down) (Lower)" },
            {0x017E, "River (Right > Down) (Middle)" },
            {0x017F, "River (Right > Down) (Upper)" },
            {0x0180, "River (Left > Down) (Lowest)" },
            {0x0181, "River (Left > Down) (Lower)" },
            {0x0182, "River (Left > Down) (Middle)" },
            {0x0183, "River (Left > Down) (Upper)" },
            {0x0184, "River (Left > Down) (Lowest)" },
            {0x0185, "River (Left > Down) (Lower)" },
            {0x0186, "River (Left > Down) (Middle)" },
            {0x0187, "River (Left > Down) (Upper)" },
            {0x0188, "Ramp (Left > Up) (Lowest)" },
            {0x0189, "Ramp (Left > Up) (Lower)" },
            {0x018A, "Ramp (Left > Up) (Middle)" },
            {0x018B, "Ramp (Left > Up) (Upper)" },
            {0x018C, "Ramp (Horizontal) (Lowest)" },
            {0x018D, "Ramp (Horizontal) (Lower)" },
            {0x018E, "Ramp (Horizontal) (Middle)" },
            {0x018F, "Ramp (Horizontal) (Upper)" },
            {0x0190, "Corner Ramp (Up > Right) (Lowest)" },
            {0x0191, "Corner Ramp (Up > Right) (Lower)" },
            {0x0192, "Corner Ramp (Up > Right) (Middle)" },
            {0x0193, "Corner Ramp (Up > Right) (Upper)" },
            {0x0194, "Ramp (Horizontal) (Lowest)" },
            {0x0195, "Ramp (Horizontal) (Lower)" },
            {0x0196, "Ramp (Horizontal) (Middle)" },
            {0x0197, "Ramp (Horizontal) (Upper)" },
            {0x0198, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x0199, "Cliff (Vertical) w/ River (South) (Lower)" },
            {0x019A, "Cliff (Vertical) w/ River (South) (Middle)" },
            {0x019B, "Cliff (Vertical) w/ River (South) (Upper)" },
            {0x019C, "Cliff (Vertical) w/ River (South) (Lowest)" },
            {0x019D, "Cliff (Vertical) w/ River (South) (Lower)" },
            {0x019E, "Cliff (Vertical) w/ River (South) (Middle)" },
            {0x019F, "Cliff (Vertical) w/ River (South) (Upper)" },
            {0x01A0, "Cliff (Vertical) (Lowest)" },
            {0x01A1, "Cliff (Vertical) (Lower)" },
            {0x01A2, "Cliff (Vertical) (Middle)" },
            {0x01A3, "Cliff (Vertical) (Upper)" },
            {0x01A4, "Cliff (Left Corner) (Lowest)" },
            {0x01A5, "Cliff (Left Corner) (Lower)" },
            {0x01A6, "Cliff (Left Corner) (Middle)" },
            {0x01A7, "Cliff (Left Corner) (Upper)" },
            {0x01A8, "Cliff (Horizontal) w/ River (East) (Lowest)" },
            {0x01A9, "Cliff (Horizontal) w/ River (East) (Lower)" },
            {0x01AA, "Cliff (Horizontal) w/ River (East) (Middle)" },
            {0x01AB, "Cliff (Horizontal) w/ River (East) (Upper)" },
            {0x01AC, "Cliff (Horizontal) w/ River (West) (Lowest)" },
            {0x01AD, "Cliff (Horizontal) w/ River (West) (Lower)" },
            {0x01AE, "Cliff (Horizontal) w/ River (West) (Middle)" },
            {0x01AF, "Cliff (Horizontal) w/ River (West) (Upper)" },
            {0x01B0, "Cliff (Left Corner) (Lowest)" },
            {0x01B1, "Cliff (Left Corner) (Lower)" },
            {0x01B2, "Cliff (Left Corner) (Middle)" },
            {0x01B3, "Cliff (Left Corner) (Upper)" },
            {0x01B4, "Cliff (Right > Down) (Lowest)" },
            {0x01B5, "Cliff (Right > Down) (Lower)" },
            {0x01B6, "Cliff (Right > Down) (Middle)" },
            {0x01B7, "Cliff (Right > Down) (Upper)" },
            {0x01B8, "Cliff (Vertical) (Lowest)" },
            {0x01B9, "Cliff (Vertical) (Lower)" },
            {0x01BA, "Cliff (Vertical) (Middle)" },
            {0x01BB, "Cliff (Vertical) (Upper)" },
            {0x01BC, "River (Horizontal) (Lowest)" },
            {0x01BD, "River (Horizontal) (Lower)" },
            {0x01BE, "River (Horizontal) (Middle)" },
            {0x01BF, "River (Horizontal) (Upper)" },
            {0x01C0, "River (Horizontal) (Lowest)" },
            {0x01C1, "River (Horizontal) (Lower)" },
            {0x01C2, "River (Horizontal) (Middle)" },
            {0x01C3, "River (Horizontal) (Upper)" },
            {0x01C4, "River (Horizontal) w/ Cliff (Up > Right) (Lowest)" },
            {0x01C5, "River (Horizontal) w/ Cliff (Up > Right) (Lower)" },
            {0x01C6, "River (Horizontal) w/ Cliff (Up > Right) (Middle)" },
            {0x01C7, "River (Horizontal) w/ Cliff (Up > Right) (Upper)" },
            {0x01C8, "Cliff (North > East) w/ River (East) (Lowest)" },
            {0x01C9, "Cliff (North > East) w/ River (East) (Lower)" },
            {0x01CA, "Cliff (North > East) w/ River (East) (Middle)" },
            {0x01CB, "Cliff (North > East) w/ River (East) (Upper)" },
            {0x01CC, "River (Vertical) w/ Cliff (Lowest)" },
            {0x01CD, "River (Vertical) w/ Cliff (Lower)" },
            {0x01CE, "River (Vertical) w/ Cliff (Middle)" },
            {0x01CF, "River (Vertical) w/ Cliff (Upper)" },
            {0x01D0, "River (Down > Left) (Lowest)" },
            {0x01D1, "River (Down > Left) (Lower)" },
            {0x01D2, "River (Down > Left) (Middle)" },
            {0x01D3, "River (Down > Left) (Upper)" },
            {0x01D4, "Cliff (East > South) w/ River (East) (Lowest)" },
            {0x01D5, "Cliff (East > South) w/ River (East) (Lower)" },
            {0x01D6, "Cliff (East > South) w/ River (East) (Middle)" },
            {0x01D7, "Cliff (East > South) w/ River (East) (Upper)" },
            {0x01D8, "Cliff (North > East) w/ River (West) (Lowest)" },
            {0x01D9, "Cliff (North > East) w/ River (West) (Lower)" },
            {0x01DA, "Cliff (North > East) w/ River (West) (Middle)" },
            {0x01DB, "Cliff (North > East) w/ River (West) (Upper)" },
            {0x01DC, "River (Horizontal) (Lowest)" },
            {0x01DD, "River (Horizontal) (Lower)" },
            {0x01DE, "River (Horizontal) (Middle)" },
            {0x01DF, "River (Horizontal) (Upper)" },
            {0x01E0, "River (Left) (Lowest)" },
            {0x01E1, "River (Left) (Lower)" },
            {0x01E2, "River (Left) (Middle)" },
            {0x01E3, "River (Left) (Upper)" },
            {0x01E4, "Cliff (East > South) w/ River (West) (Lowest)" },
            {0x01E5, "Cliff (East > South) w/ River (West) (Lower)" },
            {0x01E6, "Cliff (East > South) w/ River (West) (Middle)" },
            {0x01E7, "Cliff (East > South) w/ River (West) (Upper)" },
            {0x01E8, "Cliff (Right Corner) (Lowest)" },
            {0x01E9, "Cliff (Right Corner) (Lower)" },
            {0x01EA, "Cliff (Right Corner) (Middle)" },
            {0x01EB, "Cliff (Right Corner) (Upper)" },
            {0x01EC, "Cliff (Up > Right) (Lowest)" },
            {0x01ED, "Cliff (Up > Right) (Lower)" },
            {0x01EE, "Cliff (Up > Right) (Middle)" },
            {0x01EF, "Cliff (Up > Right) (Upper)" },
            {0x01F0, "Cliff (Vertical) (Lowest)" },
            {0x01F1, "Cliff (Vertical) (Lower)" },
            {0x01F2, "Cliff (Vertical) (Middle)" },
            {0x01F3, "Cliff (Vertical) (Upper)" },
            {0x01F4, "Cliff (Bottom Right Corner)  (Lowest)" },
            {0x01F5, "Cliff (Bottom Right Corner)  (Lower)" },
            {0x01F6, "Cliff (Bottom Right Corner)  (Middle)" },
            {0x01F7, "Cliff (Bottom Right Corner)  (Upper)" },
            //0x01F8 (Police Station Inside) (Invalid)
            {0x01FC, "Lake (Straight) (Lowest)" },
            {0x01FD, "Lake (Straight) (Lower)" },
            {0x01FE, "Lake (Straight) (Middle)" },
            {0x01FF, "Lake (Straight) (Upper)" },
            {0x0200, "Cliff (Horizontal) w/ Waterfall (Lowest)" },
            {0x0201, "Cliff (Horizontal) w/ Waterfall (Lower)" },
            {0x0202, "Cliff (Horizontal) w/ Waterfall (Middle)" },
            {0x0203, "Cliff (Horizontal) w/ Waterfall (Upper)" },
            {0x0204, "Cliff (Horizontal) w/ Waterfall (Lowest)" },
            {0x0205, "Cliff (Horizontal) w/ Waterfall (Lower)" },
            {0x0206, "Cliff (Horizontal) w/ Waterfall (Middle)" },
            {0x0207, "Cliff (Horizontal) w/ Waterfall (Upper)" },
            {0x0208, "Cliff (Horizontal) w/ River (East) (Lowest)" },
            {0x0209, "Cliff (Horizontal) w/ River (East) (Lower)" },
            {0x020A, "Cliff (Horizontal) w/ River (East) (Middle)" },
            {0x020B, "Cliff (Horizontal) w/ River (East) (Upper)" },
            {0x020C, "Cliff (Horizontal) w/ River (West) (Lowest)" },
            {0x020D, "Cliff (Horizontal) w/ River (West) (Lower)" },
            {0x020E, "Cliff (Horizontal) w/ River (West) (Middle)" },
            {0x020F, "Cliff (Horizontal) w/ River (West) (Upper)" },
            {0x0210, "Cliff (Left > Up) w/ Waterfall (Lowest)" },
            {0x0211, "Cliff (Left > Up) w/ Waterfall (Lower)" },
            {0x0212, "Cliff (Left > Up) w/ Waterfall (Middle)" },
            {0x0213, "Cliff (Left > Up) w/ Waterfall (Upper)" },
            {0x0214, "Cliff /w Waterfall (Horizontal > Down)  (Lowest)" },
            {0x0215, "Cliff /w Waterfall (Horizontal > Down)  (Lower)" },
            {0x0216, "Cliff /w Waterfall (Horizontal > Down)  (Middle)" },
            {0x0217, "Cliff /w Waterfall (Horizontal > Down)  (Upper)" },
            {0x0218, "Cliff (Vertical) (Lowest)" },
            {0x0219, "Cliff (Vertical) (Lower)" },
            {0x021A, "Cliff (Vertical) (Middle)" },
            {0x021B, "Cliff (Vertical) (Upper)" },
            {0x021C, "Cliff (Down > Right) (Lowest)" },
            {0x021D, "Cliff (Down > Right) (Lower)" },
            {0x021E, "Cliff (Down > Right) (Middle)" },
            {0x021F, "Cliff (Down > Right) (Upper)" },
            {0x0220, "River (Vertical) (Lowest)" },
            {0x0221, "River (Vertical) (Lower)" },
            {0x0222, "River (Vertical) (Middle)" },
            {0x0223, "River (Vertical) (Upper)" },
            {0x0224, "River (East) (Lowest)" },
            {0x0225, "River (East) (Lower)" },
            {0x0226, "River (East) (Middle)" },
            {0x0227, "River (East) (Upper)" },
            {0x0228, "River (Horizontal) (Lowest)" },
            {0x0229, "River (Horizontal) (Lower)" },
            {0x022A, "River (Horizontal) (Middle)" },
            {0x022B, "River (Horizontal) (Upper)" },
            {0x022C, "River (Down > Right) (Lowest)" },
            {0x022D, "River (Down > Right) (Lower)" },
            {0x022E, "River (Down > Right) (Middle)" },
            {0x022F, "River (Down > Right) (Upper)" },
            {0x0230, "River (Right > Down) (Lowest)" },
            {0x0231, "River (Right > Down) (Lower)" },
            {0x0232, "River (Right > Down) (Middle)" },
            {0x0233, "River (Right > Down) (Upper)" },
            {0x0234, "River (Down > Left) (Lowest)" },
            {0x0235, "River (Down > Left) (Lower)" },
            {0x0236, "River (Down > Left) (Middle)" },
            {0x0237, "River (Down > Left) (Upper)" },
            //0x0238 < Invalid Acre (Completely Black)
            //0x023C < Invalid Acre (Completely Black)
            //0x0240 (Invalid Empty Acre) < Corrupts map icon index
            {0x0244, "Cliff (East > North) w/ Waterfall (East > South > East) (Lowest)" },
            {0x0245, "Cliff (East > North) w/ Waterfall (East > South > East) (Lower)" },
            {0x0246, "Cliff (East > North) w/ Waterfall (East > South > East) (Middle)" },
            {0x0247, "Cliff (East > North) w/ Waterfall (East > South > East) (Upper)" },
            {0x0248, "Cliff (Vertical) w/ Waterfall (East) (Lowest)" },
            {0x0249, "Cliff (Vertical) w/ Waterfall (East) (Lower)" },
            {0x024A, "Cliff (Vertical) w/ Waterfall (East) (Middle)" },
            {0x024B, "Cliff (Vertical) w/ Waterfall (East) (Upper)" },
            {0x024C, "River (Vertical) w/ Wooden Bridge (Lowest)" },
            {0x024D, "River (Vertical) w/ Wooden Bridge (Lower)" },
            {0x024E, "River (Vertical) w/ Wooden Bridge (Middle)" },
            {0x024F, "River (Vertical) w/ Wooden Bridge (Upper)" },
            {0x0250, "River (East) w/ Wooden Bridge (Lowest)" },
            {0x0251, "River (East) w/ Wooden Bridge (Lower)" },
            {0x0252, "River (East) w/ Wooden Bridge (Middle)" },
            {0x0253, "River (East) w/ Wooden Bridge (Upper)" },
            {0x0254, "River (Down > Right) w/ Wooden Bridge (Lowest)" },
            {0x0255, "River (Down > Right) w/ Wooden Bridge (Lower)" },
            {0x0256, "River (Down > Right) w/ Wooden Bridge (Middle)" },
            {0x0257, "River (Down > Right) w/ Wooden Bridge (Upper)" },
            {0x0258, "River (West) w/ Wooden Bridge (Lowest)" },
            {0x0259, "River (West) w/ Wooden Bridge (Lower)" },
            {0x025A, "River (West) w/ Wooden Bridge (Middle)" },
            {0x025B, "River (West) w/ Wooden Bridge (Upper)" },
            {0x025C, "River (Right > Down) w/ Wooden Bridge (Lowest)" },
            {0x025D, "River (Right > Down) w/ Wooden Bridge (Lower)" },
            {0x025E, "River (Right > Down) w/ Wooden Bridge (Middle)" },
            {0x025F, "River (Right > Down) w/ Wooden Bridge (Upper)" },
            {0x0260, "River (Middle, Down > Left) w/ Bridge (Lowest)" },
            {0x0261, "River (Middle, Down > Left) w/ Bridge (Lower)" },
            {0x0262, "River (Middle, Down > Left) w/ Bridge (Middle)" },
            {0x0263, "River (Middle, Down > Left) w/ Bridge (Upper)" },
            {0x0264, "River (Left > Down) w/ Bridge (Lowest)" },
            {0x0265, "River (Left > Down) w/ Bridge (Lower)" },
            {0x0266, "River (Left > Down) w/ Bridge (Middle)" },
            {0x0267, "River (Left > Down) w/ Bridge (Upper)" },
            {0x0268, "River (Horizontal) w/ Bridge (Lowest)" },
            {0x0269, "River (Horizontal) w/ Bridge (Lower)" },
            {0x026A, "River (Horizontal) w/ Bridge (Middle)" },
            {0x026B, "River (Horizontal) w/ Bridge (Upper)" },
            {0x026C, "River (Right) w/ Bridge (Lowest)" },
            {0x026D, "River (Right) w/ Bridge (Lower)" },
            {0x026E, "River (Right) w/ Bridge (Middle)" },
            {0x026F, "River (Right) w/ Bridge (Upper)" },
            {0x0270, "River (Straight) w/ Stone Bridge (Lowest)" },
            {0x0271, "River (Straight) w/ Stone Bridge (Lower)" },
            {0x0272, "River (Straight) w/ Stone Bridge (Middle)" },
            {0x0273, "River (Straight) w/ Stone Bridge (Upper)" },
            {0x0274, "Empty Acre (3) (Lowest)" },
            {0x0275, "Empty Acre (3) (Lower)" },
            {0x0276, "Empty Acre (3) (Middle)" },
            {0x0277, "Empty Acre (3) (Upper)" },
            {0x0278, "Empty Acre (4) (Lowest)" },
            {0x0279, "Empty Acre (4) (Lower)" },
            {0x027A, "Empty Acre (4) (Middle)" },
            {0x027B, "Empty Acre (4) (Upper)" },
            {0x027C, "Empty Acre (5) (Lowest)" },
            {0x027D, "Empty Acre (5) (Lower)" },
            {0x027E, "Empty Acre (5) (Middle)" },
            {0x027F, "Empty Acre (5) (Upper)" },
            {0x0280, "Empty Acre (6) (Lowest)" },
            {0x0281, "Empty Acre (6) (Lower)" },
            {0x0282, "Empty Acre (6) (Middle)" },
            {0x0283, "Empty Acre (6) (Upper)" },
            {0x0284, "Empty Acre (7) (Lowest)" },
            {0x0285, "Empty Acre (7) (Lower)" },
            {0x0286, "Empty Acre (7) (Middle)" },
            {0x0287, "Empty Acre (7) (Upper)" },
            {0x0288, "Empty Acre (8) (Lowest)" },
            {0x0289, "Empty Acre (8) (Lower)" },
            {0x028A, "Empty Acre (8) (Middle)" },
            {0x028B, "Empty Acre (8) (Upper)" },
            {0x028C, "Empty Acre (9) (Lowest)" },
            {0x028D, "Empty Acre (9) (Lower)" },
            {0x028E, "Empty Acre (9) (Middle)" },
            {0x028F, "Empty Acre (9) (Upper)" },
            {0x0290, "Empty Acre (10) (Lowest)" },
            {0x0291, "Empty Acre (10) (Lower)" },
            {0x0292, "Empty Acre (10) (Middle)" },
            {0x0293, "Empty Acre (10) (Upper)" },
            {0x0294, "Dump (2) (Lowest)" },
            {0x0295, "Dump (2) (Lower)" },
            {0x0296, "Dump (2) (Middle)" },
            {0x0297, "Dump (2) (Upper)" },
            {0x0298, "Dump (3) (Lowest)" },
            {0x0299, "Dump (3) (Lower)" },
            {0x029A, "Dump (3) (Middle)" },
            {0x029B, "Dump (3) (Upper)" },
            //0x029C (Invalid Acre) Train Tracks w/ Pond (1)
            //0x02A0 (Invalid Acre) Train Tracks w/ Pond (2)
            //0x02A4 (Invalid Acre) Train Tracks (1)
            //0x02A8 (Invalid Acre) Train Tracks (2)
            //0x02AC (Invalid Acre) Train Tracks (3)
            //0x02B0 (Invalid Acre) Train Tracks (4)
            //0x02B4 (Invalid Acre) Train Tracks (5)
            {0x02B8, "River w/ Train Track Bridge (2) (Lowest)" },
            {0x02B9, "River w/ Train Track Bridge (2) (Lower)" },
            {0x02BA, "River w/ Train Track Bridge (2) (Middle)" },
            {0x02BB, "River w/ Train Track Bridge (2) (Upper)" },
            {0x02BC, "River w/ Train Track Bridge (3) (Lowest)" },
            {0x02BD, "River w/ Train Track Bridge (3) (Lower)" },
            {0x02BE, "River w/ Train Track Bridge (3) (Middle)" },
            {0x02BF, "River w/ Train Track Bridge (3) (Upper)" },
            {0x02C0, "River w/ Train Track Bridge (4) (Lowest)" },
            {0x02C1, "River w/ Train Track Bridge (4) (Lower)" },
            {0x02C2, "River w/ Train Track Bridge (4) (Middle)" },
            {0x02C3, "River w/ Train Track Bridge (4) (Upper)" },
            {0x02C4, "River w/ Train Track Bridge (5) (Lowest)" },
            {0x02C5, "River w/ Train Track Bridge (5) (Lower)" },
            {0x02C6, "River w/ Train Track Bridge (5) (Middle)" },
            {0x02C7, "River w/ Train Track Bridge (5) (Upper)" },
            {0x02C8, "Lake (Horizontal) (Lowest)" },
            {0x02C9, "Lake (Horizontal) (Lower)" },
            {0x02CA, "Lake (Horizontal) (Middle)" },
            {0x02CB, "Lake (Horizontal) (Upper)" },
            {0x02CC, "Lake (Left > Left) (Lowest)" },
            {0x02CD, "Lake (Left > Left) (Lower)" },
            {0x02CE, "Lake (Left > Left) (Middle)" },
            {0x02CF, "Lake (Left > Left) (Upper)" },
            //0x02D0 (Invalid Grass Acre)
            //0x02D4 (Invalid Grass Acre)
            //0x02D8 (Invalid Grass Acre)
            //0x02DC (Invalid Grass Acre)
            //0x02E0 (Invalid Grass Acre)
            //0x02E4 (Invalid River (South) Acre)
            {0x02E8, "Lake (Down > Left) (Lowest)" },
            {0x02E9, "Lake (Down > Left) (Lower)" },
            {0x02EA, "Lake (Down > Left) (Middle)" },
            {0x02EB, "Lake (Down > Left) (Upper)" },
            {0x02EC, "Lake (Left > Down) (Lowest)" },
            {0x02ED, "Lake (Left > Down) (Lower)" },
            {0x02EE, "Lake (Left > Down) (Middle)" },
            {0x02EF, "Lake (Left > Down) (Upper)" },
            {0x02F0, "Train Station (2) (Lowest)" },
            {0x02F1, "Train Station (2) (Lower)" },
            {0x02F2, "Train Station (2) (Middle)" },
            {0x02F3, "Train Station (2) (Upper)" },
            {0x02F4, "Train Station (3) (Lowest)" },
            {0x02F5, "Train Station (3) (Lower)" },
            {0x02F6, "Train Station (3) (Middle)" },
            {0x02F7, "Train Station (3) (Upper)" },
            {0x02F8, "Lake (Down > Right) (Lowest)" },
            {0x02F9, "Lake (Down > Right) (Lower)" },
            {0x02FA, "Lake (Down > Right) (Middle)" },
            {0x02FB, "Lake (Down > Right) (Upper)" },
            {0x02FC, "Lake (Right > Down) (Lowest)" },
            {0x02FD, "Lake (Right > Down) (Lower)" },
            {0x02FE, "Lake (Right > Down) (Middle)" },
            {0x02FF, "Lake (Right > Down) (Upper)" },
            //0x0300 < Invalid Acre (Katrina's Tent Inside)
            //0x0304 < Invalid Acre (Game Load (Animal Talking/K.K. Slider Opening) Room) w/ Spotlight (No hit boxes!)
            //0x0308 < Invalid Acre? (Completely Black)
            //0x030C < Invalid Acre? (Completely Black)
            //0x0310 (Valid Empty Acre, but has invisible walls surrounding it)
            //0x0314 (Valid River (South) Acre, but has invisible walls as well)
            //0x031C (Invalid Empty Acre (Shows up as a cliff acre on map)
            {0x0320, "Ramp (Horizontal) (Lowest)" },
            {0x0321, "Ramp (Horizontal) (Lower)" },
            {0x0322, "Ramp (Horizontal) (Middle)" },
            {0x0323, "Ramp (Horizontal) (Upper)" },
            {0x0324, "Train Tracks (Lowest)" },
            {0x0325, "Train Tracks (Lower)" },
            {0x0326, "Train Tracks (Middle)" },
            {0x0327, "Train Tracks (Upper)" },
            {0x0328, "Train Track Bridge (Lowest)" },
            {0x0329, "Train Track Bridge (Lower)" },
            {0x032A, "Train Track Bridge (Middle)" },
            {0x032B, "Train Track Bridge (Upper)" },
            {0x032C, "Left Border Wall (3) (Lowest)" },
            {0x032D, "Left Border Wall (3) (Lower)" },
            {0x032E, "Left Border Wall (3) (Middle)" },
            {0x032F, "Left Border Wall (3) (Upper)" },
            {0x0330, "Left Border Cliff (2) (Lowest)" },
            {0x0331, "Left Border Cliff (2) (Lower)" },
            {0x0332, "Left Border Cliff (2) (Middle)" },
            {0x0333, "Left Border Cliff (2) (Upper)" },
            {0x0334, "Left Border Cliff (1) (Lowest)" },
            {0x0335, "Left Border Cliff (1) (Lower)" },
            {0x0336, "Left Border Cliff (1) (Middle)" },
            {0x0337, "Left Border Cliff (1) (Upper)" },
            {0x0338, "Border Cliff (Right) (Lowest)" },
            {0x0339, "Border Cliff (Right) (Lower)" },
            {0x033A, "Border Cliff (Right) (Middle)" },
            {0x033B, "Border Cliff (Right) (Upper)" },
            {0x033C, "Border Cliff (Right > Lower) (Lowest)" },
            {0x033D, "Border Cliff (Right > Lower) (Lower)" },
            {0x033E, "Border Cliff (Right > Lower) (Middle)" },
            {0x033F, "Border Cliff (Right > Lower) (Upper)" },
            {0x0340, "Right Border Wall (1) (Lowest)" },
            {0x0341, "Right Border Wall (1) (Lower)" },
            {0x0342, "Right Border Wall (1) (Middle)" },
            {0x0343, "Right Border Wall (1) (Upper)" },
            {0x0344, "Left Train Tunnel (Lowest)" },
            {0x0345, "Left Train Tunnel (Lower)" },
            {0x0346, "Left Train Tunnel (Middle)" },
            {0x0347, "Left Train Tunnel (Upper)" },
            {0x0348, "Right Train Tunnel (Lowest)" },
            {0x0349, "Right Train Tunnel (Lower)" },
            {0x034A, "Right Train Tunnel (Middle)" },
            {0x034B, "Right Train Tunnel (Upper)" },
            {0x034C, "Police Station (1) (Lowest)" },
            {0x034D, "Police Station (1) (Lower)" },
            {0x034E, "Police Station (1) (Middle)" },
            {0x034F, "Police Station (1) (Upper)" },
            {0x0350, "Police Station (2) (Lowest)" },
            {0x0351, "Police Station (2) (Lower)" },
            {0x0352, "Police Station (2) (Middle)" },
            {0x0353, "Police Station (2) (Upper)" },
            {0x0354, "Police Station (3) (Lowest)" },
            {0x0355, "Police Station (3) (Lower)" },
            {0x0356, "Police Station (3) (Middle)" },
            {0x0357, "Police Station (3) (Upper)" },
            {0x0358, "Player Houses (1) (Lowest)" },
            {0x0359, "Player Houses (1) (Lower)" },
            {0x035A, "Player Houses (1) (Middle)" },
            {0x035B, "Player Houses (1) (Upper)" },
            {0x035C, "Player Houses (2) (Lowest)" },
            {0x035D, "Player Houses (2) (Lower)" },
            {0x035E, "Player Houses (2) (Middle)" },
            {0x035F, "Player Houses (2) (Upper)" },
            {0x0360, "Player Houses (3) (Lowest)" },
            {0x0361, "Player Houses (3) (Lower)" },
            {0x0362, "Player Houses (3) (Middle)" },
            {0x0363, "Player Houses (3) (Upper)" },
            {0x0364, "Wishing Well (1) (Lowest)" },
            {0x0365, "Wishing Well (1) (Lower)" },
            {0x0366, "Wishing Well (1) (Middle)" },
            {0x0367, "Wishing Well (1) (Upper)" },
            {0x0368, "Wishing Well (2) (Lowest)" },
            {0x0369, "Wishing Well (2) (Lower)" },
            {0x036A, "Wishing Well (2) (Middle)" },
            {0x036B, "Wishing Well (2) (Upper)" },
            {0x036C, "Wishing Well (3) (Lowest)" },
            {0x036D, "Wishing Well (3) (Lower)" },
            {0x036E, "Wishing Well (3) (Middle)" },
            {0x036F, "Wishing Well (3) (Upper)" },
            {0x0370, "Post Office (1) (Lowest)" },
            {0x0371, "Post Office (1) (Lower)" },
            {0x0372, "Post Office (1) (Middle)" },
            {0x0373, "Post Office (1) (Upper)" },
            {0x0374, "Nook's Acre (1) (Lowest)" },
            {0x0375, "Nook's Acre (1) (Lower)" },
            {0x0376, "Nook's Acre (1) (Middle)" },
            {0x0377, "Nook's Acre (1) (Upper)" },
            {0x0378, "Nook's Acre (2) (Lowest)" },
            {0x0379, "Nook's Acre (2) (Lower)" },
            {0x037A, "Nook's Acre (2) (Middle)" },
            {0x037B, "Nook's Acre (2) (Upper)" },
            {0x037C, "Nook's Acre (3) (Lowest)" },
            {0x037D, "Nook's Acre (3) (Lower)" },
            {0x037E, "Nook's Acre (3) (Middle)" },
            {0x037F, "Nook's Acre (3) (Upper)" },
            {0x0380, "Post Office (2) (Lowest)" },
            {0x0381, "Post Office (2) (Lower)" },
            {0x0382, "Post Office (2) (Middle)" },
            {0x0383, "Post Office (2) (Upper)" },
            {0x0384, "Post Office (3) (Lowest)" },
            {0x0385, "Post Office (3) (Lower)" },
            {0x0386, "Post Office (3) (Middle)" },
            {0x0387, "Post Office (3) (Upper)" },
            //0x0388 (Invalid? Empty Acre w/ Pond)
            //0x038C (Invalid Black Acre)
            //0x0390 (Invalid Acre, Nookington's Inside)
            //0x0394 (Invalid Black Acre)
            //0x0398 (Invalid Black Acre)
            //0x039C (Beta Test Acre (Invalid Map Acre) w/ Rasied & Lowered pits (Likely for testing beach slopes)
            {0x03A0, "Beachfront (1) (Lowest)" },
            {0x03A1, "Beachfront (1) (Lower)" },
            {0x03A2, "Beachfront (1) (Middle)" },
            {0x03A3, "Beachfront (1) (Upper)" },
            {0x03A4, "Beachfront (2) (Lowest)" },
            {0x03A5, "Beachfront (2) (Lower)" },
            {0x03A6, "Beachfront (2) (Middle)" },
            {0x03A7, "Beachfront (2) (Upper)" },
            //0x03A8 (Unsure what acre this is, causes immediate freeze)
            //0x03AC (Unsure what acre this is, causes immediate freeze)
            {0x03B0, "Beachfront w/ River (1) (Lowest)" },
            {0x03B1, "Beachfront w/ River (1) (Lower)" },
            {0x03B2, "Beachfront w/ River (1) (Middle)" },
            {0x03B3, "Beachfront w/ River (1) (Upper)" },
            {0x03B4, "Beachfront Border Cliff (Left) (Lowest)" },
            {0x03B5, "Beachfront Border Cliff (Left) (Lower)" },
            {0x03B6, "Beachfront Border Cliff (Left) (Middle)" },
            {0x03B7, "Beachfront Border Cliff (Left) (Upper)" },
            {0x03B8, "Beachfront Border Cliff (Right) (Lowest)" },
            {0x03B9, "Beachfront Border Cliff (Right) (Lower)" },
            {0x03BA, "Beachfront Border Cliff (Right) (Middle)" },
            {0x03BB, "Beachfront Border Cliff (Right) (Upper)" },
            //0x03BC (Invalid Black Acre)
            {0x03C0, "Beachfront w/ River (2) (Lowest)" },
            {0x03C1, "Beachfront w/ River (2) (Lower)" },
            {0x03C2, "Beachfront w/ River (2) (Middle)" },
            {0x03C3, "Beachfront w/ River (2) (Upper)" },
            {0x03C4, "Beachfront w/ River (3) (Lowest)" },
            {0x03C5, "Beachfront w/ River (3) (Lower)" },
            {0x03C6, "Beachfront w/ River (3) (Middle)" },
            {0x03C7, "Beachfront w/ River (3) (Upper)" },
            {0x03C8, "Beachfront w/ River (4) (Lowest)" },
            {0x03C9, "Beachfront w/ River (4) (Lower)" },
            {0x03CA, "Beachfront w/ River (4) (Middle)" },
            {0x03CB, "Beachfront w/ River (4) (Upper)" },
            {0x03CC, "Beachfront w/ River (5) (Lowest)" },
            {0x03CD, "Beachfront w/ River (5) (Lower)" },
            {0x03CE, "Beachfront w/ River (5) (Middle)" },
            {0x03CF, "Beachfront w/ River (5) (Upper)" },
            {0x03D0, "Beachfront River w/ Bridge (1) (Lowest)" },
            {0x03D1, "Beachfront River w/ Bridge (1) (Lower)" },
            {0x03D2, "Beachfront River w/ Bridge (1) (Middle)" },
            {0x03D3, "Beachfront River w/ Bridge (1) (Upper)" },
            {0x03D4, "Beachfront River w/ Bridge (2) (Lowest)" },
            {0x03D5, "Beachfront River w/ Bridge (2) (Lower)" },
            {0x03D6, "Beachfront River w/ Bridge (2) (Middle)" },
            {0x03D7, "Beachfront River w/ Bridge (2) (Upper)" },
            {0x03D8, "Beachfront River w/ Bridge (3) (Lowest)" },
            {0x03D9, "Beachfront River w/ Bridge (3) (Lower)" },
            {0x03DA, "Beachfront River w/ Bridge (3) (Middle)" },
            {0x03DB, "Beachfront River w/ Bridge (3) (Upper)" },
            {0x03DC, "Ocean (1) (Lowest)" },
            {0x03DD, "Ocean (1) (Lower)" },
            {0x03DE, "Ocean (1) (Middle)" },
            {0x03DF, "Ocean (1) (Upper)" },
            {0x03E0, "Ocean (2) (Lowest)" },
            {0x03E1, "Ocean (2) (Lower)" },
            {0x03E2, "Ocean (2) (Middle)" },
            {0x03E3, "Ocean (2) (Upper)" },
            {0x03E4, "Ocean (3) (Lowest)" },
            {0x03E5, "Ocean (3) (Lower)" },
            {0x03E6, "Ocean (3) (Middle)" },
            {0x03E7, "Ocean (3) (Upper)" },
            {0x03E8, "Ocean (4) (Lowest)" },
            {0x03E9, "Ocean (4) (Lower)" },
            {0x03EA, "Ocean (4) (Middle)" },
            {0x03EB, "Ocean (4) (Upper)" },
            {0x03EC, "Ocean (5) (Lowest)" },
            {0x03ED, "Ocean (5) (Lower)" },
            {0x03EE, "Ocean (5) (Middle)" },
            {0x03EF, "Ocean (5) (Upper)" },
            {0x03F0, "Beachfront (3) (Lowest)" },
            {0x03F1, "Beachfront (3) (Lower)" },
            {0x03F2, "Beachfront (3) (Middle)" },
            {0x03F3, "Beachfront (3) (Upper)" },
            {0x03F4, "Beachfront (4) (Lowest)" },
            {0x03F5, "Beachfront (4) (Lower)" },
            {0x03F6, "Beachfront (4) (Middle)" },
            {0x03F7, "Beachfront (4) (Upper)" },
            {0x03F8, "Beachfront (5) (Lowest)" },
            {0x03F9, "Beachfront (5) (Lower)" },
            {0x03FA, "Beachfront (5) (Middle)" },
            {0x03FB, "Beachfront (5) (Upper)" },
            {0x03FC, "Beachfront (6) (Lowest)" },
            {0x03FD, "Beachfront (6) (Lower)" },
            {0x03FE, "Beachfront (6) (Middle)" },
            {0x03FF, "Beachfront (6) (Upper)" },
            {0x0400, "Beachfront (7) (Lowest)" },
            {0x0401, "Beachfront (7) (Lower)" },
            {0x0402, "Beachfront (7) (Middle)" },
            {0x0403, "Beachfront (7) (Upper)" },
            {0x0404, "Beachfront (8) (Lowest)" },
            {0x0405, "Beachfront (8) (Lower)" },
            {0x0406, "Beachfront (8) (Middle)" },
            {0x0407, "Beachfront (8) (Upper)" },
            {0x0408, "Beachfront (9) (Lowest)" },
            {0x0409, "Beachfront (9) (Lower)" },
            {0x040A, "Beachfront (9) (Middle)" },
            {0x040B, "Beachfront (9) (Upper)" },
            {0x040C, "Beachfront (10) (Lowest)" },
            {0x040D, "Beachfront (10) (Lower)" },
            {0x040E, "Beachfront (10) (Middle)" },
            {0x040F, "Beachfront (10) (Upper)" },
            {0x0410, "Ramp (Right > Up) (Lowest)" },
            {0x0411, "Ramp (Right > Up) (Lower)" },
            {0x0412, "Ramp (Right > Up) (Middle)" },
            {0x0413, "Ramp (Right > Up) (Upper)" },
            {0x0414, "Cliff (Down > Right) w/ Leftward Waterfall (Lowest)" },
            {0x0415, "Cliff (Down > Right) w/ Leftward Waterfall (Lower)" },
            {0x0416, "Cliff (Down > Right) w/ Leftward Waterfall (Middle)" },
            {0x0417, "Cliff (Down > Right) w/ Leftward Waterfall (Upper)" },
            {0x0418, "Ramp (Left) (Corner Cliff) (Lowest)" },
            {0x0419, "Ramp (Left) (Corner Cliff) (Lower)" },
            {0x041A, "Ramp (Left) (Corner Cliff) (Middle)" },
            {0x041B, "Ramp (Left) (Corner Cliff) (Upper)" },
            {0x041C, "Ramp (Vertical) (South > East) (Lowest)" },
            {0x041D, "Ramp (Vertical) (South > East) (Lower)" },
            {0x041E, "Ramp (Vertical) (South > East) (Middle)" },
            {0x041F, "Ramp (Vertical) (South > East) (Upper)" },
            //0x0420 - 0x047C missing (0x0420 is a valid lake, but invalid acre icon index)
            {0x0480, "Museum (1) (Lowest)" },
            {0x0481, "Museum (1) (Lower)" },
            {0x0482, "Museum (1) (Middle)" },
            {0x0483, "Museum (1) (Upper)" },
            {0x0484, "Museum (2) (Lowest)" },
            {0x0485, "Museum (2) (Lower)" },
            {0x0486, "Museum (2) (Middle)" },
            {0x0487, "Museum (2) (Upper)" },
            {0x0488, "Museum (3) (Lowest)" },
            {0x0489, "Museum (3) (Lower)" },
            {0x048A, "Museum (3) (Middle)" },
            {0x048B, "Museum (3) (Upper)" },
            {0x048C, "Tailor's Shop (1) (Lowest)" },
            {0x048D, "Tailor's Shop (1) (Lower)" },
            {0x048E, "Tailor's Shop (1) (Middle)" },
            {0x048F, "Tailor's Shop (1) (Upper)" },
            {0x0490, "Tailor's Shop (2) (Lowest)" },
            {0x0491, "Tailor's Shop (2) (Lower)" },
            {0x0492, "Tailor's Shop (2) (Middle)" },
            {0x0493, "Tailor's Shop (2) (Upper)" },
            {0x0494, "Tailor's Shop (3) (Lowest)" },
            {0x0495, "Tailor's Shop (3) (Lower)" },
            {0x0496, "Tailor's Shop (3) (Middle)" },
            {0x0497, "Tailor's Shop (3) (Upper)" },
            {0x0498, "Beachfront w/ Dock (1) (Lowest)" },
            {0x0499, "Beachfront w/ Dock (1) (Lower)" },
            {0x049A, "Beachfront w/ Dock (1) (Middle)" },
            {0x049B, "Beachfront w/ Dock (1) (Upper)" },
            {0x049C, "Ocean (6) (Lowest)" },
            {0x049D, "Ocean (6) (Lower)" },
            {0x049E, "Ocean (6) (Middle)" },
            {0x049F, "Ocean (6) (Upper)" },
            {0x04A0, "Island (Right) (1) (Lowest)" },
            {0x04A1, "Island (Right) (1) (Lower)" },
            {0x04A2, "Island (Right) (1) (Middle)" },
            {0x04A3, "Island (Right) (1) (Upper)" },
            {0x04A4, "Island (Left) (1) (Lowest)" },
            {0x04A5, "Island (Left) (1) (Lower)" },
            {0x04A6, "Island (Left) (1) (Middle)" },
            {0x04A7, "Island (Left) (1) (Upper)" },
            {0x04A8, "Ocean (7) (Lowest)" },
            {0x04A9, "Ocean (7) (Lower)" },
            {0x04AA, "Ocean (7) (Middle)" },
            {0x04AB, "Ocean (7) (Upper)" },
            {0x04AC, "Ocean (8) (Lowest)" },
            {0x04AD, "Ocean (8) (Lower)" },
            {0x04AE, "Ocean (8) (Middle)" },
            {0x04AF, "Ocean (8) (Upper)" },
            {0x04B0, "Ocean (9) (Lowest)" },
            {0x04B1, "Ocean (9) (Lower)" },
            {0x04B2, "Ocean (9) (Middle)" },
            {0x04B3, "Ocean (9) (Upper)" },
            {0x04B4, "Ocean (10) (Lowest)" },
            {0x04B5, "Ocean (10) (Lower)" },
            {0x04B6, "Ocean (10) (Middle)" },
            {0x04B7, "Ocean (10) (Upper)" },
            {0x04B8, "Ocean (11) (Lowest)" },
            {0x04B9, "Ocean (11) (Lower)" },
            {0x04BA, "Ocean (11) (Middle)" },
            {0x04BB, "Ocean (11) (Upper)" },
            {0x04BC, "Ocean (12) (Lowest)" },
            {0x04BD, "Ocean (12) (Lower)" },
            {0x04BE, "Ocean (12) (Middle)" },
            {0x04BF, "Ocean (12) (Upper)" },
            {0x04C0, "Ocean (13) (Lowest)" },
            {0x04C1, "Ocean (13) (Lower)" },
            {0x04C2, "Ocean (13) (Middle)" },
            {0x04C3, "Ocean (13) (Upper)" },
            {0x04C4, "Ocean (14) (Lowest)" },
            {0x04C5, "Ocean (14) (Lower)" },
            {0x04C6, "Ocean (14) (Middle)" },
            {0x04C7, "Ocean (14) (Upper)" },
            {0x04C8, "Ocean (15) (Lowest)" },
            {0x04C9, "Ocean (15) (Lower)" },
            {0x04CA, "Ocean (15) (Middle)" },
            {0x04CB, "Ocean (15) (Upper)" },
            {0x04CC, "Ocean (16) (Lowest)" },
            {0x04CD, "Ocean (16) (Lower)" },
            {0x04CE, "Ocean (16) (Middle)" },
            {0x04CF, "Ocean (16) (Upper)" },
            {0x04D0, "Ocean (17) (Lowest)" },
            {0x04D1, "Ocean (17) (Lower)" },
            {0x04D2, "Ocean (17) (Middle)" },
            {0x04D3, "Ocean (17) (Upper)" },
            {0x04D4, "Ocean (18) (Lowest)" },
            {0x04D5, "Ocean (18) (Lower)" },
            {0x04D6, "Ocean (18) (Middle)" },
            {0x04D7, "Ocean (18) (Upper)" },
            {0x04D8, "Ocean (19) (Lowest)" },
            {0x04D9, "Ocean (19) (Lower)" },
            {0x04DA, "Ocean (19) (Middle)" },
            {0x04DB, "Ocean (19) (Upper)" },
            {0x04DC, "Ocean (20) (Lowest)" },
            {0x04DD, "Ocean (20) (Lower)" },
            {0x04DE, "Ocean (20) (Middle)" },
            {0x04DF, "Ocean (20) (Upper)" },
            {0x04E0, "Ocean (21) (Lowest)" },
            {0x04E1, "Ocean (21) (Lower)" },
            {0x04E2, "Ocean (21) (Middle)" },
            {0x04E3, "Ocean (21) (Upper)" },
            {0x04E4, "Ocean (22) (Lowest)" },
            {0x04E5, "Ocean (22) (Lower)" },
            {0x04E6, "Ocean (22) (Middle)" },
            {0x04E7, "Ocean (22) (Upper)" },
            {0x04E8, "Ocean (23) (Lowest)" },
            {0x04E9, "Ocean (23) (Lower)" },
            {0x04EA, "Ocean (23) (Middle)" },
            {0x04EB, "Ocean (23) (Upper)" },
            {0x04EC, "Ocean (24) (Lowest)" },
            {0x04ED, "Ocean (24) (Lower)" },
            {0x04EE, "Ocean (24) (Middle)" },
            {0x04EF, "Ocean (24) (Upper)" },
            //0x04F0
            //0x04F4
            {0x04F8, "Ocean (22) (Lowest)" },
            {0x04F9, "Ocean (22) (Lower)" },
            {0x04FA, "Ocean (22) (Middle)" },
            {0x04FB, "Ocean (22) (Upper)" },
            //0x04FC
            //0x0500
            {0x0504, "Ocean (23) (Lowest)" },
            {0x0505, "Ocean (23) (Lower)" },
            {0x0506, "Ocean (23) (Middle)" },
            {0x0507, "Ocean (23) (Upper)" },
            //0x0508
            //0x050C
            //0x0510
            //0x0514
            {0x0518, "Ocean (24) (Lowest)" },
            {0x0519, "Ocean (24) (Lower)" },
            {0x051A, "Ocean (24) (Middle)" },
            {0x051B, "Ocean (24) (Upper)" },
            {0x051C, "Ocean (25) (Lowest)" },
            {0x051D, "Ocean (25) (Lower)" },
            {0x051E, "Ocean (25) (Middle)" },
            {0x051F, "Ocean (25) (Upper)" },
            //0x0520
            //0x0524
            //0x0528
            //0x052C
            {0x0530, "Ocean (26) (Lowest)" },
            {0x0531, "Ocean (26) (Lower)" },
            {0x0532, "Ocean (26) (Middle)" },
            {0x0533, "Ocean (26) (Upper)" },
            {0x0534, "Ocean (27) (Lowest)" },
            {0x0535, "Ocean (27) (Lower)" },
            {0x0536, "Ocean (27) (Middle)" },
            {0x0537, "Ocean (27) (Upper)" },
            {0x0538, "Ocean (28) (Lowest)" },
            {0x0539, "Ocean (28) (Lower)" },
            {0x053A, "Ocean (28) (Middle)" },
            {0x053B, "Ocean (28) (Upper)" },
            {0x053C, "Ocean (29) (Lowest)" },
            {0x053D, "Ocean (29) (Lower)" },
            {0x053E, "Ocean (29) (Middle)" },
            {0x053F, "Ocean (29) (Upper)" },
            {0x0540, "Ocean (30) (Lowest)" },
            {0x0541, "Ocean (30) (Lower)" },
            {0x0542, "Ocean (30) (Middle)" },
            {0x0543, "Ocean (30) (Upper)" },
            {0x0544, "Ocean (31) (Lowest)" },
            {0x0545, "Ocean (31) (Lower)" },
            {0x0546, "Ocean (31) (Middle)" },
            {0x0547, "Ocean (31) (Upper)" },
            {0x0548, "Ocean (32) (Lowest)" },
            {0x0549, "Ocean (32) (Lower)" },
            {0x054A, "Ocean (32) (Middle)" },
            {0x054B, "Ocean (32) (Upper)" },
            {0x054C, "Ocean (33) (Lowest)" },
            {0x054D, "Ocean (33) (Lower)" },
            {0x054E, "Ocean (33) (Middle)" },
            {0x054F, "Ocean (33) (Upper)" },
            {0x0550, "Ocean (34) (Lowest)" },
            {0x0551, "Ocean (34) (Lower)" },
            {0x0552, "Ocean (34) (Middle)" },
            {0x0553, "Ocean (34) (Upper)" },
            {0x0554, "Ocean (35) (Lowest)" },
            {0x0555, "Ocean (35) (Lower)" },
            {0x0556, "Ocean (35) (Middle)" },
            {0x0557, "Ocean (35) (Upper)" },
            {0x0558, "Ocean (36) (Lowest)" },
            {0x0559, "Ocean (36) (Lower)" },
            {0x055A, "Ocean (36) (Middle)" },
            {0x055B, "Ocean (36) (Upper)" },
            {0x055C, "Ocean (37) (Lowest)" },
            {0x055D, "Ocean (37) (Lower)" },
            {0x055E, "Ocean (37) (Middle)" },
            {0x055F, "Ocean (37) (Upper)" },
            {0x0560, "Ocean (38) (Lowest)" },
            {0x0561, "Ocean (38) (Lower)" },
            {0x0562, "Ocean (38) (Middle)" },
            {0x0563, "Ocean (38) (Upper)" },
            {0x0564, "Ocean (39) (Lowest)" },
            {0x0565, "Ocean (39) (Lower)" },
            {0x0566, "Ocean (39) (Middle)" },
            {0x0567, "Ocean (39) (Upper)" },
            {0x0568, "Ocean (40) (Lowest)" },
            {0x0569, "Ocean (40) (Lower)" },
            {0x056A, "Ocean (40) (Middle)" },
            {0x056B, "Ocean (40) (Upper)" },
            {0x056C, "Ocean (41) (Lowest)" },
            {0x056D, "Ocean (41) (Lower)" },
            {0x056E, "Ocean (41) (Middle)" },
            {0x056F, "Ocean (41) (Upper)" },
            {0x0570, "Ocean (42) (Lowest)" },
            {0x0571, "Ocean (42) (Lower)" },
            {0x0572, "Ocean (42) (Middle)" },
            {0x0573, "Ocean (42) (Upper)" },
            {0x0574, "Ocean (43) (Lowest)" },
            {0x0575, "Ocean (43) (Lower)" },
            {0x0576, "Ocean (43) (Middle)" },
            {0x0577, "Ocean (43) (Upper)" },
            {0x0578, "Ocean (44) (Lowest)" },
            {0x0579, "Ocean (44) (Lower)" },
            {0x057A, "Ocean (44) (Middle)" },
            {0x057B, "Ocean (44) (Upper)" },
            {0x057C, "Ocean (45) (Lowest)" },
            {0x057D, "Ocean (45) (Lower)" },
            {0x057E, "Ocean (45) (Middle)" },
            {0x057F, "Ocean (45) (Upper)" },
            {0x0580, "Ocean (46) (Lowest)" },
            {0x0581, "Ocean (46) (Lower)" },
            {0x0582, "Ocean (46) (Middle)" },
            {0x0583, "Ocean (46) (Upper)" },
            {0x0584, "Ocean (47) (Lowest)" },
            {0x0585, "Ocean (47) (Lower)" },
            {0x0586, "Ocean (47) (Middle)" },
            {0x0587, "Ocean (47) (Upper)" },
            {0x0588, "Ocean (48) (Lowest)" },
            {0x0589, "Ocean (48) (Lower)" },
            {0x058A, "Ocean (48) (Middle)" },
            {0x058B, "Ocean (48) (Upper)" },
            {0x058C, "Ocean (49) (Lowest)" },
            {0x058D, "Ocean (49) (Lower)" },
            {0x058E, "Ocean (49) (Middle)" },
            {0x058F, "Ocean (49) (Upper)" },
            {0x0594, "Island (Right) (2) (Lowest)" },
            {0x0595, "Island (Right) (2) (Lower)" },
            {0x0596, "Island (Right) (2) (Middle)" },
            {0x0597, "Island (Right) (2) (Upper)" },
            {0x0598, "Island (Left) (2) (Lowest)" },
            {0x0599, "Island (Left) (2) (Lower)" },
            {0x059A, "Island (Left) (2) (Middle)" },
            {0x059B, "Island (Left) (2) (Upper)" },
            {0x059C, "Island (Right) (3) (Lowest)" },
            {0x059D, "Island (Right) (3) (Lower)" },
            {0x059E, "Island (Right) (3) (Middle)" },
            {0x059F, "Island (Right) (3) (Upper)" },
            {0x05A0, "Island (Left) (3) (Lowest)" },
            {0x05A1, "Island (Left) (3) (Lower)" },
            {0x05A2, "Island (Left) (3) (Middle)" },
            {0x05A3, "Island (Left) (3) (Upper)" },
            {0x05A4, "Island (Right) (4) (Lowest)" },
            {0x05A5, "Island (Right) (4) (Lower)" },
            {0x05A6, "Island (Right) (4) (Middle)" },
            {0x05A7, "Island (Right) (4) (Upper)" },
            {0x05A8, "Island (Left) (4) (Lowest)" },
            {0x05A9, "Island (Left) (4) (Lower)" },
            {0x05AA, "Island (Left) (4) (Middle)" },
            {0x05AB, "Island (Left) (4) (Upper)" },
            {0x05AC, "Beachfront w/ Dock (2) (Lowest)" },
            {0x05AD, "Beachfront w/ Dock (2) (Lower)" },
            {0x05AE, "Beachfront w/ Dock (2) (Middle)" },
            {0x05AF, "Beachfront w/ Dock (2) (Upper)" },
            {0x05B0, "Beachfront w/ Dock (3) (Lowest)" },
            {0x05B1, "Beachfront w/ Dock (3) (Lower)" },
            {0x05B2, "Beachfront w/ Dock (3) (Middle)" },
            {0x05B3, "Beachfront w/ Dock (3) (Upper)" },
            {0x05B4, "Ocean (50) (Lowest)" },
            {0x05B5, "Ocean (50) (Lower)" },
            {0x05B6, "Ocean (50) (Middle)" },
            {0x05B7, "Ocean (50) (Upper)" },
            {0x05B8, "Ocean (51) (Lowest)" },
            {0x05B9, "Ocean (51) (Lower)" },
            {0x05BA, "Ocean (51) (Middle)" },
            {0x05BB, "Ocean (51) (Upper)" },
            //0x05BC
            //0x05C0+??
        };

        public static Dictionary<ushort, int> CliffAcres = new Dictionary<ushort, int>()
        {
            {0x0334, 71 },
            {0x0340, 71 },
            {0x0330, 73 },
            {0x0338, 71 },
            {0x032C, 72 },
            {0x033C, 73 }, //Cliff (Right Lower Acre)
            {0x03B4, 74 }, //Beachfront Cliff (Left Lower Acre)
            {0x03B8, 74 }, //Beachfront Cliff (Right)
        };
        //Leaving this for acre image docs
        public static Dictionary<ushort, int> AcreImages = new Dictionary<ushort, int>()
        {
            {0x0345, 79 },
            {0x0346, 84 }, //Third Layer > 4th layer left transition cliff
            {0x0325, 81 },
            {0x0326, 87 },
            {0x0329, 80 },
            {0x032A, 85 },
            {0x0349, 79 },
            {0x034A, 84 }, //Third layer > 4th layer right transition cliff
            {0x0335, 71 },
            {0x0331, 79 },
            {0x0336, 88 },
            {0x0342, 88 },
            {0x032E, 88 },
            {0x033D, 79 },
            {0x0385, 2 },
            {0x0295, 4 },
            {0x02F5, 3 },
            {0x02C1, 5 },
            {0x0375, 1 },
            {0x0341, 71 },
            {0x0330, 73 },
            {0x0160, 57 },
            {0x012C, 54 },
            {0x035D, 6 },
            {0x022D, 29 },
            {0x02FD, 16 },
            {0x0339, 71 },
            {0x032C, 72 },
            {0x0278, 10 },
            {0x01B8, 49 },
            {0x0185, 17 },
            {0x01E1, 20 },
            {0x0261, 27 },
            {0x0488, 7 },
            {0x01A4, 61 },
            {0x0084, 36 },
            {0x0088, 58 },
            {0x00C0, 53 },
            {0x0290, 10 }, //Lower empty acre
            {0x036C, 9 }, //lower wishing well
            {0x024C, 12 }, //Lower river
            {0x034C, 8 }, //lower police station
            {0x021C, 61 }, //Cliff, Down > Right
            {0x033C, 73 }, //Cliff (Right Lower Acre)
            {0x03B4, 74 }, //Beachfront Cliff (Left Lower Acre)
            {0x03F8, 63 }, //Goes with Ocean (26) (0x0530)
            {0x03FC, 63 }, //Goes with Ocean (27) (0x0534)
            {0x03B0, 65 }, //Beachfront w/ river (no bridge)
            {0x0494, 64 }, //Goes with Ocean (42) (0x0570)
            {0x0498, 67 }, //Goes with Ocean (43) (0x0574)
            {0x03B8, 74 }, //Beachfront Cliff (Right)
            {0x0518, 70 }, //Ocean Border Left
            {0x0530, 70 }, //Ocean Far Left
            {0x0534, 70 }, //Ocean Left
            {0x0548, 70 }, //Ocean Middle
            {0x0570, 70 }, //Ocean Right
            {0x0574, 70 }, //Ocean Far Right
            {0x051C, 70 }, //Ocean Border Right
            {0x0381, 2 }, //Post Office
            {0x0155, 3 }, //Train Station (Orange Roof)
            {0x02B9, 5 }, //Train Bridge (2)
            {0x037D, 1 }, //Nook's Acre (2)
            {0x028D, 10 }, //Empty Acre Middle
            {0x0361, 6 }, //Player House Acre (2)
            {0x0269, 12 }, //River (Middle) Down w/ Bridge
            {0x0275, 10 }, //Empy Acre Middle (2)
            {0x0281, 10 }, //Empty Acre Middle (3)
            {0x01C4, 35 }, //River (Middle) Down w/ Cliff (Up > Right)
            {0x0194, 54 }, //Ramp (Middle > Middle) Straight > Down
            {0x0364, 9 }, //Wishing Well (Lower) (2)
            {0x00D4, 61 }, //Cliff (Middle > Lower) Down > Straight
            {0x015C, 57 }, //Cliff (Middle > Lower) Straight
            {0x0210, 37 }, //Cliff (Waterfall, Middle > Lower) Straight > Up
            {0x0184, 17 }, //River (Lower, Left > Down)
            {0x02CC, 25 }, //Lake (Lower, Straight > Straight)
            {0x0110, 27 }, //River (Lower, Down > Left) w/ Bridge
            {0x0338, 72 }, //Border Cliff (Lower)
            {0x03CC, 65 }, //Goes with Ocean (36) (0x0558)
            {0x040C, 63 }, //Goes with Ocean (31) (0x0544)
            {0x0490, 64 }, //Goes with Ocean (41) (0x056C)
            {0x0558, 70 }, //Ocean
            {0x0544, 70 }, //Ocean
            {0x056C, 70 }, //Ocean
            {0x03DC, 70 }, //Empty (Ocean)
            {0x03E8, 70 }, //Empty (Ocean)
            {0x04B8, 70 }, //Ocean (Half)
            {0x0578, 70 }, //Ocean
            {0x04A4, 89 }, //Island (Left) (1) 78
            {0x04A0, 90 }, //Island (Right (1) 77
            {0x057C, 70 }, //Ocean
            {0x04D8, 70 }, //Ocean
            {0x04D4, 70 }, //Ocean
            {0x03E0, 70 }, //Empty (Ocean)
            {0x058C, 70 }, //Ocean
            {0x0588, 70 }, //Ocean
            {0x0584, 70 }, //Ocean
            {0x0580, 70 }, //Ocean
            {0x0371, 2 }, //Post Office (3)
            {0x032D, 71 }, //Cliff (Middle, Left Boundary)
            {0x01EC, 55 }, //Cliff (Middle, Up > Right)
            {0x011C, 58 }, //Cliff (Ramp Middle, Straight)
            {0x0200, 36 }, //Cliff (Waterfall, Straight)
            {0x009C, 57 }, //Cliff (Middle, Straight)
            {0x016C, 59 }, //Cliff (Middle, Right > Up)
            {0x00F0, 17 }, //River (Lower, Left > Down)
            {0x02E8, 28 }, //Lake (Lower, Down > Left)
            {0x0094, 10 }, //Empty Acre (Lower) (2)
            {0x0178, 29 }, //River (Lower, Down > Right)
            {0x01DC, 23 }, //River (Lower, Right)
            {0x00E8, 75 }, //River (Lower, Right > Down)
            {0x0274, 10 }, //Empty Acre (Lower) (3)
            {0x0350, 8 }, //Police Station (Lower) (2)
            {0x03D8, 66 }, //Beachfront (River) /w Bridge
            {0x0400, 63 }, //Goes with Ocean (28) (0x0538)
            {0x05AC, 67 }, //Goes with Ocean (50) (0x05B4)
            {0x0564, 70 }, //Ocean
            {0x0538, 70 }, //Ocean
            {0x05B4, 70 }, //Ocean
            {0x03EC, 70 }, //Ocean (Empty)
            {0x04AC, 70 }, //Ocean (Half Empty)
            {0x0598, 82 }, //Island (Left) (2) 78
            {0x04D0, 70 }, //Ocean (Half Empty)
            {0x03E4, 70 }, //Ocean (Empty)
            {0x0000, 100 }, //No Data
            {0x04A8, 70 }, //Ocean
            {0x04CC, 70 }, //Ocean
            {0x04C8, 70 }, //Ocean
            {0x02C5, 5 }, //River (Middle Vertical) (2)
            {0x0285, 10 }, //Empty Acre (Middle) (4)
            {0x0279, 10 }, //Empty Acre (Middle) (5)
            {0x01B0, 55 }, //Cliff (Middle Left Corner)
            {0x006C, 34 }, //Cliff (Middle Right Corner) w/ Waterfall
            {0x0291, 10 }, //Empty Acre (Middle) (6)
            {0x01CC, 38 }, //River (Lower Horizontal) w/ Cliff
            {0x0320, 58 }, //Ramp (Middle Horizontal)
            {0x0264, 76 }, //River (Lower Left > Down) w/ Bridge
            {0x048C, 64 }, //Goes with Ocean (40) (0x0568)
            {0x03A0, 63 }, //Goes with Ocean (6) (0x049C)
            {0x0568, 70 }, //Ocean
            {0x049C, 70 }, //Ocean
            {0x04C0, 70 }, //Ocean
            {0x04BC, 70 }, //Ocean
            {0x0119, 4 }, //Dump (2)
            {0x02F1, 3 }, //Train Station (Green Roof)
            {0x0071, 5 }, //River (Middle Horizontal) (3)
            {0x0095, 10 }, //Empty Acre (Middle) (7)
            {0x024D, 12 }, //River (Middle Vertical) w/ Bridge
            {0x00B4, 55 }, //Cliff (Middle Left Corner)
            {0x018C, 58 }, //Ramp (Middle Horizontal)
            {0x0284, 10 }, //Empty Acre (Lower) (4)
            {0x0100, 12 }, //River (Lower Vertical) w/ Bridge
            {0x0354, 8 }, //Police Station (3)
            {0x02EC, 19 }, //Lake (Lower Left > Down)
            {0x01D0, 26 }, //River (Lower Down > Left)
            {0x0480, 7 }, //Museum (2)
            {0x0404, 63 }, //Beachfront
            {0x053C, 70 }, //Ocean
            {0x05A4, 77 }, //Island (Right) (2)
            {0x04DC, 70 }, //Ocean
            {0x04B0, 70 }, //Ocean
            {0x02E9, 28 },
            {0x026D, 24},
            {0x017D, 75 },
            {0x0164, 57 },
            {0x0204, 36 },
            {0x01E8, 53 },
            {0x0220, 11 },
            {0x03D4, 66 }, //Goes with Ocean (38) (0x0560)
            {0x0560, 70 },
            {0x05A0, 78 },
            {0x0594, 77 }, //77
            {0x0379, 1 },
            {0x00E5, 29 },
            {0x0359, 6 },
            {0x02C9, 22 },
            {0x0061, 15 },
            {0x0214, 34 },
            {0x0099, 10 },
            {0x0484, 7 },
            {0x05A8, 78 },
            {0x0115, 76},
            {0x01D1, 26 },
            {0x027C, 10 },
            {0x0138, 38 },
            {0x028C, 10 },
            {0x0234, 26 },
            {0x01B4, 53 },
            {0x0265, 76 },
            {0x00ED, 26 },
            {0x0134, 62 },
            {0x0410, 60 },
            {0x0098, 10 },
            {0x03C8, 65 }, //Goes with Ocean (35) (0x0554)
            {0x03F4, 63 }, //Goes with Ocean (23) (0x0504)
            {0x05B0, 67 }, //Goes with Ocean (51) (0x05B8)
            {0x0554, 70 },
            {0x0504, 70 },
            {0x05B8, 70 },
            {0x0101, 12 },
            {0x0180, 18 },
            {0x022C, 29 },
            {0x0280, 10 },
            {0x04C4, 70 },
            {0x01FD, 13 },
            {0x00CC, 49 },
            {0x00A8, 59 },
            {0x03D0, 66 }, //Goes with Ocean (37) (0x055C)
            {0x055C, 70 },
            {0x059C, 83 }, //77
            {0x0299, 4 },
            {0x027D, 10 },
            {0x0188, 60 },
            {0x0114, 76 },
            {0x03C0, 65 },
            {0x054C, 70 },
            {0x04B4, 70 },
            {0x02BD, 5 },
            {0x0368, 9},
            {0x00E0, 23 },
            {0x01C0, 23 },
            {0x00EC, 26 },
            {0x00E4, 30 },
            {0x0230, 75 },
            {0x0408, 63 }, //Goes with Ocean (30) (0x0540)
            {0x0540, 70 },
            {0x0060, 15 },
            {0x02ED, 19 },
            {0x0289, 10 },
            {0x0235, 26 },
            {0x0271, 21 },
            {0x0168, 57 },
            {0x00E9, 75 },
            {0x00C8, 41 },
            {0x0414, 48 },
            {0x01FC, 13 },
            {0x0255, 30 },
            {0x0231, 75 },
            {0x00DC, 20 },
            {0x017C, 75 },
            {0x03F0, 63 }, //Goes with Ocean (22) (0x04F8)
            {0x03A4, 63 }, //Goes with Ocean (21) (0x04E8)
            {0x04F8, 70 },
            {0x04E8, 70 },
            {0x0179, 29 },
            {0x00B8, 35 },
            {0x0190, 56 },
            {0x02C8, 22 },
            {0x0288, 10 },
            {0x0171, 11 },
            {0x0218, 49 },
            {0x025D, 15 },
            {0x0418, 62 },
            {0x0254, 30 },
            {0x02FC, 16 },
            {0x00D9, 11 },
            {0x0111, 27 },
            {0x0130, 50 },
            {0x01F0, 51 },
            {0x037E, 1 },
            {0x02BA, 5 },
            {0x02F2, 3 },
            {0x029A, 4 },
            {0x01B5, 53 },
            {0x024E, 12 },
            {0x035E, 6 },
            {0x0191, 56 },
            {0x0135, 62 },
            {0x0201, 36 },
            {0x0161, 57 },
            {0x00A9, 59 },
            {0x0355, 8 },
            {0x0369, 9 },
            {0x0128, 56 },
            {0x009D, 57 },
            {0x0382, 2 },
            {0x00F1, 17 },
            {0x0170, 11 },
            {0x037A, 1 },
            {0x02C2, 5 },
            {0x02F6, 3 },
            {0x0296, 4 },
            {0x0386, 2 },
            {0x027A, 10 },
            {0x0102, 12 },
            {0x035A, 6 },
            {0x015D, 57 },
            {0x0089, 58 },
            {0x0205, 36 },
            {0x0165, 57 },
            {0x0489, 7 },
            {0x0229, 20 },
            {0x0181, 17 },
            {0x03C4, 65 }, //Goes with Ocean (34)
            {0x0550, 70 }, //Goes with Beacfront w/ River (4) : 0x03C4
            {0x0174, 11 },
            {0x0124, 50 },
            {0x008C, 37 },
            {0x01E0, 20 },
            {0x010C, 30 },
            {0x0120, 60 },
            {0x02CD, 22 },
            {0x01A0, 49 },
            {0x01F4, 59 },
            {0x0221, 11 },
            {0x02F8, 31 },
            {0x01BC, 20 },
            {0x025C, 15 },
        };

        public static Dictionary<ushort, int> Acre_Image_Index = new Dictionary<ushort, int>()
        {
            {0x0000, 100},
            {0x0060, 15},
            {0x006C, 34},
            {0x0070, 5},
            {0x0084, 36},
            {0x0088, 58},
            {0x008C, 37},
            {0x0090, 47},
            {0x0094, 10},
            {0x0098, 10},
            {0x009C, 57},
            {0x00A0, 45},
            {0x00A4, 45},
            {0x00A8, 59},
            {0x00AC, 49},
            {0x00B0, 32},
            {0x00B4, 55},
            {0x00B8, 35},
            {0x00BC, 43},
            {0x00C0, 53},
            {0x00C4, 41},
            {0x00C8, 41},
            {0x00CC, 49},
            {0x00D0, 32},
            {0x00D4, 61},
            {0x00D8, 11},
            {0x00DC, 20},
            {0x00E0, 23},
            {0x00E4, 29},
            {0x00E8, 75},
            {0x00EC, 26},
            {0x00F0, 17},
            {0x00F4, 39},
            {0x00F8, 48},
            {0x00FC, 39},
            {0x0100, 12},
            {0x0104, 21},
            {0x0108, 21},
            {0x010C, 30},
            {0x0110, 27},
            {0x0114, 76},
            {0x0118, 4},
            {0x011C, 58},
            {0x0120, 60},
            {0x0124, 50},
            {0x0128, 56},
            {0x012C, 54},
            {0x0130, 50},
            {0x0134, 62},
            {0x0138, 38},
            {0x013C, 43},
            {0x0154, 3},
            {0x015C, 57},
            {0x0160, 57},
            {0x0164, 57},
            {0x0168, 57},
            {0x016C, 59},
            {0x0170, 11},
            {0x0174, 11},
            {0x0178, 29},
            {0x017C, 75},
            {0x0180, 18},
            {0x0184, 17},
            {0x0188, 60},
            {0x018C, 58},
            {0x0190, 56},
            {0x0194, 54},
            {0x0198, 32},
            {0x019C, 32},
            {0x01A0, 49},
            {0x01A4, 61},
            {0x01A8, 45},
            {0x01AC, 45},
            {0x01B0, 55},
            {0x01B4, 53},
            {0x01B8, 49},
            {0x01BC, 20},
            {0x01C0, 23},
            {0x01C4, 35},
            {0x01C8, 43},
            {0x01CC, 38},
            {0x01D0, 26},
            {0x01D4, 41},
            {0x01D8, 43},
            {0x01DC, 23},
            {0x01E0, 20},
            {0x01E4, 41},
            {0x01E8, 53},
            {0x01EC, 55},
            {0x01F0, 51},
            {0x01F4, 59},
            {0x01FC, 13},
            {0x0200, 36},
            {0x0204, 36},
            {0x0208, 45},
            {0x020C, 45},
            {0x0210, 37},
            {0x0214, 34},
            {0x0218, 49},
            {0x021C, 61},
            {0x0220, 11},
            {0x0224, 20},
            {0x0228, 20},
            {0x022C, 29},
            {0x0230, 75},
            {0x0234, 26},
            {0x0244, 47},
            {0x0248, 39},
            {0x024C, 12},
            {0x0250, 21},
            {0x0254, 30},
            {0x0258, 21},
            {0x025C, 15},
            {0x0260, 27},
            {0x0264, 76},
            {0x0268, 12},
            {0x026C, 24},
            {0x0270, 21},
            {0x0274, 10},
            {0x0278, 10},
            {0x027C, 10},
            {0x0280, 10},
            {0x0284, 10},
            {0x0288, 10},
            {0x028C, 10},
            {0x0290, 10},
            {0x0294, 4},
            {0x0298, 4},
            {0x02B8, 5},
            {0x02BC, 5},
            {0x02C0, 5},
            {0x02C4, 5},
            {0x02C8, 22},
            {0x02CC, 25},
            {0x02E8, 28},
            {0x02EC, 19},
            {0x02F0, 3},
            {0x02F4, 3},
            {0x02F8, 31},
            {0x02FC, 16},
            {0x0320, 58},
            {0x0324, 81},
            {0x0326, 87},
            {0x0328, 80},
            {0x032A, 85},
            {0x032C, 72},
            {0x032D, 71},
            {0x032E, 88},
            {0x0330, 73},
            {0x0331, 79},
            {0x0334, 71},
            {0x0335, 71},
            {0x0336, 88},
            {0x0338, 72},
            {0x0339, 71},
            {0x033C, 73},
            {0x033D, 79},
            {0x0340, 88},
            {0x0341, 71},
            {0x0342, 88},
            {0x0344, 79},
            {0x0346, 84},
            {0x0348, 79},
            {0x034A, 84},
            {0x034C, 8},
            {0x0350, 8},
            {0x0354, 8},
            {0x0358, 6},
            {0x035C, 6},
            {0x0360, 6},
            {0x0364, 9},
            {0x0368, 9},
            {0x036C, 9},
            {0x0370, 2},
            {0x0374, 1},
            {0x0378, 1},
            {0x037C, 1},
            {0x0380, 2},
            {0x0384, 2},
            {0x03A0, 63},
            {0x03A4, 63},
            {0x03B0, 65},
            {0x03B4, 74},
            {0x03B8, 74},
            {0x03C0, 65},
            {0x03C4, 65},
            {0x03C8, 65},
            {0x03CC, 65},
            {0x03D0, 66},
            {0x03D4, 66},
            {0x03D8, 66},
            {0x03DC, 70},
            {0x03E0, 70},
            {0x03E4, 70},
            {0x03E8, 70},
            {0x03EC, 70},
            {0x03F0, 63},
            {0x03F4, 63},
            {0x03F8, 63},
            {0x03FC, 63},
            {0x0400, 63},
            {0x0404, 63},
            {0x0408, 63},
            {0x040C, 63},
            {0x0410, 60},
            {0x0414, 48},
            {0x0418, 62},
            {0x041C, 62},
            {0x0480, 7},
            {0x0484, 7},
            {0x0488, 7},
            {0x048C, 64},
            {0x0490, 64},
            {0x0494, 64},
            {0x0498, 67},
            {0x049C, 70},
            {0x04A0, 90},
            {0x04A4, 89},
            {0x04A8, 70},
            {0x04AC, 70},
            {0x04B0, 70},
            {0x04B4, 70},
            {0x04B8, 70},
            {0x04BC, 70},
            {0x04C0, 70},
            {0x04C4, 70},
            {0x04C8, 70},
            {0x04CC, 70},
            {0x04D0, 70},
            {0x04D4, 70},
            {0x04D8, 70},
            {0x04DC, 70},
            {0x04E0, 70},
            {0x04E4, 70},
            {0x04E8, 70},
            {0x04EC, 70},
            {0x04F8, 70},
            {0x0504, 70},
            {0x0518, 70},
            {0x051C, 70},
            {0x0530, 70},
            {0x0534, 70},
            {0x0538, 70},
            {0x053C, 70},
            {0x0540, 70},
            {0x0544, 70},
            {0x0548, 70},
            {0x054C, 70},
            {0x0550, 70},
            {0x0554, 70},
            {0x0558, 70},
            {0x055C, 70},
            {0x0560, 70},
            {0x0564, 70},
            {0x0568, 70},
            {0x056C, 70},
            {0x0570, 70},
            {0x0574, 70},
            {0x0578, 70},
            {0x057C, 70},
            {0x0580, 70},
            {0x0584, 70},
            {0x0588, 70},
            {0x058C, 70},
            {0x0594, 91},
            {0x0598, 82},
            {0x059C, 83},
            {0x05A0, 92},
            {0x05A4, 93},
            {0x05A8, 94},
            {0x05AC, 67},
            {0x05B0, 67},
            {0x05B4, 70},
            {0x05B8, 70},
        };

        public static Dictionary<ushort, Image> Unique_Acre_Images = new Dictionary<ushort, Image>();
        public static Dictionary<string, Image> Acre_Resource_Images = new Dictionary<string, Image>();

        public static string[] Acre_Height_Identifiers = new string[4]
        {
            "Lower", "Middle", "Upper", "Uppermost" //"Uppermost" subject to change
        };

        public static void Parse_Unique_Images()
        {
            if (Directory.Exists(NewMainForm.Assembly_Location + "\\Resources")
                 && Directory.Exists(NewMainForm.Assembly_Location + "\\Resources\\Images"))
                foreach (string file in Directory.GetFiles(NewMainForm.Assembly_Location + "\\Resources\\Images\\Acre_Images", "*.jpg"))
                {
                    ushort.TryParse(Path.GetFileNameWithoutExtension(file), NumberStyles.AllowHexSpecifier, null, out ushort Acre_ID_Out);
                    if (Acre_ID_Out != 0xFFFF)
                        Unique_Acre_Images.Add(Acre_ID_Out, Image.FromFile(file));
                }
        }

        public static void Parse_Images()
        {
            if (Directory.Exists(NewMainForm.Assembly_Location + "\\Resources")
                && Directory.Exists(NewMainForm.Assembly_Location + "\\Resources\\Images"))
                foreach (string file in Directory.GetFiles(NewMainForm.Assembly_Location + "\\Resources\\Images"))
                    Acre_Resource_Images.Add(Path.GetFileNameWithoutExtension(file), Image.FromFile(file));
        }

        public static Dictionary<string, Image> GetAcreImageSet(SaveType Save_Type)
        {
            Dictionary<string, Image> Image_List = new Dictionary<string, Image>();
            string Image_Dir = NewMainForm.Assembly_Location + "\\Resources\\Images\\";
            if (Save_Type == SaveType.Animal_Crossing)
                Image_Dir += "Acre_Images";
            else if (Save_Type == SaveType.Wild_World)
                Image_Dir += "WW_Acre_Images";
            else if (Save_Type == SaveType.City_Folk)
                Image_Dir += "CF_Acre_Images";
            else if (Save_Type == SaveType.New_Leaf)
                Image_Dir += "NL_Acre_Images";
            else if (Save_Type == SaveType.Welcome_Amiibo)
                Image_Dir += "WA_Acre_Images";
            if (Directory.Exists(Image_Dir))
                foreach (string File in Directory.GetFiles(Image_Dir))
                    Image_List.Add((Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
                        ? ushort.Parse(Path.GetFileNameWithoutExtension(File).Replace("acre_", "")).ToString("X4") : Path.GetFileNameWithoutExtension(File), Image.FromFile(File));
            else
                MessageBox.Show("Acre Images Folder doesn't exist!");
            return Image_List;
        }

        public static Bitmap ToAcrePicture(ushort Acre_ID)
        {
            ushort Base_Acre_ID = (ushort)(Acre_ID - (Acre_ID % 4));
            if (((Base_Acre_ID >= 0x03DC && Base_Acre_ID <= 0x03EC) || Base_Acre_ID == 0x049C || (Base_Acre_ID >= 0x04A8 && Base_Acre_ID <= 0x058C) || (Base_Acre_ID >= 0x05B4 && Base_Acre_ID <= 0x5B8)) && Unique_Acre_Images.ContainsKey(0x03DC))
                return (Bitmap)Unique_Acre_Images[0x03DC];
            if (Unique_Acre_Images.ContainsKey(Base_Acre_ID))
                return (Bitmap)Unique_Acre_Images[Base_Acre_ID];
            if (Acre_Image_Index.ContainsKey(Acre_ID))
                return (Bitmap)Acre_Resource_Images[Acre_Image_Index[Acre_ID].ToString()]; //Get Border acre images first!
            int Image_Idx = Acre_Image_Index.ContainsKey(Base_Acre_ID) ? Acre_Image_Index[Base_Acre_ID] : 99;
            return Acre_Resource_Images.ContainsKey(Image_Idx.ToString()) ? (Bitmap)Acre_Resource_Images[Image_Idx.ToString()] : (Bitmap)Acre_Resource_Images["99"];
        }

        public static int ToAcrePictureID(ushort Acre_ID)
        {
            ushort Base_Acre_ID = (ushort)(Acre_ID - (Acre_ID % 4));
            if ((Base_Acre_ID >= 0x03DC && Base_Acre_ID <= 0x03EC) || Base_Acre_ID == 0x049C || (Base_Acre_ID >= 0x04A8 && Base_Acre_ID <= 0x058C) || (Base_Acre_ID >= 0x05B4 && Base_Acre_ID <= 0x5B8))
                return 0x03DC;
            if (Unique_Acre_Images.ContainsKey(Base_Acre_ID))
                return Base_Acre_ID;
            if (Acre_Image_Index.ContainsKey(Acre_ID))
                return Acre_Image_Index[Acre_ID]; //Get Border acre images first!
            return Acre_Image_Index.ContainsKey(Base_Acre_ID) ? Acre_Image_Index[Base_Acre_ID] : 99;
        }

        public static string ToAcreName(ushort Acre_ID)
        {
            ushort Base_Acre_ID = (ushort)(Acre_ID - (Acre_ID % 4));
            string Base_Name = Acres.ContainsKey(Base_Acre_ID) ? Acres[Base_Acre_ID] : "Unknown Acre";
            return (Base_Name + " " + Acre_Height_Identifiers[Acre_ID % 4]);
        }

        public Dictionary<int, Item> GetAcreData(ushort[] acreBuffer)
        {
            Dictionary<int, Item> acreData = new Dictionary<int, Item> { };
            int i = 0;
            foreach (ushort cellData in acreBuffer)
            {
                acreData.Add(i, new Item(cellData));
                i++;
            }
            return acreData;
        }

        public static ushort[] ClearWeeds(ushort[] acreBuffer)
        {
            int WeedsCleared = 0;
            for (int i = 0; i < acreBuffer.Length; i++)
            {
                if (acreBuffer[i] >= 0x08 && acreBuffer[i] <= 0x0A)
                {
                    acreBuffer[i] = 0x00;
                    WeedsCleared++;
                }
            }
            MessageBox.Show("Weeds Cleared: " + WeedsCleared.ToString());
            return acreBuffer;
        }

        public static ushort[] ClearTown(ushort[] buffer)
        {
            int itemsCleared = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] < 0x4000)
                {
                    buffer[i] = 0x00;
                    itemsCleared++;
                }
            }
            MessageBox.Show("Items Cleared: " + itemsCleared.ToString());
            return buffer;
        }

        public static string GetAcreName(ushort acreId)
        {
            foreach (KeyValuePair<ushort, string> acreData in Acres)
            {
                if (acreId == acreData.Key)
                    return acreData.Value;
            }
            return "Unknown";
        }

        public static Dictionary<int, Acre> GetAcreTileData(ushort[] acreBuffer)
        {
            Dictionary<int, Acre> AcreTileData = new Dictionary<int, Acre> { };
            int i = 0;
            foreach (ushort acre in acreBuffer)
            {
                //MessageBox.Show(acre.ToString("X"));
                i++;
                AcreTileData.Add(i, new Acre(acre, i));
            }
            return AcreTileData;
        }
    }

    public class Acre
    {
        public ushort AcreID = 0;
        public int Index = 0;
        public string Name = "";

        public Acre(ushort acreId, int position)
        {
            AcreID = acreId;
            Index = position;
            Name = AcreData.GetAcreName(acreId);
        }

        public Acre(byte acreId, int position)
        {
            AcreID = acreId;
            Index = position;
            Name = AcreData.GetAcreName(AcreID);
        }
    }

    public class Normal_Acre : Acre
    {
        public WorldItem[] Acre_Items = new WorldItem[16 * 16];

        public Normal_Acre(ushort acreId, int position, ushort[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, uint[] nl_items = null) : base(acreId, position)
        {
            if (items != null)
                for (int i = 0; i < 256; i++)
                {
                    Acre_Items[i] = new WorldItem(items[i], i);
                    if (burriedItemData != null)
                        SetBuried(Acre_Items[i], position, burriedItemData, saveType); //Broken in original save editor lol.. needs a position - 1 to function properly
                }
            else if (nl_items != null)
            {
                for (int i = 0; i < 256; i++)
                {
                    Acre_Items[i] = new WorldItem(nl_items[i], i);
                    //add buried logic
                }
            }
        }

        public Normal_Acre(ushort acreId, int position) : base(acreId, position) { }

        public Normal_Acre(ushort acreId, int position, uint[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing)
            : this(acreId, position, null, null, saveType, items) { }

        public Normal_Acre(ushort acreId, int position, WorldItem[] items, byte[] buriedItemData = null, SaveType saveType = SaveType.Animal_Crossing) : base(acreId, position)
        {
            Acre_Items = items;
            if (buriedItemData != null)
                for (int i = 0; i < 256; i++)
                    SetBuried(Acre_Items[i], position, buriedItemData, saveType);
        }
        //TODO: Change BuriedData from byte[] to ushort[] and use updated code
        private int GetBuriedDataLocation(WorldItem item, int acre, SaveType saveType)
        {
            int worldPosition = 0;
            if (saveType == SaveType.Animal_Crossing || saveType == SaveType.City_Folk)
                worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16; //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
            else if (saveType == SaveType.Wild_World)
                worldPosition = (acre * 256) + item.Index;
            return worldPosition / 8;
        }

        //Fix unburying objects. Seems to be broken right now (Nvm it's working??)
        public void SetBuriedInMemory(WorldItem item, int acre, byte[] burriedItemData, bool buried, SaveType saveType)
        {
            if (saveType != SaveType.New_Leaf && saveType != SaveType.Welcome_Amiibo)
            {
                int buriedLocation = GetBuriedDataLocation(item, acre, saveType);
                if (buriedLocation > -1)
                {
                    DataConverter.SetBit(ref burriedItemData[buriedLocation], item.Location.X % 8, buried);
                    item.Burried = DataConverter.ToBit(burriedItemData[buriedLocation], item.Location.X % 8) == 1;
                }
                else
                    item.Burried = false;
            }
        }
        //Correct decoding/setting of buried items. Fixes the hacky SaveType case for AC/CF. (Don't forget to implement this!)
        private void SetBuriedInMemoryFixed(WorldItem item, int acre, ushort[] buriedItemData, bool buried, SaveType saveType)
        {
            if (saveType != SaveType.New_Leaf && saveType != SaveType.Welcome_Amiibo)
            {
                int buriedLocation = (acre * 256 + item.Index) / 16;
                if (buriedLocation > -1)
                {
                    byte[] Buried_Row_Bytes = BitConverter.GetBytes(buriedItemData[buriedLocation]);
                    DataConverter.SetBit(ref Buried_Row_Bytes[item.Location.X / 8], item.Location.X % 8, buried); //Should probably rewrite bit editing functions to take any data type
                    item.Burried = DataConverter.ToBit(Buried_Row_Bytes[item.Location.X / 8], item.Location.X % 8) == 1;
                    buriedItemData[buriedLocation] = BitConverter.ToUInt16(Buried_Row_Bytes, 0);
                }
                else
                    item.Burried = false;
            }
        }

        private void SetBuried(WorldItem item, int acre, byte[] burriedItemData, SaveType saveType)
        {
            int burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < burriedItemData.Length)
                item.Burried = DataConverter.ToBit(burriedItemData[burriedDataOffset], item.Location.X % 8) == 1;
        }
    }
}
