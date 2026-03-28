using Xunit;
using TracerLib;

/*
  *using Colors;
   using Hdr;
 */
namespace TracerTests;

// namespace Hdr.Test;
public class HDRTest
{
    [Fact]
    public void TestAreCoordinatesValid()
    {
        HDRImage image1 = new HDRImage(4, 10);
        Assert.True(image1._AreCoordinatesValid(0, 0));
        Assert.True(image1._AreCoordinatesValid(3, 2));
        Assert.True(image1._AreCoordinatesValid(3, 9));
        Assert.False(image1._AreCoordinatesValid(4, 1));
        Assert.False(image1._AreCoordinatesValid(0, 10));
        Assert.False(image1._AreCoordinatesValid(-1, 2));
        Assert.False(image1._AreCoordinatesValid(2, -1));

        var image2 = new HDRImage(100, 100);
        Assert.True(image2._Valid_coord(53, 68));
        Assert.False(image2._Valid_coord(200, 39));
    }

    [Fact]
    public void TestIndex()
    {
        int width = 10;
        int height = 4;
        HDRImage image = new HDRImage(width, height);

        Color[] colors = new Color[width * height];
        float red, green, blue;
        int offset = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                offset = image.Offset(i, j);
                red = offset;
                green = offset * 2;
                blue = offset * 3;
                colors[offset] = new Color(red, green, blue);
            }
        }

        image.Pixels = colors;
        Assert.Equal(new Color(4, 8, 12), image[4]);
    }

    [Fact]
    public void TestOffset()
    {
        HDRImage image = new HDRImage(5, 13);

        Assert.Equal(0, image.Offset(0, 0));
        Assert.Equal(2, image.Offset(2, 0));
        Assert.Equal(10, image.Offset(0, 2));
        Assert.Equal(29, image.Offset(9, 4));

        var image2 = new HDImage(100, 100);
        Assert.Equal(203, image2.pixel_offset(3, 2));
    }

    //da rivedere dopo aver visto meglio le lambda functions
    [Fact]
    public void TestParseImgSize()
    {
        int width, height;
        string imgSize;

        imgSize = "1 3 8";
        Assert.Throws<ArgumentException>(() => HDRImage._parse_img_size(imgSize, out width, out height));

        imgSize = "a  b";
        Assert.Throws<ArgumentException>(() => HDRImage._parse_img_size(imgSize, out width, out height));

        imgSize = "-1 23";
        Assert.Throws<ArgumentException>(() => HDRImage._parse_img_size(imgSize, out width, out height));

        imgSize = "5 -8";
        Assert.Throws<ArgumentException>(() => HDRImage._parse_img_size(imgSize, out width, out height));

        imgSize = "-5 -8";
        Assert.Throws<ArgumentException>(() => HDRImage._parse_img_size(imgSize, out width, out height));

        imgSize = "41 78";
        HDRImage._parse_img_size(imgSize, out width, out height);
        Assert.Equal(41, width);
        Assert.Equal(78, height);
    }

/*
    imgSize = "1 2 3";
    try
    {
        HDRImage._parse_img_size(imgSize, out width, out height);
    }
    catch

(ArgumentException)
{
}

imgSize = "-1 2";
try
{
    HDRImage._parse_img_size(imgSize, out width, out height);
}
catch (ArgumentException)
{
}
*/
    [Fact]
    public void TestParseEndianness()
    {
        string endianness;

        endianness = "1 .0";
        Assert.Throws<FormatException>(() => HDRImage.Parse_endianness(endianness));
        endianness = "zws";
        Assert.Throws<FormatException>(() => HDRImage.Parse_endianness(endianness));
        endianness = "2.0";
        Assert.Throws<ArgumentException>(() => HDRImage.Parse_endianness(endianness));
        endianness = "0";
        Assert.Throws<ArgumentException>(() => HDRImage.Parse_endianness(endianness));
        endianness = "-2.0";
        Assert.Throws<ArgumentException>(() => HDRImage.Parse_endianness(endianness));

        endianness = "1.0";
        Assert.Equal(1, HDRImage.Parse_endianness(endianness));
        endianness = "-1.0";
        Assert.Equal(-1, HDRImage.Parse_endianness(endianness));
    }


    /* [Fact]
     public void TestAverageLuminosityShirleyMorley()
     {
         HDRImage image1 = new HDRImage(1, 2);
         image1[0] = new Color(5, 10, 15); //Luminosity = 10.0
         image1[1] = new Color(500, 1000, 1500); //Luminosity = 1000.0

         Assert.Equal(100.0, image1.AverageLuminosity(delta:0.0f));

         HDRImage image2 = new HDRImage(1, 3);
         image2[0] = new Color(1, 0, 2); //Luminosity = 1
         image2[1] = new Color(1550000, 1300000, 1700000); //Luminosity = 1000000
         image2[2] = new Color(0, 0, 0); // Luminosity
         //Assert.Equal(10, image2.AverageLuminosity(delta:1e-3f));
     }*/
}