// This file is released under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel.DataAnnotations;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;

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

    [Option("--inputscene", Description = "The input scene file name")]
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

[Command(Name = "averageimages",
    Description =
        "Generate an image averaging the color of multiple images using different seed in pathtracing renderer")]
public class AverageImagesCommand
{
    #region Options

    [Option("--outputaveragepfm", Description = "Name of the output pfm file")]
    [Required]
    public required string OutputPfmFileName { get; set; }

    [Option("--outputaveragepng", Description = "Name of the output png file")]
    [Required]
    public required string OutputPngFileName { get; set; }

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

    public void OnExecute()
    {
        PrintParameters();
        SetIOFilesPaths(out string inputFileFolder, out string pfmFilePath, out string pngFilePath);
        HDRImage[] images = ReadPfmImages(inputFileFolder, "*_state*_seq*.pfm");

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
        Console.WriteLine($"Luminosityfunction: {Luminosityfunction}");
        Console.WriteLine($"Averageluminosity: {AverageLuminosity}");
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
    public HDRImage[]? ReadPfmImages(string inputFileFolder, string pattern = null)
    {
        string[] files;
        if (pattern == null)
        {
            files = Directory.GetFiles(inputFileFolder);
        }
        else
        {
            files = Directory.GetFiles(inputFileFolder, pattern);
        }

        if (files.Length == 0)
        {
            Console.WriteLine("The folder is empty");
            return null;
        }

        HDRImage[] images = new HDRImage[files.Length];
        for (int i = 0; i < files.Length; i++) images[i] = HDRImage.ReadPFM_File(files[i]);

        int Width = images[0].Width;
        int Height = images[0].Height;

        foreach (HDRImage image in images)
        {
            if (image.Width != Width || image.Height != Height)
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