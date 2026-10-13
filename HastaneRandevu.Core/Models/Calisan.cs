using HastaneRandevu.Core.Enums;

namespace HastaneRandevu.Core.Models;

// Doktor ve SaglikPersoneli'nin ortak ust sinifi (karar 1). abstract: "sadece calisan" olmaz.
// Randevu bu sinifa baglanir (karar 2), yani hizmeti kim verirse versin Randevu icin fark etmez.
public abstract class Calisan : Kullanici
{
    // KOMPOZISYON/LISTE KAPSULLEME: listeler private, degisiklik sadece dogrulamali metodlarla.
    private readonly List<Hizmet> _sunulanHizmetler = new();
    private readonly List<UygunlukAraligi> _uygunluklar = new();

    public Bolum Bolum { get; }

    public IReadOnlyList<Hizmet> SunulanHizmetler => _sunulanHizmetler.AsReadOnly();
    public IReadOnlyList<UygunlukAraligi> Uygunluklar => _uygunluklar.AsReadOnly();

    protected Calisan(int id, string ad, string soyad, string email, string sifre, Bolum bolum)
        : base(id, ad, soyad, email, sifre)
    {
        ArgumentNullException.ThrowIfNull(bolum);
        Bolum = bolum;
    }

    // Calisan'a ozel yetkiler burada; Doktor ve SaglikPersoneli ortak miras alir.
    // RolAdi hala abstract kaldi: onu alt siniflar doldurur.
    public override bool YetkiVarMi(Yetki yetki)
        => yetki == Yetki.RandevuOnayla || yetki == Yetki.UygunlukDuzenle;

    // Karar 4: calisan sadece kendi bolumundeki hizmetleri sunabilir.
    public void HizmetEkle(Hizmet hizmet)
    {
        ArgumentNullException.ThrowIfNull(hizmet);

        if (!Bolum.HizmetVarMi(hizmet))
            throw new InvalidOperationException("Bu hizmet calisanin bolumune ait degil.");
        if (HizmetSunuyorMu(hizmet))
            throw new InvalidOperationException("Bu hizmet zaten ekli.");

        _sunulanHizmetler.Add(hizmet);
    }

    public bool HizmetCikar(int hizmetId)
        => _sunulanHizmetler.RemoveAll(h => h.Id == hizmetId) > 0;

    public bool HizmetSunuyorMu(Hizmet hizmet)
        => _sunulanHizmetler.Any(h => h.Id == hizmet.Id);

    // Calisanin kendi uygunluklari birbiriyle cakisamaz.
    public void UygunlukEkle(UygunlukAraligi aralik)
    {
        ArgumentNullException.ThrowIfNull(aralik);

        if (_uygunluklar.Any(u => u.Cakisiyor(aralik)))
            throw new InvalidOperationException("Bu aralik mevcut bir uygunlukla cakisiyor.");

        _uygunluklar.Add(aralik);
    }

    public bool UygunlukSil(int aralikId)
        => _uygunluklar.RemoveAll(u => u.Id == aralikId) > 0;

    // "Hasta sadece uygunluk araliklarina randevu alabilir" kurali: bu metod ona cevap verir.
    public bool UygunMu(DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
        => _uygunluklar.Any(u => u.Kapsiyor(gun, baslangic, bitis));
}
