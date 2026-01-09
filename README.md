# Triago (Email Triage Agent) ✉️🛡️

Triago je inteligentni asistent za inbox koji ocjenjuje dolazne emailove, usmjerava ih u odgovarajuće stanje i uči iz ljudskih recenzija. Poruke koje nisu sigurne idu na pregled, a povratne informacije se koriste za retreniranje agenta kroz vrijeme.

## Šta radi ✅
- Ocjenjuje rizik od spama za svaku poruku
- Usmjerava poruke u Allow, Blocked ili Pending Review
- Prikuplja ljudske oznake radi poboljšanja budućih odluka
- Radi u kratkim agentnim tick-ovima (Sense → Think → Act → Learn)
- Daje GPT objašnjenja zašto je agent donio odluku
- Podržava brisanje poruka iz detaljnog prikaza

## Struktura projekta 🧭
- `src/AiAgents.Core` – generičke apstrakcije agenata
- `src/EmailTriageAgent` – domain + application + infrastructure logika
- `src/EmailTriageAgent.Web` – API host + background workeri
- `webapp` – Triago UI

## Preduslovi 🧰
- .NET 8 SDK
- Node.js 18+
- SQL Server (LocalDB je dovoljan)

## Podešavanje baze 🗄️
Podesi connection string u:
- `src/EmailTriageAgent.Web/appsettings.json`

Primjer za LocalDB:
```
Server=(localdb)\MSSQLLocalDB;Database=EmailTriageAgent;Trusted_Connection=True;TrustServerCertificate=True
```

## Pokretanje API-ja 🚀
1. Kreiraj migracije:
```
Add-Migration InitialCreate -Project EmailTriageAgent -StartupProject EmailTriageAgent.Web
Update-Database -Project EmailTriageAgent -StartupProject EmailTriageAgent.Web
```
2. Pokreni API:
```
dotnet run --project src/EmailTriageAgent.Web
```

Swagger će biti dostupan na:
```
https://localhost:<port>/swagger
```

## Pokretanje UI-a 🎨
```
cd webapp
npm install
npm run dev
```

## Osnovna upotreba 🧪
- Otvori UI i idi na Compose da pošalješ novu poruku agentu.
- Pregledaj Inbox / Pending Review / Blocked / Completed tabove.
- Označi Pending Review poruke kao Spam ili Not Spam.
- Kada se skupi dovoljno review-a, retrain agent se pokreće automatski.
- Otvori poruku i klikni “Ask GPT Why?” da vidiš objašnjenje.
- Koristi “Delete email” u detaljnom prikazu da ukloniš poruku.

