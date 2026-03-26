using TracerLib;

Color[,] colorMatrix = new Color[2, 3];


for (int i = 0; i < colorMatrix.GetLength(0); i++)
{
    for (int j = 0; j < colorMatrix.GetLength(1); j++)
    {
        float red = (i * colorMatrix.GetLength(1) + j);
        float green = (i * colorMatrix.GetLength(1) + j) * 2;
        float blue = (i * colorMatrix.GetLength(1) + j) * 3;
        colorMatrix[i, j] = new Color(red, green, blue);
    }
}

Color[] colors = new Color[6];
for (int i = 0; i < 2; i++)
{
    for (int j = 0; j < 3; j++)
    {
        colors[i * 3 + j] = colorMatrix[i, j];
    }
}

HDRImage image = new HDRImage(2, 3, colors);
image.Print();
Console.WriteLine(image);

image[^1].Print();
Console.WriteLine();
image[1, 1].Print();

Color color1 = new Color(387, 129, 530);
image[0] = color1;
image.Print();

/*
string prova = "2 3";
try
{
    int w, h;
    HDRImage._parse_img_size(prova, out w, out h);

}
catch (ArgumentException err1)
{
    Console.WriteLine(err1.Message);
}
*/
//Console.WriteLine($"Prova: {w}x{h}");