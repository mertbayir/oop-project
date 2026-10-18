using System.Text;
using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;
using HastaneRandevu.Core.Services;

Console.OutputEncoding = Encoding.UTF8;

// ---------- Örnek veri ----------
var kurum = new Kurum(1, "Merkez Hastanesi");
var sube = new Sube(1, "Merkez Şube", "Sakarya");
kurum.SubeEkle(sube);

var kardiyoloji = new Bolum(1, "Kardiyoloji");
var muayene = new Hizmet(1, "Muayene", TimeSpan.FromMinutes(30), 500m);
var ekg = new Hizmet(2, "EKG", TimeSpan.FromMinutes(15), 250m);
kardiyoloji.HizmetEkle(muayene);
kardiyoloji.HizmetEkle(ekg);
sube.BolumEkle(kardiyoloji);

var doktor = new Doktor(2, "Ayşe", "Kaya", "ayse@hastane.com", "1234", kardiyoloji, "Kardiyolog");
var hemsire = new SaglikPersoneli(3, "Veli", "Demir", "veli@hastane.com", "1234", kardiyoloji, "Hemşire");
doktor.HizmetEkle(muayene);
hemsire.HizmetEkle(ekg);
doktor.UygunlukEkle(new UygunlukAraligi(1, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)));
sube.CalisanEkle(doktor);
sube.CalisanEkle(hemsire);

var ali = new Hasta(1, "Ali", "Yılmaz", "ali@mail.com", "1234", "5551234567");
var yonetici = new KurumYoneticisi(4, "Zeynep", "Ak", "zeynep@hastane.com", "1234", kurum);
var sistem = new SistemYoneticisi(5, "Admin", "Sistem", "admin@hastane.com", "1234");

// Hepsi "Kullanici" tipinde tek bir listede
var kullanicilar = new List<Kullanici> { ali, doktor, hemsire, yonetici, sistem };

void Baslik(string metin) => Console.WriteLine($"\n==== {metin} ====");
void Dene(string ad, Action a)
{
    try { a(); Console.WriteLine($"  OK     {ad}"); }
    catch (Exception e) { Console.WriteLine($"  HATA   {ad}\n         -> {e.Message}"); }
}

// ---------- 1. KALITIM ----------
Baslik("1. KALITIM: kim kimden türüyor?");
static string Zincir(Type t)
{
    var parcalar = new List<string>();
    for (Type? x = t; x != null && x != typeof(object); x = x.BaseType) parcalar.Add(x.Name);
    return string.Join(" -> ", parcalar);
}
foreach (var k in kullanicilar)
    Console.WriteLine($"  {Zincir(k.GetType())}");

// ---------- 2. SOYUTLAMA ----------
Baslik("2. SOYUTLAMA: abstract sınıflar doğrudan oluşturulamaz");
foreach (var t in new[] { typeof(Kullanici), typeof(Calisan), typeof(Hasta), typeof(Doktor) })
    Console.WriteLine($"  {t.Name,-10} abstract mı? {(t.IsAbstract ? "EVET (new yazılamaz)" : "hayır")}");

// ---------- 3. ÇOK BİÇİMLİLİK ----------
Baslik("3. ÇOK BİÇİMLİLİK: aynı çağrı, farklı cevap");
var yetkiler = Enum.GetValues<Yetki>();
Console.WriteLine($"  {"Rol",-18}" + string.Join(" ", yetkiler.Select(y => y.ToString()[..Math.Min(8, y.ToString().Length)].PadRight(8))));
foreach (var k in kullanicilar)   // döngüde "if (rol == ...)" yok; k.YetkiVarMi(...) tek çağrı
    Console.WriteLine($"  {k.RolAdi,-18}" + string.Join(" ", yetkiler.Select(y => (k.YetkiVarMi(y) ? "evet" : "-").PadRight(8))));

// ---------- 4. KAPSÜLLEME ----------
Baslik("4. KAPSÜLLEME: geçersiz veri sınıfın içine giremez");
Dene("Hasta adını boş yap", () => ali.Ad = "");
Dene("E-postayı '@' olmadan yaz", () => ali.Email = "abc");
Dene("Hizmet süresini 0 yap", () => muayene.Sure = TimeSpan.Zero);
Dene("Hizmet ücretini negatif yap", () => muayene.Ucret = -5);
Dene("Doktora başka bölümün hizmetini ekle", () => doktor.HizmetEkle(new Hizmet(99, "Röntgen", TimeSpan.FromMinutes(20), 300m)));
Dene("Doktorun çakışan uygunluk saati ekle", () => doktor.UygunlukEkle(new UygunlukAraligi(2, DayOfWeek.Monday, new TimeOnly(11, 0), new TimeOnly(13, 0))));
Console.WriteLine($"  Şifre doğrulama: doğru şifre -> {ali.SifreDogrula("1234")}, yanlış şifre -> {ali.SifreDogrula("0000")} (şifre hiçbir yerden okunamaz)");

// ---------- 5. RANDEVU AKIŞI ----------
Baslik("5. RANDEVU AKIŞI");
IRandevuServisi servis = new RandevuServisi();
var pzt = DateTime.Today.AddDays(1);
while (pzt.DayOfWeek != DayOfWeek.Monday) pzt = pzt.AddDays(1);

Console.WriteLine($"  Boş saatler ({pzt:dd.MM.yyyy}): " + string.Join(", ", servis.MusaitSlotlariGetir(doktor, muayene, pzt).Select(s => s.ToString("HH:mm"))));
Randevu? r = null;
Dene("Doktor randevu almaya çalışır", () => servis.RandevuOlustur(doktor, doktor, muayene, pzt.AddHours(9)));
Dene("Hasta 09:00'a randevu alır", () => r = servis.RandevuOlustur(ali, doktor, muayene, pzt.AddHours(9)));
Console.WriteLine($"  {r}");
Dene("Başka hasta aynı saate randevu alır", () => servis.RandevuOlustur(new Hasta(7, "Veli", "Can", "v@mail.com", "1234", "555"), doktor, muayene, pzt.AddHours(9)));
Console.WriteLine($"  Boş saatler (09:00 alındı): " + string.Join(", ", servis.MusaitSlotlariGetir(doktor, muayene, pzt).Select(s => s.ToString("HH:mm"))));
Dene("Hasta kendi randevusunu onaylamaya çalışır", () => servis.Onayla(r!.Id, ali));
Dene("Hemşire (başka çalışan) onaylamaya çalışır", () => servis.Onayla(r!.Id, hemsire));
Dene("Randevunun doktoru onaylar", () => servis.Onayla(r!.Id, doktor));
Console.WriteLine($"  Durum: {r!.Durum}");
Dene("Hasta iptal eder", () => servis.IptalEt(r!.Id, ali));
Dene("İptal edilmiş randevuyu tekrar onayla", () => servis.Onayla(r!.Id, doktor));
Console.WriteLine($"  Boş saatler (iptal sonrası): " + string.Join(", ", servis.MusaitSlotlariGetir(doktor, muayene, pzt).Select(s => s.ToString("HH:mm"))));

// ---------- 6. KURUM YÖNETİMİ (yetkiyle tanımlama) ----------
Baslik("6. KURUM YÖNETİMİ: kimin neyi tanımlayabildiği");
var yonetim = new KurumYonetimServisi(new KullaniciServisi());
Kurum? yeniKurum = null;
Dene("Hasta kurum tanımlamaya çalışır", () => yonetim.KurumTanimla(ali, 10, "Özel Klinik"));
Dene("Kurum yöneticisi kurum tanımlamaya çalışır", () => yonetim.KurumTanimla(yonetici, 10, "Özel Klinik"));
Dene("Sistem yöneticisi kurum tanımlar", () => yeniKurum = yonetim.KurumTanimla(sistem, 10, "Özel Klinik"));
Dene("Aynı numaralı ikinci kurum", () => yonetim.KurumTanimla(sistem, 10, "Başka Klinik"));

var klinikYoneticisi = new KurumYoneticisi(20, "Mehmet", "Er", "mehmet@klinik.com", "1234", yeniKurum!);
var digerYonetici = new KurumYoneticisi(21, "Fatma", "Su", "fatma@diger.com", "1234", kurum);   // başka kurumun yöneticisi
var yeniSube = new Sube(5, "Merkez", "Adapazarı");
Dene("Doktor şube eklemeye çalışır", () => yonetim.SubeEkle(doktor, yeniSube));
Dene("Klinik yöneticisi kendi kurumuna şube ekler", () => yonetim.SubeEkle(klinikYoneticisi, yeniSube));
Dene("Klinik yöneticisi şubeye bölüm ekler", () => yonetim.BolumEkle(klinikYoneticisi, 5, new Bolum(7, "Dahiliye")));
Dene("Başka kurumun yöneticisi bu şubeye bölüm ekler", () => yonetim.BolumEkle(digerYonetici, 5, new Bolum(8, "Göz")));
Dene("Klinik yöneticisi hizmet ekler", () => yonetim.HizmetEkle(klinikYoneticisi, 7, new Hizmet(30, "Genel Muayene", TimeSpan.FromMinutes(20), 400m)));
Console.WriteLine($"  Sonuç: {yeniKurum}; şube: {yeniSube}");

// ---------- 7. KAYIT VE GİRİŞ ----------
Baslik("7. KAYIT VE GİRİŞ");
var kullaniciServisi = new KullaniciServisi();
kullaniciServisi.Kaydet(doktor);
kullaniciServisi.Kaydet(yonetici);
Hasta? yeniHasta = null;
Dene("Yeni hasta kayıt olur", () => yeniHasta = kullaniciServisi.HastaKayitOl("Can", "Demir", "can@mail.com", "abc123", "5559998877"));
Console.WriteLine($"  Kayıtlı hasta: {yeniHasta}");
Dene("Aynı e-posta ile ikinci kayıt (büyük harfle)", () => kullaniciServisi.HastaKayitOl("Başka", "Kişi", "CAN@mail.com", "x", "555"));
Dene("Hatalı e-posta ile kayıt", () => kullaniciServisi.HastaKayitOl("Test", "Kişi", "abc", "x", "555"));

Kullanici? girisYapan = null;
Dene("Doğru şifreyle giriş", () => girisYapan = kullaniciServisi.GirisYap("can@mail.com", "abc123"));
Console.WriteLine($"  Giriş yapan: {girisYapan}");
Dene("Yanlış şifreyle giriş", () => kullaniciServisi.GirisYap("can@mail.com", "yanlis"));
Dene("Olmayan e-posta ile giriş", () => kullaniciServisi.GirisYap("yok@mail.com", "abc123"));

Console.WriteLine("  -- Giriş yapan kişi (tipi sadece 'Kullanici') randevu servisine veriliyor --");
Dene("Giriş yapan hasta 10:00'a randevu alır", () => servis.RandevuOlustur(girisYapan!, doktor, muayene, pzt.AddHours(10)));
var doktorGiris = kullaniciServisi.GirisYap("ayse@hastane.com", "1234");
Dene("Giriş yapan doktor randevu almaya çalışır", () => servis.RandevuOlustur(doktorGiris, doktor, muayene, pzt.AddHours(11)));

// ---------- 8. UYGUNLUK, ŞUBE SAATLERİNİN İÇİNDE KALMALI ----------
Baslik("8. ÇALIŞANIN UYGUNLUĞU ŞUBE SAATLERİNİN İÇİNDE KALMALI");
var yonetim2 = new KurumYonetimServisi(new KullaniciServisi());
var kurum8 = yonetim2.KurumTanimla(sistem, 50, "Test Hastanesi");
var yonetici8 = new KurumYoneticisi(50, "Nur", "Taş", "nur@t.com", "1234", kurum8);
yonetim2.SubeEkle(yonetici8, new Sube(50, "Ana Şube", "Sakarya"));
var dahiliye = new Bolum(50, "Dahiliye");
yonetim2.BolumEkle(yonetici8, 50, dahiliye);
yonetim2.CalismaSaatiEkle(yonetici8, 50, new UygunlukAraligi(1, DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(17, 0)));
var doktorA = new Doktor(51, "Sema", "Ok", "sema@t.com", "1234", dahiliye, "Dahiliye uzmanı");
var doktorB = new Doktor(52, "Orhan", "Yıl", "orhan@t.com", "1234", dahiliye, "Dahiliye uzmanı");
yonetim2.CalisanEkle(yonetici8, 50, doktorA);
yonetim2.CalisanEkle(yonetici8, 50, doktorB);
Console.WriteLine("  Şube çalışma saati: Pazartesi 08:00-17:00");

Dene("Pazartesi 09:00-12:00 (şube açık)", () => yonetim2.UygunlukEkle(doktorA, doktorA, new UygunlukAraligi(10, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0))));
Dene("Salı 09:00-12:00 (şube salı kapalı)", () => yonetim2.UygunlukEkle(doktorA, doktorA, new UygunlukAraligi(11, DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0))));
Dene("Pazartesi 07:00-10:00 (şube 08'de açılıyor)", () => yonetim2.UygunlukEkle(doktorA, doktorA, new UygunlukAraligi(12, DayOfWeek.Monday, new TimeOnly(7, 0), new TimeOnly(10, 0))));
Dene("Pazartesi 16:00-18:00 (şube 17'de kapanıyor)", () => yonetim2.UygunlukEkle(doktorA, doktorA, new UygunlukAraligi(13, DayOfWeek.Monday, new TimeOnly(16, 0), new TimeOnly(18, 0))));
Dene("Başka doktor, Sema'nın uygunluğunu düzenler", () => yonetim2.UygunlukEkle(doktorB, doktorA, new UygunlukAraligi(14, DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(15, 0))));
Dene("Yönetici uygunluk düzenlemeye çalışır", () => yonetim2.UygunlukEkle(yonetici8, doktorA, new UygunlukAraligi(15, DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(15, 0))));
Console.WriteLine($"  Sema'nın uygunlukları: {string.Join(", ", doktorA.Uygunluklar)}");
