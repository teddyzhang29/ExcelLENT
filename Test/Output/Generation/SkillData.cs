public class SkillData
{
    private static System.Collections.Generic.List<Row> s_rows;
    public static void Deserialize(string serialization)
    {
        s_rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<Row>>(serialization);
        foreach (var row in s_rows)
        {
            {
                System.Collections.Generic.Dictionary<System.Collections.Generic.List<bool>, Row> success;
                if (!s_idSuccessMap.TryGetValue(row.id, out success))
                {
                    success = new System.Collections.Generic.Dictionary<System.Collections.Generic.List<bool>, Row>();
                    s_idSuccessMap.Add(row.id, success);
                }
                success.Add(row.success, row);
            }
        }
    }

    private static System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<System.Collections.Generic.List<bool>, Row>> s_idSuccessMap = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<System.Collections.Generic.List<bool>, Row>>();
    public static Row FindByIDSuccess(int id, System.Collections.Generic.List<bool> success)
    {
        return s_idSuccessMap[id][success];
    }
    public class Row
    {
        public int id;
        public System.Collections.Generic.List<int> skills;
        public (int x, int z) pos;
        public System.Collections.Generic.List<(int id, float level)> skill2;
        public System.Collections.Generic.List<bool> success;
        public (string name, string address) desc;
        public ((string name, int level) player, (int x, int z) pos) nestedObj;
    }

}
