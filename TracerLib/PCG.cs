namespace TracerLib;

public class PCG
{
    public ulong State { get; set; }
    public ulong Inc { get; set; }

    public PCG(ulong initState = 42, ulong initSeq = 54)
    {
        State = 0;
        Inc = (initSeq << 1) | 1;
        this.random();
        State += initState;
        this.random();
    }

    public uint random()
    {
        var oldstate = State;

        State = oldstate * 6364136223846793005 + Inc;

        var xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);

        var rot = oldstate >> 59; //Convertire a variabile 32 bit

        //return (uint)((xorshifted >> rot) | (xorshifted << (-rot & 31)));

        uint gizmo = 0;

        return gizmo;
    }

    public float randomFloat()
    {
        return this.random() / 0x100000000;
    }
}