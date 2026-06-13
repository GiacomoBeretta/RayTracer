// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Diagnostics;
using System.Globalization; //per il metodo cultureInfo

namespace TracerLib;

/// <summary>
/// A class to parse the tokens in the scene text files.
/// </summary>
public class InputStream : IDisposable
{
    /// <summary>
    /// Underlying input stream used for sequential tokenization of scene files.
    /// </summary>
    public Stream Stream;
    
    /// <summary>
    /// Current read position (row and column) in the source file, used for error reporting and token tracking.
    /// </summary>
    public SourceLocation Location;
    
    /// <summary>
    /// Previously saved <see cref="SourceLocation"/> used to restore the reader state when unreading a character or token.
    /// </summary>
    public SourceLocation SavedLocation;
    
    /// <summary>
    /// Cached character used to support one-character lookahead (unread functionality).
    /// If set, it will be returned on the next read operation before consuming the stream.
    /// </summary>
    public char? SavedChar;
    
    /// <summary>
    /// Number of spaces used to correctly update the read position during parsing.
    /// </summary>
    public readonly int Tabulations;
    
    /// <summary>
    /// Cached token used to support token-level lookahead (unread functionality).
    /// If set, it will be returned on the next token read operation.
    /// </summary>
    public Token? SavedToken;
    
    /// <summary>
    /// Constructs an <see cref="InputStream"/> instance that reads from the specified file path.
    /// </summary>
    /// <param name="filePath">Path of the scene file to read.</param>
    /// <param name="tabulations">Number of spaces used to expand tab characters during parsing.
    /// If not specified is 8.</param>
    public InputStream(string filePath, int tabulations = 8)
    {
        Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Location = new SourceLocation(filePath);
        SavedLocation = Location;
        SavedChar = null;
        Tabulations = tabulations;
        SavedToken = null;
    }

    public void Dispose()
    {
        Stream.Dispose();
    }

    /// <summary>
    /// Advances the current source location based on the specified character,
    /// updating line and column counters.
    /// The previous location is stored in <see cref="SavedLocation"/> to support the unread functionality.
    /// Handles line breaks, tab expansion, and standard character advancement.
    /// </summary>
    /// <param name="c">
    /// Consumed character used to update the current position
    /// </param>
    public void UpdateLocation(char c)
    {
        SavedLocation = Location;
        switch (c)
        {
            case '\n':
                Location.line += 1;
                Location.column = 0;
                break;
            case '\t':
                Location.column += Tabulations;
                break;
            default:
                Location.column += 1;
                break;
        }
    }

    /// <summary>
    /// Returns null if it has been reached the end of file
    /// </summary>
    /// <returns></returns>
    public char? ReadChar()
    {
        char c;
        if (SavedChar == null)
        {
            int b = Stream.ReadByte();
            if (b == -1) return null; // if it has reached the end of file
            else c = (char)b;
        }
        else
        {
            c = SavedChar.Value;
            SavedChar = null;
        }

        UpdateLocation(c);
        return c;
    }

    public void UnreadChar(char c)
    {
        {
            SavedChar = c;
            Location = SavedLocation;
        }
    }

    /// <summary>
    /// Reads until a '\n' or '\r\n' is encountered or the end of file is reached.
    /// </summary>
    /// <exception cref="SceneSyntaxException"></exception>
    public void SkipLine()
    {
        while (true)
        {
            char? ch = ReadChar();
            if (ch == null) return;
            if (ch == '\n') break;
            if (ch == '\r')
            {
                ch = ReadChar();
                if (ch == '\n') break;
            }
        }
    }

    /// <summary>
    /// Reads all the whitespaces new line escape sequences and comments starting with the hash character (#).
    /// </summary>
    public void SkipWhitespacesAndComments()
    {
        const string whitespace = " \t \n \r";

        char? ch = ReadChar();

        // if reaches the end of file return to the precedent function
        if (ch == null) return;

        while (whitespace.Contains(ch.Value) || ch == '#')
        {
            if (ch == '#') SkipLine();
            ch = ReadChar();
            if (ch == null) return;
        }

        // if the character read is useful then save it to read it the next time you use readChar
        UnreadChar(ch.Value);
    }

    // Parse_token methods - Begin

    public StringToken _ParseStringToken(SourceLocation tokenLocation)
    {
        string stringToken = "";

        while (true)
        {
            char? ch = ReadChar();

            if (ch == null) throw new SceneSyntaxException(tokenLocation, "unterminated string");
            if (ch.Value == '\"') break;

            stringToken += ch;
        }

        return new StringToken(tokenLocation, stringToken);
    }

    //prova con i caratteri exp ecc. 
    /// <summary>
    /// tokenLocation serve perché leggendo mano a mano i caratteri la location si aggiorna e va avanti
    /// </summary>
    /// <param name="firstChar"></param>
    /// <param name="tokenLocation"></param>
    /// <returns></returns>
    /// <exception cref="SceneSyntaxException"></exception>
    public LiteralNumberToken _ParseFloatToken(char firstChar, SourceLocation tokenLocation)
    {
        string floatString = firstChar.ToString();
        //const string expChar = "eE";
        //const string signs = "+-"; // signs for exponents (the sign of the value is already read in the first char)
        bool hasReadExpSign = false;
        bool hasReadExpChar = false;
        bool hasReadDot = false; // the decimal point
        float value;

        // if the first char is a sign we expect to read next a digit otherwise we throw an exception
        if (firstChar == '+' || firstChar == '-')
        {
            char? ch = ReadChar();
            if (ch == null)
            {
                throw new SceneSyntaxException(tokenLocation, "unterminated number: reached end of file.");
            }

            if (!char.IsDigit(ch.Value))
            {
                UnreadChar(ch.Value);
                throw new SceneSyntaxException(tokenLocation,
                    "invalid number: after the sign of the number must follow a number");
            }

            floatString += ch.Value;
        }

        while (true)
        {
            char? ch = ReadChar();
            if (ch == null) break;
            //  if a dot is encountered then the next char must be a digit otherwise we throw an exception
            // Furthermore if the dot is read after we have already read the exponent part we throw an exception
            if (ch.Value == '.')
            {
                if (hasReadExpChar)
                {
                    UnreadChar(ch.Value);
                    throw new SceneSyntaxException(tokenLocation, "invalid number: dot following the exponent part.");
                }

                if (hasReadDot)
                {
                    UnreadChar(ch.Value);
                    throw new SceneSyntaxException(tokenLocation, "invalid number: read two dots.");
                }

                hasReadDot = true;
                floatString += ch.Value;

                // Read the char after the dot: it must be a number
                ch = ReadChar();
                if (ch == null)
                {
                    throw new SceneSyntaxException(tokenLocation, "unterminated number: reached end of file.");
                }

                if (!char.IsDigit(ch.Value))
                {
                    UnreadChar(ch.Value);
                    throw new SceneSyntaxException(tokenLocation, "invalid number");
                }

                floatString += ch.Value;
            }

            // if we read an exponent char then the next char must be:
            // a number
            // or
            // a sign and a following number 
            else if (ch == 'e' || ch == 'E')
            {
                if (hasReadExpChar)
                {
                    UnreadChar(ch.Value);
                    throw new SceneSyntaxException(tokenLocation,
                        "invalid number: have been read two exponent letters (e or E).");
                }

                hasReadExpChar = true;
                floatString += ch.Value;

                ch = ReadChar();
                if (ch == null)
                {
                    throw new SceneSyntaxException(tokenLocation, "unterminated number: reached end of file.");
                }

                if (ch == '+' || ch == '-')
                {
                    if (hasReadExpSign)
                    {
                        UnreadChar(ch.Value);
                        throw new SceneSyntaxException(tokenLocation,
                            "invalid number: have been read two exponent signs.");
                    }

                    hasReadExpSign = true;
                    floatString += ch;

                    ch = ReadChar();
                    if (ch == null)
                    {
                        throw new SceneSyntaxException(tokenLocation, "unterminated number: reached end of file.");
                    }

                    // after the exponent sign must follow a number
                    if (!char.IsDigit(ch.Value))
                    {
                        UnreadChar(ch.Value);
                        throw new SceneSyntaxException(tokenLocation,
                            "invalid number: after the e/E and the exponent sign must follow a number");
                    }

                    floatString += ch.Value;
                }
                //if there is no exponent sign there must be a number
                else if (Char.IsDigit(ch.Value))
                {
                    floatString += ch.Value;
                }
                else
                {
                    UnreadChar(ch.Value);
                    throw new SceneSyntaxException(tokenLocation,
                        "invalid number: after the e/E must follow a sign or a number");
                }
            }
            else if (char.IsDigit(ch.Value))
            {
                floatString += ch.Value;
            }
            else // if ch is not a '.', 'e', 'E', and neither a digit
            {
                UnreadChar(ch.Value);
                break;
            }
        }

        try
        {
            value = float.Parse(floatString, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            throw new SceneSyntaxException(tokenLocation, $"{floatString} is an invalid floating point number");
        }

        return new LiteralNumberToken(tokenLocation, value: value);
    }

    // vecchio parseFloatToken
    /*public LiteralNumberToken _ParseFloatToken(char firstChar, SourceLocation tokenLocation)
    {
        string floatString = firstChar.ToString();
        const string expChar = "eE";
        const string signs = "+-"; // signs for exponents (the sign of the value is already read in the first char)

        float value;

        while (true)
        {
            char? ch = ReadChar();
            // if it has been reached the end of file
            if (ch == null) break;

            if (!Char.IsDigit(ch.Value) && ch.Value != '.' &&
                !expChar.Contains(ch.Value) && !signs.Contains(ch.Value))
            {
                UnreadChar(ch.Value);
                break;
            }
            floatString += ch;
        }

        try
        {
            value = float.Parse(floatString, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            throw new SceneSyntaxException(tokenLocation, $"{floatString} is an invalid floating point number");
        }

        return new LiteralNumberToken(tokenLocation, value: value);
    }*/

    public Token _ParseKeywordIdentifierToken(char firstChar, SourceLocation tokenLocation)
    {
        string tokenString = firstChar.ToString();

        while (true)
        {
            char? ch = ReadChar();

            if (ch == null) break;
            if (!char.IsLetterOrDigit(ch.Value) && ch.Value != '_')
            {
                UnreadChar(ch.Value);
                break;
            }

            tokenString += ch;
        }

        if (Keywords.Map.TryGetValue(tokenString, out Keyword keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        return new IdentifierToken(tokenLocation, tokenString);
    }

    // Parse_Token methods - End 

    /// <summary>
    /// Reads and returns the next Token that appears in the stream (skipping whitespaces, new lines and comments)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SceneSyntaxException"></exception>
    public Token ReadNextToken()
    {
        // '<>' are for the colors, '[]' for the vectors and points, ',' for separating numbers,
        // '*' for composing transformations
        const string symbols = "()<>[],*";
        // const string op = "+-."; // non so a cosa serve il punto e per ora
        // lo commento così quando salta fuori ce ne accorgiamo subito
        const string signs = "+-";

        if (SavedToken != null)
        {
            Token result = SavedToken;
            SavedToken = null;
            return result;
        }

        SkipWhitespacesAndComments();
        char? ch = ReadChar();
        if (ch == null)
        {
            return new StopToken(Location);
        }

        //SourceLocation tokenLocation = Location;

        if (symbols.Contains(ch.Value))
        {
            //invertire location e value per conformare agli altri token o viceversa invertire gli altri
            return new SymbolToken(Location, ch.Value.ToString());
        }
        else if (ch == '\"')
        {
            return _ParseStringToken(Location);
        }
        else if (char.IsDigit(ch.Value) || signs.Contains(ch.Value))
        {
            return _ParseFloatToken(ch.Value, Location);
        }
        else if (char.IsLetter(ch.Value) || ch.Value == '_')
        {
            return _ParseKeywordIdentifierToken(ch.Value, Location);
        }
        else
        {
            throw new SceneSyntaxException(Location, $"invalid character {ch}");
        }
    }

    public void UnreadToken(Token token)
    {
        Debug.Assert(SavedToken == null);
        SavedToken = token;
    }
}


/*

//prova con stream.Position e senza unreadChar
public class InputStream
{
    private Stream _stream;
    private SourceLocation _location;

   //public InputStream(Stream stream)
    {
        _stream = stream;
        _location = new SourceLocation();
    }

   // public InputStream(Stream stream, SourceLocation location)
    {
        _stream = stream;
        _location = location;
    }

   public InputStream(string fileName)
   {
       _stream = new  FileStream(fileName, FileMode.Open, FileAccess.Read);
       _location = new SourceLocation(fileName, 0,0);
   }

   public char ReadChar()
   {
       if (_savedChar == null)
       {
           return (char)_stream.ReadByte();
       }
       else
       {
           char c = _savedChar.Value;
           _savedChar = null;
           return c;
       }
   }
}*/