namespace DotNetUtils
{
    public static class Constants
    {
        public const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        public const string
            IdListElementName = "IdList",
            IdListItemName = "Item",
            IdListAttributeName = "Id",
            // adapted the pattern from MSDN: http://msdn.microsoft.com/en-us/library/01escwtf%28v=VS.71%29.aspx
            EmailPattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public static readonly char[] LowerCharArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static readonly char[] ColonDelimCharArray = { ':' };
        public static readonly char[] CommaDelimCharArray = { ',' };
        public static readonly char[] SemiDelimCharArray = { ';' };
        public static readonly char[] TabDelimCharArray = { '\t' };
        public static readonly char ColonDelimChar = ':';
        public static readonly char CommaDelimChar = ',';
        public static readonly char SemiDelimChar = ';';
        public static readonly char TabDelimChar = '\t';
        public static readonly string ColonDelim = ":";
        public static readonly string CommaDelim = ",";
        public static readonly string SemiDelim = ";";
        public static readonly string TabDelim = "\t";

        /// <summary>
        /// Gets NumberFormatSpecifier without currency symbol
        /// </summary>
        public const string NumberFormatSpecifier = "#,0";

        /// <summary>
        /// Gets NumberFormatSpecifier with decimal part
        /// </summary>
        public const string NumberFormatWithDecimalSpecifier = "#,0.00";

        /// <summary>
        /// Gets NumberFormatSpecifier with decimal part
        /// </summary>
        public const string NumberFormatFourAfterDecimal = "#,0.0000";

        /// <summary>
        /// Gets CurrencyFormatSpecifier with currency symbol place holder
        /// </summary>
        public const string CurrencyFormatSpecifierPattern = "{0}#,0";

        /// <summary>
        /// Gets CurrencyFormatSpecifier with currency symbol place holder and decimal part
        /// </summary>
        public const string CurrencyFormatWithDecimalSpecifierPattern = "{0}#,0.00";

        /// <summary>
        /// Gets CurrencyFormatSpecifier with currency symbol place holder with 4 digits after decimal point and decimal part
        /// </summary>
        public const string CurrencyFormatWith4DigitsDecimalSpecifierPattern = "{0}#,0.0000";

    }
}
