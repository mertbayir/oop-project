namespace Randevu.Core.Models;

// Bir subenin bolumu (Kardiyoloji, Radyoloji, Laboratuvar...). Hizmetler bolume baglidir.
public class Bolum
{
    private string _ad = "";

    // KAPSULLEME: liste private. Disaridan dogrudan Add/Clear yapilamaz.
    private readonly List<Hizmet> _hizmetler = new();

    public int Id { get; }

    public string Ad
    {
        get => _ad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Bolum adi bos olamaz.", nameof(Ad));
            _ad = value.Trim();
        }
    }

    // Disariya sadece okunabilir gorunum acilir (degistirilemez).
    public IReadOnlyList<Hizmet> Hizmetler => _hizmetler.AsReadOnly();

    public Bolum(int id, string ad)
    {
        Id = id;
        Ad = ad;
    }

    // KOMPOZISYON: bolum hizmetlere sahiptir; ekleme kurallari burada kontrol edilir.
    public void HizmetEkle(Hizmet hizmet)
    {
        ArgumentNullException.ThrowIfNull(hizmet);

        if (HizmetVarMi(hizmet))
            throw new InvalidOperationException("Bu hizmet bolumde zaten var.");

        _hizmetler.Add(hizmet);
    }

    public bool HizmetCikar(int hizmetId)
        => _hizmetler.RemoveAll(h => h.Id == hizmetId) > 0;

    // Calisan, sadece kendi bolumundeki hizmetleri sunabilir (karar 4); bu metod o kontrole hizmet eder.
    public bool HizmetVarMi(Hizmet hizmet)
        => _hizmetler.Any(h => h.Id == hizmet.Id);

    public override string ToString() => $"{Ad} ({_hizmetler.Count} hizmet)";
}
