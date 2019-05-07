using NPOI.SS.UserModel;

namespace BBGo.ExcelLENT
{
    public static class ExcelEx
    {

        public static string GetStringCellValue(this ICell cell)
        {
            object value = GetCellValue(cell);
            if (value == null)
                return string.Empty;
            return value.ToString().Trim();
        }

        public static object GetCellValue(this ICell cell)
        {
            if (cell == null)
                return null;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Formula:
                    try
                    {
                        return cell.NumericCellValue;
                    }
                    catch
                    {
                        return cell.StringCellValue;
                    }
                default:
                    return null;
            }
        }

        public static double GetNumericCellValue(this ICell cell)
        {
            object value = GetCellValue(cell);
            if (value == null)
                return 0;

            if (value is double)
            {
                return (double)value;
            }
            return 0;
        }
    }
}
