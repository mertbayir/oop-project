namespace HastaneRandevu.Core.Models;

// Bir kurumun subesi (karar 3). Tek klinik = tek subeli kurum.
// Odev: "her klinik/hastanenin calisma saatleri ve bolumleri tanimlanabilecek".
public class Sube
{
    private string _ad = "";
    private string _adres = "";

    // KAPSULLEME: listeler private, degisiklik sadece dogrulamali metodlarla.
    private readonly List<Bolum> _bolumler = new();
    private readonly List<Calisan> _calisanlar = new();
    private readonly List<UygunlukAraligi> _calismaSaatleri = new();

    public int Id { get; }

    public string Ad
    {
        get => _ad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Sube adi bos olamaz.", nameof(Ad));
            _ad = value.Trim();
        }
    }

    public string Adres
    {
        get => _adres;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Adres bos olamaz.", nameof(Adres));
            _adres = value.Trim();
        }
    }

    public IReadOnlyList<Bolum> Bolumler => _bolumler.AsReadOnly();
    public IReadOnlyList<Calisan> Calisanlar => _calisanlar.AsReadOnly();
    public IReadOnlyList<UygunlukAraligi> CalismaSaatleri => _calismaSaatleri.AsReadOnly();

    public Sube(int id, string ad, string adres)
    {
        Id = id;
        Ad = ad;
        Adres = adres;
    }

    public void BolumEkle(Bolum bolum)
    {
        ArgumentNullException.ThrowIfNull(bolum);

        if (_bolumler.Any(b => b.Id == bolum.Id))
            throw new InvalidOperationException("Bu bolum subede zaten var.");

        _bolumler.Add(bolum);
    }

    // Calisanin bolumu bu subede tanimli olmali; aksi halde baska subenin bolumune ait biri eklenmis olur.
    public void CalisanEkle(Calisan calisan)
    {
        ArgumentNullException.ThrowIfNull(calisan);

        if (!_bolumler.Any(b => b.Id == calisan.Bolum.Id))
            throw new InvalidOperationException("Calisanin bolumu bu subede tanimli degil.");
        if (_calisanlar.Any(c => c.Id == calisan.Id))
            throw new InvalidOperationException("Bu calisan subede zaten var.");

        _calisanlar.Add(calisan);
    }

    public bool CalisanCikar(int calisanId)
        => _calisanlar.RemoveAll(c => c.Id == calisanId) > 0;

    // UygunlukAraligi sinifini yeniden kullaniyoruz: "haftanin gunu + saat araligi" ayni kavram.
    public void CalismaSaatiEkle(UygunlukAraligi aralik)
    {
        ArgumentNullException.ThrowIfNull(aralik);

        if (_calismaSaatleri.Any(s => s.Cakisiyor(aralik)))
            throw new InvalidOperationException("Bu aralik mevcut bir calisma saatiyle cakisiyor.");

        _calismaSaatleri.Add(aralik);
    }

    public bool AcikMi(DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
        => _calismaSaatleri.Any(s => s.Kapsiyor(gun, baslangic, bitis));

    public override string ToString() => $"{Ad} - {Adres} ({_bolumler.Count} bolum, {_calisanlar.Count} calisan)";
}
