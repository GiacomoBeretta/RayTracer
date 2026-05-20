namespace TracerLib;

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
        this.Random();
        State += initState;
        this.Random();
    }

    /// <summary>
    /// Returns a random unsigned integer (32 bit) number between 0 and uint.max
    /// </summary>
    /// <returns></returns>
    public uint Random()
    {
        var oldstate = State;

        State = oldstate * 6364136223846793005 + Inc;

        var xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);

        var rot = (int)(oldstate >> 59); 

        return (xorshifted >> rot) | (xorshifted << ((~rot + 1) & 31)); // Rivedere con 32 - rot al posto di ~rot +1
    }

    /// <summary>
    /// Returns a random floating number between 0 and 1
    /// </summary>
    /// <returns></returns>
    public float RandomFloat()
    {
        return this.Random() / (float)0x100000000;
    }
}