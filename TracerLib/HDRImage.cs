// This file is release under EUPL_v1.2 license. See LICENSE.md

// Implementare dei controlli per i constructor e in altre funzioni se necessario, vediamo cosa dice Tomasi in proposito.
// Forse si possono mettere i membri privati e rendere la classe dei test una friend?

using System.Diagnostics.CodeAnalysis; // per sopprimere i messaggi di errore
using System.Globalization; //per il metodo cultureInfo e quindi per risolvere il problema dell'1.0 che viene letto come 10
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats; //for the Rgb24 Pixel Format

namespace TracerLib;

using System.Text; //for the Encoding.ASCII.GetBytes

/// <summary>
/// An HDRImage is essentially a matrix of colors with RGB floats (see the Color class)
/// with a Width and a Height of type integer and a one dimensional (for efficiency reason) vector called Pixels
/// Attention: the matrix elements are indexed by giving first the column and then the row!
/// The first pixel is the one at the top left.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class HDRImage
{
    //Provare a mettere delle verifiche sulle funzioni get e set
    //per verificare per esempio che RGB siano positivi e che get
    //e set pixel verifichino che row e column siano positivi con
    //la funzione validCoordinates. E vedere se il programma non
    //rallenta troppo

    //Variables HDR image

    /// <summary>
    /// Width of the matrix of pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Height of the matrix of pixels.
    /// </summary>
    public int Height { get; private set; }

    public Color[] Pixels { get; set; } //Controllare nullable (Color[])?

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
     }*/

    //avendo implementato checkCoordinates non so se è più utile areCoordinatesValid
    /// <summary>
    /// Checks for the validity of the coordinates
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool _AreCoordinatesValid(int column, int row)
    {
        return column >= 0 && column < Width && row >= 0 && row < Height;
    }

    /// <summary>
    /// Checks whether the column or the row are negative
    /// or greater than or equal to the Width and Height values
    /// and throws an exception in either case
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void _CheckCoordinates(int column, int row)
    {
        if (column < 0 || column >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(column), column,
                nameof(column) + $" must be non-negative and less than {Width}");
        }

        if (row < 0 || row >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row,
                nameof(row) + $" must be non-negative and less than {Height}");
        }
    }

    /// <summary>
    /// Checks whether the width or height are negative and throws an exception in either case
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void _CheckWidthHeight(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, nameof(width) + " must be greater than zero");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height,
                nameof(height) + " must be greater than zero");
        }
    }

    /// <summary>
    /// Checks if the Pixel's lenght matches the number of elements in the HDRImage's matrix
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="colorVector"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void _CheckPixels(int width, int height, in Color[] colorVector)
    {
        ArgumentNullException.ThrowIfNull(colorVector);
        if (colorVector.Length != width * height)
        {
            throw new ArgumentException("the Length of the colorVector and the width and height passed do not match",
                nameof(colorVector));
        }
    }

    //vedere se anche per questo indice si possono mettere dei controlli
    //index and range for the pixels 1D vector with the type indexer
    /// <summary>
    /// Returns the <c>Color</c> given by the i-th element of the 1D Pixel's array
    /// </summary>
    /// <param name="index"></param>
    public Color this[Index index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }

    public Color[] this[Range range] => Pixels[range];

    //forse è troppo mettere l'eccezione per offset?
    /// <summary>
    /// Gives the index integer for the one dimensional vector,
    /// given the column and row of the corresponding matrix
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public int _PixelOffset(int column, int row)
    {
        _CheckCoordinates(column, row);
        return row * Width + column;
    }

    //vedere come mettere un controllo con l'eccezione
    /// <summary>
    /// Gives the Color at the indexes (column, row) of the corresponding matrix
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public Color this[int column, int row]
    {
        get => Pixels[_PixelOffset(column, row)];
        set => Pixels[_PixelOffset(column, row)] = value;
    }

    //Constructors - Begin
    public HDRImage(int width, int height)
    {
        _CheckWidthHeight(width, height);
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
    }

    public HDRImage(int width, int height,
        in Color[] colorVector)
    {
        _CheckWidthHeight(width, height);
        _CheckPixels(width, height, colorVector);

        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = colorVector[i];
        }
    }

    public HDRImage(Stream stream)
    {
        var img = ReadPFM_File(stream);

        Width = img.Width;
        Height = img.Height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = img.Pixels[i];
        }
    }

    public HDRImage(string fileName)
    {
        using (Stream filestream = File.OpenRead(fileName))
        {
            var img = ReadPFM_File(filestream);

            Width = img.Width;
            Height = img.Height;
            Pixels = new Color[Width * Height];
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = img.Pixels[i];
            }
        }
    }

    //Copy constructor
    protected HDRImage(HDRImage other)
    {
        Width = other.Width;
        Height = other.Height;
        Pixels = new Color[Width * Height];
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = other.Pixels[i];
        }
    }

    //Constructors - End

    /// <summary>
    /// Returns a string that displays the color matrix.
    /// </summary>
    public override string ToString()
    {
        string str = $"Height: {Height}, Width: {Width}\n" +
                     "Pixel's matrix:\n" +
                     "\tColumns ->\n" +
                     "Rows";
        for (int j = 0; j < Width; j++)
        {
            str += $"\t{j}";
        }

        str += "\n";

        for (int i = 0; i < Height; i++)
        {
            str += $"{i}";
            for (int j = 0; j < Width; j++)
            {
                str += "\t";
                str += Pixels[_PixelOffset(j, i)].ToString();
            }

            str += "\n";
        }

        return str;
    }

    /// <summary>
    /// Prints the string converted HDRImage 
    /// </summary>
    public void Print()
    {
        Console.WriteLine(this.ToString());
    }

    /// <summary>
    /// Returns a clone of this HDRImage
    /// </summary>
    /// <returns></returns>
    public HDRImage Clone()
    {
        return new HDRImage(this);
    }

    //Methods for Read and Write PFM files - Begin

    /// <summary>
    /// Returns the values of width and height (as reference values) that were written in the string stringImgSize 
    /// </summary>
    /// <param name="stringImgSize"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void _ParseImgSize(string stringImgSize, out int width, out int height)
    {
        string[] stringSizeArray = stringImgSize.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (stringSizeArray.Length != 2)
        {
            throw new InvalidPfmFileFormat(
                "there isn't the right number of sizes: there must be two sizes width and height.");
        }

        try
        {
            width = int.Parse(stringSizeArray[0], CultureInfo.InvariantCulture);
            height = int.Parse(stringSizeArray[1], CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
            throw new InvalidPfmFileFormat("The size is not made of two ints.", ex);
        }

        if (width < 0)
        {
            throw new InvalidPfmFileFormat("width must be greater than zero.");
        }

        if (height < 0)
        {
            throw new InvalidPfmFileFormat("height must be greater than zero.");
        }
    }

    /// <summary>
    /// Returns the endianness (true = Big Endian, false = Little Endian)
    /// written in the string stringEndianness as 1.0 or -1.0
    /// </summary>
    /// <param name="stringEndianness"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool _ParseEndianness(string stringEndianness)
    {
        int endian;
        try
        {
            //CultureInfo.InvariantCulture is needed to read a string like "1.0" like one with
            //only 0 as a decimal cipher. Some users may read it as 10 because they do not interpret
            //the dot as the decimal part separator.
            endian = (int)float.Parse(stringEndianness, CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
            throw new InvalidPfmFileFormat(ex.Message);
        }

        if (endian != 1 && endian != -1)
        {
            throw new InvalidPfmFileFormat("The endianness must be written as 1.0 or -1.0");
        }

        if (endian == 1)
        {
            return true;
        }
        else return false;
    }
    
    /// <summary>
    /// Read a single line of bytes in a PFM file format.
    /// </summary>
    /// <param name="br"></param>
    /// <returns></returns>
    public static string _ReadLine(BinaryReader br)
    {
        var bytes = new List<byte>();

        while (true)
        {
            byte b;

            try
            {
                b = br.ReadByte();
            }
            catch (EndOfStreamException)
            {
                if (bytes.Count == 0)
                    return null;
                break;
            }

            if (b == '\n')
                break;

            if (b != '\r')
                bytes.Add(b);
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }

    /// <summary>
    /// Returns the float value of the <c>Color</c> encoded as 4 bytes
    /// </summary>
    /// <param name="br"></param>
    /// <param name="bigEndian"></param>
    /// <returns></returns>
    /// <exception cref="InvalidPfmFileFormat"></exception>
    public static float _ReadFloat(BinaryReader br, bool bigEndian = true)
    {
        var bytes = br.ReadBytes(4);

        if (bytes.Length < 4)
        {
            throw new InvalidPfmFileFormat("Unexpected end of file");
        }

        if (BitConverter.IsLittleEndian == bigEndian)
        {
            Array.Reverse(bytes);
        }

        return BitConverter.ToSingle(bytes, 0);
    }
    
    public static void _WriteFloat(Stream outputstream, float value)
    {
        var seq = BitConverter.GetBytes(value);
        outputstream.Write(seq, 0, seq.Length);
    }

    public static HDRImage ReadPFM_File(Stream stream)
    {
        using var br = new BinaryReader(stream);
        var magic = _ReadLine(br);
        if (magic != "PF")
        {
            throw new InvalidPfmFileFormat("Invalid magic in PFM file");
        }

        var imgSize = _ReadLine(br);
        if (imgSize == null)
        {
            throw new InvalidPfmFileFormat("Missing image size line");
        }

        _ParseImgSize(imgSize, out var width, out var height);

        var endiannessLine = _ReadLine(br);
        if (endiannessLine == null)
        {
            throw new InvalidPfmFileFormat("Missing endianness line");
        }

        var endianness = _ParseEndianness(endiannessLine);

        /*Console.WriteLine($"POS BEFORE PIXELS: {br.BaseStream.Position}");
        long expectedBytes = width * height * 3 * 4;
        Console.WriteLine($"EXPECTED PIXEL BYTES: {expectedBytes}"); */

        var result = new HDRImage(width, height);
        //Console.WriteLine($"Width = {result.Width}, Height = {result.Height}");

        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j <= width - 1; j++)
            {
                //Console.WriteLine($"Column = {j}, Row = {i}");
                //Console.WriteLine($"Offset = {result._PixelOffset(j,i)}");
                //Console.WriteLine($"Offset 2 = {i * result.Width + j}");
                var color = new Color
                {
                    R = _ReadFloat(br, endianness),
                    G = _ReadFloat(br, endianness),
                    B = _ReadFloat(br, endianness)
                };
                result[j, i] = color;
            }
        }

        //if (result.Pixels != null) Console.WriteLine($"W={result.Width}, H={result.Height}, Pixels={result.Pixels.Length}");

        return result;
    }

    public static HDRImage ReadPFM_File(string filename)
    {
        using (Stream filestream = File.OpenRead(filename))
        {
            return ReadPFM_File(filestream);
        }
    }

    //io la cambierei il nome in WritePFM e basta
    public static void WritePFM_File(HDRImage img, double endian, Stream filestream)
    {
        var header = Encoding.ASCII.GetBytes($"PF\n{img.Width} {img.Height}\n{endian}\n");
        filestream.Write(header, 0, header.Length);

        for (int i = img.Height - 1; i >= 0; i--)
        {
            for (int j = 0; j <= img.Width; j++)
            {
                var color = img[j, i];
                _WriteFloat(filestream, color.R);
                _WriteFloat(filestream, color.G);
                _WriteFloat(filestream, color.B);
            }
        }
    }

    public static void WritePFM_File(HDRImage img, double endian, string filename)
    {
        using (Stream filestream = File.OpenWrite(filename))
        {
            WritePFM_File(img, endian, filestream);
        }
    }

    //Methods for Read and Write PFM files - End

    // Methods for conversion to an LDR Image - Begin

    /// <summary>
    /// Computes the average luminosity of the entire image,
    /// using a particular luminosity function of the Color class
    /// based on the value of the luminosityFunction parameter, see <c>LumFunction</c>
    /// (Shirley = Shirley and Morley method, Weighted = Weighted Average)
    /// </summary>
    /// <param name="luminosityFunction"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public float _AverageLuminosity(LumFunction luminosityFunction, float delta = 1e-10f)
    {
        float sum = 0.0f;
        //perceived value of luminosity follows a logarithmic scale
        //so we must use a logarithmic average
        //delta is needed to avoid singular values for the logarithm
        if (luminosityFunction == LumFunction.Shirley)
        {
            foreach (Color color in Pixels)
            {
                sum += MathF.Log10(delta + color.LuminosityShirleyMorley());
            }

            return MathF.Pow(10, sum / Pixels.Length);
        }
        else //if(luminosityFunction == LumFunction.Weighted)
        {
            foreach (Color color in Pixels)
            {
                sum += MathF.Log10(delta + color.LuminosityWeightedAverage());
            }

            return MathF.Pow(10, sum / Pixels.Length);
        }
    }

    /// <summary>
    /// Normalizes the RGB values of each pixel by the average luminosity computed by the AverageLuminosity function
    /// and by another empirical number (here called factor).
    /// The luminosityFunction tells which of function of the color class to use to compute the luminosity of the pixel, see <c>LumFunction</c>
    /// averageLuminosity is an optional parameter you can use if you've already computed it previously.
    /// </summary>
    /// <param name="luminosityFunction"></param>
    /// <param name="factor"></param>
    /// <param name="averageLuminosity"></param>
    /// <param name="delta"></param>
    public void _Normalize(LumFunction luminosityFunction, float factor, float? averageLuminosity = null, float delta = 1e-10f)
    {
        //if averageLuminosity is null compute it with the _AverageLuminosity function
        averageLuminosity ??= _AverageLuminosity(luminosityFunction, delta);
        for (int i = 0; i < Pixels.Length; i++)
        {
            //averageLuminosity is a nullable type so we must explicitly cast it from float? to float
            Pixels[i] = Pixels[i] * (factor / (float)averageLuminosity);
        }
    }

    /// <summary>
    /// Resizes the RGB values of each pixel under 1,
    /// it also scales possible bright spots.
    /// </summary>
    public void _ClampImage()
    {
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i]._Clamp();
        }
    }

    /// <summary>
    /// Converts each pixel to the corresponding sRGB triple
    /// corrected by the characteristic gamma factor of the display
    /// </summary>
    /// <param name="gamma"></param>
    public void _ImageTo8BitRGB(float gamma)
    {
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = Pixels[i].To8BitRGB(gamma);
        }
    }

    //Questa non è tecnicamente un HDR. Rivedere in futuro
    /// <summary>
    /// Returns the corresponding LDR image
    /// It accounts for the gamma correction of the display and of the empirical factor here named "factor"
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel.
    /// averageLuminosity is an optional parameter you can use if you've already computed it previously.
    /// </summary>
    /// <param name="luminosityFunction"></param>
    /// <param name="factor"></param>
    /// <param name="gamma"></param>
    /// <param name="averageLuminosity"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public HDRImage CreateLDR(LumFunction luminosityFunction, float factor, float gamma, float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        HDRImage image = this.Clone();
        image._Normalize(luminosityFunction, factor, averageLuminosity, delta);
        image._ClampImage();
        image._ImageTo8BitRGB(gamma);
        return image;
    }

    // Methods for conversion to an LDR Image - End

    /// <summary>
    /// Writes on the outputStream the corresponding LDR image.
    /// It accounts for the gamma correction of the display and of the empirical factor here named "factor".
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel.
    /// averageLuminosity is an optional parameter you can use if you've already computed it previously.
    /// </summary>
    /// <param name="outputStream"></param>
    /// <param name="luminosityFunction"></param>
    /// <param name="factor"></param>
    /// <param name="gamma"></param>
    /// <param name="averageLuminosity"></param>
    /// <param name="delta"></param>
    public void WritePNG(Stream outputStream, LumFunction luminosityFunction, float factor, float gamma,
        float? averageLuminosity = null,
        float delta = 1e-10f)
    {
        HDRImage LDRimage = this.CreateLDR(luminosityFunction, factor, gamma, averageLuminosity, delta);

        Image<Rgb24> bitmap = new Image<Rgb24>(Configuration.Default, Width, Height);

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                //the Rgb format requires 3 numbers of the type byte
                //so we must convert the RGB values to bytes
                bitmap[i, j] = new Rgb24((byte)LDRimage[i, j].R, (byte)LDRimage[i, j].G, (byte)LDRimage[i, j].B);
            }
        }

        bitmap.SaveAsPng(outputStream);
    }

    /// <summary>
    /// Creates a PNG file of the corresponding LDR image
    /// It accounts for the gamma correction of the display and of the empirical factor here named "factor"
    /// The luminosityFunction parameter allow to choose between some possible ways to compute the luminosity of a pixel
    /// </summary>
    /// <param name="outputFilename"></param>
    /// <param name="luminosityFunction"></param>
    /// <param name="factor"></param>
    /// <param name="gamma"></param>
    /// <param name="averageLuminosity"></param>
    /// <param name="delta"></param>
    public void WritePNG(string outputFilename, LumFunction luminosityFunction, float factor, float gamma,
        float? averageLuminosity = 0.5f,
        float delta = 1e-10f)
    {
        //using (Stream fileStream = File.OpenWrite(outputFilename))
        using(Stream fileStream = new FileStream(outputFilename, FileMode.Create))
        {
            this.WritePNG(fileStream, luminosityFunction, factor, gamma, averageLuminosity, delta);
        }
    }
}

public enum LumFunction
{
    Shirley,
    Weighted
}