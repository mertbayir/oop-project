# Analiz (kaynak: yalnızca ProjeOdevi_Son.docx)

Bu dosya ödev metninden sıfırdan çıkarıldı. "Karar" yazan yerler bizim tercihimizdir,
"Ödev" yazan yerler hocanın istediğidir. İkisi karıştırılmayacak.

## Teknoloji (Karar)
C# (.NET), WinForms, SQLite. Repo: https://github.com/mertbayir/oop-project

## Ödevin açıkça istedikleri

### NDP ilkeleri (Ödev: "sınıf yapısı, kalıtım, çok biçimlilik, kapsülleme vb.")
1. Sınıf yapısı
2. Kalıtım
3. Çok biçimlilik
4. Kapsülleme

(Soyutlama metinde yok; "vb." altında. Kullanırsak bonus, ama önce bu dördü savunulmalı.)

### Kurum
- Hastaneler/klinikler **yetki çerçevesinde** tanımlanabilir (herkes tanımlayamaz).
- Her klinik/hastanenin **çalışma saatleri** ve **bölümleri** tanımlanır.
- Kurum **sağlık hizmetleri** sunar (genel muayene, radyoloji, laboratuvar). Her hizmetin **süresi** ve **ücreti** tanımlanır.
- Hem **tek klinik/hastane** hem **çok şubeli** kuruluş çalışabilmeli.

### Çalışan
- **Doktor ve sağlık personeli** kayıt edilir (yani doktor dışında personel de var).
- Doktorun **uzmanlık alanı** ve **sunduğu hizmetler** tanımlanır.
- Doktorların **uygunluk zaman dilimleri** tanımlanır; hasta **sadece** bu aralıklara randevu alabilir.

### Randevu
- Kullanıcı uygun **çalışan** ve **işlem** seçerek randevu oluşturur.
- Oluşturulurken **çakışma kontrolü** yapılır.
- Kaydedilecek: işlem, süre, ücret, çalışan, tarih/saat.
- **Onay mekanizması** olur.

### Roller
- "En az üç rol önerilir": Hasta, Doktor/Sağlık Personeli, Hastane/Klinik Yöneticisi.
- Fazla rol sorun değil (hoca "en az" demiş).

### Teslim
- GitHub, en az 10 commit, farklı günlerde. Sabis'e yükleme.
- Proje bireysel veya en fazla 2 kişi.
- Sunum son 4 hafta (7. haftadan itibaren). Sunuma gelmeyen ve dosya yüklemeyen puan alamaz.
- Seçim en geç 13 Ekim 2026.

## Rol önerisi (Karar bekliyor)

| Rol | Ne yapar | Neden ayrı |
|-----|----------|------------|
| Hasta | Randevu alır, görür, iptal eder | Ödev |
| Doktor | Uygunluk saatlerini yönetir, randevuları onaylar/reddeder | Ödev |
| Sağlık Personeli | Kendi hizmetleri için aynı işler (hemşire, teknisyen) | Ödev: "doktor ve sağlık personeli" |
| Kurum Yöneticisi | Kendi kurumunu yapılandırır, personel ve hizmet yönetir | Ödev |
| Sistem Yöneticisi (opsiyonel) | Kurumları tanımlar, yönetici atar | Ödev: "yetki çerçevesinde tanımlanabilecek" |

## Alınan kararlar
1. **Personel modeli:** Doktor ve Sağlık Personeli ortak bir soyut üst sınıftan (`Calisan`) türer.
   Neden: ödev "doktor ve sağlık personeli" diyor; ortak alanlar (hizmetler, uygunluk saatleri) tekrar yazılmaz.
2. **Randevu çalışanla bağlanır** (`Randevu` -> `Calisan`, doktora özel değil).
   Neden: ödev randevu detayında "çalışan" diyor; radyoloji/laboratuvar gibi hizmetleri personel de verebilir.

3. **Kuruluş yapısı:** `Kurum` + `Sube` listesi. Tek klinik = tek şubeli kurum.
   Neden: ödev hem tek klinik hem çok şubeli yapıyı istiyor; tek yapı iki durumu da özel durum yazmadan çözer.

4. **Bölüm ayrı sınıf** (`Bolum`). Şube bölümlerden oluşur; `Hizmet` (ad, süre, ücret) bir bölüme bağlıdır.
   Bir çalışan sadece kendi bölümündeki hizmetleri sunabilir (`HizmetEkle` içinde doğrulama).
   Neden: ödevdeki "bölümler" ve "hizmetler" birbirine bağlanır; kapsülleme için doğrulama örneği olur.

5. **Onayı randevunun çalışanı verir** (Doktor veya Sağlık Personeli). `RandevuOnayla()` / `RandevuReddet()`
   `Calisan` sınıfında olur, iki alt sınıf miras alır.
   Neden: kendi takvimini en iyi çalışan bilir; mantık tek yerde yazılır.

6. **Slotu BEKLEMEDE ve ONAYLANDI randevular tutar.** İPTAL ve TAMAMLANDI tutmaz.
   Neden: aksi halde iki hasta aynı saate başvurabilir ve çalışan birini reddetmek zorunda kalır.

## Açık sorular
Yok. Sıradaki iş: bu kararlara göre sınıf tasarımını (alanlar, metodlar, ilişkiler) çıkarmak.
