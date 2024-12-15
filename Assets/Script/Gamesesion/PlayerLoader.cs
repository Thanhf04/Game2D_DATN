// using System.Collections;
// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;

// public class PlayerLoader : MonoBehaviour
// {
//     Gamesesion gamesesion;
//     int maxhp;
//     int hpcurrent;
//     int maxmana;
//     int manacurrrent;
//     float maxexp;
//     float expCurrent;
//     int goldht;

//     private Dichuyennv1 dichuyennv1; // Không cần khai báo lại biến dichuyennv1

//     void Start()
//     {
//         // Lấy tham chiếu đến script Dichuyennv1 trên cùng một GameObject
//         dichuyennv1 = GetComponent<Dichuyennv1>();

//         if (dichuyennv1 == null)
//         {
//             Debug.LogError("Không tìm thấy Dichuyennv1 trên GameObject này!");
//         }
//         else
//         {
//             // Khởi tạo các giá trị
//             maxhp = Gamesesion.Instance.maxHealth;
//             hpcurrent = Gamesesion.Instance.currentHealth;
//             maxmana = Gamesesion.Instance.maxHealth;
//             manacurrrent = Gamesesion.Instance.currentMana;
//             maxexp = Gamesesion.Instance.expMax;
//             expCurrent = Gamesesion.Instance.expCurrent;
//             goldht = Gamesesion.Instance.gold;
//         }
//     }

//     void Update()
//     {
//         // Lấy tham chiếu đến script Dichuyennv1 trên cùng một GameObject
//         dichuyennv1 = GetComponent<Dichuyennv1>();

//         if (dichuyennv1 == null)
//         {
//             Debug.LogError("Không tìm thấy Dichuyennv1 trên GameObject này!");
//         }
//         else
//         {
//             // Khởi tạo các giá trị
//             maxhp = Gamesesion.Instance.maxHealth;
//             hpcurrent = Gamesesion.Instance.currentHealth;
//             maxmana = Gamesesion.Instance.maxHealth;
//             manacurrrent = Gamesesion.Instance.currentMana;
//             maxexp = Gamesesion.Instance.expMax;
//             expCurrent = Gamesesion.Instance.expCurrent;
//             goldht = Gamesesion.Instance.gold;
//         }
//     }

//     public void SetSlider()
//     {
//         if (dichuyennv1 != null)
//         {
//             dichuyennv1.healthSlider.maxValue = maxhp;
//             dichuyennv1.manaSlider.maxValue = maxmana;
//             dichuyennv1.expSlider.maxValue = maxexp;
//         }
//         else
//         {
//             Debug.LogError("dichuyennv1 chưa được khởi tạo!");
//         }
//     }

//     public void loaddata()
//     {
//         if (dichuyennv1 != null)
//         {
//             dichuyennv1.healthSlider.value = hpcurrent;
//             dichuyennv1.manaSlider.value = manacurrrent;
//             dichuyennv1.expSlider.value = expCurrent;
//         }
//         else
//         {
//             Debug.LogError("dichuyennv1 chưa được khởi tạo!");
//         }
//     }
// }
