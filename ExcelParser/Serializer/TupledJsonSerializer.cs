using ExcelParser.Fields;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ExcelParser.Serializer
{
    public class TupledJsonSerializer : ISerializer
    {
        private JArray m_data = new JArray();
        private JObject m_row = null;
        private JToken m_currentJToken = null;
        private Stack<char> m_matchStack = new Stack<char>();

        public string Result
        {
            get
            {
                return m_data.ToString();
            }
        }

        public void BeginRow()
        {
            m_row = new JObject();
        }

        public void EndRow()
        {
            m_data.Add(m_row);
            m_row = null;
        }

        public void BeginField(BaseField field)
        {
        }

        public void EndField(BaseField field)
        {
            m_row.Add(field.Name, m_currentJToken);
            m_currentJToken = null;
        }

        public void BeginList(BaseField field)
        {
            BeginGroup(field, new JArray());
        }
        public void BeginObject(ObjectField field)
        {
            BeginGroup(field, new JObject());
        }

        private void BeginGroup(BaseField field, JToken group)
        {
            m_matchStack.Push('{');
            JObject jo = m_currentJToken as JObject;
            if (m_currentJToken is JObject)
            {
                JObject objField = m_currentJToken as JObject;
                objField.Add(field.Name, group);
            }
            else if (m_currentJToken is JArray)
            {
                JArray arrayField = m_currentJToken as JArray;
                arrayField.Add(group);
            }
            else if (m_currentJToken != null)
            {
                throw new Exception($"'{field.GetType()}' 类型无法添加子类型。");
            }
            m_currentJToken = group;
        }

        public void EndList(BaseField field)
        {
            EndGroup(field);
        }
        public void EndObject(ObjectField field)
        {
            EndGroup(field);
        }

        private void EndGroup(BaseField field)
        {
            if (m_matchStack.Count == 0)
            {
                throw new Exception("括号不匹配。");
            }
            m_matchStack.Pop();
            if (m_matchStack.Count > 0)
            {
                m_currentJToken = m_currentJToken.Parent;
                if (m_currentJToken is JProperty)
                {
                    JProperty jp = m_currentJToken as JProperty;
                    m_currentJToken = jp.Value;
                }
            }
        }

        public void FloatField(FloatField field, float value)
        {
            SimpleField(field, value);
        }

        public void DoubleField(DoubleField field, double value)
        {
            SimpleField(field, value);
        }

        public void IntField(IntField field, int value)
        {
            SimpleField(field, value);
        }

        public void BoolField(BoolField field, bool value)
        {
            SimpleField(field, value);
        }

        public void StringField(StringField field, string value)
        {
            SimpleField(field, value);
        }

        private void SimpleField(BaseField field, JToken value)
        {
            if (m_currentJToken == null)
            {
                m_currentJToken = value;
            }
            else if (m_currentJToken is JObject)
            {
                JObject objField = m_currentJToken as JObject;
                //因为目前设计的结构可以任意嵌套,当嵌套层次过深时,若通过子类型的方式实现,取名比较麻烦. C# 7.0的Tuple比较容易解决这个问题.
                //但是Newtonsoft.Json在Tuple的支持上暂时只支持Item1,Item2这样的名称
                objField.Add("Item" + (objField.Count + 1), value);
            }
            else if (m_currentJToken is JArray)
            {
                JArray arrayField = m_currentJToken as JArray;
                arrayField.Add(value);
            }
        }
    }
}