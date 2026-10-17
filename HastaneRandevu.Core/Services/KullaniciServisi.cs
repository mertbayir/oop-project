using HastaneRandevu.Core.Models;

namespace HastaneRandevu.Core.Services;

// Kayit ve giris. Giris yapan kisi "Kullanici" tipinde doner; hasta mi doktor mu oldugu bilinmez,
// diger servisler onu YetkiVarMi ile anlar (cok bicimlilik).
public class KullaniciServisi
{
    private readonly List<Kullanici> _kullanicilar = new();

    // Hasta kendi kendine kayit olabilir. Numarayi sistem verir.
    public Hasta HastaKayitOl(string ad, string soyad, string email, string sifre, string telefon)
    {
        int id = _kullanicilar.Count == 0 ? 1 : _kullanicilar.Max(k => k.Id) + 1;
        var hasta = new Hasta(id, ad, soyad, email, sifre, telefon);   // Hasta/Kullanici kendi dogrulamasini yapar

        Kaydet(hasta);
        return hasta;
    }

    // Calisan, yonetici gibi onceden olusturulmus kullanicilari sisteme kaydeder (giris yapabilsinler diye).
    public void Kaydet(Kullanici kullanici)
    {
        ArgumentNullException.ThrowIfNull(kullanici);

        if (_kullanicilar.Any(k => string.Equals(k.Email, kullanici.Email, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Bu e-posta adresiyle kayitli bir kullanici zaten var.");
        if (_kullanicilar.Any(k => k.Id == kullanici.Id))
            throw new InvalidOperationException("Bu numarali bir kullanici zaten var.");

        _kullanicilar.Add(kullanici);
    }

    public Kullanici GirisYap(string email, string sifre)
    {
        Kullanici? kullanici = _kullanicilar.FirstOrDefault(
            k => string.Equals(k.Email, email, StringComparison.OrdinalIgnoreCase));

        // E-posta yok ya da sifre yanlis: ikisinde de AYNI mesaj. Hangisinin yanlis oldugu disariya sizdirilmaz.
        if (kullanici is null || !kullanici.SifreDogrula(sifre))
            throw new UnauthorizedAccessException("E-posta veya sifre hatali.");

        return kullanici;
    }
}
