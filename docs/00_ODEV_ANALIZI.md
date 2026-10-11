# NDP Dersi - Ödev Analizi ve Proje Planı

## Ödev Genel Bilgileri

| Alan | Detay |
|------|-------|
| **Ders** | Nesneye Dayalı Programlama (NDP) - Yüksek Lisans |
| **Seçilen Seçenek** | Seçenek A — Hastane/Klinik Randevu Sistemi Geliştirme |
| **Grup** | Bireysel veya 2 kişi (max) |
| **Seçim Son Tarihi** | 13 Ekim 2026 |
| **Sunum Tarihi** | 7. Haftadan itibaren (duyurulacak) |
| **Platform** | GitHub (min. 10 commit, farklı günlerde) + Sabis |

---

## Seçilen Proje: Hastane/Klinik Randevu Sistemi

### Temel Amaç
Platform-bağımsız, nesneye dayalı programlama konsepti dahilinde bir Hastane/Klinik Randevu Sistemi geliştirmek. Uygulama hem **tek bir klinik/hastane** için hem de **çok şubeli sağlık kuruluşları** için çalışabilir olmalıdır.

---

## Ödev Kapsamı (Detaylı Analiz)

### 1. Hastane/Klinik Tanımlamaları
- Hastaneler veya klinikler yetki çerçevesinde tanımlanabilecek
- Her klinik/hastanenin çalışma saatleri ve bölümleri tanımlanabilecek
- Kurumun sunduğu sağlık hizmetleri (örn: genel muayene, radyoloji, laboratuvar)
- Her hizmetin süresi ve ücreti detaylı tanımlanabilecek

### 2. Çalışan Yönetimi
- Doktor ve sağlık personeli sisteme kayıt edilebilecek
- Her doktorun uzmanlık alanı ve sunduğu hizmetler tanımlanabilecek
- Doktorların uygunluk zaman dilimleri sisteme tanımlanacak
- Hastalar sadece bu zaman aralıklarına randevu alabilecek

### 3. Randevu Sistemi
- Kullanıcılar uygun çalışan ve işlem seçerek randevu oluşturabilecek
- Randevu oluştururken çakışma kontrolü yapılacak
- Randevu detayları (işlem, süre, ücret, çalışan, tarih/saat) kaydedilecek
- Randevu için onay mekanizması olacak

### 4. Kullanıcı Rolleri (En az 3 rol)
| Rol | Açıklama |
|-----|----------|
| **Hasta** | Randevu alır, randevularını görür/iptal eder |
| **Doktor / Sağlık Personeli** | Uygunluk zamanlarını yönetir, randevuları görür |
| **Hastane / Klinik Yöneticisi** | Sistemi yapılandırır, personeli yönetir |

---

## NDP Gereksinimleri (Hocanın Soracakları)

| Konsept | Projede Kullanım Yeri |
|---------|----------------------|
| **Sınıf Yapısı** | Hospital, Clinic, Doctor, Patient, Appointment vb. sınıflar |
| **Kalıtım (Inheritance)** | User -> Patient, Doctor, Manager hiyerarşisi |
| **Çok Biçimlilik (Polymorphism)** | Farklı kullanıcı rollerinin aynı arayüzden farklı davranması |
| **Kapsülleme (Encapsulation)** | Private alanlar, getter/setter metodlar |
| **Soyutlama (Abstraction)** | Abstract sınıflar ve interface'ler |

---

## Teslim Gereksinimleri

- GitHub reposu (min. 10 commit, farklı günlerde)
- Sabis sistemine yükleme
- Son 4 haftada sunum yapılacak
- Sunum yapılmadan puan alınamaz
- Dosya yüklenmeden sunuma katılınamaz

---

## Docs Klasörü Yapısı (Planlanan)

docs/
├── 00_ODEV_ANALIZI.md          <- Bu dosya
├── 01_MIMARI_VE_SINIFLAR.md    <- UML ve sinif tasarimi
├── 02_NDP_KONSEPTLERI.md       <- Kullanilan NDP tekniklerinin aciklamasi
├── 03_ALGORITMALAR.md          <- Cakisma kontrolu, randevu mantigi vb.
├── 04_KULLANICI_ROLLERI.md     <- Rol bazli erisim tasarimi
├── 05_TECH_STACK.md            <- Teknoloji kararlari ve gerekceleri
└── 06_GELISTIRME_PLANI.md      <- Adim adim gelistirme takvimi

---

## Kritik Not

Hocanin Uyarisi: Bu proje bir AI araciyla cok kolay gelistirilebilir.
Hoca bu durumun farkinda oldugu icin her detayi sorgulayacak.
Bu nedenle:
- Her fonksiyon tek tek anlasilarak yazilacak
- Her NDP teknigi neden kullanildigi bilinerek uygulanacak
- Her algoritma mantigi detayliyla not alinacak
- Cok agir adimlarla, anlayarak ilerlenecek
