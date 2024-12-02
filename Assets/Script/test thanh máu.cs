using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public Slider healthPlayer; // Để tham chiếu đến Image của HealthFill
    Dichuyennv1 dichuyennv1;


    // public void IncreaseHealth(int amount)
    // {

    //     dichuyennv1.currentHealth += amount;
    //     if (dichuyennv1.currentHealth > dichuyennv1.maxHealth)
    //     {
    //         dichuyennv1.currentHealth = dichuyennv1.maxHealth; // Không vượt quá máu tối đa
    //     }
    // }

    // public void DecreaseHealth(int amount)
    // {
    //     dichuyennv1.currentHealth -= amount;
    //     if (dichuyennv1.currentHealth < 0)
    //     {
    //         dichuyennv1.currentHealth = 0; // Không vượt quá 0 máu
    //     }
    // }


}
