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
        M =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
        InvM =
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        ];
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

    public Transformation(Vector k)
    {
        M =
        [
            1, 0, 0, k.X,
            0, 1, 0, k.Y,
            0, 0, 1, k.Z,
            0, 0, 0, 1
        ];

        InvM =
        [
            1, 0, 0, -k.X,
            0, 1, 0, -k.Y,
            0, 0, 1, -k.Z,
            0, 0, 0, 1
        ];
    }

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
        return Functions.AreArrayClose(Functions.Matrix4X4Product(M, InvM), identity, 1e-4f);
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
                           && Functions.AreArrayClose(t1.M, t2.M, epsilon)
                           && Functions.AreArrayClose(t1.InvM, t2.InvM, epsilon);

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
        float[] invm3 = Functions.Matrix4X4Product(t2.InvM, t1.InvM);

        return new Transformation(m3, invm3);
    }

    public static Vector operator *(Transformation t, Vector v)
    {
        Vector v2 = new Vector
        (
            t[0] * v.X + t[1] * v.Y + t[2] * v.Z,
            t[4] * v.X + t[5] * v.Y + t[6] * v.Z,
            t[8] * v.X + t[9] * v.Y + t[10] * v.Z
        );
        return v2;
    }

   /* public static Point operator *(Transformation t, Point p1)
    {
        Point p2 = new Point
        (
            t[0]*p1.X
        
        );
        /*
         row0, row1, row2, row3 = self.m
           newp = Point(x=p.x * row0[0] + p.y * row0[1] + p.z * row0[2] + row0[3],
                        y=p.x * row1[0] + p.y * row1[1] + p.z * row1[2] + row1[3],
                        z=p.x * row2[0] + p.y * row2[1] + p.z * row2[2] + row2[3])
           w = p.x * row3[0] + p.y * row3[1] + p.z * row3[2] + row3[3]

           if w == 1.0:
               return newp   # Avoid three (potentially costly) divisions when w = 1
           else:
               return Point(newp.x / w, newp.y / w, newp.z / w)
        
    }*/

    /*public static Normal operator *(Transformation t, Normal n)
    {
    }*/
}