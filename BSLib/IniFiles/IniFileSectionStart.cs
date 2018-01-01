﻿using System;
using System.Text;

namespace BSLib.IniFiles
{
    /// <summary>Represents section's start line, e.g. "[SectionName]".</summary>
    public class IniFileSectionStart : IniFileElement
    {
        private string fInlineComment;
        private string fSectionName;
        private readonly string fTextOnTheRight; // e.g.  "[SectionName] some text"

        private IniFileSectionStart() : base()
        {
        }

        /// <summary>Initializes a new instance IniFileSectionStart</summary>
        /// <param name="content">Actual content of a line in an INI file. Initializer assumes that it is valid.</param>
        public IniFileSectionStart(string content) : base(content)
        {
            fFormatting = ExtractFormat(content);
            content = content.TrimStart();
            if (IniFileEx.AllowInlineComments) {
                IndexOfAnyResult result = IndexOfAny(content, IniFileEx.CommentChars);
                if (result.Index > content.IndexOf(IniFileEx.SectionCloseBracket)) {
                    fInlineComment = content.Substring(result.Index + result.Any.Length);
                    content = content.Substring(0, result.Index);
                }
            }
            if (IniFileEx.AllowTextOnTheRight) {
                int closeBracketPos = content.LastIndexOf(IniFileEx.SectionCloseBracket);
                if (closeBracketPos != content.Length - 1) {
                    fTextOnTheRight = content.Substring(closeBracketPos + 1);
                    content = content.Substring(0, closeBracketPos);
                }
            }
            fSectionName = content.Substring(IniFileEx.SectionOpenBracket.Length, content.Length - IniFileEx.SectionCloseBracket.Length - IniFileEx.SectionOpenBracket.Length).Trim();
            Content = content;
            Format();
        }

        /// <summary>Gets or sets a secion's name.</summary>
        public string SectionName
        {
            get { return fSectionName; }
            set
            {
                fSectionName = value;
                Format();
            }
        }

        /// <summary>Gets or sets an inline comment, which appear after the value.</summary>
        public string InlineComment
        {
            get { return fInlineComment; }
            set
            {
                if (!IniFileEx.AllowInlineComments || IniFileEx.CommentChars.Length == 0)
                    throw new NotSupportedException("Inline comments are disabled.");
                fInlineComment = value; Format();
            }
        }

        /// <summary>Determines whether specified string is a representation of particular IniFileElement object.</summary>
        /// <param name="testLine">Trimmed test string.</param>
        public static bool IsLineValid(string testLine)
        {
            return testLine.StartsWith(IniFileEx.SectionOpenBracket) && testLine.EndsWith(IniFileEx.SectionCloseBracket);
        }

        /// <summary>Gets a string representation of this IniFileSectionStart object.</summary>
        public override string ToString()
        {
            return "Section: \"" + fSectionName + "\"";
        }

        /// <summary>Creates a new IniFileSectionStart object basing on a name of section and the formatting style of this section.</summary>
        /// <param name="sectName">Name of the new section</param>
        public IniFileSectionStart CreateNew(string sectName)
        {
            IniFileSectionStart ret = new IniFileSectionStart();
            ret.fSectionName = sectName;

            if (IniFileEx.PreserveFormatting) {
                ret.fFormatting = fFormatting;
                ret.Format();
            } else
                ret.Format();

            return ret;
        }

        /// <summary>Creates a formatting string basing on an actual content of a line.</summary>
        public static string ExtractFormat(string content)
        {
            bool beforeS = false;
            bool afterS = false;
            bool beforeEvery = true;
            string insideWhiteChars = "";

            StringBuilder form = new StringBuilder();
            for (int i = 0; i < content.Length; i++) {
                char currC = content[i];
                if (char.IsLetterOrDigit(currC) && beforeS) {
                    afterS = true;
                    beforeS = false;
                    form.Append('$');
                } else if (afterS && char.IsLetterOrDigit(currC)) {
                    insideWhiteChars = "";
                } else if (content.Length - i >= IniFileEx.SectionOpenBracket.Length && content.Substring(i, IniFileEx.SectionOpenBracket.Length) == IniFileEx.SectionOpenBracket && beforeEvery) {
                    beforeS = true;
                    beforeEvery = false;
                    form.Append('[');
                } else if (content.Length - i >= IniFileEx.SectionCloseBracket.Length && content.Substring(i, IniFileEx.SectionOpenBracket.Length) == IniFileEx.SectionCloseBracket && afterS) {
                    form.Append(insideWhiteChars);
                    afterS = false;
                    form.Append(IniFileEx.SectionCloseBracket);
                } else if ((OfAny(i, content, IniFileEx.CommentChars)) != null) {
                    form.Append(';');
                } else if (char.IsWhiteSpace(currC)) {
                    if (afterS)
                        insideWhiteChars += currC;
                    else
                        form.Append(currC);
                }
            }
            string ret = form.ToString();
            if (ret.IndexOf(';') == -1)
                ret += "   ;";
            return ret;
        }

        /// <summary>Formats the IniFileElement object using default format specified in IniFileEx.</summary>
        public override void FormatDefault()
        {
            Formatting = IniFileEx.DefaultSectionFormatting;
            Format();
        }

        /// <summary>Formats this element using a formatting string in Formatting property.</summary>
        public void Format()
        {
            Format(fFormatting);
        }

        /// <summary>Formats this element using given formatting string</summary>
        /// <param name="pFormatting">Formatting template, where '['-open bracket, '$'-section name, ']'-close bracket, ';'-inline comments.</param>
        public void Format(string pFormatting)
        {
            StringBuilder build = new StringBuilder();
            for (int i = 0; i < pFormatting.Length; i++)
            {
                char currC = pFormatting[i];
                if (currC == '$')
                    build.Append(fSectionName);
                else if (currC == '[')
                    build.Append(IniFileEx.SectionOpenBracket);
                else if (currC == ']')
                    build.Append(IniFileEx.SectionCloseBracket);
                else if (currC == ';' && IniFileEx.CommentChars.Length > 0 && fInlineComment != null)
                    build.Append(IniFileEx.CommentChars[0]).Append(fInlineComment);
                else if (char.IsWhiteSpace(pFormatting[i]))
                    build.Append(pFormatting[i]);
            }
            Content = build.ToString().TrimEnd() + (IniFileEx.AllowTextOnTheRight ? fTextOnTheRight : "");
        }

        /// <summary>Crates a IniFileSectionStart object from name of a section.</summary>
        /// <param name="sectionName">Name of a section</param>
        public static IniFileSectionStart FromName(string sectionName)
        {
            IniFileSectionStart ret = new IniFileSectionStart();
            ret.SectionName = sectionName;
            ret.FormatDefault();
            return ret;
        }
    }
}
