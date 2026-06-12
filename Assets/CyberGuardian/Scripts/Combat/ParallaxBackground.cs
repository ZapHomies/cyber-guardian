using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Menyimpan panjang horizontal sprite background dan posisi awal koordinat X
    private float length, startPos;
    
    [Header("Pengaturan Kamera & Kecepatan")]
    public GameObject cam;          // Tarik objek Main Camera ke sini di Inspector
    public float parallaxEffect;    // Nilai antara 0 sampai 1 (Contoh: 0.5f)

    void Start()
    {
        // Mencatat posisi awal X dari background saat game pertama kali dimulai
        startPos = transform.position.x;
        
        // Mengambil ukuran lebar (X) dari gambar/sprite background secara otomatis
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Menghitung seberapa jauh background harus bergeser relatif terhadap kamera
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // Mengubah posisi background ke posisi baru berdasarkan pergerakan kamera
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // LOGIKA LOOPING (Agar background menyambung terus tanpa batas saat player jalan terus)
        if (temp > startPos + length) 
        {
            startPos += length; // Geser posisi awal ke kanan jika kamera melewati batas
        }
        else if (temp < startPos - length) 
        {
            startPos -= length; // Geser posisi awal ke kiri jika kamera mundur ke belakang
        }
    }
}