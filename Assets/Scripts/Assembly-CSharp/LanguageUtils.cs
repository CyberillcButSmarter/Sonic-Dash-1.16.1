using System.Globalization;

public class LanguageUtils
{
	private static NumberFormatInfo s_numberFormatter;

	private static TextInfo s_textInfo;

	public static string FormatNumber(long numberToFormat)
	{
		if (s_numberFormatter == null)
		{
			InitialiseNumberFormatter();
		}
		if (s_numberFormatter == null)
		{
			return numberToFormat.ToString();
		}
		return numberToFormat.ToString("n", s_numberFormatter);
	}

	public static string TitleCaseString(string stringToCapitalise)
	{
		if (s_textInfo == null)
		{
			InitialiseTextInfo();
		}
		return s_textInfo.ToTitleCase(stringToCapitalise.ToLowerInvariant());
	}

	private static void InitialiseNumberFormatter()
	{
		if (!(LanguageStrings.First == null))
		{
			s_numberFormatter = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
			string text = LanguageStrings.First.GetString("NUMBER_SEPARATOR");
			if (text == "*")
			{
				text = " ";
			}
			s_numberFormatter.NumberGroupSeparator = text;
			s_numberFormatter.NumberDecimalDigits = 0;
		}
	}

	private static void InitialiseTextInfo()
	{
		s_textInfo = (TextInfo)CultureInfo.InvariantCulture.TextInfo.Clone();
	}
}
