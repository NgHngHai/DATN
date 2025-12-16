using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseController : MonoBehaviour
{
    public Image imgIllustration;
    public TextMeshProUGUI txtDataName, txtDataDescription;
    [Header("Data")]
    public Sprite mapDataSprite1, mapDataSprite2;
    string data1name1, data1name2, data2name1, data2name2, data3name1, data3name2;
    string data1description1, data1description2, data2description1, data2description2, data3description1, data3description2;
    int selectingData;

    void Awake()
    {
        selectingData = 0;
        data1name1 = "Wasteland Processor";
        data1name2 = "Khoang xả thải";
        data2name1 = "Euphonious Melodia";
        data2name2 = "Âm hưởng bất diệt";
        data3name1 = "No Data";
        data3name2 = "Chưa có dữ liệu";
        data1description1 = "Used to be the waste processor, where the ship collected its waste for recycling. Home to MEOR and many other old models working as scrap collectors. Not many automaton frequents this place anymore after the first 100 years passed. It’s basically turned into a deserted cabin by now.";
        data1description2 = "Từng là khoang xử lý chất thải của phi thuyền, nơi thu thập và tái chế rác thải thành năng lượng. Nơi đây là nhà của MEOR cùng nhiều model cũ với nhiệm vụ dọn dẹp, làm sạch phi thuyền hàng ngày. Tuy nhiên, do số lượng robot sụt giảm, khoảng 100 năm sau khi phi thuyền được phóng lên vũ trụ, hầu hết robot đã di chuyển đến các khoang khác, biến nơi đây thành một hầm chứa bỏ hoang.";
        data2description1 = "One of the ship's twelve main exhibitions focuses on the preservation of human melodies. It was the first place to be corrupted due to the sheer number of automatons it has. Ever since the attack, the ship has fallen into silence, and The First Composer has gone missing.";
        data2description2 = "Một trong mười hai khoang triển lãm chính của phi thuyền, lưu giữ những giai điệu âm nhạc nổi bất nhất của nhân loại. Do số lượng robot bảo trì khổng lồ trong khoang, đây cũng là một trong những khoang đầu tiên bị phá huỷ. Kể từ khi V.I.R.U.S chiếm được quyền kiểm soát khoang, Tổng Nhạc Trưởng đã biến mất một cách bí ẩn.";
        data3description1 = "Acquire more data to unlock this entry.";
        data3description2 = "Thu thập thêm tài liệu để mở khoá dữ liệu này.";
    }

    public void DisplayData(int id)
    {
        if (id == 0)
        {
            imgIllustration.color = new(1, 1, 1, 1);
            imgIllustration.sprite = mapDataSprite1;
            txtDataName.text = data1name1;
            txtDataDescription.text = data1description1;
        }
        else if (id == 1)
        {
            imgIllustration.color = new(1, 1, 1, 1);
            imgIllustration.sprite = mapDataSprite2;
            txtDataName.text = data2name1;
            txtDataDescription.text = data2description1;
        }
        else
        {
            imgIllustration.color = new(0, 0, 0, 0);
            txtDataName.text = data3name1;
            txtDataDescription.text = data3description1;
        }

        transform.GetChild(0).GetChild(selectingData).GetChild(0).gameObject.SetActive(false);
        selectingData = id;
    }


    public bool IsSelectingDataIndex(int i)
    {
        return selectingData == i;
    }
}
