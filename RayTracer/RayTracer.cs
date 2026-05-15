// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel;
using SixLabors.ImageSharp.Processing;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;

[Command(Name = "RayTracer")]
[Subcommand(typeof(DemoCommand))]
public class RayTracer
{
  public static int Main(string[] args)
        => CommandLineApplication.Execute<RayTracer>(args);

    private void OnExecute(CommandLineApplication app)
        => app.ShowHelp();
}

/// <summary>
/// Demo command for raytracing scene generation.
/// </summary>
[Command(Name = "Demo", Description = "Generates a simple test image composed of 10 spheres:\n" +
                                      "eight spheres are placed at the vertices of a cube, and two are positioned at the centers of two faces.\n" +
                                      "Allows configuration of image resolution and camera orientation via azimuth and zenith angles.\n" +
                                      "Supports two projection modes: perspective (default) and orthogonal.")]
class DemoCommand
{
    [Argument(0, Description = "Image's width")] public int? Width { get; }
    [Argument(1, Description = "Image's height")] public int? Height { get; }
    
    [Option("--output", Description = "the name of the png file (with the extension)")]
    public string? OutputFileName { get; }
    
    [Option("--theta", Description = "Observer's azimuthal angle in degrees")]
    public float? Theta { get; }
    
    [Option("--phi", Description = "Observer's zenithal angle in degrees")]
    public float? Phi { get; }
    
    [Option("--orthogonal", Description = "Orthogonal camera. Perspective camera passed by default")]
    public bool Orthogonal { get; }

    [Option("--factor", Description = "The empirical factor to render images")]
    public int? Factor { get; }
    
    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float? Gamma { get; }
    
    private void OnExecute()
    {
        var width = Width ?? 500;
        var height = Height ?? 500;
        var outputFileName = OutputFileName ?? "referenceShirley.png";
        var theta = Theta ?? 0;
        var phi = Phi ?? 0;
        var factor = Factor ?? 1.0f;
        var gamma = Gamma ?? 1.0f;
        
        Console.WriteLine($"width: {width}");
        Console.WriteLine($"height: {height}");
        Console.WriteLine($"outputFileName: {outputFileName}");
        Console.WriteLine($"theta: {theta}");
        Console.WriteLine($"phi: {phi}");

        Console.WriteLine("Tone Mapping parameters:");
        Console.WriteLine($"factor: {factor}");
        Console.WriteLine($"gamma: {gamma}");
        
        theta = Functions.DegToRad(theta);
        phi = Functions.DegToRad(phi);
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string pngFilePath = Path.Combine(currentPath, "../../../../Images/" + outputFileName);

        var image = new HDRImage(width, height);

        // PER LA CAMERA APPLICARE LA TRASLAZIONE PER PRIMA (ULTIMA NELLA CONCATENAZIONE)
        ICamera camera;

        if (Orthogonal)
        {
             camera = new OrthogonalCamera(transformation:  new Transformation('y', phi) * new Transformation('z', theta) * new Transformation(new Vector(-1.0f, 0f, 0f)));
        }
        else
        {
             camera = new PerspectiveCamera(transformation:  new Transformation('y', phi) * new Transformation('z', theta) * new Transformation(new Vector(-1.0f, 0f, 0f)));
        }
        
        var tracer = new ImageTracer(image, camera);
        
        //PER LE FORME APPLICARE LA TRASLAZIONE PER ULTIMA (PRIMA NELLA CONCATENAZIONE)
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

        tracer.FireAllRays(ray => world.RayIntersection(ray) != null ? new Color(1.0f, 1.0f, 1.0f) : new Color(0.0f, 0.0f, 0.0f));
        image.WritePNG(pngFilePath, 0, factor, gamma);
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
