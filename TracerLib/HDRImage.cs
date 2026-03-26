using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace TracerLib;

public class HDRImage
{
    //provare a mettere delle verifiche sulle funzioni get e set
    //per verificare per esempio che RGB siano positivi e che get
    //e set pixel verifichino che row e column siano positivi con
    //la funzione validCoordinates. E vedere se il programma non
    //rallenta troppo


    /*private int _width;
    private int _height;
    private Color[] _pixels;
    //List<Color> colors;

    //get and set functions
    public int Width
    {
        get => _width;
        set => _width = value;
    }

    public int Height
    {
        get => _height;
        set => _height = value;
    }

    public Color[] Pixels
    {
        get => _pixels;
        set => _pixels = value;
    }*/
    //al posto che scrivere i metodi get e set e le variabili private si può fare tutto in un solo colpo così
    public int Height { get; set; }
    public int Width { get; set; }
    public Color[] Pixels { get; set; }

    //con i controlli invece viene
    /*private int width;
     private height;
     private Color[] pixels;

     public int Width{
        get{return width;}
        if(value < 0){
            throw new ArgumentException("the width must be >= 0")
        }
        set{}
     }

     */

    //index and range for the pixels 1D vector with the type indexer
    public Color this[Index index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }
    public Color[] this[Range range] => Pixels[range];

    public int Offset(int column, int row)
    {
        return row * Width + column;
    }

    //index for the pixels "matrix"
    //Attention: the matrix elements are indexed by giving first the colums and then the row!
    public Color this[int column, int row]
    {
        get => Pixels[Offset(column, row)];
        set => Pixels[Offset(column, row)] = value;
    }

    //Constructors
    public HDRImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[width * height];
    }

    public HDRImage(int width, int height,
        in Color[] colorVector) //capire come mai senza aver messo public allo struct color dava errore
    {
        Debug.Assert(colorVector.Length == width * height); //da mettere nel main?
        Width = width;
        Height = height;
        Pixels = colorVector;
    }

    /* da rifinire i controlli
    public HDRImage(int width, int height,
        in Color[,] colorMatrixColumnsPerRows)
    {
        Debug.Assert(color.Length == width * height); //da mettere nel main?
        _width = width;
        _height = height;
        _pixels = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                _pixels[Offset(i, j)] = colorMatrixColumnsPerRows[i, j];
            }
        }
    }
    */

    public void Print()
    {
        Console.WriteLine("Height: {0}, Width: {1}", Height, Width);
        Console.WriteLine("Pixel's matrix:");
        Console.WriteLine("\tColumns ->");
        Console.Write("Rows");
        for (int j = 0; j < Width; j++)
        {
            Console.Write($"\t{j}");
        }

        Console.WriteLine();
        for (int i = 0; i < Height; i++)
        {
            Console.Write($"{i}\t");
            for (int j = 0; j < Width; j++)
            {
                Pixels[i * Width + j].Print();
                Console.Write("\t");
            }

            Console.WriteLine("");
        }
    }

    //non so quando usarla
    public bool _AreCoordinatesValid(int column, int row)
    {
        return column >= 0 && column < Width && row >= 0 && row < Height;
    }

    public static void _Parse_img_size(in string stringImgSize, out int width, out int height)
    {
        string[] stringSizeArray = stringImgSize.Split(" ");
        if (stringSizeArray.Length != 2)
        {
            throw new ArgumentException(
                "there isn't the right number of sizes: there must be two sizes width and height.");
        }

        try
        {
            width = int.Parse(stringSizeArray[0]);
            height = int.Parse(stringSizeArray[1]);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("The size is not made of two ints.");
        }

        if (width < 0)
        {
            throw new ArgumentException("width must be greater than zero.");
        }

        if (height < 0)
        {
            throw new ArgumentException("height must be greater than zero.");
        }
    }

    //rivedere se restituire un float o un int
    public static int _Parse_endianness(string stringEndianness)
    {
        int endian = (int)float.Parse(stringEndianness);
        if (endian != 1 && endian != -1)
        {
            throw new ArgumentException("The endianness must be written as 1.0 or -1.0");
        }

        return endian;
    }

/*
    public float _AverageLuminosity(float delta = 1e-10f)
    {
        float sum = 0;

        foreach (Color color in Pixels)
        {
            //perceived value of luminosity follows a logarithmic scale
            //so we must use a logarithmic average
            //delta is needed to avoid singular values for the logarithm
            sum += (float)Math.Log10(delta + color.Luminosity());
        }

        return (float)Math.Pow(10, sum / Pixels.Length);
    }
    */

    public void _Normalize(float factor, float? average_luminosity = null)
    {
        average_luminosity ??= AverageLuminosity();
        foreach(Color color in Pixels)
        {
            color = color * (factor/average_luminosity);
        }
    }

}
