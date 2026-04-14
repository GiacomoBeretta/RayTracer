namespace TracerLib;

/// <summary>
/// A Transformation is a 4x4 matrix that represents
/// scaling transformations, rotations and translations.
/// </summary>
public struct Transformation
{
    public float[] M { get; private set; }

    public float[] InvM { get; private set; }

    //Constructors - Begin
    public Transformation()
    {
        M = new float[16];
        InvM = new float[16];
        for (int i = 0; i < 16; i++)
        {
            M[i] = 0;
            InvM[i] = 0;
        }

        M[0] = M[5] = M[10] = M[15] = 1.0f;
        InvM[0] = InvM[5] = InvM[10] = InvM[15] = 1.0f;
    }

    /* constructor that receives only one matrix and computes the inverse
     public Transformation(float[] m)
     {
         this.m = new float[16];
         this.invm = new float[16];
         for (int i = 0; i < 16; i++)
         {
             this.m[i] = m[i];
         }

         invm = ComputeInverse(m);
         _CheckConsistency();
     }*/

    public Transformation(float[] m, float[] invm)
    {
        this.M = new float[16];
        this.InvM = new float[16];
        for (int i = 0; i < 16; i++)
        {
            this.M[i] = m[i];
            this.InvM[i] = invm[i];
        }

        _CheckConsistency();
    }

    /*public Transformation(Vector k)
    {
        M = new float[16];
    }*/

    //Constructors - End

    /// <summary>
    /// 1D index for the matrix m
    /// </summary>
    /// <param name="index"></param>
    public float this[Index index]
    {
        get => M[index];
        set => M[index] = value;
    }

    public float[] this[Range range]
    {
        get => M[range];
    }
    
    /// <summary>
    /// Checks that the row and col are non negative and less than 16
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void _CheckCoordinates(int row, int col)
    {
        if (col < 0 || col > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(col), col,
                nameof(col) + $" must be non-negative and less than 16");
        }

        if (row < 0 || row > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(row), row,
                nameof(row) + $" must be non-negative and less than 16");
        }
    }

    public int _MatrixOffset(int row, int col)
    {
        _CheckCoordinates(row, col);
        return row * 4 + col;
    }

    /// <summary>
    /// Returns the coefficient with coordinates (row,col)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public float this[int row, int col]
    {
        get => M[_MatrixOffset(row, col)];
        set => M[_MatrixOffset(row, col)] = value;
    }

    public bool _IsConsistent()
    { 
        float[] identity =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];

        float[] prova = Functions.Matrix4X4Product(M, InvM);
        
        return Functions.AreArrayClose(prova, identity);
    }

    public void _CheckConsistency()
    {
        if (!_IsConsistent())
        {
            throw new ArgumentException("if multiplied the matrix and its inverse do not return the identity matrix");
        }
    }
    
    public static bool AreTransformationsClose(Transformation t1, Transformation t2, float epsilon = 1e-5f)
    {
        bool areMatricesClose = true;

        areMatricesClose = areMatricesClose
                           && Functions.AreArrayClose(t1.M, t2.M)
                           && Functions.AreArrayClose(t2.InvM, t1.InvM);

        return areMatricesClose;
    }

    public override string ToString()
    {
        string str = "(";
        for (int i = 0; i < 4; i++)
        {
            str += "(";
            for (int j = 0; j < 4; j++)
            {
                str += this[i, j].ToString("F2");
                if (j < 3)
                {
                    str += ",\t";
                }
            }

            if (i < 3)
            {
                str += ")\n";
            }
            else
            {
                str += ")";
            }
        }
        str += ")\n";
        return str;
    }

    public void Print()
    {
        Console.WriteLine(ToString());
    }
    
    //come si fa?
    //non posso essere sicuro che la matrice che ottengo sia l'inversa per via delle operazioni sui float
    /// <summary>
    /// Returns the inverse of the matrix passed as argument using the Gauss elimination method
    /// We suppose that matrix is invertible.
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    /*public static float[] ComputeInverse(float[] mat)
    {
        //Initialize invMatrix as identity Matrix
        float[] invMatrix = new float[9];
        for (int i = 0; i < 16; i++)
        {
            invMatrix[i] = 0;
        }

        invMatrix[0] = invMatrix[5] = invMatrix[10] = invMatrix[15] = 1.0f;

        //We execute the same operations, that transforms mat into an identity matrix,
        //on the invMatrix that now is an identity matrix, the result is exactly
        //the inverse of mat.

        //The following operations woul





        return invMatrix;
    }*/
    public Transformation Inverse()
    {
        return new Transformation(InvM, M);
    }

    public static Transformation operator *(Transformation t1, Transformation t2)
    {
        float[] m3 = Functions.Matrix4X4Product(t1.M, t2.M);
        float[] invm3 = Functions.Matrix4X4Product(t2.M, t1.M);

        return new Transformation(m3, invm3);
    }

    /*public static Vector operator *(Transformation t, Vector v)
    {
    }

    public static Point operator *(Transformation t, Point p)
    {
    }

    public static Normal operator *(Transformation t, Normal n)
    {
    }*/
}