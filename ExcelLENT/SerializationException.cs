using BBGo.ExcelLENT.Fields;
using System;

namespace BBGo.ExcelLENT
{
    [Serializable]
    public class SerializationException : Exception
    {
        public SerializationException(ExcelSheet sheet, int rowNum, BaseField field, Exception inner) : base($"Sheet:`{sheet.Sheet.SheetName}`, RowNum:`{rowNum + 1}`, Field:`{field.Name}`", inner) { }
        protected SerializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
