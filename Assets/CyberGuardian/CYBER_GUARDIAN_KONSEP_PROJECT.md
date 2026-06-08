# Cyber Guardian - Dokumen Konsep Project

Terakhir diperbarui: 05-06-2026

Catatan penting: sebelumnya di folder project hanya ditemukan `README_ASSET_IMPORT.md`, yaitu panduan impor asset. Dokumen konsep utama belum ada di project, jadi file ini dibuat sebagai satu sumber rujukan jangka panjang untuk desain game, fitur, asset, scene, story, dan roadmap.

## 1. Ringkasan Game

`Cyber Guardian` adalah game PC 2D side-scrolling bergaya 2.5D dengan tema dunia di dalam komputer. Gameplay utama mengambil rasa petualangan platformer/action seperti `Metal Slug` dan `Unfair Rampage`, tetapi saat bertemu boss berubah otomatis menjadi mode `slingshot projectile` seperti Angry Birds.

Player mengendalikan manusia berbentuk Cyber Guardian yang masuk ke jaringan komputer untuk memperbaiki sistem yang diserang virus, malware, trojan, worm, dan boss digital. Game tetap membawa unsur edukasi lewat quiz cybersecurity. Quiz tidak hanya menjadi pop-up pertanyaan, tetapi menjadi bagian langsung dari rintangan gameplay lewat `quiz block` yang menghalangi jalur serangan ke boss.

## 2. Pilar Desain

1. Action side-scroller yang menantang.
2. Edukasi cybersecurity yang menyatu dengan gameplay.
3. Dunia cyber yang jelas terbaca: background berbeda dari block, trap, musuh, dan UI.
4. Boss battle unik: player harus membuka celah shield/quiz block, lalu menarik dan melempar proyektil.
5. Main menu dan HUD bersih, rapi, dan bergaya game cyber horror.
6. Asset dapat dikembangkan lewat sprite 2D, UI pack, VFX, audio, dan Blender untuk kebutuhan 2.5D/GLB.

## 3. Genre dan Target

- Platform: PC.
- Engine: Unity 6.4 `6000.4.8f1`.
- Genre utama: 2D side-scrolling action platformer.
- Gaya visual: cyber horror, dunia dalam komputer, neon cyan/magenta, circuit board, server ruins, data abyss.
- Mode boss: slingshot physics dengan trajectory indicator.
- Target versi awal: 3 level playable dengan 3 tingkat kesulitan.

## 4. Core Gameplay Loop

1. Player mulai dari checkpoint atau awal level.
2. Muncul overlay story singkat tanpa frame besar.
3. Player bergerak, lompat, menyerang jarak dekat, menembak jarak jauh, dan memakai boost.
4. Player melewati platform, jalur bercabang, jebakan, musuh, power-up, dan area yang menjebak.
5. Saat mencapai boss trigger, game otomatis masuk ke `Boss Slingshot Mode`.
6. Boss dilindungi quiz block/pagar data.
7. Player menembak atau menghancurkan quiz block.
8. Saat quiz muncul, jawaban benar menghapus block dan memperkuat shield player; jawaban salah memberi damage dan memperkuat tekanan boss.
9. Setelah celah terbuka, player drag proyektil dari layar mana saja untuk menyerang boss.
10. Boss menyerang melalui celah atau pattern projectile.
11. Boss kalah, score bertambah, checkpoint/progress tersimpan, lalu lanjut scene berikutnya.

## 5. Kontrol

- `A/D` atau `Left/Right`: bergerak.
- `Space`, `W`, atau `Up`: lompat.
- `Left Shift`, `Right Shift`, atau `K`: boost/energy dash.
- `J`: melee attack.
- `L` atau klik kiri saat adventure: ranged projectile/fireball.
- Klik/tahan/tarik di layar saat boss mode: arahkan slingshot projectile.
- Lepas klik saat boss mode: tembak slingshot projectile.
- `P` atau `Esc`: pause.

Catatan desain: boost bar adalah energy untuk bergerak cepat di darat maupun udara. Boost default saat level mulai harus 100%.

## 6. Nilai Default Saat Game Dimulai

- Player HP/shield: 100%.
- Player punya 3 nyawa.
- Boost energy: 100%.
- Boss HP: 100%, tetapi hanya tampil saat boss encounter.
- Score: 0.
- Jika HP player mencapai 0%, 1 nyawa hilang dan player kembali ke checkpoint/recovery point dengan HP pulih.
- Game over terjadi saat HP mencapai 0% ketika nyawa sudah habis.
- Player tidak mati permanen karena jatuh jurang. Jika jatuh, player dikembalikan ke recovery point/checkpoint dengan punishment damage atau reposition.

## 7. Struktur Scene Saat Ini

Lokasi project aktif per 05-06-2026:

- `E:\Tugas Project\Pengembangan Game\cyber guardian`

Catatan migrasi: folder aktif ini dibuat tanpa membawa `Library`, `Logs`, `Temp`, sample URP, tutorial template Unity, dan cache Visual Scripting lama. Unity 6.4 akan membuat ulang cache `Library` sendiri saat project dibuka, sehingga error package dari cache versi sebelumnya tidak ikut terbawa.

Scene yang sudah ada di project:

- `Assets/CyberGuardian/Scenes/CyberGuardian_MainMenu.unity`
- `Assets/CyberGuardian/Scenes/CyberGuardian_PilihKesulitan.unity`
- `Assets/CyberGuardian/Scenes/CyberGuardian_Level01.unity`
- `Assets/CyberGuardian/Scenes/CyberGuardian_Level02.unity`
- `Assets/CyberGuardian/Scenes/CyberGuardian_Level03.unity`

Scene dibuat/diregenerasi lewat:

- `Assets/CyberGuardian/Editor/CyberGuardianGameBuilder.cs`

Menu Unity yang dipakai builder:

- `Cyber Guardian/Bangun Semua Scene Game`

Catatan Unity 6.4: dependency `com.unity.modules.physicscore2d`, `com.unity.modules.timelinefoundation`, dan versi `com.unity.cinemachine` yang tidak tersedia untuk project ini sudah dilepas dari package manifest. Physics 2D tetap memakai modul bawaan Unity, bukan package bernama `physicscore2d`.

Catatan: nama menu lama seperti `Open Layout Reference` tidak diperlukan untuk versi game sekarang dan tidak boleh menjadi bagian UI utama.

## 8. Main Menu

Konsep main menu:

- Background video cyber/circuit dari file video user.
- Logo game berada di area bawah.
- Tombol menu berada dalam satu baris melengkung ke bawah.
- Tombol tengah berada paling bawah, tombol samping kiri/kanan sedikit lebih tinggi.
- Tombol hanya berisi fungsi yang berguna.
- Tidak memakai ikon dekoratif di sisi tombol.
- Tombol difficulty dihapus jika tidak punya fungsi nyata di build.
- Tombol `Continue` wajib ada untuk melanjutkan checkpoint.

Tombol utama yang dipakai:

- `MULAI`
- `LANJUTKAN`
- `PENGATURAN`
- `KREDIT`
- `KELUAR`

Alur saat `MULAI`:

1. Video/menu melakukan slow transition.
2. Visual effect cyber/glitch/scanline muncul halus.
3. Masuk ke scene `CyberGuardian_PilihKesulitan`.
4. Player memilih `MUDAH`, `NORMAL`, atau `SULIT`.
5. Game masuk ke Level 01/Hutan Data.
6. Muncul pop-up `SIAP?`.
7. Countdown `3`, `2`, `1`, `MULAI!`.

Catatan bahasa: semua teks yang terlihat pemain harus memakai Bahasa Indonesia. Istilah game/teknis yang masih dipakai harus terasa natural, misalnya `bos`, `kuis`, `blok`, `perisai`, `energi`, `skor`, dan `checkpoint`.

## 9. HUD Main Game

Layout HUD yang diinginkan:

- Kiri atas: bar HP.
- Di dekat bar HP: indikator `NYAWA 3`.
- Tepat di bawah bar HP: bar energi/boost, hampir menyatu seperti HUD fighting game.
- Atas tengah/kanan bersih: skor besar dan tombol menu.
- Kanan atas: area bersih, hanya tombol menu/pause dan score jika diperlukan.
- Bar HP bos: tersembunyi saat adventure, tampil hanya saat boss encounter.
- Alert/story: overlay teks jelas, muncul-hilang sepanjang permainan, tanpa frame besar.
- Game over: kamera zoom ke karakter, animasi hancur/shatter, lalu pop-up game over muncul.

UI style:

- Cyber horror UI.
- Frame hitam gelap dengan outline cyan/magenta.
- Font ala game, tebal, pixel/blocky.
- Hindari UI terlalu ramai.

## 10. Player: Cyber Guardian

Asset player dari user sudah dikonversi menjadi frame PNG di:

- `Assets/CyberGuardian/Art/Player/CyberGuardianSprites`

Kategori frame yang tersedia:

- Idle.
- Running east/west.
- Jump east/west.
- Fire east/west.
- Rotation 8 direction.

Perilaku player:

- Bergerak kanan/kiri.
- Lompat responsif dengan coyote time dan jump buffer.
- Boost di darat dan udara memakai energy bar.
- Melee attack untuk musuh dekat.
- Ranged fireball/projectile untuk musuh dan block quiz jarak jauh.
- Saat boss mode, player tetap berada di arena dan input layar dipakai untuk slingshot projectile.

Prioritas polish player:

- Pastikan scale tidak berubah saat bergerak atau ganti arah.
- Pastikan animasi idle/run/jump/fire terpanggil sesuai state.
- Pastikan collider stabil dan tidak berubah mengikuti ukuran sprite.
- Pastikan tidak terjadi bug player menjadi besar saat gerak.

Script terkait:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianPlayerController.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianSpriteAnimator2D.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianCharacterAnimator2D.cs`

## 11. Musuh Basic

Asset musuh basic dari user sudah dikonversi menjadi frame PNG di:

- `Assets/CyberGuardian/Art/Enemies/BasicEnemySprites`

Tipe musuh:

1. `Malware Beast`
   - Monster berkaki 4.
   - Walk east/west.
   - Attack east/west.
   - Cocok untuk musuh dasar yang mengejar player.

2. `Cyber Hunter`
   - Musuh humanoid/cyber.
   - Walk east/west.
   - Cocok untuk variasi musuh yang lebih lincah.

Perilaku musuh:

- Patrol di atas platform.
- Mengejar player saat player masuk radius.
- Menyerang jika dekat.
- Dapat memberi damage.
- Dapat dihancurkan dengan melee atau ranged projectile.
- Harus selalu ditempatkan di atas block/platform, bukan melayang atau tertanam.

Script terkait:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianEnemy.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianEnemyAnimator2D.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianEnemySpriteAnimator2D.cs`

## 12. Boss

Boss adalah puncak tiap scene. Saat player mendekati arena, game otomatis masuk ke `Boss Slingshot Mode`.

Boss mode harus memiliki:

- Boss HP bar muncul hanya saat encounter.
- Player dikunci di arena boss.
- Slingshot projectile muncul.
- Klik/tarik dari layar mana pun mengontrol projectile.
- Trajectory indicator terlihat.
- Boss menyerang berkala.
- Quiz block/pagar data melindungi boss.
- Jawaban benar membuka celah.
- Jawaban salah memberi damage dan memberi peluang boss menyerang lebih agresif.

Level 03 boss:

- Boss super besar di udara.
- Serangan lebih banyak dan cepat.
- Projectile volley.
- Quiz block mengitari/memutari boss.
- Arena lebih luas dan punya platform untuk menghindar.

Script terkait:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianBossArenaTrigger.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianBossCore.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianBossProjectile.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianBossShieldBlock.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianOrbitingShieldBlock.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianSlingshotProjectile2D.cs`

## 13. Quiz Block

Quiz block menggantikan konsep bola quiz lama. Block harus terasa seperti object dunia cyber, bukan UI melayang.

Visual quiz block:

- Bentuk block/panel/circuit.
- Warna berbeda untuk kategori.
- Kontras jelas dengan background.
- Collider solid, tidak bisa dilewati player kecuali memang didesain sebagai trigger.
- Bisa dihancurkan oleh projectile atau interaksi boss mode.

Kategori quiz:

- Password.
- Malware.
- Network.
- Privacy.

Efek jawaban:

- Benar: block hilang, score bertambah, shield/HP/boost mendapat reward kecil, jalur boss terbuka.
- Salah: player terkena damage, boss mendapat peluang menyerang, feedback tampil.

Data quiz berada di:

- `Assets/CyberGuardian/Data/Quiz/CyberSecurity_Starter_QuestionBank.asset`

Script data quiz:

- `Assets/CyberGuardian/Scripts/Quiz/QuizQuestionBank.cs`

## 14. Trap dan Rintangan

Trap yang perlu ada atau sudah mulai diterapkan:

- Spinning saw.
- Swinging saw/gergaji gantung ayun.
- Laser barrier.
- Spike trap.
- Electric node.
- Glitch mine.
- Crushing block.
- Virus turret.
- Corrupted platform.
- Breakaway platform.
- Data abyss trap.
- Trap jalur palsu.
- Area jatuh yang mengembalikan player ke recovery zone.

Prinsip desain trap:

- Sulit tetapi terbaca.
- Ada timing yang bisa dipelajari.
- Tidak membunuh karena jurang secara instan.
- Memaksa player memakai jump, boost, stop, dan pilihan jalur.

Script terkait:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianDamageZone.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianSwingingTrap.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianRotator.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianMover.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianBreakawayPlatform.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianTurret.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianRecoveryZone.cs`

## 15. Power-Up dan Skill

Power-up yang cocok untuk versi awal:

- `Patch Kesehatan`: memulihkan HP/perisai.
- `Cache Energi`: mengisi energi boost.
- `Data Skor`: menambah skor.
- `Kunci Firewall`: membuka jalur aman.
- `Inti Patch`: memperkuat projectile sementara.

Skill player:

- Melee slash.
- Ranged fireball/projectile.
- Boost dash di darat.
- Boost dash di udara.
- Slingshot projectile saat boss mode.

Script terkait:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianPowerUp.cs`

## 16. Environment dan Visual

Tema environment:

1. Data Forest
   - Circuit tree, kabel, portal data, neon hijau/cyan.
   - Level 01 cocok sebagai pengenalan.

2. Server Ruins
   - Server rusak, platform metal, firewall gate, data debris.
   - Level 02 lebih panjang dan penuh jebakan.

3. Code Abyss
   - Dunia cyber yang retak, platform melayang, background lebih gelap, boss udara besar.
   - Level 03 paling menantang.

Masalah visual yang harus terus dijaga:

- Background dan block harus mudah dibedakan.
- Platform foreground perlu lebih terang/solid.
- Background perlu lebih redup, blur, atau parallax agar tidak bersaing dengan gameplay.
- Trap dan projectile harus punya warna warning yang jelas.
- UI tidak boleh menutupi area penting.

Script visual/parallax:

- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianParallaxLayer.cs`
- `Assets/CyberGuardian/Scripts/SideScroller/CyberGuardianPulseVisual.cs`

## 17. Camera

Kamera harus mengikuti player secara halus:

- Mengikuti X saat player berjalan.
- Mengikuti Y saat player lompat atau turun.
- Tidak terlalu jauh meninggalkan player.
- Saat boss mode, framing berpindah ke arena boss.
- Saat game over, kamera zoom ke karakter dan menampilkan animasi hancur.

Variabel camera berada di `CyberGuardianSideScrollerGame`:

- `cameraMin`
- `cameraMax`
- Boss arena center/min/max.

## 18. Difficulty

Difficulty profile berada di:

- `Assets/CyberGuardian/Data/Difficulty/Easy.asset`
- `Assets/CyberGuardian/Data/Difficulty/Normal.asset`
- `Assets/CyberGuardian/Data/Difficulty/Hard.asset`

Fungsi pilihan kesulitan:

- Starting shield/HP.
- Damage jawaban salah.
- Damage musuh/trap.
- Kecepatan/tekanan boss.
- Reward quiz.

Alur UI saat ini: tombol difficulty tidak berada di main menu. Tombol `MULAI` membuka scene `CyberGuardian_PilihKesulitan`, lalu pilihan `MUDAH`, `NORMAL`, atau `SULIT` disimpan ke `PlayerPrefs` sebelum Level 01 dimulai.

## 19. Story Utama

Premis:

Sebuah jaringan pendidikan digital diserang malware bernama `Digital Overlord`. Sistem belajar, data siswa, dan modul keamanan terkunci di dalam dunia komputer. Player berperan sebagai Cyber Guardian, avatar keamanan yang dikirim untuk memperbaiki firewall, membersihkan malware, dan mengajari user prinsip keamanan cyber melalui tindakan langsung.

Tema edukasi:

- Password kuat.
- Phishing dan lampiran mencurigakan.
- Malware dan file tidak dikenal.
- Firewall dan network safety.
- Privasi data seperti OTP, password, NIK.
- Backup dan update sistem.

Gaya penyampaian story:

- Overlay singkat dan jelas.
- Muncul saat masuk zona penting.
- Hilang otomatis.
- Tidak memakai bingkai besar sepanjang permainan.

## 20. Story Per Level

### Level 01 - Data Forest

Tujuan:

- Mengenalkan movement, jump, melee, ranged attack, boost, checkpoint, quiz block, dan boss slingshot.

Story:

Cyber Guardian masuk ke Data Forest, area memori yang menyimpan kebiasaan login pengguna. Malware kecil mulai meniru file aman dan memasang block password palsu. Player harus membersihkan jalur, belajar bahwa password harus panjang, unik, dan tidak dipakai ulang, lalu membuka firewall gate menuju boss pertama.

Boss:

- Firewall Virus.
- Dilindungi quiz wall.
- Fokus kategori password, malware dasar, dan privacy.

### Level 02 - Server Ruins

Tujuan:

- Memperpanjang platforming, memperkenalkan jalur bercabang, turret, laser, trap ayun, dan pilihan jalur.

Story:

Cyber Guardian tiba di Server Ruins, tempat data backup dan routing network rusak. Malware memasang koneksi palsu dan jebakan update palsu. Player harus membaca tanda bahaya, memilih jalur aman, memakai boost dengan hemat, dan menghancurkan quiz block yang mengajarkan firewall, phishing, update, dan file attachment.

Boss:

- Data Reaper.
- Shield lebih rapat.
- Boss menyerang melalui celah.
- Quiz block lebih banyak dan trap arena lebih aktif.

### Level 03 - Code Abyss

Tujuan:

- Level lebih panjang, lebih vertikal, lebih menantang, dan boss udara super besar.

Story:

Cyber Guardian masuk ke Code Abyss, inti sistem yang sudah dipenuhi kode rusak. Digital Overlord mengunci modul keamanan dengan orbit quiz block. Setiap jawaban benar memulihkan bagian sistem, tetapi setiap kesalahan membuka celah serangan balik. Player harus bergerak cepat, membaca jalur, menghindari serangan boss udara, dan menciptakan celah untuk projectile final.

Boss:

- Digital Overlord.
- Terbang/di udara.
- Super besar.
- Quiz block mengorbit boss.
- Serangan volley, laser/packet, dan projectile multi arah.

## 21. Asset yang Sudah Ada di Project

Folder utama:

- `Assets/CyberGuardian/Art/Player/CyberGuardianSprites`
- `Assets/CyberGuardian/Art/Enemies/BasicEnemySprites`
- `Assets/CyberGuardian/Art/Enemies/VirusBigPack`
- `Assets/CyberGuardian/Art/UI/CyberpunkPixelUI`
- `Assets/CyberGuardian/Art/UI/KenneySpaceUI`
- `Assets/CyberGuardian/Art/VFX/KenneyParticles`
- `Assets/CyberGuardian/Audio/SFX/KenneySciFi`
- `Assets/CyberGuardian/GeneratedBlenderAssets`

Blender/generated asset:

- `cg_guardian_player.glb`
- `cg_malware_boss.glb`
- `cg_platform_traps.glb`
- `cg_projectiles.glb`
- `cg_quiz_shield_blocks.glb`
- `cg_virus_grunt.glb`
- Sprite render hasil generator.

Script generator Blender:

- `Assets/CyberGuardian/Tools/Blender/cyber_guardian_asset_factory.py`

Catatan asset strategy:

- Untuk player dan basic enemy, sprite 2D dari user lebih mudah dikontrol untuk animasi gameplay.
- Blender/GLB cocok untuk boss, trap besar, platform 2.5D, background object, dan render sprite tambahan.
- Jangan mengganti player sprite user dengan model GLB jika animasi sprite sudah lebih jelas.

## 22. Roadmap Versi Pertama

Prioritas 1 - Stabilitas gameplay:

- Pastikan player tidak berubah scale saat bergerak.
- Pastikan player selalu bisa lompat dengan collider/ground check yang benar.
- Pastikan quiz block solid dan tidak bisa dilewati.
- Pastikan ranged projectile bisa menghancurkan quiz block dari jarak jauh.
- Pastikan boss menyerang saat boss mode.
- Pastikan slingshot selalu aktif saat boss muncul.
- Pastikan kamera mengikuti X dan Y player.

Prioritas 2 - Layout dan polish UI:

- Bersihkan main menu.
- Pastikan tombol satu baris melengkung di bawah.
- Terapkan video background.
- Buat transition start ke gameplay.
- Rapikan HUD HP/boost/score/menu.
- Boss bar hanya tampil di boss mode.
- Game over zoom dan shatter animation.

Prioritas 3 - Level design:

- Perpanjang setiap scene minimal 2x dari prototype awal.
- Tambahkan verticality: jalur naik, turun, dan loop.
- Tambahkan jalur benar/salah.
- Tambahkan trap timing.
- Tambahkan checkpoint yang adil.
- Pastikan musuh selalu berada di atas platform.

Prioritas 4 - Asset dan visual:

- Bedakan background dan foreground.
- Terapkan sprite player final.
- Terapkan sprite enemy final.
- Terapkan block quiz seperti asset pack cyber horror.
- Tambahkan VFX attack, shield, wrong answer, correct answer, boss hit.
- Tambahkan audio feedback.

Prioritas 5 - Edukasi dan story:

- Perbanyak question bank.
- Buat story overlay per zona.
- Hubungkan tiap quiz dengan rintangan nyata.
- Tambahkan pesan feedback singkat setelah jawaban.

## 23. Checklist Build/Regenerate

Saat ingin memperbarui scene dari script builder:

1. Buka Unity project `C:\Users\Wafda\My project`.
2. Tunggu import selesai.
3. Jalankan menu `Cyber Guardian/Bangun Semua Scene Game`.
4. Cek scene:
   - `CyberGuardian_MainMenu`
   - `CyberGuardian_PilihKesulitan`
   - `CyberGuardian_Level01`
   - `CyberGuardian_Level02`
   - `CyberGuardian_Level03`
5. Play dari main menu.
6. Tes `MULAI`, pilih kesulitan, `LANJUTKAN`, countdown, movement, boost/energi, attack, kuis, bos, sistem jebol, dan next scene.

## 24. Known Risk dan Hal yang Perlu Diverifikasi

- Jika Unity menampilkan `patch error`, cek Console untuk nama file dan line error.
- Jika boss tidak menyerang, cek prefab `BossPacketProjectile` dan `bossProjectileSpawn`.
- Jika quiz block bisa dilewati, cek collider block: harus `isTrigger = false` untuk obstacle solid.
- Jika player/enemy scale membesar saat bergerak, cek visual root scale dan animator.
- Jika kamera tidak mengikuti vertical movement, cek `UpdateCamera` dan batas `cameraMin/cameraMax`.
- Jika asset tampak belum berubah, jalankan ulang builder dan pastikan path asset sprite sesuai.
- Jika video menu tidak muncul, cek import video dan komponen `VideoPlayer` di scene menu.

## 25. Prinsip Lanjutan

Setiap perubahan baru harus dicek terhadap 5 pertanyaan:

1. Apakah gameplay lebih jelas dan tetap menantang?
2. Apakah edukasi cyber masuk ke aksi player, bukan hanya teks?
3. Apakah UI lebih rapi dan tidak menutupi gameplay?
4. Apakah asset foreground, enemy, trap, dan background mudah dibedakan?
5. Apakah fitur bisa diregenerasi dari script builder tanpa manual scene setup yang mudah hilang?
