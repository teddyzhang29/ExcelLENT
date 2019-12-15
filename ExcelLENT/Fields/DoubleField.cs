using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class DoubleField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            serializer.DoubleField(this, double.Parse(reader.NextContent()));
        }
    }
}