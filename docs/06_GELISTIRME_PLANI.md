# Geliştirme Planı - Adım Adım İlerleme

Bu dosya projeyi kaç adımda ve ne sırayla geliştireceğimizi planlar.
Her adım tamamlanmadan bir sonrakine geçilmeyecektir.

---

## FELSEFE
- Her fonksiyon tek tek yazılır ve anlaşılır
- Kod yazılmadan önce ne yapacağı bu dokümanlara not alınır
- AI sadece yazım kolaylığı sağlar; mantık ve karar Mert'e aittir
- Her commit anlamlı ve açıklayıcı olacak (farklı günlerde, min 10 adet)

---

## AŞAMA 0: Hazırlık (Şimdiki Aşama)
- [x] Ödev analizi yapıldı
- [x] Docs klasörü oluşturuldu
- [x] NDP konseptleri belgelendi
- [x] Mimari tasarım taslağı oluşturuldu
- [x] Algoritmalar pseudo-kod olarak yazıldı
- [ ] Tech stack kararı verilecek (Mert ile konuşulacak)
- [ ] GitHub reposu oluşturulacak

---

## AŞAMA 1: Temel Sınıflar
Sırayla yazılacak:
1. Kullanici (User) sınıfı - temel alanlar ve metodlar
2. Hasta (Patient) sınıfı - Kullanici'dan kalıtım
3. Doktor (Doctor) sınıfı - Kullanici'dan kalıtım
4. Yonetici (Manager) sınıfı - Kullanici'dan kalıtım
5. HizmetTuru (ServiceType) sınıfı
6. CalismaSaati (WorkHour) sınıfı

---

## AŞAMA 2: Kurum Yapısı
7. Klinik (Clinic) sınıfı
8. Hastane (Hospital) sınıfı - Klinik listesi içerir

---

## AŞAMA 3: Randevu Mantığı (EN KRİTİK)
9. Randevu (Appointment) sınıfı
10. Çakışma kontrol algoritması implementasyonu
11. Müsait slot hesaplama algoritması
12. Randevu oluşturma iş akışı

---

## AŞAMA 4: Kullanıcı Arayüzü (TBD - tech stack kararına göre)
13. Giriş/Kayıt ekranı
14. Hasta paneli
15. Doktor paneli
16. Yönetici paneli

---

## AŞAMA 5: Kalite ve Test
17. Temel senaryoların testi
18. Edge case'lerin kontrolü (çakışma senaryoları)
19. Kod temizliği ve yorumlar

---

## GitHub Commit Planı (Min 10, Farklı Günlerde)
1. "Proje iskelet yapisi olusturuldu" (dokumantasyon)
2. "Kullanici (User) sinifi eklendi"
3. "Hasta ve Doktor sinifları eklendi - Kalitim"
4. "Yonetici sinifi eklendi"
5. "Kurum sinifları (Klinik, Hastane) eklendi"
6. "HizmetTuru ve CalismaSaati sinifları eklendi"
7. "Randevu sinifi ve temel mantigi eklendi"
8. "Cakisma kontrol algoritmasi implementasyonu"
9. "Musait slot hesaplama algoritmasi"
10. "Kullanici arayuzu - Temel ekranlar"
11. "Kullanici arayuzu - Panel tasarimlari"
12. "Test ve hata duzeltmeleri"
