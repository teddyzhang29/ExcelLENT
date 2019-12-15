using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class FloatField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            serializer.FloatField(this, float.Parse(reader.NextContent()));
        }
    }
}