using BBGo.ExcelLENT.Serializer;

namespace BBGo.ExcelLENT.Fields
{
    public class BoolField : BaseField
    {
        internal override void OnSerialize(ISerializer serializer, Reader reader)
        {
            serializer.BoolField(this, bool.Parse(reader.NextContent()));
        }
    }
}
