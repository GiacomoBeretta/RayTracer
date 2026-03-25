﻿using Colors;
 using System.Diagnostics;
 using System.Text;
 using Exception;

namespace Hdr;

public class HdrImage
{
    
    //Costruttore immagine Hdr
    public HdrImage(int width, int height)
    {
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];
    }
    
    //Costruttore immagine Hdr a partire da una stream
    public HdrImage(Stream stream)
    {
        // read_pfm_file(stream);
    }

    //Costruttore immagine Hdr a partire da un file
    public HdrImage(string fileName)
    {
        using (Stream filestream = File.OpenRead(fileName))
        {
           // read_pfm_file(filestream);
        }
    }
    
    //Variabili immagini Hdr
    public int Width { get; set; }
    public int Height { get; set; }
    public Color[]? Pixels { get; set; } //Controllare nullable

    //Checker per la validità delle coordinate
    public bool _Valid_coord( int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public int pixel_offset(int x, int y)
    {
        Debug.Assert(_Valid_coord(x,y));
        return y * Width + x;
    }

    public Color Get_pixel(int x, int y)
    {
        Debug.Assert(_Valid_coord(x,y));
        return Pixels[pixel_offset(x,y)];
    }

    public void Set_pixel(int x, int y, Color a)
    {
        Debug.Assert(_Valid_coord(x,y));
        Pixels[pixel_offset(x, y)] = a;
    }
    
    public float ReadFloat(Stream stream, bool bigEndian = true){
        
        var value = new byte[4];
        var totalRead=0;

        while (totalRead <4)
        {
            var bytesRead = stream.Read(value, totalRead, 4-totalRead);

            if (bytesRead == 0)
            {
                throw new InvalidPfmFileFormat("Impossibile leggere i file binari dai dati");
            }

            totalRead += bytesRead;

        }

        if (BitConverter.IsLittleEndian != bigEndian){
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
     
}