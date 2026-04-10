using TracerLib;

///
/// This file is release under ... license. See LICENSE.md
/// 

//using TracerLib;
public static class RayTracer
{
    public static string InputFileName = "";
    public static string OutputFileName = "";
    public static float AFactor = 1.0f;
    public static float Gamma = 1.0f;

    public static int Main(string[] args)
    {
        /*
            non ho capito perché mettere un try except. Tanto se non funziona verrà lanciata l'eccezione comunque
            e il programma andrà in crash lo stesso, no?
         */
        try
        {
            ParseArgs(args);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }

        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        
        /* suggerimento della lezione 4 in python
         * with open(parameters.input_pfm_file_name, "rb") as inpf:
           img = hdrimages.read_pfm_image(inpf)

           print(f'File "{parameters.input_pfm_file_name}" has been read from disk.')

           img.normalize_image(factor=parameters.factor)
           img.clamp_image()

           # Same as above: use try…except to produce a human-readable message
           # if something goes wrong
           with open(parameters.output_png_file_name, "wb") as outf:
               img.write_ldr_image(stream=outf, format="PNG", gamma=parameters.gamma)

           print(f'File "{parameters.output_png_file_name}" has been written to disk.')
         */

        return 0;
    }

    private static void ParseArgs(string[] args)
    {
        if (args.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(args),
                "Usage: main inputFileName.pfm aFactor gamma outputFileName.png");
        }

        InputFileName = args[0];

        if (!Single.TryParse(args[1], out AFactor))
        {
            throw new ArgumentException($"Invalid aFactor ('{args[1]}'). It must be a floating number.",
                nameof(AFactor));
        }

        if (!Single.TryParse(args[2], out Gamma))
        {
            throw new ArgumentException($"Invalid gamma ('{args[2]}'). It must be a floating number.", nameof(Gamma));
        }

        OutputFileName = args[3];

        Console.WriteLine(
            $"I parametri passati sono: InputFileName={args[0]}, aFactor={args[1]}, gamma={args[2]}, OutputFileName={args[3]}");
    }
}