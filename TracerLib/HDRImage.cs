using System.Diagnostics;

/*
 * using Colors;
   using System.Diagnostics;
   using System.Text;
   using Exception;
 */
namespace TracerLib;

namespace Hdr;

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

    //Variables HDR image
    public int Width { get; set; }
    public int Height { get; set; }
    public Color[]? Pixels { get; set; } //Controllare nullable

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
    
    //Checker per la validità delle coordinate
    public bool _AreCoordinatesValid(int column, int row)
    {
        return column >= 0 && column < Width && row >= 0 && row < Height;
    }

    //index and range for the pixels 1D vector with the type indexer
    public Color this[Index index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }

    public Color[] this[Range range] => Pixels[range];

    public int PixelOffset(int column, int row)
    {
        Debug.Assert(_AreCoordinatesValid(column, row));
        return row * Width + column;
    }

    //index for the pixels "matrix"
    //Attention: the matrix elements are indexed by giving first the colums and then the row!
    public Color this[int column, int row]
    {
        //Debug.Assert(_AreCoordinatesValid(column,row));
        get => Pixels[PixelOffset(column, row)];
        set => Pixels[PixelOffset(column, row)] = value;
    }

    //Begin - Constructors 
    public HDRImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
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

    //Costruttore immagine Hdr a partire da una stream
    public HDRImage(Stream stream)
    {
        // read_pfm_file(stream);
    }

    //Costruttore immagine Hdr a partire da un file
    public HDRImage(string fileName)
    {
        using (Stream filestream = File.OpenRead(fileName))
        {
            // read_pfm_file(filestream);
        }
    }

    //End - Constructors

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

    

    /*//io preferisco che le funzioni vengano chiamate are... o is... ma è questione di gusto se
     //non sei d'accordo va bene anche _ValidCoord. Meglio togliere i trattini bassi tra le parole
     //se vogliamo seguire la convenzione. Almeno io ho capito così cercando su internet.
    public bool _Valid_coord( int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }*/

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


    public float ReadFloat(Stream stream, bool bigEndian = true)
    {
        var value = new byte[4];
        var totalRead = 0;

        while (totalRead < 4)
        {
            var bytesRead = stream.Read(value, totalRead, 4 - totalRead);

            if (bytesRead == 0)
            {
                throw new InvalidPfmFileFormat("Impossibile leggere i file binari dai dati");
            }

            totalRead += bytesRead;
        }

        if (BitConverter.IsLittleEndian != bigEndian)
        {
            Array.Reverse(value);
        }

        return BitConverter.ToSingle(value, 0);
    }

    public static void WriteFloat(Stream outputstream, float value)
    {
        var seq = BitConverter.GetBytes(value);
        outputstream.Write(seq, 0, seq.Length);
    }

    /* public HdrImage read_pfm_file(Stream stream)
     {
         StreamReader sr = new StreamReader(stream);
         var magic = sr.ReadLine();
         if (magic != "PF")
         {
             throw new InvalidPfmFileFormat("Invalid magic in PFM file");
         }

         var imgSize = sr.ReadLine();
         var width, height = _parse_img_size(imgSize);

         var endiannessLine = sr.ReadLine();
         var endianness = _parse_endianness(endiannessLine);

         var result = new HdrImage(width, height);
         var color = new Color();
         for (int i = height-1; i >= 0; i--){
             for (int j = 0; j <= width; j++){
                 color.R = ReadFloat(stream, endianness);
                 color.G = ReadFloat(stream, endianness);
                 color.B = ReadFloat(stream, endianness);
                 result.Set_pixel(j, i, color);
             }
         }

         return result;
     } */

    public static void write_pfm_file(HdrImage img, double endian, string filename)
    {
        using (Stream filestream = File.OpenWrite(filename))
        {
            var header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n{endian}\n");
            filestream.Write(header, 0, header.Length);

            for (int i = img.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j <= img.Width; j++)
                {
                    var color = img.Get_pixel(j, i);
                    WriteFloat(filestream, color.R);
                    WriteFloat(filestream, color.G);
                    WriteFloat(filestream, color.B);
                }
            }
        }
    }

    // Oveloading write_pfm con stream
    public static void write_pfm_file(HdrImage img, double endian, Stream filestream)
    {
        var header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n{endian}\n");
        filestream.Write(header, 0, header.Length);

        for (int i = img.Height - 1; i >= 0; i--)
        {
            for (int j = 0; j <= img.Width; j++)
            {
                var color = img.Get_pixel(j, i);
                WriteFloat(filestream, color.R);
                WriteFloat(filestream, color.G);
                WriteFloat(filestream, color.B);
            }
        }
    }

    public void _Normalize(float factor, float? average_luminosity = null)
    {
        average_luminosity ??= AverageLuminosity();
        foreach (Color color in Pixels)
        {
            color = color * (factor / average_luminosity);
        }
    }
    
    /*(Giacomo)
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
}