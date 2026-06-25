// This file is released under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using SixLabors.ImageSharp.Processing;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;

[Command(Name = "RayTracer")]
[Subcommand( typeof(RenderCommand), typeof(AverageImageCommand), typeof(PfmToPngCommand))]
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

[Command(Name = "render", Description = "Read a scene file and creates the corresponding image")]
public class RenderCommand
{
    #region Options

    [Option("--inputrender", Description = "The input scene file name")]
    public string InputSceneName { get; set; } = "scene.txt";

    [Option("--outputpfm", Description = "Name of the pfm file output")]
    public string OutputPfmName { get; set; } = "output.pfm";

    [Option("--outputpng", Description = "Name of the png file output")]
    public string OutputPngName { get; set; } = "output.png";

    [Option("--width", Description = "The width of the image")]
    [Range(1, Int32.MaxValue)]
    public int Width { get; set; } = 500;

    [Option("--height", Description = "The height of the image")]
    [Range(1, Int32.MaxValue)]
    public int Height { get; set; } = 500;

    [Option("--sampleside", Description = "Number of samples per pixel's side (used for antialiasing)")]
    [Range(1, Int32.MaxValue)]
    public int SampleSide { get; set; } = 1;

    [Option("--algorithm", Description = "Render's algorithm; pathTracer passed by default")]
    public RenderFunc Algorithm { get; set; } = RenderFunc.PathTracer;

    [Option("--numrays",
        Description =
            "Number of rays departing from each surface (this command only works for the Pathtracing algorithm)")]
    public int NumRays { get; set; } = 10;

    [Option("--maxdepth",
        Description = "Maximum allowed ray depth (this command only works for the Pathtracing algorithm)")]
    public int MaxDepth { get; set; } = 2;

    [Option("--initstate", Description = "Initial seed for the random number generator")]
    [Range(0, ulong.MaxValue)]
    public ulong InitState { get; set; } = 45;

    [Option("--initseq", Description = "Identifier of the sequence produced by the random number generator")]
    [Range(0, ulong.MaxValue)]
    public ulong InitSeq { get; set; } = 54;

    [Option("--roulettestart",
        Description = "Number of ray reflections after which the Russian roulette algorithm is applied")]
    [Range(0, Int32.MaxValue)]
    public int RussianRouletteStartDepth { get; set; } = 3;

    [Option("--rouletteprob", Description = "Optional fixed probability for the Russian roulette algorithm " +
                                            "(when null, the probability is computed dynamically at each recursive call of RenderFunction)")]
    [Range(0, 1)]
    public float? RussianRouletteFixedProb { get; set; } = null;

    [Option("--luminosityfunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction Luminosityfunction { get; set; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. If the value is null is computed with the luminosity function")]
    public float? AverageLuminosity { get; set; } = null;

    [Option("--factor", Description = "The empirical factor to render images")]
    public float Factor { get; set; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; set; } = 1f;

    [Option("--declarefloat|-d", Description = "Declare a variable. The syntax is --declarefloat=NAME:VALUE")]
    public string[] Definitions { get; set; } = [];

    #endregion

    public void OnExecute()
    {
        PrintParameters();
        SetIOFilesPaths(out string scenePath, out string pfmFilePath, out string pngFilePath);
        Scene scene = ReadSceneFile(scenePath);
        HDRImage image = new HDRImage(Width, Height);
        Renderer renderer = BuildRenderer(scene);
        ICamera camera = GetOrCreateCamera(scene);
        ImageTracer tracer = new ImageTracer(image, camera, pixelSideSubdivisions: SampleSide);

        tracer.FireAllRays(ray => renderer.RenderFunction(ray));

        HDRImage.WritePFM_File(image, pfmFilePath);
        Console.WriteLine($"Pfm file created in: {pfmFilePath}");

        image.WritePNG(pngFilePath, Luminosityfunction, Factor, Gamma, AverageLuminosity);
        Console.WriteLine($"Png file created in: {pngFilePath}");
    }

    /// <summary>
    /// Prints all the parameters passed through the command line.
    /// </summary>
    public void PrintParameters()
    {
        Console.WriteLine("File names:");
        Console.WriteLine($"Input: {InputSceneName}");
        Console.WriteLine($"OutputPfm: {OutputPfmName}");
        Console.WriteLine($"OutputPng: {OutputPngName}");
        Console.WriteLine();
        Console.WriteLine($"Width: {Width}");
        Console.WriteLine($"Height: {Height}");
        Console.WriteLine($"SampleSide: {SampleSide}");
        Console.WriteLine($"Algorithm: {Algorithm}");
        Console.WriteLine();
        Console.WriteLine("extra parameters for path tracing:");
        Console.WriteLine($"NumRay: {NumRays}");
        Console.WriteLine($"MaxDepth: {MaxDepth}");
        Console.WriteLine($"InitState: {InitState}");
        Console.WriteLine($"InitSeq: {InitSeq}");
        Console.WriteLine($"RouletteStart: {RussianRouletteStartDepth}");
        Console.WriteLine($"RouletteFixedProb: {RussianRouletteFixedProb}");
        Console.WriteLine();
        Console.WriteLine("Tone Mapping parameters:");
        Console.WriteLine($"Luminosityfunction: {Luminosityfunction}");
        Console.WriteLine($"Averageluminosity: {AverageLuminosity}");
        Console.WriteLine($"Factor: {Factor}");
        Console.WriteLine($"Gamma: {Gamma}");
        Console.WriteLine();
        Console.Write("Definitions: ");
        for (int i = 0; i < Definitions.Length; i++)
        {
            Console.WriteLine($"{Definitions[i]}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Builds the paths of the input scene file and the output PFM and PNG files,
    /// <c>.pfm</c> and <c>.png</c> extensions are appended to the output file
    /// names if they are missing.
    /// </summary>
    /// <param name="scenePath">Full path to the input scene file.</param>
    /// <param name="pfmFilePath">Full path to the output PFM file.</param>
    /// <param name="pngFilePath">Full path to the output PNG file.</param>
    public void SetIOFilesPaths(out string scenePath, out string pfmFilePath, out string pngFilePath)
    {
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        string pfmName = OutputPfmName.EndsWith(".pfm") ? OutputPfmName : OutputPfmName + ".pfm";
        string pngName = OutputPngName.EndsWith(".png") ? OutputPngName : OutputPngName + ".png";

        scenePath = Path.Combine(currentPath, "../../../../Scenes/", InputSceneName);
        pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages/", pfmName);
        pngFilePath = Path.Combine(currentPath, "../../../../PngImages/", pngName);
    }

    /// <summary>
    /// Loads and parses a scene file from the specified path.
    /// Command-line defined variables take precedence over file-defined variables
    /// during parsing.
    /// </summary>
    /// <param name="scenePath">Path to the scene file.</param>
    /// <returns>The parsed <see cref="Scene"/>.</returns>
    public Scene ReadSceneFile(string scenePath)
    {
        Scene scene = new Scene();
        InputStream inputStream = new InputStream(scenePath);
        Dictionary<string, float> CLvariables = Functions.ParseVariableTable(Definitions);

        scene.ReadScene(inputStream, CLvariables);

        return scene;
    }

    /// <summary>
    /// Creates and returns a renderer instance based on the rendering algorithm
    /// specified via command line options.
    /// </summary>
    /// <param name="scene">
    /// The scene used to initialize the renderer.
    /// </param>
    /// <returns>
    /// A <see cref="Renderer"/> instance configured according to the selected algorithm.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the selected rendering algorithm is not supported.
    /// </exception>
    public Renderer BuildRenderer(Scene scene)
    {
        Renderer renderer;
        switch (Algorithm)
        {
            case RenderFunc.OnOff:
                renderer = new OnOffRenderer(scene.World);
                break;
            case RenderFunc.Flat:
                renderer = new FlatRenderer(scene.World);
                break;
            case RenderFunc.PathTracer:
                renderer = new PathTracingRenderer(scene.World, new PCG(InitState, InitSeq), backgroundColor: null,
                    NumRays, MaxDepth, RussianRouletteStartDepth, RussianRouletteFixedProb);
                break;
            default:
                throw new ArgumentException("Invalid algorithm, accepted onoff, flat or pathtracer");
        }

        return renderer;
    }

    /// <summary>
    /// Returns the scene camera, creating a default PerspectiveCamera if none exists.
    /// </summary>
    /// <param name="scene">The scene to retrieve or initialize the camera from.</param>
    /// <returns>An existing camera if present, otherwise a newly created PerspectiveCamera.</returns>
    public ICamera GetOrCreateCamera(Scene scene)
    {
        if (scene.Camera == null)
        {
            Console.WriteLine("Not initialized camera. Follows default initialization [perspective]");
            scene.Camera = new PerspectiveCamera();
        }

        return scene.Camera;
    }
}

[Command(Name = "averageimage",
    Description =
        "Generate an image averaging the color of multiple images using different seed in pathtracing renderer")]
public class AverageImageCommand
{
    [Option("--outputaveragepfm", Description = "Name of the output pfm file")]
    [Required]
    public required string OutputFilePathPfm { get; set; }

    [Option("--outputaveragepng", Description = "Name of the output png file")]
    [Required]
    public required string OutputFilePathPng { get; set; }

    [Option("--luminosityfunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction Luminosityfunction { get; set; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. If the value is null is computed with the luminosity function")]
    public float? AverageLuminosity { get; set; } = null;

    [Option("--factor", Description = "The empirical factor to render images")]
    public float Factor { get; set; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; set; } = 1f;

    public void OnExecute()
    {
        Console.WriteLine($"Name of the output pfm file path: {OutputFilePathPfm}");
        Console.WriteLine($"Name of the output png file path: {OutputFilePathPng}");
        Console.WriteLine($"Luminosityfunction: {Luminosityfunction}");
        Console.WriteLine($"Averageluminosity: {AverageLuminosity}");
        Console.WriteLine($"Factor: {Factor}");
        Console.WriteLine($"Gamma: {Gamma}");

        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string inputFileFolder = Path.Combine(currentPath, "../../../../PfmImages");

        if (OutputFilePathPng[^4..] != ".png")
            OutputFilePathPng += ".png"; //OutputFilename[^4..] Legge gli ultimi 4 caratteri
        string pngFilePath =
            Path.Combine(currentPath, "../../../../PngImages/",
                OutputFilePathPng); //"../../../../DemoImages/" dal path dell'eseguibile torna indietro (Controllare)

        if (OutputFilePathPfm[^4..] != ".pfm") OutputFilePathPfm += ".pfm";
        string pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages", OutputFilePathPfm);

        string[] files = Directory.GetFiles(inputFileFolder, "*_state*_seq*.pfm"); //search for pattern in folder

        if (files.Length == 0)
        {
            Console.WriteLine("The folder is empty");
            return;
        }

        //Using first file as accumulator

        HDRImage acc = HDRImage.ReadPFM_File(files[0]);

        int width = acc.Width;
        int height = acc.Height;
        int length = width * height;

        Color[] average = new Color[length];

        HDRImage[] images = new HDRImage[files.Length];

        for (int i = 0; i < files.Length; i++) images[i] = HDRImage.ReadPFM_File(files[i]);

        foreach (HDRImage image in images)
        {
            if (image.Width != acc.Width || image.Height != acc.Height)
                throw new ArgumentException("Images must have equal width and height");
        }

        for (int i = 0; i < length; i++)
        {
            foreach (HDRImage image in images)
            {
                average[i] += image[i];
            }

            average[i] *= (1.0f / files.Length);
        }

        HDRImage output = new HDRImage(acc.Width, acc.Height, average);

        HDRImage.WritePFM_File(output, pfmFilePath);
        Console.WriteLine($"Pfm file created in: {pfmFilePath}");

        output.WritePNG(OutputFilePathPng, Luminosityfunction, Factor, Gamma, AverageLuminosity);
        Console.WriteLine($"Png file created in: {pngFilePath}");
    }
}

[Command(Name = "pfmtopng", Description = "Converts a PFM image to PNG")]
public class PfmToPngCommand
{
    #region Options

    [Option("--inputpfm", Description = "The input file name")]
    [Required]
    public required string InputFileName { get; set; }

    [Option("--output", Description = "The output file name")]
    [Required]
    public required string OutputFileName { get; set; }

    [Option("--luminosityfunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction Luminosityfunction { get; set; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. If the value is null is computed with the luminosity function")]
    public float? AverageLuminosity { get; set; } = null;

    [Option("--factor", Description = "The empirical factor to render images")]
    public float Factor { get; set; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; set; } = 1f;

    #endregion

    internal void OnExecute()
    {
        Console.WriteLine($"input path: {InputFileName}");
        Console.WriteLine($"output path: {OutputFileName}");
        Console.WriteLine($"luminosityFunction: {Luminosityfunction}");
        Console.WriteLine($"Averageluminosity: {AverageLuminosity}");
        Console.WriteLine($"factor: {Factor}");
        Console.WriteLine($"gamma: {Gamma}");

        if (!OutputFileName.EndsWith(".png")) OutputFileName += ".png";
        if (!InputFileName.EndsWith(".pfm")) InputFileName += ".pfm";

        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages", InputFileName);
        string pngFilePath = Path.Combine(currentPath, "../../../../PngImages/", OutputFileName);

        HDRImage image = HDRImage.ReadPFM_File(pfmFilePath);
        Console.WriteLine($"File read: {pfmFilePath}");

        image.WritePNG(pngFilePath, Luminosityfunction, Factor, Gamma, AverageLuminosity);

        Console.WriteLine($"File saved in: {pngFilePath}");
    }
}

public enum Projection
{
    Perspective,
    Orthogonal
}

public enum RenderFunc
{
    OnOff,
    Flat,
    PathTracer
}

/*
private void OnExecute()
   {
       Console.WriteLine($"width: {Width}");
       Console.WriteLine($"height: {Height}");
       Console.WriteLine($"outputFileName: {OutputFileName}");
       Console.WriteLine($"Render's algorithm: {Algorithm}");
       Console.WriteLine($"theta: {Theta}");
       Console.WriteLine($"phi: {Phi}");
       Console.WriteLine($"projection: {Projection}");
       Console.WriteLine();
       Console.WriteLine("Tone Mapping parameters:");
       Console.WriteLine($"luminosity function: {LuminosityFunction}");
       Console.WriteLine($"factor: {Factor}");
       Console.WriteLine($"gamma: {Gamma}");

       // Adjust parameters
       float thetaRad = Functions.DegToRad(Theta);
       float phiRad = Functions.DegToRad(Phi);
       string currentPath = AppDomain.CurrentDomain.BaseDirectory;
       string pngFilePath = Path.Combine(currentPath, "../../../../DemoImages/" + OutputFileName);
       if (OutputFileName[^4..] != ".png") pngFilePath += ".png";

       // Define materials
       var sphereTexture = new HDRImage(2, 2)
       {
           [0] = new Color(0.1f, 0.2f, 0.3f),
           [1] = new Color(0.2f, 0.1f, 0.3f),
           [2] = new Color(0.3f, 0.2f, 0.1f),
           [3] = new Color(0.3f, 0.1f, 0.2f)
       };
       var material1 = new Material(new UniformPigment(new Color(0.7f, 0.3f, 0.2f)), new DiffuseBRDF());
       var material2 =
           new Material(new CheckeredPigment(new Color(0.2f, 0.7f, 0.3f), new Color(0.3f, 0.2f, 0.7f), numsteps: 4),
               new DiffuseBRDF());
       var material3 = new Material(new ImagePigment(sphereTexture), new DiffuseBRDF());

       // PER LA CAMERA APPLICARE LA TRASLAZIONE PER PRIMA (ULTIMA NELLA CONCATENAZIONE)
       ICamera camera;

       if(Projection == Projection.Perspective)
       {
           camera = new PerspectiveCamera(transformation: new Transformation('z', phiRad) *
                                                          new Transformation('y', thetaRad) *
                                                          new Transformation(new Vector(-1.0f, 0f, 1.0f)));
       }
       else if (Projection == Projection.Orthogonal)
       {
           camera = new OrthogonalCamera(transformation: new Transformation('z', phiRad) *
                                                         new Transformation('y', thetaRad) *
                                                         new Transformation(new Vector(-1.0f, 0f, 1.0f)));
       }
       else
       {
           throw new ArgumentException("Invalid camera mode, accepted orthogonal or perspective");
       }

       var image = new HDRImage(Width, Height);
       var tracer = new ImageTracer(image, camera, samplePerSide: 4);

       //PER LE FORME APPLICARE LA TRASLAZIONE PER ULTIMA (PRIMA NELLA CONCATENAZIONE)
       var s1 = new Sphere(new Transformation(new Vector(0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s2 = new Sphere(new Transformation(new Vector(-0.5f, 0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s3 = new Sphere(new Transformation(new Vector(0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s4 = new Sphere(new Transformation(new Vector(0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s5 = new Sphere(new Transformation(new Vector(-0.5f, -0.5f, 0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s6 = new Sphere(new Transformation(new Vector(0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s7 = new Sphere(new Transformation(new Vector(-0.5f, 0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s8 = new Sphere(new Transformation(new Vector(-0.5f, -0.5f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material1);
       var s9 = new Sphere(new Transformation(new Vector(0f, 0f, -0.5f)) * new Transformation(0.1f, 0.1f, 0.1f), material2);
       var s10 = new Sphere(new Transformation(new Vector(0f, 0.5f, 0f)) * new Transformation(0.1f, 0.1f, 0.1f), material3);

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

       Render render;

       switch (Algorithm)
       {
           case RenderFunc.OnOff:
               render = new OnOff(world);
               break;
           case RenderFunc.Flat:
               render = new Flat(world);
               break;
           //case RenderFunc.PathTracer:
               //render = new PathTracer(world);
               //break;
           default:
               throw new ArgumentException("Invalid renderer mode, accepted onoff, flat or pathtracer");
       }

       tracer.FireAllRays(ray => render.RenderFunction(ray));

<<<<<<< HEAD
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