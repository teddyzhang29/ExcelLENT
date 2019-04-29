public class SkillData
{
    public static SkillData Deserialize(string serialization)
    {
        SkillData data = new SkillData();
        data.m_rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<Row>>(serialization);
        return data;
    }

    public class Row
    {
        public System.Collections.Generic.List<int> skills;
        public (int x, int z) pos;
        public System.Collections.Generic.List<(int id, float level)> skill2;
        public System.Collections.Generic.List<bool> success;
        public (string name, string address) desc;
        public ((string name, int level) player, (int x, int z) pos) nestedObj;
    }

    private System.Collections.Generic.List<Row> m_rows;
}
