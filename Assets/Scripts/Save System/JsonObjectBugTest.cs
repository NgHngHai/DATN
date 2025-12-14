using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Cần cái này để xử lý JObject

public class JsonObjectBugFix : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int hp;
        public int mana;
    }

    [System.Serializable]
    public class GameData
    {
        public Dictionary<string, object> savedObjects = new Dictionary<string, object>();
    }

    private string cachedJson;

    void Start()
    {
        Debug.Log("===== STEP 1: CREATE DATA =====");
        GameData data = new GameData();
        data.savedObjects["player"] = new PlayerStats { hp = 100, mana = 50 };

        // Lưu dữ liệu
        cachedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log("JSON SAVED:\n" + cachedJson);


        Debug.Log("===== STEP 2: DESERIALIZE =====");
        // Load lại dữ liệu
        GameData loaded = JsonConvert.DeserializeObject<GameData>(cachedJson);
        object state = loaded.savedObjects["player"];

        // Kiểm tra type thực tế (Nó sẽ là Newtonsoft.Json.Linq.JObject)
        Debug.Log("Loaded type đang là: " + state.GetType());


        Debug.Log("===== STEP 3: THỬ CAST TRỰC TIẾP (SẼ LỖI) =====");
        try
        {
            // Dòng này sẽ ném ra InvalidCastException
            PlayerStats statsFail = (PlayerStats)state;
        }
        catch (System.InvalidCastException e)
        {
            Debug.LogError("❌ Lỗi xảy ra đúng như dự đoán: " + e.Message);
            Debug.LogError("Lý do: Không thể ép kiểu JObject thẳng sang PlayerStats.");
        }


        // ==========================================================
        // 👇 GIẢI PHÁP Ở ĐÂY 👇
        // ==========================================================
        Debug.Log("===== STEP 4: GIẢI PHÁP (SỬ DỤNG TOOBJECT) =====");

        // Kiểm tra xem state có phải là JObject không (thường là có)
        if (state is JObject jState)
        {
            // ✅ Dùng hàm ToObject<T>() để convert JObject về class mong muốn
            PlayerStats statsSuccess = jState.ToObject<PlayerStats>();

            Debug.Log("✅ Fix thành công!");
            Debug.Log($"HP: {statsSuccess.hp}, Mana: {statsSuccess.mana}");
        }
        else
        {
            // Trường hợp nếu bạn config TypeNameHandling thì nó có thể tự về đúng type, 
            // nhưng mặc định thì nó sẽ chạy vào if bên trên.
            Debug.Log("Object không phải là JObject (có thể đã đúng type sẵn).");
        }
    }
}