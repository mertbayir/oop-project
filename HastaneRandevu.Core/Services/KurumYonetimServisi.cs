using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Odev: "hastaneler/klinikler yetki cercevesinde tanimlanabilecek", "doktor ve saglik personeli
// sisteme kayit edilebilecek", "calisma saatleri, bolumler ve hizmetler tanimlanabilecek".
// Her metod ayni iki kapidan gecer: 1) rolun yetkisi var mi? 2) bu kurumun yoneticisi mi?
public class KurumYonetimServisi
{
    private readonly List<Kurum> _kurumlar = new();

    // Calisan kaydi hem subeye hem giris listesine yazilir; bu yuzden KullaniciServisi'ni taniyoruz.
    private readonly KullaniciServisi _kullanicilar;

    public KurumYonetimServisi(KullaniciServisi kullaniciServisi)
    {
        ArgumentNullException.ThrowIfNull(kullaniciServisi);
        _kullanicilar = kullaniciServisi;
    }

    public IReadOnlyList<Kurum> Kurumlar => _kurumlar.AsReadOnly();

    // Sadece Sistem Yoneticisi (Yetki.KurumTanimla) yeni kurum tanimlayabilir.
    public Kurum KurumTanimla(Kullanici islemYapan, int id, string ad)
    {
        Yetkilendirme.Kontrol(islemYapan, Yetki.KurumTanimla);

        if (_kurumlar.Any(k => k.Id == id))
            throw new InvalidOperationException("Bu numarali bir kurum zaten var.");

        var kurum = new Kurum(id, ad);
        _kurumlar.Add(kurum);
        return kurum;
    }

    // Ayni isim, farkli parametre (OVERLOAD): numarayi servis uretir, ekran sadece adi verir.
    public Kurum KurumTanimla(Kullanici islemYapan, string ad)
        => KurumTanimla(islemYapan, SonrakiId(_kurumlar.Select(k => k.Id)), ad);

    // Sadece Sistem Yoneticisi (Yetki.KurumTanimla) bir kuruma yonetici atayabilir.
    // Yonetici hem olusturulur hem giris listesine yazilir; e-posta onceden kontrol edilir.
    public KurumYoneticisi KurumYoneticisiAta(Kullanici islemYapan, int kurumId, string ad, string soyad,
                                              string email, string sifre)
    {
        Yetkilendirme.Kontrol(islemYapan, Yetki.KurumTanimla);

        Kurum kurum = _kurumlar.FirstOrDefault(k => k.Id == kurumId)
            ?? throw new InvalidOperationException("Kurum bulunamadi.");

        if (_kullanicilar.EpostaKayitliMi(email))
            throw new InvalidOperationException("Bu e-posta adresiyle kayitli bir kullanici zaten var.");

        var yonetici = new KurumYoneticisi(_kullanicilar.SonrakiId(), ad, soyad, email, sifre, kurum);
        _kullanicilar.Kaydet(yonetici);
        return yonetici;
    }

    public void SubeEkle(Kullanici islemYapan, Sube sube)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.KurumDuzenle);
        yonetici.Kurum.SubeEkle(sube);
    }

    public void BolumEkle(Kullanici islemYapan, int subeId, Bolum bolum)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.KurumDuzenle);
        SubeBul(yonetici, subeId).BolumEkle(bolum);
    }

    public void CalismaSaatiEkle(Kullanici islemYapan, int subeId, UygunlukAraligi aralik)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.KurumDuzenle);
        SubeBul(yonetici, subeId).CalismaSaatiEkle(aralik);
    }

    public void HizmetEkle(Kullanici islemYapan, int bolumId, Hizmet hizmet)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.HizmetYonet);

        Bolum bolum = yonetici.Kurum.Subeler
            .SelectMany(s => s.Bolumler)
            .FirstOrDefault(b => b.Id == bolumId)
            ?? throw new InvalidOperationException("Bu bolum kurumunuzda bulunamadi.");

        bolum.HizmetEkle(hizmet);
    }

    public void CalisanEkle(Kullanici islemYapan, int subeId, Calisan calisan)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.CalisanYonet);
        SubeBul(yonetici, subeId).CalisanEkle(calisan);
    }

    // ---- Numarayi servisin kendisinin verdigi kolay yollar (ekranlar numara uretmek zorunda kalmasin) ----

    public Sube SubeOlustur(Kullanici islemYapan, string ad, string adres)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.KurumDuzenle);

        int id = SonrakiId(yonetici.Kurum.Subeler.Select(s => s.Id));
        var sube = new Sube(id, ad, adres);
        yonetici.Kurum.SubeEkle(sube);
        return sube;
    }

    public Bolum BolumOlustur(Kullanici islemYapan, int subeId, string ad)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.KurumDuzenle);
        Sube sube = SubeBul(yonetici, subeId);

        // Bolum numarasi kurumdaki TUM bolumler arasinda benzersiz olur (HizmetEkle bolumu numarayla arar).
        int id = SonrakiId(yonetici.Kurum.Subeler.SelectMany(s => s.Bolumler).Select(b => b.Id));
        var bolum = new Bolum(id, ad);
        sube.BolumEkle(bolum);
        return bolum;
    }

    public Hizmet HizmetOlustur(Kullanici islemYapan, int bolumId, string ad, TimeSpan sure, decimal ucret)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.HizmetYonet);

        int id = SonrakiId(yonetici.Kurum.Subeler
            .SelectMany(s => s.Bolumler).SelectMany(b => b.Hizmetler).Select(h => h.Id));
        var hizmet = new Hizmet(id, ad, sure, ucret);
        HizmetEkle(islemYapan, bolumId, hizmet);   // bolumu bulur ve ekler (yetkiyi tekrar kontrol eder)
        return hizmet;
    }

    // Mevcut numaralarin en buyugunun bir fazlasi (hic yoksa 1).
    private static int SonrakiId(IEnumerable<int> mevcutNumaralar)
        => mevcutNumaralar.DefaultIfEmpty(0).Max() + 1;

    // ---- Calisani tek adimda kaydet: sube + bolum + hizmetler + giris hesabi ----

    public Doktor DoktorEkle(Kullanici islemYapan, int subeId, int bolumId, string ad, string soyad,
                             string email, string sifre, string uzmanlikAlani, IEnumerable<int> hizmetNumaralari)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.CalisanYonet);
        Bolum bolum = BolumBul(yonetici, subeId, bolumId);

        var doktor = new Doktor(_kullanicilar.SonrakiId(), ad, soyad, email, sifre, bolum, uzmanlikAlani);
        CalisanYerlestir(yonetici, subeId, doktor, hizmetNumaralari);
        return doktor;
    }

    public SaglikPersoneli PersonelEkle(Kullanici islemYapan, int subeId, int bolumId, string ad, string soyad,
                                        string email, string sifre, string unvan, IEnumerable<int> hizmetNumaralari)
    {
        var yonetici = YoneticiKontrol(islemYapan, Yetki.CalisanYonet);
        Bolum bolum = BolumBul(yonetici, subeId, bolumId);

        var personel = new SaglikPersoneli(_kullanicilar.SonrakiId(), ad, soyad, email, sifre, bolum, unvan);
        CalisanYerlestir(yonetici, subeId, personel, hizmetNumaralari);
        return personel;
    }

    // Ikisi icin ortak is. Parametre "Calisan" tipinde, yani Doktor da SaglikPersoneli de kabul edilir (kalitim).
    // Once kontroller, kalici degisiklikler (subeye ekleme, giris hesabi) EN SONA: biri patlarsa yari kalmis kayit olmaz.
    private void CalisanYerlestir(KurumYoneticisi yonetici, int subeId, Calisan calisan, IEnumerable<int> hizmetNumaralari)
    {
        if (_kullanicilar.EpostaKayitliMi(calisan.Email))
            throw new InvalidOperationException("Bu e-posta adresiyle kayitli bir kullanici zaten var.");

        foreach (int hizmetNo in hizmetNumaralari)
        {
            Hizmet hizmet = calisan.Bolum.Hizmetler.FirstOrDefault(h => h.Id == hizmetNo)
                ?? throw new InvalidOperationException("Secilen hizmet bu bolume ait degil.");
            calisan.HizmetEkle(hizmet);
        }

        SubeBul(yonetici, subeId).CalisanEkle(calisan);
        _kullanicilar.Kaydet(calisan);
    }

    private static Bolum BolumBul(KurumYoneticisi yonetici, int subeId, int bolumId)
        => SubeBul(yonetici, subeId).Bolumler.FirstOrDefault(b => b.Id == bolumId)
           ?? throw new InvalidOperationException("Bu bolum secilen subede bulunamadi.");

    // Calisan KENDI uygunluk saatini ekler; saat, calistigi subenin acik oldugu saatlerin icinde kalmali.
    public void UygunlukEkle(Kullanici islemYapan, Calisan calisan, UygunlukAraligi aralik)
    {
        Yetkilendirme.Kontrol(islemYapan, Yetki.UygunlukDuzenle);
        ArgumentNullException.ThrowIfNull(calisan);
        ArgumentNullException.ThrowIfNull(aralik);

        if (islemYapan.Id != calisan.Id)
            throw new UnauthorizedAccessException("Sadece kendi uygunluk saatlerinizi duzenleyebilirsiniz.");

        Sube sube = _kurumlar.SelectMany(k => k.Subeler)
            .FirstOrDefault(s => s.Calisanlar.Any(c => c.Id == calisan.Id))
            ?? throw new InvalidOperationException("Calisan hicbir subeye kayitli degil.");

        if (!sube.AcikMi(aralik.Gun, aralik.Baslangic, aralik.Bitis))
            throw new InvalidOperationException("Uygunluk saati subenin calisma saatleri disinda.");

        calisan.UygunlukEkle(aralik);
    }

    // Kapi 1: rolun bu yetkisi var mi?  Kapi 2: islemi yapan gercekten bir KurumYoneticisi mi?
    // (Kurum yoneticisi kendi kurumuna bagli oldugu icin, sadece kendi kurumunu degistirebilir.)
    private static KurumYoneticisi YoneticiKontrol(Kullanici islemYapan, Yetki yetki)
    {
        Yetkilendirme.Kontrol(islemYapan, yetki);

        return islemYapan as KurumYoneticisi
            ?? throw new UnauthorizedAccessException("Bu islemi sadece kurum yoneticisi yapabilir.");
    }

    // Subeyi sadece yoneticinin kendi kurumunun subeleri arasinda arar.
    private static Sube SubeBul(KurumYoneticisi yonetici, int subeId)
        => yonetici.Kurum.Subeler.FirstOrDefault(s => s.Id == subeId)
           ?? throw new InvalidOperationException("Bu sube kurumunuzda bulunamadi.");
}
