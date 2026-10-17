using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Odev: "hastaneler/klinikler yetki cercevesinde tanimlanabilecek", "doktor ve saglik personeli
// sisteme kayit edilebilecek", "calisma saatleri, bolumler ve hizmetler tanimlanabilecek".
// Her metod ayni iki kapidan gecer: 1) rolun yetkisi var mi? 2) bu kurumun yoneticisi mi?
public class KurumYonetimServisi
{
    private readonly List<Kurum> _kurumlar = new();

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
