using BBGo.ExcelLENT.Fields;

namespace BBGo.ExcelLENT.Serializer
{
    public interface ISerializer
    {
        string Result { get; }
        void BeginRow();
        void EndRow();
        void BeginField(BaseField field);
        void EndField(BaseField field);
        void BeginList(BaseField field);
        void EndList(BaseField field);
        void BeginObject(ObjectField field);
        void EndObject(ObjectField field);
        void FloatField(FloatField field, float value);
        void DoubleField(DoubleField field, double value);
        void IntField(IntField field, int value);
        void BoolField(BoolField field, bool value);
        void StringField(StringField field, string value);
    }
}
