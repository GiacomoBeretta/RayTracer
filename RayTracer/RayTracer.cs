// This file is release under EUPL_v1.2 license. See LICENSE.md

using SixLabors.ImageSharp.Processing;
using TracerLib;

//using TracerLib;
public static class RayTracer
{
    /*public static string InputFileName = "";
    public static string OutputFileName = "";
    public static float AFactor = 1.0f;
    public static float Gamma = 1.0f;*/

    public static int Main(/*string[] args*/)
    {
        /*try
        {
            ParseArgs(args);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Error: " + e.Message);
            return 1;
        }*/

        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string pfmFilePath = Path.Combine(currentPath, "../../../../TracerTests/reference_be.pfm");
        string pngFilePathShirley = Path.Combine(currentPath, "../../../../TracerTests/referenceShirley.png");
        string pngFilePathWeighted = Path.Combine(currentPath, "../../../../TracerTests/referenceWeighted.png");
        /*HDRImage hdrImage = HDRImage.ReadPFM_File(pfmFilePath);
        hdrImage.Print();
        hdrImage.WritePNG(pngFilePathShirley, 0, AFactor, Gamma);
        hdrImage.WritePNG(pngFilePathWeighted, 1, AFactor, Gamma);*/

        var image = new HDRImage(500, 500);

        // PER LA CAMERA APPLICARE LA TRASLAZIONE PER PRIMA (ULTIMA NELLA CONCATENAZIONE)
        var camera = new OrthogonalCamera(transformation:  new Transformation('y', MathF.PI/20) * new Transformation('z', MathF.PI/20) * new Transformation(new Vector(-1.0f, 0f, 0f)));

        var tracer = new ImageTracer(image, camera);
        
        //PER LE SFERE APPLICARE LA TRASLAZIONE PER ULTIMA (PRIMA NELLA CONCATENAZIONE)
        var s1 = new Sphere( new Transformation(new Vector(0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s2 = new Sphere( new Transformation(new Vector(-0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s3 = new Sphere( new Transformation(new Vector(0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s4 = new Sphere( new Transformation(new Vector(0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s5 = new Sphere( new Transformation(new Vector(-0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s6 = new Sphere( new Transformation(new Vector(0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s7 = new Sphere( new Transformation(new Vector(-0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s8 = new Sphere( new Transformation(new Vector(-0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s9 = new Sphere( new Transformation(new Vector(0f, 0f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s10 = new Sphere( new Transformation(new Vector(0f, 0.5f, 0f)) * new Transformation(0.1f, 0.1f, 0.1f));
        
        //var p = new Plane(new Transformation(20f, 20f, 20f));
        
        var shapes = new List<Shape>
        {
            s1,
            s2,
            s3,
            s4,
            s5,
            s6,
            s7,
            s8,
            s9,
            s10,
        };

        var world = new World(shapes);
        
        tracer.FireAllRays(ray => world.RayIntersection(ray) != null ? new Color(1.0f, 0.0f, 0.0f) : new Color(0.0f, 0.0f, 0.0f));
        
        image.WritePNG(pngFilePathShirley, 0,1.0f , 1.0f);
        

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

    /*private static void ParseArgs(string[] args)
    {
        if (args.Length != 4)
        {
            throw new ArgumentOutOfRangeException(nameof(args),
                "Usage: dotnet run inputFileName.pfm aFactor gamma outputFileName.png");
        }

        //InputFileName = args[0];

        if (!Single.TryParse(args[0], out AFactor))
        {
            throw new ArgumentException($"Invalid aFactor ('{args[1]}'). It must be a floating number.",
                nameof(AFactor));
        }

        if (!Single.TryParse(args[1], out Gamma))
        {
            throw new ArgumentException($"Invalid gamma ('{args[2]}'). It must be a floating number.", nameof(Gamma));
        }

        OutputFileName = args[2];

        //Console.WriteLine(
           // $"I parametri passati sono: InputFileName={args[0]}, aFactor={args[1]}, gamma={args[2]}, OutputFileName={args[3]}");
    }*/
}