# Mimari ve Sınıf Tasarımı

Bu dosya projenin nesne modelini ve sınıflar arası ilişkileri açıklamaktadır.
Her sınıf için alanlar, metodlar ve ilişkiler detaylandırılacaktır.

---

## Sınıf Hiyerarşisi (Genel Bakış)

```
Kullanici (User) [abstract/base]
├── Hasta (Patient)
├── Doktor (Doctor)
└── Yonetici (Manager)

Kurum [abstract/base]
├── Hastane (Hospital)  -> birden fazla Klinik içerebilir
└── Klinik (Clinic)     -> tek birim

HizmetTuru (ServiceType)  -> bir Klinik'e bağlı
CalismaSaati (WorkHour)   -> bir Doktor'a bağlı
Randevu (Appointment)     -> Hasta + Doktor + HizmetTuru + Zaman
```

---

## Kullanici Sınıfı (User) - Ana Sınıf

### Alanlar
- id           : int (benzersiz kimlik)
- ad           : string (zorunlu)
- soyad        : string (zorunlu)
- email        : string (benzersiz, zorunlu)
- sifre        : string (hashlenmis)
- rol          : enum {HASTA, DOKTOR, YONETICI}
- olusturulmaTarihi : datetime

### Metodlar
- girisYap(email, sifre) : bool
- sifredegistir(eskiSifre, yeniSifre) : bool
- profilGuncelle(ad, soyad) : void
- toString() : string

---

## Hasta Sınıfı (Patient) extends Kullanici

### Ek Alanlar
- tcKimlik     : string (opsiyonel)
- telefonNo    : string
- dogumTarihi  : date
- randevuListesi : List<Randevu>

### Ek Metodlar
- randevuAl(doktor, hizmet, zaman) : Randevu
- randevuIptalEt(randevuId) : bool
- randevularimiBak() : List<Randevu>
- gemisRandevular() : List<Randevu>

---

## Doktor Sınıfı (Doctor) extends Kullanici

### Ek Alanlar
- uzmanlikAlani   : string
- sunduguHizmetler : List<HizmetTuru>
- musaitZamanlar  : List<CalismaSaati>
- klinik          : Klinik (çalıştığı yer)
- randevuListesi  : List<Randevu>

### Ek Metodlar
- musaitSaatEkle(gun, baslangic, bitis) : void
- musaitSaatSil(saatId) : bool
- randevularimiBak() : List<Randevu>
- randevuOnayla(randevuId) : bool
- randevuReddet(randevuId) : bool

---

## Yonetici Sınıfı (Manager) extends Kullanici

### Ek Alanlar
- yonettigiKurum : Kurum

### Ek Metodlar
- doktorEkle(doktor) : void
- doktorCikar(doktorId) : bool
- hizmetEkle(hizmet) : void
- calismaReporuGor() : Rapor
- randevuRaporuGor(baslangic, bitis) : Rapor

---

## Randevu Sınıfı (Appointment)

### Alanlar
- id            : int
- hasta         : Hasta
- doktor        : Doktor
- hizmetTuru    : HizmetTuru
- tarihSaat     : datetime
- sure          : int (dakika cinsinden)
- ucret         : double
- durum         : enum {BEKLEMEDE, ONAYLANDI, IPTAL, TAMAMLANDI}
- notlar        : string (opsiyonel)

### Metodlar
- onayla() : void
- iptalEt() : void
- tamamla() : void
- cakisiyorMu(digerRandevu) : bool     <- KRITIK ALGORITMA
- toString() : string

---

## CalismaSaati Sınıfı (WorkHour)

### Alanlar
- id         : int
- doktor     : Doktor
- gun        : enum {PAZARTESI, SALI, ..., PAZAR}
- baslangic  : time (örn: 09:00)
- bitis      : time (örn: 17:00)
- aralikDak  : int (randevular arası boşluk, örn: 30 dk)

### Metodlar
- musaitSlotlarGetir(tarih) : List<TimeSlot>
- slotMesgulMu(tarih, saat) : bool

---

## HizmetTuru Sınıfı (ServiceType)

### Alanlar
- id       : int
- ad       : string (örn: "Genel Muayene")
- sure     : int (dakika)
- ucret    : double
- klinik   : Klinik

---

## Hastane Sınıfı (Hospital) extends Kurum

### Alanlar
- id          : int
- ad          : string
- adres       : string
- klinikler   : List<Klinik>
- yonetici    : Yonetici

### Metodlar
- klinikEkle(klinik) : void
- klinikBul(id) : Klinik
- aktifDoktorlar() : List<Doktor>

---

## Klinik Sınıfı (Clinic)

### Alanlar
- id            : int
- ad            : string
- bolum         : string (örn: "Kardiyoloji")
- calismaSaatleri : List<WorkHour> (klinik genel saatleri)
- doktorlar     : List<Doktor>
- hizmetler     : List<HizmetTuru>
- hastane       : Hospital (null ise bağımsız klinik)

---

## Sınıflar Arası İlişkiler

- Kullanici -> Hasta, Doktor, Yonetici  : Kalıtım (is-a)
- Doktor -> Klinik                       : Association (çalışır-da)
- Randevu -> Hasta, Doktor               : Association (bağlıdır)
- Randevu -> HizmetTuru                  : Association
- Klinik -> HizmetTuru                   : Composition (klinik silinince hizmetler de silinir)
- Hastane -> Klinik                      : Composition
- Doktor -> CalismaSaati                 : Composition

---

*Bu dosya kodlama başladıkça güncellenecektir. Her sınıf yazılmadan önce buradaki tanım gözden geçirilecektir.*
