# Savunma Notları (Hoca Soruları ve Cevaplar)

Her sınıf yazıldıkça bu dosya büyür. Amaç: kodu ezberlemek değil, "neyi neden yaptık" cümlelerini hazır tutmak.

## 1. Dört kavram, tek bakışta

| Kavram | Tek cümle | Projede nerede |
|---|---|---|
| Sınıf yapısı | Gerçek dünyadaki varlığı koda dönüştüren şablon | Kullanici, Hasta, Calisan, Doktor, Hizmet, Bolum, Sube, Kurum, ... |
| Kalıtım | "X bir Y'dir" ilişkisiyle ortak özellikleri üst sınıftan almak | Kullanici -> Hasta / Calisan / KurumYoneticisi / SistemYoneticisi; Calisan -> Doktor / SaglikPersoneli |
| Kapsülleme | Veriyi sınıfın içinde saklayıp erişimi doğrulamalı yollarla vermek | private alanlar, doğrulamalı property'ler, IReadOnlyList, _sifreHash |
| Çok biçimlilik | Aynı çağrının nesnenin gerçek tipine göre farklı çalışması | RolAdi, YetkiVarMi, ToString |

Soyutlama ödev metninde yok ("vb." altında), bonus olarak: abstract Kullanici ve Calisan.

## 2. Sık sorulabilecek sorular

**Kalıtım mı kompozisyon mu? Nasıl ayırırsın?**
"X bir Y'dir" diyebiliyorsak kalıtım (Doktor bir Kullanici'dir). "X bir Y'ye sahiptir" diyorsak kompozisyon (Calisan bir Bolum'e sahiptir, Bolum hizmetlere sahiptir).

**Kullanici neden abstract?**
`new Kullanici()` yazılabilseydi "kimliği ve yetkisi belli olmayan" bir nesne oluşurdu. Abstract ile "Hasta, Çalışan veya Yönetici olmadan kullanıcı olmaz" kuralını derleyiciye zorlattık. Ortak sözleşmeyi (RolAdi, YetkiVarMi) tanımlar, detayı alt sınıfa bırakır.

**Kapsülleme ile soyutlama arasındaki fark ne?**
Kapsülleme: "Veriye kim, nasıl ulaşır?" (private _sifreHash + SifreDogrula). Soyutlama: "Ortak sözleşme ne, detay kimde?" (abstract RolAdi, YetkiVarMi).

**Çok biçimliliği nerede gösterirsin?**
Aynı döngü, tipi Kullanici olan bir listede dolaşır; her nesne kendi RolAdi'ni, kendi yetkisini ve kendi ToString'ini verir. Döngüde hiç `if (rol == ...)` yok.

**ToString nereden geliyor?**
Her C# sınıfı gizlice `object`'ten türer; ToString, Equals, GetHashCode oradan gelir. `override` ile kendi davranışımızla ezeriz.

**Calisan abstract ama RolAdi'ni yazmadı, neden derleniyor?**
Soyut sınıf soyut üyelerin hepsini doldurmak zorunda değil. İlk somut alt sınıf (Doktor, SaglikPersoneli) doldurmak zorunda. Calisan YetkiVarMi'yi kendisi doldurdu (iki alt sınıfın yetkisi aynı), RolAdi'ni bıraktı.

**private ve protected farkı?**
private: sadece o sınıf. protected: o sınıf ve alt sınıfları. public: herkes. Kullanici'nın constructor'ı protected, çünkü alt sınıf `: base(...)` ile çağırır. _sifreHash bilerek private, alt sınıflar bile dokunamasın.

**Id neden sadece get?**
Id oluştuktan sonra değişmemeli. Eşsizliğini ise veritabanı (otomatik artan birincil anahtar) garanti eder; get-only bunu sağlamaz.

**Email setter'ında neden throw var?**
Bozuk nesne hiç oluşmasın diye (fail fast). throw olmasa email="abc" ile geçerli görünen ama bozuk bir kullanıcı oluşurdu, hata çok sonra çıkardı.

**Şifre güvenli mi?**
SHA-256 hash'i saklıyoruz, düz metin değil. Gerçek sistemde salt'lı yavaş hash (PBKDF2/bcrypt/Argon2) kullanılır; ödev için sade tuttuk.

**Liste neden IReadOnlyList döndürüyor?**
Dışarıdan `Add/Clear` yapılamasın, doğrulamamız atlanmasın. Hata çalışırken değil derleme sırasında çıkar, çünkü arayüzde bu metodlar yok.

**Aynı hizmet neden iki kez eklenemiyor?**
Hasta ekranında çift görünür, süre/ücret çakışabilir, silme tutarsızlık yaratır. Not: kontrol Id'ye bakar, aynı adlı farklı Id'li hizmet geçebilir (bilinen sınır).

**decimal mi double mı, neden?**
Para için decimal: double ikilik tabanda ondalığı tam tutamaz (0.1 + 0.2 != 0.3).

**TimeSpan ve TimeOnly neden?**
TimeSpan: "30 dakika" gibi süre, saat+süre toplanabilir. TimeOnly: sadece saat; DateTime kullansak anlamsız bir tarih alanı gelirdi. DayOfWeek hazır enum olduğu için kendimiz yazmadık.

**UygunlukAraligi neden değişmez (immutable)?**
Doğrulama sadece constructor'da yapılır ve yetmez; geçersiz ara durum (bitiş <= başlangıç) hiç oluşamaz. Değişecekse eskisi silinir, yenisi eklenir.

**Çakışma kontrolü nasıl çalışır?** (projenin kalbi)
```
A.Baslangic < B.Bitis  &&  B.Baslangic < A.Bitis
```
Biri diğerinin bitişinden önce başlıyorsa ve diğeri de ilkinin bitişinden önce başlıyorsa örtüşür. Sınırlar dışarıda: 09-12 ile 12-14 çakışmaz (`<`, `<=` değil).

**Kapsama kontrolü (UygunMu) neden `<=`?**
Sınırlar dahil: günün ilk randevusu (09:00 başlangıç) ve son randevusu (17:00 bitiş) alınabilsin. `<` yazsak ikisi de reddedilirdi. Çakışmada `<`, kapsamada `<=`, bilinçli fark.

**İki aralığı birlikte kapsayan randevu alınabilir mi?**
Hayır. UygunMu her aralığa tek tek sorar (Any). Bilinen bir sadelik; 11:45-12:15 randevusu 09-12 ve 12-17 bitişik olsa bile reddedilir.

**Neden Electron değil de WinForms?**
Odak NDP. Electron'da JS'te gerçek private/abstract/interface yok, kodun çoğu HTML/IPC olur, savunması zor.

**Neden Doktor yanında SaglikPersoneli var, Randevu neden Calisan'a bağlı?**
Ödev "doktor ve sağlık personeli" ve randevu detayında "çalışan" diyor. Radyoloji/laboratuvar hizmetini personel de verebilir. Yeni rol eklemek = yeni bir alt sınıf, mevcut dosyalara dokunmadan.

**Tek klinik ile çok şubeli kurumu nasıl destekliyorsun?**
Kurum -> Sube listesi. Tek klinik, tek şubeli bir kurumdur; ek sınıf ve özel durum yok.

**Neden bu kısıtlar var? (Hizmeti sadece kendi bölümünden eklemek, çalışanı sadece şubenin bölümüne eklemek)**
Tutarsız veri girişini kapsülleme ile sınıfın içinde engellemek: bölümü olmayan hizmeti çalışan sunamaz, başka şubenin bölümündeki çalışan bu şubeye eklenemez.

## 2b. Randevu ve teknik temel (C# bellek modeli)

**`public Hasta Hasta { get; }` ne demek?**
İlk Hasta tip, ikinci Hasta isim ("bu randevunun hastası"). C# bunu izin verir. `{ get; }` salt okunur property; değeri sadece constructor'da atanır.

**Referans mı işaretçi mi?**
Sınıf tipindeki değişken nesnenin kendisini değil, bellekteki yerine giden güvenli bir referansı tutar. İşaretçiden farkı: aritmetik yok, geçersiz adrese erişilemez, çöp toplayıcı (GC) nesneyi kendisi siler. Değer tipleri (int, decimal, DateTime, TimeSpan, enum) değerin kendisini tutar; kopyalayınca değer kopyalanır. Referans tipleri (class) kopyalayınca adres kopyalanır.

**`var h2 = h1; h2.Telefon = "999";` sonrası h1.Telefon?**
"999". İkisi de aynı nesneyi gösteriyor, nesne tek.

**Aynı hasta DB'den iki kez yüklenirse `h1 == h2` neden false olabilir?**
Sınıflarda `==` varsayılan olarak adresleri karşılaştırır. İki ayrı nesne olur, Id'leri aynı olsa da uygulama onları farklı kişi sanır. Bu yüzden karşılaştırmayı Id üzerinden yapıyoruz (`hasta.Id != Hasta.Id`).

**Randevu neden Sure ve Ucret'i kopyalıyor?**
Ödev "randevu detayları (işlem, süre, ücret...) kaydedilecek" diyor. decimal/TimeSpan değer tipi olduğu için kopyalanır; hizmetin ücreti sonradan değişse geçmiş randevu değişmez. `Hizmet` ise referans olduğundan `randevu.Hizmet.Ucret` güncel fiyatı gösterir, `randevu.Ucret` randevu anındaki fiyatı.

**null nedir, neden `ThrowIfNull`?**
Referans hiçbir nesneyi göstermiyorsa null'dır; üzerinden üye çağırınca NullReferenceException çıkar. Hatayı en başta, anlaşılır mesajla yakalamak için constructor'larda kontrol ediyoruz.

**Randevunun durumu nasıl korunuyor?**
`Durum { get; private set; }`: dışarıdan atanamaz. Tek yol Onayla / Reddet / IptalEt / Tamamla; hepsi `GecisYap` üzerinden geçer: yeni durum ancak izin verilen önceki durumlardan gelinebilir. Geçişler: Beklemede -> Onaylandi | Iptal; Onaylandi -> Tamamlandi | Iptal. İptal ve Tamamlandi son durumlardır.

**Yetki randevunun içinde nasıl?**
Onayla/Reddet/Tamamla sadece randevunun kendi çalışanından, IptalEt sadece randevunun kendi hastasından kabul edilir (Id karşılaştırması). Sıra: önce kim yapıyor (yetki), sonra durum uygun mu.

**Randevu çakışması nasıl bulunur?**
Aynı çalışan VE ikisi de slotu tutuyor (Beklemede/Onaylandi) VE `A.Baslangic < B.Bitis && B.Baslangic < A.Bitis`. `&&` kısa devre yapar; farklı çalışanlarda zaman karşılaştırması hiç çalışmaz. 10:00-10:30 ile 10:30-11:00 çakışmaz.

**Constructor'da hangi kurallar kontrol ediliyor?**
Çalışan hizmeti sunuyor mu; randevu gece yarısını aşmıyor mu (TimeOnly 24 saatte başa sarar, bit <= bas olur); çalışanın uygunluk aralığına sığıyor mu. Kural dışı randevu hiç var olmaz.

**Bilinen sınırlar (dürüstçe söylenecek):**
- "Yeni randevu mevcut tüm randevularla çakışıyor mu" kontrolü Randevu içinde değil, tüm randevuları bilen servis katmanında yapılacak.
- Geçmiş tarihe randevu engeli serviste olacak.
- Constructor kuralları DB'den eski randevu yüklerken sorun çıkarabilir (çalışanın uygunluğu sonradan değiştiyse); SQLite aşamasında ele alınacak.

## 2c. Servis katmanı ve isimlendirme

**RandevuServisi ne işe yarar? Neden ayrı bir sınıf?**
Tek bir randevu sadece kendi kurallarını bilir (yetki, durum geçişi, uygunluğa sığma). "Bu çalışanın bu saatte başka randevusu var mı?" sorusu tüm randevuları bilmeyi gerektirir; bunu bilen katman servistir. Servis randevu listesini tutar ve çakışma, geçmiş tarih, boş slot gibi kuralları uygular. Kişisel kuralları Randevu'ya bırakır.

**RandevuOlustur nasıl çalışır?**
1) Zaman geçmişte mi? 2) Randevu nesnesi oluşturulur (kendi kuralları constructor'da kontrol edilir). 3) Listedeki randevulardan biri yenisiyle çakışıyor mu? 4) Hepsi tamamsa sayaç artırılır, listeye eklenir. Numarayı kontrollerden önce artırmıyoruz (`_sonId + 1`), böylece hatalı denemeler numara tüketmez.

**Boş slotlar nasıl hesaplanır?**
Çalışanın o günkü uygunluk aralığı, hizmet süresi kadar adımlarla taranır (09:00, 09:30, ...). Her aday için slotu tutan (Beklemede/Onaylandi) bir randevuyla örtüşüp örtüşmediğine bakılır; örtüşmeyen ve geçmişte kalmayanlar listeye eklenir. Uygunluk aralığı sadece saat (TimeOnly), randevu ise tam tarih (DateTime) tuttuğu için karşılaştırmadan önce `tarih.Date + saat` ile birleştirilir. Örnek: 09-11 aralığı, 30 dk hizmet, 09:30-10:00 dolu -> 09:00, 10:00, 10:30.

**Neden ayrı bir depo (repository) katmanı yok?**
İlk tasarımda IRandevuDeposu vardı; kaldırdık. Ödev için zorunlu değil, "ileride lazım olur" diye eklenmişti ve anlaşılırlığı düşürüyordu. Gereksiz soyutlama eklememek de bir tasarım kararıdır. Soyutlama örneklerimiz zaten var: abstract Kullanici/Calisan, IRandevuServisi, ilerde IBildirimServisi. SQLite eklenirken ihtiyaç somutlaşınca ele alınır.

**IRandevuServisi neden var?**
Arayüz (ekranlar) servisin nasıl çalıştığını değil, ne yapabildiğini bilir. Ekran "randevu oluştur" der, kuralları servis uygular.

**Neden namespace `HastaneRandevu`, `Randevu` değil?**
Proje `Randevu.Core` adındayken "Randevu" hem en üst namespace hem de Models içindeki sınıf adıydı. Derleyici bir ismi ararken en üstteki namespace'i bulup durur, `using` ile getirilen sınıfa ulaşmaz ("'Randevu' bir ad alanı öğesidir ancak tür olarak kullanılır"). Geçici çözüm `Models.Randevu` yazmaktı. Kalıcı çözüm olarak kök namespace `HastaneRandevu` yapıldı; artık `Randevu` yazmak yeter.

**IReadOnlyList / ToList neden?**
Dışarıya listenin kendisini değil salt okunur bir görünüm veya kopya veriyoruz; dışarıdan Add/Clear yapılamaz.

## 3. Alınan tasarım kararları (özet)
1. Calisan soyut üst sınıf -> Doktor, SaglikPersoneli
2. Randevu Calisan'a bağlı
3. Kurum + Sube (tek klinik = tek şubeli kurum)
4. Bolum ayrı sınıf, Hizmet bölüme bağlı; çalışan sadece kendi bölümünün hizmetini sunar
5. Randevuyu o çalışan onaylar
6. BEKLEMEDE ve ONAYLANDI randevular slotu tutar; IPTAL ve TAMAMLANDI tutmaz
