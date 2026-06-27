// This file is release under EUPL_v1.2 license. See LICENSE.md

namespace TracerLib;

/// <summary>
/// A random generator described by Melissa O'Neill in this page:
/// https://www.pcg-random.org/paper.html
/// </summary>
public class PCG
{
    public ulong State { get; set; }
    public ulong Inc { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initState"></param>
    /// <param name="initSeq"></param>
    public PCG(ulong initState = 42UL, ulong initSeq = 54UL)
    {
        State = 0;
        Inc = (initSeq << 1) | 1;
        Random();
        State += initState;
        Random();
    }

    /// <summary>
    /// Returns a random unsigned integer (32 bit) number between 0 and <code>uint.max</code>.
    /// </summary>
    /// <returns></returns>
    public uint Random()
    {
        ulong oldstate = State;

        State = oldstate * 6364136223846793005 + Inc;

        uint xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);

        int rot = (int)(oldstate >> 59);

        return (xorshifted >> rot) | (xorshifted << ((~rot + 1) & 31)); // Rivedere con 32 - rot al posto di ~rot +1
    }

    /// <summary>
    /// Returns a random floating number between 0 and 1 following a uniform distribution.
    /// </summary>
    /// <returns></returns>
    public float RandomFloat()
    {
        return Random() / (float)0x100000000;
    }
}