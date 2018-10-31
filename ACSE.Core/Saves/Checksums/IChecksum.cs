using System.Collections.Generic;

namespace ACSE.Core.Saves.Checksums
{
    /// <summary>
    /// Interface for all checksum calculating objects.
    /// </summary>
    /// <typeparam name="T">The return type of the checksum.</typeparam>
    public interface IChecksum<T>
    {
        /// <summary>
        /// A generic checksum calculation method.
        /// </summary>
        /// <param name="buffer">The IList of bytes whose checksum will be calculated.</param>
        /// <param name="variableArgument">An additional argument to be used by the checksum calculation method.</param>
        /// <returns>Returns T checksum.</returns>
        T Calculate(in IList<byte> buffer, uint variableArgument);

        /// <summary>
        /// Verifies whether or not the checksum for the given IList of bytes is correct.
        /// </summary>
        /// <param name="buffer">The IList of bytes whose checksum will be verified.</param>
        /// <param name="checksum">The current checksum to verify against.</param>
        /// <param name="variableArgument">An additional argument to be used by the checksum calculation method.</param>
        /// <returns>Returns bool isChecksumCorrect</returns>
        bool Verify(in IList<byte> buffer, T checksum, uint variableArgument);
    }
}
