namespace Randevu.Core.Models;

// Hastane veya klinik. Bir veya daha fazla subesi olur (karar 3).
public class Kurum
{
    private string _ad = "";
    private readonly List<Sube> _subeler = new();

    public int Id { get; }

    public string Ad
    {
        get => _ad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Kurum adi bos olamaz.", nameof(Ad));
            _ad = value.Trim();
        }
    }

    public IReadOnlyList<Sube> Subeler => _subeler.AsReadOnly();

    public Kurum(int id, string ad)
    {
        Id = id;
        Ad = ad;
    }

    public void SubeEkle(Sube sube)
    {
        ArgumentNullException.ThrowIfNull(sube);

        if (_subeler.Any(s => s.Id == sube.Id))
            throw new InvalidOperationException("Bu sube kurumda zaten var.");

        _subeler.Add(sube);
    }

    public override string ToString() => $"{Ad} ({_subeler.Count} sube)";
}
