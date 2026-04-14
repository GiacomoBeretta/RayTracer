using TracerLib;
using Xunit.Abstractions;

//namespace TracerTests;

public class TransformationTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TransformationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestConstructor()
    {
        Transformation t = new Transformation();
        float[] identity =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
        Assert.Equal(identity, t.M);
        Assert.Equal(identity, t.InvM);
    }

    [Fact]
    public void TestConstructorWithInverse()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];

        Transformation t = new Transformation(m, invM);
        Assert.Equal(m, t.M);
        Assert.Equal(invM, t.InvM);
    }

    [Fact]
    public void Test1DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(6.0, t.M[5]);
        Assert.Equal(4.0, t.M[14]);
    }

    [Fact]
    public void TestCheckCoordinates()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(16, 7));
        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(3, 16));
        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(19, 20));
        Assert.Throws<ArgumentOutOfRangeException>(() => t._CheckCoordinates(-17, -40));
    }

    [Fact]
    public void TestMatrixOffset()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(9, t._MatrixOffset(2, 1));
        Assert.Equal(10, t._MatrixOffset(2, 2));
        Assert.Equal(15, t._MatrixOffset(3, 3));
    }

    [Fact]
    public void Test2DIndex()
    {
        float[] mat =
        [
            1.0f, 2.0f, 3.0f, 4.0f,
            5.0f, 6.0f, 7.0f, 8.0f,
            9.0f, 9.0f, 8.0f, 7.0f,
            6.0f, 5.0f, 4.0f, 1.0f,
        ];

        float[] invMat =
        [
            -3.75f, 2.75f, -1, 0,
            4.375f, -3.875f, 2.0f, -0.5f,
            0.5f, 0.5f, -1.0f, 1.0f,
            -1.375f, 0.875f, 0.0f, -0.5f
        ];
        Transformation t = new Transformation(mat, invMat);

        Assert.Equal(1.0f, t[0, 0]);
        Assert.Equal(7.0f, t[2, 3]);
        Assert.Equal(4.0f, t[3, 2]);
    }

    [Fact]
    public void TestIsConsistent()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];

        Transformation t1 = new Transformation(m, invM);
        Assert.True(t1._IsConsistent());
    }

    [Fact]
    public void TestCheckConsistency()
    {
        float[] m =
        [
            1,2,3,4,
            5,6,7,8,
            9,10,11,12,
            13,14,15,16
        ];

        float[] invM =
        [
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0
        ];

        Assert.Throws<ArgumentException>(()=>new Transformation(m, invM));
    }

    [Fact]
    public void TestAreTransformationsClose()
    {
        
    }
    
    [Fact]
    public void TestToString()
    {
        float[] m =
        [
            3, -23.25f, 9, 4,
            4, 6, 2, 7,
            -2, -10, -2, 12,
            1.4f, 17, 0, 3.589f
        ];

        float[] invM =
        [
            -0.112625f, 0.410949f, -0.0958651f, -0.355464f,
            0.00648398f, -0.0342974f, -0.00511954f, 0.0767848f,
            0.159527f, -0.226542f, -0.00866817f, 0.293034f,
            0.0132203f, 0.00215331f, 0.0616448f, 0.0535824f
        ];
        Transformation t = new Transformation(m, invM);

        string str = "((3.00,\t-23.25,\t9.00,\t4.00)\n"
                     + "(4.00,\t6.00,\t2.00,\t7.00)\n"
                     + "(-2.00,\t-10.00,\t-2.00,\t12.00)\n"
                     + "(1.40,\t17.00,\t0.00,\t3.59))\n";
        Assert.Equal(str, t.ToString());
    }
}