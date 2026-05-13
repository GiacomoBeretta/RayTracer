namespace TracerLib;

public class PCG
{
    public ulong State { get; set; }
    public ulong Inc { get; set; }

    public PCG(ulong initState = 42, ulong initSeq = 54)
    {
        State = 0;
        Inc = (initSeq << 1) | 1;
        this.Random();
        State += initState;
        this.Random();
    }

    public uint Random()
    {
        var oldstate = State;

        State = oldstate * 6364136223846793005 + Inc;

        var xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);

        var rot = (int)(oldstate >> 59); 

        return (xorshifted >> rot) | (xorshifted << ((~rot + 1) & 31)); // Rivedere con 32 - rot al posto di ~rot +1
    }

    public float RandomFloat()
    {
        return this.Random() / 0x100000000;
    }
}