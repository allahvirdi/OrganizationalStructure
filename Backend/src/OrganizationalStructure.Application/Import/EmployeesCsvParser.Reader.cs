using System.Text;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// خواننده مشترک ردیف‌های فایل CSV پرسنل.
/// </summary>
public sealed partial class EmployeesCsvParser
{
    /// <summary>
    /// خواندن همه ردیف‌های CSV با پشتیبانی از نقل‌قول‌دوتایی.
    /// </summary>
    private static List<string[]> ReadRows(TextReader reader)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;
        int charCode;

        while ((charCode = reader.Read()) != -1)
        {
            var ch = (char)charCode;

            if (inQuotes)
            {
                if (ch == '"')
                {
                    if (reader.Peek() == '"')
                    {
                        field.Append('"');
                        reader.Read();
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(ch);
                }

                continue;
            }

            switch (ch)
            {
                case '"':
                    inQuotes = true;
                    break;

                case ',':
                    fields.Add(field.ToString());
                    field.Clear();
                    break;

                case '\r':
                    break;

                case '\n':
                    fields.Add(field.ToString());
                    rows.Add(fields.ToArray());
                    fields.Clear();
                    field.Clear();
                    break;

                default:
                    field.Append(ch);
                    break;
            }
        }

        if (field.Length > 0 || fields.Count > 0)
        {
            fields.Add(field.ToString());
            rows.Add(fields.ToArray());
        }

        return rows;
    }
}