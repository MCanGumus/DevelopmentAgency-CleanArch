# DevelopmentAgency - Clean Architecture

Bu proje, **Clean Architecture** prensiplerini temel alarak hazırlanmış örnek bir yazılım geliştirme çalışmasıdır. Amacım, proje geliştirme sürecinde katmanlı mimariyi doğru şekilde uygulamak ve kodun okunabilirliğini, sürdürülebilirliğini ve test edilebilirliğini artırmaktır.

## Katmanlar

* **Domain**
  Projenin kalbini oluşturan katmandır. Temel iş kuralları, entity tanımları ve domain modelleri burada yer alır. Dış dünyadan bağımsızdır.

* **Application**
  İş mantığının uygulandığı katmandır. Use case'ler, servisler burada bulunur. Domain katmanına bağımlıdır ancak dış dünyaya bağımlı değildir.

* **Persistence**
  Verilerin nasıl saklanacağı ve erişileceğiyle ilgilenir. Repository pattern uygulanarak veri erişim işlemleri soyutlanmıştır. Bu katman, veritabanı veya başka bir depolama sistemiyle etkileşimi yönetir.

* **Core**
  Ortak altyapı, yardımcı fonksiyonlar ve tüm katmanların ihtiyaç duyabileceği temel bileşenleri içerir.

* **Source (UI/API)**
  Uygulamanın dış dünya ile iletişim kurduğu katmandır. API ya da kullanıcı arayüzü bu bölümde yer alır.

## Neden Clean Architecture?

* Katmanların birbirinden bağımsız olması sayesinde kolayca geliştirilebilir ve test edilebilir.
* Yeni teknolojiler veya framework’ler eklenmek istendiğinde sadece ilgili katman değiştirilir.
* Kodun anlaşılır, düzenli ve sürdürülebilir olmasını sağlar.

## Özet

Bu proje, **katmanlı mimariyi** ve **Clean Architecture yaklaşımını** örneklemek amacıyla geliştirilmiştir. Yapıyı inceleyen herkes, modern yazılım geliştirme pratikleriyle düzenlenmiş bir iskelet proje olduğunu kolayca görebilir.
