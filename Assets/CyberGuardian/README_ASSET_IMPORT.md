# Cyber Guardian Asset Import Guide

Folder ini berisi copy runtime asset yang siap dipakai Unity. File asli tetap ada di:

`E:\Tugas Project\Pengembangan Game\asset`

## Mapping Folder

- `Art/UI/CyberpunkPixelUI`
  - UI utama: frame, panel, tombol, bar, cursor, font, dan elemen HUD cyberpunk.
- `Art/UI/KenneySpaceUI`
  - UI sci-fi fallback: panel, ikon, crosshair, dan elemen interface bersih.
- `Art/VFX/KenneyParticles`
  - Sprite particle untuk shield, hit burst, portal data, efek benar/salah.
- `Art/Enemies/VirusBigPack`
  - PNG musuh/virus untuk target utama dan variasi malware.
- `Audio/SFX/KenneySciFi`
  - Efek suara laser, UI, shield, ambience, dan feedback interaksi.

## Langkah di Unity

1. Buka project `C:\Users\Wafda\My project` di Unity.
2. Tunggu Unity selesai import asset baru.
3. Pilih folder `Assets/CyberGuardian/Art`.
4. Untuk file PNG yang dipakai sebagai sprite gameplay:
   - `Texture Type`: Sprite (2D and UI)
   - `Sprite Mode`: Single, atau Multiple untuk spritesheet
   - `Pixels Per Unit`: 100 untuk UI, 64 atau 100 untuk gameplay
   - `Filter Mode`: Point untuk pixel art, Bilinear untuk efek halus
   - `Compression`: None atau Low Quality untuk sprite kecil
5. Untuk UI, drag sprite ke Canvas sebagai Image.
6. Untuk musuh/virus, drag PNG ke Scene lalu buat Prefab di `Assets/CyberGuardian/Prefabs/Enemies`.
7. Untuk audio `.ogg`, drag ke komponen AudioSource atau panggil lewat script.

## Saran Pemakaian V1

- Quiz orb: buat 4 prefab bola data dari sprite lingkaran/particle, lalu beri warna biru, merah, kuning, dan ungu di SpriteRenderer.
- Virus target: mulai dari satu PNG VirusBigPack sebagai `VirusCore`, lalu tambahkan aura merah/glitch dengan particle.
- Shield guardian: gunakan KenneyParticles sebagai ring/glow di sekitar karakter guardian.
- HUD quiz: pakai CyberpunkPixelUI sebagai modal pertanyaan dan health/shield bar.

Kenney Puzzle Pack tidak wajib untuk V1. Jika download paket itu bermasalah, orb quiz bisa dibuat langsung dari sprite lingkaran/particle yang sudah ada.
