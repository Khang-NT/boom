using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
    public MapManager mapManager;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                Debug.Log(mapManager[i, j]);
    }
	
	// Update is called once per frame
	void Update () {
        // con ma muốn tinh1 đường đi thi phải biết hết cái map phai k 
        // neu ma brick hay vat can minh khong viet script lay toa do thi dung vay :D 
        // cái ô góc dưới trái là tọa độ 00, giống như oxy, muốn xem ô nào đó là game object gì thì
        // get trong cái map manager ra
        
	}
}
