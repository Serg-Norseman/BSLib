/**
 *	Author: Andrew Deren
 *	Date: July, 2004
 *	http://www.adersoftware.com
 * 
 *	StringTokenizer class. You can use this class in any way you want
 *  as long as this header remains in this file.
 */

using System;
using System.Globalization;

namespace BSLib
{
    public enum TokenKind
    {
        Unknown,
        Word,
        Ident,
        Number,
        HexNumber,
        BinNumber,
        QuotedString,
        WhiteSpace,
        Symbol,
        EOL,
        EOF
    }

    public sealed class Token
    {
        public readonly TokenKind Kind;
        public readonly int Line;
        public readonly int Column;

        public readonly string Value;
        public readonly object ValObj;

        public Token(TokenKind kind, int line, int column, string value, object valObj = null)
        {
            Kind = kind;
            Line = line;
            Column = column;
            Value = value;
            ValObj = valObj;
        }
    }

    /// <summary>
    /// StringTokenizer tokenized string into tokens.
    /// </summary>
    public class StringTokenizer
    {
        public static readonly char[] StdSymbols = new char[] {'=', '+', '-', '/', ',', '.', '*', '~', '!', '@', '#', '$', '%', '^', '&', '(', ')', '{', '}', '[', ']', ':', ';', '<', '>', '?', '|', '\\'};

        private static readonly NumberFormatInfo NumberFormat = ConvertHelper.CreateDefaultNumberFormat();

        private const char EOF = (char)0;

        private int fColumn;
        private Token fCurrentToken;
        private char[] fData;
        private bool fIgnoreWhiteSpace;
        private int fLine;
        private int fPos;
        private bool fRecognizeDecimals;
        private bool fRecognizeHex;
        private bool fRecognizeBin;
        private bool fRecognizeIdents;
        private int fSaveCol;
        private int fSaveLine;
        private int fSavePos;
        private char[] fSymbolChars;

        #region Properties

        public Token CurrentToken
        {
            get { return fCurrentToken; }
        }

        /// <summary>
        /// If set to true, white space characters will be ignored,
        /// but EOL and whitespace inside of string will still be tokenized
        /// </summary>
        public bool IgnoreWhiteSpace
        {
            get { return fIgnoreWhiteSpace; }
            set { fIgnoreWhiteSpace = value; }
        }

        public bool RecognizeDecimals
        {
            get { return fRecognizeDecimals; }
            set { fRecognizeDecimals = value; }
        }

        public bool RecognizeHex
        {
            get { return fRecognizeHex; }
            set { fRecognizeHex = value; }
        }

        public bool RecognizeBin
        {
            get { return fRecognizeBin; }
            set { fRecognizeBin = value; }
        }

        public bool RecognizeIdents
        {
            get { return fRecognizeIdents; }
            set { fRecognizeIdents = value; }
        }

        /// <summary>
        /// Gets or sets which characters are part of TokenKind.Symbol
        /// </summary>
        public char[] SymbolChars
        {
            get { return fSymbolChars; }
            set { fSymbolChars = value; }
        }

        public int Position
        {
            get { return fPos; }
        }

        #endregion

        #region Private methods

        private void SetDefaults()
        {
            fIgnoreWhiteSpace = false;
            fRecognizeDecimals = false;
            fSymbolChars = StdSymbols;
        }

        /// <summary>
        /// save read point positions so that CreateToken can use those
        /// </summary>
        private void StartRead()
        {
            fSaveCol = fColumn;
            fSaveLine = fLine;
            fSavePos = fPos;
        }

        private char LookAhead(int count)
        {
            int newPos = fPos + count;
            return (newPos >= fData.Length) ? EOF : fData[newPos];
        }

        private void Consume()
        {
            fPos++;
            fColumn++;
        }

        private Token CreateToken(TokenKind kind, string value, object valObj)
        {
            fCurrentToken = new Token(kind, fSaveLine, fSaveCol, value, valObj);
            return fCurrentToken;
        }

        private Token CreateToken(TokenKind kind)
        {
            string tokenData = new string(fData, fSavePos, fPos - fSavePos);
            fCurrentToken = new Token(kind, fSaveLine, fSaveCol, tokenData);
            return fCurrentToken;
        }

        /// <summary>
        /// reads all whitespace characters (does not include newline)
        /// </summary>
        /// <returns></returns>
        private Token ReadWhitespace()
        {
            StartRead();

            Consume(); // consume the looked-ahead whitespace char

            while (true)
            {
                char ch = LookAhead(0);
                if (ch == '\t' || ch == ' ')
                    Consume();
                else
                    break;
            }

            return CreateToken(TokenKind.WhiteSpace);
        }

        /// <summary>
        /// reads number. Number is: DIGIT+ ("." DIGIT*)?
        /// </summary>
        /// <returns></returns>
        private Token ReadNumber()
        {
            StartRead();

            TokenKind kind = TokenKind.Number;
            bool hadDot = false;
            bool hadHex = false;
            bool hadBin = false;

            Consume(); // read first digit
            while (true)
            {
                char ch = LookAhead(0);

                if (char.IsDigit(ch)) {
                    Consume();
                } else if (((ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f')) && fRecognizeHex && hadHex) {
                    Consume();
                } else if (ch == '.' && fRecognizeDecimals && !hadDot) {
                    hadDot = true;
                    Consume();
                } else if ((ch == 'e' || ch == 'E' || ch == '+' || ch == '-') && fRecognizeDecimals && hadDot) {
                    Consume();
                } else if (ch == 'x' && fRecognizeHex && !hadHex) {
                    hadHex = true;
                    kind = TokenKind.HexNumber;
                    Consume();
                } else if (ch == 'b' && fRecognizeBin && !hadBin) {
                    hadBin = true;
                    kind = TokenKind.BinNumber;
                    Consume();
                }
                else break;
            }

            string tokVal = new string(fData, fSavePos, fPos - fSavePos);
            object val = null;

            switch (kind) {
                case TokenKind.Number:
                    if (hadDot) {
                        val = Convert.ToDouble(tokVal, NumberFormat);
                    } else {
                        val = ConvertIntNumber(fSavePos, fPos, 10);
                    }
                    break;

                case TokenKind.HexNumber:
                    val = ConvertIntNumber(fSavePos + 2, fPos, 16);
                    break;

                case TokenKind.BinNumber:
                    val = ConvertIntNumber(fSavePos + 2, fPos, 2);
                    break;
            }

            return CreateToken(kind, tokVal, val);
        }

        /// <summary>
        /// reads word. Word contains any alpha character or _
        /// </summary>
        private Token ReadWord()
        {
            StartRead();

            TokenKind kind = TokenKind.Word;

            Consume(); // consume first character of the word
            while (true)
            {
                char ch = LookAhead(0);
                if (char.IsLetter(ch) || ch == '_') {
                    Consume();
                } else if (char.IsDigit(ch) && fRecognizeIdents) {
                    kind = TokenKind.Ident;
                    Consume();
                } else
                    break;
            }

            return CreateToken(kind);
        }

        /// <summary>
        /// reads all characters until next " is found.
        /// If "" (2 quotes) are found, then they are consumed as
        /// part of the string
        /// </summary>
        /// <returns></returns>
        private Token ReadString()
        {
            StartRead();

            Consume(); // read "

            while (true)
            {
                char ch = LookAhead(0);
                if (ch == EOF) break;
                
                if (ch == '\r')	// handle CR in strings
                {
                    Consume();
                    if (LookAhead(0) == '\n')	// for DOS & windows
                        Consume();

                    fLine++;
                    fColumn = 1;
                }
                else if (ch == '\n')	// new line in quoted string
                {
                    Consume();

                    fLine++;
                    fColumn = 1;
                }
                else if (ch == '"')
                {
                    Consume();
                    if (LookAhead(0) != '"') break; // done reading, and this quotes does not have escape character
                    
                    Consume(); // consume second ", because first was just an escape
                }
                else
                    Consume();
            }

            return CreateToken(TokenKind.QuotedString);
        }

        /// <summary>
        /// checks whether c is a symbol character.
        /// </summary>
        /*private bool IsSymbol(char c)
        {
            for (int i = 0; i < fSymbolChars.Length; i++)
                if (fSymbolChars[i] == c)
                    return true;

            return false;
        }*/

        private int ConvertIntNumber(int first, int last, byte numBase)
        {
            /*if (last - first <= 0) {
                throw new ArgumentException();
            }*/
            int fvalue = 0;
            //try {
                while (first < last) {
                    char ch = fData[first];
                    byte c = (byte)((int)ch - 48);
                    if (c > 9) {
                        c -= 7;

                        if (c > 15) {
                            c -= 32;
                        }
                    }

                    if (c >= numBase) {
                        throw new OverflowException();
                    }

                    fvalue = (fvalue * numBase + c);
                    first++;
                }
            //} catch (OverflowException) {
                // KBR Parser blows up when trying to parse a large number
            //}
            return fvalue;
        }

        #endregion

        public StringTokenizer()
        {
            SetDefaults();
        }

        public StringTokenizer(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            SetDefaults();
            Reset(data.ToCharArray());
        }

        public StringTokenizer(char[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            SetDefaults();
            Reset(data);
        }

        public void Reset(char[] data)
        {
            fData = data;
            fCurrentToken = null;
            fLine = 1;
            fColumn = 1;
            fPos = 0;
        }

        public string GetRest()
        {
            int len = fData.Length;
            string result = (fPos >= len) ? string.Empty : new string(fData, fPos, len - fPos);
            return result;
        }

        public Token Next()
        {
            while (true) {
                char ch = LookAhead(0);

                if (char.IsLetter(ch) || ch == '_')
                    return ReadWord();

                if (char.IsDigit(ch))
                    return ReadNumber();

                if (ch == ' ' || ch == '\t') {
                    if (!fIgnoreWhiteSpace) return ReadWhitespace();
                    Consume();
                    continue;
                }

                switch (ch)
                {
                    case EOF:
                        return CreateToken(TokenKind.EOF);

                    case '\r':
                        StartRead();
                        Consume();
                        // on DOS/Windows we have \r\n for new line
                        if (LookAhead(0) == '\n') Consume();	
                        fLine++;
                        fColumn = 1;
                        return CreateToken(TokenKind.EOL);

                    case '\n':
                        StartRead();
                        Consume();
                        fLine++;
                        fColumn = 1;
                        return CreateToken(TokenKind.EOL);

                    case '"':
                        return ReadString();

                    default:
                        //if (IsSymbol(ch)) {
                            StartRead();
                            Consume();
                            return CreateToken(TokenKind.Symbol);
                        /*} else {
                            StartRead();
                            Consume();
                            return CreateToken(TokenKind.Unknown);
                        }*/
                }
            }
        }

        public bool RequireToken(TokenKind tokenKind)
        {
            return (fCurrentToken != null && fCurrentToken.Kind == tokenKind);
        }

        public void RequestSymbol(char symbol)
        {
            if (fCurrentToken == null || fCurrentToken.Kind != TokenKind.Symbol || fCurrentToken.Value[0] != symbol) {
                throw new Exception("Required symbol not found");
            }
        }

        public void SkipWhiteSpaces()
        {
            while (fCurrentToken != null && fCurrentToken.Kind == TokenKind.WhiteSpace) {
                Next();
            }
        }

        public int RequestInt()
        {
            if (fCurrentToken == null || fCurrentToken.Kind != TokenKind.Number) {
                throw new Exception("Required integer not found");
            }

            return (int)fCurrentToken.ValObj;
        }
    }
}
