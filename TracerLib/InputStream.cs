// This file is release under EUPL_v1.2 license. See LICENSE.md

using System.Globalization;
using System.Text; //per il metodo cultureInfo

namespace TracerLib;

//rivedere dopo aver scritto le docstring di tutta la classe
/// <summary>
/// A class to parse the tokens in the scene text files written in ASCII.
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
                Location.line++;
                Location.column = 0;
                break;
            case '\t':
                Location.column += Tabulations;
                break;
            default:
                Location.column++;
                break;
        }
    }

    /// <summary>
    /// Reads the next char in the stream, updating the current source location accordingly.
    /// If a saved character is available, it is returned before reading from the underlying stream.
    /// Returns <c>null</c> when the end of the stream is reached (without updating the <see cref="Location"/>).
    /// </summary>
    /// <returns>The next character in the input stream, or <c>null</c> if the end of the file has been reached.</returns>
    public char? ReadChar()
    {
        char c;
        if (SavedChar == null)
        {
            int b = Stream.ReadByte();
            if (b == -1) return null; // if it has reached the end of file
            c = (char)b;
        }
        else
        {
            c = SavedChar.Value;
            SavedChar = null;
        }

        // if there was a saved char, the location was behind of this char c, so we need to update the position now.
        UpdateLocation(c);
        return c;
    }

    /// <summary>
    /// Reads the next character from the input stream.
    /// Throws a <see cref="SceneSyntaxException"/> if the end of the input
    /// is reached before a character can be read.
    /// </summary>
    /// <param name="errorLocation">
    /// The source location to associate with the generated exception.
    /// </param>
    /// <returns>
    /// The next character from the input stream.
    /// </returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when the end of the input is reached.
    /// </exception>
    public char ReadRequiredChar(SourceLocation errorLocation)
    {
        char? ch = ReadChar();
        if (ch == null)
        {
            throw new SceneSyntaxException(errorLocation, "unterminated number: reached end of file.");
        }

        return ch.Value;
    }

    /// <summary>
    /// Saves the char specified, so that it is returned at the next call of <see cref="ReadChar"/>.
    /// Update the <see cref="Location"/> at the one it was before calling <see cref="UpdateLocation"/>.
    /// </summary>
    /// <param name="c">The character to unread.</param>
    public void UnreadChar(char c)
    {
        if (SavedChar != null)
        {
            throw new InvalidOperationException("Tried to unread char, but there was already a previously saved char");
        }

        SavedChar = c;
        Location = SavedLocation;
    }

    /// <summary>
    /// Skips all remaining characters in the current line.
    /// Reading stops when a line terminator or the end of the file is reached.
    /// Supports both LF ('\n') and CRLF ('\r\n') line endings.
    /// </summary>
    public void SkipLine()
    {
        while (true)
        {
            char? ch = ReadChar();
            if (ch == null) return;
            if (ch == '\n') return;
            if (ch == '\r')
            {
                ch = ReadChar();
                if (ch == null) return;
                if (ch == '\n') return;
            }
        }
    }

    /// <summary>
    /// Skips whitespace characters (including spaces, tabs, and newline characters)
    /// and comments (that starts with '#' and extend to the end of the line) from the input stream.
    /// Stops when the next non-whitespace, non-comment character is reached,
    /// which is left available for reading.
    /// </summary>
    public void SkipWhitespacesAndComments()
    {
        while (true)
        {
            char? ch = ReadChar();

            if (ch == null) return;

            switch (ch.Value)
            {
                case '#':
                    SkipLine();
                    continue;

                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    continue;

                default:
                    // push back non-whitespace, non-comment character
                    UnreadChar(ch.Value);
                    return;
            }
        }
    }

    // Parse_token methods - Begin

    /// <summary>
    /// Parses a string literal token starting at the given source location.
    /// Characters are read verbatim until a closing quotation mark (") is encountered.
    /// The closing quotation mark is read
    /// and not included in the resulting string value.
    /// </summary>
    /// <param name="tokenLocation">The source location where the string literal starts.</param>
    /// <returns>A <see cref="StringToken"/> containing the parsed string value.</returns>
    /// <exception cref="SceneSyntaxException">Thrown if the end of the input is reached before a closing quotation mark is found.</exception>
    public StringToken _ParseStringToken(SourceLocation tokenLocation)
    {
        StringBuilder sb = new StringBuilder();

        while (true)
        {
            char? ch = ReadChar();

            if (ch == null) throw new SceneSyntaxException(tokenLocation, "unterminated string");
            if (ch.Value == '"') break;
            sb.Append(ch.Value);
        }

        return new StringToken(tokenLocation, sb.ToString());
    }

    ///  <summary>
    /// Parses a floating-point numeric literal starting with the specified character.
    /// Supports optional leading sign (+/-), decimal notation, and scientific
    /// notation using an exponent (e.g. 1.23e-4).
    /// </summary>
    /// <param name="tokenLocation">The source location where the numeric token begins,
    /// used in the constructor of <see cref="LiteralNumberToken"/>, and to throw the exceptions.
    /// </param>
    /// <param name="firstChar">The first character of the numeric literal that has already been read.
    /// This may be a digit or a leading sign (+/-).</param>
    /// <returns>A <see cref="LiteralNumberToken"/> containing the parsed floating-point value.</returns>
    /// <exception cref="SceneSyntaxException">Thrown when the numeric literal is malformed or reaches the end of the
    /// input before a valid number can be completed.</exception>
    public LiteralNumberToken _ParseFloatToken(SourceLocation tokenLocation, char firstChar)
    {
        string floatString = firstChar.ToString();
        // bool hasReadExpSign = false;
        bool hasReadExpChar = false;
        bool hasReadDot = false; // the decimal point
        float value;

        // if the first char is a sign we expect to read next a digit otherwise we throw an exception
        if (firstChar == '+' || firstChar == '-')
        {
            char ch = ReadRequiredChar(tokenLocation);

            if (!char.IsDigit(ch))
            {
                UnreadChar(ch);
                throw new SceneSyntaxException(tokenLocation,
                    "invalid number: after the sign of the number must follow a number");
            }

            floatString += ch;
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
                ch = ReadRequiredChar(tokenLocation);

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

                ch = ReadRequiredChar(tokenLocation);

                // if there is an exponent sign after it there must be a number
                if (ch == '+' || ch == '-')
                {
                    floatString += ch.Value;

                    ch = ReadRequiredChar(tokenLocation);

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

        if (!float.TryParse(floatString, CultureInfo.InvariantCulture, out value))
        {
            throw new SceneSyntaxException(tokenLocation, $"{floatString} is an invalid floating point number");
        }

        return new LiteralNumberToken(tokenLocation, value);
    }

    /// <summary>
    /// Parses an identifier or keyword token starting from the first character already read.
    /// </summary>
    /// <param name="tokenLocation">The source location where the token starts.</param>
    /// <param name="firstChar">The first character of the identifier, already read from the input.</param>
    /// <returns>
    /// A <see cref="KeywordToken"/> if the parsed lexeme matches a known keyword;
    /// otherwise an <see cref="IdentifierToken"/> containing the parsed identifier.
    /// </returns>
    /// <remarks>
    /// This method reads characters from the input stream until it encounters a character
    /// that is not a letter, digit, or underscore ('_').
    /// The first non-matching character is unread.
    /// 
    /// The initial character (<paramref name="firstChar"/>) is assumed to already be validated
    /// as a valid identifier start character.
    /// 
    /// Identifier grammar:
    /// <code>
    /// identifier := (letter | '_') (letter | digit | '_')*
    /// </code>
    /// </remarks>
    public Token _ParseKeywordIdentifierToken(SourceLocation tokenLocation, char firstChar)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(firstChar);

        while (true)
        {
            char? ch = ReadChar();

            if (ch == null) break;
            if (ch.Value != '_' && !char.IsLetterOrDigit(ch.Value))
            {
                UnreadChar(ch.Value);
                break;
            }

            sb.Append(ch.Value);
        }

        string tokenString = sb.ToString();

        if (Keywords.Map.TryGetValue(tokenString, out Keyword keyword))
        {
            return new KeywordToken(tokenLocation, keyword);
        }

        return new IdentifierToken(tokenLocation, tokenString);
    }

    // Parse_Token methods - End 

    /// <summary>
    /// Reads the next token from the input stream, skipping whitespace,
    /// newlines, and comments.
    /// </summary>
    /// <remarks>
    /// If a previously saved token is available, it is returned immediately.
    /// Otherwise, the lexer skips whitespace and comments, determines the token
    /// type based on the next character, and parses the corresponding token.
    /// Supported token categories include symbols, string literals, numeric values,
    /// identifiers, and keywords. If the end of the input is reached, a
    /// <see cref="StopToken"/> is returned.
    /// </remarks>
    /// <returns>The next token in the stream.</returns>
    /// <exception cref="SceneSyntaxException">
    /// Thrown when an invalid character is encountered.
    /// </exception>
    public Token ReadNextToken()
    {
        if (SavedToken != null)
        {
            Token result = SavedToken;
            SavedToken = null;
            return result;
        }

        SkipWhitespacesAndComments();

        // save the token location as the position before reading the first char of the token
        SourceLocation tokenLocation = Location;

        char? ch = ReadChar();
        if (ch == null)
        {
            return new StopToken(tokenLocation);
        }

        switch (ch.Value)
        {
            case '(':
            case ')':
            case '[': // '[]' for the vectors and points,
            case ']':
            case '<': // '<>' are for the colors,
            case '>':
            case ',':
            case '*': // '*' for composing transformations
                //invertire location e value per conformare agli altri token o viceversa invertire gli altri
                return new SymbolToken(tokenLocation, ch.Value.ToString());
            case '\"':
                return _ParseStringToken(tokenLocation);
        }

        if (char.IsDigit(ch.Value) || ch.Value == '+' || ch.Value == '-')
        {
            return _ParseFloatToken(tokenLocation, ch.Value);
        }

        if (char.IsLetter(ch.Value) || ch.Value == '_')
        {
            return _ParseKeywordIdentifierToken(tokenLocation, ch.Value);
        }

        throw new SceneSyntaxException(tokenLocation, $"invalid character '{ch}'");
    }

    /// <summary>
    /// Saves a token to be returned by the next call to <see cref="ReadNextToken"/>.
    /// Only one unread token can be saved at a time.
    /// </summary>
    /// <exception cref="SceneSyntaxException">
    /// Thrown if a token is already saved.
    /// </exception>
    public void UnreadToken(Token token)
    {
        if (SavedToken != null)
        {
            throw new SceneSyntaxException(token.Location,
                $"Tried to unread the token {token}, but there was already a saved token {SavedToken}.");
        }

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