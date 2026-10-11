# NDP Konseptleri - Projede Kullanılacak Teknikler

Bu dosya projedeki her NDP tekniğini, NEDEN kullanıldığını ve NASIL uygulanacağını
sade bir dille açıklamaktadır. Her bölüm ders sunumunda sözlü olarak savunulabilir
düzeyde hazırlanmıştır.

---

## 1. SINIF YAPISI (Class)

### Nedir?
Gerçek dünyadaki bir nesneyi (varlığı) yazılımda temsil eden şablondur.
Sınıf bir kalıptır; o kalıptan üretilen her şey "nesne" (object) dir.

### Projede Neden Kullanılıyor?
Randevu sistemindeki gerçek dünyadaki her varlık (hasta, doktor, randevu)
bir sınıfla temsil edilmektedir.

### Temel Sınıflar (Planlanan)
- Kullanici (User)         -> Sisteme giriş yapan herkes
- Hasta (Patient)          -> Randevu alan kişi
- Doktor (Doctor)          -> Randevu veren kişi
- Yonetici (Manager)       -> Sistemi yöneten kişi
- Randevu (Appointment)    -> Bir randevu kaydı
- Hastane (Hospital)       -> Çok şubeli kurum
- Klinik (Clinic)          -> Tek şube/bölüm
- HizmetTuru (ServiceType) -> Muayene, radyoloji vb. hizmet tanımı
- CalismaSaati (WorkHour)  -> Doktorun müsait olduğu zaman dilimi

---

## 2. KALITIM (Inheritance)

### Nedir?
Bir sınıfın başka bir sınıfın özelliklerini ve metodlarını devralmasıdır.
"is-a" ilişkisiyle açıklanır: "Hasta bir Kullanıcıdır."

### Projede Neden Kullanılıyor?
Hasta, Doktor ve Yöneticinin ortak özellikleri var (ad, soyad, email, şifre).
Bu ortak alanları tekrar yazmak yerine üst sınıftan (Kullanici) devralıyoruz.

### Hiyerarşi
Kullanici (üst sınıf)
├── Hasta          (alt sınıf) - sadece hastalara ait ek özellikler
├── Doktor         (alt sınıf) - uzmanlik alani, musait saatler
└── Yonetici       (alt sınıf) - yönettiği klinik/hastane bilgisi

### Kazanım
- Kod tekrarı (code duplication) önlenir
- Kullanıcı yönetimi tek bir noktadan yapılır
- Yeni bir rol eklemek kolaylaşır (örn: Hemşire)

---

## 3. KAPSÜLLEME (Encapsulation)

### Nedir?
Sınıfın iç verilerini (alanlarını) dışarıdan doğrudan erişime kapatmak,
sadece kontrollü erişim yolları (getter/setter) sunmaktır.

### Projede Neden Kullanılıyor?
Örneğin bir doktorun "müsait saatleri" listesi dışarıdan rastgele değiştirilmemeli.
Randevu saati de çakışma kontrolü yapılmadan set edilememelidir.

### Kurallar
- Tüm alanlar private/protected olacak
- Okuma için getter metodları
- Yazma için setter metodları (içinde doğrulama mantığı olacak)
- Kritik alanlar sadece okunabilir (read-only) olabilir

---

## 4. ÇOK BİÇİMLİLİK (Polymorphism)

### Nedir?
Aynı isimli metodun, farklı sınıflarda farklı davranış göstermesidir.
"Aynı mesaj, farklı tepki."

### Projede Neden Kullanılıyor?
"randevuAl()" metodu Hasta için farklı, Doktor için farklı çalışır.
Veya "bildirimGonder()" metodu email, SMS, sistem bildirimi olarak
farklı alt sınıflarda farklı uygulanabilir.

### Uygulama Yöntemleri
a) Override (Geçersiz Kılma): Alt sınıf üst sınıfın metodunu yeniden yazar
b) Overload (Aşırı Yükleme): Aynı metod adı, farklı parametreler

---

## 5. SOYUTLAMA (Abstraction)

### Nedir?
Karmaşık implementasyon detaylarını gizleyip sadece önemli arayüzü
kullanıcıya sunmaktır. Abstract sınıf ve interface ile sağlanır.

### Projede Neden Kullanılıyor?
"IBildirimServisi" interface'i tanımlanır. Bunun email veya SMS ile
nasıl çalıştığı sistemi kullanan kodu ilgilendirmez.
Sistemi kullanan kod sadece "bildirimGonder()" der.

### Planlanan Interface'ler
- IBildirimServisi    -> bildirimGonder() metodu
- IRandevuServisi     -> randevuOlustur(), randevuIptalEt(), musaitSaatlerGetir()
- IKullaniciServisi   -> girisYap(), cikisYap(), kayitOl()

---

## 6. DESIGN PATTERNS (Tasarım Desenleri)

### Singleton Pattern
Kullanım: Veritabanı bağlantısı veya uygulama konfigürasyonu için
Amaç: Bir sınıftan sadece 1 nesne oluşturulmasını garantilemek

### Factory Pattern
Kullanım: Kullanici nesnesi oluştururken (Hasta mı, Doktor mu, Yönetici mi?)
Amaç: Nesne oluşturma mantığını merkezi bir noktada toplamak

### Observer Pattern
Kullanım: Randevu onaylandığında / iptal edildiğinde bildirim göndermek
Amaç: Bir olay gerçekleştiğinde ilgili tüm tarafları otomatik haberdar etmek

---

*Not: Her pattern proje kodunda tek tek açıklanacak ve savunulacaktır.*
