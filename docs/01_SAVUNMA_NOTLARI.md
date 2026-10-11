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

## 3. Alınan tasarım kararları (özet)
1. Calisan soyut üst sınıf -> Doktor, SaglikPersoneli
2. Randevu Calisan'a bağlı
3. Kurum + Sube (tek klinik = tek şubeli kurum)
4. Bolum ayrı sınıf, Hizmet bölüme bağlı; çalışan sadece kendi bölümünün hizmetini sunar
5. Randevuyu o çalışan onaylar
6. BEKLEMEDE ve ONAYLANDI randevular slotu tutar; IPTAL ve TAMAMLANDI tutmaz
