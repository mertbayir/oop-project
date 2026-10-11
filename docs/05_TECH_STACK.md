# Tech Stack - Teknoloji Kararları

Bu dosya, proje için seçilen teknolojileri ve seçim gerekçelerini belgeler.

---

## KARAR (11 Ekim 2026)

| Katman | Seçim |
|--------|-------|
| Dil | **C#** (.NET) |
| Arayüz | **Windows Forms (WinForms)** |
| Veritabanı | **SQLite** (tek dosya, kurulum gerektirmez) |
| IDE | Visual Studio |
| Sürüm kontrol | Git + GitHub |

---

## Gerekçeler

### Neden C#?
- `abstract class`, `interface`, `private/protected`, `virtual/override` sözdizimi çok net
- **Property** (`get`/`set`) ile kapsülleme temiz ve savunması kolay
- Ders dili serbest; Java'ya çok yakın OOP yapısı
- Gerçek erişim belirleyicileri var (Python'daki gibi "naming convention" değil)

### Neden WinForms?
- Visual Studio'da sürükle-bırak tasarım; arayüz için az zaman harcanır
- Zaman asıl NDP katmanına (sınıflar, algoritmalar) ayrılır
- Windows'ta ek kurulum gerektirmez, sunumda çalıştırmak kolay

### Neden Electron seçilmedi?
- Ödevin odağı NDP; Electron'da kodun büyük kısmı HTML/CSS/IPC olur
- JavaScript'te gerçek `private`, `abstract`, `interface` yok
- Ağır kurulum, native SQLite modül sorunları, savunma sırasında gereksiz karmaşıklık

### Neden SQLite?
- Tek dosya, sunucu kurulumu yok
- `Microsoft.Data.Sqlite` ile doğrudan kullanılabilir

---

## Katmanlı Yapı

```
UI (WinForms)                 -> sadece ekranlar, iş mantığı YOK
Servisler (IRandevuServisi..) -> iş mantığı, RBAC, çakışma kontrolü
Modeller (Kullanici, Doktor,  -> NDP'nin kalbi (kalıtım, kapsülleme...)
          Randevu ...)
Veri (SQLite / Repository)    -> kayıt ve okuma
```

Modeller arayüzden bağımsız yazılır (UI değişse de model değişmez).

---

## Önerilen Solution Yapısı

```
NDP.sln
├── Randevu.Core      (class library: modeller, enum'lar, interface'ler)
├── Randevu.Data      (class library: SQLite repository'ler)
├── Randevu.App       (WinForms: formlar)
└── Randevu.Tests     (birim testleri, özellikle çakışma algoritması)
```

---

## Çalışma Sırası
1. Önce `Randevu.Core` (modeller + algoritmalar), konsol/test ile doğrula
2. Sonra `Randevu.Data` (SQLite)
3. En son WinForms ekranları
