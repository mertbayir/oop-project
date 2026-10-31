# Hastane / Klinik Randevu Sistemi

Nesneye Dayalı Programlama (Yüksek Lisans) dersi ödevi, **Seçenek A: Hastane/Klinik Randevu Sistemi Geliştirme**.

Amaç, nesneye dayalı programlama ilkelerini (sınıf yapısı, kalıtım, çok biçimlilik, kapsülleme, soyutlama) somut bir yazılımda göstermektir. Uygulama hem **tek bir klinik** hem de **çok şubeli bir kurum** için çalışır: tek klinik, tek şubeli bir kurum olarak tanımlanır.

## Neler yapabiliyor?

| Rol | Yapabildikleri |
|---|---|
| **Hasta** | Kayıt olur, bölüm/hizmet/çalışan/tarih seçerek boş saatleri görür, randevu alır, kendi randevusunu iptal eder |
| **Doktor / Sağlık personeli** | Gelen randevuları onaylar, reddeder, tamamlar; kendi uygunluk saatlerini ekler |
| **Kurum yöneticisi** | Kendi kurumunun şubelerini, çalışma saatlerini, bölümlerini, hizmetlerini (süre ve ücret) ve çalışanlarını (uzmanlık alanı, sunduğu hizmetler) tanımlar |
| **Sistem yöneticisi** | Yeni kurum tanımlar ve kuruma yönetici atar |

Kurallar:
- Hasta **yalnızca** çalışanın uygunluk aralığına sığan saatlere randevu alabilir.
- Aynı çalışanın aynı saatte iki randevusu olamaz (bekleyen ve onaylı randevular saati tutar, iptal edilenler tutmaz).
- Randevuyu o randevunun çalışanı onaylar; hasta yalnızca kendi randevusunu iptal edebilir.
- Çalışan yalnızca kendi bölümündeki hizmetleri sunabilir; uygunluk saati, çalıştığı şubenin açık olduğu saatler içinde kalmalıdır.
- Her rol yalnızca kendi yetkisi olan işleri yapabilir.

## Teknoloji

- **C# / .NET 8**, arayüz için **Windows Forms**
- Veritabanı **yoktur**: veriler bellekte tutulur, program açılırken örnek verilerle doldurulur (ödev veri saklama istemediği için bilinçli bir karar). Program kapanınca veriler gider.

## Çalıştırma

Gereksinim: Windows ve .NET 8 SDK (veya daha yenisi).

```bash
# Arayüzlü uygulama
dotnet run --project HastaneRandevu.App

# Konsol demosu (dört OOP kavramını ve tüm randevu akışını ekrana yazdırır)
dotnet run --project HastaneRandevu.Demo
```

### Örnek hesaplar (hepsinin şifresi `1234`)

| E-posta | Rol |
|---|---|
| `ali@mail.com` | Hasta |
| `ayse@hastane.com` | Doktor (Kardiyoloji) |
| `veli@hastane.com` | Sağlık personeli (Kardiyoloji, EKG) |
| `mehmet@hastane.com` | Doktor (Dahiliye) |
| `zeynep@hastane.com` | Kurum yöneticisi |
| `admin@hastane.com` | Sistem yöneticisi |

Bunlar yalnızca gösterim için üretilen örnek hesaplardır.

## Proje yapısı

```
HastaneRandevu.Core/      Modeller ve iş mantığı (arayüzden bağımsız)
  Models/                   Kullanici, Hasta, Calisan, Doktor, SaglikPersoneli, KurumYoneticisi,
                            SistemYoneticisi, Kurum, Sube, Bolum, Hizmet, UygunlukAraligi, Randevu
  Enums/                    Yetki, RandevuDurumu
  Services/                 RandevuServisi, KurumYonetimServisi, KullaniciServisi, Yetkilendirme
HastaneRandevu.App/       Windows Forms arayüzü (iş kuralı içermez, servisleri çağırır)
HastaneRandevu.Demo/      Konsol demosu
docs/                     Analiz ve savunma notları
```

Sınıf hiyerarşisi:

```
Kullanici (abstract)
 ├─ Hasta
 ├─ Calisan (abstract)
 │    ├─ Doktor
 │    └─ SaglikPersoneli
 ├─ KurumYoneticisi
 └─ SistemYoneticisi

Kurum ─< Sube ─< Bolum ─< Hizmet        (kompozisyon)
Calisan ─< UygunlukAraligi
Randevu ──> Hasta, Calisan, Hizmet
```

## OOP kavramları projede nerede?

| Kavram | Nerede |
|---|---|
| **Sınıf yapısı** | Her varlık ayrı bir sınıf: `Hasta`, `Doktor`, `Randevu`, `Hizmet`, `Sube`... |
| **Kalıtım** | `Kullanici` → `Hasta` / `Calisan` / yöneticiler; `Calisan` → `Doktor` / `SaglikPersoneli` (iki seviyeli). Arayüzde `PanelFormu` → `HastaPaneli`, `CalisanPaneli`, `YoneticiPaneli`, `SistemPaneli` |
| **Kapsülleme** | `private` alanlar ve doğrulamalı property'ler (`Kullanici.Ad`, `Email`), hash'li şifre (dışarıdan okunamaz), `Randevu.Durum` için `private set`, `IReadOnlyList` ile korunan listeler, değişmez `UygunlukAraligi` |
| **Çok biçimlilik** | `YetkiVarMi` ve `RolAdi` her rolde farklı çalışır (override); servis kullanıcının türünü bilmeden yetkiyi sorar; `ToString` ezmeleri arayüz listelerinde kullanılır; `KurumTanimla` aşırı yüklemesi (overload) |
| **Soyutlama** | `Kullanici` ve `Calisan` soyut sınıf; `IRandevuServisi` arayüzü; `PanelFormu` soyut taban sınıf |

Ek olarak erişim belirleyiciler bilinçli seçilmiştir: `protected` yapıcılar, `internal` yardımcı sınıf (`Yetkilendirme`), `private set`.

## Tasarım kararları ve savunma notları

Ayrıntılar `docs/` klasöründedir:

- [`docs/00_ANALIZ.md`](docs/00_ANALIZ.md): ödev metninden çıkarılan gereksinimler ve alınan tasarım kararları
- [`docs/01_SAVUNMA_NOTLARI.md`](docs/01_SAVUNMA_NOTLARI.md): "neden böyle yaptık" soru-cevap notları

## Bilinen sınırlar

- Veritabanı yok, veriler kalıcı değildir.
- Saati gelmemiş bir randevu da "tamamlandı" yapılabilir (geçmişe randevu alınamadığı için bu kuralı eklemek gösterimi imkânsız kılardı).
- Çalışan ve randevu silme yoktur.
- Otomatik test projesi yoktur.
