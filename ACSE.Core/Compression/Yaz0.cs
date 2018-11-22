using System;
using System.Collections.Generic;
using System.Text;
using ACSE.Core.Utilities;

namespace ACSE.Core.Compression
{
    public static class Yaz0
    {
        /// <summary>
        /// Verifies that the supplied byte array is Yaz0 compressed.
        /// </summary>
        /// <param name="data">The Yaz0 compressed data array</param>
        /// <returns>IsDataYaz0Compressed</returns>
        public static bool IsYaz0(byte[] data)
            => data.Length > 0x10 && Encoding.ASCII.GetString(data, 0, 4).Equals("Yaz0");

        /// <summary>
        /// Decompresses Yaz0 compressed data.
        /// </summary>
        /// <param name="data">The Yaz0 compressed data array</param>
        /// <returns>The decompressed data</returns>
        public static byte[] Decompress(byte[] data)
        {
            if (!IsYaz0(data))
            {
                throw new ArgumentException("The supplied data does not appear to be Yaz0 compressed!");
            }

            var size = (uint)(data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7]);
            var output = new byte[size];
            var readOffset = 16;
            var outputOffset = 0;

            while (true)
            {
                var bitmap = data[readOffset++];
                for (var i = 0; i < 8; i++)
                {
                    if ((bitmap & 0x80) != 0)
                    {
                        output[outputOffset++] = data[readOffset++];
                    }
                    else
                    {
                        var b = data[readOffset++];
                        var offsetAdjustment = ((b & 0xF) << 8 | data[readOffset++]) + 1;
                        var length = (b >> 4) + 2;
                        if (length == 2)
                        {
                            length = data[readOffset++] + 0x12;
                        }

                        for (var j = 0; j < length; j++)
                        {
                            output[outputOffset] = output[outputOffset - offsetAdjustment];
                            outputOffset++;
                        }
                    }

                    bitmap <<= 1;

                    if (outputOffset >= size)
                    {
                        return output;
                    }
                }
            }
        }

        public static byte[] Compress(byte[] file)
        {
            var layoutBits = new List<byte>();
            var dictionary = new List<byte>();

            var uncompressedData = new List<byte>();
            var compressedData = new List<int[]>();

            const int maxDictionarySize = 4096;
            const int minMatchLength = 3;
            const int maxMatchLength = 255 + 0x12;
            var decompressedSize = 0;

            for (var i = 0; i < file.Length; i++)
            {

                if (dictionary.Contains(file[i]))
                {
                    //compressed data
                    var matches = Utility.FindAllMatches(ref dictionary, file[i]);
                    var bestMatch = Utility.FindLargestMatch(ref dictionary, matches, ref file, i, maxMatchLength);

                    if (bestMatch[1] >= minMatchLength)
                    {
                        layoutBits.Add(0);
                        bestMatch[0] = dictionary.Count - bestMatch[0];

                        for (var j = 0; j < bestMatch[1]; j++)
                        {
                            dictionary.Add(file[i + j]);
                        }

                        i = i + bestMatch[1] - 1;

                        compressedData.Add(bestMatch);
                        decompressedSize += bestMatch[1];
                    }
                    else
                    {
                        //uncompressed data
                        layoutBits.Add(1);
                        uncompressedData.Add(file[i]);
                        dictionary.Add(file[i]);
                        decompressedSize++;
                    }
                }
                else
                {
                    //uncompressed data
                    layoutBits.Add(1);
                    uncompressedData.Add(file[i]);
                    dictionary.Add(file[i]);
                    decompressedSize++;
                }

                if (dictionary.Count <= maxDictionarySize) continue;
                var overflow = dictionary.Count - maxDictionarySize;
                dictionary.RemoveRange(0, overflow);
            }

            return BuildYaz0CompressedBlock(ref layoutBits, ref uncompressedData, ref compressedData, decompressedSize);
        }

        public static byte[] BuildYaz0CompressedBlock(ref List<byte> layoutBits, ref List<byte> uncompressedData, ref List<int[]> offsetLengthPairs, int decompressedSize)
        {
            var finalYaz0Block = new List<byte>();
            var layoutBytes = new List<byte>();
            var compressedDataBytes = new List<byte>();
            var extendedLengthBytes = new List<byte>();

            //add Yaz0 magic number & decompressed file size
            finalYaz0Block.AddRange(Encoding.ASCII.GetBytes("Yaz0"));
            finalYaz0Block.AddRange(BitConverter.GetBytes(decompressedSize.Reverse()));

            //add 8 zeros per format specification
            for (var i = 0; i < 8; i++)
            {
                finalYaz0Block.Add(0);
            }

            //assemble layout bytes
            while (layoutBits.Count > 0)
            {
                byte B = 0;
                for (var i = 0; i < (8 > layoutBits.Count ? layoutBits.Count : 8); i++)
                {
                    B |= (byte)(layoutBits[i] << (7 - i));
                }

                layoutBytes.Add(B);
                layoutBits.RemoveRange(0, (layoutBits.Count < 8) ? layoutBits.Count : 8);
            }

            //assemble offsetLength shorts
            foreach (var offsetLengthPair in offsetLengthPairs)
            {
                //if < 18, set 4 bits -2 as matchLength
                //if >= 18, set matchLength == 0, write length to new byte - 0x12

                var adjustedOffset = offsetLengthPair[0];
                var adjustedLength = (offsetLengthPair[1] >= 18) ? 0 : offsetLengthPair[1] - 2; //vital, 4 bit range is 0-15. Number must be at least 3 (if 2, when -2 is done, it will think it is 3 byte format), -2 is how it can store up to 17 without an extra byte because +2 will be added on decompression

                if (adjustedLength == 0)
                {
                    extendedLengthBytes.Add((byte)(offsetLengthPair[1] - 18));
                }

                var compressedInt = ((adjustedLength << 12) | adjustedOffset - 1);

                var compressed2Byte = new byte[2];
                compressed2Byte[0] = (byte)(compressedInt & 0xFF);
                compressed2Byte[1] = (byte)((compressedInt >> 8) & 0xFF);

                compressedDataBytes.Add(compressed2Byte[1]);
                compressedDataBytes.Add(compressed2Byte[0]);
            }

            //add rest of file
            foreach (var layoutByte in layoutBytes)
            {
                finalYaz0Block.Add(layoutByte);
                var currentLayoutByte = layoutByte;

                for (var j = 7; ((j > -1) && ((uncompressedData.Count > 0) || (compressedDataBytes.Count > 0))); j--)
                {
                    if (((currentLayoutByte >> j) & 1) == 1)
                    {
                        finalYaz0Block.Add(uncompressedData[0]);
                        uncompressedData.RemoveAt(0);
                    }
                    else
                    {
                        if (compressedDataBytes.Count <= 0) continue;
                        var length = compressedDataBytes[0] >> 4;

                        finalYaz0Block.Add(compressedDataBytes[0]);
                        finalYaz0Block.Add(compressedDataBytes[1]);
                        compressedDataBytes.RemoveRange(0, 2);

                        if (length != 0) continue;
                        finalYaz0Block.Add(extendedLengthBytes[0]);
                        extendedLengthBytes.RemoveAt(0);
                    }
                }
            }

            return finalYaz0Block.ToArray();
        }
    }
}
