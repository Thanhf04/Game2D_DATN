using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadscene : MonoBehaviour
{
    Dichuyennv1 dichuyennv1;

    // Start is called before the first frame update
    void Start()
    {
        dichuyennv1 = FindObjectOfType<Dichuyennv1>();
        PlayerStats.Instance.LoadStats();
    }

    // Update is called once per frame
    void Update()
    {
        dichuyennv1.healthSlider.value = PlayerStats.Instance.hp;
        dichuyennv1.manaSlider.value = PlayerStats.Instance.mana;
        dichuyennv1.expSlider.value = PlayerStats.Instance.exp;
    }
}
