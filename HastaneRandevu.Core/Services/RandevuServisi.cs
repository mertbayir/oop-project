using HastaneRandevu.Core.Enums;
using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Tum randevulari bilen katman: tek bir randevunun bilemeyecegi kurallari (cakisma, gecmis tarih,
// bos slotlar) burada kontrol eder. Ayrica islemi yapanin YETKISI olup olmadigini kontrol eder.
public class RandevuServisi : IRandevuServisi
{
    // KAPSULLEME: randevu listesi private, disaridan sadece bu sinifin metodlariyla degisir.
    private readonly List<Randevu> _randevular = new();
    private int _sonId;

    public Randevu RandevuOlustur(Kullanici islemYapan, Calisan calisan, Hizmet hizmet, DateTime baslangic)
    {
        // COK BICIMLILIK: islemYapan Hasta mi Doktor mu bilmiyoruz; kendi YetkiVarMi'si cevap verir.
        YetkiKontrol(islemYapan, Yetki.RandevuAl);
        Hasta hasta = HastaOlmali(islemYapan);

        if (baslangic <= DateTime.Now)
            throw new InvalidOperationException("Gecmis bir zamana randevu alinamaz.");

        // Randevunun kendi kurallari (hizmet, uygunluk) constructor'da kontrol edilir.
        var yeni = new Randevu(_sonId + 1, hasta, calisan, hizmet, baslangic);

        // Calisanin mevcut randevulariyla cakisiyor mu?
        if (_randevular.Any(r => r.Cakisiyor(yeni)))
            throw new InvalidOperationException("Bu saatte calisanin baska bir randevusu var.");

        _sonId++;
        _randevular.Add(yeni);
        return yeni;
    }

    // MUSAIT SLOT ALGORITMASI: calisanin o gunku uygunluk araligini hizmet suresi adimlariyla tara;
    // gecmiste kalmayan ve baska bir randevuyla ortusmeyen baslangiclari dondur.
    public IReadOnlyList<DateTime> MusaitSlotlariGetir(Calisan calisan, Hizmet hizmet, DateTime tarih)
    {
        ArgumentNullException.ThrowIfNull(calisan);
        ArgumentNullException.ThrowIfNull(hizmet);

        if (!calisan.HizmetSunuyorMu(hizmet))
            throw new InvalidOperationException("Calisan bu hizmeti sunmuyor.");

        var mevcut = _randevular.Where(r => r.Calisan.Id == calisan.Id && r.SlotuTutuyor).ToList();
        var slotlar = new List<DateTime>();

        foreach (var aralik in calisan.Uygunluklar.Where(u => u.Gun == tarih.DayOfWeek).OrderBy(u => u.Baslangic))
        {
            DateTime slot = tarih.Date + aralik.Baslangic.ToTimeSpan();
            DateTime aralikBitis = tarih.Date + aralik.Bitis.ToTimeSpan();

            while (slot + hizmet.Sure <= aralikBitis)
            {
                DateTime slotBitis = slot + hizmet.Sure;
                bool bos = !mevcut.Any(r => r.ZamanOrtusuyor(slot, slotBitis));

                if (bos && slot > DateTime.Now)
                    slotlar.Add(slot);

                slot = slotBitis;
            }
        }

        return slotlar;
    }

    public void Onayla(int randevuId, Kullanici islemYapan)
    {
        YetkiKontrol(islemYapan, Yetki.RandevuOnayla);
        Bul(randevuId).Onayla(CalisanOlmali(islemYapan));
    }

    public void Reddet(int randevuId, Kullanici islemYapan)
    {
        YetkiKontrol(islemYapan, Yetki.RandevuOnayla);
        Bul(randevuId).Reddet(CalisanOlmali(islemYapan));
    }

    public void Tamamla(int randevuId, Kullanici islemYapan)
    {
        YetkiKontrol(islemYapan, Yetki.RandevuOnayla);
        Bul(randevuId).Tamamla(CalisanOlmali(islemYapan));
    }

    public void IptalEt(int randevuId, Kullanici islemYapan)
    {
        YetkiKontrol(islemYapan, Yetki.RandevuIptalEt);
        Bul(randevuId).IptalEt(HastaOlmali(islemYapan));
    }

    public IReadOnlyList<Randevu> CalisaninRandevulari(Calisan calisan)
        => _randevular.Where(r => r.Calisan.Id == calisan.Id).ToList();

    public IReadOnlyList<Randevu> HastaninRandevulari(Hasta hasta)
        => _randevular.Where(r => r.Hasta.Id == hasta.Id).ToList();

    private Randevu Bul(int randevuId)
        => _randevular.FirstOrDefault(r => r.Id == randevuId)
           ?? throw new InvalidOperationException("Randevu bulunamadi.");

    // Rol ne olursa olsun ayni soru sorulur; cevabi kullanicinin kendi sinifi verir.
    private static void YetkiKontrol(Kullanici kullanici, Yetki yetki)
    {
        ArgumentNullException.ThrowIfNull(kullanici);

        if (!kullanici.YetkiVarMi(yetki))
            throw new UnauthorizedAccessException($"{kullanici.RolAdi} rolunun bu islem icin yetkisi yok.");
    }

    // Yetki kontrolunden gectikten sonra Randevu'nun metodlari Hasta/Calisan tipi ister; tipe cevirir.
    private static Hasta HastaOlmali(Kullanici kullanici)
        => kullanici as Hasta ?? throw new UnauthorizedAccessException("Bu islemi sadece hasta yapabilir.");

    private static Calisan CalisanOlmali(Kullanici kullanici)
        => kullanici as Calisan ?? throw new UnauthorizedAccessException("Bu islemi sadece calisan yapabilir.");
}
