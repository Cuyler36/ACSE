using System;
using System.Collections.Generic;

namespace ACSE.Core.Saves.Checksums
{
    /// <summary>
    /// 16-bit little-endian checksum implementation used by Animal Crossing: Wild World.
    /// </summary>
    public sealed class UInt16LEChecksum : IChecksum<ushort>
    {
        /// <inheritdoc />
        /// <summary>
        /// Calculates a 16-bit additive checksum for a given array.
        /// </summary>
        /// <param name="buffer">The array to checksum</param>
        /// <param name="checksumOffset">The offset of the addendum in the array</param>
        /// <returns>The calculated 16-bit checksum addendum. When added with the sum of all the other values, the result will be 0.</returns>
        public ushort Calculate(in IList<byte> buffer, uint checksumOffset)
        {
            if ((checksumOffset & 1) == 1)
                throw new ArgumentException($"{nameof(checksumOffset)} must be 16-bit aligned!");

            ushort checksum = 0;
            for (var i = 0; i < buffer.Count - 1; i += 2)
            {
                if (i == checksumOffset) continue;
                checksum += (ushort) ((buffer[i + 1] << 8) | buffer[i + 0]);
            }

            return (ushort) -checksum;
        }

        /// <inheritdoc />
        /// <summary>
        /// Checks whether or not the supplied buffer has a correct checksum.
        /// </summary>
        /// <param name="buffer">The array to check</param>
        /// <param name="currentChecksum">The current checksum</param>
        /// <param name="checksumOffset">The offset of the checksum in the IList of bytes.</param>
        /// <returns>Returns bool isChecksumCorrect</returns>
        public bool Verify(in IList<byte> buffer, ushort currentChecksum, uint checksumOffset) =>
            Calculate(buffer, checksumOffset) == currentChecksum;
    }
}
