using System.Security.Cryptography;
using System.Text;
using HastaneRandevu.Core.Enums;

namespace HastaneRandevu.Core.Models;

// SOYUTLAMA: abstract oldugu icin "new Kullanici()" yazilamaz.
// Her kullanici ya Hasta, ya Calisan, ya Yonetici olmak zorunda.
public abstract class Kullanici
{
    // KAPSULLEME: alanlar private, dis dunya sadece property uzerinden erisir.
    private string _ad = "";
    private string _soyad = "";
    private string _email = "";
    private string _sifreHash = "";

    public int Id { get; }

    public string Ad
    {
        get => _ad;
        set => _ad = BosOlamaz(value, nameof(Ad));
    }

    public string Soyad
    {
        get => _soyad;
        set => _soyad = BosOlamaz(value, nameof(Soyad));
    }

    public string Email
    {
        get => _email;
        set
        {
            string temiz = BosOlamaz(value, nameof(Email));
            if (!temiz.Contains('@'))
                throw new ArgumentException("Gecerli bir e-posta giriniz.", nameof(Email));
            _email = temiz;
        }
    }

    public string TamAd => $"{Ad} {Soyad}";

    protected Kullanici(int id, string ad, string soyad, string email, string sifre)
    {
        Id = id;
        Ad = ad;
        Soyad = soyad;
        Email = email;
        _sifreHash = Hashle(sifre);
    }

    // COK BICIMLILIK: alt siniflar bu uyeleri kendi davranislariyla doldurur (override).
    // Sistem kullanicinin Hasta mi Doktor mu oldugunu bilmeden bunlari cagirir.
    public abstract string RolAdi { get; }
    public abstract bool YetkiVarMi(Yetki yetki);

    // KAPSULLEME: sifre hash'i disariya acilmaz, sadece dogrulama sonucu doner.
    public bool SifreDogrula(string girilen) => Hashle(girilen) == _sifreHash;

    public bool SifreDegistir(string eskiSifre, string yeniSifre)
    {
        if (!SifreDogrula(eskiSifre)) return false;
        _sifreHash = Hashle(yeniSifre);
        return true;
    }

    private static string BosOlamaz(string deger, string alanAdi)
    {
        if (string.IsNullOrWhiteSpace(deger))
            throw new ArgumentException($"{alanAdi} bos olamaz.", alanAdi);
        return deger.Trim();
    }

    private static string Hashle(string sifre)
    {
        if (string.IsNullOrEmpty(sifre))
            throw new ArgumentException("Sifre bos olamaz.", nameof(sifre));
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(sifre));
        return Convert.ToHexString(hash);
    }

    public override string ToString() => $"[{RolAdi}] {TamAd} <{Email}>";
}
