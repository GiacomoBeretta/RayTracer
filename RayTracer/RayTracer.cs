// This file is released under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel.DataAnnotations;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;

namespace RayTracer;

[Command(Name = "RayTracer")]
[Subcommand(typeof(RenderCommand), typeof(AverageImagesCommand), typeof(PfmToPngCommand))]
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

    [Option("--inputscene", Description = "Name of the input scene file.")]
    [Required]
    public string InputSceneName { get; init; } = null!;

    [Option("--outputpfm", Description = "Name of the pfm output file.")]
    [Required]
    public string OutputPfmName { get; init; } = null!;

    [Option("--outputpng", Description = "Name of the png output file.")]
    [Required]
    public string OutputPngName { get; init; } = null!;

    [Option("--width", Description = "The width of the image. Must be >= 1 (default: 500).")]
    [Range(1, Int32.MaxValue)]
    public int Width { get; init; } = 500;

    [Option("--height", Description = "The height of the image. Must be >= 1 (default: 500).")]
    [Range(1, Int32.MaxValue)]
    public int Height { get; init; } = 500;

    [Option("--sampleside", Description = "Number of samples per pixel's side used for antialiasing. " +
                                          "Must be >= 1 (default: 1).")]
    [Range(1, Int32.MaxValue)]
    public int SampleSide { get; init; } = 1;

    [Option("--algorithm",
        Description = "Render's algorithm. Options are: onoff, flat or pathtracing. (default: pathtracing).")]
    public RenderFunc Algorithm { get; init; } = RenderFunc.PathTracing;

    [Option("--numrays",
        Description =
            "Number of rays departing from each surface. Must be >= 1 " +
            "(this command only works for the path tracing algorithm, default: 10).")]
    [Range(1, Int32.MaxValue)]
    public int NumRays { get; init; } = 10;

    [Option("--maxdepth",
        Description = "Maximum allowed ray depth. Must be >= 1 " +
                      "(this command only works for the path tracing algorithm, default: 2).")]
    [Range(1, Int32.MaxValue)]
    public int MaxDepth { get; init; } = 2;

    [Option("--initstate",
        Description =
            "Initial seed for the random number generator. Must be >= 0 " +
            "(this command only works for the path tracing algorithm, default: 45).")]
    [Range(0, ulong.MaxValue)]
    public ulong InitState { get; init; } = 45;

    [Option("--initseq",
        Description =
            "Identifier of the sequence produced by the random number generator. Must be >= 0 " +
            "(this command only works for the path tracing algorithm, default: 54).")]
    [Range(0, ulong.MaxValue)]
    public ulong InitSeq { get; init; } = 54;

    [Option("--roulettestart",
        Description =
            "Number of ray reflections after which the Russian roulette algorithm is applied. Must be >= 0 " +
            "(this command only works for the path tracing algorithm, default: 3).")]
    [Range(0, Int32.MaxValue)]
    public int RussianRouletteStartDepth { get; init; } = 3;

    [Option("--rouletteprob",
        Description =
            "Optional fixed probability for the Russian roulette algorithm. Accepted Range: [0,1] U {null}. " +
            "When null, the probability is computed dynamically at each recursive call of RenderFunction " +
            "(this command only works for the path tracing algorithm, default: null).")]
    [Range(0.0, 1.0)]
    public float? RussianRouletteFixedProb { get; init; } = null;

    [Option("--luminosityfunction",
        Description = "Luminosity function, options are: shirley, weighted (default: shirley).")]
    public LumFunction Luminosityfunction { get; init; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. Must be > 0. " +
            "If the value is null is computed with the luminosity function (default: null).")]
    public float? AverageLuminosity { get; init; } = null;

    [Option("--factor", Description = "The empirical factor to render images. Must be >= 0 (default: 1).")]
    [Range(0, float.MaxValue)]
    public float Factor { get; init; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen. Must be > 0 (default: 1).")]
    public float Gamma { get; init; } = 1f;

    [Option("--declarefloat|-d", Description = "Declare a variable. " +
                                               "The syntax is '--declarefloat=NAME:VALUE' or '-d=NAME:VALUE'.")]
    public string[] Definitions { get; init; } = [];

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

    public void ValidateParameters()
    {
        Functions.EnsureGreaterThanOrEqual<int>(Width, nameof(Width), 1);
        Functions.EnsureGreaterThanOrEqual<int>(Height, nameof(Height), 1);
        Functions.EnsureGreaterThanOrEqual<int>(SampleSide, nameof(SampleSide), 1);
        Functions.EnsureGreaterThanOrEqual<int>(NumRays, nameof(NumRays), 1);
        Functions.EnsureGreaterThanOrEqual<int>(MaxDepth, nameof(MaxDepth), 1);
        Functions.EnsureGreaterThanOrEqual<ulong>(InitState, nameof(InitState), 0);
        Functions.EnsureGreaterThanOrEqual<ulong>(InitSeq, nameof(InitSeq), 0);
        Functions.EnsureGreaterThanOrEqual<int>(RussianRouletteStartDepth, nameof(RussianRouletteStartDepth), 0);
       
        if (RussianRouletteFixedProb.HasValue)
        {
            Functions.EnsureInRange<float>(RussianRouletteFixedProb.Value, nameof(RussianRouletteFixedProb), 0, 1);
        }

        if (AverageLuminosity.HasValue)
        {
            Functions.EnsureGreaterThan<float>(AverageLuminosity.Value, nameof(AverageLuminosity), 0);
        }

        Functions.EnsureGreaterThanOrEqual<float>(Factor, nameof(Factor), 0);
        Functions.EnsureGreaterThan<float>(Gamma, nameof(Gamma), 0);
    }

    /// <summary>
    /// Prints all the parameters passed through the command line.
    /// </summary>
    public void PrintParameters()
    {
        Console.WriteLine("File names:");
        Console.WriteLine($"Input: {InputSceneName}");
        Console.WriteLine($"Output PFM name: {OutputPfmName}");
        Console.WriteLine($"Output PNG name: {OutputPngName}");
        Console.WriteLine();
        Console.WriteLine($"Width: {Width}");
        Console.WriteLine($"Height: {Height}");
        Console.WriteLine($"Sample per Side: {SampleSide}");
        Console.WriteLine($"Algorithm: {Algorithm}");
        Console.WriteLine();

        if (Algorithm == RenderFunc.PathTracing)
        {
            Console.WriteLine("extra parameters for path tracing:");
            Console.WriteLine($"NumRay: {NumRays}");
            Console.WriteLine($"Max Depth: {MaxDepth}");
            Console.WriteLine($"InitState: {InitState}");
            Console.WriteLine($"InitSeq: {InitSeq}");
            Console.WriteLine($"Roulette Start Depth: {RussianRouletteStartDepth}");
            Console.WriteLine($"Roulette Fixed Probability: {RussianRouletteFixedProb}");
            Console.WriteLine();
        }

        Console.WriteLine("Tone Mapping parameters:");
        Console.WriteLine($"Luminosity Function: {Luminosityfunction}");
        Console.WriteLine($"Average Luminosity: {AverageLuminosity}");
        Console.WriteLine($"Factor: {Factor}");
        Console.WriteLine($"Gamma: {Gamma}");
        Console.WriteLine();
        Console.Write("Definitions: ");
        foreach (string def in Definitions)
        {
            Console.WriteLine($"{def}");
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
            case RenderFunc.PathTracing:
                renderer = new PathTracingRenderer(scene.World, new PCG(InitState, InitSeq), backgroundColor: null,
                    NumRays, MaxDepth, RussianRouletteStartDepth, RussianRouletteFixedProb);
                break;
            default:
                throw new ArgumentException("Invalid algorithm, accepted onoff, flat or pathtracing");
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

[Command(Name = "averageimages",
    Description =
        "Generate an image averaging the color of multiple images using different seed in pathtracing renderer")]
public class AverageImagesCommand
{
    #region Options

    [Option("--outputpfm", Description = "Name of the averaged output PFM file.")]
    [Required]
    public string OutputPfmFileName { get; init; } = null!;

    [Option("--outputpng", Description = "Name of the averaged output PNG file.")]
    [Required]
    public string OutputPngFileName { get; init; } = null!;

    [Option("--luminosityfunction",
        Description = "Luminosity function, options are: shirley, weighted (default: shirley).")]
    public LumFunction Luminosityfunction { get; init; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. " +
            "If the value is null is computed with the luminosity function (default: null).")]
    public float? AverageLuminosity { get; init; } = null;

    [Option("--factor", Description = "The empirical factor to render images (default: 1).")]
    public float Factor { get; init; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen (default: 1).")]
    public float Gamma { get; init; } = 1f;

    #endregion

    public void OnExecute()
    {
        PrintParameters();
        SetIOFilesPaths(out string inputFileFolder, out string pfmFilePath, out string pngFilePath);
        HDRImage[]? images = ReadPfmImages(inputFileFolder, "*_state*_seq*.pfm");

        if (images == null)
        {
            Console.WriteLine("No images found");
            return;
        }

        HDRImage output = AverageImages(images);

        HDRImage.WritePFM_File(output, pfmFilePath);
        Console.WriteLine($"Pfm file created in: {pfmFilePath}");

        output.WritePNG(OutputPngFileName, Luminosityfunction, Factor, Gamma, AverageLuminosity);
        Console.WriteLine($"Png file created in: {pngFilePath}");
    }

    /// <summary>
    /// Prints all the parameters passed through the command line.
    /// </summary>
    public void PrintParameters()
    {
        Console.WriteLine($"Name of the output pfm file path: {OutputPfmFileName}");
        Console.WriteLine($"Name of the output png file path: {OutputPngFileName}");
        Console.WriteLine();
        Console.WriteLine("Tone Mapping parameters");
        Console.WriteLine($"Luminosity Function: {Luminosityfunction}");
        Console.WriteLine($"Average Luminosity: {AverageLuminosity}");
        Console.WriteLine($"Factor: {Factor}");
        Console.WriteLine($"Gamma: {Gamma}");
    }

    /// <summary>
    /// Builds the path of the folder of the pfm images to average
    /// and the paths of the output pfm and png images.
    /// <c>.pfm</c> and <c>.png</c> extensions are appended to the output file
    /// names if they are missing.
    /// </summary>
    /// <param name="inputFileFolder">Full path to the input folder</param>
    /// <param name="pfmFilePath">Full path to the output PFM file.</param>
    /// <param name="pngFilePath">Full path to the output PNG file.</param>
    public void SetIOFilesPaths(out string inputFileFolder, out string pfmFilePath, out string pngFilePath)
    {
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        inputFileFolder = Path.Combine(currentPath, "../../../../PfmImages");

        string pfmFileName = OutputPfmFileName.EndsWith(".pfm") ? OutputPfmFileName : OutputPfmFileName + ".pfm";
        string pngFileName = OutputPngFileName.EndsWith(".png") ? OutputPngFileName : OutputPngFileName + ".png";

        pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages", pfmFileName);
        pngFilePath = Path.Combine(currentPath, "../../../../PngImages/", pngFileName);
    }

    /// <summary>
    /// Loads all PFM images from a specified folder and returns them as an array of HDRImage objects.
    /// </summary>
    /// <param name="inputFileFolder">The directory containing the PFM files.</param>
    /// <param name="pattern">
    /// Optional search pattern used to filter files (e.g. "*.pfm").
    /// If null, all files in the directory are loaded.
    /// </param>
    /// <returns>
    /// An array of HDRImage objects loaded from the folder, or null if the folder contains no files.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if at least one loaded image has a different width or height compared to the first image.
    /// </exception>
    /// <remarks>
    /// All images in the folder must have identical dimensions.
    /// The method prints a message to the console and returns null if no files are found.
    /// </remarks>
    public HDRImage[]? ReadPfmImages(string inputFileFolder, string? pattern = null)
    {
        string[] files;
        if (pattern == null)
        {
            Console.WriteLine($"Searching for PFM images from the {inputFileFolder} folder.");
            files = Directory.GetFiles(inputFileFolder);
        }
        else
        {
            Console.WriteLine(
                $"Searching for PFM images from the {inputFileFolder} folder, with the pattern {pattern}.");
            files = Directory.GetFiles(inputFileFolder, pattern);
        }

        if (files.Length == 0)
        {
            Console.WriteLine("The folder is empty");
            return null;
        }

        HDRImage[] images = new HDRImage[files.Length];
        for (int i = 0; i < files.Length; i++) images[i] = HDRImage.ReadPFM_File(files[i]);

        int width = images[0].Width;
        int height = images[0].Height;

        foreach (HDRImage image in images)
        {
            if (image.Width != width || image.Height != height)
                throw new ArgumentException("Images must have equal width and height");
        }

        return images;
    }

    /// <summary>
    /// Returns the averaged image from an array of HDR images.
    /// </summary>
    /// <param name="images">
    /// Array of HDRImage objects to be averaged. All images must have the same dimensions.
    /// </param>
    /// <returns>
    /// A new HDRImage containing the average pixel values computed across all input images.
    /// </returns>
    /// <remarks>
    /// The method performs a per-pixel arithmetic mean across all images.
    /// All images are assumed to have identical width, height, and pixel layout.
    /// </remarks>
    public HDRImage AverageImages(HDRImage[] images)
    {
        //Using first file as accumulator
        HDRImage acc = images[0];

        for (int i = 0; i < acc.Pixels.Length; i++)
        {
            for (int j = 1; j < images.Length; j++)
            {
                acc[i] += images[j][i];
            }

            acc[i] *= (1.0f / images.Length);
        }

        return acc;
    }
}

[Command(Name = "pfmtopng", Description = "Converts a PFM image to PNG.")]
public class PfmToPngCommand
{
    #region Options

    [Option("--inputpfm", Description = "The input PFM file name.")]
    [Required]
    public string InputFileName { get; init; } = null!;

    [Option("--outputpng", Description = "The output PNG file name.")]
    [Required]
    public string OutputFileName { get; init; } = null!;

    [Option("--luminosityfunction",
        Description = "Luminosity function, options are: shirley, weighted (default: shirley).")]
    public LumFunction Luminosityfunction { get; init; } = LumFunction.Shirley;

    [Option("--averageluminosity",
        Description =
            "Fixed luminosity for the tone mapping. " +
            "If the value is null is computed with the luminosity function (default: null).")]
    public float? AverageLuminosity { get; init; } = null;

    [Option("--factor", Description = "The empirical factor to render images (default: 1).")]
    public float Factor { get; init; } = 1f;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen (default: 1).")]
    public float Gamma { get; init; } = 1f;

    #endregion

    internal void OnExecute()
    {
        Console.WriteLine($"input PFM file name: {InputFileName}");
        Console.WriteLine($"output PNG file name: {OutputFileName}");
        Console.WriteLine($"Luminosity Function: {Luminosityfunction}");
        Console.WriteLine($"Average Luminosity: {AverageLuminosity}");
        Console.WriteLine($"factor: {Factor}");
        Console.WriteLine($"gamma: {Gamma}");

        string pngfilename = OutputFileName.EndsWith(".png") ? OutputFileName : OutputFileName + ".png";

        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        string pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages", InputFileName);
        string pngFilePath = Path.Combine(currentPath, "../../../../PngImages/", pngfilename);

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
    PathTracing
}