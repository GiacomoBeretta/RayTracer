// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel;
using SixLabors.ImageSharp.Processing;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations; // per rendere le opzioni o gli argomenti required


[Command(Name = "RayTracer")]
[Subcommand(typeof(DemoCommand), typeof(PfmToPngCommand))]
public class RayTracer
{
    public static int Main(string[] args)
        => CommandLineApplication.Execute<RayTracer>(args);
    
    internal int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 0;
    }
}

/// <summary>
/// Demo command for raytracing scene generation.
/// </summary>
[Command(Name = "Demo", Description = "Generates a simple test image composed of 10 spheres:\n" +
                                      "eight spheres are placed at the vertices of a cube, and two are positioned at the centers of two faces.\n" +
                                      "Allows configuration of image resolution and camera orientation via azimuth and zenith angles.\n" +
                                      "Supports two projection modes: perspective (default) and orthogonal.")]
public class DemoCommand
{
    /*[Argument(0, Description = "Image's width")]
    public int Width { get; set; } = 500;

    [Argument(1, Description = "Image's height")]
    public int Height { get; set; } = 500;*/

    [Option("--width", Description = "The width of the image")]
    [Range(1, Int32.MaxValue)]
    public int Width { get; init; } = 500;

    [Option("--height", Description = "The height of the image")]
    [Range(1, Int32.MaxValue)]
    public int Height { get; init; } = 500;

    [Option("--output", Description = "the name of the png file")]
    public string OutputFileName { get; init; } = "referenceShirley.png";
    
    [Option("--theta", Description = "Observer's azimuthal angle in degrees")]
    [Range(0, 360)]
    public float Theta { get; init; } = 0;

    [Option("--phi", Description = "Observer's zenithal angle in degrees")]
    [Range(0, 360)]
    public float Phi { get; init; } = 0;

    [Option("--orthogonal", Description = "Orthogonal camera. Perspective camera passed by default")]
    public bool Orthogonal { get; }

    [Option("--luminosityFunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction LuminosityFunction { get; init; } = LumFunction.Shirley;
    
    //aggiungere range
    [Option("--factor", Description = "The empirical factor to render images")]
    public int Factor { get; init; } = 1;

    //aggiungere range
    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; init; } = 1;

    private void OnExecute()
    {
        Console.WriteLine($"width: {Width}");
        Console.WriteLine($"height: {Height}");
        Console.WriteLine($"outputFileName: {OutputFileName} (it is in 'DemoImages/')");
        Console.WriteLine($"theta: {Theta}");
        Console.WriteLine($"phi: {Phi}");
        Console.WriteLine($"projection: {(Orthogonal? "orthogonal" : "perspective")}");
        Console.WriteLine();
        Console.WriteLine("Tone Mapping parameters:");
        Console.WriteLine($"luminosity function: {LuminosityFunction}");
        Console.WriteLine($"factor: {Factor}");
        Console.WriteLine($"gamma: {Gamma}");

        float thetaRad = Functions.DegToRad(Theta);
        float phiRad = Functions.DegToRad(Phi);
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string pngFilePath = Path.Combine(currentPath, "../../../../DemoImages/" + OutputFileName);

        var image = new HDRImage(Width, Height);

        // PER LA CAMERA APPLICARE LA TRASLAZIONE PER PRIMA (ULTIMA NELLA CONCATENAZIONE)
        ICamera camera;

        if (Orthogonal)
        {
            camera = new OrthogonalCamera(transformation: new Transformation('y', phiRad) *
                                                          new Transformation('z', thetaRad) *
                                                          new Transformation(new Vector(-1.0f, 0f, 0f)));
        }
        else
        {
            camera = new PerspectiveCamera(transformation: new Transformation('y', phiRad) *
                                                           new Transformation('z', thetaRad) *
                                                           new Transformation(new Vector(-1.0f, 0f, 0f)));
        }

        var tracer = new ImageTracer(image, camera);

        //PER LE FORME APPLICARE LA TRASLAZIONE PER ULTIMA (PRIMA NELLA CONCATENAZIONE)
        var s1 = new Sphere(new Transformation(new Vector(0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s2 = new Sphere(new Transformation(new Vector(-0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s3 = new Sphere(new Transformation(new Vector(0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s4 = new Sphere(new Transformation(new Vector(0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s5 = new Sphere(new Transformation(new Vector(-0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s6 = new Sphere(new Transformation(new Vector(0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s7 = new Sphere(new Transformation(new Vector(-0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s8 = new Sphere(new Transformation(new Vector(-0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s9 = new Sphere(new Transformation(new Vector(0f, 0f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f));
        var s10 = new Sphere(new Transformation(new Vector(0f, 0.5f, 0f)) * new Transformation(0.1f, 0.1f, 0.1f));

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

        tracer.FireAllRays(ray =>
            world.RayIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f));
        image.WritePNG(pngFilePath, LuminosityFunction, Factor, Gamma);
    }
}

[Command(Name = "pfmtopng", Description = "Converts a PFM image to PNG")]
public class PfmToPngCommand
{
    [Option("--input", Description = "The input file path")]
    [Required]
    public required string InputFilePath { get; init; }

    [Option("--output", Description = "The output file path")]
    [Required]
    public required string OutputFilePath { get; init; }

    [Option("--luminosityFunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction LuminosityFunction { get; init; } = LumFunction.Shirley;

    [Option("--factor", Description = "The empirical factor to render images")]
    public int Factor { get; init; } = 1;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; init; } = 1;

    internal void OnExecute()
    {
        Console.WriteLine($"input path: {InputFilePath}");
        Console.WriteLine($"output path: {OutputFilePath}");
        Console.WriteLine($"luminosity function: {LuminosityFunction}");
        Console.WriteLine($"factor: {Factor}");
        Console.WriteLine($"gamma: {Gamma}");
        
        HDRImage image = HDRImage.ReadPFM_File(InputFilePath);
        image.WritePNG(OutputFilePath, LuminosityFunction, Factor, Gamma);
    }
}

/*try
{
    ParseArgs(args);
}
catch (ArgumentException e)
{
    Console.WriteLine("Error: " + e.Message);
    return 1;
}

string currentPath = AppDomain.CurrentDomain.BaseDirectory;
string pfmFilePath = Path.Combine(currentPath, "../../../../TracerTests/reference_be.pfm");
string pngFilePathShirley = Path.Combine(currentPath, "../../../../TracerTests/referenceShirley.png");
string pngFilePathWeighted = Path.Combine(currentPath, "../../../../TracerTests/referenceWeighted.png");
HDRImage hdrImage = HDRImage.ReadPFM_File(pfmFilePath);
hdrImage.Print();
hdrImage.WritePNG(pngFilePathShirley, 0, AFactor, Gamma);
hdrImage.WritePNG(pngFilePathWeighted, 1, AFactor, Gamma);


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


return 0; */


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