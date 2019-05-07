using System.IO;
using System.Text;

namespace BBGo.ExcelLENT
{
    public static class Utility
    {
        public static void SaveToFile(string text, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(text);
                }
            }
        }

        public static string ToPascalCase(params string[] values)
        {
            if (values == null || values.Length == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Length <= 2)
                {
                    builder.Append(values[i].ToUpper());
                    continue;
                }

                builder.Append(values[i].Substring(0, 1).ToUpper());
                builder.Append(values[i].Substring(1, values[i].Length - 1));
            }
            return builder.ToString();
        }

        public static string ToCamelCase(string[] values, int start = 0)
        {
            if (values == null || values.Length == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.Append(values[start].ToLower());
            for (int i = start + 1; i < values.Length; i++)
            {
                builder.Append(ToPascalCase(values[i]));
            }
            return builder.ToString();
        }
    }
}
