using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider; // Slider để điều chỉnh âm lượng
    public AudioSource audioSource; // AudioSource chứa âm thanh bạn muốn điều chỉnh

    private void Start()
    {
        // Thiết lập giá trị ban đầu của slider từ âm lượng hiện tại
        volumeSlider.value = audioSource.volume;

        // Thêm listener để thay đổi âm lượng khi kéo thanh slider
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    // Phương thức thay đổi âm lượng khi kéo thanh slider
    private void UpdateVolume(float value)
    {
        audioSource.volume = value; // Áp dụng âm lượng ngay lập tức
    }
}
