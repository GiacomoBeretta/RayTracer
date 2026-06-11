// This file is released under EUPL_v1.2 license. See LICENSE.md

using System.ComponentModel;
using SixLabors.ImageSharp.Processing;
using TracerLib;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp.Metadata.Profiles.Exif; // per rendere le opzioni o gli argomenti required

[Command(Name = "RayTracer")]
[Subcommand(typeof(DemoCommand), typeof(PfmToPngCommand)/*, typeof(AverageImageCommand)*/, typeof(RenderCommand))]
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

[Command(Name = "Demo", Description = "Generates a simple test image composed of 10 spheres:\n" +
                                      "eight spheres are placed at the vertices of a cube, and two are positioned at the centers of two faces.\n" +
                                      "Allows configuration of image resolution and camera orientation via azimuth and zenith angles.\n" +
                                      "Supports two projection modes: perspective (default) and orthogonal.")]
public class DemoCommand
{
    [Option("--width", Description = "The width of the image")]
    [Range(1, Int32.MaxValue)]
    public int Width { get; init; } = 500;

    [Option("--height", Description = "The height of the image")]
    [Range(1, Int32.MaxValue)]
    public int Height { get; init; } = 500;

    [Option("--output", Description = "The name of the png file")]
    public string OutputFileName { get; init; } = "referenceShirley.png";

    [Option("--algorithm", Description = "Render's algorithm. OnOffRenderer passed by default")]
    public RenderFunc Algorithm { get; init; } = RenderFunc.OnOff;      
    
    [Option("--theta", Description = "Observer's azimuthal angle in degrees")]
    [Range(0.0f, 180.0f)]
    public float Theta { get; init; } = 0.0f;

    [Option("--phi", Description = "Observer's zenithal angle in degrees")]
    [Range(0.0f, 360.0f)]
    public float Phi { get; init; } = 0.0f;

    [Option("--projection", Description = "projection used to render the image.")]
    public Projection Projection { get; } = Projection.Perspective;

    [Option("--luminosityFunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction LuminosityFunction { get; init; } = LumFunction.Shirley;
    
    //aggiungere range
    [Option("--factor", Description = "The empirical factor to render images")]
    public float Factor { get; init; } = 1.0f;

    //aggiungere range
    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; init; } = 1.0f;

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
       string pngFilePath = Path.Combine(currentPath, "../../../../DemoImages/" + OutputFileName); //"../../../../DemoImages/" dal path dell'eseguibile torna indietro (Controllare)
       if (OutputFileName[^4..] != ".png") pngFilePath += ".png"; //OutputFilename[^4..] Legge gli ultimi 4 caratteri
       
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
       
       Renderer render;

       switch (Algorithm)
       {
           case RenderFunc.OnOff:
               render = new OnOffRenderer(world);
               break;
           case RenderFunc.Flat:
               render = new FlatRenderer(world);
               break;
           //case RenderFunc.PathTracer:
               //render = new PathTracer(world);
               //break;
           default:
               throw new ArgumentException("Invalid renderer mode, accepted onoff, flat or pathtracer");
       }
       
       tracer.FireAllRays(ray => render.RenderFunction(ray));
       
       image.WritePNG(pngFilePath, LuminosityFunction, Factor, Gamma, averageLuminosity: 0.5f);
       Console.WriteLine("The PNG file has been saved in DemoImages/" + OutputFileName);
   }
    /*
    private void OnExecute()
    {
        // Print the parameters passed on the command line
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
        var skyMaterial = new Material(new UniformPigment(new Color(1.0f, 0.9f, 0.5f)),
            new DiffuseBRDF(new UniformPigment(new Color(0.0f, 0.0f, 0.0f))));
        var groundMaterial =
            new Material(new CheckeredPigment(new Color(0.3f, 0.5f, 0.1f), new Color(0.1f, 0.2f, 0.5f)), new DiffuseBRDF());
        var sphereMaterial = new Material(new UniformPigment(new Color(0.3f, 0.4f, 0.8f)), new DiffuseBRDF());
        var mirrorMaterial = new Material(new UniformPigment(new Color(0.6f, 0.2f, 0.3f)), new SpecularBRDF());
        
        // Define the shapes of the scene
        var world = new World();
        world.Add(new Sphere(new Transformation(new Vector(0f, 0f, 0.4f)) * new Transformation(200f,200f,200f), skyMaterial));
        world.Add(new Plane(new Transformation(), groundMaterial));
        world.Add(new Sphere(new Transformation(new Vector(0f, 0f, 1f)), sphereMaterial));
        world.Add(new Sphere(new Transformation(new Vector(1f, 2.5f, 0f)), mirrorMaterial));
        
        // Choose and position the camera
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
        
        // Choose the render algorithm
        Render render;
        switch (Algorithm)
        {
            case RenderFunc.OnOff:
                render = new OnOff(world);
                break;
            case RenderFunc.Flat:
                render = new Flat(world);
                break;
            case RenderFunc.PathTracer:
                render = new PathTracer(world);
                break;
            default:
                throw new ArgumentException("Invalid renderer mode, accepted onoff, flat or pathtracer");
        }
        
        // Create the PFM image
        var image = new HDRImage(Width, Height);
        var tracer = new ImageTracer(image, camera, samplePerSide: 4);
        tracer.FireAllRays(ray => render.RenderFunction(ray));
        
        // Write the PNG file
        image.WritePNG(pngFilePath, LuminosityFunction, Factor, Gamma, averageLuminosity: 0.5f);
        Console.WriteLine("The PNG file has been saved in DemoImages/" + OutputFileName);
    }*/
}


[Command(Name = "pfmtopng", Description = "Converts a PFM image to PNG")]
public class PfmToPngCommand
{
    
    [Option("--input", Description = "The input file path")]
    [Required]
    public required string InputFilePath { get; set; } 

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

[Command(Name = "render", Description = "Read a scene file and creates the corresponding image")]
public class RenderCommand
{
    [Option("--input", Description = "The input scene file path")]
    public string InputScene { get; init; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Scenes/scene.txt");
    
    [Option("--width", Description = "The width of the image")]
    [Range(1, Int32.MaxValue)]
    public int Width { get; init; } = 500;

    [Option("--height", Description = "The height of the image")]
    [Range(1, Int32.MaxValue)]
    public int Height { get; init; } = 500;

    [Option("--algorithm", Description = "Render's algorithm; pathTracer passed by default")]
    public RenderFunc Algorithm { get; init; } = RenderFunc.PathTracer;

    [Option("--output-pfm", Description = "Name of the pfm file output")]
    public string OutputPfm { get; init; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../PfmImages/output.pfm");

    [Option("--output-png", Description = "Name of the png file output")]
    public string OutputPng { get; init; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../PngImages/output.png");

    [Option("--num-rays",
        Description =
            "Number of rays departing from each surface (this command only works for the Pathtracing algorithm)")]
    public int NumRays { get; init; } = 10;

    [Option("--max-depth",
        Description = "Maximum allowed ray depth (this command only works for the Pathtracing algorithm)")]
    public int MaxDepth { get; init; } = 3;

    [Option("--init-state", Description = "Initial seed for the random number generator")]
    [Range(0, ulong.MaxValue)]
    public ulong InitState { get; init; } = 45;

    [Option("--init-seq", Description = "Identifier of the sequence produced by the random number generator")]
    [Range(0, ulong.MaxValue)]
    public ulong InitSeq { get; init; } = 54;

    [Option("--sample-side", Description = "Number of samples per pixel's side")]
    [Range(1, Int32.MaxValue)]
    public int SampleSide { get; init; } = 1;
    
    [Option("--luminosityFunction", Description = "Luminosity function, options are: shirley (default), weighted")]
    public LumFunction LuminosityFunction { get; init; } = LumFunction.Shirley;
    
    [Option("--factor", Description = "The empirical factor to render images")]
    public int Factor { get; init; } = 1;

    [Option("--gamma", Description = "The gamma factor characteristic of the screen")]
    public float Gamma { get; init; } = 1;

    [Option("--declare-float|-d", Description = "Declare a variable. The syntax is --declare-float=NAME:VALUE")]
    public string[] Definitions { get; init; } = [];

    [Option("--roulette-start",
        Description = "Number of ray reflections after which the Russian roulette algorithm is applied")]
    [Range(0, Int32.MaxValue)]
    public int RussianRouletteStartDepth { get; init; } = 3;

    [Option("--roulette-prob", Description = "Optional fixed probability for the Russian roulette algorithm " +
                                             "(when null, the probability is computed dynamically at each recursive call of RenderFunction)")]
    [Range(0, 1)]
    public float? RussianRouletteFixedProb { get; init; } = null;

    public void OnExecute()
    {
        string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        
        string pngFilePath = Path.Combine(currentPath, "../../../../PngImages/" + OutputPng); //"../../../../DemoImages/" dal path dell'eseguibile torna indietro (Controllare)
        if (OutputPng[^4..] != ".png") pngFilePath += ".png"; //OutputFilename[^4..] Legge gli ultimi 4 caratteri

        string pfmFilePath = Path.Combine(currentPath, "../../../../PfmImages" + OutputPfm);
        if (OutputPfm[^4..] != ".pfm") pfmFilePath += ".pfm";
        
        var scene = new Scene();
        var input = new InputStream(InputScene);
        var variables = Functions.VariableTable(Definitions);

        try
        {
            scene.ParseScene(input, variables);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message); //Verificare che stampi anche la posizione dell'errore
        }

        var image = new HDRImage(Width, Height);
        
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
                renderer = new PathTracingRenderer(scene.World, new PCG(InitState, InitSeq), backgroundColor: null, NumRays, MaxDepth, RussianRouletteStartDepth, RussianRouletteFixedProb);
                break;
            default:
                throw new ArgumentException("Invalid renderer mode, accepted onoff, flat or pathtracer");
        }
        
        if(scene.Camera == null)
        {
            Console.WriteLine("Not initialized camera. Follows default initialization [perspective]");
            scene.Camera = new PerspectiveCamera();
        }
        
        var tracer = new ImageTracer(image, scene.Camera, samplePerSide: SampleSide);
        tracer.FireAllRays(ray => renderer.RenderFunction(ray));
        
        HDRImage.WritePFM_File(image, pfmFilePath);
        image.WritePNG(pngFilePath, LuminosityFunction, Factor, Gamma, averageLuminosity: 0.5f); 
        
    }
}

/*[Command(Name = "averageimage", Description = "Generate an image averaging the color of multiple images using different seed in pathtracing renderer")]
public  class AverageImageCommand
{
    [Option("--input", Description = "Input file path")]
    [Required]
    public required string InputFilePath { get; init; }
    
    [Option("--output", Description = "Output file path")]
    [Required]
    public required string OutputFilePath { get; init; }
}*/

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
       
       image.WritePNG(pngFilePath, LuminosityFunction, Factor, Gamma, averageLuminosity: 0.5f);
       Console.WriteLine("The PNG file has been saved in DemoImages/" + OutputFileName);
   }*/